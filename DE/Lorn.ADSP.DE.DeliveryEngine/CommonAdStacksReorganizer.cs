using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;
using Lorn.ADSP.DE.Interfaces;
using System.ComponentModel.Composition;
using System.Collections.Concurrent;

namespace Lorn.ADSP.DE.DeliveryEngine
{
    [Export(typeof(IReorganizeAdStacks))]
    public class CommonAdStacksReorganizer : IReorganizeAdStacks
    {
        #region IReorganizeAdStacks 成员

        public void ReorganizeAdStacks(IDictionary<DateTime,  IDictionary<Guid, IDictionary<Ad, IDictionary<int, long>>>> adDispatchPlans, DateTime currentTime)
        {
            var time = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0);
            if (adDispatchPlans != null && adDispatchPlans.ContainsKey(time.AddMinutes(-1)) && adDispatchPlans.ContainsKey(time.AddMinutes(1)))
            {
                //上一分钟
                var lastMeidaAdPlans = adDispatchPlans[time.AddMinutes(-1)];
                //下一分钟
                var nextMediaAdPlans = adDispatchPlans[time.AddMinutes(1)];
                //轮询处理每个媒体的广告
                foreach (var lastAdMediaAdPlan in lastMeidaAdPlans.AsParallel())
                {
                    var mediaId = lastAdMediaAdPlan.Key;
                    if (!nextMediaAdPlans.ContainsKey(mediaId))
                    {
                        nextMediaAdPlans[mediaId] = new ConcurrentDictionary<Ad, IDictionary<int, long>>();
                    }
                    //轮询处理该媒体内每个广告
                    foreach (var lastAdPlan in lastAdMediaAdPlan.Value)
                    {
                        int i = 0;
                        while (lastAdPlan.Value.ContainsKey(i))
                        {
                            if (!nextMediaAdPlans[mediaId].ContainsKey(lastAdPlan.Key))
                            {
                                nextMediaAdPlans[mediaId][lastAdPlan.Key] = new ConcurrentDictionary<int, long>();
                            }
                            //TODO:对于贴片位置定向的情况需要进行处理
                            nextMediaAdPlans[mediaId][lastAdPlan.Key][i + 1] = lastAdPlan.Value[i];
                            i++;
                        }
                    }
                }
            }
        }

        #endregion
    }
}


