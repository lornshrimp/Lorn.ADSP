namespace Lorn.ADSP.DE.AdFilters

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open System.Collections
open System.Collections.Generic
open System.Linq
open System

[<Export(typeof<IFilterAd>)>]
[<ExportMetadata("ProcesserId", "{71584D69-EBAF-4037-9915-9F9DF9326453}")>]
[<ExportMetadata("ProcesserName", "AdPositionAdFilter")>]
[<ExportMetadata("Description", "根据广告位对待投广告进行过滤")>]
[<ExportMetadata("Version", "1.0")>]
[<ExportMetadata("RedirectDimensionId", "{E532F6A9-BC5D-4331-B634-F1F9645E97AF}")>]
[<ExportMetadata("FilterType", AdFilterType.ExcludeFilter)>]
[<ExportMetadata("RequiredAdditionalParameters", AdPosition.STR_ADPOSITION)>]
type public AdPositionAdFilter() = 
    interface IFilterAd with
        member this.FilterAd(cookie, parameters, adDispatchPlans, filterConfiguration, redirectConditionDefinitions, 
                             filteredAds, additionalParameters) = 
            let adPosition = additionalParameters.[AdPosition.STR_ADPOSITION] :?> AdPosition
            adDispatchPlans.Where(fun (o : Ad) -> o.AdPositionId.HasValue = false || o.AdPositionId.Value = adPosition.AdPositionId)
                           .ToList() :> ICollection<Ad>
