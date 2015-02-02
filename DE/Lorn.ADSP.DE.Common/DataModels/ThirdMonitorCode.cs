using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.Common.DataModels;

namespace Lorn.ADSP.DE.DataModels
{
    public class ThirdMonitorCode
    {
        public string MonitorCode { get; set; }
        public virtual AdMaterial AdMaterial { get; set; }
        public string MonitorTypeCode { get; set; }
    }
}
