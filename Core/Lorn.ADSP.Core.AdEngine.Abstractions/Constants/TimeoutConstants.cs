namespace Lorn.ADSP.Core.AdEngine.Abstractions.Constants
{
    /// <summary>
    /// 超时常量
    /// </summary>
    public static class TimeoutConstants
    {
        /// <summary>
        /// 快速操作超时时间
        /// </summary>
        public static readonly TimeSpan FastOperation = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// 正常操作超时时间
        /// </summary>
        public static readonly TimeSpan NormalOperation = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// 慢速操作超时时间
        /// </summary>
        public static readonly TimeSpan SlowOperation = TimeSpan.FromSeconds(2);

        /// <summary>
        /// 数据库查询超时时间
        /// </summary>
        public static readonly TimeSpan DatabaseQuery = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 网络调用超时时间
        /// </summary>
        public static readonly TimeSpan NetworkCall = TimeSpan.FromSeconds(10);

        /// <summary>
        /// 文件操作超时时间
        /// </summary>
        public static readonly TimeSpan FileOperation = TimeSpan.FromSeconds(15);

        /// <summary>
        /// 批处理操作超时时间
        /// </summary>
        public static readonly TimeSpan BatchOperation = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 长时间运行操作超时时间
        /// </summary>
        public static readonly TimeSpan LongRunningOperation = TimeSpan.FromMinutes(5);
    }
}
