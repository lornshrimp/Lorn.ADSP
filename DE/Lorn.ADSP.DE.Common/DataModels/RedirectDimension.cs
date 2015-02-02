using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    public class RedirectDimension
    {
        public System.Guid RedirectDimensionId { get; set; }
        public string RedirectDimensionCode { get; set; }
        public virtual ICollection<RedirectConditionDefinition> RedirectConditionDefinitions { get; set; }
    }
}
