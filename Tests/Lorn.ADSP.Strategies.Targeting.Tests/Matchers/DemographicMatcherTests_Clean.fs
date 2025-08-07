namespace Lorn.ADSP.Strategies.Targeting.Tests.Matchers

open System
open System.Threading
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Strategies.Targeting.Matchers

/// <summary>
/// 人口属性定向匹配器单元测试
/// 简化测试，重点测试matcher的基本功能
/// </summary>
module DemographicMatcherTests =

    // ==================== 基础功能测试 ====================

    [<Fact>]
    let ``DemographicTargetingMatcher应该正确实现ITargetingMatcher接口`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher()
        let imatcher = matcher :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Assert
        imatcher.MatcherId |> should equal "demographic-matcher-v1"
        imatcher.MatcherName |> should equal "人口属性定向匹配器"
        imatcher.Version |> should equal "1.0.0"
        imatcher.MatcherType |> should equal "Demographic"
        imatcher.Priority |> should equal 100
        imatcher.IsEnabled |> should equal true
        imatcher.CanRunInParallel |> should equal true

    [<Fact>]
    let ``IsSupported应该正确识别支持的定向条件类型`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Act & Assert
        matcher.IsSupported("Demographic") |> should equal true
        matcher.IsSupported("Demographics") |> should equal true
        matcher.IsSupported("UserDemographic") |> should equal true
        matcher.IsSupported("Geographic") |> should equal false
        matcher.IsSupported("Behavioral") |> should equal false

    [<Fact>]
    let ``GetMetadata应该返回正确的匹配器元数据`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Act
        let metadata = matcher.GetMetadata()

        // Assert
        metadata.MatcherId |> should equal "demographic-matcher-v1"
        metadata.Name |> should equal "人口属性定向匹配器"
        metadata.MatcherType |> should equal "Demographic"
        metadata.SupportedCriteriaTypes |> should contain "Demographic"
        metadata.SupportedDimensions |> should contain "Age"
        metadata.SupportedDimensions |> should contain "Gender"
        metadata.SupportedDimensions |> should contain "Keywords"

    [<Fact>]
    let ``ValidateCriteria应该拒绝空的定向条件`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Act
        let result = matcher.ValidateCriteria(null)

        // Assert
        result.IsValid |> should equal false
        result.Errors.Count |> should be (greaterThan 0)

    [<Fact>]
    let ``CalculateMatchScoreAsync应该正确处理空用户上下文`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Act
        let result = matcher.CalculateMatchScoreAsync(null, null, null, CancellationToken.None).Result

        // Assert
        result.IsMatch |> should equal false
        result.NotMatchReason |> should contain "用户上下文为空"

    [<Fact>]
    let ``CalculateMatchScoreAsync应该正确处理空定向条件`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Act
        let result = matcher.CalculateMatchScoreAsync(null, null, null, CancellationToken.None).Result

        // Assert
        result.IsMatch |> should equal false
        result.NotMatchReason |> should contain "定向条件为空"

    [<Fact>]
    let ``DemographicTargetingMatcher应该能够正确创建实例`` () =
        // Arrange & Act
        let matcher = DemographicTargetingMatcher()

        // Assert
        matcher |> should not' (be null)

    [<Fact>]
    let ``ExpectedExecutionTime应该设置合理的值`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Assert
        matcher.ExpectedExecutionTime |> should equal (TimeSpan.FromMilliseconds(10.0))

    [<Fact>]
    let ``Priority应该设置为100`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Assert
        matcher.Priority |> should equal 100

    [<Fact>]
    let ``CanRunInParallel应该返回true`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Assert
        matcher.CanRunInParallel |> should equal true
