using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ISerializeAd
    {
        string SerializeAd(AdMaterialReleaseInfo adMaterialRelease, ICollection<SerializerMapping> creativeSerializerMappings);
    }
    public interface ISerializerMetadata
    {
        Guid SerializerId { get; }
        string SerializerName { get; }
        string Description { get; }
        float Version { get; }
    }
}
