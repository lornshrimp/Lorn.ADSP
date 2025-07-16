namespace Lorn.ADSP.Infrastructure.Common.Abstractions;

/// <summary>
/// 定义组件的基础接口
/// 所有业务组件都应该继承此接口或其子接口
/// </summary>
public interface IComponent
{
    /// <summary>
    /// 组件名称
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// 组件是否已初始化
    /// </summary>
    bool IsInitialized { get; }
    
    /// <summary>
    /// 初始化组件
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 释放组件资源
    /// </summary>
    Task DisposeAsync();
}