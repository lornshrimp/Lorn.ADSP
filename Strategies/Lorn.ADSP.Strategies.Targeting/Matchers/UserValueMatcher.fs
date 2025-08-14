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

// 定义价值匹配记录类型
type ValueMatchRecord =
    { AttributeName: string
      IsMatch: bool
      HasCondition: bool
      ExpectedValue: obj
      ActualValue: obj
      MatchScore: decimal }

// 定义价值匹配结果类型
type ValueMatchResult =
    { IsMatch: bool
      Score: decimal
      MatchReason: string
      NotMatchReason: string
      MatchDetails: IReadOnlyList<ContextProperty>
      ValueTierMatch: bool
      SpendingLevelMatch: bool
      ConversionProbabilityMatch: bool
      LifetimeValueMatch: bool }

/// <summary>
/// 用户价值定向匹配器
/// 实现 ITargetingMatcher 接口，处理用户价值相关的定向匹配
/// 基于用户价值等级、消费能力、转化概率和生命周期价值进行精准匹配
/// </summary>
type UserValueTargetingMatcher() =

    // 实现 ITargetingMatcher 接口的属性
    interface ITargetingMatcher with
        member this.MatcherId = "user-value-matcher-v1"
        member this.MatcherName = "用户价值定向匹配器"
        member this.Version = "1.0.0"
        member this.MatcherType = "UserValue"
        member this.Priority = 85
        member this.IsEnabled = true
        member this.ExpectedExecutionTime = TimeSpan.FromMilliseconds(15.0)
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
                        MatchResult.CreateNoMatch("UserValue", Guid.NewGuid(), "定向条件为空", TimeSpan.Zero, 0, 0.0m, false)
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
                                this.ExecuteValueMatchingLogic(context, criteria, cancellationToken).Result

                            let executionTime = DateTime.UtcNow - startTime

                            // 返回最终匹配结果
                            if matchResult.IsMatch then
                                MatchResult.CreateMatch(
                                    criteria.CriteriaType,
                                    criteria.CriteriaId,
                                    matchResult.Score,
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
                        $"用户价值匹配计算异常: {ex.Message}",
                        executionTime,
                        0,
                        0.0m,
                        false
                    ))

        /// <summary>
        /// 检查是否支持指定的定向条件类型
        /// </summary>
        member this.IsSupported(criteriaType) =
            let supportedTypes =
                [ "UserValue"; "Value"; "UserValueTargeting"; "ValueTargeting" ]

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
                        // 验证用户价值定向的具体规则
                        let hasAnyCondition =
                            criteria.HasRule("TargetValueTiers")
                            || criteria.HasRule("MinValueTier")
                            || criteria.HasRule("TargetSpendingLevel")
                            || criteria.HasRule("MinSpendingLevel")
                            || criteria.HasRule("MinConversionProbability")
                            || criteria.HasRule("MaxConversionProbability")
                            || criteria.HasRule("MinLifetimeValue")
                            || criteria.HasRule("MaxLifetimeValue")
                            || criteria.HasRule("MinOverallScore")
                            || criteria.HasRule("MaxOverallScore")

                        if not hasAnyCondition then
                            ValidationResult.Failure(
                                [ ValidationError(Message = "至少需要配置一个用户价值定向条件") ],
                                "验证失败",
                                TimeSpan.Zero
                            )
                        else
                            // 验证转化概率范围
                            let minConversionProb = criteria.GetRule<decimal option>("MinConversionProbability")
                            let maxConversionProb = criteria.GetRule<decimal option>("MaxConversionProbability")

                            match minConversionProb, maxConversionProb with
                            | Some min, Some max when min > max ->
                                ValidationResult.Failure(
                                    [ ValidationError(Message = "最小转化概率不能大于最大转化概率") ],
                                    "验证失败",
                                    TimeSpan.Zero
                                )
                            | Some min, _ when min < 0.0m || min > 1.0m ->
                                ValidationResult.Failure(
                                    [ ValidationError(Message = "转化概率必须在0.0-1.0之间") ],
                                    "验证失败",
                                    TimeSpan.Zero
                                )
                            | _, Some max when max < 0.0m || max > 1.0m ->
                                ValidationResult.Failure(
                                    [ ValidationError(Message = "转化概率必须在0.0-1.0之间") ],
                                    "验证失败",
                                    TimeSpan.Zero
                                )
                            | _ ->
                                // 验证生命周期价值范围
                                let minLTV = criteria.GetRule<decimal option>("MinLifetimeValue")
                                let maxLTV = criteria.GetRule<decimal option>("MaxLifetimeValue")

                                match minLTV, maxLTV with
                                | Some min, Some max when min > max ->
                                    ValidationResult.Failure(
                                        [ ValidationError(Message = "最小生命周期价值不能大于最大生命周期价值") ],
                                        "验证失败",
                                        TimeSpan.Zero
                                    )
                                | Some min, _ when min < 0.0m ->
                                    ValidationResult.Failure(
                                        [ ValidationError(Message = "生命周期价值不能为负数") ],
                                        "验证失败",
                                        TimeSpan.Zero
                                    )
                                | _ ->
                                    // 验证评分范围
                                    let minScore = criteria.GetRule<int option>("MinOverallScore")
                                    let maxScore = criteria.GetRule<int option>("MaxOverallScore")

                                    match minScore, maxScore with
                                    | Some min, Some max when min > max ->
                                        ValidationResult.Failure(
                                            [ ValidationError(Message = "最小综合评分不能大于最大综合评分") ],
                                            "验证失败",
                                            TimeSpan.Zero
                                        )
                                    | Some min, _ when min < 0 || min > 100 ->
                                        ValidationResult.Failure(
                                            [ ValidationError(Message = "综合评分必须在0-100之间") ],
                                            "验证失败",
                                            TimeSpan.Zero
                                        )
                                    | _, Some max when max < 0 || max > 100 ->
                                        ValidationResult.Failure(
                                            [ ValidationError(Message = "综合评分必须在0-100之间") ],
                                            "验证失败",
                                            TimeSpan.Zero
                                        )
                                    | _ -> ValidationResult.Success("验证通过", TimeSpan.Zero)

            with ex ->
                ValidationResult.Failure([ ValidationError(Message = $"验证过程异常: {ex.Message}") ], "验证异常", TimeSpan.Zero)

        /// <summary>
        /// 获取匹配器元数据信息
        /// </summary>
        member this.GetMetadata() =
            TargetingMatcherMetadata(
                MatcherId = (this :> ITargetingMatcher).MatcherId,
                Name = (this :> ITargetingMatcher).MatcherName,
                Description = "用户价值定向匹配器，支持价值层级、消费水平、转化概率、生命周期价值等用户价值维度的匹配",
                Version = (this :> ITargetingMatcher).Version,
                MatcherType = (this :> ITargetingMatcher).MatcherType,
                Author = "Lorn.ADSP Team",
                SupportedCriteriaTypes = [ "UserValue"; "Value"; "ValueTier"; "SpendingLevel" ],
                SupportedDimensions = [ "ValueTier"; "SpendingLevel"; "ConversionProbability"; "LifetimeValue" ],
                SupportsParallelExecution = true,
                SupportsCaching = true,
                SupportsBatchProcessing = false,
                ExpectedExecutionTime = TimeSpan.FromMilliseconds(15.0),
                MaxExecutionTime = TimeSpan.FromMilliseconds(200.0)
            )

        /// <summary>
        /// 预热匹配器
        /// </summary>
        member this.WarmUpAsync(callbackProvider: ICallbackProvider, cancellationToken: CancellationToken) =
            Task.Run(
                (fun () ->
                    // 用户价值匹配器不需要预热操作
                    ()),
                cancellationToken
            )

        /// <summary>
        /// 清理匹配器资源
        /// </summary>
        member this.CleanupAsync() =
            Task.Run(fun () ->
                // 用户价值匹配器不需要清理操作
                ())

    /// <summary>
    /// 执行用户价值匹配逻辑的核心方法
    /// </summary>
    member this.ExecuteValueMatchingLogic
        (context: ITargetingContext, criteria: ITargetingCriteria, cancellationToken: CancellationToken)
        =
        async {
            try
                // 从上下文中获取用户价值信息
                let userValueContext = this.ExtractUserValueContext(context)

                match userValueContext with
                | None ->
                    return
                        { IsMatch = false
                          Score = 0.0m
                          MatchReason = ""
                          NotMatchReason = "未能获取用户价值信息"
                          MatchDetails = []
                          ValueTierMatch = false
                          SpendingLevelMatch = false
                          ConversionProbabilityMatch = false
                          LifetimeValueMatch = false }
                | Some userValue ->
                    // 并行执行各个维度的匹配
                    let! matchResults =
                        [ this.MatchValueTier(userValue, criteria)
                          this.MatchSpendingLevel(userValue, criteria)
                          this.MatchConversionProbability(userValue, criteria)
                          this.MatchLifetimeValue(userValue, criteria)
                          this.MatchOverallScore(userValue, criteria) ]
                        |> Async.Parallel

                    // 合并匹配结果
                    let overallResult = this.CombineMatchResults(matchResults, userValue, criteria)
                    return overallResult

            with ex ->
                return
                    { IsMatch = false
                      Score = 0.0m
                      MatchReason = ""
                      NotMatchReason = $"用户价值匹配异常: {ex.Message}"
                      MatchDetails = []
                      ValueTierMatch = false
                      SpendingLevelMatch = false
                      ConversionProbabilityMatch = false
                      LifetimeValueMatch = false }
        }
        |> Async.StartAsTask

    /// <summary>
    /// 从上下文中提取用户价值信息
    /// </summary>
    member private this.ExtractUserValueContext(context: ITargetingContext) =
        try
            // 尝试从上下文中获取 UserValue 对象
            match context with
            | :? UserValue as userValue -> Some userValue
            | _ ->
                // 如果上下文不是 UserValue 类型，尝试从属性中构造
                let engagementScore = context.GetPropertyValue("EngagementScore", 0)
                let loyaltyScore = context.GetPropertyValue("LoyaltyScore", 0)
                let monetaryScore = context.GetPropertyValue("MonetaryScore", 0)
                let potentialScore = context.GetPropertyValue("PotentialScore", 0)
                let estimatedLTV = context.GetPropertyValue("EstimatedLTV", 0.0m)
                let spendingLevel = context.GetPropertyValue("SpendingLevel", SpendingLevel.Medium)
                let valueTier = context.GetPropertyValue("ValueTier", ValueTier.Standard)
                let conversionProbability = context.GetPropertyValue("ConversionProbability", 0.0m)

                Some(
                    UserValue(
                        engagementScore = engagementScore,
                        loyaltyScore = loyaltyScore,
                        monetaryScore = monetaryScore,
                        potentialScore = potentialScore,
                        estimatedLTV = estimatedLTV,
                        spendingLevel = spendingLevel,
                        valueTier = valueTier,
                        conversionProbability = conversionProbability
                    )
                )
        with _ ->
            None

    /// <summary>
    /// 匹配用户价值等级
    /// </summary>
    member private this.MatchValueTier(userValue: UserValue, criteria: ITargetingCriteria) =
        async {
            try
                let targetValueTier = criteria.GetRule<ValueTier option>("TargetValueTier")
                let minValueTier = criteria.GetRule<ValueTier option>("MinValueTier")

                match targetValueTier, minValueTier with
                | Some targetTier, _ ->
                    let isMatch = userValue.ValueTier = targetTier

                    return
                        { AttributeName = "ValueTier"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = targetTier :> obj
                          ActualValue = userValue.ValueTier :> obj
                          MatchScore = if isMatch then 1.0m else 0.0m }
                | None, Some minTier ->
                    let isMatch = userValue.ValueTier >= minTier

                    return
                        { AttributeName = "MinValueTier"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = minTier :> obj
                          ActualValue = userValue.ValueTier :> obj
                          MatchScore = if isMatch then 1.0m else 0.0m }
                | None, None ->
                    return
                        { AttributeName = "ValueTier"
                          IsMatch = true
                          HasCondition = false
                          ExpectedValue = "无条件" :> obj
                          ActualValue = userValue.ValueTier :> obj
                          MatchScore = 1.0m }
            with ex ->
                return
                    { AttributeName = "ValueTier"
                      IsMatch = false
                      HasCondition = true
                      ExpectedValue = "异常" :> obj
                      ActualValue = userValue.ValueTier :> obj
                      MatchScore = 0.0m }
        }

    /// <summary>
    /// 匹配消费能力等级
    /// </summary>
    member private this.MatchSpendingLevel(userValue: UserValue, criteria: ITargetingCriteria) =
        async {
            try
                let targetSpendingLevel =
                    criteria.GetRule<SpendingLevel option>("TargetSpendingLevel")

                let minSpendingLevel = criteria.GetRule<SpendingLevel option>("MinSpendingLevel")

                match targetSpendingLevel, minSpendingLevel with
                | Some targetLevel, _ ->
                    let isMatch = userValue.SpendingLevel = targetLevel

                    return
                        { AttributeName = "SpendingLevel"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = targetLevel :> obj
                          ActualValue = userValue.SpendingLevel :> obj
                          MatchScore = if isMatch then 1.0m else 0.0m }
                | None, Some minLevel ->
                    let isMatch = userValue.SpendingLevel >= minLevel

                    return
                        { AttributeName = "MinSpendingLevel"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = minLevel :> obj
                          ActualValue = userValue.SpendingLevel :> obj
                          MatchScore = if isMatch then 1.0m else 0.0m }
                | None, None ->
                    return
                        { AttributeName = "SpendingLevel"
                          IsMatch = true
                          HasCondition = false
                          ExpectedValue = "无条件" :> obj
                          ActualValue = userValue.SpendingLevel :> obj
                          MatchScore = 1.0m }
            with ex ->
                return
                    { AttributeName = "SpendingLevel"
                      IsMatch = false
                      HasCondition = true
                      ExpectedValue = "异常" :> obj
                      ActualValue = userValue.SpendingLevel :> obj
                      MatchScore = 0.0m }
        }

    /// <summary>
    /// 匹配转化概率
    /// </summary>
    member private this.MatchConversionProbability(userValue: UserValue, criteria: ITargetingCriteria) =
        async {
            try
                let minConversionProb = criteria.GetRule<decimal option>("MinConversionProbability")
                let maxConversionProb = criteria.GetRule<decimal option>("MaxConversionProbability")

                match minConversionProb, maxConversionProb with
                | Some min, Some max ->
                    let isMatch =
                        userValue.ConversionProbability >= min && userValue.ConversionProbability <= max

                    let score =
                        if isMatch then
                            // 根据转化概率在范围内的位置计算评分
                            let position = (userValue.ConversionProbability - min) / (max - min)
                            0.5m + position * 0.5m
                        else
                            0.0m

                    return
                        { AttributeName = "ConversionProbability"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = $"{min:P1}-{max:P1}" :> obj
                          ActualValue = userValue.ConversionProbability :> obj
                          MatchScore = score }
                | Some min, None ->
                    let isMatch = userValue.ConversionProbability >= min

                    let score =
                        if isMatch then
                            // 基于超过最小值的程度计算评分
                            let excess = userValue.ConversionProbability - min
                            0.5m + Math.Min(excess * 2.0m, 0.5m)
                        else
                            0.0m

                    return
                        { AttributeName = "MinConversionProbability"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = min :> obj
                          ActualValue = userValue.ConversionProbability :> obj
                          MatchScore = score }
                | None, Some max ->
                    let isMatch = userValue.ConversionProbability <= max

                    return
                        { AttributeName = "MaxConversionProbability"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = max :> obj
                          ActualValue = userValue.ConversionProbability :> obj
                          MatchScore = if isMatch then 1.0m else 0.0m }
                | None, None ->
                    return
                        { AttributeName = "ConversionProbability"
                          IsMatch = true
                          HasCondition = false
                          ExpectedValue = "无条件" :> obj
                          ActualValue = userValue.ConversionProbability :> obj
                          MatchScore = 1.0m }
            with ex ->
                return
                    { AttributeName = "ConversionProbability"
                      IsMatch = false
                      HasCondition = true
                      ExpectedValue = "异常" :> obj
                      ActualValue = userValue.ConversionProbability :> obj
                      MatchScore = 0.0m }
        }

    /// <summary>
    /// 匹配生命周期价值
    /// </summary>
    member private this.MatchLifetimeValue(userValue: UserValue, criteria: ITargetingCriteria) =
        async {
            try
                let minLTV = criteria.GetRule<decimal option>("MinLifetimeValue")
                let maxLTV = criteria.GetRule<decimal option>("MaxLifetimeValue")

                match minLTV, maxLTV with
                | Some min, Some max ->
                    let isMatch = userValue.EstimatedLTV >= min && userValue.EstimatedLTV <= max

                    let score =
                        if isMatch then
                            // 根据LTV在范围内的位置计算评分
                            if max = min then
                                1.0m
                            else
                                let position = (userValue.EstimatedLTV - min) / (max - min)
                                0.5m + position * 0.5m
                        else
                            0.0m

                    return
                        { AttributeName = "LifetimeValue"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = $"{min:C}-{max:C}" :> obj
                          ActualValue = userValue.EstimatedLTV :> obj
                          MatchScore = score }
                | Some min, None ->
                    let isMatch = userValue.EstimatedLTV >= min

                    let score =
                        if isMatch then
                            // 基于超过最小值的程度计算评分
                            let ratio = userValue.EstimatedLTV / min
                            Math.Min(0.5m + ratio * 0.1m, 1.0m)
                        else
                            0.0m

                    return
                        { AttributeName = "MinLifetimeValue"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = min :> obj
                          ActualValue = userValue.EstimatedLTV :> obj
                          MatchScore = score }
                | None, Some max ->
                    let isMatch = userValue.EstimatedLTV <= max

                    return
                        { AttributeName = "MaxLifetimeValue"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = max :> obj
                          ActualValue = userValue.EstimatedLTV :> obj
                          MatchScore = if isMatch then 1.0m else 0.0m }
                | None, None ->
                    return
                        { AttributeName = "LifetimeValue"
                          IsMatch = true
                          HasCondition = false
                          ExpectedValue = "无条件" :> obj
                          ActualValue = userValue.EstimatedLTV :> obj
                          MatchScore = 1.0m }
            with ex ->
                return
                    { AttributeName = "LifetimeValue"
                      IsMatch = false
                      HasCondition = true
                      ExpectedValue = "异常" :> obj
                      ActualValue = userValue.EstimatedLTV :> obj
                      MatchScore = 0.0m }
        }

    /// <summary>
    /// 匹配综合评分
    /// </summary>
    member private this.MatchOverallScore(userValue: UserValue, criteria: ITargetingCriteria) =
        async {
            try
                let minScore = criteria.GetRule<int option>("MinOverallScore")
                let maxScore = criteria.GetRule<int option>("MaxOverallScore")

                match minScore, maxScore with
                | Some min, Some max ->
                    let isMatch = userValue.OverallScore >= min && userValue.OverallScore <= max

                    let score =
                        if isMatch then
                            // 根据评分在范围内的位置计算评分
                            if max = min then
                                1.0m
                            else
                                let position = decimal (userValue.OverallScore - min) / decimal (max - min)
                                0.5m + position * 0.5m
                        else
                            0.0m

                    return
                        { AttributeName = "OverallScore"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = $"{min}-{max}" :> obj
                          ActualValue = userValue.OverallScore :> obj
                          MatchScore = score }
                | Some min, None ->
                    let isMatch = userValue.OverallScore >= min

                    let score =
                        if isMatch then
                            // 基于超过最小值的程度计算评分
                            let excess = decimal (userValue.OverallScore - min) / 100.0m
                            0.5m + Math.Min(excess * 2.0m, 0.5m)
                        else
                            0.0m

                    return
                        { AttributeName = "MinOverallScore"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = min :> obj
                          ActualValue = userValue.OverallScore :> obj
                          MatchScore = score }
                | None, Some max ->
                    let isMatch = userValue.OverallScore <= max

                    return
                        { AttributeName = "MaxOverallScore"
                          IsMatch = isMatch
                          HasCondition = true
                          ExpectedValue = max :> obj
                          ActualValue = userValue.OverallScore :> obj
                          MatchScore = if isMatch then 1.0m else 0.0m }
                | None, None ->
                    return
                        { AttributeName = "OverallScore"
                          IsMatch = true
                          HasCondition = false
                          ExpectedValue = "无条件" :> obj
                          ActualValue = userValue.OverallScore :> obj
                          MatchScore = 1.0m }
            with ex ->
                return
                    { AttributeName = "OverallScore"
                      IsMatch = false
                      HasCondition = true
                      ExpectedValue = "异常" :> obj
                      ActualValue = userValue.OverallScore :> obj
                      MatchScore = 0.0m }
        }

    /// <summary>
    /// 合并各个维度的匹配结果
    /// </summary>
    member private this.CombineMatchResults
        (matchResults: ValueMatchRecord[], userValue: UserValue, criteria: ITargetingCriteria)
        =
        try
            // 分离有条件的匹配结果和无条件的结果
            let conditionResults = matchResults |> Array.filter (fun x -> x.HasCondition)
            let allConditionResults = matchResults

            // 如果没有任何条件，则默认匹配
            if conditionResults.Length = 0 then
                { IsMatch = true
                  Score = 0.5m // 默认评分
                  MatchReason = "无价值定向条件，默认匹配"
                  NotMatchReason = ""
                  MatchDetails =
                    [ ContextProperty(
                          "用户价值等级",
                          userValue.ValueTier.ToString(),
                          "String",
                          "UserValue",
                          false,
                          1.0m,
                          Nullable(),
                          "System"
                      )
                      ContextProperty(
                          "消费能力",
                          userValue.SpendingLevel.ToString(),
                          "String",
                          "UserValue",
                          false,
                          1.0m,
                          Nullable(),
                          "System"
                      )
                      ContextProperty(
                          "转化概率",
                          $"{userValue.ConversionProbability:P1}",
                          "String",
                          "UserValue",
                          false,
                          1.0m,
                          Nullable(),
                          "System"
                      )
                      ContextProperty(
                          "生命周期价值",
                          $"{userValue.EstimatedLTV:C}",
                          "String",
                          "UserValue",
                          false,
                          1.0m,
                          Nullable(),
                          "System"
                      )
                      ContextProperty(
                          "综合评分",
                          userValue.OverallScore.ToString(),
                          "String",
                          "UserValue",
                          false,
                          1.0m,
                          Nullable(),
                          "System"
                      ) ]
                  ValueTierMatch = true
                  SpendingLevelMatch = true
                  ConversionProbabilityMatch = true
                  LifetimeValueMatch = true }
            else
                // 检查所有有条件的匹配是否都成功
                let allMatched = conditionResults |> Array.forall (fun x -> x.IsMatch)

                if allMatched then
                    // 计算加权平均评分
                    let weightedScore =
                        conditionResults |> Array.map (fun x -> x.MatchScore) |> Array.average

                    let matchedConditions =
                        conditionResults
                        |> Array.filter (fun x -> x.IsMatch)
                        |> Array.map (fun x -> x.AttributeName)
                        |> String.concat ", "

                    let matchDetails =
                        [ for result in allConditionResults do
                              yield
                                  ContextProperty(
                                      result.AttributeName,
                                      result.ActualValue.ToString(),
                                      "String",
                                      "UserValue",
                                      false,
                                      result.MatchScore,
                                      Nullable(),
                                      "System"
                                  ) ]

                    { IsMatch = true
                      Score = weightedScore
                      MatchReason = $"用户价值匹配成功: {matchedConditions}"
                      NotMatchReason = ""
                      MatchDetails = matchDetails
                      ValueTierMatch =
                        allConditionResults
                        |> Array.exists (fun x -> x.AttributeName.Contains("ValueTier") && x.IsMatch)
                      SpendingLevelMatch =
                        allConditionResults
                        |> Array.exists (fun x -> x.AttributeName.Contains("SpendingLevel") && x.IsMatch)
                      ConversionProbabilityMatch =
                        allConditionResults
                        |> Array.exists (fun x -> x.AttributeName.Contains("ConversionProbability") && x.IsMatch)
                      LifetimeValueMatch =
                        allConditionResults
                        |> Array.exists (fun x -> x.AttributeName.Contains("LifetimeValue") && x.IsMatch) }
                else
                    // 找出不匹配的条件
                    let notMatchedConditions =
                        conditionResults
                        |> Array.filter (fun x -> not x.IsMatch)
                        |> Array.map (fun x -> $"{x.AttributeName}({x.ExpectedValue} vs {x.ActualValue})")
                        |> String.concat ", "

                    let matchDetails =
                        [ for result in allConditionResults do
                              yield
                                  ContextProperty(
                                      result.AttributeName,
                                      result.ActualValue.ToString(),
                                      "String",
                                      "UserValue",
                                      false,
                                      result.MatchScore,
                                      Nullable(),
                                      "System"
                                  ) ]

                    { IsMatch = false
                      Score = 0.0m
                      MatchReason = ""
                      NotMatchReason = $"用户价值不匹配: {notMatchedConditions}"
                      MatchDetails = matchDetails
                      ValueTierMatch = false
                      SpendingLevelMatch = false
                      ConversionProbabilityMatch = false
                      LifetimeValueMatch = false }

        with ex ->
            { IsMatch = false
              Score = 0.0m
              MatchReason = ""
              NotMatchReason = $"合并匹配结果异常: {ex.Message}"
              MatchDetails = []
              ValueTierMatch = false
              SpendingLevelMatch = false
              ConversionProbabilityMatch = false
              LifetimeValueMatch = false }
