using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 广告策略引擎运行接口
    /// </summary>
    public interface IRunDeliveryPolicy
    {
       
        /// <summary>
        /// 运行广告策略引擎
        /// </summary>
        /// <param name="adPositionOrAdPositionGroupId"></param>
        /// <param name="cookie"></param>
        /// <param name="parameters"></param>
        /// <param name="deliveryPiplineConfigurations">管线配置</param>
        /// <param name="redirectConditionDefinitions">定向条件定义</param>
        /// <param name="adPositionGroups">广告位组</param>
        /// <param name="adPositions">广告位</param>
        /// <param name="adDispatchPlans">待投广告投放计划集合：投放时序(贴片位序)，广告投放计划，计划请求量</param>
        /// <param name="creativeSerializerMappings">广告创意序列化器映射</param>
        /// <returns>已经筛选出来的待投广告集合：广告位，广告结构化信息</returns>
        IDictionary<AdPositionReleaseInfo, Queue<AdMaterialReleaseInfo>> RunDeliveryPolicy(Guid adPositionOrAdPositionGroupId, UserMediaCookie cookie, IDictionary<string, string> parameters, IDictionary<Guid, DeliveryPiplineConfiguration> deliveryPiplineConfigurations, IDictionary<Guid, AdPosition> adPositions, IDictionary<Guid, AdPositionGroup> adPositionGroups, IDictionary<Guid, RedirectDimension> redirectConditionDefinitions,IDictionary<Guid, IpLibrary> ipLibraries, IDictionary<Ad, IDictionary<int, long>> adDispatchPlans, ICollection<SerializerMapping> creativeSerializerMappings);
    }
    public interface IRunDeliveryPolicyMetadata:IProcesserMetadata
    {
    }
}
