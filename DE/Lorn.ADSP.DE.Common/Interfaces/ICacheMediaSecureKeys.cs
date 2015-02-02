using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ICacheMediaSecureKeys
    {
        void SetMediaSecureKeys(IDictionary<Guid,Guid> meidaSecureKeys);
        IDictionary<Guid, Guid> GetMediaSecureKeys();
    }
}
