namespace Lorn.ADSP.Strategies.Targeting.Matchers

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Shared.Entities

/// 用户兴趣定向匹配器 (语义相似度 + 标签补强 + 时间衰减 + 子类扩展)

// 私有帮助函数模块
module private UserInterestMatcherHelpers =
    let synonymMap: IDictionary<string, string list> =
        dict
            [ "tech", [ "technology"; "it"; "computer" ]
              "sport", [ "sports"; "athletics"; "game" ]
              "finance", [ "financial"; "money"; "investment" ]
              "fashion", [ "style"; "clothing"; "apparel" ] ]

    let normalize (s: string) =
        if String.IsNullOrWhiteSpace s then
            ""
        else
            s.Trim().ToLowerInvariant()

    let expand term =
        let n = normalize term

        seq {
            if n <> "" then
                yield n

            for kv in synonymMap do
                if kv.Key = n || kv.Value |> List.exists ((=) n) then
                    yield kv.Key

                    for v in kv.Value do
                        yield v
        }
        |> Seq.distinct
        |> Set.ofSeq

    let similarity a b =
        let aN, bN = normalize a, normalize b

        if aN = "" || bN = "" then
            0m
        elif aN = bN then
            1m
        else
            let sa = expand aN
            let sb = expand bN
            let inter = Set.intersect sa sb |> Set.count |> decimal
            let union = Set.union sa sb |> Set.count |> decimal
            if union = 0m then 0m else inter / union |> min 1m

    let boost (subs: seq<string>) (tags: seq<string>) (target: IReadOnlyList<string>) =
        if isNull target || target.Count = 0 then
            0m
        else
            let t = target |> Seq.map normalize |> set
            let u = Seq.append subs tags |> Seq.map normalize |> set
            let hit = Set.intersect t u |> Set.count |> decimal

            if hit = 0m then
                0m
            else
                (hit / decimal t.Count) * 0.2m |> min 0.3m

open UserInterestMatcherHelpers

type InternalMatch =
    { Category: string
      Decayed: decimal
      Raw: decimal
      Interest: UserInterest }

type UserInterestMatcher(?decayLambda: decimal) as self =
    let lambda = defaultArg decayLambda 0.05m

    let decay (last: DateTime) =
        let days = (DateTime.UtcNow - last).TotalDays |> decimal |> max 0m
        Math.Exp(float (-lambda * days)) |> decimal |> max 0.0m |> min 1.0m

    interface ITargetingMatcher with
        member _.MatcherId = "interest-matcher-v1"
        member _.MatcherName = "兴趣定向匹配器"
        member _.Version = "1.0.0"
        member _.MatcherType = "Interest"
        member _.Priority = 120
        member _.IsEnabled = true
        member _.ExpectedExecutionTime = TimeSpan.FromMilliseconds 6.
        member _.CanRunInParallel = true

        member _.IsSupported(t: string) = t = "Interest"

        member _.ValidateCriteria(criteria: ITargetingCriteria) =
            if isNull (box criteria) then
                ValidationResult.Failure([ ValidationError(Message = "定向条件为空") ], "验证失败", TimeSpan.Zero)
            elif not ((self :> ITargetingMatcher).IsSupported(criteria.CriteriaType)) then
                let msg = sprintf "不支持的定向类型: %s" criteria.CriteriaType
                ValidationResult.Failure([ ValidationError(Message = msg) ], "验证失败", TimeSpan.Zero)
            elif not (criteria :? UserInterestTargeting) then
                ValidationResult.Failure(
                    [ ValidationError(Message = "Criteria 不是 UserInterestTargeting") ],
                    "验证失败",
                    TimeSpan.Zero
                )
            else
                let c = criteria :?> UserInterestTargeting

                if (not (c.TargetCategories.Any())) && (not (c.TargetTags.Any())) then
                    ValidationResult.Failure([ ValidationError(Message = "必须配置目标兴趣类别或标签") ], "验证失败", TimeSpan.Zero)
                else
                    ValidationResult.Success("兴趣定向条件验证通过", TimeSpan.Zero)

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
                    elif isNull (box criteria) || not (criteria :? UserInterestTargeting) then
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
                        let c = criteria :?> UserInterestTargeting
                        // 收集兴趣
                        let interests = ResizeArray<UserInterest>()

                        try
                            let mi = context.GetType().GetMethod("GetInterests", Array.empty)

                            if mi <> null then
                                let r = mi.Invoke(context, null) :?> IReadOnlyList<ContextProperty>

                                if r <> null then
                                    for p in r do
                                        if p <> null && not (String.IsNullOrWhiteSpace p.PropertyKey) then
                                            // 构造基本兴趣 (Score 暂给 0.5 可扩展)
                                            interests.Add(
                                                UserInterest(
                                                    p.PropertyKey,
                                                    0.5m,
                                                    p.Weight,
                                                    1.0m,
                                                    DateTime.UtcNow,
                                                    null,
                                                    null,
                                                    "ContextProperty"
                                                )
                                            )
                        with _ ->
                            ()

                        if context :? UserInterest then
                            interests.Add(context :?> UserInterest)
                        // 若上下文包含 NestedContexts, 递归收集一级 UserInterest
                        let nestedProp = context.GetType().GetProperty("NestedContexts")

                        if nestedProp <> null then
                            let nestedVal = nestedProp.GetValue(context) :?> System.Collections.IEnumerable

                            if nestedVal <> null then
                                for n in nestedVal do
                                    if n :? UserInterest then
                                        interests.Add(n :?> UserInterest)

                        if interests.Count = 0 then
                            MatchResult.CreateNoMatch(
                                criteria.CriteriaType,
                                criteria.CriteriaId,
                                "用户无兴趣数据",
                                DateTime.UtcNow - start,
                                0,
                                criteria.Weight,
                                false,
                                null
                            )
                        else
                            // 计算匹配
                            let matches =
                                [ for ui in interests do
                                      let candidateCategories: seq<string> =
                                          if c.IncludeSubCategories && not (isNull (box ui.SubCategories)) then
                                              Seq.append (seq { ui.Category }) ui.SubCategories
                                          else
                                              seq { ui.Category }

                                      let mutable best = 0m
                                      let mutable bestCat = ui.Category

                                      for cand in candidateCategories do
                                          for tc in c.TargetCategories do
                                              let s = similarity cand tc

                                              if s > best then
                                                  best <- s
                                                  bestCat <- tc

                                      if best > 0m && c.TargetTags.Any() then
                                          let b = boost ui.SubCategories ui.Tags c.TargetTags
                                          best <- (best + b) |> min 1m

                                      if
                                          best > 0m
                                          && ui.Score >= c.MinScoreThreshold
                                          && ui.Confidence >= c.MinConfidenceThreshold
                                      then
                                          let d = decay ui.LastUpdated

                                          yield
                                              { Category = bestCat
                                                Raw = best
                                                Decayed = best * d * ui.Confidence * ui.Weight
                                                Interest = ui } ]

                            if matches.IsEmpty then
                                MatchResult.CreateNoMatch(
                                    criteria.CriteriaType,
                                    criteria.CriteriaId,
                                    "无匹配兴趣或未达阈值",
                                    DateTime.UtcNow - start,
                                    0,
                                    criteria.Weight,
                                    false,
                                    null
                                )
                            else
                                // 每个目标类别取最高得分 (可替换为衰减后累加策略)
                                let grouped =
                                    matches
                                    |> Seq.groupBy (fun m -> m.Category)
                                    |> Seq.map (fun (k, vs) -> k, vs |> Seq.maxBy (fun x -> x.Decayed))
                                    |> Seq.toList

                                let total = grouped |> List.sumBy (fun (_, m) -> m.Decayed)

                                let denom =
                                    if (c.TargetCategories.Any()) then
                                        decimal c.TargetCategories.Count
                                    else
                                        decimal grouped.Length

                                let score = if denom > 0m then min 1m (total / denom) else 0m

                                let details: IReadOnlyList<ContextProperty> =
                                    grouped
                                    |> List.map (fun (cat, m) ->
                                        ContextProperty(
                                            cat,
                                            sprintf "%.3f" m.Decayed,
                                            "Decimal",
                                            "InterestMatch",
                                            false,
                                            1.0m,
                                            Nullable(),
                                            "UserInterestMatcher"
                                        ))
                                    |> List.toArray
                                    :> _

                                if score > 0m then
                                    MatchResult.CreateMatch(
                                        criteria.CriteriaType,
                                        criteria.CriteriaId,
                                        score,
                                        sprintf "匹配兴趣类别数=%d" grouped.Length,
                                        DateTime.UtcNow - start,
                                        0,
                                        criteria.Weight,
                                        false,
                                        details
                                    )
                                else
                                    MatchResult.CreateNoMatch(
                                        criteria.CriteriaType,
                                        criteria.CriteriaId,
                                        "未匹配兴趣类别",
                                        DateTime.UtcNow - start,
                                        0,
                                        criteria.Weight,
                                        false,
                                        details
                                    )
                with ex ->
                    MatchResult.CreateNoMatch(
                        "Interest",
                        criteria.CriteriaId,
                        sprintf "匹配异常: %s" ex.Message,
                        DateTime.UtcNow - start,
                        0,
                        criteria.Weight,
                        false,
                        null
                    ))

        member _.GetMetadata() =
            TargetingMatcherMetadata(
                MatcherId = "interest-matcher-v1",
                Name = "兴趣定向匹配器",
                Description = "基于兴趣类别/标签 + 语义相似度 + 时间衰减的匹配",
                Version = "1.0.0",
                MatcherType = "Interest",
                Author = "Lorn.ADSP Team",
                SupportedCriteriaTypes = [ "Interest" ],
                SupportedDimensions = [ "InterestCategory"; "InterestTag" ],
                SupportsParallelExecution = true,
                SupportsCaching = true,
                SupportsBatchProcessing = false,
                ExpectedExecutionTime = TimeSpan.FromMilliseconds 6.,
                MaxExecutionTime = TimeSpan.FromMilliseconds 60.
            )
