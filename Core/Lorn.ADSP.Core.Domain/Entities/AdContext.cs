using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// 广告上下文实体
/// 广告引擎根据AdRequest和其他信息组成的完整请求上下文，按广告位组织候选广告集合
/// 生命周期：请求创建→策略使用→销毁，在整个请求处理过程中保持不变
/// </summary>
public class AdContext : EntityBase
{
    /// <summary>
    /// 请求唯一标识
    /// </summary>
    public string RequestId { get; private set; }

    /// <summary>
    /// 用户标识
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime RequestTime { get; private set; }

    /// <summary>
    /// 按广告位分组的候选广告集合
    /// Key为广告位ID，Value为AdCandidate数组
    /// </summary>
    private readonly Dictionary<string, List<AdCandidate>> _candidatesByPlacement = new();
    public IReadOnlyDictionary<string, IReadOnlyList<AdCandidate>> CandidatesByPlacement =>
        _candidatesByPlacement.ToDictionary(
            kv => kv.Key,
            kv => (IReadOnlyList<AdCandidate>)kv.Value.AsReadOnly()
        ).AsReadOnly();

    /// <summary>
    /// 定向上下文
    /// </summary>
    public TargetingContext TargetingContext { get; private set; }

    /// <summary>
    /// 环境信息
    /// 包含时间、竞争情况等环境数据
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentInfo { get; private set; }


    /// <summary>
    /// 用户画像信息（从TargetingContext获取的快捷访问）
    /// </summary>
    public UserProfile? UserProfile => TargetingContext?.GetTargetingContext<UserProfile>("user") ?? TargetingContext?.GetTargetingContext<UserProfile>();

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private AdContext(
        string requestId,
        string? userId,
        DateTime requestTime,
        TargetingContext targetingContext,
        IDictionary<string, object>? environmentInfo = null)
    {
        RequestId = requestId;
        UserId = userId;
        RequestTime = requestTime;
        TargetingContext = targetingContext;
        EnvironmentInfo = environmentInfo?.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly() ??
                          new Dictionary<string, object>().AsReadOnly();
    }

    /// <summary>
    /// 创建广告上下文对象
    /// </summary>
    public static AdContext Create(
        string requestId,
        string? userId,
        DateTime requestTime,
        TargetingContext targetingContext,
        IDictionary<string, object>? environmentInfo = null)
    {
        ValidateParameters(requestId, targetingContext);

        return new AdContext(
            requestId,
            userId,
            requestTime,
            targetingContext,
            environmentInfo);
    }



    /// <summary>
    /// 获取指定广告位的候选广告列表
    /// </summary>
    public IReadOnlyList<AdCandidate> GetCandidatesForPlacement(string placementId)
    {
        if (string.IsNullOrEmpty(placementId))
            return new List<AdCandidate>().AsReadOnly();

        return _candidatesByPlacement.TryGetValue(placementId, out var candidates)
            ? candidates.AsReadOnly()
            : new List<AdCandidate>().AsReadOnly();
    }

    /// <summary>
    /// 添加候选广告到指定广告位
    /// </summary>
    public void AddCandidateToPlacement(string placementId, AdCandidate candidate)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (candidate == null)
            throw new ArgumentNullException(nameof(candidate));

        // 确保候选广告的PlacementId与目标广告位一致
        if (candidate.PlacementId != placementId)
        {
            candidate.AssignToPlacement(placementId);
        }

        if (!_candidatesByPlacement.ContainsKey(placementId))
        {
            _candidatesByPlacement[placementId] = new List<AdCandidate>();
        }

        _candidatesByPlacement[placementId].Add(candidate);
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 批量添加候选广告到指定广告位
    /// </summary>
    public void AddCandidatesToPlacement(string placementId, IEnumerable<AdCandidate> candidates)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (candidates == null)
            throw new ArgumentNullException(nameof(candidates));

        foreach (var candidate in candidates)
        {
            AddCandidateToPlacement(placementId, candidate);
        }
    }

    /// <summary>
    /// 移除指定广告位的候选广告
    /// </summary>
    public bool RemoveCandidateFromPlacement(string placementId, AdCandidate candidate)
    {
        if (string.IsNullOrEmpty(placementId) || candidate == null)
            return false;

        if (_candidatesByPlacement.TryGetValue(placementId, out var candidates))
        {
            var removed = candidates.Remove(candidate);
            if (removed)
            {
                UpdateLastModifiedTime();
            }
            return removed;
        }

        return false;
    }

    /// <summary>
    /// 清空指定广告位的候选广告
    /// </summary>
    public void ClearCandidatesForPlacement(string placementId)
    {
        if (string.IsNullOrEmpty(placementId))
            return;

        if (_candidatesByPlacement.TryGetValue(placementId, out var candidates))
        {
            candidates.Clear();
            UpdateLastModifiedTime();
        }
    }

    /// <summary>
    /// 获取所有广告位ID列表
    /// </summary>
    public IReadOnlyList<string> GetAllPlacements()
    {
        return _candidatesByPlacement.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取候选广告总数
    /// </summary>
    public int GetTotalCandidatesCount()
    {
        return _candidatesByPlacement.Values.Sum(candidates => candidates.Count);
    }

    /// <summary>
    /// 获取时间段
    /// </summary>
    public string GetTimeSlot()
    {
        var hour = RequestTime.Hour;
        return hour switch
        {
            >= 6 and < 12 => "上午",
            >= 12 and < 18 => "下午",
            >= 18 and < 24 => "晚上",
            _ => "凌晨"
        };
    }

    /// <summary>
    /// 获取星期几
    /// </summary>
    public DayOfWeek GetDayOfWeek()
    {
        return RequestTime.DayOfWeek;
    }

    /// <summary>
    /// 获取定向上下文
    /// </summary>
    public TargetingContext GetTargetingContext()
    {
        return TargetingContext;
    }

    /// <summary>
    /// 更新环境信息
    /// </summary>
    public void UpdateEnvironmentInfo(IDictionary<string, object> environmentInfo)
    {
        if (environmentInfo == null)
            throw new ArgumentNullException(nameof(environmentInfo));

        EnvironmentInfo = environmentInfo.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly();
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// 添加环境信息
    /// </summary>
    public void AddEnvironmentInfo(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("环境信息键不能为空", nameof(key));

        var envInfo = EnvironmentInfo.ToDictionary(kv => kv.Key, kv => kv.Value);
        envInfo[key] = value;
        EnvironmentInfo = envInfo.AsReadOnly();
        UpdateLastModifiedTime();
    }



    /// <summary>
    /// 参数验证
    /// </summary>
    private static void ValidateParameters(string requestId, TargetingContext targetingContext)
    {
        if (string.IsNullOrEmpty(requestId))
            throw new ArgumentException("请求ID不能为空", nameof(requestId));

        if (targetingContext == null)
            throw new ArgumentNullException(nameof(targetingContext));
    }

}

















































