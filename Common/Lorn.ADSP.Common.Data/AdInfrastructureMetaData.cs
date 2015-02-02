using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Lorn.ADSP.Common.DataModels
{
    [MetadataType(typeof(AdPositionMetaData))]
    public partial class AdPosition
    {
        partial void InitPKDefaultValues()
        {
            this.AdPositionId = Guid.NewGuid();
        }
    }
    public partial class AdPositionMetaData
    {
        [Display(Name = "广告位名称")]
        public object AdPositionName { get; set; }
        [Display(Name="广告位代码")]
        public object AdPositionCode { get; set; }
        [Display(Name = "支持高宽比")]
        public object AdPositionAspectRatios { get; set; }
        [Display(Name = "描述")]
        public object AdPositionDesc { get; set; }
        [Display(Name = "所属广告组")]
        public object AdPositionGroups { get; set; }
        [Display(AutoGenerateField=false)]
        public object AdPositionId { get; set; }
        [Display(Name="广告位尺寸")]
        public object AdPositionSize { get; set; }
        [Display(Name = "广告位Url")]
        public object AdPositionUrl { get; set; }
        [Display(Name = "支持创意类型")]
        public object CreativeTypes { get; set; }
        [Display(Name = "默认售卖方式")]
        public object DefaultSaleType { get; set; }
        [Display(Name = "最大轮播次数")]
        public object MaxSlideshowNumber { get; set; }
    }
}
