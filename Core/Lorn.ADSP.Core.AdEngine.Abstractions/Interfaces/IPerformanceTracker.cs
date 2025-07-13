namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
{
    /// <summary>
    /// 性能追踪器接口
    /// </summary>
    public interface IPerformanceTracker : IDisposable
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        string OperationName { get; }

        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// 添加标签
        /// </summary>
        void AddTag(string key, string value);

        /// <summary>
        /// 添加属性
        /// </summary>
        void AddProperty(string key, object value);

        /// <summary>
        /// 标记成功
        /// </summary>
        void MarkSuccess();

        /// <summary>
        /// 标记失败
        /// </summary>
        void MarkFailure(string? reason = null);

        /// <summary>
        /// 停止追踪
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
