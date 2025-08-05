namespace Lorn.ADSP.Strategies.Targeting.Tests.Core

open System
open System.Collections.Generic
open System.Threading
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Strategies.Targeting.Core.MatchingAlgorithms

/// <summary>
/// F#通用匹配算法模块的单元测试
/// 使用xUnit和FsUnit进行标准化测试
/// </summary>
module MatchingAlgorithmsTests =

    [<Fact>]
    let ``getModuleVersion应该返回正确的版本号`` () =
        // Arrange & Act
        let version = getModuleVersion ()

        // Assert
        version |> should equal "1.0.0"

    [<Fact>]
    let ``getModuleSummary应该返回非空摘要`` () =
        // Arrange & Act
        let summary = getModuleSummary ()

        // Assert
        summary |> should not' (be EmptyString)
        summary.Contains("F#通用匹配算法模块") |> should equal true

    [<Fact>]
    let ``defaultConfiguration应该包含有效的默认值`` () =
        // Arrange & Act
        let config = defaultConfiguration

        // Assert
        config.Timeout |> should be (greaterThan TimeSpan.Zero)
        config.Weight |> should be (greaterThan 0.0m)
        config.MaxParallelism |> should be (greaterThan 0)
        config.CacheEnabled |> should equal true
        config.FastFailThreshold |> should equal 0.1m

    [<Fact>]
    let ``createMatchResult应该创建正确的匹配成功结果`` () =
        // Arrange
        let details = Dictionary<string, obj>()
        details.Add("test", "value")
        let score = 0.8m
        let reason = "测试匹配成功"
        let executionTime = TimeSpan.FromMilliseconds(10.0)

        // Act
        let result = createMatchResult score reason details executionTime 1.0m 0 false

        // Assert
        result.IsMatch |> should equal true
        result.Score |> should equal score
        result.Reason |> should equal reason
        result.NotMatchReason |> should equal ""
        result.Details.Count |> should be (greaterThan 0)
        result.ExecutionTime |> should equal executionTime

    [<Fact>]
    let ``createNoMatchResult应该创建正确的匹配失败结果`` () =
        // Arrange
        let details = Dictionary<string, obj>()
        let notMatchReason = "测试不匹配"
        let executionTime = TimeSpan.FromMilliseconds(5.0)

        // Act
        let result = createNoMatchResult notMatchReason details executionTime 1.0m 0 false

        // Assert
        result.IsMatch |> should equal false
        result.Score |> should equal 0.0m
        result.Reason |> should equal ""
        result.NotMatchReason |> should equal notMatchReason
        result.ExecutionTime |> should equal executionTime

    [<Fact>]
    let ``calculateWeightedScore应该正确计算加权分数`` () =
        // Arrange
        let score = 0.8m
        let weight = 1.5m
        let expected = score * weight

        // Act
        let weightedScore = calculateWeightedScore score weight

        // Assert
        weightedScore |> should equal expected

    [<Fact>]
    let ``normalizeScore应该正确归一化分数`` () =
        // Arrange
        let score = 75.0m
        let minScore = 0.0m
        let maxScore = 100.0m
        let expected = 0.75m

        // Act
        let normalized = normalizeScore score minScore maxScore

        // Assert
        normalized |> should equal expected

    [<Fact>]
    let ``normalizeScore在最大最小值相等时应该返回1`` () =
        // Arrange
        let score = 50.0m
        let minScore = 100.0m
        let maxScore = 100.0m

        // Act
        let normalized = normalizeScore score minScore maxScore

        // Assert
        normalized |> should equal 1.0m

    [<Fact>]
    let ``applyTimeDecay应该应用时间衰减`` () =
        // Arrange
        let score = 1.0m
        let lastUpdateTime = DateTime.UtcNow.AddDays(-1.0) // 1天前
        let decayRate = 0.1m

        // Act
        let decayedScore = applyTimeDecay score lastUpdateTime decayRate

        // Assert
        decayedScore |> should be (lessThan score)
        decayedScore |> should be (greaterThan 0.0m)

    [<Fact>]
    let ``checkFastFail在分数低于阈值时应该返回失败结果`` () =
        // Arrange
        let lowScore = 0.05m
        let threshold = 0.1m
        let reason = "分数过低"

        // Act
        let failResult = checkFastFail lowScore threshold reason

        // Assert
        match failResult with
        | Some result ->
            result.IsMatch |> should equal false
            result.NotMatchReason |> should equal reason
        | None -> failwith "应该返回失败结果"

    [<Fact>]
    let ``checkFastFail在分数高于阈值时应该返回None`` () =
        // Arrange
        let highScore = 0.8m
        let threshold = 0.1m
        let reason = "分数过低"

        // Act
        let failResult = checkFastFail highScore threshold reason

        // Assert
        failResult |> should equal None

    [<Fact>]
    let ``validateMatchingContext应该验证上下文有效性`` () =
        // Arrange
        let invalidContext =
            { TargetingContext = null
              Criteria = null
              CallbackProvider = null
              CancellationToken = CancellationToken.None
              MatcherId = ""
              MatcherType = ""
              StartTime = DateTime.UtcNow }

        // Act
        let errors = validateMatchingContext invalidContext

        // Assert
        errors |> should not' (be Empty)
        errors |> should contain "定向上下文不能为空"
        errors |> should contain "定向条件不能为空"
        errors |> should contain "回调提供者不能为空"

    [<Fact>]
    let ``createPerformanceStats应该创建正确的性能统计`` () =
        // Arrange
        let results =
            [ { IsMatch = true
                Score = 0.8m
                Reason = ""
                NotMatchReason = ""
                Details = Dictionary<string, obj>()
                ExecutionTime = TimeSpan.FromMilliseconds(10.0)
                Weight = 1.0m
                Priority = 0
                IsRequired = false }
              { IsMatch = false
                Score = 0.0m
                Reason = ""
                NotMatchReason = "失败"
                Details = Dictionary<string, obj>()
                ExecutionTime = TimeSpan.FromMilliseconds(5.0)
                Weight = 1.0m
                Priority = 0
                IsRequired = false }
              { IsMatch = true
                Score = 0.9m
                Reason = ""
                NotMatchReason = ""
                Details = Dictionary<string, obj>()
                ExecutionTime = TimeSpan.FromMilliseconds(15.0)
                Weight = 1.0m
                Priority = 0
                IsRequired = false } ]

        // Act
        let stats = createPerformanceStats results

        // Assert
        stats.["TotalRequests"] |> should equal 3
        stats.["SuccessfulMatches"] |> should equal 2
        stats.["FailedMatches"] |> should equal 1
        stats.["AverageExecutionTimeMs"] |> should equal 10.0
        let successRate = stats.["SuccessRate"] :?> float
        successRate |> should (equalWithin 0.01) (2.0 / 3.0)

    [<Fact>]
    let ``aggregateMatchResults应该正确聚合多个匹配结果`` () =
        // Arrange
        let results =
            [ { IsMatch = true
                Score = 0.8m
                Reason = ""
                NotMatchReason = ""
                Details = Dictionary<string, obj>()
                ExecutionTime = TimeSpan.FromMilliseconds(10.0)
                Weight = 1.0m
                Priority = 0
                IsRequired = false }
              { IsMatch = true
                Score = 0.6m
                Reason = ""
                NotMatchReason = ""
                Details = Dictionary<string, obj>()
                ExecutionTime = TimeSpan.FromMilliseconds(5.0)
                Weight = 2.0m
                Priority = 0
                IsRequired = false } ]

        // Act
        let aggregated = aggregateMatchResults results

        // Assert
        aggregated.IsMatch |> should equal true
        let expectedScore = (0.8m * 1.0m + 0.6m * 2.0m) / (1.0m + 2.0m) // = 2.0m/3.0m ≈ 0.667m
        aggregated.Score |> should (equalWithin 0.01m) expectedScore
        aggregated.Reason |> should equal "所有条件匹配成功"
        aggregated.ExecutionTime |> should equal (TimeSpan.FromMilliseconds(15.0))
