using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 整理待投广告堆栈接口
    /// </summary>
    public interface IReorganizeAdStacks
    {
        /// <summary>
        /// 整理待投广告堆栈
        /// </summary>
        /// <param name="adDispatchPlans">广告投放计划：媒体Id，广告，投放时序（贴片位置），投放量</param>
        /// <param name="currentTime">当前时点</param>
        void ReorganizeAdStacks(IDictionary<DateTime, IDictionary<Guid,  IDictionary<Ad, IDictionary<int, long>>>> adDispatchPlans, DateTime currentTime);
    }
}
