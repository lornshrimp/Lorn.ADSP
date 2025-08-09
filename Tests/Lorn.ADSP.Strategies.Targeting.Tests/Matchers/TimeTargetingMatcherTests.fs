namespace Lorn.ADSP.Strategies.Targeting.Tests.Matchers

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Shared.Entities
open Lorn.ADSP.Strategies.Targeting.Matchers
open Lorn.ADSP.Strategies.Targeting.Utils

module TimeTargetingMatcherTests =

    // ---------- 简易 Mock 实现 ----------
    type TestCriteria(rules: IReadOnlyList<TargetingRule>) =
        interface ITargetingCriteria with
            member _.CriteriaId = Guid.NewGuid()
            member _.CriteriaName = "TimeTest"
            member _.CriteriaType = "Time"
            member _.Rules = rules
            member _.Weight = 1.0m
            member _.IsEnabled = true
            member _.CreatedAt = DateTime.UtcNow
            member _.UpdatedAt = DateTime.UtcNow

            member _.GetRule<'T>(key: string) =
                rules
                |> Seq.tryFind (fun r -> r.RuleKey = key)
                |> Option.bind (fun r ->
                    match r.Value with
                    | :? 'T as v -> Some v
                    | _ -> None)
                |> Option.toObj

            member _.GetRule<'T>(key: string, defaultValue: 'T) =
                let v =
                    (rules
                     |> Seq.tryFind (fun r -> r.RuleKey = key)
                     |> Option.bind (fun r ->
                         match r.Value with
                         | :? 'T as v -> Some v
                         | _ -> None))

                match v with
                | Some x -> x
                | None -> defaultValue

            member _.GetRuleObject(key: string) =
                rules |> Seq.tryFind (fun r -> r.RuleKey = key) |> Option.toObj

            member _.HasRule(key: string) =
                rules |> Seq.exists (fun r -> r.RuleKey = key)

            member _.GetRuleKeys() =
                rules
                |> Seq.map (fun r -> r.RuleKey)
                |> HashSet
                |> fun s -> s :> IReadOnlyCollection<_>

            member _.GetRulesByCategory(category: string) =
                rules |> Seq.filter (fun r -> r.Category = category) |> Seq.toList :> _

            member _.GetRequiredRules() = List<TargetingRule>() :> _
            member _.IsValid() = true
            member _.GetConfigurationSummary() = sprintf "Rules=%d" rules.Count

    type TestContext(props: ContextProperty list, ?tz: string) =
        let dict = props |> List.map (fun p -> p.PropertyKey, p) |> dict

        interface ITargetingContext with
            member _.ContextName = "AdRequest"
            member _.ContextType = "Request"
            member _.Properties = props :> IReadOnlyList<_>
            member _.Timestamp = DateTime.UtcNow
            member _.ContextId = Guid.NewGuid()
            member _.DataSource = "UnitTest"

            member _.GetProperty(key) =
                if dict.ContainsKey key then dict[key] else null

            member _.GetPropertyValue<'T>(key) =
                if dict.ContainsKey key then
                    match dict[key].Value with
                    | :? 'T as v -> v
                    | _ -> Unchecked.defaultof<'T>
                else
                    Unchecked.defaultof<'T>

            member _.GetPropertyValue<'T>(key, defaultValue: 'T) =
                if dict.ContainsKey key then
                    match dict[key].Value with
                    | :? 'T as v -> v
                    | _ -> defaultValue
                else
                    defaultValue

            member _.GetPropertyAsString(key) =
                if dict.ContainsKey key && dict[key].Value <> null then
                    dict[key].Value.ToString()
                else
                    String.Empty

            member _.HasProperty(key) = dict.ContainsKey key
            member _.GetPropertyKeys() = dict.Keys :> _

            member _.GetPropertiesByCategory(category) =
                props |> List.filter (fun p -> p.Category = category) :> _

            member _.GetActiveProperties() = props :> _
            member _.IsValid() = true
            member _.IsExpired(_) = false
            member _.GetMetadata() = List<ContextProperty>() :> _
            member _.GetDebugInfo() = "TestContext"
            member this.CreateLightweightCopy(keys) = this :> ITargetingContext
            member this.CreateCategorizedCopy(categories) = this :> ITargetingContext
            member this.Merge(other, overwriteExisting) = this :> ITargetingContext

    let mkRule key value =
        TargetingRule(RuleKey = key, Value = value, Category = "Time", DataType = value.GetType().Name)

    let mkList (xs: 'a list) =
        let l = List<'a>() in
        xs |> List.iter l.Add
        l

    let buildCriteria (rules: (string * obj) list) =
        let rs = rules |> List.map (fun (k, v) -> mkRule k v)
        TestCriteria(rs :> IReadOnlyList<_>) :> ITargetingCriteria

    let prop key value =
        ContextProperty(
            PropertyKey = key,
            Value = value,
            DataType = value.GetType().Name,
            Category = "Time",
            IsSensitive = false,
            Confidence = 1.0m
        )

    [<Fact>]
    let ``单一小时匹配_当前小时在列表中_应匹配`` () =
        // Arrange
        let now = DateTime.UtcNow

        let ctx =
            TestContext(
                [ prop "ClientTimestampUtc" now
                  prop "ClientTimestampReliable" true
                  prop "TimeZone" "UTC" ]
            )
            :> ITargetingContext

        let criteria = buildCriteria [ "Hours", (mkList [ now.Hour ]) :> obj ]
        let matcher = TimeTargetingMatcher() :> ITargetingMatcher
        // Act
        let result =
            matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result
        // Assert
        result.IsMatch |> should equal true
        result.MatchScore |> should equal 1.0m
        result.MatchReason |> should haveSubstring "所有时间条件匹配"

    [<Fact>]
    let ``多维全部匹配_得分应为1`` () =
        let now = DateTime.UtcNow
        let dow = int now.DayOfWeek |> fun d -> if d = 0 then 7 else d

        let season =
            match now.Month with
            | 3
            | 4
            | 5 -> "Spring"
            | 6
            | 7
            | 8 -> "Summer"
            | 9
            | 10
            | 11 -> "Autumn"
            | _ -> "Winter"

        let ctx =
            TestContext(
                [ prop "ClientTimestampUtc" now
                  prop "ClientTimestampReliable" true
                  prop "TimeZone" "UTC" ]
            )
            :> ITargetingContext

        let criteria =
            buildCriteria
                [ "Hours", (mkList [ now.Hour ]) :> obj
                  "DaysOfWeek", (mkList [ dow ]) :> obj
                  "Seasons", (mkList [ season ]) :> obj ]

        let matcher = TimeTargetingMatcher() :> ITargetingMatcher

        let result =
            matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

        result.IsMatch |> should equal true
        result.MatchScore |> should equal 1.0m

    [<Fact>]
    let ``部分匹配_三维中两维匹配_得分为2_3`` () =
        let now = DateTime.UtcNow
        let dow = int now.DayOfWeek |> fun d -> if d = 0 then 7 else d
        let wrongHour = (now.Hour + 1) % 24

        let ctx =
            TestContext(
                [ prop "ClientTimestampUtc" now
                  prop "ClientTimestampReliable" true
                  prop "TimeZone" "UTC" ]
            )
            :> ITargetingContext

        let criteria =
            buildCriteria
                [ "Hours", (mkList [ wrongHour ]) :> obj
                  "DaysOfWeek", (mkList [ dow ]) :> obj
                  "Seasons",
                  (mkList
                      [ match now.Month with
                        | 3
                        | 4
                        | 5 -> "Spring"
                        | 6
                        | 7
                        | 8 -> "Summer"
                        | 9
                        | 10
                        | 11 -> "Autumn"
                        | _ -> "Winter" ])
                  :> obj ]

        let matcher = TimeTargetingMatcher() :> ITargetingMatcher

        let result =
            matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

        result.IsMatch |> should equal false
        Math.Round(result.MatchScore, 2) |> should equal 0.67m

    [<Fact>]
    let ``日期范围匹配_当前日期在范围内`` () =
        let now = DateTime.UtcNow.Date

        let ctx =
            TestContext(
                [ prop "ClientTimestampUtc" (now.AddHours 10.0)
                  prop "ClientTimestampReliable" true
                  prop "TimeZone" "UTC" ]
            )
            :> ITargetingContext

        let criteria =
            buildCriteria
                [ "DateRangeStart", (Nullable(now.AddDays(-1.0))) :> obj
                  "DateRangeEnd", (Nullable(now.AddDays(1.0))) :> obj ]

        let matcher = TimeTargetingMatcher() :> ITargetingMatcher

        let result =
            matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

        result.IsMatch |> should equal true
        result.MatchScore |> should equal 1.0m

    // 自定义节假日 Provider
    type TestHolidayProvider(map: Map<DateTime, string>) =
        interface IHolidayProvider with
            member _.GetHolidayName(date, country, region) = map |> Map.tryFind date
            member _.IsHoliday(date, country, region) = map |> Map.containsKey date

    [<Fact>]
    let ``节假日匹配_使用自定义Provider`` () =
        let today = DateTime.UtcNow.Date
        let hp = TestHolidayProvider(Map.ofList [ today, "TestDay" ]) :> IHolidayProvider

        let ctx =
            TestContext(
                [ prop "ClientTimestampUtc" (today.AddHours 8.0)
                  prop "ClientTimestampReliable" true
                  prop "TimeZone" "UTC"
                  prop "Country" "CN" ]
            )
            :> ITargetingContext

        let criteria = buildCriteria [ "Holidays", (mkList [ "TestDay" ]) :> obj ]
        let matcher = TimeTargetingMatcher(hp) :> ITargetingMatcher

        let result =
            matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

        result.IsMatch |> should equal true
        result.MatchScore |> should equal 1.0m

    [<Fact>]
    let ``CampaignPeriods_在活动期内应匹配`` () =
        let today = DateTime.UtcNow.Date

        let ctx =
            TestContext(
                [ prop "ClientTimestampUtc" (today.AddHours 5.0)
                  prop "ClientTimestampReliable" true
                  prop "TimeZone" "UTC" ]
            )
            :> ITargetingContext

        let period =
            sprintf "%s:%s" (today.AddDays(-1.0).ToString("yyyy-MM-dd")) (today.AddDays(1.0).ToString("yyyy-MM-dd"))

        let criteria = buildCriteria [ "CampaignPeriods", (mkList [ period ]) :> obj ]
        let matcher = TimeTargetingMatcher() :> ITargetingMatcher

        let result =
            matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

        result.IsMatch |> should equal true

    [<Fact>]
    let ``CampaignPeriods_不在活动期内不匹配`` () =
        let today = DateTime.UtcNow.Date

        let ctx =
            TestContext(
                [ prop "ClientTimestampUtc" (today.AddHours 5.0)
                  prop "ClientTimestampReliable" true
                  prop "TimeZone" "UTC" ]
            )
            :> ITargetingContext

        let period =
            sprintf "%s:%s" (today.AddDays(-10.0).ToString("yyyy-MM-dd")) (today.AddDays(-5.0).ToString("yyyy-MM-dd"))

        let criteria = buildCriteria [ "CampaignPeriods", (mkList [ period ]) :> obj ]
        let matcher = TimeTargetingMatcher() :> ITargetingMatcher

        let result =
            matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

        result.IsMatch |> should equal false

    [<Fact>]
    let ``ValidateCriteria_开始晚于结束_应失败`` () =
        let rules =
            [ "DateRangeStart", (Nullable(DateTime.UtcNow.AddDays 1.0)) :> obj
              "DateRangeEnd", (Nullable(DateTime.UtcNow)) :> obj ]

        let criteria = buildCriteria rules
        let matcher = TimeTargetingMatcher() :> ITargetingMatcher
        let vr = matcher.ValidateCriteria(criteria)
        vr.IsValid |> should equal false

    [<Fact>]
    let ``客户端时间不可靠_应使用服务器UTC转换`` () =
        let nowUtc = DateTime.UtcNow

        let ctx =
            TestContext(
                [ prop "ClientTimestampUtc" nowUtc
                  prop "ClientTimestampReliable" false
                  prop "TimeZone" "UTC" ]
            )
            :> ITargetingContext
        // 选择未来一小时，若使用客户端时间会不匹配
        let criteria = buildCriteria [ "Hours", (mkList [ nowUtc.Hour ]) :> obj ]
        let matcher = TimeTargetingMatcher() :> ITargetingMatcher

        let result =
            matcher.CalculateMatchScoreAsync(ctx, criteria, null, CancellationToken.None).Result

        result.IsMatch |> should equal true
