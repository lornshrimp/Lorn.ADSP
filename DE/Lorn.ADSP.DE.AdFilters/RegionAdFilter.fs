namespace Lorn.ADSP.DE.AdFilters

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open Lorn.ADSP.DE.Helpers
open System.Collections
open System.Collections.Generic
open System.Linq
open System.Net
open System.Numerics
open System

[<Export(typeof<IFilterAd>)>]
[<ExportMetadata("ProcesserId", "{758DC7CA-C2B3-454C-B4F1-2171168799E3}")>]
[<ExportMetadata("ProcesserName", "RegionAdFilter")>]
[<ExportMetadata("Description", "根据地域对待投广告进行过滤")>]
[<ExportMetadata("RedirectDimensionId", "{F91C7383-37DC-430A-9CA3-282BC2BB30A7}")>]
[<ExportMetadata("Version", "1.0")>]
[<ExportMetadata("FilterType", AdFilterType.ExcludeFilter)>]
[<ExportMetadata("RequiredAdditionalParameters", IpLibrary.STR_IPLIBRARIES)>]
type public RegionAdFilter() = 
    interface IFilterAd with
        member this.FilterAd(cookie, parameters, adDispatchPlans, filterConfiguration, redirectConditionDefinitions, 
                             filteredAds, additionalParameters) = 
            let mutable ads = adDispatchPlans
            let ipParameterKey = "IP"
            if parameters.ContainsKey(ipParameterKey) then 
                let ipString = parameters.[ipParameterKey]
                let redirectConditionDimensionId = new Guid("{F91C7383-37DC-430A-9CA3-282BC2BB30A7}")
                let ipLibraries = additionalParameters.[IpLibrary.STR_IPLIBRARIES] :?> IDictionary<Guid, IpLibrary>
                let mutable ipAddress : IPAddress = null
                let ipAddressResult = IPAddress.TryParse(ipString, ref ipAddress)
                if ipAddressResult = true then 
                    let ip = new BigInteger(ipAddress.GetAddressBytes())
                    let ipRegions = new Dictionary<Guid, Guid>()
                    for ipLibrary in ipLibraries do
                        let ipMap = ipLibrary.Value.IpMaps.FirstOrDefault(fun o -> o.IpStart <= ip && o.IpEnd >= ip)
                        if ipMap <> null then ipRegions.[ipLibrary.Key] <- ipMap.RegionId
                    ads <- adDispatchPlans.Where(fun (o : Ad) -> 
                                          //没有对地域进行定向
                                          o.RedirctConditions.ContainsKey(redirectConditionDimensionId) = false 
                                          //如果对地域进行了定向
                                          //但没有进行反定向
                                          || ipRegions.Any
                                                 (fun p -> 
                                                 o.IpLibraryId = p.Key 
                                                 && ((o.RedirctConditions.[redirectConditionDimensionId].ExcludeRedirctConditionDetails = null 
                                                      //虽然进行了反定向但没有命中 
                                                      || o.RedirctConditions.[redirectConditionDimensionId]
                                                             .ExcludeRedirctConditionDetails.RetriveRedirectCondition(redirectConditionDefinitions.[redirectConditionDimensionId], 
                                                                                                                      
                                                                                                                      p.Value) = false) 
                                                     //没有进行正定向
                                                     && (o.RedirctConditions.[redirectConditionDimensionId].IncludeRedirctConditionDetails = null 
                                                         //进行了正定向且命中了
                                                         || o.RedirctConditions.[redirectConditionDimensionId]
                                                             .IncludeRedirctConditionDetails.RetriveRedirectCondition(redirectConditionDefinitions.[redirectConditionDimensionId], 
                                                                                                                      
                                                                                                                      p.Value)))))
                                          .ToList()
            ads
