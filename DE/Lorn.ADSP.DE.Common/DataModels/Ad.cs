using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.Common.DataModels;

namespace Lorn.ADSP.DE.DataModels
{
    /// <summary>
    /// 广告
    /// </summary>
    public abstract class Ad
    {
        /// <summary>
        /// 投放计划Id
        /// </summary>
        public System.Guid AdSpotPlanId { get; set; }
        /// <summary>
        /// 主计划Id
        /// </summary>
        public System.Guid AdMasterPlanId { get; set; }
        /// <summary>
        /// 投放计划版本Id
        /// </summary>
        public Guid AdSpotPlanEditionId { get; set; }
        /// <summary>
        /// 投放计划组Id
        /// </summary>
        public System.Guid SpotPlanGroupId { get; set; }
        /// <summary>
        /// 投放广告位Id
        /// </summary>
        public Guid? AdPositionId { get; set; }
        /// <summary>
        /// 投放广告位尺寸Id
        /// </summary>
        public Guid? AdPositionSizeId { get; set; }
        /// <summary>
        /// 投放位置Id
        /// </summary>
        public Guid? AdLocationId { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 投放计划类型优先级
        /// </summary>
        public int SpotplanTypePriority { get; set; }
        /// <summary>
        /// 是否分类排他
        /// </summary>
        public bool CategoryExclusive { get; set; }
        /// <summary>
        /// 产品类别Id
        /// </summary>
        public Nullable<System.Guid> ProductCategoryId { get; set; }
        /// <summary>
        /// 总投放额
        /// </summary>
        public decimal BudgetAmount { get; set; }
        /// <summary>
        /// 打包折扣
        /// </summary>
        public decimal PackageDiscount { get; set; }
        /// <summary>
        /// 投放定向条件:维度Id，定向明细
        /// </summary>
        public virtual IDictionary<Guid,RedirectCondition> RedirctConditions { get; set; }
        /// <summary>
        /// 广告物料
        /// </summary>
        public virtual ICollection<AdMaterialDeliveryInfo> AdMaterials { get; set; }
        /// <summary>
        /// 频次控制
        /// </summary>
        public virtual ICollection<FrequencyControl> FrequencyControls { get; set; }
        /// <summary>
        /// IP库Id
        /// </summary>
        public virtual Guid IpLibraryId { get; set; }
        /// <summary>
        /// 销售类型
        /// </summary>
        public Lorn.ADSP.Common.DataModels.SaleType SaleType { get; set; }
        /// <summary>
        /// 宏观轮播顺序
        /// </summary>
        public Nullable<int> MacroSlideshowSequenceNo { get; set; }
        /// <summary>
        /// 微轮播类型
        /// </summary>
        public Nullable<MicroSlideshowType> MicroSlideshowType { get; set; }
        /// <summary>
        /// 刊例价
        /// </summary>
        public decimal PublishPrice { get; set; }
        /// <summary>
        /// 净价
        /// </summary>
        public decimal NetPrice { get; set; }
        /// <summary>
        /// 价格单位
        /// </summary>
        public Lorn.ADSP.Common.DataModels.PriceUnit PriceUnit { get; set; }

    }
}
