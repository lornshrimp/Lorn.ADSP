namespace Lorn.ADSP.Strategies.Targeting.Matchers

open System
open System.Collections.Generic
open System.Globalization
open System.Threading
open System.Threading.Tasks
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Shared.Entities
open Lorn.ADSP.Strategies.Targeting.Utils
open Lorn.ADSP.Strategies.Targeting.Utils.TimePrecomputeCache

// 内部执行结果（顶层定义避免类型内布局引起的解析问题）
type TimeExecInternalResult =
    { IsMatch: bool
      MatchScore: decimal
      MatchReason: string
      NotMatchReason: string
      MatchDetails: IReadOnlyList<ContextProperty> }

/// 时间定向匹配器
/// 支持多维度：周几、小时、日期范围、节假日、季节
/// 需求覆盖: 3.1(时区),3.2(客户端时间不可靠处理),3.3(多维匹配并行/聚合),3.4(结果返回)
type TimeTargetingMatcher(?holidayProvider: IHolidayProvider) =

    // ---- ITargetingMatcher 接口实现 ----
    interface ITargetingMatcher with
        member _.MatcherId = "time-matcher-v1"
        member _.MatcherName = "时间定向匹配器"
        member _.Version = "1.0.0"
        member _.MatcherType = "Time"
        member _.Priority = 120
        member _.IsEnabled = true
        member _.ExpectedExecutionTime = TimeSpan.FromMilliseconds(8.)
        member _.CanRunInParallel = true

        member this.CalculateMatchScoreAsync
            (context: ITargetingContext, criteria: ITargetingCriteria, callbackProvider, _ct: CancellationToken)
            : Task<MatchResult> =
            Task.Run(fun () ->
                let startUtc = DateTime.UtcNow

                try
                    if isNull (box criteria) then
                        MatchResult.CreateNoMatch("Time", Guid.NewGuid(), "定向条件为空", TimeSpan.Zero, 0, 0m, false)
                    elif isNull (box context) then
                        MatchResult.CreateNoMatch(
                            criteria.CriteriaType,
                            criteria.CriteriaId,
                            "用户上下文为空",
                            TimeSpan.Zero,
                            0,
                            0m,
                            false
                        )
                    elif not ((this :> ITargetingMatcher).IsSupported(criteria.CriteriaType)) then
                        MatchResult.CreateNoMatch(
                            criteria.CriteriaType,
                            criteria.CriteriaId,
                            sprintf "不支持的定向条件类型: %s" criteria.CriteriaType,
                            TimeSpan.Zero,
                            0,
                            0m,
                            false
                        )
                    else
                        let exec = this.ExecuteLogic(context, criteria)
                        let elapsed = DateTime.UtcNow - startUtc

                        if exec.IsMatch then
                            MatchResult.CreateMatch(
                                criteria.CriteriaType,
                                criteria.CriteriaId,
                                exec.MatchScore,
                                exec.MatchReason,
                                elapsed,
                                0,
                                criteria.Weight,
                                false,
                                exec.MatchDetails
                            )
                        else
                            MatchResult.CreateNoMatch(
                                criteria.CriteriaType,
                                criteria.CriteriaId,
                                exec.NotMatchReason,
                                elapsed,
                                0,
                                criteria.Weight,
                                false,
                                exec.MatchDetails
                            )
                with ex ->
                    let elapsed = DateTime.UtcNow - startUtc

                    let cid =
                        if isNull (box criteria) then
                            Guid.NewGuid()
                        else
                            criteria.CriteriaId

                    MatchResult.CreateNoMatch("Time", cid, sprintf "匹配计算异常: %s" ex.Message, elapsed, 0, 0m, false))

        member _.IsSupported(criteriaType: string) =
            [ "Time"; "Schedule"; "Temporal" ] |> List.contains criteriaType

        member _.ValidateCriteria(criteria: ITargetingCriteria) =
            try
                if isNull (box criteria) then
                    ValidationResult.Failure([ ValidationError(Message = "定向条件不能为空") ], "验证失败", TimeSpan.Zero)
                else
                    let hasRule (names: string list) = names |> List.exists criteria.HasRule

                    if
                        not (
                            hasRule
                                [ "DaysOfWeek"
                                  "Hours"
                                  "DateRangeStart"
                                  "DateRangeEnd"
                                  "Holidays"
                                  "Seasons" ]
                        )
                    then
                        ValidationResult.Failure([ ValidationError(Message = "至少配置一个时间定向条件") ], "验证失败", TimeSpan.Zero)
                    else
                        let startDate = criteria.GetRule<Nullable<DateTime>>("DateRangeStart")
                        let endDate = criteria.GetRule<Nullable<DateTime>>("DateRangeEnd")

                        if startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value then
                            ValidationResult.Failure(
                                [ ValidationError(Message = "开始日期不能晚于结束日期") ],
                                "验证失败",
                                TimeSpan.Zero
                            )
                        else
                            ValidationResult.Success("时间定向条件验证通过", TimeSpan.Zero)
            with ex ->
                ValidationResult.Failure(
                    [ ValidationError(Message = sprintf "验证异常: %s" ex.Message) ],
                    "验证异常",
                    TimeSpan.Zero
                )

        member _.GetMetadata() =
            TargetingMatcherMetadata(
                MatcherId = "time-matcher-v1",
                Name = "时间定向匹配器",
                Description = "支持周几/小时/日期/节假日/季节的多维时间匹配",
                Version = "1.0.0",
                MatcherType = "Time",
                Author = "Lorn.ADSP Team",
                SupportedCriteriaTypes = [ "Time"; "Schedule"; "Temporal" ],
                SupportedDimensions = [ "DayOfWeek"; "Hour"; "Date"; "Holiday"; "Season" ],
                SupportsParallelExecution = true,
                SupportsCaching = true,
                SupportsBatchProcessing = false,
                ExpectedExecutionTime = TimeSpan.FromMilliseconds(8.),
                MaxExecutionTime = TimeSpan.FromMilliseconds(80.)
            )

    // 提取上下文中的时区 (优先级: 用户 -> 请求 -> 默认UTC)
    member private _.ExtractTimeZone(context: ITargetingContext) =
        try
            if context.HasProperty("TimeZone") then
                let tzId = context.GetPropertyAsString("TimeZone")

                if String.IsNullOrWhiteSpace tzId then
                    TimeZoneInfo.Utc
                else
                    TimeZoneInfo.FindSystemTimeZoneById(tzId)
            else
                TimeZoneInfo.Utc
        with _ ->
            TimeZoneInfo.Utc

    // 获取当前用户本地时间（考虑客户端不可靠: 若HasProperty("ClientTimestampReliable")=false则使用服务器UTC转换）
    member private this.GetUserLocalNow(context: ITargetingContext) =
        let tz = this.ExtractTimeZone context

        let clientTsReliable =
            if context.HasProperty("ClientTimestampReliable") then
                context.GetPropertyValue<bool>("ClientTimestampReliable")
            else
                false

        let baseUtc =
            if clientTsReliable && context.HasProperty("ClientTimestampUtc") then
                context.GetPropertyValue<DateTime>("ClientTimestampUtc")
            else
                DateTime.UtcNow

        TimeZoneInfo.ConvertTimeFromUtc(baseUtc, tz), tz

    // 获取规则集合
    member private _.GetRuleList<'T> (criteria: ITargetingCriteria) (ruleName: string) : List<'T> =
        let lst = criteria.GetRule<List<'T>>(ruleName)
        if isNull lst then List<'T>() else lst

    // 计算匹配逻辑
    member private this.ExecuteLogic
        (context: ITargetingContext, criteria: ITargetingCriteria)
        : TimeExecInternalResult =
        let nowLocal, tz = this.GetUserLocalNow context
        let pre = TimePrecomputeCache.get nowLocal
        // 维度：周几 (1-7 兼容), 小时(0-23), 日期范围, 节假日, 季节
        let days = this.GetRuleList<int> criteria "DaysOfWeek" // 1=Monday? 需要与系统对齐: DayOfWeek Monday=1 但 .NET Monday=1? -> DayOfWeek.Monday=1 (Sunday=0)
        let hours = this.GetRuleList<int> criteria "Hours"
        let seasonRules = this.GetRuleList<string> criteria "Seasons"
        let holidayRules = this.GetRuleList<string> criteria "Holidays"
        let campaignPeriods = this.GetRuleList<string> criteria "CampaignPeriods" // 期望格式: yyyy-MM-dd:yyyy-MM-dd，可多个
        let startDate = criteria.GetRule<Nullable<DateTime>>("DateRangeStart")
        let endDate = criteria.GetRule<Nullable<DateTime>>("DateRangeEnd")

        // 计算各维度匹配
        let dayMatch, dayHas =
            if days.Count = 0 then
                true, false
            else
                let dow = pre.DayOfWeekNorm // 使用预计算
                // 允许规则使用 0-6 或 1-7; 统一转换: 若存在>6 则视为1-7模式
                let normalize d =
                    if days |> Seq.exists (fun v -> v > 6) then
                        (if d = 0 then 7 else d)
                    else
                        d

                let dowNorm = normalize dow
                days |> Seq.exists (fun d -> d = dowNorm), true

        let hourMatch, hourHas =
            if hours.Count = 0 then
                true, false
            else
                let h = nowLocal.Hour
                hours |> Seq.contains h, true

        let dateMatch, dateHas =
            if startDate.HasValue || endDate.HasValue then
                let d = nowLocal.Date

                let okStart =
                    if startDate.HasValue then
                        d >= startDate.Value.Date
                    else
                        true

                let okEnd = if endDate.HasValue then d <= endDate.Value.Date else true
                okStart && okEnd, true
            else
                true, false

        // 简化节假日：如果上下文提供 HolidayName 则直接匹配
        let holidayMatch, holidayHas =
            if holidayRules.Count = 0 then
                true, false
            else
                let ctxHoliday =
                    if context.HasProperty("HolidayName") then
                        context.GetPropertyAsString("HolidayName")
                    else
                        null

                if String.IsNullOrWhiteSpace ctxHoliday then
                    false, true
                else
                    (holidayRules
                     |> Seq.exists (fun h -> String.Equals(h, ctxHoliday, StringComparison.OrdinalIgnoreCase))),
                    true

        let seasonMatch, seasonHas =
            if seasonRules.Count = 0 then
                true, false
            else
                let season = pre.Season

                seasonRules
                |> Seq.exists (fun s -> String.Equals(s, season, StringComparison.OrdinalIgnoreCase)),
                true

        // 节假日使用 provider (优先) 或 context / 规则对比
        let holidayMatchEnhanced, holidayHasEnhanced =
            if holidayRules.Count = 0 then
                true, false
            else
                let country =
                    if context.HasProperty("Country") then
                        context.GetPropertyAsString("Country")
                    else
                        ""

                let region =
                    if context.HasProperty("Region") then
                        context.GetPropertyAsString("Region") |> Some
                    else
                        None

                let provider =
                    defaultArg holidayProvider (InMemoryHolidayProvider(DefaultHolidaySets.sample) :> IHolidayProvider)

                let nameOpt = provider.GetHolidayName(nowLocal.Date, country, region)

                match nameOpt with
                | Some hn ->
                    holidayRules
                    |> Seq.exists (fun h -> h.Equals(hn, StringComparison.OrdinalIgnoreCase)),
                    true
                | None -> false, true

        // 特殊营销活动期间: CampaignPeriods (多个 dateStart:dateEnd)
        let campaignMatch, campaignHas =
            if campaignPeriods.Count = 0 then
                true, false
            else
                let today = nowLocal.Date

                let parsePeriod (s: string) =
                    let parts = s.Split(':')

                    if parts.Length = 2 then
                        match DateTime.TryParse(parts[0]), DateTime.TryParse(parts[1]) with
                        | (true, ds), (true, de) -> Some(ds.Date, de.Date)
                        | _ -> None
                    else
                        None

                let anyActive =
                    campaignPeriods
                    |> Seq.choose parsePeriod
                    |> Seq.exists (fun (ds, de) -> today >= ds && today <= de)

                anyActive, true

        // 聚合
        let dimensionList =
            [ ("DayOfWeek", dayMatch, dayHas)
              ("Hour", hourMatch, hourHas)
              ("DateRange", dateMatch, dateHas)
              ("Holiday", holidayMatch, holidayHas)
              ("HolidayProvider", holidayMatchEnhanced, holidayHasEnhanced)
              ("Season", seasonMatch, seasonHas)
              ("CampaignPeriod", campaignMatch, campaignHas) ]

        let active = dimensionList |> List.filter (fun (_, _, has) -> has)
        let allMatch = active |> List.forall (fun (_, m, _) -> m)
        let matchedCount = active |> List.filter (fun (_, m, _) -> m) |> List.length
        let totalCount = active.Length

        let score =
            if totalCount = 0 then
                0m
            else
                decimal matchedCount / decimal totalCount

        let details: IReadOnlyList<ContextProperty> =
            dimensionList
            |> List.filter (fun (_, _, has) -> has)
            |> List.map (fun (n, m, _) ->
                let value = if m then "匹配" else "不匹配"
                ContextProperty(n, value, "String", "TimeMatch", false, 1.0m, Nullable(), "TimeTargetingMatcher"))
            |> List.toArray
            :> IReadOnlyList<_>

        if allMatch then
            { IsMatch = true
              MatchScore = score
              MatchReason = "所有时间条件匹配"
              NotMatchReason = ""
              MatchDetails = details }
        else
            let notMatchedNames =
                active
                |> List.filter (fun (_, m, _) -> not m)
                |> List.map (fun (n, _, _) -> n)
                |> String.concat ", "

            { IsMatch = false
              MatchScore = score
              MatchReason = ""
              NotMatchReason = $"未匹配的时间维度: {notMatchedNames}"
              MatchDetails = details }
