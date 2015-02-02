using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ISerializeAdBreak
    {
        string SerializeAdBreak(AdPositionReleaseInfo adPositionRelease, Queue<string> adMaterialReleases, IDictionary<string, string> extensions = null);
    }
}
