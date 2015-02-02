namespace Lorn.ADSP.DE.AdFilters

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open System.Collections
open System.Collections.Generic
open System.Linq
open System

[<Export(typeof<IFilterAd>)>]
[<ExportMetadata("ProcesserId", "{D3BBD201-45A6-4BFC-B814-18F56351B7A8}")>]
[<ExportMetadata("ProcesserName", "AdLocationAdFilter")>]
[<ExportMetadata("Description", "根据广告位置对待投广告进行过滤")>]
[<ExportMetadata("Version", "1.0")>]
[<ExportMetadata("FilterType", AdFilterType.ExcludeFilter)>]
[<ExportMetadata("RequiredAdditionalParameters", AdPosition.STR_ADPOSITION)>]
type public AdLocationAdFilter() = 
    interface IFilterAd with
        member this.FilterAd(cookie, parameters, adDispatchPlans, filterConfiguration, redirectConditionDefinitions, 
                             filteredAds, additionalParameters) = 
            let adPosition = additionalParameters.[AdPosition.STR_ADPOSITION] :?> AdPosition
            adDispatchPlans.Where(fun (o : Ad) -> 
                           o.AdLocationId.HasValue = false || o.AdLocationId.Value = adPosition.AdLocationId).ToList() :> ICollection<Ad>
