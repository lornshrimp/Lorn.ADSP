using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ISelectSerializer
    {
        KeyValuePair<Guid,float> SelectSerializer(IDictionary<string, string> parameters, ICollection<SerializerMapping> SerializerMappings, Guid? adPositionId = null,Guid? creativeTypeId = null);
    }
    public interface ISelectSerializerMetadata
    {
        SerializerType SerializerSelectType { get; }
    }
}
