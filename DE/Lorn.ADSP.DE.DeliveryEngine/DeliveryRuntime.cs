using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.Interfaces;
using System.ComponentModel.Composition;
using Lorn.ADSP.DE.DataModels;
using System.Collections;
using Lorn.ADSP.Common.Interfaces;
using System.Timers;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading;

namespace Lorn.ADSP.DE.DeliveryEngine
{
    [Export(typeof(IRequestAd))]
    public class DeliveryRuntime : IRequestAd, IDisposable
    {
        protected IDictionary<Guid, IDictionary<Guid, DeliveryPiplineConfiguration>> deliveryPiplineConfigurations;
        protected IDictionary<Guid, IDictionary<Guid, RedirectDimension>> redirectConditionDefinitions;
        protected IDictionary<Guid, IDictionary<Guid, AdPosition>> adPositions;
        protected IDictionary<Guid, IDictionary<Guid, AdPositionGroup>> adPositionGroups;
        protected IDictionary<Guid, IDictionary<string, Guid>> adpAndAdpgCodeMappings;
        protected IDictionary<DateTime, IDictionary<Guid, IDictionary<Ad, IDictionary<int, long>>>> adDispatchPlans;
        protected IDictionary<string, object> serviceConfigurations;
        protected IDictionary<Guid, ICollection<SerializerMapping>> creativeSerializerMappings;
        protected IDictionary<Guid, ICollection<SerializerMapping>> adPositionReleaseSerializerMappings;
        protected IDictionary<Guid, ICollection<SerializerMapping>> adStacksSerializerMappings;
        protected IDictionary<Guid, IDictionary<Guid, IpLibrary>> ipLibraries;

        protected DateTime lastReadCacheTime;
        protected DateTime lastReorganizeAdStacksTime;
        protected DateTime lastReportHealthTime;
        protected bool readingCache = false;
        protected bool reorganizingAdStacks = false;
        protected string serverId;

        protected System.Timers.Timer timer;

        

        [ImportMany(typeof(IRunDeliveryPolicy), RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<Lazy<IRunDeliveryPolicy, IRunDeliveryPolicyMetadata>> DeliveryPolicies { get; set; }
        [Import(typeof(IAccessUserCookie), RequiredCreationPolicy = CreationPolicy.Shared)]
        public IAccessUserCookie UserCookieAccesser { get; set; }
        [Import(typeof(ITime), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ITime Time { get; set; }
        [Import(typeof(IReportAdMonitor), RequiredCreationPolicy = CreationPolicy.Shared)]
        public IReportAdMonitor AdMonitorReporter { get; set; }
        [ImportMany(typeof(ISerializeAd), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICollection<Lazy<ISerializeAd, ISerializerMetadata>> AdSerializers { get; set; }
        [ImportMany(typeof(ISerializeAdBreaksSet), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICollection<Lazy<ISerializeAdBreaksSet, ISerializerMetadata>> AdBreaksSerializers { get; set; }
        [Import(typeof(ICacheServiceConfigurations), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheServiceConfigurations ServiceConfigurationCacher { get; set; }
        [Import(typeof(ICacheDeliveryPiplineConfigurations), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheDeliveryPiplineConfigurations DeliveryPiplineConfigurationsCacher { get; set; }
        [Import(typeof(ICacheRedirectConditionDefinitions), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheRedirectConditionDefinitions RedirectConditionDefinitionsCacher { get; set; }
        [Import(typeof(ICacheAdPositionAndAdPositionGroups), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheAdPositionAndAdPositionGroups AdpInfoCacher { get; set; }
        [Import(typeof(ICacheAdDispatchPlans), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheAdDispatchPlans AdDispatchPlansCacher { get; set; }
        [Import(typeof(IMonitorServiceStatus), RequiredCreationPolicy = CreationPolicy.Shared)]
        public IMonitorServiceStatus ServiceStatusMoniter { get; set; }
        [Import(typeof(IReorganizeAdStacks), RequiredCreationPolicy = CreationPolicy.Shared)]
        public IReorganizeAdStacks AdStacksReorganizer { get; set; }
        [ImportMany(typeof(ISerializeAdBreak), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICollection<Lazy<ISerializeAdBreak, ISerializerMetadata>> AdPositionRelaseSerializers { get; set; }
        [ImportMany(typeof(ISelectSerializer), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICollection<Lazy<ISelectSerializer, ISelectSerializerMetadata>> SerializerSelectors { get; set; }
        [Import(typeof(ICacheSerializerMappings), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheSerializerMappings SerializerMappingsCacher { get; set; }

        public DeliveryRuntime()
        {
            lastReadCacheTime = Time.Now;
            lastReorganizeAdStacksTime = Time.Now;
            lastReportHealthTime = Time.Now;

            this.serverId = ConfigurationManager.AppSettings["ServerId"];
            this.ServiceStatusMoniter.InitService(ConstStrings.STR_DELIVERYRUNTIMESERVICENAME, serverId);

            this.adDispatchPlans = new ConcurrentDictionary<DateTime, IDictionary<Guid, IDictionary<Ad, IDictionary<int, long>>>>();
            var dispatchServiceStatuses = this.ServiceStatusMoniter.GetServerStatuses(ConstStrings.STR_DISPATCHSERVICENAME);
            while (dispatchServiceStatuses.Count > 0 && dispatchServiceStatuses.First().Value.Key != ServiceStatuses.Running)
            {
                Thread.Sleep(1000);
                dispatchServiceStatuses = this.ServiceStatusMoniter.GetServerStatuses(ConstStrings.STR_DISPATCHSERVICENAME);
            }

            ReadCache();

            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 1000;
            timer.Start();

            this.ServiceStatusMoniter.InitServiceComplete(ConstStrings.STR_DELIVERYRUNTIMESERVICENAME, serverId);
        }
        protected void ReadCache()
        {
            readingCache = true;

            var servicecfg = this.ServiceConfigurationCacher.GetServiceConfiguration(ConstStrings.STR_DELIVERYRUNTIMESERVICENAME, this.serverId);
            if (servicecfg != null)
            {
                this.serviceConfigurations = servicecfg;
            }

            var deliveryPiplinecfg = this.DeliveryPiplineConfigurationsCacher.GetDeliveryPiplineConfigurations();
            if (deliveryPiplinecfg != null)
            {
                this.deliveryPiplineConfigurations = deliveryPiplinecfg;
            }

            var redirectConditiondef = this.RedirectConditionDefinitionsCacher.GetRedirectConditionDefinitions();
            if (redirectConditiondef != null)
            {
                this.redirectConditionDefinitions = redirectConditiondef;
            }

            var adp = this.AdpInfoCacher.GetAdPositions();
            if (adp != null)
            {
                this.adPositions = adp;
            }

            var adpg = this.AdpInfoCacher.GetAdPositionGroups();
            if (adpg != null)
            {
                this.adPositionGroups = adpg;
            }

            var adpmapping = this.AdpInfoCacher.GetAdPositionAndAdPositionGroupCodeMappings();
            if (adpmapping != null)
            {
                this.adpAndAdpgCodeMappings = adpmapping;
            }

            var csMapping = this.SerializerMappingsCacher.GetCreativeSerializerMappings();
            if (csMapping != null)
            {
                this.creativeSerializerMappings = csMapping;
            }

            var adprsMapping = this.SerializerMappingsCacher.GetAdPositionReleaseSerializerMappings();
            if (adprsMapping != null)
            {
                this.adPositionReleaseSerializerMappings = adprsMapping;
            }

            var adssmapping = this.SerializerMappingsCacher.GetAdStacksSerializerMappings();
            if (adssmapping != null)
            {
                this.adStacksSerializerMappings = adssmapping;
            }



            var cachedAdPlans = this.AdDispatchPlansCacher.GetCachedAdDispatchPlans(this.serverId);

            InitAdDispatchPlan(cachedAdPlans);

            readingCache = true;
        }

        private void InitAdDispatchPlan(IDictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>> cachedAdPlans)
        {
            if (cachedAdPlans != null)
            {
                var adPlans = new ConcurrentDictionary<DateTime, IDictionary<Guid, IDictionary<Ad, IDictionary<int, long>>>>();

                foreach (var cachedMediaAdPlan in cachedAdPlans)
                {
                    var mediaId = cachedMediaAdPlan.Key;
                    //处理每个时间点的投放计划
                    foreach (var cachedAdPlan in cachedMediaAdPlan.Value)
                    {
                        var releaseTime = cachedAdPlan.Key;
                        if (!adPlans.ContainsKey(releaseTime))
                        {
                            adPlans[releaseTime] = new ConcurrentDictionary<Guid, IDictionary<Ad, IDictionary<int, long>>>();
                        }
                        var timeAdPlan = adPlans[releaseTime];
                        if (!timeAdPlan.ContainsKey(mediaId))
                        {
                            timeAdPlan[mediaId] = new ConcurrentDictionary<Ad, IDictionary<int, long>>();
                        }
                        var mediaAdPlan = timeAdPlan[mediaId];

                        //将广告投放计划放到第1贴片
                        foreach (var item in cachedAdPlan.Value)
                        {
                            if (!mediaAdPlan.ContainsKey(item.Key))
                            {
                                mediaAdPlan[item.Key] = new ConcurrentDictionary<int, long>();
                            }
                            //TODO:对于贴片位置定向需要进行处理
                            mediaAdPlan[item.Key][0] = item.Value;
                        }
                    }
                }

                this.adDispatchPlans = adPlans;
            }
        }

        protected void ReorganizeAdStacks()
        {
            reorganizingAdStacks = true;
            this.AdStacksReorganizer.ReorganizeAdStacks(this.adDispatchPlans, Time.Now);
            reorganizingAdStacks = false;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int reportHealthInterval = 1;
            if (this.serviceConfigurations.ContainsKey(ConstStrings.STR_REPORTHEALTHINTERVAL))
            {
                reportHealthInterval = (int)this.serviceConfigurations[ConstStrings.STR_REPORTHEALTHINTERVAL];
            }

            int readCacheInterval = 5;
            if (this.serviceConfigurations.ContainsKey(ConstStrings.STR_READCACHEINTERVAL))
            {
                readCacheInterval = (int)this.serviceConfigurations[ConstStrings.STR_READCACHEINTERVAL];
            }
            int reorganizingAdStacksInterval = 1;
            if (this.serviceConfigurations.ContainsKey(ConstStrings.STR_REORGANIZEADSTACKSINTERVAL))
            {
                reorganizingAdStacksInterval = (int)this.serviceConfigurations[ConstStrings.STR_REORGANIZEADSTACKSINTERVAL];
            }

            DateTime now = Time.Now;
            if (now > lastReportHealthTime.AddMinutes(reportHealthInterval))
            {
                this.ServiceStatusMoniter.ReportStatus(ConstStrings.STR_DELIVERYRUNTIMESERVICENAME, this.serverId);
            }
            if (now > lastReadCacheTime.AddMinutes(readCacheInterval) && readingCache == false)
            {
                ReadCache();
            }
            else if (now > lastReorganizeAdStacksTime.AddMinutes(reorganizingAdStacksInterval) && reorganizingAdStacks == false)
            {
                ReorganizeAdStacks();
            }
        }

        #region IRequestAd 成员

        public string RequestAd(string requestUrl, ICollection<string> adPositionOrAdPositionGroupCodes, Guid mediaId, Guid cookieId, Guid sessionId, Guid viewId, Guid requestId, IDictionary<string, string> parameters)
        {
            var currentTime = Time.Now;
            var currentTime2 = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0);
            //发送广告请求监测
            this.AdMonitorReporter.RequestAdMonitor(ConstStrings.STR_REQUESTAD, requestUrl, mediaId, cookieId, sessionId, viewId, requestId, parameters: parameters);

            //查找用户Cookie
            var cookie = UserCookieAccesser.GetUserCookie(cookieId, mediaId);
            var filteredAds = new List<IDictionary<AdPositionReleaseInfo, Queue<AdMaterialReleaseInfo>>>();

            //筛选广告
            foreach (var adPositionOrAdPositionGroupCode in adPositionOrAdPositionGroupCodes)
            {
                if (adpAndAdpgCodeMappings.ContainsKey(mediaId) && adpAndAdpgCodeMappings[mediaId] != null && adpAndAdpgCodeMappings[mediaId].ContainsKey(adPositionOrAdPositionGroupCode))
                {
                    Guid adpOrAdpgId = adpAndAdpgCodeMappings[mediaId][adPositionOrAdPositionGroupCode];
                    if (deliveryPiplineConfigurations.ContainsKey(adpOrAdpgId))
                    {
                        var piplineConfiguration = deliveryPiplineConfigurations[mediaId][adpOrAdpgId];
                        IRunDeliveryPolicy policy = this.DeliveryPolicies.First(o => o.Metadata.ProcesserId == piplineConfiguration.AdDeliveryPolicy.ProcesserId && o.Metadata.Version >= piplineConfiguration.AdDeliveryPolicy.ProcesserMinVersion).Value;
                        IDictionary<Ad, IDictionary<int, long>> dispatchPlans = null;
                        if (adDispatchPlans != null && adDispatchPlans.ContainsKey(currentTime2) && adDispatchPlans[currentTime2].ContainsKey(mediaId))
                        {
                            dispatchPlans = adDispatchPlans[currentTime2][mediaId];
                        }
                        var adQueue = policy.RunDeliveryPolicy(adpOrAdpgId, cookie, parameters, deliveryPiplineConfigurations[mediaId], adPositions[mediaId], adPositionGroups[mediaId], redirectConditionDefinitions[mediaId], ipLibraries[mediaId], dispatchPlans, this.creativeSerializerMappings[mediaId]);
                        if (adQueue != null)
                        {
                            filteredAds.Add(adQueue);
                        }
                    }
                }

            }
            //对广告队列进行按广告位整理
            var returnAdQueue = new Dictionary<AdPositionReleaseInfo, Queue<AdMaterialReleaseInfo>>();
            foreach (var item in filteredAds)
            {
                foreach (var item2 in item)
                {
                    if (!returnAdQueue.ContainsKey(item2.Key))
                    {
                        returnAdQueue[item2.Key] = new Queue<AdMaterialReleaseInfo>();
                    }
                    var adQueue = returnAdQueue[item2.Key];
                    while (item2.Value.Count > 0)
                    {
                        var adInfo = item2.Value.Dequeue();
                        adQueue.Enqueue(adInfo);
                    }
                }
            }

            //序列化
            var serializedAdPositionReleases = new Dictionary<AdPositionReleaseInfo, string>();
            //首先按广告位进行序列化
            foreach (var adPositionKeyValuePair in returnAdQueue)
            {
                var adPositionSerializerSelector = this.SerializerSelectors.First(o => o.Metadata.SerializerSelectType == SerializerType.AdPositionReleaseSerializer).Value;
                var adPositionSerializerInfo = adPositionSerializerSelector.SelectSerializer(parameters, this.adPositionReleaseSerializerMappings[mediaId]);
                var adPositionSerializer = this.AdPositionRelaseSerializers.First(o => o.Metadata.SerializerId == adPositionSerializerInfo.Key && o.Metadata.Version >= adPositionSerializerInfo.Value).Value;
                var QueueString = new Queue<string>();
                while (adPositionKeyValuePair.Value.Count > 0)
                {
                    var currentSequenceNo = QueueString.Count;
                    var adMaterial = adPositionKeyValuePair.Value.Dequeue();
                    //对广告返回信息进行序列化
                    var adSerializerSelector = this.SerializerSelectors.First(o => o.Metadata.SerializerSelectType == SerializerType.AdSerializer).Value;
                    var adSerializerInfo = adSerializerSelector.SelectSerializer(parameters, this.creativeSerializerMappings[mediaId], adPositionKeyValuePair.Key.AdPositionId, adMaterial.CreativeTypeId);
                    var adSerializer = this.AdSerializers.First(o => o.Metadata.SerializerId == adSerializerInfo.Key && o.Metadata.Version >= adSerializerInfo.Value).Value;
                    var serializedAd = adSerializer.SerializeAd(adMaterial, this.creativeSerializerMappings[mediaId]);
                    //扣减待投量，如果待投量大于零说明是非流量比例释放，可以扣减
                    if (this.adDispatchPlans[currentTime2][mediaId][adMaterial.Ad][currentSequenceNo] > 0)
                    {
                        this.adDispatchPlans[currentTime2][mediaId][adMaterial.Ad][currentSequenceNo]--;
                    }
                    //发送广告返回监测
                    this.AdMonitorReporter.RequestAdMonitor(ConstStrings.STR_RETURNAD, requestUrl, mediaId, cookieId, sessionId, viewId, requestId, adPositionKeyValuePair.Key.AdPositionId, currentSequenceNo, adMaterial.Ad.AdSpotPlanId, adMaterial.Ad.AdMasterPlanId, adMaterial.Ad.AdSpotPlanEditionId, adMaterial.Ad.SpotPlanGroupId, adMaterial.MaterialId, parameters, adMaterial.DeliveryPiplineConfigurationId, adMaterial.DeliveryPiplineConfigurationVersion);
                    QueueString.Enqueue(serializedAd);

                }
                var serializedAdPositionRelease = adPositionSerializer.SerializeAdBreak(adPositionKeyValuePair.Key, QueueString);
                serializedAdPositionReleases[adPositionKeyValuePair.Key] = serializedAdPositionRelease;
            }
            //然后进行全局序列化
            var adBreaksSerializerSelector = this.SerializerSelectors.First(o => o.Metadata.SerializerSelectType == SerializerType.AdBreaksSerializer).Value;
            var adBreaksSerializerInfo = adBreaksSerializerSelector.SelectSerializer(parameters, this.adStacksSerializerMappings[mediaId]);
            var adBreaksSerializer = this.AdBreaksSerializers.First(o => o.Metadata.SerializerId == adBreaksSerializerInfo.Key && o.Metadata.Version >= adBreaksSerializerInfo.Value).Value;

            return adBreaksSerializer.SerializeAdBreaksSet(serializedAdPositionReleases);
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool dispose)
        {
            timer.Dispose();
        }

        #endregion
    }
}
