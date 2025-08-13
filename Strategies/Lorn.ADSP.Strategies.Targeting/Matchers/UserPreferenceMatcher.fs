namespace Lorn.ADSP.Strategies.Targeting.Matchers

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Shared.Enums
open Lorn.ADSP.Core.Shared.Entities

// 用户偏好定向匹配器：
// - 广告类型偏好匹配
// - 屏蔽类别过滤
// - 时间偏好验证 (PreferredTimeSlots)
// - 隐私合规 (Require* 与 UserPreference/PrivacySettings)

type private PrefScore = { AdType: string; Score: decimal }

type UserPreferenceMatcher() =
    interface ITargetingMatcher with
        member _.MatcherId = "preference-matcher-v1"
        member _.MatcherName = "用户偏好定向匹配器"
        member _.Version = "1.0.0"
        member _.MatcherType = "Preference"
        member _.Priority = 125
        member _.IsEnabled = true
        member _.ExpectedExecutionTime = TimeSpan.FromMilliseconds 5.
        member _.CanRunInParallel = true

        member _.IsSupported(t: string) = t = "Preference"

        member this.ValidateCriteria(criteria: ITargetingCriteria) =
            if isNull (box criteria) then
                ValidationResult.Failure([ ValidationError(Message = "定向条件为空") ], "验证失败", TimeSpan.Zero)
            elif not ((this :> ITargetingMatcher).IsSupported(criteria.CriteriaType)) then
                let msg = sprintf "不支持的定向类型: %s" criteria.CriteriaType
                ValidationResult.Failure([ ValidationError(Message = msg) ], "验证失败", TimeSpan.Zero)
            elif not (criteria :? UserPreferenceTargeting) then
                ValidationResult.Failure(
                    [ ValidationError(Message = "Criteria 不是 UserPreferenceTargeting") ],
                    "验证失败",
                    TimeSpan.Zero
                )
            else
                let c = criteria :?> UserPreferenceTargeting
                // 至少配置一个偏好维度或约束
                if
                    (not (c.TargetAdTypes.Count > 0))
                    && (not (c.TargetTopics.Count > 0))
                    && (not (c.TargetLanguages.Count > 0))
                    && not c.RequirePersonalizedAdsConsent
                    && not c.RequireBehaviorTrackingConsent
                    && not c.RequireCrossDeviceTrackingConsent
                    && (not c.MaxDailyImpressions.HasValue)
                then
                    ValidationResult.Failure([ ValidationError(Message = "需要至少一个偏好或合规约束") ], "验证失败", TimeSpan.Zero)
                else
                    ValidationResult.Success("偏好定向条件验证通过", TimeSpan.Zero)

        member _.CalculateMatchScoreAsync
            (context: ITargetingContext, criteria: ITargetingCriteria, _cb, ct: CancellationToken)
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
                    elif
                        isNull (box context)
                        || isNull (box criteria)
                        || not (criteria :? UserPreferenceTargeting)
                    then
                        MatchResult.CreateNoMatch(
                            criteria.CriteriaType,
                            criteria.CriteriaId,
                            "不支持的条件或上下文为空",
                            TimeSpan.Zero,
                            0,
                            criteria.Weight,
                            false,
                            null
                        )
                    else
                        let c = criteria :?> UserPreferenceTargeting

                        // 读取用户偏好上下文 (强类型 UserPreference 优先，否则从通用属性读取)
                        let prefOpt =
                            if context :? UserPreference then
                                Some(context :?> UserPreference)
                            else
                                None

                        // 合规检查
                        let isPersonalizationAllowed =
                            match prefOpt with
                            | Some p -> p.AllowPersonalizedAds
                            | None -> context.GetPropertyValue<bool>("AllowPersonalizedAds", true)

                        let isBehaviorTrackingAllowed =
                            match prefOpt with
                            | Some p -> p.AllowBehaviorTracking
                            | None -> context.GetPropertyValue<bool>("AllowBehaviorTracking", true)

                        let isCrossDeviceAllowed =
                            match prefOpt with
                            | Some p -> p.AllowCrossDeviceTracking
                            | None -> context.GetPropertyValue<bool>("AllowCrossDeviceTracking", true)

                        // 仅当明确要求同意时才阻断；若未要求（默认），即使用户未显式允许，也不阻断
                        if c.RequirePersonalizedAdsConsent && not isPersonalizationAllowed then
                            MatchResult.CreateNoMatch(
                                c.CriteriaType,
                                c.CriteriaId,
                                "未获个性化广告同意",
                                DateTime.UtcNow - start,
                                0,
                                c.Weight,
                                false,
                                null
                            )
                        elif c.RequireBehaviorTrackingConsent && not isBehaviorTrackingAllowed then
                            MatchResult.CreateNoMatch(
                                c.CriteriaType,
                                c.CriteriaId,
                                "未获行为追踪同意",
                                DateTime.UtcNow - start,
                                0,
                                c.Weight,
                                false,
                                null
                            )
                        elif c.RequireCrossDeviceTrackingConsent && not isCrossDeviceAllowed then
                            MatchResult.CreateNoMatch(
                                c.CriteriaType,
                                c.CriteriaId,
                                "未获跨设备追踪同意",
                                DateTime.UtcNow - start,
                                0,
                                c.Weight,
                                false,
                                null
                            )
                        else if
                            // 若 criteria 未设置任何目标偏好，视为中性接受
                            (isNull (box c.TargetAdTypes) || c.TargetAdTypes.Count = 0)
                            && (isNull (box c.TargetTopics) || c.TargetTopics.Count = 0)
                            && (isNull (box c.TargetLanguages) || c.TargetLanguages.Count = 0)
                        then
                            let details: IReadOnlyList<ContextProperty> =
                                [| ContextProperty(
                                       "NeutralAcceptance",
                                       "true",
                                       "Boolean",
                                       "PreferenceMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserPreferenceMatcher"
                                   ) |]
                                :> _

                            MatchResult.CreateMatch(
                                c.CriteriaType,
                                c.CriteriaId,
                                1.0m,
                                "无目标偏好，默认接受",
                                DateTime.UtcNow - start,
                                0,
                                c.Weight,
                                false,
                                details
                            )
                        else
                            // 时间偏好：若存在 PreferredTimeSlots 则必须命中
                            let timeOk =
                                match prefOpt with
                                | Some p when p.PreferredTimeSlots.Count > 0 ->
                                    p.IsInPreferredTimeSlot(DateTime.UtcNow)
                                | _ -> true

                            if not timeOk then
                                MatchResult.CreateNoMatch(
                                    c.CriteriaType,
                                    c.CriteriaId,
                                    "不在偏好时间段",
                                    DateTime.UtcNow - start,
                                    0,
                                    c.Weight,
                                    false,
                                    null
                                )
                            else
                                // 屏蔽类别：如果上下文中标注了 Category 且在排除列表内，直接不匹配
                                let category =
                                    if context.HasProperty "Category" then
                                        context.GetPropertyAsString "Category"
                                    else
                                        null

                                if
                                    not (isNull category)
                                    && (c.ExcludedCategories
                                        |> Seq.exists (fun s ->
                                            String.Equals(s, category, StringComparison.OrdinalIgnoreCase)))
                                then
                                    MatchResult.CreateNoMatch(
                                        c.CriteriaType,
                                        c.CriteriaId,
                                        "命中屏蔽类别",
                                        DateTime.UtcNow - start,
                                        0,
                                        c.Weight,
                                        false,
                                        null
                                    )
                                else
                                    // 广告类型偏好评分：若目标类型为空视为1.0；否则根据用户 PreferredAdTypes 计算
                                    let adTypeScore =
                                        if isNull (box c.TargetAdTypes) || c.TargetAdTypes.Count = 0 then
                                            1.0m
                                        else
                                            let userPrefTypes: IReadOnlyList<AdType> =
                                                match prefOpt with
                                                | Some p -> p.PreferredAdTypes
                                                | None ->
                                                    let v = context.GetPropertyValue<List<AdType>>("PreferredAdTypes")

                                                    if isNull (box v) then
                                                        (ResizeArray<AdType>() :> IReadOnlyList<AdType>)
                                                    else
                                                        (v :> IReadOnlyList<AdType>)

                                            if isNull (box userPrefTypes) || userPrefTypes.Count = 0 then
                                                0.5m // 未声明偏好时给中性分
                                            else
                                                let hits =
                                                    c.TargetAdTypes
                                                    |> Seq.filter (fun t -> userPrefTypes |> Seq.exists ((=) t))
                                                    |> Seq.length
                                                    |> decimal

                                                let denom = decimal c.TargetAdTypes.Count
                                                if denom > 0m then min 1m (hits / denom) else 0m

                                    // 语言与话题偏好：按覆盖率简单折中
                                    let topicScore =
                                        if isNull (box c.TargetTopics) || c.TargetTopics.Count = 0 then
                                            1.0m
                                        else
                                            let userTopics: IReadOnlyList<string> =
                                                match prefOpt with
                                                | Some p -> p.PreferredTopics
                                                | None ->
                                                    let v = context.GetPropertyValue<List<string>>("PreferredTopics")

                                                    if isNull (box v) then
                                                        (ResizeArray<string>() :> IReadOnlyList<string>)
                                                    else
                                                        (v :> IReadOnlyList<string>)

                                            if isNull (box userTopics) || userTopics.Count = 0 then
                                                0.5m
                                            else
                                                let hit =
                                                    c.TargetTopics
                                                    |> Seq.filter (fun s ->
                                                        userTopics
                                                        |> Seq.exists (fun u ->
                                                            String.Equals(u, s, StringComparison.OrdinalIgnoreCase)))
                                                    |> Seq.length
                                                    |> decimal

                                                min 1m (hit / decimal c.TargetTopics.Count)

                                    let langScore =
                                        if isNull (box c.TargetLanguages) || c.TargetLanguages.Count = 0 then
                                            1.0m
                                        else
                                            let userLangs: IReadOnlyList<string> =
                                                match prefOpt with
                                                | Some p -> p.PreferredLanguages
                                                | None ->
                                                    let v =
                                                        context.GetPropertyValue<List<string>>("PreferredLanguages")

                                                    if isNull (box v) then
                                                        (ResizeArray<string>() :> IReadOnlyList<string>)
                                                    else
                                                        (v :> IReadOnlyList<string>)

                                            if isNull (box userLangs) || userLangs.Count = 0 then
                                                0.5m
                                            else
                                                let hit =
                                                    c.TargetLanguages
                                                    |> Seq.filter (fun s ->
                                                        userLangs
                                                        |> Seq.exists (fun u ->
                                                            String.Equals(u, s, StringComparison.OrdinalIgnoreCase)))
                                                    |> Seq.length
                                                    |> decimal

                                                min 1m (hit / decimal c.TargetLanguages.Count)

                                    // 隐私级别与内容成熟度：从用户偏好读取限制，与 criteria 上限比较
                                    let privacyOk =
                                        let userPrivacyLevel =
                                            match prefOpt with
                                            | Some p -> p.PrivacyLevel
                                            | None ->
                                                context.GetPropertyValue<PrivacyLevel>(
                                                    "PrivacyLevel",
                                                    PrivacyLevel.Standard
                                                )
                                        // 与 criteria 使用同一命名空间的枚举比较，保证序值一致
                                        int userPrivacyLevel <= int c.MaxPrivacyLevel

                                    let contentOk =
                                        let userContentLevel =
                                            match prefOpt with
                                            | Some p -> p.ContentMaturityLevel
                                            | None ->
                                                context.GetPropertyValue<ContentMaturityLevel>(
                                                    "ContentMaturityLevel",
                                                    ContentMaturityLevel.General
                                                )
                                        // 与 criteria 的上限比较
                                        int userContentLevel <= int c.MaxContentMaturityLevel

                                    if not privacyOk then
                                        MatchResult.CreateNoMatch(
                                            c.CriteriaType,
                                            c.CriteriaId,
                                            "超出隐私级别要求",
                                            DateTime.UtcNow - start,
                                            0,
                                            c.Weight,
                                            false,
                                            null
                                        )
                                    elif not contentOk then
                                        MatchResult.CreateNoMatch(
                                            c.CriteriaType,
                                            c.CriteriaId,
                                            "超出内容成熟度要求",
                                            DateTime.UtcNow - start,
                                            0,
                                            c.Weight,
                                            false,
                                            null
                                        )
                                    else
                                        // 综合评分：简单乘积后取上限
                                        let score =
                                            [ adTypeScore; topicScore; langScore ]
                                            |> List.fold (fun acc x -> acc * x) 1.0m
                                            |> min 1.0m

                                        let details: IReadOnlyList<ContextProperty> =
                                            [| ContextProperty(
                                                   "AdTypeScore",
                                                   string adTypeScore,
                                                   "Decimal",
                                                   "PreferenceMatch",
                                                   false,
                                                   1.0m,
                                                   Nullable(),
                                                   "UserPreferenceMatcher"
                                               )
                                               ContextProperty(
                                                   "TopicScore",
                                                   string topicScore,
                                                   "Decimal",
                                                   "PreferenceMatch",
                                                   false,
                                                   1.0m,
                                                   Nullable(),
                                                   "UserPreferenceMatcher"
                                               )
                                               ContextProperty(
                                                   "LanguageScore",
                                                   string langScore,
                                                   "Decimal",
                                                   "PreferenceMatch",
                                                   false,
                                                   1.0m,
                                                   Nullable(),
                                                   "UserPreferenceMatcher"
                                               ) |]
                                            :> _

                                        if score > 0m then
                                            MatchResult.CreateMatch(
                                                c.CriteriaType,
                                                c.CriteriaId,
                                                score,
                                                "偏好匹配",
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
                                                "未达到偏好评分阈值",
                                                DateTime.UtcNow - start,
                                                0,
                                                c.Weight,
                                                false,
                                                details
                                            )
                with ex ->
                    MatchResult.CreateNoMatch(
                        "Preference",
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
                MatcherId = "preference-matcher-v1",
                Name = "用户偏好定向匹配器",
                Description = "广告类型/话题/语言偏好 + 时间段验证 + 隐私合规",
                Version = "1.0.0",
                MatcherType = "Preference",
                Author = "Lorn.ADSP Team",
                SupportedCriteriaTypes = [ "Preference" ],
                SupportedDimensions = [ "AdType"; "Topic"; "Language"; "PrivacyLevel" ],
                SupportsParallelExecution = true,
                SupportsCaching = true,
                SupportsBatchProcessing = false,
                ExpectedExecutionTime = TimeSpan.FromMilliseconds 5.,
                MaxExecutionTime = TimeSpan.FromMilliseconds 60.
            )
