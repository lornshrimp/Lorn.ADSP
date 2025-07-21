namespace Lorn.ADSP.Infrastructure.DataAccess.Abstractions.Enums;

/// <summary>
/// 数据一致性级别枚举
/// 定义数据访问时的一致性要求
/// </summary>
public enum DataConsistencyLevel
{
    /// <summary>
    /// 最终一致性 - 允许数据在一段时间后达到一致状态
    /// </summary>
    Eventual,

    /// <summary>
    /// 弱一致性 - 可以读取到过期或不一致的数据
    /// </summary>
    Weak,

    /// <summary>
    /// 强一致性 - 要求读取最新的一致数据
    /// </summary>
    Strong,

    /// <summary>
    /// 严格一致性 - 要求所有节点的数据完全一致
    /// </summary>
    Strict
}
