using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// �����ṩ�߲�ѯ����
/// ���ڴ�ע����в���ƥ������ݷ����ṩ��
/// </summary>
public class DataProviderQuery
{
    /// <summary>
    /// ҵ��ʵ������
    /// </summary>
    public string? BusinessEntity { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? TechnologyType { get; set; }

    /// <summary>
    /// ƽ̨����
    /// </summary>
    public string? PlatformType { get; set; }

    /// <summary>
    /// �ṩ������
    /// </summary>
    public DataProviderType? ProviderType { get; set; }

    /// <summary>
    /// ������ȼ�
    /// </summary>
    public int? MinPriority { get; set; }

    /// <summary>
    /// �Ƿ�ֻ��ѯ���õ��ṩ��
    /// </summary>
    public bool EnabledOnly { get; set; } = true;

    /// <summary>
    /// �Ƿ�ֻ��ѯ�������ṩ��
    /// </summary>
    public bool HealthyOnly { get; set; } = true;

    /// <summary>
    /// ����֧�ֵĲ�������
    /// </summary>
    public string[]? RequiredOperations { get; set; }

    /// <summary>
    /// ��չ��ѯ����
    /// </summary>
    public Dictionary<string, object> ExtendedFilters { get; set; } = new();

    /// <summary>
    /// �����ֶ�
    /// </summary>
    public string? OrderBy { get; set; } = "Priority";

    /// <summary>
    /// ������
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;

    /// <summary>
    /// �����������
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// ����������ݲ�ѯ
    /// </summary>
    /// <returns>������ݲ�ѯ����</returns>
    public static DataProviderQuery ForAdvertisement()
    {
        return new DataProviderQuery
        {
            BusinessEntity = "Advertisement",
            ProviderType = DataProviderType.BusinessLogic
        };
    }

    /// <summary>
    /// �����û��������ݲ�ѯ
    /// </summary>
    /// <returns>�û��������ݲ�ѯ����</returns>
    public static DataProviderQuery ForUserProfile()
    {
        return new DataProviderQuery
        {
            BusinessEntity = "UserProfile",
            ProviderType = DataProviderType.BusinessLogic
        };
    }

    /// <summary>
    /// �����������ݲ�ѯ
    /// </summary>
    /// <returns>�������ݲ�ѯ����</returns>
    public static DataProviderQuery ForTargeting()
    {
        return new DataProviderQuery
        {
            BusinessEntity = "Targeting",
            ProviderType = DataProviderType.BusinessLogic
        };
    }

    /// <summary>
    /// ����Ͷ�����ݲ�ѯ
    /// </summary>
    /// <returns>Ͷ�����ݲ�ѯ����</returns>
    public static DataProviderQuery ForDelivery()
    {
        return new DataProviderQuery
        {
            BusinessEntity = "Delivery",
            ProviderType = DataProviderType.BusinessLogic
        };
    }

    /// <summary>
    /// ���������ṩ�߲�ѯ
    /// </summary>
    /// <param name="technology">���漼������</param>
    /// <returns>�����ṩ�߲�ѯ����</returns>
    public static DataProviderQuery ForCache(string technology = "Redis")
    {
        return new DataProviderQuery
        {
            TechnologyType = technology,
            ProviderType = DataProviderType.Cache
        };
    }

    /// <summary>
    /// �������ݿ��ṩ�߲�ѯ
    /// </summary>
    /// <param name="technology">���ݿ⼼������</param>
    /// <returns>���ݿ��ṩ�߲�ѯ����</returns>
    public static DataProviderQuery ForDatabase(string technology = "SqlServer")
    {
        return new DataProviderQuery
        {
            TechnologyType = technology,
            ProviderType = DataProviderType.Database
        };
    }

    /// <summary>
    /// ������ƽ̨�ṩ�߲�ѯ
    /// </summary>
    /// <param name="platform">��ƽ̨����</param>
    /// <returns>��ƽ̨�ṩ�߲�ѯ����</returns>
    public static DataProviderQuery ForCloudPlatform(string platform = "AlibabaCloud")
    {
        return new DataProviderQuery
        {
            PlatformType = platform,
            ProviderType = DataProviderType.Cloud
        };
    }

    /// <summary>
    /// �����չ��������
    /// </summary>
    /// <param name="key">���˼�</param>
    /// <param name="value">����ֵ</param>
    /// <returns>��ǰ��ѯʵ��</returns>
    public DataProviderQuery WithFilter(string key, object value)
    {
        ExtendedFilters[key] = value;
        return this;
    }

    /// <summary>
    /// ���ý����������
    /// </summary>
    /// <param name="count">��������</param>
    /// <returns>��ǰ��ѯʵ��</returns>
    public DataProviderQuery Take(int count)
    {
        Limit = count;
        return this;
    }
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="field">�����ֶ�</param>
    /// <param name="direction">������</param>
    /// <returns>��ǰ��ѯʵ��</returns> 
    public DataProviderQuery SetOrderBy(string field, SortDirection direction = SortDirection.Ascending)
    {
        OrderBy = field;
        SortDirection = direction;
        return this;
    }
}

