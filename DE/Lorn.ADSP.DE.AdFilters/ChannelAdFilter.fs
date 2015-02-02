namespace Lorn.ADSP.DE.AdFilters

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open Lorn.ADSP.DE.Helpers
open System.Collections
open System.Collections.Generic
open System.Linq
open System

[<Export(typeof<IFilterAd>)>]
[<ExportMetadata("ProcesserId", "{A457F2E7-1D2B-470D-B709-A098F649E63F}")>]
[<ExportMetadata("ProcesserName", "ChannelAdFilter")>]
[<ExportMetadata("Description", "根据频道对待投广告进行过滤")>]
[<ExportMetadata("RedirectDimensionId", "{23B252A1-0CC4-4A51-AEF5-E17FD6B403D2}")>]
[<ExportMetadata("Version", "1.0")>]
[<ExportMetadata("FilterType", AdFilterType.ExcludeFilter)>]
type public ChannelAdFilter() = 
    interface IFilterAd with
        member this.FilterAd(cookie, parameters, adDispatchPlans, filterConfiguration, redirectConditionDefinitions, 
                             filteredAds, additionalParameters) = 
            let mutable ads : ICollection<Ad> = adDispatchPlans
            let redirectConditionDimensionId = new Guid("{23B252A1-0CC4-4A51-AEF5-E17FD6B403D2}")
            if parameters.ContainsKey("ChannelId") then 
                let channelId = new Guid(parameters.["ChannelId"])
                ads <- adDispatchPlans.Where(fun (o : Ad) -> 
                                      //没有对频道进行定向
                                      o.RedirctConditions.ContainsKey(redirectConditionDimensionId) = false 
                                      //如果对频道进行了定向
                                      //但没有进行反定向
                                      || ((o.RedirctConditions.[redirectConditionDimensionId].ExcludeRedirctConditionDetails = null 
                                           //虽然进行了反定向但没有命中 
                                           || o.RedirctConditions.[redirectConditionDimensionId]
                                                  .ExcludeRedirctConditionDetails.RetriveRedirectCondition(redirectConditionDefinitions.[redirectConditionDimensionId], 
                                                                                                           channelId) = false) 
                                          //没有进行正定向
                                          && (o.RedirctConditions.[redirectConditionDimensionId].IncludeRedirctConditionDetails = null 
                                              //进行了正定向且命中了
                                              || o.RedirctConditions.[redirectConditionDimensionId]
                                                  .IncludeRedirctConditionDetails.RetriveRedirectCondition(redirectConditionDefinitions.[redirectConditionDimensionId], 
                                                                                                           channelId))))
                                      .ToList()
            else 
                if parameters.ContainsKey("ChannelCode") then 
                    let channelCode = parameters.["ChannelCode"]
                    ads <- adDispatchPlans.Where(fun (o : Ad) -> 
                                          //没有对频道进行定向
                                          o.RedirctConditions.ContainsKey(redirectConditionDimensionId) = false 
                                          //如果对频道进行了定向
                                          //但没有进行反定向
                                          || ((o.RedirctConditions.[redirectConditionDimensionId].ExcludeRedirctConditionDetails = null 
                                               //虽然进行了反定向但没有命中 
                                               || o.RedirctConditions.[redirectConditionDimensionId]
                                                      .ExcludeRedirctConditionDetails.RetriveRedirectCondition(redirectConditionDefinitions.[redirectConditionDimensionId], 
                                                                                                               
                                                                                                               channelCode) = false) 
                                              //没有进行正定向
                                              && (o.RedirctConditions.[redirectConditionDimensionId].IncludeRedirctConditionDetails = null 
                                                  //进行了正定向且命中了
                                                  || o.RedirctConditions.[redirectConditionDimensionId]
                                                      .IncludeRedirctConditionDetails.RetriveRedirectCondition(redirectConditionDefinitions.[redirectConditionDimensionId], 
                                                                                                               
                                                                                                               channelCode))))
                                          .ToList()
            ads
