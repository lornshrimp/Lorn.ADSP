namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 预算状态类型
/// </summary>
public enum BudgetStatusType
{
    /// <summary>
    /// 活跃
    /// </summary>
    Active = 1,

    /// <summary>
    /// 耗尽
    /// </summary>
    Exhausted = 2,

    /// <summary>
    /// 暂停
    /// </summary>
    Paused = 3,

    /// <summary>
    /// 超限
    /// </summary>
    OverLimit = 4,

    /// <summary>
    /// 预警
    /// </summary>
    Warning = 5
}

/// <summary>
/// 黑名单类型
/// </summary>
public enum BlacklistType
{
    /// <summary>
    /// 用户黑名单
    /// </summary>
    User = 1,

    /// <summary>
    /// 设备黑名单
    /// </summary>
    Device = 2,

    /// <summary>
    /// IP黑名单
    /// </summary>
    IpAddress = 3,

    /// <summary>
    /// 广告黑名单
    /// </summary>
    Ad = 4,

    /// <summary>
    /// 广告主黑名单
    /// </summary>
    Advertiser = 5,

    /// <summary>
    /// 域名黑名单
    /// </summary>
    Domain = 6,

    /// <summary>
    /// 关键词黑名单
    /// </summary>
    Keyword = 7,

    /// <summary>
    /// 地理位置黑名单
    /// </summary>
    Geo = 8
}

/// <summary>
/// 库存状态类型
/// </summary>
public enum InventoryStatusType
{
    /// <summary>
    /// 可用
    /// </summary>
    Available = 1,

    /// <summary>
    /// 预订
    /// </summary>
    Reserved = 2,

    /// <summary>
    /// 售罄
    /// </summary>
    SoldOut = 3,

    /// <summary>
    /// 维护中
    /// </summary>
    Maintenance = 4,

    /// <summary>
    /// 不可用
    /// </summary>
    Unavailable = 5
}



/// <summary>
/// 创意格式
/// </summary>
public enum CreativeFormat
{
    /// <summary>
    /// 横幅广告
    /// </summary>
    Banner = 1,

    /// <summary>
    /// 插屏广告
    /// </summary>
    Interstitial = 2,

    /// <summary>
    /// 视频广告
    /// </summary>
    Video = 3,

    /// <summary>
    /// 原生广告
    /// </summary>
    Native = 4,

    /// <summary>
    /// 弹窗广告
    /// </summary>
    Popup = 5,

    /// <summary>
    /// 覆盖广告
    /// </summary>
    Overlay = 6,

    /// <summary>
    /// 可扩展广告
    /// </summary>
    Expandable = 7,

    /// <summary>
    /// 富媒体广告
    /// </summary>
    RichMedia = 8
}

/// <summary>
/// 执行状态
/// </summary>
public enum ExecutionStatus
{
    /// <summary>
    /// 待执行
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 运行中
    /// </summary>
    Running = 2,

    /// <summary>
    /// 成功
    /// </summary>
    Success = 3,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 4,

    /// <summary>
    /// 已失败
    /// </summary>
    Failed = 5,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// 已跳过
    /// </summary>
    Skipped = 7
}



/// <summary>
/// 告警级别
/// </summary>
public enum AlertLevel
{
    /// <summary>
    /// 信息
    /// </summary>
    Info = 1,

    /// <summary>
    /// 警告
    /// </summary>
    Warning = 2,

    /// <summary>
    /// 错误
    /// </summary>
    Error = 3,

    /// <summary>
    /// 严重
    /// </summary>
    Critical = 4,

    /// <summary>
    /// 紧急
    /// </summary>
    Emergency = 5
}

/// <summary>
/// 审计结果
/// </summary>
public enum AuditResult
{
    /// <summary>
    /// 成功
    /// </summary>
    Success = 1,

    /// <summary>
    /// 失败
    /// </summary>
    Failure = 2,

    /// <summary>
    /// 部分成功
    /// </summary>
    PartialSuccess = 3,

    /// <summary>
    /// 跳过
    /// </summary>
    Skipped = 4,

    /// <summary>
    /// 未授权
    /// </summary>
    Unauthorized = 5,

    /// <summary>
    /// 拒绝访问
    /// </summary>
    AccessDenied = 6
}