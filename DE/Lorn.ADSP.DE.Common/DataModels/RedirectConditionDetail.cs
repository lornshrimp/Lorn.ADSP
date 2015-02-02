using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    public class RedirectConditionDetail
    {
        public Guid? RedirectConditionDefinitionId { get; set; }
        public Nullable<double> StartValue { get; set; }
        public Nullable<double> EndValue { get; set; }
        public string CustomKey { get; set; }
        public string CustomValue { get; set; }
    }
}
