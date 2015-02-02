using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    public class FrequencyControl
    {
        public Guid FrequencyControlId { get; set; }
        public ICollection<FrequencyControlDetail> FrequencyControlDetails { get; set; }
    }
}
