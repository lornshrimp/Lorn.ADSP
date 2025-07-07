namespace Lorn.ADSP.Core.Domain.Enums
{
    /// <summary>
    /// 隐私级别枚举
    /// </summary>
    public enum PrivacyLevel
    {
        /// <summary>
        /// 低隐私（允许所有数据使用）
        /// </summary>
        Low = 1,

        /// <summary>
        /// 标准隐私（允许基本数据使用）
        /// </summary>
        Standard = 2,

        /// <summary>
        /// 高隐私（限制数据使用）
        /// </summary>
        High = 3,

        /// <summary>
        /// 最高隐私（禁止大部分数据使用）
        /// </summary>
        Maximum = 4
    }
}
