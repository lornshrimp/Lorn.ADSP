using Lorn.ADSP.Infrastructure.Common.Abstractions;

namespace Lorn.ADSP.Infrastructure.Common.Base;

/// <summary>
/// 组件基类
/// 提供组件生命周期管理的基础实现
/// </summary>
public abstract class ComponentBase : IComponent, IAsyncDisposable
{
    private bool _isInitialized;
    private bool _isDisposed;

    /// <summary>
    /// 组件名称
    /// </summary>
    public virtual string Name => GetType().Name;

    /// <summary>
    /// 组件是否已初始化
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// 初始化组件
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
            return;

        if (_isDisposed)
            throw new ObjectDisposedException(Name);

        await OnInitializeAsync(cancellationToken);
        _isInitialized = true;
    }

    /// <summary>
    /// 组件初始化的具体实现，由子类重写
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    protected virtual Task OnInitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 释放组件资源
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_isDisposed)
            return;

        await OnDisposeAsync();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 组件资源释放的具体实现，由子类重写
    /// </summary>
    protected virtual Task OnDisposeAsync()
    {
        return Task.CompletedTask;
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        // 释放非托管资源
        if (!_isDisposed)
        {
            // 这里可以添加释放资源的逻辑
            _isDisposed = true;
        }
        return ValueTask.CompletedTask;
    }
}