using Lorn.ADSP.Core.Shared.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models;

/// <summary>
/// 批量操作错误
/// </summary>
public class BatchOperationError
{
    /// <summary>
    /// 错误索引
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 错误代码
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 异常详情
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// 错误上下文
    /// </summary>
    public Dictionary<string, object> Context { get; set; } = new();
}



/// <summary>
/// 排序条件
/// </summary>
public class SortCriteria
{
    /// <summary>
    /// 排序字段
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// 排序方向
    /// </summary>
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}

/// <summary>
/// 分页信息
/// </summary>
public class PaginationInfo
{
    /// <summary>
    /// 页码（从0开始）
    /// </summary>
    public int PageIndex { get; set; } = 0;

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 跳过的项目数
    /// </summary>
    public int Skip => PageIndex * PageSize;

    /// <summary>
    /// 游标令牌（用于游标分页）
    /// </summary>
    public string? CursorToken { get; set; }
}

/// <summary>
/// 数据提供者统计信息
/// </summary>
public class DataProviderStatistics
{
    /// <summary>
    /// 请求总数
    /// </summary>
    public long TotalRequests { get; set; }

    /// <summary>
    /// 成功请求数
    /// </summary>
    public long SuccessfulRequests { get; set; }

    /// <summary>
    /// 失败请求数
    /// </summary>
    public long FailedRequests { get; set; }

    /// <summary>
    /// 平均响应时间（毫秒）
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// 最大响应时间（毫秒）
    /// </summary>
    public double MaxResponseTimeMs { get; set; }

    /// <summary>
    /// 最小响应时间（毫秒）
    /// </summary>
    public double MinResponseTimeMs { get; set; }

    /// <summary>
    /// 当前并发连接数
    /// </summary>
    public int CurrentConnections { get; set; }

    /// <summary>
    /// 最大并发连接数
    /// </summary>
    public int MaxConnections { get; set; }

    /// <summary>
    /// 缓存命中率
    /// </summary>
    public double CacheHitRate { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 扩展统计信息
    /// </summary>
    public Dictionary<string, object> ExtendedStatistics { get; set; } = new();
}

/// <summary>
/// 注册表统计信息
/// </summary>
public class RegistryStatistics
{
    /// <summary>
    /// 已注册提供者总数
    /// </summary>
    public int TotalProviders { get; set; }

    /// <summary>
    /// 启用的提供者数
    /// </summary>
    public int EnabledProviders { get; set; }

    /// <summary>
    /// 健康的提供者数
    /// </summary>
    public int HealthyProviders { get; set; }

    /// <summary>
    /// 按类型分组的提供者统计
    /// </summary>
    public Dictionary<DataProviderType, int> ProvidersByType { get; set; } = new();

    /// <summary>
    /// 按业务实体分组的提供者统计
    /// </summary>
    public Dictionary<string, int> ProvidersByBusinessEntity { get; set; } = new();

    /// <summary>
    /// 按技术类型分组的提供者统计
    /// </summary>
    public Dictionary<string, int> ProvidersByTechnology { get; set; } = new();

    /// <summary>
    /// 最后统计时间
    /// </summary>
    public DateTime LastCalculated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 路由统计信息
/// </summary>
public class RoutingStatistics
{
    /// <summary>
    /// 路由请求总数
    /// </summary>
    public long TotalRoutingRequests { get; set; }

    /// <summary>
    /// 成功路由数
    /// </summary>
    public long SuccessfulRoutes { get; set; }

    /// <summary>
    /// 失败路由数
    /// </summary>
    public long FailedRoutes { get; set; }

    /// <summary>
    /// 平均路由时间（毫秒）
    /// </summary>
    public double AverageRoutingTimeMs { get; set; }

    /// <summary>
    /// 按提供者类型分组的路由统计
    /// </summary>
    public Dictionary<DataProviderType, long> RoutesByProviderType { get; set; } = new();

    /// <summary>
    /// 按业务实体分组的路由统计
    /// </summary>
    public Dictionary<string, long> RoutesByBusinessEntity { get; set; } = new();

    /// <summary>
    /// 活跃路由规则数
    /// </summary>
    public int ActiveRoutingRules { get; set; }

    /// <summary>
    /// 最后统计时间
    /// </summary>
    public DateTime LastCalculated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 路由测试结果
/// </summary>
public class RoutingTestResult
{
    /// <summary>
    /// 测试是否成功
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// 测试消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 选择的提供者
    /// </summary>
    public IDataAccessProvider? SelectedProvider { get; set; }

    /// <summary>
    /// 应用的规则
    /// </summary>
    public IReadOnlyList<string> AppliedRules { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 测试时间
    /// </summary>
    public DateTime TestTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 测试耗时
    /// </summary>
    public TimeSpan Duration { get; set; }
}