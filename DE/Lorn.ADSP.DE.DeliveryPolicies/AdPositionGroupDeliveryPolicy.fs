namespace Lorn.ADSP.DE.DeliveryPolicies

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open System.Collections
open System.Collections.Generic
open System.Linq

[<Export(typeof<IRunDeliveryPolicy>)>]
[<ExportMetadata("ProcesserId","{78F47172-C3CC-4BDF-AF09-6038E2D19B7A}")>]
[<ExportMetadata("ProcesserName","AdPositionGroupDeliveryPolicy")>]
[<ExportMetadata("Version","1.0")>]
[<ExportMetadata("Description","用于处理广告位组的广告投放策略")>]
type public AdPositionGroupDeliveryPolicy() =
    ///定义投放策略
    [<ImportMany(typeof<IRunDeliveryPolicy>,RequiredCreationPolicy = CreationPolicy.Shared)>]
    member this.DeliveryPolicies :IEnumerable<System.Lazy<IRunDeliveryPolicy, IRunDeliveryPolicyMetadata>> = null
    ///实现接口
    interface IRunDeliveryPolicy with
        member this.RunDeliveryPolicy(adPositionOrAdPositionGroupId,cookie,parameters, deliveryPiplineConfigurations, adPositions,adPositionGroups,  redirectConditionDefinitions, ipLibraries, adDispatchPlans, creativeSerializerMappings)=
            let returnAdQueue  = new Dictionary<AdPositionReleaseInfo, Queue<AdMaterialReleaseInfo>>()
            ///查找广告位组信息
            let adpositionGroup = adPositionGroups.[adPositionOrAdPositionGroupId]
            if adpositionGroup <> null then
            ///取出广告位组中的广告位
                for adPositionId in adpositionGroup.AdPositions do
                    ///查找配置
                    if deliveryPiplineConfigurations.ContainsKey(adPositionId) then
                        let piplineConfiguation = deliveryPiplineConfigurations.[adPositionId]
                        ///取出策略
                        let policy = this.DeliveryPolicies.First(fun o ->o.Metadata.ProcesserId = piplineConfiguation.AdDeliveryPolicy.ProcesserId && o.Metadata.Version >= piplineConfiguation.AdDeliveryPolicy.ProcesserMinVersion).Value
                        ///执行广告位策略
                        let adpReturnAdQueues = policy.RunDeliveryPolicy    (adPositionId,cookie,parameters,deliveryPiplineConfigurations,adPositions,adPositionGroups,redirectConditionDefinitions,ipLibraries, adDispatchPlans,creativeSerializerMappings)
                        ///合并返回的广告队列,尚未完成：暂未实现对伴随广告的处理
                        if adpReturnAdQueues <> null then
                            for adpReturnAdQueue in adpReturnAdQueues do
                            let positionId = adpReturnAdQueue.Key.AdPositionId
                            let adps = query{
                                for q in returnAdQueue do
                                where (q.Key.AdPositionId = positionId)
                                select q}
                            if adps.Count() > 0 then
                                while adpReturnAdQueue.Value.Count > 0 do
                                    let adp = query{
                                        for q in adps do
                                        select q
                                        headOrDefault}
                                    adp.Value.Enqueue(adpReturnAdQueue.Value.Dequeue())
                            else
                                returnAdQueue.[adpReturnAdQueue.Key] <- adpReturnAdQueue.Value
            returnAdQueue :> IDictionary<AdPositionReleaseInfo, Queue<AdMaterialReleaseInfo>>