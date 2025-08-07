namespace Lorn.ADSP.Strategies.Targeting.Tests.Matchers

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open Microsoft.FSharp.Control
open Xunit
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Shared.Entities
open Lorn.ADSP.Core.Shared.Enums
open Lorn.ADSP.Strategies.Targeting.Matchers

/// <summary>
/// 地理位置定向匹配器性能测试
/// 测试不同地理匹配算法的性能表现，确保满足实时广告竞价的时间要求
/// </summary>
module GeoLocationMatcherPerformanceTests =

    // 测试用数据
    let beijingGeoInfo = GeoInfo(
        countryCode = "CN",
        provinceCode = "11",
        cityName = "北京市",
        latitude = 39.9042m,
        longitude = 116.4074m
    )

    let mockCallbackProvider = 
        { new ICallbackProvider with
            member _.ExecuteAsync<'T>(callback: ICallback<'T>) = Task.FromResult(Unchecked.defaultof<'T>) }

    let performanceThresholdMs = 15L // 性能阈值：15毫秒以内

    [<Fact>]
    let ``圆形地理围栏性能测试`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let targeting = CircularGeoFenceTargeting(
                latitude = 39.9042m,
                longitude = 116.4074m,
                radiusMeters = 10000
            )
            let iterations = 1000

            // Warmup
            for i in 1 .. 10 do
                let! _ = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
                ()

            // Act
            let stopwatch = Stopwatch.StartNew()
            for i in 1 .. iterations do
                let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
                Assert.True(result.IsMatch)
            stopwatch.Stop()

            // Assert
            let avgTimeMs = stopwatch.ElapsedMilliseconds / int64 iterations
            Assert.True(avgTimeMs < performanceThresholdMs, $"圆形围栏匹配平均时间应少于{performanceThresholdMs}ms，实际: {avgTimeMs}ms")
        }

    [<Fact>]
    let ``多边形地理围栏性能测试`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            
            // 创建复杂多边形（20个顶点）
            let points = 
                [|for i in 0 .. 19 ->
                    let angle = float i * 2.0 * Math.PI / 20.0
                    let lat = 39.9042m + decimal (Math.Sin(angle) * 0.1)
                    let lng = 116.4074m + decimal (Math.Cos(angle) * 0.1)
                    GeoPoint.Create(lat, lng)|]
                    
            let targeting = PolygonGeoFenceTargeting(points)
            let iterations = 1000

            // Warmup
            for i in 1 .. 10 do
                let! _ = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
                ()

            // Act
            let stopwatch = Stopwatch.StartNew()
            for i in 1 .. iterations do
                let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
                Assert.True(result.IsMatch)
            stopwatch.Stop()

            // Assert
            let avgTimeMs = stopwatch.ElapsedMilliseconds / int64 iterations
            Assert.True(avgTimeMs < performanceThresholdMs, $"多边形围栏匹配平均时间应少于{performanceThresholdMs}ms，实际: {avgTimeMs}ms")
        }

    [<Fact>]
    let ``行政地理定向性能测试`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            
            // 创建多个包含位置的行政定向
            let includedLocations = [
                GeoInfo(countryCode = "CN");
                GeoInfo(provinceCode = "11");
                GeoInfo(cityName = "北京市");
                GeoInfo(countryCode = "US");
                GeoInfo(provinceCode = "31")
            ]
            
            let targeting = AdministrativeGeoTargeting(
                includedLocations = includedLocations,
                mode = GeoTargetingMode.Include
            )
            let iterations = 1000

            // Warmup
            for i in 1 .. 10 do
                let! _ = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
                ()

            // Act
            let stopwatch = Stopwatch.StartNew()
            for i in 1 .. iterations do
                let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
                Assert.True(result.IsMatch)
            stopwatch.Stop()

            // Assert
            let avgTimeMs = stopwatch.ElapsedMilliseconds / int64 iterations
            Assert.True(avgTimeMs < performanceThresholdMs, $"行政地理定向匹配平均时间应少于{performanceThresholdMs}ms，实际: {avgTimeMs}ms")
        }

    [<Fact>]
    let ``混合地理条件性能测试`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let iterations = 300 // 降低迭代次数，因为要测试多种条件

            let circularTargeting = CircularGeoFenceTargeting(
                latitude = 39.9042m,
                longitude = 116.4074m,
                radiusMeters = 5000
            )
            
            let polygonPoints = [|
                GeoPoint.Create(40.0m, 116.0m);
                GeoPoint.Create(40.0m, 116.8m);
                GeoPoint.Create(39.8m, 116.8m);
                GeoPoint.Create(39.8m, 116.0m)
            |]
            let polygonTargeting = PolygonGeoFenceTargeting(polygonPoints)
            
            let adminTargeting = AdministrativeGeoTargeting(
                includedLocations = [GeoInfo(cityName = "北京市")],
                mode = GeoTargetingMode.Include
            )

            // Warmup
            for i in 1 .. 5 do
                let! _ = matcher.CalculateMatchScoreAsync(beijingGeoInfo, circularTargeting, mockCallbackProvider, CancellationToken.None)
                let! _ = matcher.CalculateMatchScoreAsync(beijingGeoInfo, polygonTargeting, mockCallbackProvider, CancellationToken.None)
                let! _ = matcher.CalculateMatchScoreAsync(beijingGeoInfo, adminTargeting, mockCallbackProvider, CancellationToken.None)
                ()

            // Act
            let stopwatch = Stopwatch.StartNew()
            for i in 1 .. iterations do
                let! result1 = matcher.CalculateMatchScoreAsync(beijingGeoInfo, circularTargeting, mockCallbackProvider, CancellationToken.None)
                let! result2 = matcher.CalculateMatchScoreAsync(beijingGeoInfo, polygonTargeting, mockCallbackProvider, CancellationToken.None)
                let! result3 = matcher.CalculateMatchScoreAsync(beijingGeoInfo, adminTargeting, mockCallbackProvider, CancellationToken.None)
                
                Assert.True(result1.IsMatch)
                Assert.True(result2.IsMatch)
                Assert.True(result3.IsMatch)
            stopwatch.Stop()

            // Assert
            let avgTimeMs = stopwatch.ElapsedMilliseconds / int64 (iterations * 3)
            Assert.True(avgTimeMs < performanceThresholdMs, $"混合地理条件匹配平均时间应少于{performanceThresholdMs}ms，实际: {avgTimeMs}ms")
        }

    [<Fact>]
    let ``高并发场景性能测试`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let targeting = CircularGeoFenceTargeting(
                latitude = 39.9042m,
                longitude = 116.4074m,
                radiusMeters = 10000
            )
            
            let concurrentTasks = 100
            let iterationsPerTask = 50

            // Act
            let stopwatch = Stopwatch.StartNew()
            
            let tasks = [|
                for i in 1 .. concurrentTasks ->
                    task {
                        for j in 1 .. iterationsPerTask do
                            let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
                            Assert.True(result.IsMatch)
                    }
            |]
            
            do! Task.WhenAll(tasks)
            stopwatch.Stop()

            // Assert
            let totalOperations = concurrentTasks * iterationsPerTask
            let avgTimeMs = stopwatch.ElapsedMilliseconds / int64 totalOperations
            Assert.True(avgTimeMs < performanceThresholdMs, $"高并发场景平均时间应少于{performanceThresholdMs}ms，实际: {avgTimeMs}ms")
        }

    [<Fact>]
    let ``内存使用情况基准测试`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let targeting = CircularGeoFenceTargeting(
                latitude = 39.9042m,
                longitude = 116.4074m,
                radiusMeters = 10000
            )

            // 强制垃圾回收以获得基准内存使用量
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            
            let memoryBefore = GC.GetTotalMemory(false)

            // Act - 执行大量匹配操作
            for i in 1 .. 10000 do
                let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
                Assert.True(result.IsMatch)

            // 再次强制垃圾回收
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            
            let memoryAfter = GC.GetTotalMemory(false)

            // Assert
            let memoryUsedMB = (memoryAfter - memoryBefore) / 1024L / 1024L
            Assert.True(memoryUsedMB < 100L, $"内存使用应少于100MB，实际: {memoryUsedMB}MB")
        }

    [<Fact>]
    let ``极端数据量性能测试`` () =
        task {
            // Arrange - 创建具有大量顶点的多边形（1000个顶点）
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            
            let points = 
                [|for i in 0 .. 999 ->
                    let angle = float i * 2.0 * Math.PI / 1000.0
                    let radius = 0.1 + (float i / 10000.0) // 变化的半径创建复杂形状
                    let lat = 39.9042m + decimal (Math.Sin(angle) * radius)
                    let lng = 116.4074m + decimal (Math.Cos(angle) * radius)
                    GeoPoint.Create(lat, lng)|]
                    
            let targeting = PolygonGeoFenceTargeting(points)
            let iterations = 100 // 减少迭代次数，因为多边形很复杂

            // Act
            let stopwatch = Stopwatch.StartNew()
            for i in 1 .. iterations do
                let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
                Assert.True(result.IsMatch)
            stopwatch.Stop()

            // Assert - 对于极端复杂的多边形，放宽时间限制
            let avgTimeMs = stopwatch.ElapsedMilliseconds / int64 iterations
            Assert.True(avgTimeMs < 50L, $"极端数据量测试平均时间应少于50ms，实际: {avgTimeMs}ms")
        }
