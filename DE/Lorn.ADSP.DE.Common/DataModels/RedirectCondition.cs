using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    public class RedirectCondition
    {
        /// <summary>
        /// 正定向明细
        /// </summary>
        public virtual ICollection<RedirectConditionDetail> IncludeRedirctConditionDetails { get; set; }
        /// <summary>
        /// 反定向明细
        /// </summary>
        public virtual ICollection<RedirectConditionDetail> ExcludeRedirctConditionDetails { get; set; }
    }
}
