open System
open System.Threading
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.Enums
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Strategies.Targeting.Matchers

// 创建测试数据
let beijingGeoInfo =
    GeoInfo(
        countryCode = "CN",
        countryName = "中国",
        provinceCode = "11",
        provinceName = "北京市",
        cityName = "北京市",
        latitude = Nullable(39.9042m),
        longitude = Nullable(116.4074m),
        dataSource = "GPS",
        source = "GPS"
    )

// Mock回调提供器
type TestCallbackProvider() =
    interface ICallbackProvider with
        member _.TryGetCallback<'T>() = (false, Unchecked.defaultof<'T>)

let testCallbackProvider = TestCallbackProvider() :> ICallbackProvider

// 创建匹配器和定向条件
let matcher = GeoLocationMatcher() :> ITargetingMatcher

let targeting =
    CircularGeoFenceTargeting(
        latitude = 39.9042m,
        longitude = 116.4074m,
        radiusMeters = 10000 // 10公里半径
    )

// 执行测试
let result = 
    matcher.CalculateMatchScoreAsync(
        beijingGeoInfo,
        targeting,
        testCallbackProvider,
        CancellationToken.None
    ).Result

// 输出调试信息
printfn "IsMatch: %b" result.IsMatch
printfn "MatchScore: %M" result.MatchScore
printfn "MatchReason: %s" result.MatchReason
printfn "NotMatchReason: %s" result.NotMatchReason
printfn "CriteriaType: %s" result.CriteriaType
printfn "CriteriaId: %A" result.CriteriaId
printfn "ExecutionTime: %A" result.ExecutionTime

// 检查GeoInfo是否实现了ITargetingContext
printfn "GeoInfo type: %s" (beijingGeoInfo.GetType().FullName)
printfn "Implements ITargetingContext: %b" (beijingGeoInfo :? ITargetingContext)

if beijingGeoInfo :? ITargetingContext then
    let context = beijingGeoInfo :?> ITargetingContext
    printfn "Context Type: %s" context.ContextType
    printfn "Context Name: %s" context.ContextName
    printfn "Has Latitude: %b" (context.HasProperty("Latitude"))
    printfn "Has Longitude: %b" (context.HasProperty("Longitude"))