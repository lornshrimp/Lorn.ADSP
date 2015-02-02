namespace Lorn.ADSP.DE.AdFilters

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open System.Collections
open System.Collections.Generic
open System.Linq
open System

[<Export(typeof<IFilterAd>)>]
[<ExportMetadata("ProcesserId", "{21353809-1024-4DB3-A5D6-BC704DC381E9}")>]
[<ExportMetadata("ProcesserName", "AdPositionSizeFilter")>]
[<ExportMetadata("Description", "根据广告尺寸对待投广告进行过滤")>]
[<ExportMetadata("Version", "1.0")>]
[<ExportMetadata("FilterType", AdFilterType.ExcludeFilter)>]
[<ExportMetadata("RequiredAdditionalParameters", AdPosition.STR_ADPOSITION)>]
type public AdPositionSizeAdFilter() = 
    interface IFilterAd with
        member this.FilterAd(cookie, parameters, adDispatchPlans, filterConfiguration, redirectConditionDefinitions, 
                             filteredAds, additionalParameters) = 
            let adPosition = additionalParameters.[AdPosition.STR_ADPOSITION] :?> AdPosition
            adDispatchPlans.Where(fun (o : Ad) -> 
                           o.AdPositionSizeId.HasValue = false || o.AdPositionSizeId.Value = adPosition.AdPositionSizeId)
                           .ToList() :> ICollection<Ad>
