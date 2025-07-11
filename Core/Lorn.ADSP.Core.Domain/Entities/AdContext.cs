using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Entities;

/// <summary>
/// ���������ʵ��
/// ����������AdRequest��������Ϣ��ɵ��������������ģ������λ��֯��ѡ��漯��
/// �������ڣ����󴴽�������ʹ�á����٣�����������������б��ֲ���
/// </summary>
public class AdContext : EntityBase
{
    /// <summary>
    /// ����Ψһ��ʶ
    /// </summary>
    public string RequestId { get; private set; }

    /// <summary>
    /// �û���ʶ
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime RequestTime { get; private set; }

    /// <summary>
    /// �����λ����ĺ�ѡ��漯��
    /// KeyΪ���λID��ValueΪAdCandidate����
    /// </summary>
    private readonly Dictionary<string, List<AdCandidate>> _candidatesByPlacement = new();
    public IReadOnlyDictionary<string, IReadOnlyList<AdCandidate>> CandidatesByPlacement =>
        _candidatesByPlacement.ToDictionary(
            kv => kv.Key,
            kv => (IReadOnlyList<AdCandidate>)kv.Value.AsReadOnly()
        ).AsReadOnly();

    /// <summary>
    /// ����������
    /// </summary>
    public TargetingContext TargetingContext { get; private set; }

    /// <summary>
    /// ������Ϣ
    /// ����ʱ�䡢��������Ȼ�������
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentInfo { get; private set; }


    /// <summary>
    /// �û�������Ϣ����TargetingContext��ȡ�Ŀ�ݷ��ʣ�
    /// </summary>
    public UserProfile? UserProfile => TargetingContext?.GetTargetingContext<UserProfile>("user") ?? TargetingContext?.GetTargetingContext<UserProfile>();

    /// <summary>
    /// ˽�й��캯��
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
    /// ������������Ķ���
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
    /// ��ȡָ�����λ�ĺ�ѡ����б�
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
    /// ��Ӻ�ѡ��浽ָ�����λ
    /// </summary>
    public void AddCandidateToPlacement(string placementId, AdCandidate candidate)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        if (candidate == null)
            throw new ArgumentNullException(nameof(candidate));

        // ȷ����ѡ����PlacementId��Ŀ����λһ��
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
    /// ������Ӻ�ѡ��浽ָ�����λ
    /// </summary>
    public void AddCandidatesToPlacement(string placementId, IEnumerable<AdCandidate> candidates)
    {
        if (string.IsNullOrEmpty(placementId))
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        if (candidates == null)
            throw new ArgumentNullException(nameof(candidates));

        foreach (var candidate in candidates)
        {
            AddCandidateToPlacement(placementId, candidate);
        }
    }

    /// <summary>
    /// �Ƴ�ָ�����λ�ĺ�ѡ���
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
    /// ���ָ�����λ�ĺ�ѡ���
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
    /// ��ȡ���й��λID�б�
    /// </summary>
    public IReadOnlyList<string> GetAllPlacements()
    {
        return _candidatesByPlacement.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// ��ȡ��ѡ�������
    /// </summary>
    public int GetTotalCandidatesCount()
    {
        return _candidatesByPlacement.Values.Sum(candidates => candidates.Count);
    }

    /// <summary>
    /// ��ȡʱ���
    /// </summary>
    public string GetTimeSlot()
    {
        var hour = RequestTime.Hour;
        return hour switch
        {
            >= 6 and < 12 => "����",
            >= 12 and < 18 => "����",
            >= 18 and < 24 => "����",
            _ => "�賿"
        };
    }

    /// <summary>
    /// ��ȡ���ڼ�
    /// </summary>
    public DayOfWeek GetDayOfWeek()
    {
        return RequestTime.DayOfWeek;
    }

    /// <summary>
    /// ��ȡ����������
    /// </summary>
    public TargetingContext GetTargetingContext()
    {
        return TargetingContext;
    }

    /// <summary>
    /// ���»�����Ϣ
    /// </summary>
    public void UpdateEnvironmentInfo(IDictionary<string, object> environmentInfo)
    {
        if (environmentInfo == null)
            throw new ArgumentNullException(nameof(environmentInfo));

        EnvironmentInfo = environmentInfo.ToDictionary(kv => kv.Key, kv => kv.Value).AsReadOnly();
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ��ӻ�����Ϣ
    /// </summary>
    public void AddEnvironmentInfo(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("������Ϣ������Ϊ��", nameof(key));

        var envInfo = EnvironmentInfo.ToDictionary(kv => kv.Key, kv => kv.Value);
        envInfo[key] = value;
        EnvironmentInfo = envInfo.AsReadOnly();
        UpdateLastModifiedTime();
    }



    /// <summary>
    /// ������֤
    /// </summary>
    private static void ValidateParameters(string requestId, TargetingContext targetingContext)
    {
        if (string.IsNullOrEmpty(requestId))
            throw new ArgumentException("����ID����Ϊ��", nameof(requestId));

        if (targetingContext == null)
            throw new ArgumentNullException(nameof(targetingContext));
    }

}

















































