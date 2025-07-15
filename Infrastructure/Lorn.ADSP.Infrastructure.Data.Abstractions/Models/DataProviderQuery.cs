using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// 数据提供者查询条件
/// 用于从注册表中查找匹配的数据访问提供者
/// </summary>
public class DataProviderQuery
{
    /// <summary>
    /// 业务实体类型
    /// </summary>
    public string? BusinessEntity { get; set; }

    /// <summary>
    /// 技术类型
    /// </summary>
    public string? TechnologyType { get; set; }

    /// <summary>
    /// 平台类型
    /// </summary>
    public string? PlatformType { get; set; }

    /// <summary>
    /// 提供者类型
    /// </summary>
    public DataProviderType? ProviderType { get; set; }

    /// <summary>
    /// 最低优先级
    /// </summary>
    public int? MinPriority { get; set; }

    /// <summary>
    /// 是否只查询启用的提供者
    /// </summary>
    public bool EnabledOnly { get; set; } = true;

    /// <summary>
    /// 是否只查询健康的提供者
    /// </summary>
    public bool HealthyOnly { get; set; } = true;

    /// <summary>
    /// 必须支持的操作类型
    /// </summary>
    public string[]? RequiredOperations { get; set; }

    /// <summary>
    /// 扩展查询条件
    /// </summary>
    public Dictionary<string, object> ExtendedFilters { get; set; } = new();

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? OrderBy { get; set; } = "Priority";

    /// <summary>
    /// 排序方向
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;

    /// <summary>
    /// 结果数量限制
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// 创建广告数据查询
    /// </summary>
    /// <returns>广告数据查询条件</returns>
    public static DataProviderQuery ForAdvertisement()
    {
        return new DataProviderQuery
        {
            BusinessEntity = "Advertisement",
            ProviderType = DataProviderType.BusinessLogic
        };
    }

    /// <summary>
    /// 创建用户画像数据查询
    /// </summary>
    /// <returns>用户画像数据查询条件</returns>
    public static DataProviderQuery ForUserProfile()
    {
        return new DataProviderQuery
        {
            BusinessEntity = "UserProfile",
            ProviderType = DataProviderType.BusinessLogic
        };
    }

    /// <summary>
    /// 创建定向数据查询
    /// </summary>
    /// <returns>定向数据查询条件</returns>
    public static DataProviderQuery ForTargeting()
    {
        return new DataProviderQuery
        {
            BusinessEntity = "Targeting",
            ProviderType = DataProviderType.BusinessLogic
        };
    }

    /// <summary>
    /// 创建投放数据查询
    /// </summary>
    /// <returns>投放数据查询条件</returns>
    public static DataProviderQuery ForDelivery()
    {
        return new DataProviderQuery
        {
            BusinessEntity = "Delivery",
            ProviderType = DataProviderType.BusinessLogic
        };
    }

    /// <summary>
    /// 创建缓存提供者查询
    /// </summary>
    /// <param name="technology">缓存技术类型</param>
    /// <returns>缓存提供者查询条件</returns>
    public static DataProviderQuery ForCache(string technology = "Redis")
    {
        return new DataProviderQuery
        {
            TechnologyType = technology,
            ProviderType = DataProviderType.Cache
        };
    }

    /// <summary>
    /// 创建数据库提供者查询
    /// </summary>
    /// <param name="technology">数据库技术类型</param>
    /// <returns>数据库提供者查询条件</returns>
    public static DataProviderQuery ForDatabase(string technology = "SqlServer")
    {
        return new DataProviderQuery
        {
            TechnologyType = technology,
            ProviderType = DataProviderType.Database
        };
    }

    /// <summary>
    /// 创建云平台提供者查询
    /// </summary>
    /// <param name="platform">云平台类型</param>
    /// <returns>云平台提供者查询条件</returns>
    public static DataProviderQuery ForCloudPlatform(string platform = "AlibabaCloud")
    {
        return new DataProviderQuery
        {
            PlatformType = platform,
            ProviderType = DataProviderType.Cloud
        };
    }

    /// <summary>
    /// 添加扩展过滤条件
    /// </summary>
    /// <param name="key">过滤键</param>
    /// <param name="value">过滤值</param>
    /// <returns>当前查询实例</returns>
    public DataProviderQuery WithFilter(string key, object value)
    {
        ExtendedFilters[key] = value;
        return this;
    }

    /// <summary>
    /// 设置结果数量限制
    /// </summary>
    /// <param name="count">数量限制</param>
    /// <returns>当前查询实例</returns>
    public DataProviderQuery Take(int count)
    {
        Limit = count;
        return this;
    }
    /// <summary>
    /// 设置排序
    /// </summary>
    /// <param name="field">排序字段</param>
    /// <param name="direction">排序方向</param>
    /// <returns>当前查询实例</returns> 
    public DataProviderQuery SetOrderBy(string field, SortDirection direction = SortDirection.Ascending)
    {
        OrderBy = field;
        SortDirection = direction;
        return this;
    }
}

