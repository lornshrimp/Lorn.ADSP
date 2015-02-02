using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface IReportAdMonitor
    {
        void RequestAdMonitor(string monitorTypeCode, string requestUrl, Guid mediaId, Guid cookieId, Guid sessionId, Guid viewId, Guid requestId, Guid? adPositionId = null, int? playOder = null, Guid? adSpotPlanId = null, Guid? adMasterPlanId = null, Guid? adSpotPlanEditionId = null, Guid? adSpotplanGroupId = null, Guid? materialId = null, IDictionary<string, string> parameters = null, Guid? deliveryPiplineConfigurationId = null, float? deliveryPiplineConfigurationVersion = null, ICollection<Guid> filteredAds = null, IDictionary<string, object> extensionDatas = null);
    }
}
