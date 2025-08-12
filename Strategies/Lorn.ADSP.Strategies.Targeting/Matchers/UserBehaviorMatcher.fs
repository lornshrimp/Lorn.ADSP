namespace Lorn.ADSP.Strategies.Targeting.Matchers

open System
open System.Collections.Generic
open System.Text.Json
open System.Threading
open System.Threading.Tasks
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Shared.Entities

/// 用户行为定向匹配器
/// - 行为类型分类匹配
/// - 频次统计算法（时间窗口）
/// - 权重与时间衰减评分
/// - JSON 上下文解析与模式识别（简化版）
/// 参照需求: 9.1~9.6, 7.1~7.2
module private BehaviorHelpers =
    let normalize (s: string) =
        if String.IsNullOrWhiteSpace s then
            ""
        else
            s.Trim().ToLowerInvariant()

    // 指数衰减：score * exp(-lambda * days)
    let decay (lambda: decimal) (time: DateTime) =
        let days = (DateTime.UtcNow - time).TotalDays |> decimal |> max 0m
        Math.Exp(float (-lambda * days)) |> decimal |> max 0m |> min 1m

    // 尝试解析 JSON 上下文，返回键值对
    let tryParseJsonContext (json: string) : IDictionary<string, obj> =
        let dict = Dictionary<string, obj>() :> IDictionary<string, obj>

        try
            let el = JsonDocument.Parse(json).RootElement

            if el.ValueKind = JsonValueKind.Object then
                for p in el.EnumerateObject() do
                    match p.Value.ValueKind with
                    | JsonValueKind.String -> dict.[p.Name] <- box (p.Value.GetString())
                    | JsonValueKind.Number ->
                        match p.Value.TryGetInt64() with
                        | true, v -> dict.[p.Name] <- box v
                        | _ ->
                            match p.Value.TryGetDouble() with
                            | true, v -> dict.[p.Name] <- box v
                            | _ -> ()
                    | JsonValueKind.True -> dict.[p.Name] <- box true
                    | JsonValueKind.False -> dict.[p.Name] <- box false
                    | _ -> ()
        with _ ->
            ()

        dict

    // 时间序列模式：在窗口内是否存在阈值以上的连续行为（简化：累计频次>=阈值）
    let hasBurst
        (records: IReadOnlyList<Lorn.ADSP.Core.Domain.Targeting.BehaviorRecord>)
        (window: TimeSpan)
        (threshold: int)
        =
        if isNull records || records.Count = 0 then
            false
        else
            let sorted = records |> Seq.sortBy (fun r -> r.Timestamp) |> Seq.toArray
            let mutable i = 0
            let mutable j = 0
            let mutable acc = 0
            let mutable ok = false

            while i < sorted.Length && not ok do
                while j < sorted.Length && (sorted[j].Timestamp - sorted[i].Timestamp) <= window do
                    acc <- acc + max 1 sorted[j].Frequency
                    j <- j + 1

                if acc >= threshold then
                    ok <- true
                // slide
                acc <- acc - max 1 sorted[i].Frequency
                i <- i + 1

            ok

open BehaviorHelpers

type UserBehaviorMatcher(?lambda: decimal) =
    let lambda = defaultArg lambda 0.08m // 行为较快衰减

    interface ITargetingMatcher with
        member _.MatcherId = "behavior-matcher-v1"
        member _.MatcherName = "用户行为定向匹配器"
        member _.Version = "1.0.0"
        member _.MatcherType = "Behavior"
        member _.Priority = 110
        member _.IsEnabled = true
        member _.ExpectedExecutionTime = TimeSpan.FromMilliseconds 8.
        member _.CanRunInParallel = true

        member _.IsSupported(t: string) =
            String.Equals(t, "Behavior", StringComparison.OrdinalIgnoreCase)

        member this.ValidateCriteria(criteria: ITargetingCriteria) =
            if isNull (box criteria) then
                ValidationResult.Failure([ ValidationError(Message = "定向条件为空") ], "验证失败", TimeSpan.Zero)
            elif not (String.Equals(criteria.CriteriaType, "Behavior", StringComparison.OrdinalIgnoreCase)) then
                let msg = sprintf "不支持的定向类型: %s" criteria.CriteriaType
                ValidationResult.Failure([ ValidationError(Message = msg) ], "验证失败", TimeSpan.Zero)
            elif not (criteria :? UserBehaviorTargeting) then
                ValidationResult.Failure(
                    [ ValidationError(Message = "Criteria 不是 UserBehaviorTargeting") ],
                    "验证失败",
                    TimeSpan.Zero
                )
            else
                let c = criteria :?> UserBehaviorTargeting
                let hasInterests = not (c.InterestTags :> seq<_> |> Seq.isEmpty)
                let hasTypes = not (c.BehaviorTypes :> seq<_> |> Seq.isEmpty)

                if (not hasInterests) && (not hasTypes) then
                    ValidationResult.Failure([ ValidationError(Message = "必须配置兴趣标签或行为类型") ], "验证失败", TimeSpan.Zero)
                else
                    ValidationResult.Success("行为定向条件验证通过", TimeSpan.Zero)

        member _.CalculateMatchScoreAsync
            (context: ITargetingContext, criteria: ITargetingCriteria, _callbackProvider, ct: CancellationToken)
            : Task<MatchResult> =
            Task.Run(fun () ->
                let start = DateTime.UtcNow

                try
                    if ct.IsCancellationRequested then
                        MatchResult.CreateNoMatch(
                            criteria.CriteriaType,
                            criteria.CriteriaId,
                            "取消",
                            TimeSpan.Zero,
                            0,
                            criteria.Weight,
                            false,
                            null
                        )
                    elif isNull (box criteria) || not (criteria :? UserBehaviorTargeting) then
                        MatchResult.CreateNoMatch(
                            criteria.CriteriaType,
                            criteria.CriteriaId,
                            "不支持的条件",
                            TimeSpan.Zero,
                            0,
                            criteria.Weight,
                            false,
                            null
                        )
                    else
                        let c = criteria :?> UserBehaviorTargeting
                        // 从上下文提取 UserBehavior 或 BehaviorRecords
                        let behaviors = ResizeArray<Lorn.ADSP.Core.Domain.Targeting.BehaviorRecord>()

                        if context :? UserBehavior then
                            let ub = context :?> UserBehavior

                            if not (isNull (box ub.BehaviorRecords)) then
                                for r in ub.BehaviorRecords do
                                    if r <> null then
                                        behaviors.Add r
                            // 若单一字段存在也加入
                            if
                                (not (String.IsNullOrWhiteSpace ub.BehaviorType))
                                && (not (String.IsNullOrWhiteSpace ub.BehaviorValue))
                                && ub.BehaviorTimestamp.HasValue
                            then
                                behaviors.Add(
                                    Lorn.ADSP.Core.Domain.Targeting.BehaviorRecord(
                                        ub.BehaviorType,
                                        ub.BehaviorValue,
                                        ub.BehaviorTimestamp.Value,
                                        ub.Frequency,
                                        ub.Weight,
                                        ub.BehaviorContext
                                    )
                                )
                        else
                            // 兜底：尝试从属性中恢复（可选）
                            ()

                        if behaviors.Count = 0 then
                            MatchResult.CreateNoMatch(
                                c.CriteriaType,
                                c.CriteriaId,
                                "无用户行为数据",
                                DateTime.UtcNow - start,
                                0,
                                c.Weight,
                                false,
                                null
                            )
                        else
                            // 1) 按类型分类并筛选
                            let typesSet = c.BehaviorTypes |> Seq.map normalize |> Set.ofSeq
                            let interestSet = c.InterestTags |> Seq.map normalize |> Set.ofSeq

                            let matches =
                                behaviors
                                |> Seq.choose (fun r ->
                                    let t = normalize r.BehaviorType

                                    if typesSet.Count = 0 || typesSet.Contains t then
                                        Some r
                                    else
                                        None)
                                |> Seq.toArray

                            if matches.Length = 0 then
                                MatchResult.CreateNoMatch(
                                    c.CriteriaType,
                                    c.CriteriaId,
                                    "无匹配行为类型",
                                    DateTime.UtcNow - start,
                                    0,
                                    c.Weight,
                                    false,
                                    null
                                )
                            else
                                // 2) 频次统计：最近 N 天窗口（默认 7 天）
                                let windowDays = 7
                                let window = TimeSpan.FromDays(float windowDays)

                                let recent =
                                    matches |> Array.filter (fun r -> (DateTime.UtcNow - r.Timestamp) <= window)

                                let totalFreq = matches |> Array.sumBy (fun r -> max 1 r.Frequency)
                                let recentFreq = recent |> Array.sumBy (fun r -> max 1 r.Frequency)

                                // 3) JSON 上下文增强：若包含 category/tag 与 interestSet 交集，则加分
                                let ctxBoost =
                                    matches
                                    |> Array.tryPick (fun r ->
                                        if String.IsNullOrWhiteSpace r.Context then
                                            None
                                        else
                                            let kv = tryParseJsonContext r.Context

                                            let cat =
                                                if kv.ContainsKey "category" then
                                                    kv.["category"].ToString() |> normalize
                                                else
                                                    ""

                                            let tag =
                                                if kv.ContainsKey "tag" then
                                                    kv.["tag"].ToString() |> normalize
                                                else
                                                    ""

                                            let hit =
                                                [ cat; tag ]
                                                |> List.filter (fun x -> x <> "")
                                                |> List.exists (fun x -> interestSet.Contains x)

                                            if hit then Some 0.1m else None)
                                    |> Option.defaultValue 0m

                                // 4) 时间衰减 + 权重评分
                                let rawScore = decimal recentFreq / decimal (max 1 totalFreq)

                                let decayed =
                                    matches
                                    |> Array.sumBy (fun r ->
                                        (max 1 r.Frequency |> decimal)
                                        * (decay lambda r.Timestamp)
                                        * (max 0m r.Weight))

                                let denom =
                                    matches
                                    |> Array.sumBy (fun r -> (max 1 r.Frequency |> decimal) * (max 0m r.Weight))
                                    |> max 0.0001m

                                let timeWeighted = (decayed / denom) |> min 1m

                                // 5) 行为“爆发”模式
                                let asList: IReadOnlyList<Lorn.ADSP.Core.Domain.Targeting.BehaviorRecord> =
                                    let l =
                                        new System.Collections.Generic.List<
                                            Lorn.ADSP.Core.Domain.Targeting.BehaviorRecord
                                         >(
                                            matches
                                        )

                                    l :> _

                                let burst =
                                    if hasBurst asList (TimeSpan.FromHours 1.) 5 then
                                        0.1m
                                    else
                                        0m

                                let score = rawScore * 0.4m + timeWeighted * 0.5m + ctxBoost + burst |> min 1m

                                let details: IReadOnlyList<ContextProperty> =
                                    [| ContextProperty(
                                           "TotalFrequency",
                                           totalFreq.ToString(),
                                           "Int32",
                                           "Behavior",
                                           false,
                                           1.0m,
                                           Nullable(),
                                           "UserBehaviorMatcher"
                                       )
                                       ContextProperty(
                                           "RecentFrequency",
                                           recentFreq.ToString(),
                                           "Int32",
                                           "Behavior",
                                           false,
                                           1.0m,
                                           Nullable(),
                                           "UserBehaviorMatcher"
                                       )
                                       ContextProperty(
                                           "ContextBoost",
                                           ctxBoost.ToString("0.###"),
                                           "Decimal",
                                           "Behavior",
                                           false,
                                           1.0m,
                                           Nullable(),
                                           "UserBehaviorMatcher"
                                       )
                                       ContextProperty(
                                           "TimeWeighted",
                                           timeWeighted.ToString("0.###"),
                                           "Decimal",
                                           "Behavior",
                                           false,
                                           1.0m,
                                           Nullable(),
                                           "UserBehaviorMatcher"
                                       ) |]
                                    :> _

                                if score > 0m then
                                    MatchResult.CreateMatch(
                                        c.CriteriaType,
                                        c.CriteriaId,
                                        score,
                                        "行为匹配成功",
                                        DateTime.UtcNow - start,
                                        0,
                                        c.Weight,
                                        false,
                                        details
                                    )
                                else
                                    MatchResult.CreateNoMatch(
                                        c.CriteriaType,
                                        c.CriteriaId,
                                        "行为匹配得分为0",
                                        DateTime.UtcNow - start,
                                        0,
                                        c.Weight,
                                        false,
                                        details
                                    )
                with ex ->
                    MatchResult.CreateNoMatch(
                        "Behavior",
                        criteria.CriteriaId,
                        $"匹配异常: {ex.Message}",
                        DateTime.UtcNow - start,
                        0,
                        criteria.Weight,
                        false,
                        null
                    ))

        member _.GetMetadata() =
            TargetingMatcherMetadata(
                MatcherId = "behavior-matcher-v1",
                Name = "用户行为定向匹配器",
                Description = "基于行为类型/频次/时间衰减/上下文特征的匹配",
                Version = "1.0.0",
                MatcherType = "Behavior",
                Author = "Lorn.ADSP Team",
                SupportedCriteriaTypes = [ "Behavior" ],
                SupportedDimensions = [ "BehaviorType"; "BehaviorFrequency"; "BehaviorContext" ],
                SupportsParallelExecution = true,
                SupportsCaching = true,
                SupportsBatchProcessing = false,
                ExpectedExecutionTime = TimeSpan.FromMilliseconds 8.,
                MaxExecutionTime = TimeSpan.FromMilliseconds 80.
            )
