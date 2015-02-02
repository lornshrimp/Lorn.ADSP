using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 筛选广告接口
    /// </summary>
    public interface IFilterAd
    {
       
        /// <summary>
        /// 筛选广告
        /// </summary>
        /// <param name="cookie">用户Cookie</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="adDispatchPlans">待过滤的候选广告</param>
        /// <param name="filteredAds">已经筛选出来的待投广告队列：广告位投放信息,广告结构化数据、已序列化的数据</param>
        /// <param name="additionalParameters">附加参数：键值对</param>
        /// <param name="filterConfiguration">过滤器配置信息</param>
        /// <param name="redirectConditionDefinitions">定向条件定义</param>
        /// <returns>筛选出来的广告集合</returns>
        ICollection<Ad> FilterAd(UserMediaCookie cookie, IDictionary<string, string> parameters, ICollection<Ad> adDispatchPlans, AdProcesserConfiguration filterConfiguration, IDictionary<Guid, RedirectDimension> redirectConditionDefinitions = null, IDictionary<AdPositionReleaseInfo, Queue<AdMaterialReleaseInfo>> filteredAds = null, IDictionary<string, object> additionalParameters = null);
    }
    public interface IFilterMetadata:IRedirectDimensionProcesserMetada
    {
        AdFilterType FilterType { get; }
    }
}
