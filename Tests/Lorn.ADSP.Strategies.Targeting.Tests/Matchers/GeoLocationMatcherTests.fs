namespace Lorn.ADSP.Strategies.Targeting.Tests.Matchers

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
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
/// 地理位置定向匹配器测试
/// 测试圆形地理围栏、多边形地理围栏和行政地理定向的匹配逻辑
/// 使用Core.Domain.Targeting中的实际类型进行测试
/// </summary>
module GeoLocationMatcherTestsNew =

    // 测试用数据
    let beijingGeoInfo = GeoInfo(
        countryCode = "CN",
        countryName = "中国",
        provinceCode = "11",
        provinceName = "北京市", 
        cityName = "北京市",
        latitude = 39.9042m,
        longitude = 116.4074m
    )

    let shanghaiGeoInfo = GeoInfo(
        countryCode = "CN",
        countryName = "中国",
        provinceCode = "31",
        provinceName = "上海市",
        cityName = "上海市", 
        latitude = 31.2304m,
        longitude = 121.4737m
    )

    let mockCallbackProvider = 
        { new ICallbackProvider with
            member _.GetCallback<'T when 'T : not struct and 'T :> IAdEngineCallback>() = null
            member _.GetCallback<'T when 'T : not struct and 'T :> IAdEngineCallback>(name: string) = null
            member _.HasCallback<'T when 'T : not struct and 'T :> IAdEngineCallback>() = false
            member _.HasCallback(callbackName: string) = false
            member _.GetAllCallbacks() = Dictionary<string, IAdEngineCallback>() :> IReadOnlyDictionary<string, IAdEngineCallback>
            member _.TryGetCallback<'T when 'T : not struct and 'T :> IAdEngineCallback>(callback: byref<'T>) = 
                callback <- null
                false
            member _.TryGetCallback<'T when 'T : not struct and 'T :> IAdEngineCallback>(name: string, callback: byref<'T>) = 
                callback <- null
                false }

    [<Fact>]
    let ``圆形地理围栏匹配 - 用户在围栏内`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let targeting = CircularGeoFenceTargeting(
                latitude = 39.9042m,
                longitude = 116.4074m,
                radiusMeters = 10000 // 10公里半径
            )
            
            // Act
            let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
            
            // Assert
            Assert.True(result.IsMatch)
            Assert.Equal("geo", result.CriteriaType)
            Assert.True(result.MatchScore > 0.9m)
            Assert.Contains("圆形围栏内", result.MatchReason)
        }

    [<Fact>]
    let ``圆形地理围栏匹配 - 用户在围栏外`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let targeting = CircularGeoFenceTargeting(
                latitude = 39.9042m,
                longitude = 116.4074m,
                radiusMeters = 1000 // 1公里半径
            )
            
            // Act
            let! result = matcher.CalculateMatchScoreAsync(shanghaiGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
            
            // Assert
            Assert.False(result.IsMatch)
            Assert.Equal(0.0m, result.MatchScore)
            Assert.Contains("不在圆形围栏内", result.NotMatchReason)
        }

    [<Fact>]
    let ``多边形地理围栏匹配 - 用户在多边形内`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            
            // 创建一个包含北京的大致正方形
            let points = [|
                GeoPoint.Create(40.2m, 116.0m)    // 西北角
                GeoPoint.Create(40.2m, 116.8m)    // 东北角
                GeoPoint.Create(39.6m, 116.8m)    // 东南角
                GeoPoint.Create(39.6m, 116.0m)    // 西南角
            |]
            
            let targeting = PolygonGeoFenceTargeting(points)
            
            // Act
            let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
            
            // Assert
            Assert.True(result.IsMatch)
            Assert.Equal("GeoLocation", result.MatcherType)
            Assert.True(result.Score > 0.9m)
            Assert.Contains("多边形围栏内", result.Reason)
        }

    [<Fact>]
    let ``多边形地理围栏匹配 - 用户在多边形外`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            
            // 创建一个只包含天安门附近的小正方形
            let points = [|
                GeoPoint.Create(39.910m, 116.400m)
                GeoPoint.Create(39.910m, 116.410m)
                GeoPoint.Create(39.900m, 116.410m)
                GeoPoint.Create(39.900m, 116.400m)
            |]
            
            let targeting = PolygonGeoFenceTargeting(points)
            
            // Act
            let! result = matcher.CalculateMatchScoreAsync(shanghaiGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
            
            // Assert
            Assert.False(result.IsMatch)
            Assert.Equal(0.0m, result.Score)
            Assert.Contains("不在多边形围栏内", result.Reason)
        }

    [<Fact>]
    let ``行政地理定向匹配 - 按国家匹配`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let includedLocation = GeoInfo(countryCode = "CN")
            let targeting = AdministrativeGeoTargeting(
                includedLocations = [includedLocation],
                mode = GeoTargetingMode.Include
            )
            
            // Act
            let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
            
            // Assert
            Assert.True(result.IsMatch)
            Assert.Equal("GeoLocation", result.MatcherType)
            Assert.True(result.Score > 0.8m)
            Assert.Contains("包含的地理区域内", result.Reason)
        }

    [<Fact>]
    let ``行政地理定向匹配 - 按省份匹配`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let includedLocation = GeoInfo(provinceCode = "11") // 北京市
            let targeting = AdministrativeGeoTargeting(
                includedLocations = [includedLocation],
                mode = GeoTargetingMode.Include
            )
            
            // Act
            let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
            
            // Assert
            Assert.True(result.IsMatch)
            Assert.Contains("包含的地理区域内", result.Reason)
        }

    [<Fact>]
    let ``行政地理定向匹配 - 按城市匹配`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let includedLocation = GeoInfo(cityName = "北京市")
            let targeting = AdministrativeGeoTargeting(
                includedLocations = [includedLocation],
                mode = GeoTargetingMode.Include
            )
            
            // Act
            let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
            
            // Assert
            Assert.True(result.IsMatch)
            Assert.Contains("包含的地理区域内", result.Reason)
        }

    [<Fact>]
    let ``行政地理定向匹配 - 排除模式`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let excludedLocation = GeoInfo(cityName = "上海市")
            let targeting = AdministrativeGeoTargeting(
                excludedLocations = [excludedLocation],
                mode = GeoTargetingMode.Exclude
            )
            
            // Act
            let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
            
            // Assert
            Assert.True(result.IsMatch)
            Assert.Contains("不在排除的地理区域内", result.Reason)
        }

    [<Fact>]
    let ``行政地理定向匹配 - 排除模式被排除`` () =
        task {
            // Arrange
            let matcher = GeoLocationMatcher() :> ITargetingMatcher
            let excludedLocation = GeoInfo(cityName = "北京市")
            let targeting = AdministrativeGeoTargeting(
                excludedLocations = [excludedLocation],
                mode = GeoTargetingMode.Exclude
            )
            
            // Act
            let! result = matcher.CalculateMatchScoreAsync(beijingGeoInfo, targeting, mockCallbackProvider, CancellationToken.None)
            
            // Assert
            Assert.False(result.IsMatch)
            Assert.Equal(0.0m, result.Score)
            Assert.Contains("在排除的地理区域内", result.Reason)
        }

    [<Fact>]
    let ``匹配器元数据验证`` () =
        // Arrange
        let matcher = GeoLocationMatcher() :> ITargetingMatcher
        
        // Act
        let metadata = matcher.GetMetadata()
        
        // Assert
        Assert.Equal("GeoLocationMatcher", metadata.MatcherId)
        Assert.Equal("地理位置定向匹配器", metadata.Name)
        Assert.Equal("1.0.0", metadata.Version)
        Assert.Equal("GeoLocation", metadata.MatcherType)
        Assert.True(metadata.SupportsParallelExecution)
        Assert.Equal(15, metadata.ExpectedExecutionTimeMs)

    [<Fact>]
    let ``支持的条件类型验证`` () =
        // Arrange
        let matcher = GeoLocationMatcher() :> ITargetingMatcher
        
        // Act & Assert
        Assert.True(matcher.IsSupported("circulargeoфence"))
        Assert.True(matcher.IsSupported("polygongeofence"))
        Assert.True(matcher.IsSupported("administrativegeo"))
        Assert.True(matcher.IsSupported("geolocation"))
        Assert.True(matcher.IsSupported("geo"))
        Assert.False(matcher.IsSupported("unknown"))

    [<Fact>]
    let ``圆形围栏条件验证 - 有效条件`` () =
        // Arrange
        let matcher = GeoLocationMatcher() :> ITargetingMatcher
        let targeting = CircularGeoFenceTargeting(
            latitude = 39.9042m,
            longitude = 116.4074m,
            radiusMeters = 1000
        )
        
        // Act
        let result = matcher.ValidateCriteria(targeting)
        
        // Assert
        Assert.True(result.IsSuccess)
        Assert.Contains("验证通过", result.Message)

    [<Fact>]
    let ``圆形围栏条件验证 - 无效半径`` () =
        // Arrange
        let matcher = GeoLocationMatcher() :> ITargetingMatcher
        let targeting = CircularGeoFenceTargeting(
            latitude = 39.9042m,
            longitude = 116.4074m,
            radiusMeters = -100
        )
        
        // Act
        let result = matcher.ValidateCriteria(targeting)
        
        // Assert
        Assert.False(result.IsSuccess)
        Assert.Contains("半径必须大于0", result.Errors.[0].Message)

    [<Fact>]
    let ``圆形围栏条件验证 - 无效纬度`` () =
        // Arrange
        let matcher = GeoLocationMatcher() :> ITargetingMatcher
        let targeting = CircularGeoFenceTargeting(
            latitude = 100.0m, // 超出范围
            longitude = 116.4074m,
            radiusMeters = 1000
        )
        
        // Act
        let result = matcher.ValidateCriteria(targeting)
        
        // Assert
        Assert.False(result.IsSuccess)
        Assert.Contains("纬度必须在-90到90之间", result.Errors.[0].Message)

    [<Fact>]
    let ``多边形围栏条件验证 - 顶点不足`` () =
        // Arrange
        let matcher = GeoLocationMatcher() :> ITargetingMatcher
        let points = [|
            GeoPoint.Create(39.9m, 116.4m)
            GeoPoint.Create(39.9m, 116.5m)
        |] // 只有2个点，不足3个
        let targeting = PolygonGeoFenceTargeting(points)
        
        // Act
        let result = matcher.ValidateCriteria(targeting)
        
        // Assert
        Assert.False(result.IsSuccess)
        Assert.Contains("至少需要3个顶点", result.Errors.[0].Message)

    [<Fact>]
    let ``行政地理条件验证 - 空位置列表`` () =
        // Arrange
        let matcher = GeoLocationMatcher() :> ITargetingMatcher
        let targeting = AdministrativeGeoTargeting(
            includedLocations = [],
            excludedLocations = []
        )
        
        // Act
        let result = matcher.ValidateCriteria(targeting)
        
        // Assert
        Assert.False(result.IsSuccess)
        Assert.Contains("不能同时为空", result.Errors.[0].Message)
