module Lorn.ADSP.Strategies.Targeting.Tests.Matchers.UserInterestMatcherTests

open System
open System.Threading
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Strategies.Targeting.Matchers
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects

// 精简 ITargetingContext stub，仅实现接口实际定义的成员
type SimpleContext(props: (string * obj) list) =
    let propEntities: ContextProperty list =
        props
        |> List.map (fun (k, v) ->
            let valueStr = if isNull (box v) then "" else v.ToString()
            ContextProperty(k, valueStr, v.GetType().Name, "Test", false, 1.0m, System.Nullable(), "UnitTest"))

    interface ITargetingContext with
        member _.ContextName = "TestCtx"
        member _.ContextType = "Test"
        member _.Properties = propEntities |> List.toArray :> _
        member _.Timestamp = DateTime.UtcNow
        member _.ContextId = Guid.Empty
        member _.DataSource = "UnitTest"

        member _.GetProperty key =
            propEntities |> List.tryFind (fun p -> p.PropertyKey = key) |> Option.toObj

        member _.GetPropertyValue<'T>(key) =
            propEntities
            |> List.tryFind (fun p -> p.PropertyKey = key)
            |> Option.map (fun p -> p.GetValue<'T>())
            |> Option.defaultValue Unchecked.defaultof<'T>

        member _.GetPropertyValue<'T>(key, defaultValue: 'T) =
            propEntities
            |> List.tryFind (fun p -> p.PropertyKey = key)
            |> Option.map (fun p ->
                let v = p.GetValue<'T>() in if obj.ReferenceEquals(v, null) then defaultValue else v)
            |> Option.defaultValue defaultValue

        member _.GetPropertyAsString key =
            propEntities
            |> List.tryFind (fun p -> p.PropertyKey = key)
            |> Option.map (fun p -> p.PropertyValue)
            |> Option.defaultValue ""

        member _.HasProperty key =
            propEntities |> List.exists (fun p -> p.PropertyKey = key)

        member _.GetPropertyKeys() =
            propEntities |> List.map (fun p -> p.PropertyKey) |> List.toArray :> _

        member _.GetPropertiesByCategory(category) =
            propEntities
            |> List.filter (fun p -> String.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase))
            |> List.toArray
            :> _

        member _.GetActiveProperties() =
            propEntities
            |> List.filter (fun p -> (not p.ExpiresAt.HasValue) || p.ExpiresAt.Value > DateTime.UtcNow)
            |> List.toArray
            :> _

        member _.IsValid() = true
        member _.IsExpired(_maxAge) = false
        member _.GetMetadata() = Array.empty :> _

        member _.GetDebugInfo() =
            sprintf "SimpleContext(props=%d)" propEntities.Length

        member this.CreateLightweightCopy(includeKeys) =
            let subset =
                includeKeys
                |> Seq.choose (fun k ->
                    propEntities
                    |> List.tryFind (fun p -> p.PropertyKey = k)
                    |> Option.map (fun p -> k, box p.PropertyValue))
                |> Seq.toList

            upcast SimpleContext subset

        member this.CreateCategorizedCopy(categories) =
            let cats = categories |> Seq.map (fun c -> c.ToLowerInvariant()) |> Set.ofSeq

            let subset =
                propEntities
                |> List.filter (fun p -> cats.Contains(p.Category.ToLowerInvariant()))
                |> List.map (fun p -> p.PropertyKey, box p.PropertyValue)

            upcast SimpleContext subset

        member this.Merge(other, overwriteExisting) =
            let otherProps =
                other.GetPropertyKeys()
                |> Seq.choose (fun k ->
                    let cp = other.GetProperty k
                    if cp <> null then Some(k, box cp.PropertyValue) else None)
                |> Seq.toList

            let combined =
                if overwriteExisting then
                    // other overrides
                    let map = props |> List.fold (fun acc (k, v) -> Map.add k v acc) Map.empty
                    let map' = otherProps |> List.fold (fun acc (k, v) -> Map.add k v acc) map
                    map' |> Map.toList
                else
                    // keep existing
                    let existingKeys = props |> List.map fst |> Set.ofList

                    props
                    @ (otherProps |> List.filter (fun (k, _) -> not (existingKeys.Contains k)))

            upcast SimpleContext combined

let mkInterest
    (cat: string)
    (score: decimal)
    (conf: decimal)
    (last: DateTime)
    (subs: string list)
    (tags: string list)
    w
    =
    let subsI: System.Collections.Generic.IList<string> =
        (new System.Collections.Generic.List<string>(subs)) :> _

    let tagsI: System.Collections.Generic.IList<string> =
        (new System.Collections.Generic.List<string>(tags)) :> _

    UserInterest(cat, score, w, conf, last, subsI, tagsI, "Test")

let matcher = UserInterestMatcher()

[<Fact>]
let ``Exact category match produces positive score`` () =
    let categories: System.Collections.Generic.IList<string> =
        upcast System.Collections.Generic.List<string>([ "tech" ])

    let criteria =
        UserInterestTargeting.Create(
            targetCategories = categories,
            minScoreThreshold = 0.0m,
            minConfidenceThreshold = 0.0m,
            includeSubCategories = true
        )

    let ui = mkInterest "tech" 0.6m 0.9m DateTime.UtcNow [] [] 1.0m
    let ctx = ui :> ITargetingContext

    let r =
        (matcher :> ITargetingMatcher).CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal true
    r.MatchScore |> should be (greaterThan 0m)

[<Fact>]
let ``Synonym boosts similarity`` () =
    let categories: System.Collections.Generic.IList<string> =
        upcast System.Collections.Generic.List<string>([ "technology" ])

    let criteria =
        UserInterestTargeting.Create(
            targetCategories = categories,
            minScoreThreshold = 0.0m,
            minConfidenceThreshold = 0.0m,
            includeSubCategories = true
        )

    let ui = mkInterest "tech" 0.7m 0.8m DateTime.UtcNow [] [] 1.0m
    let ctx = ui :> ITargetingContext

    let r =
        (matcher :> ITargetingMatcher).CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal true
    r.MatchScore |> should be (greaterThan 0m)

[<Fact>]
let ``Tag boost increases score up to cap`` () =
    let categories: System.Collections.Generic.IList<string> =
        upcast System.Collections.Generic.List<string>([ "tech" ])

    let tags: System.Collections.Generic.IList<string> =
        upcast System.Collections.Generic.List<string>([ "ai"; "ml" ])

    let criteria =
        UserInterestTargeting.Create(
            targetCategories = categories,
            targetTags = tags,
            minScoreThreshold = 0.0m,
            minConfidenceThreshold = 0.0m,
            includeSubCategories = true
        )

    let ui = mkInterest "tech" 0.9m 0.9m DateTime.UtcNow [] [ "ai" ] 1.0m
    let ctx = ui :> ITargetingContext

    let r =
        (matcher :> ITargetingMatcher).CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

    r.MatchScore |> should be (greaterThan 0.89m)

[<Fact>]
let ``Old interest decays score`` () =
    let categories: System.Collections.Generic.IList<string> =
        upcast System.Collections.Generic.List<string>([ "tech" ])

    let criteria =
        UserInterestTargeting.Create(
            targetCategories = categories,
            minScoreThreshold = 0.0m,
            minConfidenceThreshold = 0.0m,
            includeSubCategories = true
        )

    let recent = mkInterest "tech" 0.8m 0.9m DateTime.UtcNow [] [] 1.0m
    let old = mkInterest "tech" 0.8m 0.9m (DateTime.UtcNow.AddDays(-60)) [] [] 1.0m

    let rRecent =
        (matcher :> ITargetingMatcher).CalculateMatchScoreAsync(recent, criteria, null, CancellationToken.None).Result

    let rOld =
        (matcher :> ITargetingMatcher).CalculateMatchScoreAsync(old, criteria, null, CancellationToken.None).Result

    rRecent.MatchScore |> should be (greaterThan rOld.MatchScore)

[<Fact>]
let ``Threshold filtering prevents low score`` () =
    let categories: System.Collections.Generic.IList<string> =
        upcast System.Collections.Generic.List<string>([ "tech" ])

    let criteria =
        UserInterestTargeting.Create(
            targetCategories = categories,
            minScoreThreshold = 0.9m,
            minConfidenceThreshold = 0.9m,
            includeSubCategories = true
        )

    let ui = mkInterest "tech" 0.5m 0.5m DateTime.UtcNow [] [] 1.0m

    let r =
        (matcher :> ITargetingMatcher).CalculateMatchScoreAsync(ui, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal false

[<Fact>]
let ``IncludeSubCategories expands match set`` () =
    let categories: System.Collections.Generic.IList<string> =
        upcast System.Collections.Generic.List<string>([ "parent" ])

    let criteria =
        UserInterestTargeting.Create(
            targetCategories = categories,
            minScoreThreshold = 0.0m,
            minConfidenceThreshold = 0.0m,
            includeSubCategories = true
        )

    let ui = mkInterest "child" 0.7m 0.8m DateTime.UtcNow [ "parent"; "child" ] [] 1.0m

    let r =
        (matcher :> ITargetingMatcher).CalculateMatchScoreAsync(ui, criteria, null, CancellationToken.None).Result

    r.MatchScore |> should be (greaterThan 0m)
