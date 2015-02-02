using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 缓存投放调度计划接口
    /// </summary>
    public interface ICacheAdDispatchPlans
    {
        /// <summary>
        /// 获取指定服务器上的投放调度计划
        /// </summary>
        /// <param name="serverId">服务器Id</param>
        /// <returns>调度计划：媒体Id，时间点，广告调度计划，计划请求量</returns>
        /// <remarks>计划请求量负值在0~-10000之间，表示流量比例消耗</remarks>
        IDictionary<Guid,IDictionary<DateTime,IDictionary<Ad,long>>> GetCachedAdDispatchPlans(string serverId);
        /// <summary>
        /// 保存投放调度计划
        /// </summary>
        /// <param name="adDispatchPlans">投放调度计划：服务器Id，媒体Id，时间点，广告调度计划，计划曝光量</param>
        void SetAdDispatchPlansCache(IDictionary<string, IDictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>>> adDispatchPlans);
    }
}
