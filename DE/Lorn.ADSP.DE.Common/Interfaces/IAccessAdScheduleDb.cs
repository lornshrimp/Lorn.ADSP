using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 广告投放计划数据库访问接口
    /// </summary>
    public interface IAccessAdScheduleDb
    {
        IDictionary<Guid,IEnumerable<AdSpotPlan>> GetAdSpotPlans();
        IDictionary<Guid, IDictionary<Guid, string>> GetMonitorTypeMappings();
        IDictionary<Guid, IDictionary<Guid, AdPosition>> GetAdPositions();
        IDictionary<Guid, IDictionary<Guid, AdPositionGroup>> GetAdPositionGroups();
        IDictionary<Guid, IDictionary<string, Guid>> GetAdPositionAndAdPositionGroupCodeMappings();
        IDictionary<Guid, IDictionary<Guid, DeliveryPiplineConfiguration>> GetDeliveryPiplineConfigurations();
        IDictionary<Guid, FlowData> GetFlowControls();
        IDictionary<Guid, Guid> GetMediaSecureKeys();
        /// <summary>
        /// 获取定向维度信息
        /// </summary>
        /// <returns>媒体、定向维度Id，定向维度</returns>
        IDictionary<Guid, IDictionary<Guid, RedirectDimension>> GetRedirectConditionDefinitions();
        /// <summary>
        /// 获取Ip库
        /// </summary>
        /// <returns></returns>
        IDictionary<Guid, IDictionary<Guid, IpLibrary>> GetIpLibraries();

        IDictionary<Guid, ICollection<SerializerMapping>> GetCreativeSerializerMappings();

        IDictionary<Guid, ICollection<SerializerMapping>> GetAdPositionReleaseSerializerMappings();

        IDictionary<Guid, ICollection<SerializerMapping>> GetAdStacksSerializerMappings();
    }
}
