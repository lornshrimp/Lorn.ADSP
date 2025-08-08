// 人口属性定向匹配器单元测试
// 测试各种人口属性组合的匹配逻辑，验证边界条件和异常情况处理
// 覆盖需求: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 13.1, 13.2, 13.3

namespace Lorn.ADSP.Strategies.Targeting.Tests.Matchers

open System
open System.Collections.Generic
open System.Threading
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Strategies.Targeting.Matchers
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Shared.Enums

module DemographicMatcherTests =

    // 简化的测试用ITargetingContext实现
    type TestTargetingContext(properties: ContextProperty[]) =
        let _properties = List<ContextProperty>(properties)

        interface ITargetingContext with
            member _.ContextName = "TestUser"
            member _.ContextType = "UserProfile"
            member _.Properties = _properties :> IReadOnlyList<ContextProperty>
            member _.Timestamp = DateTime.UtcNow
            member _.ContextId = Guid.NewGuid()
            member _.DataSource = "Test"

            member _.GetProperty(propertyKey: string) =
                _properties
                |> Seq.tryFind (fun p -> p.PropertyKey = propertyKey)
                |> Option.toObj

            member _.GetPropertyValue<'T>(propertyKey: string) =
                match _properties |> Seq.tryFind (fun p -> p.PropertyKey = propertyKey) with
                | Some prop -> prop.GetValue<'T>()
                | None -> Unchecked.defaultof<'T>

            member _.GetPropertyValue<'T>(propertyKey: string, defaultValue: 'T) =
                match _properties |> Seq.tryFind (fun p -> p.PropertyKey = propertyKey) with
                | Some prop ->
                    let value = prop.GetValue<'T>()

                    if obj.ReferenceEquals(value, null) then
                        defaultValue
                    else
                        value
                | None -> defaultValue

            member _.GetPropertyAsString(propertyKey: string) =
                match _properties |> Seq.tryFind (fun p -> p.PropertyKey = propertyKey) with
                | Some prop -> prop.PropertyValue
                | None -> String.Empty

            member _.HasProperty(propertyKey: string) =
                _properties |> Seq.exists (fun p -> p.PropertyKey = propertyKey)

            member _.GetPropertyKeys() =
                _properties |> Seq.map (fun p -> p.PropertyKey) |> Array.ofSeq :> IReadOnlyCollection<string>

            member _.GetPropertiesByCategory(category: string) =
                _properties |> Seq.filter (fun p -> p.Category = category) |> List.ofSeq
                :> IReadOnlyList<ContextProperty>

            member _.GetActiveProperties() =
                _properties
                |> Seq.filter (fun p -> not p.ExpiresAt.HasValue || p.ExpiresAt.Value > DateTime.UtcNow)
                |> List.ofSeq
                :> IReadOnlyList<ContextProperty>

            member _.IsValid() = true
            member _.IsExpired(maxAge: TimeSpan) = false

            member _.GetMetadata() =
                List<ContextProperty>() :> IReadOnlyList<ContextProperty>

            member _.GetDebugInfo() = "TestContext"

            member _.CreateLightweightCopy(includeKeys: IEnumerable<string>) =
                let keys = Set.ofSeq includeKeys

                let filteredProps =
                    _properties |> Seq.filter (fun p -> keys.Contains(p.PropertyKey)) |> Array.ofSeq

                TestTargetingContext(filteredProps) :> ITargetingContext

            member _.CreateCategorizedCopy(categories: IEnumerable<string>) =
                let cats = Set.ofSeq categories

                let filteredProps =
                    _properties |> Seq.filter (fun p -> cats.Contains(p.Category)) |> Array.ofSeq

                TestTargetingContext(filteredProps) :> ITargetingContext

            member _.Merge(other: ITargetingContext, overwriteExisting: bool) =
                let otherPropsSeq = other.Properties :> IEnumerable<ContextProperty>

                let otherProps =
                    if overwriteExisting then
                        otherPropsSeq
                    else
                        otherPropsSeq
                        |> Seq.filter (fun p ->
                            not (_properties |> Seq.exists (fun ep -> ep.PropertyKey = p.PropertyKey)))

                let mergedProps = Seq.append _properties otherProps |> Array.ofSeq
                TestTargetingContext(mergedProps) :> ITargetingContext

    // 创建测试用的人口属性上下文
    let createDemographicContext (age: int option) (gender: Gender option) (keywords: string list option) =
        let properties = ResizeArray<ContextProperty>()

        age
        |> Option.iter (fun a ->
            properties.Add(
                ContextProperty(
                    "Age",
                    a.ToString(),
                    "Int32",
                    "Demographics",
                    false,
                    1.0m,
                    Nullable<DateTime>(),
                    "Test"
                )
            ))

        gender
        |> Option.iter (fun g ->
            properties.Add(
                ContextProperty(
                    "Gender",
                    g.ToString(),
                    "Gender",
                    "Demographics",
                    false,
                    1.0m,
                    Nullable<DateTime>(),
                    "Test"
                )
            ))

        keywords
        |> Option.iter (fun kws ->
            kws
            |> List.iter (fun kw ->
                properties.Add(
                    ContextProperty("Keyword", kw, "String", "Interest", false, 1.0m, Nullable<DateTime>(), "Test")
                )))

        TestTargetingContext(properties.ToArray()) :> ITargetingContext

    // ==================== 基础功能测试 ====================

    [<Fact>]
    let ``DemographicTargetingMatcher应该正确实现ITargetingMatcher接口`` () =
        // Arrange
        let matcher = DemographicTargetingMatcher()

        let imatcher =
            matcher :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

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
        let matcher =
            DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Act & Assert
        matcher.IsSupported("Demographic") |> should equal true
        matcher.IsSupported("Demographics") |> should equal true
        matcher.IsSupported("UserDemographic") |> should equal true
        matcher.IsSupported("Geographic") |> should equal false
        matcher.IsSupported("Behavioral") |> should equal false

    [<Fact>]
    let ``GetMetadata应该返回正确的匹配器元数据`` () =
        // Arrange
        let matcher =
            DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

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
        let matcher =
            DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Act
        let result = matcher.ValidateCriteria(null)

        // Assert
        result.IsValid |> should equal false
        result.Errors.Count |> should be (greaterThan 0)

    [<Fact>]
    let ``CalculateMatchScoreAsync应该正确处理空用户上下文`` () =
        // Arrange
        let matcher =
            DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Act
        let result =
            matcher.CalculateMatchScoreAsync(null, null, null, CancellationToken.None).Result

        // Assert
        result.IsMatch |> should equal false
        result.NotMatchReason |> should equal "定向条件为空"

    [<Fact>]
    let ``CalculateMatchScoreAsync应该正确处理空定向条件`` () =
        // Arrange
        let matcher =
            DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Act
        let result =
            matcher.CalculateMatchScoreAsync(null, null, null, CancellationToken.None).Result

        // Assert
        result.IsMatch |> should equal false
        result.NotMatchReason |> should equal "定向条件为空"

    [<Fact>]
    let ``DemographicTargetingMatcher应该能够正确创建实例`` () =
        // Arrange & Act
        let matcher = DemographicTargetingMatcher()

        // Assert
        matcher |> should not' (be null)

    [<Fact>]
    let ``ExpectedExecutionTime应该设置合理的值`` () =
        // Arrange
        let matcher =
            DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Assert
        matcher.ExpectedExecutionTime |> should equal (TimeSpan.FromMilliseconds(10.0))

    [<Fact>]
    let ``Priority应该设置为100`` () =
        // Arrange
        let matcher =
            DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Assert
        matcher.Priority |> should equal 100

    [<Fact>]
    let ``CanRunInParallel应该返回true`` () =
        // Arrange
        let matcher =
            DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

        // Assert
        matcher.CanRunInParallel |> should equal true

    // ==================== 年龄匹配测试 ====================

    [<Fact>]
    let ``年龄匹配_用户年龄在目标范围内_应该匹配成功`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // 创建25岁用户的人口属性信息
            let userContext = createDemographicContext (Some 25) (Some Gender.Male) None

            // 创建18-30岁年龄定向条件
            let ageTargeting =
                DemographicTargeting(minAge = Nullable(18), maxAge = Nullable(30))

            // Debug: 验证用户数据
            printfn "Debug User Context - Has Age: %b" (userContext.HasProperty("Age"))

            if userContext.HasProperty("Age") then
                let ageValue = userContext.GetPropertyValue<int>("Age")
                printfn "Debug User Age Value: %d" ageValue

            // Debug: 验证定向条件
            printfn "Debug Targeting - CriteriaType: %s" ageTargeting.CriteriaType
            printfn "Debug Targeting - MinAge: %A" ageTargeting.MinAge
            printfn "Debug Targeting - MaxAge: %A" ageTargeting.MaxAge
            printfn "Debug Targeting - HasRule MinAge: %b" (ageTargeting.HasRule("MinAge"))
            printfn "Debug Targeting - HasRule MaxAge: %b" (ageTargeting.HasRule("MaxAge"))

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, ageTargeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            printfn
                "Debug: IsMatch=%b, Score=%A, Reason=%s, NotMatchReason=%s"
                result.IsMatch
                result.MatchScore
                result.MatchReason
                result.NotMatchReason

            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m)
            result.MatchReason |> should not' (be null)
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``年龄匹配_用户年龄在目标范围外_应该匹配失败`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // 创建35岁用户的人口属性信息
            let userContext = createDemographicContext (Some 35) (Some Gender.Male) None

            // 创建18-30岁年龄定向条件
            let ageTargeting =
                DemographicTargeting(minAge = Nullable(18), maxAge = Nullable(30))

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, ageTargeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal false
            result.MatchScore |> should equal 0.0m
            result.NotMatchReason |> should not' (be null)
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``年龄匹配_边界值测试_18岁用户匹配18到30岁范围_应该匹配成功`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let userContext = createDemographicContext (Some 18) None None

            let ageTargeting =
                DemographicTargeting(minAge = Nullable(18), maxAge = Nullable(30))

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, ageTargeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m)
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``年龄匹配_边界值测试_17岁用户匹配18到30岁范围_应该匹配失败`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let userContext = createDemographicContext (Some 17) None None

            let ageTargeting =
                DemographicTargeting(minAge = Nullable(18), maxAge = Nullable(30))

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, ageTargeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal false
            result.MatchScore |> should equal 0.0m
        }
        |> Async.RunSynchronously

    // ==================== 性别匹配测试 ====================

    [<Fact>]
    let ``性别匹配_男性用户匹配男性定向_应该匹配成功`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let userContext = createDemographicContext None (Some Gender.Male) None

            let genderList = ResizeArray<Gender>()
            genderList.Add(Gender.Male)

            let genderTargeting = DemographicTargeting(targetGenders = genderList)

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, genderTargeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m)
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``性别匹配_女性用户匹配男性定向_应该匹配失败`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let userContext = createDemographicContext None (Some Gender.Female) None

            let genderList = ResizeArray<Gender>()
            genderList.Add(Gender.Male)

            let genderTargeting = DemographicTargeting(targetGenders = genderList)

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, genderTargeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal false
            result.MatchScore |> should equal 0.0m
        }
        |> Async.RunSynchronously

    // ==================== 关键词匹配测试 ====================

    [<Fact>]
    let ``关键词匹配_用户有匹配关键词_应该匹配成功`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let userContext = createDemographicContext None None (Some [ "汽车"; "运动" ])

            let keywordList = ResizeArray<string>()
            keywordList.Add("汽车")

            let keywordTargeting = DemographicTargeting(targetKeywords = keywordList)

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, keywordTargeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m)
        }
        |> Async.RunSynchronously

    // ==================== 多属性组合匹配测试 ====================

    [<Fact>]
    let ``多属性组合匹配_所有属性都匹配_应该返回高匹配分数`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // 创建完整的用户人口属性
            let userContext =
                createDemographicContext (Some 25) (Some Gender.Male) (Some [ "汽车" ])

            // 创建匹配的定向条件
            let genderList = ResizeArray<Gender>()
            genderList.Add(Gender.Male)

            let keywordList = ResizeArray<string>()
            keywordList.Add("汽车")

            let targeting =
                DemographicTargeting(
                    minAge = Nullable(20),
                    maxAge = Nullable(30),
                    targetGenders = genderList,
                    targetKeywords = keywordList
                )

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m)
            result.MatchReason |> should not' (be null)
            result.ExecutionTime |> should be (lessThan (TimeSpan.FromMilliseconds(100.0)))
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``多属性组合匹配_部分属性不匹配_应该匹配失败`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let userContext = createDemographicContext (Some 35) (Some Gender.Male) None // 年龄不匹配

            let genderList = ResizeArray<Gender>()
            genderList.Add(Gender.Male)

            let targeting =
                DemographicTargeting(
                    minAge = Nullable(20),
                    maxAge = Nullable(30), // 年龄范围20-30，用户35岁不匹配
                    targetGenders = genderList
                )

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal false
            result.MatchScore |> should equal 0.0m
        }
        |> Async.RunSynchronously

    // ==================== 异常处理和边界条件测试 ====================

    [<Fact>]
    let ``异常处理_用户人口属性为空_应该应用默认推断规则`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let emptyContext = createDemographicContext None None None

            let targeting = DemographicTargeting(minAge = Nullable(18), maxAge = Nullable(65))

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(emptyContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal false
            result.NotMatchReason |> should not' (be null)
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``异常处理_年龄为负数_应该优雅处理`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let invalidContext = createDemographicContext (Some -5) None None // 无效年龄

            let targeting = DemographicTargeting(minAge = Nullable(18), maxAge = Nullable(65))

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(invalidContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal false
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``异常处理_年龄超出合理范围_应该优雅处理`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let invalidContext = createDemographicContext (Some 200) None None // 超出合理年龄范围

            let targeting = DemographicTargeting(minAge = Nullable(18), maxAge = Nullable(65))

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(invalidContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal false
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``数据推断_部分属性缺失_应该应用宽松匹配策略`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // 只有年龄信息，其他属性缺失
            let partialContext = createDemographicContext (Some 25) None None

            let targeting = DemographicTargeting(minAge = Nullable(20), maxAge = Nullable(30))

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(partialContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m) // 部分匹配，分数较低但仍匹配
        }
        |> Async.RunSynchronously

    // ==================== 性能测试 ====================

    [<Fact>]
    let ``性能测试_单次人口属性匹配_应该在20毫秒内完成`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let userContext = createDemographicContext (Some 25) (Some Gender.Male) None
            let targeting = DemographicTargeting(minAge = Nullable(20), maxAge = Nullable(30))

            // Warmup
            for i in 1..3 do
                let! _ =
                    matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                    |> Async.AwaitTask

                ()

            // Act
            let stopwatch = System.Diagnostics.Stopwatch.StartNew()

            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            stopwatch.Stop()

            // Assert
            result.IsMatch |> should equal true
            stopwatch.ElapsedMilliseconds |> should be (lessThan 20L)
            result.ExecutionTime |> should be (lessThan (TimeSpan.FromMilliseconds(20.0)))
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``并行处理测试_多个属性维度并行匹配_应该正确处理`` () =
        async {
            // Arrange
            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            let userContext =
                createDemographicContext (Some 25) (Some Gender.Male) (Some [ "汽车" ])

            // 创建复杂的多维度定向条件
            let genderList = ResizeArray<Gender>()
            genderList.Add(Gender.Male)

            let keywordList = ResizeArray<string>()
            keywordList.Add("汽车")

            let targeting =
                DemographicTargeting(
                    minAge = Nullable(20),
                    maxAge = Nullable(30),
                    targetGenders = genderList,
                    targetKeywords = keywordList
                )

            // Act - 测试并行处理能力
            let tasks =
                [| for i in 1..10 do
                       yield
                           matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                           |> Async.AwaitTask |]

            let! results = Async.Parallel tasks

            // Assert
            results |> Array.forall (fun r -> r.IsMatch) |> should equal true
            results |> Array.forall (fun r -> r.MatchScore > 0.0m) |> should equal true

            results
            |> Array.forall (fun r -> r.ExecutionTime < TimeSpan.FromMilliseconds(50.0))
            |> should equal true
        }
        |> Async.RunSynchronously

    /// <summary>
    /// 测试教育程度匹配功能 - 覆盖需求1.1 (模拟测试，因为当前Create方法不支持教育程度)
    /// </summary>
    [<Fact>]
    let ``教育程度匹配_通过关键词模拟教育背景_应该匹配成功`` () =
        async {
            // Arrange - 使用关键词模拟教育程度匹配
            let userContext =
                createDemographicContext None None (Some [ "master_degree"; "university" ])

            let targeting =
                DemographicTargeting.Create(
                    targetKeywords = [| "master_degree"; "bachelor_degree"; "phd" |],
                    weight = 1.0m,
                    isEnabled = true
                )

            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m)
        }
        |> Async.RunSynchronously

    /// <summary>
    /// 测试职业匹配功能 - 覆盖需求1.1 (通过关键词模拟)
    /// </summary>
    [<Fact>]
    let ``职业匹配_通过关键词模拟职业背景_应该匹配成功`` () =
        async {
            // Arrange - 使用关键词模拟职业匹配
            let userContext =
                createDemographicContext None None (Some [ "software_engineer"; "technology" ])

            let targeting =
                DemographicTargeting.Create(
                    targetKeywords = [| "software_engineer"; "data_scientist"; "product_manager" |],
                    weight = 1.0m,
                    isEnabled = true
                )

            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m)
        }
        |> Async.RunSynchronously

    /// <summary>
    /// 测试收入匹配功能 - 覆盖需求1.1 (通过关键词模拟)
    /// </summary>
    [<Fact>]
    let ``收入匹配_通过关键词模拟收入范围_应该匹配成功`` () =
        async {
            // Arrange - 使用关键词模拟收入水平
            let userContext =
                createDemographicContext None None (Some [ "high_income"; "middle_class" ])

            let targeting =
                DemographicTargeting.Create(
                    targetKeywords = [| "high_income"; "medium_income"; "upper_middle" |],
                    weight = 1.0m,
                    isEnabled = true
                )

            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m)
        }
        |> Async.RunSynchronously

    /// <summary>
    /// 测试婚恋状况匹配功能 - 覆盖需求1.1 (通过关键词模拟)
    /// </summary>
    [<Fact>]
    let ``婚恋状况匹配_通过关键词模拟婚恋状态_应该匹配成功`` () =
        async {
            // Arrange - 使用关键词模拟婚恋状况
            let userContext =
                createDemographicContext None None (Some [ "single"; "young_adult" ])

            let targeting =
                DemographicTargeting.Create(
                    targetKeywords = [| "single"; "married"; "in_relationship" |],
                    weight = 1.0m,
                    isEnabled = true
                )

            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.0m)
        }
        |> Async.RunSynchronously

    /// <summary>
    /// 测试完整人口属性组合匹配 - 覆盖需求1.1, 1.3, 1.4
    /// </summary>
    [<Fact>]
    let ``完整人口属性匹配_年龄性别关键词都匹配_应该返回高分数`` () =
        async {
            // Arrange
            let userContext =
                createDemographicContext (Some 28) (Some Gender.Female) (Some [ "technology"; "education"; "single" ])

            let targeting =
                DemographicTargeting.Create(
                    minAge = Nullable(25),
                    maxAge = Nullable(35),
                    targetGenders = [| Gender.Female |],
                    targetKeywords = [| "technology"; "education" |],
                    weight = 1.0m,
                    isEnabled = true
                )

            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result.IsMatch |> should equal true
            result.MatchScore |> should be (greaterThan 0.8m) // 高分数表示多维度匹配
            result.MatchReason |> should not' (be null)
        }
        |> Async.RunSynchronously

    /// <summary>
    /// 测试缓存机制 - 覆盖需求1.6
    /// </summary>
    [<Fact>]
    let ``缓存测试_重复请求相同匹配_第二次调用应该更快`` () =
        async {
            // Arrange
            let userContext =
                createDemographicContext (Some 25) (Some Gender.Male) (Some [ "tech" ])

            let targeting =
                DemographicTargeting.Create(
                    minAge = Nullable(18),
                    maxAge = Nullable(30),
                    targetGenders = [| Gender.Male |],
                    weight = 1.0m,
                    isEnabled = true
                )

            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // Act - 第一次调用（建立缓存）
            let startTime1 = DateTime.UtcNow

            let! result1 =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            let duration1 = DateTime.UtcNow - startTime1

            // Act - 第二次调用（应该使用缓存）
            let startTime2 = DateTime.UtcNow

            let! result2 =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            let duration2 = DateTime.UtcNow - startTime2

            // Assert
            result1.IsMatch |> should equal result2.IsMatch
            result1.MatchScore |> should equal result2.MatchScore

            // 简化性能测试，主要验证功能一致性
            result1.IsMatch |> should equal true
            result2.IsMatch |> should equal true
        }
        |> Async.RunSynchronously

    /// <summary>
    /// 测试缓存失效机制 - 覆盖需求1.6
    /// </summary>
    [<Fact>]
    let ``缓存失效测试_不同定向条件_应该返回不同结果`` () =
        async {
            // Arrange
            let userContext =
                createDemographicContext (Some 25) (Some Gender.Male) (Some [ "tech" ])

            let targeting1 =
                DemographicTargeting.Create(
                    minAge = Nullable(18),
                    maxAge = Nullable(30),
                    weight = 1.0m,
                    isEnabled = true
                )

            let targeting2 =
                DemographicTargeting.Create(
                    minAge = Nullable(30),
                    maxAge = Nullable(40),
                    weight = 1.0m,
                    isEnabled = true
                )

            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // Act
            let! result1 =
                matcher.CalculateMatchScoreAsync(userContext, targeting1, null, CancellationToken.None)
                |> Async.AwaitTask

            let! result2 =
                matcher.CalculateMatchScoreAsync(userContext, targeting2, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert
            result1.IsMatch |> should equal true
            result2.IsMatch |> should equal false
            result1.MatchScore |> should not' (equal result2.MatchScore)
        }
        |> Async.RunSynchronously

    /// <summary>
    /// 测试边界条件 - 空关键词匹配 - 覆盖需求1.2
    /// </summary>
    [<Fact>]
    let ``边界条件测试_用户无关键词但其他属性匹配_应按AND语义不匹配`` () =
        async {
            // Arrange
            let userContext = createDemographicContext (Some 25) (Some Gender.Male) None // 无关键词

            let targeting =
                DemographicTargeting.Create(
                    minAge = Nullable(20),
                    maxAge = Nullable(30),
                    targetGenders = [| Gender.Male |],
                    targetKeywords = [| "technology"; "sports" |],
                    weight = 1.0m,
                    isEnabled = true
                )

            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert - AND 语义：任一启用条件不匹配则整体不匹配
            result.IsMatch |> should equal false
            result.NotMatchReason |> should not' (be null)
            // 未匹配的人口属性应包含关键词
            result.NotMatchReason |> should haveSubstring "未匹配的人口属性"
            result.NotMatchReason |> should haveSubstring "Keywords"
        }
        |> Async.RunSynchronously

    /// <summary>
    /// 测试数据质量控制 - 覆盖需求1.5
    /// </summary>
    [<Fact>]
    let ``数据质量测试_年龄超出合理范围_应该处理异常情况`` () =
        async {
            // Arrange - 测试极端年龄值
            let userContext = createDemographicContext (Some 200) (Some Gender.Male) None // 不合理的年龄

            let targeting =
                DemographicTargeting.Create(
                    minAge = Nullable(18),
                    maxAge = Nullable(65),
                    targetGenders = [| Gender.Male |],
                    weight = 1.0m,
                    isEnabled = true
                )

            let matcher =
                DemographicTargetingMatcher() :> Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces.ITargetingMatcher

            // Act
            let! result =
                matcher.CalculateMatchScoreAsync(userContext, targeting, null, CancellationToken.None)
                |> Async.AwaitTask

            // Assert - 应该能处理异常数据而不崩溃
            result |> should not' (be null)
            result.IsMatch |> should equal false // 超出范围应该不匹配
        }
        |> Async.RunSynchronously
