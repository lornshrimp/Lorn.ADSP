namespace Lorn.ADSP.Infrastructure.Common.Abstractions;

/// <summary>
/// 定义可配置组件的基础接口
/// 实现此接口的组件可以接收配置变更通知并重新配置自身
/// </summary>
public interface IConfigurable
{
    /// <summary>
    /// 当配置发生变更时调用此方法重新配置组件
    /// </summary>
    /// <param name="configuration">新的配置对象</param>
    void Configure(object configuration);
    
    /// <summary>
    /// 获取组件的配置类型
    /// </summary>
    Type ConfigurationType { get; }
}