using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    public class AdMonitorRecord
    {
        public string MonitorTypeCode { get; set; }
        public string RequestUrl { get; set; }
        public Guid MediaId { get; set; }
        public Guid CookieId { get; set; }
        public Guid SessionId { get; set; }
        public Guid ViewId { get; set; }
        public Guid RequestId { get; set; }
        public Guid AdSpotPlanId { get; set; }
        public Guid AdMasterPlanId { get; set; }
        public Guid AdSpotPlanEditionId { get; set; }
        public Guid AdSpotPlanGroupId { get; set; }
        public Guid MaterialId { get; set; }
        public Guid AdPositionId { get; set; }
        IDictionary<string, string> Parameters { get; set; }
    }
}
