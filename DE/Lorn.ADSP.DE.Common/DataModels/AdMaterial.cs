using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    /// <summary>
    /// 广告物料
    /// </summary>
    public class AdMaterial
    {
        /// <summary>
        /// 物料Id
        /// </summary>
        public Guid MaterialId { get; set; }
        /// <summary>
        /// 创意类型Id
        /// </summary>
        public System.Guid CreativeTypeId { get; set; }
        /// <summary>
        /// 时长
        /// </summary>
        public Nullable<System.TimeSpan> TimeLength { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public Nullable<int> Height { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        public Nullable<int> Width { get; set; }
        /// <summary>
        /// 高宽比Id
        /// </summary>
        public Nullable<System.Guid> AspectRatioId { get; set; }
        /// <summary>
        /// 扩展数据
        /// </summary>
        public string ExtData { get; set; }
        /// <summary>
        /// 物料表单信息
        /// </summary>
        public virtual ICollection<AdMaterialFormInfo> AdMaterialFormInfos { get; set; }
        /// <summary>
        /// 第三方监测代码
        /// </summary>
        public virtual ICollection<ThirdMonitorCode> ThirdMonitorCodes { get; set; }
        /// <summary>
        /// 频次控制
        /// </summary>
        public virtual ICollection<FrequencyControl> FrequencyControls { get; set; }
    }
    public class AdMaterialDeliveryInfo : AdMaterial
    {
        /// <summary>
        /// 微轮播顺序
        /// </summary>
        public Nullable<int> MicroSlideshowSequenceNo { get; set; }
    }
    public class AdMaterialReleaseInfo : AdMaterial
    {
        /// <summary>
        /// 广告
        /// </summary>
        public Ad Ad { get; set; }
        /// <summary>
        /// 投放管线配置Id
        /// </summary>
        public Guid DeliveryPiplineConfigurationId { get; set; }
        /// <summary>
        /// 投放管线配置版本
        /// </summary>
        public float DeliveryPiplineConfigurationVersion { get; set; }
    }
}
