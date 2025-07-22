using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Core.Domain.Entities
{
    /// <summary>
    /// 广告位实体
    /// </summary>
    public class AdPlacement : EntityBase
    {
        /// <summary>
        /// 广告位名称
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// 所属媒体资源ID
        /// </summary>
        public Guid MediaResourceId { get; private set; }

        /// <summary>
        /// 广告位尺寸
        /// </summary>
        public AdSize Size { get; private set; } = null!;

        /// <summary>
        /// 广告位位置
        /// </summary>
        public string Position { get; private set; } = string.Empty;

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; private set; } = true;

        /// <summary>
        /// 底价（分）
        /// </summary>
        public decimal FloorPrice { get; private set; }

        /// <summary>
        /// 私有构造函数，用于ORM
        /// </summary>
        private AdPlacement() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdPlacement(Guid mediaResourceId, string name, AdSize size, string position, decimal floorPrice)
        {
            MediaResourceId = mediaResourceId;
            Name = name;
            Size = size;
            Position = position;
            FloorPrice = floorPrice;
        }
    }

}
