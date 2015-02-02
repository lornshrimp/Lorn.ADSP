using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    public class RedirectConditionDefinition
    {
        public Guid? MediaId { get; set; }
        public System.Guid RedirectConditionDefinitionId { get; set; }
        public string ConditionCode { get; set; }
        public long? Value { get; set; }
        public virtual ICollection<RedirectConditionDefinition> ChildRedirectConditionDefinitions { get; set; }
    }
}
