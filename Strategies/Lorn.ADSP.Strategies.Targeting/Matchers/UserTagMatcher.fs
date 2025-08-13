namespace Lorn.ADSP.Strategies.Targeting.Matchers

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Shared.Entities

// 用户标签定向匹配器
// - 名称/类型/分类匹配
// - 置信度与权重阈值过滤
// - AND/OR 组合逻辑
// - 过期检查与排除标签

[<CLIMutable>]
type TagDto =
    { TagName: string
      TagType: string
      Category: string
      Confidence: decimal
      Weight: decimal
      AssignedAt: DateTime
      ExpiresAt: Nullable<DateTime> }

module private TagHelpers =
    let inline eq (a: string) (b: string) =
        String.Equals(a, b, StringComparison.OrdinalIgnoreCase)

    let isExpired (includeExpired: bool) (expiresAt: Nullable<DateTime>) =
        if includeExpired then
            false
        else
            expiresAt.HasValue && expiresAt.Value <= DateTime.UtcNow

    let fromUserTag (t: UserTag) : TagDto =
        { TagName = t.TagName
          TagType = t.TagType
          Category = t.Category |> Option.ofObj |> Option.defaultValue String.Empty
          Confidence = t.Confidence
          Weight = t.Weight
          AssignedAt = t.AssignedAt
          ExpiresAt =
            (if t.ExpiresAt.HasValue then
                 Nullable t.ExpiresAt.Value
             else
                 Nullable()) }

    let tryParseJsonList (prop: ContextProperty) : TagDto list =
        try
            if prop.DataType = "Json" then
                let lst =
                    System.Text.Json.JsonSerializer.Deserialize<List<TagDto>>(prop.PropertyValue)

                if isNull (box lst) then [] else List.ofSeq lst
            else
                []
        with _ ->
            []

    let tryParseJsonSingle (prop: ContextProperty) : TagDto option =
        try
            if prop.DataType = "Json" then
                let t = System.Text.Json.JsonSerializer.Deserialize<TagDto>(prop.PropertyValue)
                if isNull (box t) then None else Some t
            else
                None
        with _ ->
            None

open TagHelpers

type UserTagMatcher() =
    interface ITargetingMatcher with
        member _.MatcherId = "tag-matcher-v1"
        member _.MatcherName = "用户标签定向匹配器"
        member _.Version = "1.0.0"
        member _.MatcherType = "Tag"
        member _.Priority = 120
        member _.IsEnabled = true
        member _.ExpectedExecutionTime = TimeSpan.FromMilliseconds 5.
        member _.CanRunInParallel = true

        member _.IsSupported(t: string) = t = "Tag"

        member this.ValidateCriteria(criteria: ITargetingCriteria) =
            if isNull (box criteria) then
                ValidationResult.Failure([ ValidationError(Message = "定向条件为空") ], "验证失败", TimeSpan.Zero)
            elif not ((this :> ITargetingMatcher).IsSupported(criteria.CriteriaType)) then
                let msg = sprintf "不支持的定向类型: %s" criteria.CriteriaType
                ValidationResult.Failure([ ValidationError(Message = msg) ], "验证失败", TimeSpan.Zero)
            elif not (criteria :? UserTagTargeting) then
                ValidationResult.Failure(
                    [ ValidationError(Message = "Criteria 不是 UserTagTargeting") ],
                    "验证失败",
                    TimeSpan.Zero
                )
            else
                let c = criteria :?> UserTagTargeting

                if
                    (isNull (box c.TargetTagNames) || c.TargetTagNames.Count = 0)
                    && (isNull (box c.TargetTagTypes) || c.TargetTagTypes.Count = 0)
                    && (isNull (box c.TargetCategories) || c.TargetCategories.Count = 0)
                then
                    ValidationResult.Failure([ ValidationError(Message = "需要至少一个目标标签条件") ], "验证失败", TimeSpan.Zero)
                elif c.MinConfidenceThreshold < 0m || c.MinConfidenceThreshold > 1m then
                    ValidationResult.Failure([ ValidationError(Message = "置信度阈值必须在0-1之间") ], "验证失败", TimeSpan.Zero)
                elif c.MinWeightThreshold < 0m then
                    ValidationResult.Failure([ ValidationError(Message = "权重阈值不能为负数") ], "验证失败", TimeSpan.Zero)
                elif not (c.MatchMode = "Any" || c.MatchMode = "All") then
                    ValidationResult.Failure([ ValidationError(Message = "匹配模式必须是Any或All") ], "验证失败", TimeSpan.Zero)
                else
                    ValidationResult.Success("标签定向条件验证通过", TimeSpan.Zero)

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
                        || not (criteria :? UserTagTargeting)
                    then
                        MatchResult.CreateNoMatch(
                            "Tag",
                            criteria.CriteriaId,
                            "不支持的条件或上下文为空",
                            TimeSpan.Zero,
                            0,
                            criteria.Weight,
                            false,
                            null
                        )
                    else
                        let c = criteria :?> UserTagTargeting

                        // 收集用户标签
                        let tags: TagDto list =
                            if context :? UserTag then
                                [ fromUserTag (context :?> UserTag) ]
                            else
                                let propsTagCategory = context.GetPropertiesByCategory("Tag")

                                let jsonUserTagsProp =
                                    if context.HasProperty "UserTags" then
                                        context.GetProperty "UserTags"
                                    else
                                        null

                                let fromJson: TagDto list =
                                    if isNull (box jsonUserTagsProp) then
                                        []
                                    else
                                        tryParseJsonList jsonUserTagsProp

                                let fromCat: TagDto list =
                                    propsTagCategory |> Seq.choose (fun p -> tryParseJsonSingle p) |> Seq.toList

                                List.append fromJson fromCat

                        // 过滤过期与阈值
                        let filtered =
                            tags
                            |> List.filter (fun t -> not (isExpired c.IncludeExpiredTags t.ExpiresAt))
                            |> List.filter (fun t ->
                                t.Confidence >= c.MinConfidenceThreshold && t.Weight >= c.MinWeightThreshold)

                        // 排除标签命中立即不匹配
                        if
                            c.ExcludedTagNames
                            |> Seq.exists (fun ex -> filtered |> List.exists (fun t -> eq t.TagName ex))
                        then
                            MatchResult.CreateNoMatch(
                                c.CriteriaType,
                                c.CriteriaId,
                                "命中排除标签",
                                DateTime.UtcNow - start,
                                0,
                                c.Weight,
                                false,
                                null
                            )
                        else
                            // 目标集合
                            let needNames = c.TargetTagNames |> Seq.toList
                            let needTypes = c.TargetTagTypes |> Seq.toList
                            let needCats = c.TargetCategories |> Seq.toList

                            let hasName (n: string) =
                                filtered |> List.exists (fun t -> eq t.TagName n)

                            let hasType (n: string) =
                                filtered |> List.exists (fun t -> eq t.TagType n)

                            let hasCat (n: string) =
                                filtered |> List.exists (fun t -> eq t.Category n)

                            let anyName =
                                if needNames.IsEmpty then
                                    false
                                else
                                    needNames |> List.exists hasName

                            let anyType =
                                if needTypes.IsEmpty then
                                    false
                                else
                                    needTypes |> List.exists hasType

                            let anyCat =
                                if needCats.IsEmpty then
                                    false
                                else
                                    needCats |> List.exists hasCat

                            let allName =
                                if needNames.IsEmpty then
                                    true
                                else
                                    needNames |> List.forall hasName

                            let allType =
                                if needTypes.IsEmpty then
                                    true
                                else
                                    needTypes |> List.forall hasType

                            let allCat =
                                if needCats.IsEmpty then
                                    true
                                else
                                    needCats |> List.forall hasCat

                            let matched, requiredCount =
                                if String.Equals(c.MatchMode, "All", StringComparison.OrdinalIgnoreCase) then
                                    let ok = allName && allType && allCat

                                    let reqCnt =
                                        (if needNames.IsEmpty then 0 else needNames.Length)
                                        + (if needTypes.IsEmpty then 0 else needTypes.Length)
                                        + (if needCats.IsEmpty then 0 else needCats.Length)

                                    ok, reqCnt
                                else
                                    let ok = anyName || anyType || anyCat
                                    let reqCnt = 1
                                    ok, reqCnt

                            let diagDetails: IReadOnlyList<ContextProperty> =
                                [| ContextProperty(
                                       "FilteredCount",
                                       string filtered.Length,
                                       "Integer",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   )
                                   ContextProperty(
                                       "NeedNames",
                                       string needNames.Length,
                                       "Integer",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   )
                                   ContextProperty(
                                       "NeedTypes",
                                       string needTypes.Length,
                                       "Integer",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   )
                                   ContextProperty(
                                       "NeedCats",
                                       string needCats.Length,
                                       "Integer",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   )
                                   ContextProperty(
                                       "AnyName",
                                       anyName.ToString(),
                                       "Boolean",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   )
                                   ContextProperty(
                                       "AnyType",
                                       anyType.ToString(),
                                       "Boolean",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   )
                                   ContextProperty(
                                       "AnyCat",
                                       anyCat.ToString(),
                                       "Boolean",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   )
                                   ContextProperty(
                                       "AllName",
                                       allName.ToString(),
                                       "Boolean",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   )
                                   ContextProperty(
                                       "AllType",
                                       allType.ToString(),
                                       "Boolean",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   )
                                   ContextProperty(
                                       "AllCat",
                                       allCat.ToString(),
                                       "Boolean",
                                       "TagMatch",
                                       false,
                                       1.0m,
                                       Nullable(),
                                       "UserTagMatcher"
                                   ) |]
                                :> _

                            if not matched then
                                let reason =
                                    if String.Equals(c.MatchMode, "All", StringComparison.OrdinalIgnoreCase) then
                                        sprintf
                                            "未满足标签匹配条件(AllMode): AllName=%b AllType=%b AllCat=%b | Filtered=%d Need(N/T/C)=(%d/%d/%d)"
                                            allName
                                            allType
                                            allCat
                                            filtered.Length
                                            needNames.Length
                                            needTypes.Length
                                            needCats.Length
                                    else
                                        sprintf
                                            "未满足标签匹配条件(AnyMode): AnyName=%b AnyType=%b AnyCat=%b | Filtered=%d Need(N/T/C)=(%d/%d/%d)"
                                            anyName
                                            anyType
                                            anyCat
                                            filtered.Length
                                            needNames.Length
                                            needTypes.Length
                                            needCats.Length

                                MatchResult.CreateNoMatch(
                                    c.CriteriaType,
                                    c.CriteriaId,
                                    reason,
                                    DateTime.UtcNow - start,
                                    0,
                                    c.Weight,
                                    false,
                                    diagDetails
                                )
                            else
                                // 评分：按命中的标签 confidence*weight 取平均（All）或最大（Any），归一化至 [0,1]
                                let matchedTags =
                                    filtered
                                    |> List.filter (fun t ->
                                        (needNames |> List.exists (eq t.TagName))
                                        || (needTypes |> List.exists (eq t.TagType))
                                        || (needCats |> List.exists (eq t.Category)))

                                let scores =
                                    matchedTags |> List.map (fun t -> t.Confidence * (min 1.0m (max 0.0m t.Weight)))

                                let score =
                                    if scores.IsEmpty then
                                        0m
                                    else if String.Equals(c.MatchMode, "All", StringComparison.OrdinalIgnoreCase) then
                                        (scores |> List.average) |> min 1.0m
                                    else
                                        (scores |> List.max) |> min 1.0m

                                let details: IReadOnlyList<ContextProperty> =
                                    [| yield! (diagDetails :> seq<_>)
                                       yield
                                           ContextProperty(
                                               "MatchedCount",
                                               string matchedTags.Length,
                                               "Integer",
                                               "TagMatch",
                                               false,
                                               1.0m,
                                               Nullable(),
                                               "UserTagMatcher"
                                           )
                                       yield
                                           ContextProperty(
                                               "RequiredCount",
                                               string requiredCount,
                                               "Integer",
                                               "TagMatch",
                                               false,
                                               1.0m,
                                               Nullable(),
                                               "UserTagMatcher"
                                           )
                                       yield
                                           ContextProperty(
                                               "ConfidenceAvg",
                                               (if scores.IsEmpty then
                                                    "0"
                                                else
                                                    string (scores |> List.average)),
                                               "Decimal",
                                               "TagMatch",
                                               false,
                                               1.0m,
                                               Nullable(),
                                               "UserTagMatcher"
                                           ) |]
                                    :> _

                                if score > 0m then
                                    MatchResult.CreateMatch(
                                        c.CriteriaType,
                                        c.CriteriaId,
                                        score,
                                        "标签匹配",
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
                                        "未达到评分阈值",
                                        DateTime.UtcNow - start,
                                        0,
                                        c.Weight,
                                        false,
                                        details
                                    )
                with ex ->
                    MatchResult.CreateNoMatch(
                        "Tag",
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
                MatcherId = "tag-matcher-v1",
                Name = "用户标签定向匹配器",
                Description = "根据用户标签名称/类型/分类进行匹配，支持AND/OR组合以及置信度与权重阈值，并处理过期标签",
                Version = "1.0.0",
                MatcherType = "Tag",
                Author = "Lorn.ADSP Team",
                SupportedCriteriaTypes = [ "Tag" ],
                SupportedDimensions = [ "TagName"; "TagType"; "Category" ],
                SupportsParallelExecution = true,
                SupportsCaching = true,
                SupportsBatchProcessing = false,
                ExpectedExecutionTime = TimeSpan.FromMilliseconds 5.,
                MaxExecutionTime = TimeSpan.FromMilliseconds 60.
            )
