using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.Entities;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 广告上下文值对象
/// 封装广告请求的完整上下文信息，为广告投放决策提供环境数据
/// 设计原则：不可变、基于值相等、临时存在（请求创建→使用→销毁）
/// </summary>
public class AdContext : ValueObject
{
    /// <summary>
    /// 请求唯一标识
    /// </summary>
    public string RequestId { get; }

    /// <summary>
    /// 用户标识
    /// </summary>
    public Guid? UserId { get; }

    /// <summary>
    /// 请求时间
    /// </summary>
    public DateTime RequestTime { get; }

    /// <summary>
    /// 定向上下文
    /// </summary>
    public TargetingContext? TargetingContext { get; }

    /// <summary>
    /// 用户画像
    /// </summary>
    public UserProfile? UserProfile { get; }

    /// <summary>
    /// 环境信息
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentInfo { get; }

    /// <summary>
    /// 各广告位的候选广告
    /// Key为广告位ID，Value为AdCandidate列表
    /// </summary>
    public IReadOnlyDictionary<Guid, IReadOnlyList<AdCandidate>> CandidatesByPlacement { get; }

    /// <summary>
    /// 私有构造函数，强制使用工厂方法创建
    /// </summary>
    private AdContext(
        string requestId,
        Guid? userId,
        DateTime requestTime,
        TargetingContext? targetingContext,
        UserProfile? userProfile,
        IReadOnlyDictionary<string, object> environmentInfo,
        IReadOnlyDictionary<Guid, IReadOnlyList<AdCandidate>> candidatesByPlacement)
    {
        RequestId = requestId;
        UserId = userId;
        RequestTime = requestTime;
        TargetingContext = targetingContext;
        UserProfile = userProfile;
        EnvironmentInfo = environmentInfo;
        CandidatesByPlacement = candidatesByPlacement;
    }

    /// <summary>
    /// 创建空的广告上下文
    /// </summary>
    public static AdContext CreateEmpty(string requestId, Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(requestId))
            throw new ArgumentException("请求ID不能为空", nameof(requestId));

        return new AdContext(
            requestId,
            userId,
            DateTime.UtcNow,
            null,
            null,
            new Dictionary<string, object>(),
            new Dictionary<Guid, IReadOnlyList<AdCandidate>>());
    }

    /// <summary>
    /// 创建包含完整信息的广告上下文
    /// </summary>
    public static AdContext Create(
        string requestId,
        Guid? userId,
        TargetingContext? targetingContext,
        UserProfile? userProfile,
        IDictionary<string, object>? environmentInfo = null)
    {
        if (string.IsNullOrWhiteSpace(requestId))
            throw new ArgumentException("请求ID不能为空", nameof(requestId));

        var envInfo = environmentInfo ?? new Dictionary<string, object>();

        return new AdContext(
            requestId,
            userId,
            DateTime.UtcNow,
            targetingContext,
            userProfile,
            new Dictionary<string, object>(envInfo),
            new Dictionary<Guid, IReadOnlyList<AdCandidate>>());
    }

    /// <summary>
    /// 添加候选广告到指定广告位（返回新的AdContext实例）
    /// </summary>
    public AdContext WithCandidateForPlacement(Guid placementId, AdCandidate candidate)
    {
        if (placementId == Guid.Empty)
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (candidate == null)
            throw new ArgumentNullException(nameof(candidate));

        var newCandidates = new Dictionary<Guid, IReadOnlyList<AdCandidate>>(CandidatesByPlacement);

        if (newCandidates.ContainsKey(placementId))
        {
            var existingCandidates = newCandidates[placementId].ToList();
            existingCandidates.Add(candidate);
            newCandidates[placementId] = existingCandidates.AsReadOnly();
        }
        else
        {
            newCandidates[placementId] = new List<AdCandidate> { candidate }.AsReadOnly();
        }

        return new AdContext(
            RequestId,
            UserId,
            RequestTime,
            TargetingContext,
            UserProfile,
            EnvironmentInfo,
            newCandidates);
    }

    /// <summary>
    /// 添加多个候选广告到指定广告位（返回新的AdContext实例）
    /// </summary>
    public AdContext WithCandidatesForPlacement(Guid placementId, IEnumerable<AdCandidate> candidates)
    {
        if (placementId == Guid.Empty)
            throw new ArgumentException("广告位ID不能为空", nameof(placementId));

        if (candidates == null)
            throw new ArgumentNullException(nameof(candidates));

        var candidateList = candidates.ToList();
        if (candidateList.Count == 0)
            return this; // 如果没有候选广告，返回当前实例

        // 使用第一个候选广告开始，然后逐个添加其余的
        var result = WithCandidateForPlacement(placementId, candidateList[0]);

        for (int i = 1; i < candidateList.Count; i++)
        {
            result = result.WithCandidateForPlacement(placementId, candidateList[i]);
        }

        return result;
    }

    /// <summary>
    /// 更新环境信息（返回新的AdContext实例）
    /// </summary>
    public AdContext WithEnvironmentInfo(IDictionary<string, object> newEnvironmentInfo)
    {
        if (newEnvironmentInfo == null)
            throw new ArgumentNullException(nameof(newEnvironmentInfo));

        var mergedEnvInfo = new Dictionary<string, object>(EnvironmentInfo);
        foreach (var kvp in newEnvironmentInfo)
        {
            mergedEnvInfo[kvp.Key] = kvp.Value;
        }

        return new AdContext(
            RequestId,
            UserId,
            RequestTime,
            TargetingContext,
            UserProfile,
            mergedEnvInfo,
            CandidatesByPlacement);
    }

    /// <summary>
    /// 获取指定广告位的候选广告数量
    /// </summary>
    public int GetCandidateCountForPlacement(Guid placementId)
    {
        return CandidatesByPlacement.TryGetValue(placementId, out var candidates)
            ? candidates.Count
            : 0;
    }

    /// <summary>
    /// 获取指定广告位的候选广告
    /// </summary>
    public IReadOnlyList<AdCandidate> GetCandidatesForPlacement(Guid placementId)
    {
        return CandidatesByPlacement.TryGetValue(placementId, out var candidates)
            ? candidates
            : new List<AdCandidate>().AsReadOnly();
    }

    /// <summary>
    /// 检查是否有指定广告位的候选广告
    /// </summary>
    public bool HasCandidatesForPlacement(Guid placementId)
    {
        return CandidatesByPlacement.ContainsKey(placementId) &&
               CandidatesByPlacement[placementId].Count > 0;
    }

    /// <summary>
    /// 获取所有广告位的候选广告总数
    /// </summary>
    public int GetTotalCandidateCount()
    {
        return CandidatesByPlacement.Values.Sum(candidates => candidates.Count);
    }

    /// <summary>
    /// 获取有候选广告的广告位数量
    /// </summary>
    public int GetPlacementWithCandidatesCount()
    {
        return CandidatesByPlacement.Count(kvp => kvp.Value.Count > 0);
    }

    /// <summary>
    /// 获取环境信息中的指定键值
    /// </summary>
    public T? GetEnvironmentValue<T>(string key, T? defaultValue = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return defaultValue;

        if (EnvironmentInfo.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return defaultValue;
    }

    /// <summary>
    /// 验证上下文的有效性
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(RequestId) &&
               RequestTime > DateTime.MinValue;
    }

    /// <summary>
    /// 实现值对象的相等性比较组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RequestId;
        yield return UserId?.ToString() ?? string.Empty;
        yield return RequestTime;
        yield return TargetingContext ?? new object();
        yield return UserProfile?.ToString() ?? string.Empty;

        // 环境信息按键排序后比较
        foreach (var kvp in EnvironmentInfo.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }

        // 候选广告按广告位ID排序后比较
        foreach (var kvp in CandidatesByPlacement.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            foreach (var candidate in kvp.Value)
            {
                yield return candidate;
            }
        }
    }
}
