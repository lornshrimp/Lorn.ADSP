using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// �������ֵ����
/// </summary>
public class TargetingPolicy : ValueObject
{
    private readonly Dictionary<string, ITargetingCriteria> _criteria;

    /// <summary>
    /// �����������ϣ�ֻ����
    /// </summary>
    public IReadOnlyDictionary<string, ITargetingCriteria> Criteria => _criteria.AsReadOnly();

    /// <summary>
    /// ����Ȩ��
    /// </summary>
    public decimal Weight { get; private set; } = 1.0m;

    /// <summary>
    /// �Ƿ����ö���
    /// </summary>
    public bool IsEnabled { get; private set; } = true;

    /// <summary>
    /// ���Դ���ʱ��
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// ���Ը���ʱ��
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private TargetingPolicy()
    {
        _criteria = new Dictionary<string, ITargetingCriteria>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="criteria">������������</param>
    /// <param name="weight">����Ȩ��</param>
    /// <param name="isEnabled">�Ƿ�����</param>
    public TargetingPolicy(
        IDictionary<string, ITargetingCriteria>? criteria = null,
        decimal weight = 1.0m,
        bool isEnabled = true)
    {
        _criteria = criteria?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, ITargetingCriteria>();
        Weight = weight;
        IsEnabled = isEnabled;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        ValidateWeight(weight);
    }

    /// <summary>
    /// �����յĶ������
    /// </summary>
    public static TargetingPolicy CreateEmpty()
    {
        return new TargetingPolicy();
    }

    /// <summary>
    /// ����ȫ���������ƣ�������
    /// </summary>
    public static TargetingPolicy CreateUnrestricted()
    {
        return new TargetingPolicy(isEnabled: false);
    }

    /// <summary>
    /// ��ӻ���¶�������
    /// </summary>
    /// <param name="criteria">��������</param>
    public void AddOrUpdateCriteria(ITargetingCriteria criteria)
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        _criteria[criteria.CriteriaType] = criteria;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// �Ƴ���������
    /// </summary>
    /// <param name="criteriaType">��������</param>
    /// <returns>�Ƿ�ɹ��Ƴ�</returns>
    public bool RemoveCriteria(string criteriaType)
    {
        if (string.IsNullOrEmpty(criteriaType))
            return false;

        var result = _criteria.Remove(criteriaType);
        if (result)
        {
            UpdatedAt = DateTime.UtcNow;
        }
        return result;
    }

    /// <summary>
    /// ��ȡָ�����͵Ķ�������
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="criteriaType">�������ͱ�ʶ</param>
    /// <returns>��������������������򷵻�null</returns>
    public T? GetCriteria<T>(string criteriaType) where T : class, ITargetingCriteria
    {
        if (string.IsNullOrEmpty(criteriaType))
            return null;

        return _criteria.TryGetValue(criteriaType, out var criteria) ? criteria as T : null;
    }

    /// <summary>
    /// ����Ƿ����ָ�����͵Ķ�������
    /// </summary>
    /// <param name="criteriaType">�������ͱ�ʶ</param>
    /// <returns>�Ƿ����</returns>
    public bool HasCriteria(string criteriaType)
    {
        return !string.IsNullOrEmpty(criteriaType) && _criteria.ContainsKey(criteriaType);
    }

    /// <summary>
    /// ��ȡ���������õĶ�������
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetEnabledCriteria()
    {
        return _criteria.Values.Where(c => c.IsEnabled);
    }

    /// <summary>
    /// ��ȡ���ж�����������
    /// </summary>
    public IReadOnlyCollection<string> GetCriteriaTypes()
    {
        return _criteria.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// ��֤�����Ƿ���Ч
    /// </summary>
    public bool IsValid()
    {
        if (!IsEnabled)
            return true;

        // ������Ҫһ����������
        var enabledCriteria = GetEnabledCriteria().ToList();
        if (!enabledCriteria.Any())
            return false;

        // ��֤�������õ�����������Ч��
        return enabledCriteria.All(criteria => criteria.IsValid());
    }

    /// <summary>
    /// ��ȡ��������ժҪ
    /// </summary>
    public string GetConfigurationSummary()
    {
        if (!IsEnabled)
            return "Targeting: Disabled (Unrestricted)";

        var enabledCriteria = GetEnabledCriteria().ToList();
        if (!enabledCriteria.Any())
            return "Targeting: No criteria defined";

        var criteriaTypes = enabledCriteria.Select(c => c.CriteriaType);
        return $"Targeting: {string.Join(", ", criteriaTypes)} (Weight: {Weight:F2})";
    }

    /// <summary>
    /// ���²���Ȩ��
    /// </summary>
    /// <param name="newWeight">��Ȩ��</param>
    public void UpdateWeight(decimal newWeight)
    {
        ValidateWeight(newWeight);
        Weight = newWeight;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ���²�������״̬
    /// </summary>
    /// <param name="enabled">�Ƿ�����</param>
    public void UpdateEnabled(bool enabled)
    {
        IsEnabled = enabled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Weight;
        yield return IsEnabled;

        // ��������������ȷ��һ�µıȽϽ��
        foreach (var criteria in _criteria.OrderBy(kv => kv.Key))
        {
            yield return criteria.Key;
            yield return criteria.Value;
        }
    }

    /// <summary>
    /// ��֤Ȩ��ֵ
    /// </summary>
    /// <param name="weight">Ȩ��ֵ</param>
    private static void ValidateWeight(decimal weight)
    {
        if (weight < 0)
            throw new ArgumentException("Ȩ�ز���Ϊ����", nameof(weight));
    }

    // ��ݷ�����Ϊ�˱����������ԣ��ṩһЩ���ö��������Ŀ�ݷ��ʷ���

    /// <summary>
    /// ��ȡ������������������
    /// </summary>
    public AdministrativeGeoTargeting? AdministrativeGeoTargeting => GetCriteria<AdministrativeGeoTargeting>("AdministrativeGeo");

    /// <summary>
    /// ��ȡԲ�ε���Χ����������
    /// </summary>
    public CircularGeoFenceTargeting? CircularGeoFenceTargeting => GetCriteria<CircularGeoFenceTargeting>("CircularGeoFence");

    /// <summary>
    /// ��ȡ����ε���Χ����������
    /// </summary>
    public PolygonGeoFenceTargeting? PolygonGeoFenceTargeting => GetCriteria<PolygonGeoFenceTargeting>("PolygonGeoFence");

    /// <summary>
    /// ��ȡ�˿����Զ�������
    /// </summary>
    public DemographicTargeting? DemographicTargeting => GetCriteria<DemographicTargeting>("Demographic");

    /// <summary>
    /// ��ȡ�豸��������
    /// </summary>
    public DeviceTargeting? DeviceTargeting => GetCriteria<DeviceTargeting>("Device");

    /// <summary>
    /// ��ȡʱ�䶨������
    /// </summary>
    public TimeTargeting? TimeTargeting => GetCriteria<TimeTargeting>("Time");

    /// <summary>
    /// ��ȡ��Ϊ��������
    /// </summary>
    public BehaviorTargeting? BehaviorTargeting => GetCriteria<BehaviorTargeting>("Behavior");

    /// <summary>
    /// ��ȡ���е���������
    /// </summary>
    public IEnumerable<ITargetingCriteria> GetGeoTargetingCriteria()
    {
        if (AdministrativeGeoTargeting != null && AdministrativeGeoTargeting.IsEnabled)
            yield return AdministrativeGeoTargeting;

        if (CircularGeoFenceTargeting != null && CircularGeoFenceTargeting.IsEnabled)
            yield return CircularGeoFenceTargeting;

        if (PolygonGeoFenceTargeting != null && PolygonGeoFenceTargeting.IsEnabled)
            yield return PolygonGeoFenceTargeting;
    }

    /// <summary>
    /// ����Ƿ����κε���������
    /// </summary>
    public bool HasGeoTargeting()
    {
        return GetGeoTargetingCriteria().Any();
    }
}









