using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.Interfaces;
using Lorn.ADSP.DE.DataModels;
using System.ComponentModel.Composition;


namespace Lorn.ADSP.DE.DeliveryCache
{
    [Export(typeof(ICacheAdDispatchPlans))]
    [Export(typeof(ICacheAdPositionAndAdPositionGroups))]
    [Export(typeof(ICacheDeliveryPiplineConfigurations))]
    [Export(typeof(ICacheMediaSecureKeys))]
    [Export(typeof(ICacheRedirectConditionDefinitions))]
    [Export(typeof(ICacheSerializerMappings))]
    public class DeliveryCacheRuntime : IDisposable, ICacheAdDispatchPlans, ICacheAdPositionAndAdPositionGroups, ICacheDeliveryPiplineConfigurations, ICacheMediaSecureKeys, ICacheRedirectConditionDefinitions, ICacheSerializerMappings
    {
        IDictionary<string, IDictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>>> adDispatchPlans;
        IDictionary<Guid, IDictionary<Guid, AdPosition>> adPositions;
        IDictionary<Guid, IDictionary<Guid, AdPositionGroup>> adPositionGroups;
        IDictionary<Guid, IDictionary<string, Guid>> adPositionAndAdPositionGroupCodeMappings;
        IDictionary<Guid, IDictionary<Guid, DeliveryPiplineConfiguration>> deliveryPiplineConfigurations;
        IDictionary<Guid, Guid> meidaSecureKeys;
        IDictionary<Guid, IDictionary<Guid, RedirectDimension>> redirectConditionDefinitions;
        IDictionary<Guid, IDictionary<Guid, IpLibrary>> ipLibraries;
        IDictionary<Guid, ICollection<SerializerMapping>> creativeSerializerMappings;
        IDictionary<Guid, ICollection<SerializerMapping>> adPositionReleaseSerializerMappings;
        IDictionary<Guid, ICollection<SerializerMapping>> adStacksSerializerMapping;
        #region IDisposable 成员

        public void Dispose()
        {
   
        }

        #endregion

        #region ICacheAdDispatchPlans 成员

        public IDictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>> GetCachedAdDispatchPlans(string serverId)
        {
            return adDispatchPlans[serverId];
        }

        public void SetAdDispatchPlansCache(IDictionary<string, IDictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>>> adDispatchPlans)
        {
            this.adDispatchPlans = adDispatchPlans;
        }

        #endregion

        #region ICacheAdPositionAndAdPositionGroups 成员

        public void SetAdPositions(IDictionary<Guid, IDictionary<Guid, AdPosition>> adPositions)
        {
            this.adPositions = adPositions;
        }

        public IDictionary<Guid, IDictionary<Guid, AdPosition>> GetAdPositions()
        {
            return this.adPositions;
        }

        public void SetAdPositionGroups(IDictionary<Guid, IDictionary<Guid, AdPositionGroup>> adPositionGroups)
        {
            this.adPositionGroups = adPositionGroups;
        }

        public IDictionary<Guid, IDictionary<Guid, AdPositionGroup>> GetAdPositionGroups()
        {
            return this.adPositionGroups;
        }

        public void SetAdPositionAndAdPositionGroupCodeMappings(IDictionary<Guid, IDictionary<string, Guid>> mappings)
        {
            this.adPositionAndAdPositionGroupCodeMappings=mappings;
        }

        public IDictionary<Guid, IDictionary<string, Guid>> GetAdPositionAndAdPositionGroupCodeMappings()
        {
            return this.adPositionAndAdPositionGroupCodeMappings;
        }

        #endregion

        #region ICacheDeliveryPiplineConfigurations 成员

        public void SetDeliveryPiplineConfigurations(IDictionary<Guid, IDictionary<Guid, DeliveryPiplineConfiguration>> deliveryPiplineConfigurations)
        {
            this.deliveryPiplineConfigurations = deliveryPiplineConfigurations;
        }

        public IDictionary<Guid, IDictionary<Guid, DeliveryPiplineConfiguration>> GetDeliveryPiplineConfigurations()
        {
            return this.deliveryPiplineConfigurations;
        }

        #endregion

        #region ICacheMediaSecureKeys 成员

        public void SetMediaSecureKeys(IDictionary<Guid, Guid> meidaSecureKeys)
        {
            this.meidaSecureKeys = meidaSecureKeys;
        }

        public IDictionary<Guid, Guid> GetMediaSecureKeys()
        {
            return this.meidaSecureKeys;
        }

        #endregion

        #region ICacheRedirectConditionDefinitions 成员

        public void SetRedirectConditionDefinitions(IDictionary<Guid, IDictionary<Guid, RedirectDimension>> redirectConditionDefinitions)
        {
            this.redirectConditionDefinitions = redirectConditionDefinitions;
        }

        public IDictionary<Guid, IDictionary<Guid, RedirectDimension>> GetRedirectConditionDefinitions()
        {
            return this.redirectConditionDefinitions;
        }

        public void SetIpLibraries(IDictionary<Guid, IDictionary<Guid, IpLibrary>> ipLibraries)
        {
            this.ipLibraries = ipLibraries;
        }

        public IDictionary<Guid, IDictionary<Guid, IpLibrary>> GetIpLibraries()
        {
            return this.ipLibraries;
        }

        #endregion

        #region ICacheSerializerMappings 成员

        public void SetCreativeSerializerMappings(IDictionary<Guid, ICollection<SerializerMapping>> mappings)
        {
            this.creativeSerializerMappings = mappings;
        }

        public IDictionary<Guid, ICollection<SerializerMapping>> GetCreativeSerializerMappings()
        {
            return this.creativeSerializerMappings;
        }

        public void SetAdPositionReleaseSerializerMappings(IDictionary<Guid, ICollection<SerializerMapping>> mappings)
        {
            this.adPositionReleaseSerializerMappings = mappings;
        }

        public IDictionary<Guid, ICollection<SerializerMapping>> GetAdPositionReleaseSerializerMappings()
        {
            return this.adPositionReleaseSerializerMappings;
        }

        public void SetAdStacksSerializerMapping(IDictionary<Guid, ICollection<SerializerMapping>> mappings)
        {
            this.adStacksSerializerMapping = mappings;
        }

        public IDictionary<Guid, ICollection<SerializerMapping>> GetAdStacksSerializerMappings()
        {
            return this.adStacksSerializerMapping;
        }

        #endregion

    }
}
