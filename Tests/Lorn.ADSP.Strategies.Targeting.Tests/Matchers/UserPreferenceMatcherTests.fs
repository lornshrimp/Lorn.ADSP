module Lorn.ADSP.Strategies.Targeting.Tests.Matchers.UserPreferenceMatcherTests

open System
open System.Threading
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Strategies.Targeting.Matchers
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Shared.Enums
open System.Collections.Generic

// 复用极简 ITargetingContext stub
type SimpleContext(props: (string * obj) list) =
    let propEntities: ContextProperty list =
        props
        |> List.map (fun (k, v) ->
            let valueStr = if isNull (box v) then "" else v.ToString()
            ContextProperty(k, valueStr, v.GetType().Name, "Test", false, 1.0m, System.Nullable(), "UnitTest"))

    interface ITargetingContext with
        member _.ContextName: string = "TestCtx"
        member _.ContextType: string = "Test"

        member _.Properties: IReadOnlyList<ContextProperty> =
            propEntities |> List.toArray :> IReadOnlyList<ContextProperty>

        member _.Timestamp: DateTime = DateTime.UtcNow
        member _.ContextId: Guid = Guid.Empty
        member _.DataSource: string = "UnitTest"

        member _.GetProperty(key: string) : ContextProperty =
            propEntities |> List.tryFind (fun p -> p.PropertyKey = key) |> Option.toObj

        member _.GetPropertyValue<'T>(key: string) : 'T =
            propEntities
            |> List.tryFind (fun p -> p.PropertyKey = key)
            |> Option.map (fun p -> p.GetValue<'T>())
            |> Option.defaultValue Unchecked.defaultof<'T>

        member _.GetPropertyValue<'T>(key: string, defaultValue: 'T) : 'T =
            propEntities
            |> List.tryFind (fun p -> p.PropertyKey = key)
            |> Option.map (fun p ->
                let v = p.GetValue<'T>() in if obj.ReferenceEquals(v, null) then defaultValue else v)
            |> Option.defaultValue defaultValue

        member _.GetPropertyAsString(key: string) : string =
            propEntities
            |> List.tryFind (fun p -> p.PropertyKey = key)
            |> Option.map (fun p -> p.PropertyValue)
            |> Option.defaultValue ""

        member _.HasProperty(key: string) : bool =
            propEntities |> List.exists (fun p -> p.PropertyKey = key)

        member _.GetPropertyKeys() : IReadOnlyCollection<string> =
            propEntities |> List.map (fun p -> p.PropertyKey) |> List.toArray :> IReadOnlyCollection<string>

        member _.GetPropertiesByCategory(category: string) : IReadOnlyList<ContextProperty> =
            propEntities
            |> List.filter (fun p -> String.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase))
            |> List.toArray
            :> IReadOnlyList<ContextProperty>

        member _.GetActiveProperties() : IReadOnlyList<ContextProperty> =
            propEntities
            |> List.filter (fun p -> (not p.ExpiresAt.HasValue) || p.ExpiresAt.Value > DateTime.UtcNow)
            |> List.toArray
            :> IReadOnlyList<ContextProperty>

        member _.IsValid() : bool = true
        member _.IsExpired(_maxAge: TimeSpan) : bool = false

        member _.GetMetadata() : IReadOnlyList<ContextProperty> =
            Array.empty<ContextProperty> :> IReadOnlyList<ContextProperty>

        member _.GetDebugInfo() : string =
            sprintf "SimpleContext(props=%d)" propEntities.Length

        member this.CreateLightweightCopy(includeKeys: IEnumerable<string>) : ITargetingContext =
            this :> ITargetingContext

        member this.CreateCategorizedCopy(categories: IEnumerable<string>) : ITargetingContext =
            this :> ITargetingContext

        member this.Merge(other: ITargetingContext, overwriteExisting: bool) : ITargetingContext =
            this :> ITargetingContext

let matcher =
    UserPreferenceMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

[<Fact>]
let ``AdType scoring uses preferences`` () =
    let pref =
        UserPreference(
            preferredAdTypes = (ResizeArray<AdType>([ AdType.Display; AdType.Video ]) :> IList<AdType>),
            preferredLanguages = (ResizeArray<string>([ "zh" ]) :> IList<string>),
            preferredTopics = (ResizeArray<string>([ "tech" ]) :> IList<string>),
            allowPersonalizedAds = true
        )

    let criteria =
        UserPreferenceTargeting.Create(
            targetAdTypes = (ResizeArray<AdType>([ AdType.Display; AdType.Native ]) :> IList<AdType>),
            targetLanguages = (ResizeArray<string>([ "zh" ]) :> IList<string>),
            targetTopics = (ResizeArray<string>([ "tech" ]) :> IList<string>)
        )

    let r =
        matcher.CalculateMatchScoreAsync(pref, criteria, null, CancellationToken.None).Result

    printfn "Pref AdType -> IsMatch=%b Score=%M Reason=%s Not=%s" r.IsMatch r.MatchScore r.MatchReason r.NotMatchReason
    r.IsMatch |> should equal true
    r.MatchScore |> should be (greaterThan 0m)

[<Fact>]
let ``Excluded category blocks match`` () =
    let ctx = SimpleContext [ "Category", box "gambling" ] :> ITargetingContext

    let criteria =
        UserPreferenceTargeting.Create(
            excludedCategories = (ResizeArray<string>([ "Gambling" ]) :> IList<string>),
            targetAdTypes = (ResizeArray<AdType>([ AdType.Display ]) :> IList<AdType>)
        )

    let r =
        matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

    printfn
        "Pref Excluded -> IsMatch=%b Score=%M Reason=%s Not=%s"
        r.IsMatch
        r.MatchScore
        r.MatchReason
        r.NotMatchReason

    r.IsMatch |> should equal false

[<Fact>]
let ``Time window respected when provided`` () =
    let now = DateTime.UtcNow
    let slot = TimeWindow.CreateFixed(TimeSpan.FromHours(2.0), now.AddHours(-1.0))

    let pref =
        UserPreference(
            preferredTimeSlots = (ResizeArray<TimeWindow>([ slot ]) :> IList<TimeWindow>),
            preferredAdTypes = (ResizeArray<AdType>([ AdType.Display ]) :> IList<AdType>)
        )

    let criteria =
        UserPreferenceTargeting.Create(targetAdTypes = (ResizeArray<AdType>([ AdType.Display ]) :> IList<AdType>))

    let r =
        matcher.CalculateMatchScoreAsync(pref, criteria, null, CancellationToken.None).Result

    printfn "Pref Time -> IsMatch=%b Score=%M Reason=%s Not=%s" r.IsMatch r.MatchScore r.MatchReason r.NotMatchReason
    r.IsMatch |> should equal true

[<Fact>]
let ``Consent required must be honored`` () =
    let pref =
        UserPreference(allowPersonalizedAds = false, allowBehaviorTracking = false, allowCrossDeviceTracking = false)

    let criteria =
        UserPreferenceTargeting.Create(
            targetAdTypes = (ResizeArray<AdType>([ AdType.Display ]) :> IList<AdType>),
            requirePersonalizedAdsConsent = true,
            requireBehaviorTrackingConsent = true,
            requireCrossDeviceTrackingConsent = true
        )

    let r =
        matcher.CalculateMatchScoreAsync(pref, criteria, null, CancellationToken.None).Result

    printfn "Pref Consent -> IsMatch=%b Score=%M Reason=%s Not=%s" r.IsMatch r.MatchScore r.MatchReason r.NotMatchReason
    r.IsMatch |> should equal false

[<Fact>]
let ``Privacy and content maturity constraints`` () =
    let pref =
        UserPreference(
            preferredAdTypes = (ResizeArray<AdType>([ AdType.Display ]) :> IList<AdType>),
            preferredTopics = (ResizeArray<string>([ "tech" ]) :> IList<string>),
            privacyLevel = PrivacyLevel.High,
            contentMaturityLevel = ContentMaturityLevel.General
        )

    let criteria =
        UserPreferenceTargeting.Create(
            targetAdTypes = (ResizeArray<AdType>([ AdType.Display ]) :> IList<AdType>),
            targetTopics = (ResizeArray<string>([ "tech" ]) :> IList<string>),
            maxPrivacyLevel = PrivacyLevel.Standard, // 标准 < 高，不满足
            maxContentMaturityLevel = ContentMaturityLevel.Restricted
        )

    let r =
        matcher.CalculateMatchScoreAsync(pref, criteria, null, CancellationToken.None).Result

    printfn "Pref Privacy -> IsMatch=%b Score=%M Reason=%s Not=%s" r.IsMatch r.MatchScore r.MatchReason r.NotMatchReason
    r.IsMatch |> should equal false

[<Fact>]
let ``No explicit preferences yields neutral acceptance`` () =
    let ctx = SimpleContext [] :> ITargetingContext

    let criteria =
        UserPreferenceTargeting.Create(
            targetAdTypes = (ResizeArray<AdType>() :> IList<AdType>),
            targetTopics = (ResizeArray<string>() :> IList<string>),
            targetLanguages = (ResizeArray<string>() :> IList<string>)
        )

    let r =
        matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

    printfn "Pref Neutral -> IsMatch=%b Score=%M Reason=%s Not=%s" r.IsMatch r.MatchScore r.MatchReason r.NotMatchReason
    r.IsMatch |> should equal true
