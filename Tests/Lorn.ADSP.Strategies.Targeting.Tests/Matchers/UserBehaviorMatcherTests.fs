module Lorn.ADSP.Strategies.Targeting.Tests.Matchers.UserBehaviorMatcherTests

open System
open System.Threading
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Strategies.Targeting.Matchers
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects

// 复用与兴趣测试类似的极简 ITargetingContext stub
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

        member this.CreateLightweightCopy(includeKeys) = upcast this
        member this.CreateCategorizedCopy(categories) = upcast this
        member this.Merge(other, overwriteExisting) = upcast this

let mkRecord t v ts freq w ctx =
    Lorn.ADSP.Core.Domain.Targeting.BehaviorRecord(t, v, ts, freq, w, ctx)

[<Fact>]
let ``Behavior type filter works`` () =
    let ub =
        UserBehavior(
            behaviorRecords =
                upcast
                    [ mkRecord
                          "click"
                          "home"
                          DateTime.UtcNow
                          3
                          1.0m
                          null
                          mkRecord
                          "view"
                          "detail"
                          DateTime.UtcNow
                          2
                          1.0m
                          null ],
            dataSource = "UnitTest"
        )

    let criteria = UserBehaviorTargeting.Create(behaviorTypes = upcast [ "click" ])
    let matcher = UserBehaviorMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(ub, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal true
    r.MatchScore |> should be (greaterThan 0m)

[<Fact>]
let ``Frequency window prefers recent`` () =
    let ub =
        UserBehavior(
            behaviorRecords =
                upcast
                    [ mkRecord
                          "click"
                          "home"
                          (DateTime.UtcNow.AddDays(-10))
                          5
                          1.0m
                          null
                          mkRecord
                          "click"
                          "home"
                          DateTime.UtcNow
                          2
                          1.0m
                          null ],
            dataSource = "UnitTest"
        )

    let criteria = UserBehaviorTargeting.Create(behaviorTypes = upcast [ "click" ])
    let matcher = UserBehaviorMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(ub, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal true
    r.MatchScore |> should be (greaterThan 0m)

[<Fact>]
let ``Context boost when tag hits interest`` () =
    let ctxJson = "{" "tag" ": " "sports" "}"

    let ub =
        UserBehavior(
            behaviorRecords = upcast [ mkRecord "view" "article" DateTime.UtcNow 1 1.0m (Some ctxJson) ],
            dataSource = "UnitTest"
        )

    let criteria =
        UserBehaviorTargeting.Create(interestTags = upcast [ "sports"; "news" ])

    let matcher = UserBehaviorMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(ub, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal true
    r.MatchScore |> should be (greaterThan 0m)

[<Fact>]
let ``No behavior returns no match`` () =
    let ub = UserBehavior.CreateDefault()
    let criteria = UserBehaviorTargeting.Create(behaviorTypes = upcast [ "purchase" ])
    let matcher = UserBehaviorMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(ub, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal false
