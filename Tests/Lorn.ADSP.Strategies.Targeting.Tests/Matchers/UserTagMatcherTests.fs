module Lorn.ADSP.Strategies.Targeting.Tests.Matchers.UserTagMatcherTests

open System
open System.Collections.Generic
open System.Threading
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Strategies.Targeting.Matchers
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects

[<Fact>]
let ``Match by tag name Any succeeds`` () =
    // context as strong UserTag
    let tag =
        UserTag("sports", "Interest", confidence = 0.9m, weight = 0.8m, dataSource = "UnitTest")

    let names: IList<string> = upcast List<string>([ "sports" ])
    let criteria = UserTagTargeting.CreateByTagNames(names)
    let matcher = UserTagMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(tag, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal true
    r.MatchScore |> should be (greaterThan 0m)

[<Fact>]
let ``Exclude tag blocks match`` () =
    let tag =
        UserTag("travel", "Interest", confidence = 0.9m, weight = 0.8m, dataSource = "UnitTest")

    let inc: IList<string> = upcast List<string>([ "sports" ])
    let exc: IList<string> = upcast List<string>([ "travel" ])
    let criteria = UserTagTargeting.Create(targetTagNames = inc, excludedTagNames = exc)
    let matcher = UserTagMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(tag, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal false
    // 对字符串使用Contains避免将string视为seq<char>导致的装箱问题
    r.NotMatchReason.Contains("排除") |> should equal true

[<Fact>]
let ``All mode requires all dimensions`` () =
    // build context with multiple tags via properties JSON array under key UserTags
    let tagsJson =
        "[{\"TagName\":\"sports\",\"TagType\":\"Interest\",\"Category\":\"hobby\",\"Confidence\":0.8,\"Weight\":0.7,\"AssignedAt\":\"2024-01-01T00:00:00Z\"}]"

    let props =
        [ ContextProperty("UserTags", tagsJson, "Json", "Tag", false, 1.0m, Nullable(), "UnitTest") ]

    let ctx = TargetingContextBase("TestCtx", props, "UnitTest") :> ITargetingContext

    let n: IList<string> = upcast List<string>([ "sports" ])
    let t: IList<string> = upcast List<string>([ "Interest" ])
    let c: IList<string> = upcast List<string>([ "hobby" ])

    let criteria =
        UserTagTargeting.Create(targetTagNames = n, targetTagTypes = t, targetCategories = c, matchMode = "All")

    let matcher = UserTagMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result
    // Debug print to help diagnose when it fails locally
    if not r.IsMatch then
        printfn
            "AllMode Debug -> IsMatch=%b Score=%A MatchReason=%s Not=%s"
            r.IsMatch
            r.MatchScore
            r.MatchReason
            r.NotMatchReason

    r.IsMatch |> should equal true

[<Fact>]
let ``Expired tag filtered by default`` () =
    let expires = DateTime.UtcNow.AddDays(-1)

    let tag =
        UserTag(
            "old",
            "Interest",
            confidence = 0.9m,
            weight = 0.8m,
            expiresAt = Nullable(expires),
            dataSource = "UnitTest"
        )

    let names2: IList<string> = upcast List<string>([ "old" ])
    let criteria = UserTagTargeting.CreateByTagNames(names2)
    let matcher = UserTagMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(tag, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal false

[<Fact>]
let ``IncludeExpired allows expired`` () =
    let expires = DateTime.UtcNow.AddDays(-1)

    let tag =
        UserTag(
            "old",
            "Interest",
            confidence = 0.9m,
            weight = 0.8m,
            expiresAt = Nullable(expires),
            dataSource = "UnitTest"
        )

    let names3: IList<string> = upcast List<string>([ "old" ])

    let criteria =
        UserTagTargeting.Create(targetTagNames = names3, includeExpiredTags = true)

    let matcher = UserTagMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(tag, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal true

[<Fact>]
let ``Thresholds filter low confidence/weight`` () =
    let tag =
        UserTag("sports", "Interest", confidence = 0.2m, weight = 0.1m, dataSource = "UnitTest")

    let names4: IList<string> = upcast List<string>([ "sports" ])

    let criteria =
        UserTagTargeting.Create(targetTagNames = names4, minConfidenceThreshold = 0.5m, minWeightThreshold = 0.5m)

    let matcher = UserTagMatcher() :> ITargetingMatcher

    let r =
        matcher.CalculateMatchScoreAsync(tag, criteria, null, CancellationToken.None).Result

    r.IsMatch |> should equal false
