using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.Common.DataModels;

namespace Lorn.ADSP.DE.DataModels
{
    public class AdSpotPlan:Ad
    {
        public int PlanTrafficRatio { get; set; }
        public long PlanImpressionNumber { get; set; }
        public long PlanClickNumber { get; set; }
        public Guid ValuationMonitorTypeId { get; set; }

        /// <summary>
        /// 消耗方式
        /// </summary>
        public ConsumeType ComsumeType { get; set; }
    }
}
