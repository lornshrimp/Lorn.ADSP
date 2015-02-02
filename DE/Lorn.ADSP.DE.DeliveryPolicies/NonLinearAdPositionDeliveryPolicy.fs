namespace Lorn.ADSP.DE.DeliveryPolicies

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open System.Collections
open System.Collections.Generic
open System.Linq

[<Export(typeof<IRunDeliveryPolicy>)>]
[<ExportMetadata("ProcesserId","{93E7A7DF-1F9D-401E-8426-3EBAAF9AA187}")>]
[<ExportMetadata("ProcesserName","NonLinearAdPositionDeliveryPolicy")>]
[<ExportMetadata("Version","1.0")>]
[<ExportMetadata("Description","用于非线性广告位的广告投放策略")>]
type public NonLinearAdPositionDeliveryPolicy() =
    [<ImportMany(typeof<IFilterAd>,RequiredCreationPolicy = CreationPolicy.Shared)>]
    member this.AdFilters :IEnumerable<System.Lazy<IFilterAd, IFilterMetadata>> = null
    [<ImportMany(typeof<IFilterAdMaterial>,RequiredCreationPolicy = CreationPolicy.Shared)>]
    member this.AdMaterialFilters :IEnumerable<System.Lazy<IFilterAdMaterial, IProcesserMetadata>> = null
    interface IRunDeliveryPolicy with
        member this.RunDeliveryPolicy(adPositionOrAdPositionGroupId,cookie,parameters, deliveryPiplineConfigurations, adPositions,adPositionGroups,  redirectConditionDefinitions,ipLibraries, adDispatchPlans, creativeSerializerMappings)=
            let returnAdQueue  = new Dictionary<AdPositionReleaseInfo, Queue<AdMaterialReleaseInfo>>()
            ///获取广告管线配置
            if deliveryPiplineConfigurations.ContainsKey(adPositionOrAdPositionGroupId) && adPositions.ContainsKey(adPositionOrAdPositionGroupId) then
                let deliveryPiplineConfiguration = deliveryPiplineConfigurations.[adPositionOrAdPositionGroupId]
                let adPosition = adPositions.[adPositionOrAdPositionGroupId]
                //取出广告信息
                let mutable excludeFilteredAds:ICollection<Ad> = new List<Ad>() :>ICollection<Ad>
                for item in adDispatchPlans do
                    ///不等于0表示还有量可投
                    if item.Value.Any(fun o ->o.Value <> (int64)0) then
                        excludeFilteredAds.Add(item.Key)
                ///依次通过排除过滤器进行筛选
                while deliveryPiplineConfiguration.ExcludeFilters.Count > 0 do
                    let excludeFilterConfiguration = deliveryPiplineConfiguration.ExcludeFilters.Dequeue()
                    let adFilterLazy = this.AdFilters.First(fun o ->o.Metadata.ProcesserId = excludeFilterConfiguration.ProcesserId && o.Metadata.Version >=excludeFilterConfiguration.ProcesserMinVersion)
                    let adFilter = adFilterLazy.Value
                    let mutable additionalParameters = new Dictionary<string,System.Object>()
                    if adFilterLazy.Metadata.RequiredAdditionalParameters.Contains(AdPosition.STR_ADPOSITION) then
                        additionalParameters.[AdPosition.STR_ADPOSITION] <- adPositions.[adPositionOrAdPositionGroupId]
                    if adFilterLazy.Metadata.RequiredAdditionalParameters.Contains(IpLibrary.STR_IPLIBRARIES) then
                        additionalParameters.[IpLibrary.STR_IPLIBRARIES] <- ipLibraries
                    excludeFilteredAds <- adFilter.FilterAd(cookie,parameters,excludeFilteredAds,excludeFilterConfiguration,redirectConditionDefinitions,null,additionalParameters)
                //依次优选每个位置的广告
                for i = 0 to adPosition.MaxReturnAdCount - 1 do
                    let mutable preferredAds = excludeFilteredAds
                    //取出在该位置上还有量的广告,不等于0表示还有量可投
                    preferredAds <- preferredAds.Where(fun o -> adDispatchPlans.[o].ContainsKey(i) && adDispatchPlans.[o].[i] <> (int64)0).ToList()
                    ///依次通过每个优选过滤器进行筛选
                    while deliveryPiplineConfiguration.PreferredFilters.Count > 0 do
                        let preferredFilterConfiguration = deliveryPiplineConfiguration.PreferredFilters.Dequeue()
                        let adFilter = this.AdFilters.First(fun o -> o.Metadata.ProcesserId = preferredFilterConfiguration.ProcesserId && o.Metadata.Version >=preferredFilterConfiguration.ProcesserMinVersion).Value
                        preferredAds <- adFilter.FilterAd(cookie,parameters,preferredAds,preferredFilterConfiguration,redirectConditionDefinitions,returnAdQueue)
                    ///如果这时候选的广告还比较多，则使用随机广告过滤器随便选一个
                    if preferredAds.Count > 1 then
                        let randomAdFilterConfiguration = deliveryPiplineConfiguration.RandomFilter
                        let adFilter = this.AdFilters.First(fun o->o.Metadata.ProcesserId = randomAdFilterConfiguration.ProcesserId && o.Metadata.Version >=randomAdFilterConfiguration.ProcesserMinVersion).Value
                        preferredAds <- adFilter.FilterAd(cookie,parameters,preferredAds,randomAdFilterConfiguration)
                    if preferredAds.Count = 1 then
                        ///筛选物料
                        let adMaterialFilterConfiguration = deliveryPiplineConfiguration.AdMaterialFilter
                        let adMaterialFilter = this.AdMaterialFilters.First(fun o -> o.Metadata.ProcesserId = adMaterialFilterConfiguration.ProcesserId && o.Metadata.Version >=adMaterialFilterConfiguration.ProcesserMinVersion).Value
                        let adMaterial = adMaterialFilter.FilterAdMaterial(cookie,parameters,preferredAds.First(),adMaterialFilterConfiguration)
                        //将筛选出的广告物料加入返回队列
                        adMaterial.DeliveryPiplineConfigurationId <- deliveryPiplineConfigurations.[adPositionOrAdPositionGroupId].DeliveryPiplineConfigurationId
                        adMaterial.DeliveryPiplineConfigurationVersion <- deliveryPiplineConfigurations.[adPositionOrAdPositionGroupId].Version
                        let mutable adPositionReleaseInfo = returnAdQueue.Keys.FirstOrDefault(fun o ->o.AdPositionId = adPositionOrAdPositionGroupId)
                        if adPositionReleaseInfo = null then
                            adPositionReleaseInfo <- new AdPositionReleaseInfo(adPosition)
                        returnAdQueue.Add(adPositionReleaseInfo,new Queue<AdMaterialReleaseInfo>())
                        returnAdQueue.[adPositionReleaseInfo].Enqueue(adMaterial)
            returnAdQueue :> IDictionary<AdPositionReleaseInfo, Queue<AdMaterialReleaseInfo>>

