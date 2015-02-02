using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ICacheAdMonitor
    {
        /// <summary>
        /// 设置广告监测缓存
        /// </summary>
        /// <param name="adMonitorRecords">广告监测缓存记录</param>
        void SetCacheAdMonitor(AdMonitorRecord adMonitorRecords);
    }
}
