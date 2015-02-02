using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 缓存序列化器映射
    /// </summary>
    public interface ICacheSerializerMappings
    {
        void SetCreativeSerializerMappings(IDictionary<Guid,ICollection<SerializerMapping>> mappings);
        IDictionary<Guid,ICollection<SerializerMapping>> GetCreativeSerializerMappings();

        void SetAdPositionReleaseSerializerMappings(IDictionary<Guid,ICollection<SerializerMapping>> mappings);
        IDictionary<Guid,ICollection<SerializerMapping>> GetAdPositionReleaseSerializerMappings();

        void SetAdStacksSerializerMapping(IDictionary<Guid,ICollection<SerializerMapping>> mappings);
        IDictionary<Guid,ICollection<SerializerMapping>> GetAdStacksSerializerMappings();
    }
}
