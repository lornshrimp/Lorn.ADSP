using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ISerializeAdBreaksSet
    {
        string SerializeAdBreaksSet(IDictionary<AdPositionReleaseInfo, string> adBreaks, IDictionary<string, string> extensions = null);
    }
}
