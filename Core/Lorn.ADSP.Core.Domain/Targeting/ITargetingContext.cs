using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Targeting
{
    /// <summary>
    /// 定向上下文抽象接口
    /// 定义统一的上下文数据访问规范，支持多种类型的定向信息存储和检索
    /// </summary>
    public interface ITargetingContext
    {
        /// <summary>
        /// 上下文名称
        /// 用于标识和描述上下文的友好名称
        /// </summary>
        string ContextName { get; }

        /// <summary>
        /// 上下文类型标识
        /// 例如：AdRequest、UserProfile、DeviceInfo等
        /// </summary>
        string ContextType { get; }

        /// <summary>
        /// 上下文属性集合 - 集合导航属性
        /// 存储各种定向相关的属性信息，替代原有的Properties字典
        /// </summary>
        IReadOnlyList<ContextProperty> Properties { get; }

        /// <summary>
        /// 上下文创建时间戳
        /// 用于缓存管理和数据有效性检查
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// 上下文的唯一标识
        /// 用于缓存键生成和调试追踪
        /// </summary>
        Guid ContextId { get; }

        /// <summary>
        /// 上下文数据来源
        /// 标识数据的来源系统或组件，用于数据质量评估
        /// </summary>
        string DataSource { get; }

        /// <summary>
        /// 获取指定键的属性实体
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <returns>属性实体，如果不存在则返回null</returns>
        ContextProperty? GetProperty(string propertyKey);

        /// <summary>
        /// 获取指定类型的属性值
        /// </summary>
        /// <typeparam name="T">属性值类型</typeparam>
        /// <param name="propertyKey">属性键</param>
        /// <returns>属性值，如果不存在则返回默认值</returns>
        T? GetPropertyValue<T>(string propertyKey);

        /// <summary>
        /// 获取指定类型的属性值，如果不存在则返回默认值
        /// </summary>
        /// <typeparam name="T">属性值类型</typeparam>
        /// <param name="propertyKey">属性键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>属性值或默认值</returns>
        T GetPropertyValue<T>(string propertyKey, T defaultValue);

        /// <summary>
        /// 获取属性值的字符串表示
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <returns>属性值的字符串表示，如果不存在则返回空字符串</returns>
        string GetPropertyAsString(string propertyKey);

        /// <summary>
        /// 检查是否包含指定的属性
        /// </summary>
        /// <param name="propertyKey">属性键</param>
        /// <returns>是否包含该属性</returns>
        bool HasProperty(string propertyKey);

        /// <summary>
        /// 获取所有属性键
        /// </summary>
        /// <returns>属性键集合</returns>
        IReadOnlyCollection<string> GetPropertyKeys();

        /// <summary>
        /// 获取指定分类的属性
        /// </summary>
        /// <param name="category">属性分类</param>
        /// <returns>属性集合</returns>
        IReadOnlyList<ContextProperty> GetPropertiesByCategory(string category);

        /// <summary>
        /// 获取未过期的属性
        /// </summary>
        /// <returns>未过期的属性集合</returns>
        IReadOnlyList<ContextProperty> GetActiveProperties();

        /// <summary>
        /// 检查上下文数据是否有效
        /// 根据时间戳和数据完整性进行验证
        /// </summary>
        /// <returns>是否有效</returns>
        bool IsValid();

        /// <summary>
        /// 检查上下文数据是否已过期
        /// </summary>
        /// <param name="maxAge">最大有效期</param>
        /// <returns>是否已过期</returns>
        bool IsExpired(TimeSpan maxAge);

        /// <summary>
        /// 获取上下文的元数据信息
        /// 包括数据来源、创建时间、属性数量等信息
        /// </summary>
        /// <returns>元数据属性集合</returns>
        IReadOnlyList<ContextProperty> GetMetadata();

        /// <summary>
        /// 获取上下文的调试信息
        /// 用于问题排查和日志记录
        /// </summary>
        /// <returns>调试信息字符串</returns>
        string GetDebugInfo();

        /// <summary>
        /// 创建上下文的轻量级副本
        /// 只包含指定的属性键，用于性能优化
        /// </summary>
        /// <param name="includeKeys">要包含的属性键</param>
        /// <returns>轻量级上下文副本</returns>
        ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys);

        /// <summary>
        /// 创建上下文的分类副本
        /// 只包含指定分类的属性，用于定向策略优化
        /// </summary>
        /// <param name="categories">要包含的属性分类</param>
        /// <returns>分类上下文副本</returns>
        ITargetingContext CreateCategorizedCopy(IEnumerable<string> categories);

        /// <summary>
        /// 合并另一个上下文的属性
        /// 用于组合多个数据源的上下文信息
        /// </summary>
        /// <param name="other">要合并的上下文</param>
        /// <param name="overwriteExisting">是否覆盖已存在的属性</param>
        /// <returns>合并后的新上下文</returns>
        ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false);
    }
}