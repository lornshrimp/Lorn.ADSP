using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ICacheDeliveryPiplineConfigurations
    {
        void SetDeliveryPiplineConfigurations(IDictionary<Guid,IDictionary<Guid, DeliveryPiplineConfiguration>> deliveryPiplineConfigurations);
        IDictionary<Guid,IDictionary<Guid, DeliveryPiplineConfiguration>> GetDeliveryPiplineConfigurations();
    }
}
