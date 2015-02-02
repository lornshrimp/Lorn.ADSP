using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    public class AdPosition
    {
        public const string STR_ADPOSITION = "AdPosition";
        public Guid AdPositionId { get; set; }
        public int MaxReturnAdCount { get; set; }
        /// <summary>
        /// 广告位尺寸Id
        /// </summary>
        public Guid AdPositionSizeId { get; set; }
        /// <summary>
        /// 位置Id
        /// </summary>
        public Guid AdLocationId { get; set; }
        public Lorn.ADSP.Common.DataModels.AdPositionType PositionType { get; set; }
        public AdPosition() { }
        public AdPosition(AdPosition adPosition)
        {
            this.AdPositionId = adPosition.AdPositionId;
            this.MaxReturnAdCount = adPosition.MaxReturnAdCount;
            this.PositionType = adPosition.PositionType;
            this.AdLocationId = adPosition.AdLocationId;
            this.AdPositionSizeId = adPosition.AdPositionSizeId;
        }
    }

    public class AdPositionReleaseInfo:AdPosition
    {
        /// <summary>
        /// VAST版本
        /// </summary>
        public float VASTVersion { get; set; }
        /// <summary>
        /// 跟踪事件和Url的键值对
        /// </summary>
        public Dictionary<string, string> TrackingEvents { get; set; }
        /// <summary>
        /// 时间偏移，格式为：hh:mm:ss.mmm, “start”, “end”, n% (n 是0-100的整数), #m (m represents sequence and is an integer > 0)
        /// </summary>
        public string TimeOffset { get; set; }

        public AdPositionReleaseInfo():base()
        { }

        public AdPositionReleaseInfo(AdPosition adPosition):base(adPosition)
        { 
        }
    }
}
