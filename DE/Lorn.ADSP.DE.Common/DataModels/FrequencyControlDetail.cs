using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    public class FrequencyControlDetail
    {
        public Lorn.ADSP.Common.DataModels.FrequencyControlPeriod FrequencyControlPeriod { get; set; }
        public int PeriodNumber { get; set; }
        public Nullable<int> MaxCount { get; set; }
        public Nullable<int> MinCount { get; set; }
    }
}
