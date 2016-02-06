using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Lorn.ADSP.DE.DataModels;
using Lorn.ADSP.DE.Interfaces;
using System.ComponentModel.Composition;
using Lorn.ADSP.Common.Interfaces;
using System.Configuration;

namespace Lorn.ADSP.DE.DispatchEngine
{
    public class DispatchRuntime : IDisposable
    {
        protected Timer timer;
        [Import(typeof(ICountAd), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICountAd adCounter { get; set; }
        [Import(typeof(ICacheAdDispatchPlans), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheAdDispatchPlans adDispatchPlansCacher { get; set; }
        [ImportMany(typeof(ICountAd), RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<Lazy<ICalculateAdDispatchPlan, ICalculateAdDispatchPlanMetadata>> adDispatchPlanCalculators { get; set; }
        [Import(typeof(ITime), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ITime Time { get; set; }
        [Import(typeof(IMonitorServiceStatus), RequiredCreationPolicy = CreationPolicy.Shared)]
        public IMonitorServiceStatus ServiceStatusMoniter { get; set; }
        [Import(typeof(ICacheServiceConfigurations), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheServiceConfigurations ServiceConfigurationCacher { get; set; }
        [Import(typeof(ICacheDeliveryPiplineConfigurations), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheDeliveryPiplineConfigurations DeliveryPiplineConfigurationsCacher { get; set; }
        [Import(typeof(ICacheRedirectConditionDefinitions), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheRedirectConditionDefinitions RedirectConditionDefinitionsCacher { get; set; }
        [Import(typeof(ICacheAdPositionAndAdPositionGroups), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheAdPositionAndAdPositionGroups AdpInfoCacher { get; set; }
        [Import(typeof(ICacheSerializerMappings), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheSerializerMappings SerializerMappingsCacher { get; set; }
        [Import(typeof(IAccessAdScheduleDb), RequiredCreationPolicy = CreationPolicy.Shared)]
        public IAccessAdScheduleDb AdScheduleDbAccesser { get; set; }
        [Import(typeof(ICacheMediaSecureKeys), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheMediaSecureKeys MediaScureKeysCacher { get; set; }
        [Import(typeof(ICalculateFlowControl), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICalculateFlowControl FlowControlCalculator { get; set; }

        protected IDictionary<string, object> serviceConfigurations;

        protected DateTime lastDispatchTime;
        protected DateTime lastReportHealthTime;
        protected bool dispatchingAdPlan = false;
        protected string serverId;


        public DispatchRuntime()
        {
            lastDispatchTime = Time.Now;
            lastReportHealthTime = Time.Now;

            this.serverId = ConfigurationManager.AppSettings[ConstStrings.STR_SERVERID];
            this.ServiceStatusMoniter.InitService(ConstStrings.STR_DISPATCHSERVICENAME, serverId);

            RunDispatch();

            timer = new Timer();
            timer.AutoReset = true;
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 1000;
            timer.Start();

            this.ServiceStatusMoniter.InitServiceComplete(ConstStrings.STR_DISPATCHSERVICENAME, serverId);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int reportHealthInterval = 1;
            if (this.serviceConfigurations.ContainsKey(ConstStrings.STR_REPORTHEALTHINTERVAL))
            {
                reportHealthInterval = (int)this.serviceConfigurations[ConstStrings.STR_REPORTHEALTHINTERVAL];
            }

            int adDispatchInterval = 60;
            if (this.serviceConfigurations.ContainsKey(ConstStrings.STR_DISPATCHADPLANINTERVAL))
            {
                adDispatchInterval = (int)this.serviceConfigurations[ConstStrings.STR_DISPATCHADPLANINTERVAL];
            }

            DateTime now = Time.Now;
            if (now > lastReportHealthTime.AddMinutes(reportHealthInterval))
            {
                this.ServiceStatusMoniter.ReportStatus(ConstStrings.STR_DISPATCHSERVICENAME, this.serverId);
            }
            if (now > lastDispatchTime.AddMinutes(adDispatchInterval) && dispatchingAdPlan == false)
            {
                RunDispatch();
            }
        }

        protected void RunDispatch()
        {
            dispatchingAdPlan = true;

            var servicecfg = this.ServiceConfigurationCacher.GetServiceConfiguration(ConstStrings.STR_DISPATCHSERVICENAME, this.serverId);
            if (servicecfg != null)
            {
                this.serviceConfigurations = servicecfg;
            }
            this.AdpInfoCacher.SetAdPositionAndAdPositionGroupCodeMappings(this.AdScheduleDbAccesser.GetAdPositionAndAdPositionGroupCodeMappings());
            this.AdpInfoCacher.SetAdPositionGroups(this.AdScheduleDbAccesser.GetAdPositionGroups());
            this.AdpInfoCacher.SetAdPositions(this.AdScheduleDbAccesser.GetAdPositions());
            this.DeliveryPiplineConfigurationsCacher.SetDeliveryPiplineConfigurations(this.AdScheduleDbAccesser.GetDeliveryPiplineConfigurations());
            this.RedirectConditionDefinitionsCacher.SetRedirectConditionDefinitions(this.AdScheduleDbAccesser.GetRedirectConditionDefinitions());
            this.RedirectConditionDefinitionsCacher.SetIpLibraries(this.AdScheduleDbAccesser.GetIpLibraries());
            this.SerializerMappingsCacher.SetAdPositionReleaseSerializerMappings(this.AdScheduleDbAccesser.GetAdPositionReleaseSerializerMappings());
            this.SerializerMappingsCacher.SetAdStacksSerializerMapping(this.AdScheduleDbAccesser.GetAdStacksSerializerMappings());
            this.SerializerMappingsCacher.SetCreativeSerializerMappings(this.AdScheduleDbAccesser.GetCreativeSerializerMappings());
            this.MediaScureKeysCacher.SetMediaSecureKeys(this.AdScheduleDbAccesser.GetMediaSecureKeys());
            var monitorTypeMappings = this.AdScheduleDbAccesser.GetMonitorTypeMappings();
            var flowControls = this.AdScheduleDbAccesser.GetFlowControls();

            var adSchedulePlans = this.AdScheduleDbAccesser.GetAdSpotPlans();
            var deliveryServers = this.ServiceStatusMoniter.GetServerStatuses(ConstStrings.STR_DELIVERYRUNTIMESERVICENAME).Where(o => o.Value.Key != ServiceStatuses.NotRunning && o.Value.Value.HasValue);
            var adCounts = this.adCounter.GetAdMonitorCount();
            var adDispatchPlans = new Dictionary<string, IDictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>>>();
            var sumPerformance = deliveryServers.Sum(o => o.Value.Value.Value);

            var calculateTimeSpanNumber = 3;
            if (this.serviceConfigurations.ContainsKey(ConstStrings.STR_CALCULATETIMESPANNUMBER))
            {
                calculateTimeSpanNumber = (int)this.serviceConfigurations[ConstStrings.STR_CALCULATETIMESPANNUMBER];
            }
            int adDispatchInterval = 30;
            if (this.serviceConfigurations.ContainsKey(ConstStrings.STR_DISPATCHADPLANINTERVAL))
            {
                adDispatchInterval = (int)this.serviceConfigurations[ConstStrings.STR_DISPATCHADPLANINTERVAL];
            }

            foreach (var mediaSpotPlans in adSchedulePlans)
            {
                var mediaId = mediaSpotPlans.Key;
                var mediaAdCounts = adCounts[mediaId];
                var adSpotPlanEnumerator = mediaSpotPlans.Value.GetEnumerator();
                var mediaMonitorTypeMappings = monitorTypeMappings[mediaId];
                while (adSpotPlanEnumerator.MoveNext())
                {
                    var adSpotPlan = adSpotPlanEnumerator.Current;
                    long planReleaseNumber = 0;
                    float broadcastRatio = 1;

                    if (adSpotPlan.PriceUnit == Common.DataModels.PriceUnit.CPT)
                    {
                        //负数表示无需进行流量调度
                        planReleaseNumber = adSpotPlan.PlanTrafficRatio * -100;
                    }
                    else
                    {
                        //读取今天的计划量
                        switch (adSpotPlan.PriceUnit)
                        {
                            case Lorn.ADSP.Common.DataModels.PriceUnit.CPM:
                                planReleaseNumber = adSpotPlan.PlanImpressionNumber;
                                break;
                            case Lorn.ADSP.Common.DataModels.PriceUnit.CPC:
                                planReleaseNumber = adSpotPlan.PlanClickNumber;
                                broadcastRatio = adSpotPlan.PlanClickNumber / adSpotPlan.PlanImpressionNumber;
                                break;
                            default:
                                break;
                        }
                        //计算今天的剩余量
                        if (mediaAdCounts != null && mediaAdCounts.ContainsKey(adSpotPlan.AdSpotPlanId) && mediaMonitorTypeMappings != null && mediaMonitorTypeMappings.ContainsKey(adSpotPlan.ValuationMonitorTypeId) && mediaAdCounts[adSpotPlan.AdSpotPlanId].ContainsKey(mediaMonitorTypeMappings[adSpotPlan.ValuationMonitorTypeId]))
                        {
                            planReleaseNumber -= mediaAdCounts[adSpotPlan.AdSpotPlanId][mediaMonitorTypeMappings[adSpotPlan.ValuationMonitorTypeId]];
                            if (mediaAdCounts[adSpotPlan.AdSpotPlanId].ContainsKey(ConstStrings.STR_RETURNAD))
                            {
                                broadcastRatio = (mediaAdCounts[adSpotPlan.AdSpotPlanId][mediaMonitorTypeMappings[adSpotPlan.ValuationMonitorTypeId]]) / (mediaAdCounts[adSpotPlan.AdSpotPlanId][ConstStrings.STR_RETURNAD]);
                            }
                        }
                        planReleaseNumber = (long)(planReleaseNumber / broadcastRatio);
                    }


                    //调度流量
                    var adDispatchPlanCalculator = this.adDispatchPlanCalculators.First(o => o.Metadata.ConsumeType == adSpotPlan.ComsumeType).Value;
                    adDispatchPlanCalculator.CalculateTimeSpan = new TimeSpan(0, adDispatchInterval, 0);
                    adDispatchPlanCalculator.CalculateTimeSpanNumber = calculateTimeSpanNumber;
                    var adDispatchPlan = adDispatchPlanCalculator.CalculateAdDispatchPlan(Time.Now, planReleaseNumber, FlowControlCalculator.CalculateFlowControl(adSpotPlan.RedirctConditions, flowControls[mediaId]));

                    //将流量分配到各投放服务器
                    foreach (var server in deliveryServers)
                    {

                        if (!adDispatchPlans.ContainsKey(server.Key))
                        {
                            adDispatchPlans[server.Key] = new Dictionary<Guid, IDictionary<DateTime, IDictionary<Ad, long>>>();
                        }
                        if (!adDispatchPlans[server.Key].ContainsKey(mediaId))
                        {
                            adDispatchPlans[server.Key][mediaId] = new Dictionary<DateTime, IDictionary<Ad, long>>();
                        }
                        foreach (var adDispatchTimePlan in adDispatchPlan)
                        {
                            if (!adDispatchPlans[server.Key][mediaId].ContainsKey(adDispatchTimePlan.Key))
                            {
                                adDispatchPlans[server.Key][mediaId][adDispatchTimePlan.Key] = new Dictionary<Ad, long>();
                            }
                            if (adDispatchTimePlan.Value >= 0)
                            {
                                adDispatchPlans[server.Key][mediaId][adDispatchTimePlan.Key][adSpotPlan] = adDispatchTimePlan.Value * server.Value.Value.Value / sumPerformance;
                            }
                            else
                            {
                                adDispatchPlans[server.Key][mediaId][adDispatchTimePlan.Key][adSpotPlan] = adDispatchTimePlan.Value;
                            }
                        }
                    }
                }
            }

            this.adDispatchPlansCacher.SetAdDispatchPlansCache(adDispatchPlans);

            dispatchingAdPlan = false;
        }

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
