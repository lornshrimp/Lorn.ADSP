namespace Lorn.ADSP.Strategies.Targeting.Tests.Matchers

open System
open System.Collections.Generic
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Strategies.Targeting.Matchers

// 简易 ITargetingCriteria Mock
 type TestCriteria(rules: (string*obj) list, ?ctype:string, ?id:Guid, ?weight:decimal) =
    interface ITargetingCriteria with
        member _.CriteriaId = defaultArg id (Guid.NewGuid())
        member _.CriteriaType = defaultArg ctype "Device"
        member _.CriteriaName = "TestDevice"
        member _.Weight = defaultArg weight 1.0m
        member _.IsEnabled = true
        member _.CreatedAt = DateTime.UtcNow
        member _.UpdatedAt = DateTime.UtcNow
        member _.GetRule<'T>(name:string) =
            match rules |> List.tryFind (fun (k,_) -> k = name) with
            | Some (_,v) ->
                match v with
                | :? 'T as tv -> tv
                | _ -> Unchecked.defaultof<'T>
            | None -> Unchecked.defaultof<'T>
        member _.SetRule(_,_) = ()
        member _.HasRule(name) = rules |> List.exists (fun (k,_) -> k = name)
        member _.GetConfigurationSummary() = ""

 // 简易 ITargetingContext Mock
 type TestContext(props: (string*obj) list) =
    interface ITargetingContext with
        member _.ContextId = Guid.NewGuid()
        member _.ContextType = "Device"
        member _.UserId = Guid.Empty
        member _.SessionId = ""
        member _.RequestId = Guid.NewGuid()
        member _.TimestampUtc = DateTime.UtcNow
        member _.IsValid() = true
        member _.GetPropertyValue<'T>(name:string) =
            match props |> List.tryFind (fun (k,_) -> k = name) with
            | Some (_,v) ->
                match v with
                | :? 'T as tv -> tv
                | _ -> Unchecked.defaultof<'T>
            | None -> Unchecked.defaultof<'T>
        member _.HasProperty(name) = props |> List.exists (fun (k,_) -> k = name)
        member _.GetPropertyAsString(name) =
            match props |> List.tryFind (fun (k,_) -> k = name) with
            | Some (_,v) -> v.ToString()
            | None -> null
        member _.GetPropertiesByCategory(_) = ResizeArray()
        member _.AddOrUpdateProperty(_,_,_,_,_,_) = ()
        member _.GetAllProperties() = ResizeArray()
        member _.GetDebugInfo() = ""

module Helper =
    let listStr xs = xs |> List<string> |> fun l -> l

open Helper

module DeviceTargetingMatcherTests =
    [<Fact>]
    let ``Full match with all attributes`` () =
        let criteria = TestCriteria([
            "OperatingSystems", listStr ["android"; "ios"] :> obj
            "Brands", listStr ["Apple"] :> obj
            "Models", listStr ["iPhone 14 Pro"] :> obj
            "DeviceTypes", listStr ["Smartphone"] :> obj
            "CapabilityTiers", listStr ["High"] :> obj
            "MinOSVersion", box "16.0"
            "TargetOSVersion", box "16.0"
        ]) :> ITargetingCriteria

        let context = TestContext([
            "OperatingSystem", box "iOS"
            "OSVersion", box "16.3.1"
            "Brand", box "iPhone" // 别名 -> Apple
            "Model", box "iPhone 14 Pro"
            "DeviceType", box "Smartphone"
            "ScreenWidth", box 1290
            "ScreenHeight", box 2796
        ]) :> ITargetingContext

        let matcher = DeviceTargetingMatcher() :> ITargetingMatcher
        let result = matcher.CalculateMatchScoreAsync(context, criteria, null, CancellationToken.None).Result
        result.IsMatch |> should be True
        result.MatchScore |> should (equal) 1.0m

    [<Fact>]
    let ``Brand alias normalization works`` () =
        let criteria = TestCriteria([
            "Brands", listStr ["Apple"] :> obj
        ]) :> ITargetingCriteria
        let context = TestContext([
            "Brand", box "蘋果"
        ]) :> ITargetingContext
        let matcher = DeviceTargetingMatcher() :> ITargetingMatcher
        let result = matcher.CalculateMatchScoreAsync(context, criteria, null, CancellationToken.None).Result
        result.IsMatch |> should be True

    [<Fact>]
    let ``OS fast fail when not in list`` () =
        let criteria = TestCriteria([
            "OperatingSystems", listStr ["android"] :> obj
            "Brands", listStr ["Samsung"] :> obj
        ]) :> ITargetingCriteria
        let context = TestContext([
            "OperatingSystem", box "iOS"
            "Brand", box "Apple"
        ]) :> ITargetingContext
        let matcher = DeviceTargetingMatcher() :> ITargetingMatcher
        let result = matcher.CalculateMatchScoreAsync(context, criteria, null, CancellationToken.None).Result
        result.IsMatch |> should be False
        result.MatchScore |> should (equal) 0.0m

    [<Fact>]
    let ``Partial score when one of two conditions matches`` () =
        let criteria = TestCriteria([
            "Brands", listStr ["Apple"; "Huawei"] :> obj
            "Models", listStr ["P60"] :> obj
        ]) :> ITargetingCriteria
        let context = TestContext([
            "Brand", box "Apple"
            "Model", box "iPhone 13"
        ]) :> ITargetingContext
        let matcher = DeviceTargetingMatcher() :> ITargetingMatcher
        let result = matcher.CalculateMatchScoreAsync(context, criteria, null, CancellationToken.None).Result
        result.IsMatch |> should be False
        // 2 条件中品牌匹配1个 -> score 0.5
        Math.Round(result.MatchScore,2) |> should (equal) 0.50m

    [<Fact>]
    let ``Min version requirement satisfied`` () =
        let criteria = TestCriteria([
            "MinOSVersion", box "15.0"
        ]) :> ITargetingCriteria
        let context = TestContext([
            "OSVersion", box "16.2"
        ]) :> ITargetingContext
        let matcher = DeviceTargetingMatcher() :> ITargetingMatcher
        let result = matcher.CalculateMatchScoreAsync(context, criteria, null, CancellationToken.None).Result
        result.IsMatch |> should be True

    [<Fact>]
    let ``Min version requirement not satisfied`` () =
        let criteria = TestCriteria([
            "MinOSVersion", box "17.0"
        ]) :> ITargetingCriteria
        let context = TestContext([
            "OSVersion", box "16.2"
        ]) :> ITargetingContext
        let matcher = DeviceTargetingMatcher() :> ITargetingMatcher
        let result = matcher.CalculateMatchScoreAsync(context, criteria, null, CancellationToken.None).Result
        result.IsMatch |> should be False

    [<Fact>]
    let ``Backward compatibility fails when major version lower`` () =
        let criteria = TestCriteria([
            "TargetOSVersion", box "17.0"
        ]) :> ITargetingCriteria
        let context = TestContext([
            "OSVersion", box "16.5"
        ]) :> ITargetingContext
        let matcher = DeviceTargetingMatcher() :> ITargetingMatcher
        let result = matcher.CalculateMatchScoreAsync(context, criteria, null, CancellationToken.None).Result
        result.IsMatch |> should be False

    [<Fact>]
    let ``Capability tier inferred from resolution`` () =
        let criteria = TestCriteria([
            "CapabilityTiers", listStr ["High"] :> obj
        ]) :> ITargetingCriteria
        let context = TestContext([
            "Brand", box "Apple"
            "Model", box "iPhone 14 Pro"
            "ScreenWidth", box 1290
            "ScreenHeight", box 2796
        ]) :> ITargetingContext
        let matcher = DeviceTargetingMatcher() :> ITargetingMatcher
        let result = matcher.CalculateMatchScoreAsync(context, criteria, null, CancellationToken.None).Result
        result.IsMatch |> should be True
