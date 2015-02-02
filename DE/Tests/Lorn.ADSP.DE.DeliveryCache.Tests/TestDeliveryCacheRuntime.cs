using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lorn.ADSP.DE.DataModels;
using Lorn.ADSP.DE.DeliveryCache;
using Lorn.ADSP.DE.Interfaces;
using System.Collections.Generic;

namespace Lorn.ADSP.DE.DeliveryCache.Tests
{
    [TestClass]
    public class TestDeliveryCacheRuntime
    {
        [TestMethod]
        public void TestICacheAdDispatchPlans()
        {
            ICacheAdDispatchPlans cache = new DeliveryCacheRuntime();
            IDictionary<string, IDictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>>> adDispatchPlans = new Dictionary<string, IDictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>>>();
            string serverId = Guid.NewGuid().ToString();
            var expected = new Dictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>>();
            adDispatchPlans[serverId] = expected;
            cache.SetAdDispatchPlansCache(adDispatchPlans);
            var actual = cache.GetCachedAdDispatchPlans(serverId);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheAdPositions()
        {
            ICacheAdPositionAndAdPositionGroups cache = new DeliveryCacheRuntime();
            IDictionary<Guid, IDictionary<Guid, AdPosition>> adPositions = new Dictionary<Guid, IDictionary<Guid, AdPosition>>(); ;
            var expected = adPositions;
            cache.SetAdPositions(adPositions);
            var actual = cache.GetAdPositions();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheAdPositionGroups()
        {
            ICacheAdPositionAndAdPositionGroups cache = new DeliveryCacheRuntime();
            IDictionary<Guid, IDictionary<Guid, AdPositionGroup>> adPositionGroups = new Dictionary<Guid, IDictionary<Guid, AdPositionGroup>>();
            var expected = adPositionGroups;
            cache.SetAdPositionGroups(adPositionGroups);
            var actual = cache.GetAdPositionGroups();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheAdPositionAndAdPositionGroupCodeMappings()
        {
            ICacheAdPositionAndAdPositionGroups cache = new DeliveryCacheRuntime();
            IDictionary<Guid, IDictionary<string, Guid>> adPositionAndAdPositionGroupCodeMappings = new Dictionary<Guid, IDictionary<string, Guid>>();
            var expected = adPositionAndAdPositionGroupCodeMappings;
            cache.SetAdPositionAndAdPositionGroupCodeMappings(adPositionAndAdPositionGroupCodeMappings);
            var actual = cache.GetAdPositionAndAdPositionGroupCodeMappings();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheDeliveryPiplineConfigurations()
        {
            ICacheDeliveryPiplineConfigurations cache = new DeliveryCacheRuntime();
            IDictionary<Guid, IDictionary<Guid, DeliveryPiplineConfiguration>> deliveryPiplineConfigurations = new Dictionary<Guid, IDictionary<Guid, DeliveryPiplineConfiguration>>();
            var expected = deliveryPiplineConfigurations;
            cache.SetDeliveryPiplineConfigurations(deliveryPiplineConfigurations);
            var actual = cache.GetDeliveryPiplineConfigurations();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheMediaSecureKeys()
        {
            ICacheMediaSecureKeys cache = new DeliveryCacheRuntime();
            IDictionary<Guid, Guid> meidaSecureKeys = new Dictionary<Guid, Guid>();
            var expected = meidaSecureKeys;
            cache.SetMediaSecureKeys(meidaSecureKeys);
            var actual = cache.GetMediaSecureKeys();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheRedirectConditionDefinitions()
        {
            ICacheRedirectConditionDefinitions cache = new DeliveryCacheRuntime();
            IDictionary<Guid, IDictionary<Guid, RedirectDimension>> redirectConditionDefinitions = new Dictionary<Guid, IDictionary<Guid, RedirectDimension>>();
            var expected = redirectConditionDefinitions;
            cache.SetRedirectConditionDefinitions(redirectConditionDefinitions);
            var actual = cache.GetRedirectConditionDefinitions();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheIpLibraries()
        {
            ICacheRedirectConditionDefinitions cache = new DeliveryCacheRuntime();
            IDictionary<Guid, IDictionary<Guid, IpLibrary>> ipLibraries = new Dictionary<Guid, IDictionary<Guid, IpLibrary>>();
            var expected = ipLibraries;
            cache.SetIpLibraries(ipLibraries);
            var actual = cache.GetIpLibraries();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheCreativeSerializerMappings()
        {
            ICacheSerializerMappings cache = new DeliveryCacheRuntime();
            IDictionary<Guid, ICollection<SerializerMapping>> creativeSerializerMappings = new Dictionary<Guid, ICollection<SerializerMapping>>();
            var expected = creativeSerializerMappings;
            cache.SetCreativeSerializerMappings(creativeSerializerMappings);
            var actual = cache.GetCreativeSerializerMappings();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheAdPositionReleaseSerializerMappings()
        {
            ICacheSerializerMappings cache = new DeliveryCacheRuntime();
            IDictionary<Guid, ICollection<SerializerMapping>> adPositionReleaseSerializerMappings = new Dictionary<Guid, ICollection<SerializerMapping>>();
            var expected = adPositionReleaseSerializerMappings;
            cache.SetAdPositionReleaseSerializerMappings(adPositionReleaseSerializerMappings);
            var actual = cache.GetAdPositionReleaseSerializerMappings();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestICacheAdStacksSerializerMapping()
        {
            ICacheSerializerMappings cache = new DeliveryCacheRuntime();
            IDictionary<Guid, ICollection<SerializerMapping>> adStacksSerializerMapping = new Dictionary<Guid, ICollection<SerializerMapping>>();
            var expected = adStacksSerializerMapping;
            cache.SetAdStacksSerializerMapping(adStacksSerializerMapping);
            var actual = cache.GetAdStacksSerializerMappings();
            Assert.AreEqual(expected, actual);
        }
    }
}
