using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    public class AdMaterialFormInfo
    {
        public System.Guid CreativeTypeFormdefinitionId { get; set; }
        public string Value { get; set; }
        public Nullable<int> SortNo { get; set; }

        public virtual ICollection<AdMaterialFormInfo> ChildAdMaterialFormInfos { get; set; } 
    }
}
