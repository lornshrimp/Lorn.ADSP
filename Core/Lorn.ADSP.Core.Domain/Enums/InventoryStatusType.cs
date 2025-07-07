namespace Lorn.ADSP.Core.Domain.Enums
{
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
}
