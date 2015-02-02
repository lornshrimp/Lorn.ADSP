using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface IFilterAdMaterial
    {
        AdMaterialReleaseInfo FilterAdMaterial(UserMediaCookie cookie, IDictionary<string, string> parameters, Ad filteredAd, AdProcesserConfiguration filterConfiguration, IDictionary<string, object> additionalParameters = null);
    }
}
