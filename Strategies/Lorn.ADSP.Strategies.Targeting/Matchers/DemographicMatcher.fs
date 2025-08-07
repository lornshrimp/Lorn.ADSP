namespace Lorn.ADSP.Strategies.Targeting.Matchers

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open FSharp.Control
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Shared.Entities
open Lorn.ADSP.Core.Shared.Enums

// 定义匹配结果记录类型
type MatchRecord =
    { AttributeName: string
      IsMatch: bool
      HasCondition: bool }

// 定义整体匹配结果类型
type OverallMatchResult =
    { IsMatch: bool
      Score: decimal
      MatchReason: string
      NotMatchReason: string
      MatchDetails: IReadOnlyList<ContextProperty> }

/// <summary>
/// 人口属性定向匹配器
/// 实现 ITargetingMatcher 接口，处理人口统计学信息的定向匹配
/// 复用 UserProfile 中的用户信息和 DemographicTargeting 中的定向条件
/// </summary>
type DemographicTargetingMatcher() =

    // 实现 ITargetingMatcher 接口的属性
    interface ITargetingMatcher with
        member this.MatcherId = "demographic-matcher-v1"
        member this.MatcherName = "人口属性定向匹配器"
        member this.Version = "1.0.0"
        member this.MatcherType = "Demographic"
        member this.Priority = 100
        member this.IsEnabled = true
        member this.ExpectedExecutionTime = TimeSpan.FromMilliseconds(10.0)
        member this.CanRunInParallel = true

        /// <summary>
        /// 计算匹配评分
        /// </summary>
        member this.CalculateMatchScoreAsync(context, criteria, callbackProvider, cancellationToken) =
            Task.Run(fun () ->
                let startTime = DateTime.UtcNow

                try
                    // 验证输入参数
                    if criteria = null then
                        MatchResult.CreateNoMatch(
                            "Demographic",
                            Guid.NewGuid(),
                            "定向条件为空",
                            TimeSpan.Zero,
                            0,
                            0.0m,
                            false
                        )
                    elif context = null then
                        MatchResult.CreateNoMatch(
                            criteria.CriteriaType,
                            criteria.CriteriaId,
                            "用户上下文为空",
                            TimeSpan.Zero,
                            0,
                            0.0m,
                            false
                        )
                    else
                        // 检查是否支持此类型的定向条件
                        let matcher = this :> ITargetingMatcher

                        if not (matcher.IsSupported(criteria.CriteriaType)) then
                            MatchResult.CreateNoMatch(
                                criteria.CriteriaType,
                                criteria.CriteriaId,
                                $"不支持的定向条件类型: {criteria.CriteriaType}",
                                TimeSpan.Zero,
                                0,
                                0.0m,
                                false
                            )
                        else
                            // 执行具体的匹配逻辑
                            let matchResult =
                                this.ExecuteMatchingLogic(context, criteria, cancellationToken).Result

                            let executionTime = DateTime.UtcNow - startTime

                            // 返回最终匹配结果
                            if matchResult.IsMatch then
                                MatchResult.CreateMatch(
                                    criteria.CriteriaType,
                                    criteria.CriteriaId,
                                    matchResult.MatchScore,
                                    matchResult.MatchReason,
                                    executionTime,
                                    0,
                                    criteria.Weight,
                                    false,
                                    matchResult.MatchDetails
                                )
                            else
                                MatchResult.CreateNoMatch(
                                    criteria.CriteriaType,
                                    criteria.CriteriaId,
                                    matchResult.NotMatchReason,
                                    executionTime,
                                    0,
                                    criteria.Weight,
                                    false,
                                    matchResult.MatchDetails
                                )

                with ex ->
                    let executionTime = DateTime.UtcNow - startTime

                    let criteriaType =
                        if criteria <> null then
                            criteria.CriteriaType
                        else
                            "Unknown"

                    let criteriaId =
                        if criteria <> null then
                            criteria.CriteriaId
                        else
                            Guid.NewGuid()

                    MatchResult.CreateNoMatch(
                        criteriaType,
                        criteriaId,
                        $"匹配计算异常: {ex.Message}",
                        executionTime,
                        0,
                        0.0m,
                        false
                    ))

        /// <summary>
        /// 检查是否支持指定的定向条件类型
        /// </summary>
        member this.IsSupported(criteriaType) =
            let supportedTypes = [ "Demographic"; "Demographics"; "UserDemographic" ]
            supportedTypes |> List.contains criteriaType

        /// <summary>
        /// 验证定向条件的有效性
        /// </summary>
        member this.ValidateCriteria(criteria) =
            try
                if criteria = null then
                    ValidationResult.Failure([ ValidationError(Message = "定向条件不能为空") ], "验证失败", TimeSpan.Zero)
                else
                    let matcher = this :> ITargetingMatcher

                    if not (matcher.IsSupported(criteria.CriteriaType)) then
                        ValidationResult.Failure(
                            [ ValidationError(Message = $"不支持的定向条件类型: {criteria.CriteriaType}") ],
                            "验证失败",
                            TimeSpan.Zero
                        )
                    else
                        // 验证 DemographicTargeting 的具体规则
                        let hasAnyCondition =
                            criteria.HasRule("MinAge")
                            || criteria.HasRule("MaxAge")
                            || criteria.HasRule("TargetGenders")
                            || criteria.HasRule("TargetKeywords")

                        if not hasAnyCondition then
                            ValidationResult.Failure(
                                [ ValidationError(Message = "至少需要配置一个人口属性定向条件") ],
                                "验证失败",
                                TimeSpan.Zero
                            )
                        else
                            // 验证年龄范围
                            let minAge = criteria.GetRule<int option>("MinAge")
                            let maxAge = criteria.GetRule<int option>("MaxAge")

                            match minAge, maxAge with
                            | Some min, Some max when min > max ->
                                ValidationResult.Failure(
                                    [ ValidationError(Message = "最小年龄不能大于最大年龄") ],
                                    "验证失败",
                                    TimeSpan.Zero
                                )
                            | Some min, _ when min < 0 ->
                                ValidationResult.Failure(
                                    [ ValidationError(Message = "最小年龄不能为负数") ],
                                    "验证失败",
                                    TimeSpan.Zero
                                )
                            | _, Some max when max < 0 ->
                                ValidationResult.Failure(
                                    [ ValidationError(Message = "最大年龄不能为负数") ],
                                    "验证失败",
                                    TimeSpan.Zero
                                )
                            | _ -> ValidationResult.Success("人口属性定向条件验证通过", TimeSpan.Zero)

            with ex ->
                ValidationResult.Failure(
                    [ ValidationError(Message = $"验证过程中发生异常: {ex.Message}") ],
                    "验证异常",
                    TimeSpan.Zero
                )

        /// <summary>
        /// 获取匹配器元数据信息
        /// </summary>
        member this.GetMetadata() =
            TargetingMatcherMetadata(
                MatcherId = (this :> ITargetingMatcher).MatcherId,
                Name = (this :> ITargetingMatcher).MatcherName,
                Description = "人口属性定向匹配器，支持年龄、性别、关键词等人口统计学信息的匹配",
                Version = (this :> ITargetingMatcher).Version,
                MatcherType = (this :> ITargetingMatcher).MatcherType,
                Author = "Lorn.ADSP Team",
                SupportedCriteriaTypes = [ "Demographic"; "Demographics"; "UserDemographic" ],
                SupportedDimensions = [ "Age"; "Gender"; "Keywords" ],
                SupportsParallelExecution = true,
                SupportsCaching = true,
                SupportsBatchProcessing = false,
                ExpectedExecutionTime = TimeSpan.FromMilliseconds(10.0),
                MaxExecutionTime = TimeSpan.FromMilliseconds(100.0)
            )

    /// <summary>
    /// 执行匹配逻辑的私有方法
    /// </summary>
    member private this.ExecuteMatchingLogic
        (context: ITargetingContext, criteria: ITargetingCriteria, cancellationToken: CancellationToken)
        : Task<MatchResult> =
        task {
            // 从用户上下文中提取人口属性信息
            let userAge = this.ExtractUserAge(context)
            let userGender = this.ExtractUserGender(context)
            let userKeywords = this.ExtractUserKeywords(context)

            // 从定向条件中提取目标条件
            let targetMinAge = criteria.GetRule<int option>("MinAge")
            let targetMaxAge = criteria.GetRule<int option>("MaxAge")

            let targetGenders =
                let genders = criteria.GetRule<List<Gender>>("TargetGenders")
                if genders = null then List<Gender>() else genders

            let targetKeywords =
                let keywords = criteria.GetRule<List<string>>("TargetKeywords")
                if keywords = null then List<string>() else keywords

            // 执行各项匹配
            let ageMatch: MatchRecord = this.MatchAge(userAge, targetMinAge, targetMaxAge)
            let genderMatch: MatchRecord = this.MatchGender(userGender, targetGenders)
            let keywordMatch: MatchRecord = this.MatchKeywords(userKeywords, targetKeywords)

            // 计算综合匹配结果
            let matches =
                [ ageMatch; genderMatch; keywordMatch ] |> List.filter (fun m -> m.IsMatch)

            let totalMatches = matches.Length

            let totalConditions =
                [ ageMatch; genderMatch; keywordMatch ]
                |> List.filter (fun m -> m.HasCondition)
                |> List.length

            let isMatch = totalMatches > 0 && totalConditions > 0

            let matchScore =
                if totalConditions > 0 then
                    decimal totalMatches / decimal totalConditions
                else
                    0.0m

            let matchReason =
                if isMatch then
                    let matchedItems =
                        matches |> List.map (fun m -> m.AttributeName) |> String.concat ", "

                    $"匹配的人口属性: {matchedItems}"
                else
                    "未找到匹配的人口属性条件"

            let notMatchReason =
                if not isMatch then
                    let unmatchedItems =
                        [ ageMatch; genderMatch; keywordMatch ]
                        |> List.filter (fun m -> m.HasCondition && not m.IsMatch)
                        |> List.map (fun m -> m.AttributeName)
                        |> String.concat ", "

                    $"未匹配的人口属性: {unmatchedItems}"
                else
                    ""

            let matchDetails =
                [ ageMatch; genderMatch; keywordMatch ]
                |> List.filter (fun m -> m.HasCondition)
                |> List.map (fun m ->
                    ContextProperty(
                        m.AttributeName,
                        (if m.IsMatch then "匹配" else "不匹配"),
                        "String",
                        "MatchResult",
                        false,
                        1.0m,
                        Nullable(),
                        "DemographicMatcher"
                    ))
                |> List.toArray
                |> Array.toList
                :> IReadOnlyList<ContextProperty>

            return
                if isMatch then
                    MatchResult.CreateMatch(
                        "Demographic",
                        Guid.NewGuid(),
                        matchScore,
                        matchReason,
                        TimeSpan.Zero,
                        0,
                        1.0m,
                        false,
                        matchDetails
                    )
                else
                    MatchResult.CreateNoMatch(
                        "Demographic",
                        Guid.NewGuid(),
                        notMatchReason,
                        TimeSpan.Zero,
                        0,
                        1.0m,
                        false,
                        matchDetails
                    )
        }

    /// <summary>
    /// 从用户上下文中提取年龄信息
    /// </summary>
    member private this.ExtractUserAge(context: ITargetingContext) =
        try
            context.GetPropertyValue<int option>("Age")
        with _ ->
            None

    /// <summary>
    /// 从用户上下文中提取性别信息
    /// </summary>
    member private this.ExtractUserGender(context: ITargetingContext) =
        try
            let genderStr = context.GetPropertyAsString("Gender")

            if String.IsNullOrEmpty(genderStr) then
                None
            else
                let mutable gender = Gender.Unknown

                if Enum.TryParse<Gender>(genderStr, true, &gender) then
                    Some gender
                else
                    None
        with _ ->
            None

    /// <summary>
    /// 从用户上下文中提取关键词信息
    /// </summary>
    member private this.ExtractUserKeywords(context: ITargetingContext) =
        try
            let keywords =
                context.GetPropertiesByCategory("Interest")
                |> Seq.map (fun p -> p.PropertyKey)
                |> Seq.toList

            keywords
        with _ ->
            []

    /// <summary>
    /// 匹配年龄条件
    /// </summary>
    member private this.MatchAge(userAge: int option, minAge: int option, maxAge: int option) : MatchRecord =
        let hasCondition = minAge.IsSome || maxAge.IsSome

        let isMatch =
            if not hasCondition then
                true // 没有年龄条件时默认匹配
            else
                match userAge with
                | None -> false // 用户年龄未知时不匹配
                | Some age ->
                    let minMatch =
                        minAge |> Option.map (fun min -> age >= min) |> Option.defaultValue true

                    let maxMatch =
                        maxAge |> Option.map (fun max -> age <= max) |> Option.defaultValue true

                    minMatch && maxMatch

        { AttributeName = "Age"
          IsMatch = isMatch
          HasCondition = hasCondition }

    /// <summary>
    /// 匹配性别条件
    /// </summary>
    member private this.MatchGender(userGender: Gender option, targetGenders: List<Gender>) : MatchRecord =
        let hasCondition = targetGenders.Count > 0

        let isMatch =
            if not hasCondition then
                true // 没有性别条件时默认匹配
            else
                match userGender with
                | None -> false // 用户性别未知时不匹配
                | Some gender -> targetGenders |> Seq.contains gender

        { AttributeName = "Gender"
          IsMatch = isMatch
          HasCondition = hasCondition }

    /// <summary>
    /// 匹配关键词条件
    /// </summary>
    member private this.MatchKeywords(userKeywords: string list, targetKeywords: List<string>) : MatchRecord =
        let hasCondition = targetKeywords.Count > 0

        let isMatch =
            if not hasCondition then
                true // 没有关键词条件时默认匹配
            else if userKeywords.IsEmpty then
                false // 用户没有关键词时不匹配
            else
                // 检查是否有交集
                let userKeywordSet = Set.ofList userKeywords
                let targetKeywordSet = Set.ofSeq targetKeywords
                not (Set.isEmpty (Set.intersect userKeywordSet targetKeywordSet))

        { AttributeName = "Keywords"
          IsMatch = isMatch
          HasCondition = hasCondition }
