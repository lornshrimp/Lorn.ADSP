module Lorn.ADSP.Strategies.Targeting.Tests.Matchers.UserValueMatcherTests

open System
open System.Collections.Generic
open System.Threading
open Xunit
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Strategies.Targeting.Matchers

[<Fact>]
let ``UserValueMatcher should have correct identifier`` () =
    // Arrange
    let matcher = UserValueTargetingMatcher()
    let targetingMatcher = matcher :> ITargetingMatcher

    // Act & Assert
    Assert.Equal("user-value-matcher-v1", targetingMatcher.MatcherId)

[<Fact>]
let ``UserValueMatcher should have correct name`` () =
    // Arrange
    let matcher = UserValueTargetingMatcher()
    let targetingMatcher = matcher :> ITargetingMatcher

    // Act & Assert
    Assert.Equal("用户价值定向匹配器", targetingMatcher.MatcherName)

[<Fact>]
let ``UserValueMatcher should support criteria type`` () =
    // Arrange
    let matcher = UserValueTargetingMatcher()
    let targetingMatcher = matcher :> ITargetingMatcher

    // Act
    let isSupported = targetingMatcher.IsSupported("UserValue")

    // Assert
    Assert.True(isSupported)

[<Fact>]
let ``UserValueMatcher should validate valid criteria`` () =
    // Arrange
    let matcher = UserValueTargetingMatcher()
    let targetingMatcher = matcher :> ITargetingMatcher
    let userValueTargeting = UserValueTargeting()

    // 添加一个有效的定向条件
    userValueTargeting.AddTargetValueTier(ValueTier.Premium)

    // 调试：检查规则是否已设置
    let hasTargetValueTiers = userValueTargeting.HasRule("TargetValueTiers")
    printfn "Debug: HasRule TargetValueTiers = %b" hasTargetValueTiers
    let criteriaType = userValueTargeting.CriteriaType
    printfn "Debug: CriteriaType = %s" criteriaType

    // Act
    let result = targetingMatcher.ValidateCriteria(userValueTargeting)

    // Assert
    printfn "Debug: Validation result IsValid = %b" result.IsValid

    if not result.IsValid then
        printfn "Debug: Validation errors = %A" result.Errors

    Assert.True(result.IsValid)

[<Fact>]
let ``UserValueMatcher should return metadata`` () =
    // Arrange
    let matcher = UserValueTargetingMatcher()
    let targetingMatcher = matcher :> ITargetingMatcher

    // Act
    let metadata = targetingMatcher.GetMetadata()

    // Assert
    Assert.NotNull(metadata)
    Assert.NotNull(metadata.MatcherId)
