using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ICountAd
    {
        /// <summary>
        /// 获取广告监测计数
        /// </summary>
        /// <returns>广告监测计数：媒体Id，AdSpotPlanId,MonitorTypeCode,计数</returns>
        IDictionary<Guid,IDictionary<Guid, Dictionary<string, long>>> GetAdMonitorCount();
    }
}
