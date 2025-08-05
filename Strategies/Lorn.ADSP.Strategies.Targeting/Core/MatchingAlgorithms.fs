namespace Lorn.ADSP.Strategies.Targeting.Core

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open FSharp.Control
open FSharp.Collections.ParallelSeq
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces

/// <summary>
/// F#通用匹配算法模块
/// 实现通用匹配函数、并行匹配计算、权重评分、快速失败等算法
/// 直接使用Core.Domain中的C#类型，提供高性能的异步匹配计算
/// </summary>
module MatchingAlgorithms =

    /// <summary>
    /// 匹配计算上下文，封装匹配所需的所有信息
    /// </summary>
    type MatchingContext =
        {
            /// 定向上下文
            TargetingContext: ITargetingContext
            /// 定向条件
            Criteria: ITargetingCriteria
            /// 回调提供者
            CallbackProvider: ICallbackProvider
            /// 取消令牌
            CancellationToken: CancellationToken
            /// 匹配器标识
            MatcherId: string
            /// 匹配器类型
            MatcherType: string
            /// 开始时间
            StartTime: DateTime
        }

    /// <summary>
    /// 匹配计算结果，包含详细的匹配信息
    /// </summary>
    type MatchingResult =
        {
            /// 是否匹配
            IsMatch: bool
            /// 匹配分数 (0.0 - 1.0)
            Score: decimal
            /// 匹配原因
            Reason: string
            /// 不匹配原因
            NotMatchReason: string
            /// 匹配详情
            Details: IDictionary<string, obj>
            /// 执行时间
            ExecutionTime: TimeSpan
            /// 权重
            Weight: decimal
            /// 优先级
            Priority: int
            /// 是否必需
            IsRequired: bool
        }

    /// <summary>
    /// 批量匹配结果
    /// </summary>
    type BatchMatchingResult =
        {
            /// 匹配结果列表
            Results: MatchingResult list
            /// 总执行时间
            TotalExecutionTime: TimeSpan
            /// 成功匹配数量
            SuccessCount: int
            /// 失败匹配数量
            FailureCount: int
            /// 平均匹配分数
            AverageScore: decimal
        }

    /// <summary>
    /// 匹配器配置
    /// </summary>
    type MatcherConfiguration =
        {
            /// 超时时间
            Timeout: TimeSpan
            /// 是否启用缓存
            CacheEnabled: bool
            /// 缓存过期时间
            CacheExpiration: TimeSpan
            /// 最大并行度
            MaxParallelism: int
            /// 快速失败阈值
            FastFailThreshold: decimal
            /// 权重
            Weight: decimal
            /// 优先级
            Priority: int
        }

    /// <summary>
    /// 默认匹配器配置
    /// </summary>
    let defaultConfiguration =
        { Timeout = TimeSpan.FromMilliseconds(50.0)
          CacheEnabled = true
          CacheExpiration = TimeSpan.FromMinutes(5.0)
          MaxParallelism = Environment.ProcessorCount
          FastFailThreshold = 0.1m
          Weight = 1.0m
          Priority = 0 }

    /// <summary>
    /// 创建匹配上下文
    /// </summary>
    let createMatchingContext
        (targetingContext: ITargetingContext)
        (criteria: ITargetingCriteria)
        (callbackProvider: ICallbackProvider)
        (cancellationToken: CancellationToken)
        (matcherId: string)
        (matcherType: string)
        =
        { TargetingContext = targetingContext
          Criteria = criteria
          CallbackProvider = callbackProvider
          CancellationToken = cancellationToken
          MatcherId = matcherId
          MatcherType = matcherType
          StartTime = DateTime.UtcNow }

    /// <summary>
    /// 创建匹配成功结果
    /// </summary>
    let createMatchResult
        (score: decimal)
        (reason: string)
        (details: IDictionary<string, obj>)
        (executionTime: TimeSpan)
        (weight: decimal)
        (priority: int)
        (isRequired: bool)
        =
        { IsMatch = true
          Score = score
          Reason = reason
          NotMatchReason = ""
          Details = details
          ExecutionTime = executionTime
          Weight = weight
          Priority = priority
          IsRequired = isRequired }

    /// <summary>
    /// 创建匹配失败结果
    /// </summary>
    let createNoMatchResult
        (notMatchReason: string)
        (details: IDictionary<string, obj>)
        (executionTime: TimeSpan)
        (weight: decimal)
        (priority: int)
        (isRequired: bool)
        =
        { IsMatch = false
          Score = 0.0m
          Reason = ""
          NotMatchReason = notMatchReason
          Details = details
          ExecutionTime = executionTime
          Weight = weight
          Priority = priority
          IsRequired = isRequired }

    /// <summary>
    /// 将F#匹配结果转换为C# MatchResult
    /// </summary>
    let convertToMatchResult (criteriaType: string) (criteriaId: Guid) (result: MatchingResult) =

        let contextProperties =
            result.Details
            |> Seq.map (fun kvp ->
                new ContextProperty(
                    kvp.Key,
                    kvp.Value.ToString(),
                    kvp.Value.GetType().Name,
                    "MatchDetail",
                    false,
                    1.0m,
                    Nullable<DateTime>(),
                    "MatchingAlgorithms"
                ))
            |> List.ofSeq
            |> fun list -> list :> IReadOnlyList<ContextProperty>

        if result.IsMatch then
            MatchResult.CreateMatch(
                criteriaType,
                criteriaId,
                result.Score,
                result.Reason,
                result.ExecutionTime,
                result.Priority,
                result.Weight,
                result.IsRequired,
                contextProperties
            )
        else
            MatchResult.CreateNoMatch(
                criteriaType,
                criteriaId,
                result.NotMatchReason,
                result.ExecutionTime,
                result.Priority,
                result.Weight,
                result.IsRequired,
                contextProperties
            )

    /// <summary>
    /// 计算加权分数
    /// 根据权重和原始分数计算最终的加权分数
    /// </summary>
    let calculateWeightedScore (score: decimal) (weight: decimal) = score * weight

    /// <summary>
    /// 计算归一化分数
    /// 将分数归一化到0-1范围内
    /// </summary>
    let normalizeScore (score: decimal) (minScore: decimal) (maxScore: decimal) =
        if maxScore = minScore then
            1.0m
        else
            let normalized = (score - minScore) / (maxScore - minScore)
            Math.Max(0.0m, Math.Min(1.0m, normalized))

    /// <summary>
    /// 应用时间衰减
    /// 根据时间差应用衰减因子
    /// </summary>
    let applyTimeDecay (score: decimal) (lastUpdateTime: DateTime) (decayRate: decimal) =
        let timeDiff = DateTime.UtcNow - lastUpdateTime
        let decayFactor = Math.Exp(float (-decayRate * decimal timeDiff.TotalDays))
        score * decimal decayFactor

    /// <summary>
    /// 快速失败检查
    /// 如果分数低于阈值，立即返回失败结果
    /// </summary>
    let checkFastFail (score: decimal) (threshold: decimal) (reason: string) =
        if score < threshold then
            Some(createNoMatchResult reason (Dictionary<string, obj>()) TimeSpan.Zero 1.0m 0 false)
        else
            None

    /// <summary>
    /// 并行计算多个匹配分数
    /// 使用F#的并行序列处理多个匹配计算
    /// </summary>
    let calculateParallelScores
        (contexts: MatchingContext list)
        (matchingFunction: MatchingContext -> Async<MatchingResult>)
        (maxParallelism: int)
        =

        async {
            let! results =
                contexts
                |> PSeq.withDegreeOfParallelism maxParallelism
                |> PSeq.map (fun context ->
                    async {
                        try
                            let! result = matchingFunction context
                            return Some result
                        with
                        | :? OperationCanceledException -> return None
                        | ex ->
                            let errorResult =
                                createNoMatchResult
                                    $"计算异常: {ex.Message}"
                                    (Dictionary<string, obj>())
                                    TimeSpan.Zero
                                    1.0m
                                    0
                                    false

                            return Some errorResult
                    })
                |> PSeq.toArray
                |> fun asyncArray -> Async.Parallel asyncArray

            return results |> Array.choose id |> List.ofArray
        }

    /// <summary>
    /// 批量匹配处理
    /// 对多个上下文执行批量匹配，优化共同计算部分
    /// </summary>
    let processBatchMatching
        (contexts: MatchingContext list)
        (matchingFunction: MatchingContext -> Async<MatchingResult>)
        (config: MatcherConfiguration)
        =

        async {
            let startTime = DateTime.UtcNow

            let! results = calculateParallelScores contexts matchingFunction config.MaxParallelism

            let totalExecutionTime = DateTime.UtcNow - startTime
            let successCount = results |> List.filter (fun r -> r.IsMatch) |> List.length
            let failureCount = results.Length - successCount

            let averageScore =
                if results.Length > 0 then
                    results |> List.map (fun r -> r.Score) |> List.average
                else
                    0.0m

            return
                { Results = results
                  TotalExecutionTime = totalExecutionTime
                  SuccessCount = successCount
                  FailureCount = failureCount
                  AverageScore = averageScore }
        }

    /// <summary>
    /// 异步匹配计算包装器
    /// 提供超时控制和异常处理的异步匹配计算
    /// </summary>
    let executeMatchingWithTimeout
        (context: MatchingContext)
        (matchingFunction: MatchingContext -> Async<MatchingResult>)
        (timeout: TimeSpan)
        =

        async {
            try
                use cts = new CancellationTokenSource(timeout)
                let! result = Async.StartChild(matchingFunction context, int timeout.TotalMilliseconds)
                let! finalResult = result
                return finalResult
            with
            | :? TimeoutException ->
                return createNoMatchResult "匹配计算超时" (Dictionary<string, obj>()) timeout 1.0m 0 false
            | :? OperationCanceledException ->
                return createNoMatchResult "匹配计算被取消" (Dictionary<string, obj>()) TimeSpan.Zero 1.0m 0 false
            | ex ->
                return
                    createNoMatchResult $"匹配计算异常: {ex.Message}" (Dictionary<string, obj>()) TimeSpan.Zero 1.0m 0 false
        }

    /// <summary>
    /// 创建异步工作流处理高并发匹配请求
    /// 支持背压控制和流量限制
    /// </summary>
    let createMatchingWorkflow
        (matchingFunction: MatchingContext -> Async<MatchingResult>)
        (config: MatcherConfiguration)
        =

        let semaphore = new SemaphoreSlim(config.MaxParallelism, config.MaxParallelism)

        fun (context: MatchingContext) ->
            async {
                let! acquired =
                    semaphore.WaitAsync(int config.Timeout.TotalMilliseconds, context.CancellationToken)
                    |> Async.AwaitTask

                if not acquired then
                    return
                        createNoMatchResult
                            "系统繁忙，请稍后重试"
                            (Dictionary<string, obj>())
                            config.Timeout
                            config.Weight
                            config.Priority
                            false
                else
                    try
                        let! result = executeMatchingWithTimeout context matchingFunction config.Timeout

                        return
                            { result with
                                Weight = config.Weight
                                Priority = config.Priority }
                    finally
                        semaphore.Release() |> ignore
            }

    /// <summary>
    /// 聚合多个匹配结果
    /// 使用"与"逻辑合并多个匹配器的结果
    /// </summary>
    let aggregateMatchResults (results: MatchingResult list) =
        let totalWeight = results |> List.sumBy (fun r -> r.Weight)

        let weightedScore =
            if totalWeight > 0.0m then
                results
                |> List.map (fun r -> r.Score * r.Weight)
                |> List.sum
                |> fun sum -> sum / totalWeight
            else
                0.0m

        let allMatched = results |> List.forall (fun r -> r.IsMatch)

        let totalExecutionTime =
            results |> List.map (fun r -> r.ExecutionTime) |> List.fold (+) TimeSpan.Zero

        let aggregatedDetails = Dictionary<string, obj>()

        results
        |> List.iteri (fun i result ->
            result.Details
            |> Seq.iter (fun kvp -> aggregatedDetails.Add($"{i}_{kvp.Key}", kvp.Value)))

        { IsMatch = allMatched
          Score = weightedScore
          Reason = if allMatched then "所有条件匹配成功" else "部分条件匹配失败"
          NotMatchReason = if not allMatched then "存在不匹配的条件" else ""
          Details = aggregatedDetails
          ExecutionTime = totalExecutionTime
          Weight = 1.0m
          Priority = 0
          IsRequired = false }

    /// <summary>
    /// 验证匹配上下文的有效性
    /// </summary>
    let validateMatchingContext (context: MatchingContext) =
        [ if isNull context.TargetingContext then
              "定向上下文不能为空"
          if isNull context.Criteria then
              "定向条件不能为空"
          if isNull context.CallbackProvider then
              "回调提供者不能为空"
          if String.IsNullOrWhiteSpace(context.MatcherId) then
              "匹配器ID不能为空"
          if String.IsNullOrWhiteSpace(context.MatcherType) then
              "匹配器类型不能为空" ]

    /// <summary>
    /// 创建匹配器性能统计信息
    /// </summary>
    let createPerformanceStats (results: MatchingResult list) =
        let executionTimes =
            results |> List.map (fun r -> r.ExecutionTime.TotalMilliseconds)

        let scores = results |> List.map (fun r -> float r.Score)

        let avgExecutionTime =
            if executionTimes.Length > 0 then
                List.average executionTimes
            else
                0.0

        let maxExecutionTime =
            if executionTimes.Length > 0 then
                List.max executionTimes
            else
                0.0

        let minExecutionTime =
            if executionTimes.Length > 0 then
                List.min executionTimes
            else
                0.0

        let avgScore = if scores.Length > 0 then List.average scores else 0.0

        let successRate =
            if results.Length > 0 then
                float (results |> List.filter (fun r -> r.IsMatch) |> List.length)
                / float results.Length
            else
                0.0

        let dict = Dictionary<string, obj>()
        dict.Add("AverageExecutionTimeMs", box avgExecutionTime)
        dict.Add("MaxExecutionTimeMs", box maxExecutionTime)
        dict.Add("MinExecutionTimeMs", box minExecutionTime)
        dict.Add("AverageScore", box avgScore)
        dict.Add("SuccessRate", box successRate)
        dict.Add("TotalRequests", box results.Length)
        dict.Add("SuccessfulMatches", box (results |> List.filter (fun r -> r.IsMatch) |> List.length))
        dict.Add("FailedMatches", box (results |> List.filter (fun r -> not r.IsMatch) |> List.length))
        dict

    /// <summary>
    /// 获取模块版本信息
    /// </summary>
    let getModuleVersion () = "1.0.0"

    /// <summary>
    /// 获取模块功能摘要
    /// </summary>
    let getModuleSummary () =
        "F#通用匹配算法模块 - 提供高性能并行匹配计算、权重评分、快速失败、异步工作流等功能"
