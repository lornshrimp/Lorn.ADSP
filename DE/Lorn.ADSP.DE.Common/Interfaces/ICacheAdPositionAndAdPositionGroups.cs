using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ICacheAdPositionAndAdPositionGroups
    {
        void SetAdPositions(IDictionary<Guid,IDictionary<Guid,AdPosition>> adPositions);
        IDictionary<Guid, IDictionary<Guid, AdPosition>> GetAdPositions();
        void SetAdPositionGroups(IDictionary<Guid, IDictionary<Guid, AdPositionGroup>> adPositionGroups);
        IDictionary<Guid, IDictionary<Guid, AdPositionGroup>> GetAdPositionGroups();

        void SetAdPositionAndAdPositionGroupCodeMappings(IDictionary<Guid,IDictionary<string, Guid>> mappings);
        IDictionary<Guid, IDictionary<string, Guid>> GetAdPositionAndAdPositionGroupCodeMappings();
    }
}
