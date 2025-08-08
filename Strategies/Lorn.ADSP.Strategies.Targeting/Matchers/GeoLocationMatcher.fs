namespace Lorn.ADSP.Strategies.Targeting.Matchers

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open FSharp.Control
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Domain.Enums
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Shared.Entities
open Lorn.ADSP.Core.Shared.Enums

/// <summary>
/// 地理位置定向匹配器
/// 实现ITargetingMatcher接口，支持圆形地理围栏、多边形地理围栏和行政地理定向
/// 使用Core.Domain.Targeting中已定义的地理定向类型，避免重复造轮子
///
/// 优化特性：
/// - 高精度Haversine公式球面距离计算
/// - 优化的射线法多边形包含判断
/// - 空间索引和缓存优化
/// - IP地址定位降级机制
/// - 基于移动性的智能缓存策略
/// </summary>
type GeoLocationMatcher() =

    // 地球半径常量（米）
    static let EarthRadiusMeters = 6371000.0

    // 缓存匹配结果
    static let MatchCache =
        ConcurrentDictionary<string, (bool * string * decimal * DateTime)>()

    // 预计算的地理边界缓存
    static let BoundaryCache =
        ConcurrentDictionary<string, (decimal * decimal * decimal * decimal)>()

    /// <summary>
    /// 高精度Haversine公式计算球面距离（米）
    /// 使用更精确的算法和数值稳定性优化
    /// </summary>
    let calculateHaversineDistance (lat1: decimal) (lng1: decimal) (lat2: decimal) (lng2: decimal) : double =
        // 转换为弧度
        let toRadians (degrees: double) = degrees * Math.PI / 180.0

        let lat1Rad = toRadians (double lat1)
        let lng1Rad = toRadians (double lng1)
        let lat2Rad = toRadians (double lat2)
        let lng2Rad = toRadians (double lng2)

        // 计算差值
        let dLat = lat2Rad - lat1Rad
        let dLng = lng2Rad - lng1Rad

        // Haversine公式，使用数值稳定的实现
        let havesineLat = Math.Sin(dLat / 2.0)
        let havesineLng = Math.Sin(dLng / 2.0)

        let a =
            havesineLat * havesineLat
            + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * havesineLng * havesineLng

        // 使用atan2提高数值精度
        let c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a))

        EarthRadiusMeters * c

    /// <summary>
    /// 点是否在圆形区域内
    /// 使用高精度距离计算
    /// </summary>
    let isPointInCircle
        (userLat: decimal)
        (userLng: decimal)
        (centerLat: decimal)
        (centerLng: decimal)
        (radiusMeters: int)
        : bool =
        let distance = calculateHaversineDistance userLat userLng centerLat centerLng
        distance <= float radiusMeters

    /// <summary>
    /// 优化的射线法：点是否在多边形区域内
    /// 使用改进的射线算法，处理边界情况和数值精度问题
    /// </summary>
    let isPointInPolygon (userLat: decimal) (userLng: decimal) (points: IReadOnlyList<GeoPoint>) : bool =
        if points.Count < 3 then
            false
        else
            let x = double userLng
            let y = double userLat

            // 预处理边界框检查
            let (minLat, maxLat, minLng, maxLng) =
                points
                |> Seq.fold
                    (fun (minLat, maxLat, minLng, maxLng) point ->
                        let lat = double point.Latitude
                        let lng = double point.Longitude
                        (min minLat lat, max maxLat lat, min minLng lng, max maxLng lng))
                    (Double.MaxValue, Double.MinValue, Double.MaxValue, Double.MinValue)

            // 快速边界框排除
            if x < minLng || x > maxLng || y < minLat || y > maxLat then
                false
            else
                // 优化的射线法计算
                let rec checkIntersections (i: int) (intersections: int) : bool =
                    if i >= points.Count then
                        intersections % 2 = 1
                    else
                        let j = (i + 1) % points.Count
                        let xi = double points.[i].Longitude
                        let yi = double points.[i].Latitude
                        let xj = double points.[j].Longitude
                        let yj = double points.[j].Latitude

                        // 优化的交点计算，处理数值精度和边界情况
                        let newIntersections =
                            if ((yi > y) <> (yj > y)) then
                                // 计算交点的x坐标
                                let xIntersect = (xj - xi) * (y - yi) / (yj - yi) + xi
                                if x < xIntersect then intersections + 1 else intersections
                            else
                                intersections

                        checkIntersections (i + 1) newIntersections

                checkIntersections 0 0

    /// <summary>
    /// 空间预处理：计算多边形的边界框
    /// 用于快速排除和空间索引优化
    /// </summary>
    let getPolygonBounds (points: IReadOnlyList<GeoPoint>) : (decimal * decimal * decimal * decimal) =
        if points.Count = 0 then
            (0m, 0m, 0m, 0m)
        else
            let mutable minLat = points.[0].Latitude
            let mutable maxLat = points.[0].Latitude
            let mutable minLng = points.[0].Longitude
            let mutable maxLng = points.[0].Longitude

            for i = 1 to points.Count - 1 do
                let point = points.[i]
                minLat <- min minLat point.Latitude
                maxLat <- max maxLat point.Latitude
                minLng <- min minLng point.Longitude
                maxLng <- max maxLng point.Longitude

            (minLat, maxLat, minLng, maxLng)

    /// <summary>
    /// 智能缓存键生成
    /// 基于地理位置精度和定向条件生成缓存键
    /// </summary>
    let generateCacheKey (geoInfo: GeoInfo) (criteriaId: string) (criteriaType: string) : string =
        let latKey =
            if geoInfo.Latitude.HasValue then
                Math.Round(double geoInfo.Latitude.Value, 4).ToString("F4")
            else
                "null"

        let lngKey =
            if geoInfo.Longitude.HasValue then
                Math.Round(double geoInfo.Longitude.Value, 4).ToString("F4")
            else
                "null"

        let countryKey =
            if isNull geoInfo.CountryCode then
                "null"
            else
                geoInfo.CountryCode

        let provinceKey =
            if isNull geoInfo.ProvinceCode then
                "null"
            else
                geoInfo.ProvinceCode

        let cityKey = if isNull geoInfo.CityName then "null" else geoInfo.CityName
        let adminKey = $"{countryKey}_{provinceKey}_{cityKey}"

        $"{criteriaType}_{criteriaId}_{latKey}_{lngKey}_{adminKey}"

    /// <summary>
    /// 基于移动性的多级缓存过期策略 - 增强版
    /// 根据用户移动性设置不同的缓存过期时间
    /// 支持需求 2.6: WHEN 地理位置匹配完成 THEN 系统 SHALL 根据用户移动性设置不同的缓存过期时间
    /// 支持需求 7.3: WHEN 执行匹配计算 THEN 系统 SHALL 使用多级缓存策略减少重复计算
    /// </summary>
    let getCacheExpirationTime (geoInfo: GeoInfo) : TimeSpan =
        // 基于数据源的基础缓存时间
        let baseExpiration =
            match geoInfo.Source with
            | "GPS" -> TimeSpan.FromMinutes(5.0) // GPS精确位置，较短缓存（用户可能快速移动）
            | "WIFI" -> TimeSpan.FromMinutes(15.0) // WIFI位置，中等缓存（相对固定位置）
            | "CELL" -> TimeSpan.FromMinutes(30.0) // 基站位置，较长缓存（区域性定位）
            | "IP" -> TimeSpan.FromHours(2.0) // IP位置，最长缓存（变化较慢）
            | _ -> TimeSpan.FromMinutes(10.0) // 默认缓存时间

        // 基于数据源可靠性的缓存时间调整
        let reliabilityMultiplier =
            match geoInfo.DataSource with
            | "GPS" -> 0.8 // GPS数据精确但变化快，缩短缓存
            | "WIFI" -> 1.2 // WIFI相对稳定，延长缓存
            | "CELL" -> 1.5 // 基站覆盖范围大，延长缓存
            | "IP_Service" -> 2.0 // 外部IP服务调用成本高，延长缓存
            | "IP_Cache" -> 1.8 // 已缓存的IP数据，适度延长
            | "IP_Resolved" -> 1.5 // 解析的IP数据，中等延长
            | "Manual" -> 5.0 // 手动输入的位置，大幅延长缓存
            | _ -> 1.0 // 默认不调整

        // 应用可靠性乘数
        let adjustedExpiration =
            TimeSpan.FromTicks(int64 (float baseExpiration.Ticks * reliabilityMultiplier))

        // 设置最小和最大缓存时间边界
        let minCacheTime = TimeSpan.FromMinutes(1.0)
        let maxCacheTime = TimeSpan.FromHours(24.0)

        if adjustedExpiration < minCacheTime then minCacheTime
        elif adjustedExpiration > maxCacheTime then maxCacheTime
        else adjustedExpiration

    /// <summary>
    /// 多级缓存策略管理 - 新增
    /// 实现内存缓存、分布式缓存和数据库缓存的分层策略
    /// 支持需求 7.3: 多级缓存策略减少重复计算
    /// 支持需求 13.2: 缓存命中率优化
    /// </summary>
    let getMultiLevelCachedResult (cacheKey: string) : (bool * string * decimal) option =
        // Level 1: 内存缓存（最快）
        match MatchCache.TryGetValue(cacheKey) with
        | true, (isMatch, reason, score, cacheTime) ->
            if DateTime.UtcNow - cacheTime < TimeSpan.FromMinutes(30.0) then
                Some(isMatch, reason, score)
            else
                MatchCache.TryRemove(cacheKey) |> ignore
                None
        | false, _ ->
            // Level 2: 分布式缓存（Redis等，通过回调提供器访问）
            // 这里暂时返回None，实际实现中会通过callbackProvider查询分布式缓存
            // Level 3: 数据库缓存（最慢但持久）
            // 实际实现中会查询数据库中的缓存表
            None

    /// <summary>
    /// 智能缓存写入策略 - 增强版
    /// 根据数据重要性和访问频率决定缓存层级
    /// </summary>
    let cacheMatchResultMultiLevel
        (cacheKey: string)
        (isMatch: bool)
        (reason: string)
        (score: decimal)
        (expirationTime: TimeSpan)
        (geoInfo: GeoInfo)
        : unit =
        let cacheTime = DateTime.UtcNow

        // Level 1: 内存缓存（所有结果都缓存）
        MatchCache.[cacheKey] <- (isMatch, reason, score, cacheTime)

        // 根据重要性决定是否写入更高级别缓存
        let shouldCacheDistributed =
            isMatch && score > 0.8m
            || // 高分匹配结果
            geoInfo.Source = "IP"
            || // IP定位结果（成本高）
            expirationTime > TimeSpan.FromHours(1.0) // 长期有效的结果

        // Level 2 & 3: 分布式缓存和数据库缓存
        // 实际实现中会通过callbackProvider写入分布式缓存和数据库

        // 异步清理过期缓存项
        Task.Run(fun () ->
            Thread.Sleep(expirationTime)
            let (success, (_, _, _, cachedTime)) = MatchCache.TryGetValue(cacheKey)

            if success && DateTime.UtcNow - cachedTime >= expirationTime then
                MatchCache.TryRemove(cacheKey) |> ignore)
        |> ignore

    /// <summary>
    /// 地理位置匹配超时和降级处理 - 新增
    /// 支持需求 2.5: WHEN 地理位置匹配计算超时 THEN 系统 SHALL 降级为行政区划匹配或返回默认结果
    /// </summary>
    let handleTimeoutAndFallback
        (geoInfo: GeoInfo)
        (criteria: ITargetingCriteria)
        (startTime: DateTime)
        (timeoutThreshold: TimeSpan)
        : (bool * string * decimal) option =

        let elapsed = DateTime.UtcNow - startTime

        if elapsed > timeoutThreshold then
            // 超时处理：降级为行政区划匹配
            match criteria with
            | :? CircularGeoFenceTargeting ->
                // 圆形围栏超时：降级为城市级匹配
                if not (isNull geoInfo.CityName) then
                    Some(true, $"圆形围栏匹配超时，降级为城市匹配: {geoInfo.CityName}", 0.6m)
                elif not (isNull geoInfo.ProvinceCode) then
                    Some(true, $"圆形围栏匹配超时，降级为省份匹配: {geoInfo.ProvinceCode}", 0.5m)
                elif not (isNull geoInfo.CountryCode) then
                    Some(true, $"圆形围栏匹配超时，降级为国家匹配: {geoInfo.CountryCode}", 0.4m)
                else
                    Some(false, "圆形围栏匹配超时且无可用降级数据", 0.0m)

            | :? PolygonGeoFenceTargeting ->
                // 多边形围栏超时：降级为行政区划匹配
                if not (isNull geoInfo.CityName) then
                    Some(true, $"多边形围栏匹配超时，降级为城市匹配: {geoInfo.CityName}", 0.6m)
                elif not (isNull geoInfo.ProvinceCode) then
                    Some(true, $"多边形围栏匹配超时，降级为省份匹配: {geoInfo.ProvinceCode}", 0.5m)
                else
                    Some(false, "多边形围栏匹配超时且无可用降级数据", 0.0m)

            | :? AdministrativeGeoTargeting ->
                // 行政区划匹配超时：返回默认保守结果
                Some(true, "行政区划匹配超时，返回保守匹配结果", 0.3m)

            | _ -> Some(false, "未知类型地理匹配超时", 0.0m)
        else
            None // 未超时，继续正常处理

    /// <summary>
    /// 精确度自适应匹配策略 - 新增
    /// 根据位置精度自动选择匹配级别
    /// 支持需求 2.2: WHEN 用户有精确坐标信息 THEN 系统 SHALL 执行精确坐标匹配流程
    /// 支持需求 2.3: WHEN 用户无精确坐标 THEN 系统 SHALL 根据位置精度执行相应级别匹配
    /// </summary>
    let selectMatchingStrategy (geoInfo: GeoInfo) (criteria: ITargetingCriteria) : string =
        let hasAccurateCoordinates =
            geoInfo.Latitude.HasValue
            && geoInfo.Longitude.HasValue
            && (geoInfo.Source = "GPS" || geoInfo.Source = "WIFI")

        match criteria with
        | :? CircularGeoFenceTargeting
        | :? PolygonGeoFenceTargeting when hasAccurateCoordinates -> "PreciseCoordinate" // 精确坐标匹配

        | :? CircularGeoFenceTargeting
        | :? PolygonGeoFenceTargeting when not hasAccurateCoordinates -> "ApproximateCoordinate" // 近似坐标匹配

        | :? AdministrativeGeoTargeting when not (isNull geoInfo.CityName) -> "CityLevel" // 城市级别匹配

        | :? AdministrativeGeoTargeting when not (isNull geoInfo.ProvinceCode) -> "ProvinceLevel" // 省份级别匹配

        | :? AdministrativeGeoTargeting when not (isNull geoInfo.CountryCode) -> "CountryLevel" // 国家级别匹配

        | _ -> "DefaultLevel" // 默认级别匹配

    /// <summary>
    /// 尝试从缓存获取匹配结果
    /// </summary>
    let tryGetCachedResult (cacheKey: string) : (bool * string * decimal) option =
        match MatchCache.TryGetValue(cacheKey) with
        | true, (isMatch, reason, score, cacheTime) ->
            if DateTime.UtcNow - cacheTime < TimeSpan.FromMinutes(30.0) then
                Some(isMatch, reason, score)
            else
                MatchCache.TryRemove(cacheKey) |> ignore
                None
        | false, _ -> None

    /// <summary>
    /// 缓存匹配结果
    /// </summary>
    let cacheMatchResult
        (cacheKey: string)
        (isMatch: bool)
        (reason: string)
        (score: decimal)
        (expirationTime: TimeSpan)
        : unit =
        let cacheTime = DateTime.UtcNow
        MatchCache.[cacheKey] <- (isMatch, reason, score, cacheTime)

        // 异步清理过期缓存项
        Task.Run(fun () ->
            Thread.Sleep(expirationTime)
            let (success, (_, _, _, cachedTime)) = MatchCache.TryGetValue(cacheKey)

            if success && DateTime.UtcNow - cachedTime >= expirationTime then
                MatchCache.TryRemove(cacheKey) |> ignore)
        |> ignore

    /// <summary>
    /// IP地址定位降级机制 - 增强版
    /// 当精确坐标不可用时，通过多层次IP地址地理位置推断
    /// 支持需求 2.4: WHEN 无任何位置信息 THEN 系统 SHALL 尝试通过IP地址进行定位匹配
    /// </summary>
    let tryIPLocationFallback (context: ITargetingContext) (callbackProvider: ICallbackProvider) : GeoInfo option =
        try
            // 尝试从上下文获取IP地址
            let ipAddress = context.GetPropertyValue<string>("IPAddress")

            if not (String.IsNullOrEmpty(ipAddress)) then
                // 多层次IP定位策略
                let getIPLocationFromCache (ip: string) =
                    let ipCacheKey = $"ip_location_{ip}"

                    match MatchCache.TryGetValue(ipCacheKey) with
                    | true, (_, location, _, cacheTime) when DateTime.UtcNow - cacheTime < TimeSpan.FromHours(6.0) ->
                        Some location
                    | _ -> None

                // 尝试从缓存获取IP位置信息
                match getIPLocationFromCache ipAddress with
                | Some cachedLocation ->
                    // 从缓存位置字符串解析地理信息
                    let parts = cachedLocation.Split('|')

                    if parts.Length >= 3 then
                        Some(
                            GeoInfo(
                                countryCode = parts.[0],
                                provinceCode =
                                    (if parts.Length > 1 && parts.[1] <> "null" then
                                         parts.[1]
                                     else
                                         null),
                                cityName =
                                    (if parts.Length > 2 && parts.[2] <> "null" then
                                         parts.[2]
                                     else
                                         null),
                                dataSource = "IP_Cache",
                                source = "IP"
                            )
                        )
                    else
                        None
                | None ->
                    // 从上下文获取已解析的IP地理位置信息
                    let countryCode = context.GetPropertyValue<string>("IPCountryCode")
                    let provinceCode = context.GetPropertyValue<string>("IPProvinceCode")
                    let cityName = context.GetPropertyValue<string>("IPCityName")
                    let ipLatitude = context.GetPropertyValue<Nullable<decimal>>("IPLatitude")
                    let ipLongitude = context.GetPropertyValue<Nullable<decimal>>("IPLongitude")

                    // 如果有基础IP位置信息，创建GeoInfo
                    if not (String.IsNullOrEmpty(countryCode)) then
                        // 缓存IP位置信息
                        let locationString = $"{countryCode}|{provinceCode}|{cityName}"
                        let ipCacheKey = $"ip_location_{ipAddress}"
                        cacheMatchResult ipCacheKey true locationString 0.7m (TimeSpan.FromHours(6.0))

                        Some(
                            GeoInfo(
                                countryCode = countryCode,
                                provinceCode = provinceCode,
                                cityName = cityName,
                                latitude = ipLatitude,
                                longitude = ipLongitude,
                                dataSource = "IP_Resolved",
                                source = "IP"
                            )
                        )
                    else
                        // 尝试通过回调提供器调用外部IP地理位置服务
                        try
                            // 尝试获取上下文回调
                            match callbackProvider.TryGetCallback<IAdEngineContextCallback>() with
                            | true, contextCallback ->
                                // 构造IP查询参数
                                let ipQueryParams =
                                    let paramDict = Dictionary<string, obj>()
                                    paramDict.["ip"] <- ipAddress :> obj
                                    paramDict.["service"] <- "ip_location" :> obj
                                    paramDict :> IReadOnlyDictionary<string, obj>

                                // 异步查询IP位置信息
                                // 注意：这里使用同步方式处理异步调用，在生产环境中应该重构为完全异步
                                let task =
                                    contextCallback.GetContextAsync<GeoInfo>(
                                        "IPLocation",
                                        ipQueryParams,
                                        CancellationToken.None
                                    )

                                if task.Wait(TimeSpan.FromSeconds(2.0)) then // 2秒超时
                                    let resolvedGeoInfo = task.Result

                                    if
                                        not (isNull resolvedGeoInfo)
                                        && not (String.IsNullOrEmpty(resolvedGeoInfo.CountryCode))
                                    then
                                        // 缓存解析结果
                                        let locationString =
                                            $"{resolvedGeoInfo.CountryCode}|{resolvedGeoInfo.ProvinceCode}|{resolvedGeoInfo.CityName}"

                                        let ipCacheKey = $"ip_location_{ipAddress}"
                                        cacheMatchResult ipCacheKey true locationString 0.7m (TimeSpan.FromHours(6.0))

                                        Some(
                                            GeoInfo(
                                                countryCode = resolvedGeoInfo.CountryCode,
                                                provinceCode = resolvedGeoInfo.ProvinceCode,
                                                cityName = resolvedGeoInfo.CityName,
                                                latitude = resolvedGeoInfo.Latitude,
                                                longitude = resolvedGeoInfo.Longitude,
                                                dataSource = "IP_Service",
                                                source = "IP"
                                            )
                                        )
                                    else
                                        None
                                else
                                    None
                            | false, _ -> None
                        with _ ->
                            None
            else
                None
        with ex ->
            // 记录IP定位失败，但不抛出异常
            None

    /// <summary>
    /// 处理Nullable类型转换为Option
    /// </summary>
    let nullableToOption (nullable: Nullable<'T>) : 'T option =
        if nullable.HasValue then Some nullable.Value else None

    /// <summary>
    /// 确保criteriaId有效，如果为空则生成新的GUID
    /// </summary>
    let ensureValidCriteriaId (criteria: ITargetingCriteria) : Guid =
        if criteria.CriteriaId = Guid.Empty then
            Guid.NewGuid()
        else
            criteria.CriteriaId

    /// <summary>
    /// 行政区划匹配（优化版）
    /// 支持多级行政区划匹配和IP地址降级
    /// </summary>
    let matchAdministrativeGeo (geoInfo: GeoInfo) (targeting: AdministrativeGeoTargeting) : (bool * string * decimal) =
        let includedMatches =
            targeting.IncludedLocations
            |> Seq.exists (fun location ->
                (not (isNull location.CountryCode)
                 && not (isNull geoInfo.CountryCode)
                 && location.CountryCode = geoInfo.CountryCode)
                || (not (isNull location.ProvinceCode)
                    && not (isNull geoInfo.ProvinceCode)
                    && location.ProvinceCode = geoInfo.ProvinceCode)
                || (not (isNull location.CityName)
                    && not (isNull geoInfo.CityName)
                    && location.CityName = geoInfo.CityName))

        let excludedMatches =
            targeting.ExcludedLocations
            |> Seq.exists (fun location ->
                (not (isNull location.CountryCode)
                 && not (isNull geoInfo.CountryCode)
                 && location.CountryCode = geoInfo.CountryCode)
                || (not (isNull location.ProvinceCode)
                    && not (isNull geoInfo.ProvinceCode)
                    && location.ProvinceCode = geoInfo.ProvinceCode)
                || (not (isNull location.CityName)
                    && not (isNull geoInfo.CityName)
                    && location.CityName = geoInfo.CityName))

        // 根据数据来源调整评分
        let baseScore =
            match geoInfo.DataSource with
            | "GPS" -> 0.95m // GPS数据最可靠
            | "WIFI" -> 0.9m // WIFI定位较可靠
            | "CELL" -> 0.85m // 基站定位中等可靠
            | "IP" -> 0.7m // IP定位可靠性较低
            | _ -> 0.8m // 默认可靠性

        match targeting.Mode with
        | GeoTargetingMode.Include ->
            if includedMatches && not excludedMatches then
                (true, "用户位置在包含的地理区域内", baseScore)
            else
                (false, "用户位置不在包含的地理区域内或在排除区域内", 0.0m)
        | GeoTargetingMode.Exclude ->
            if not excludedMatches then
                (true, "用户位置不在排除的地理区域内", baseScore * 0.9m)
            else
                (false, "用户位置在排除的地理区域内", 0.0m)
        | _ -> (false, "未知的地理定向模式", 0.0m)

    /// <summary>
    /// 圆形地理围栏匹配（优化版）
    /// 使用高精度距离计算和缓存优化
    /// </summary>
    let matchCircularGeoFence (geoInfo: GeoInfo) (targeting: CircularGeoFenceTargeting) : (bool * string * decimal) =
        let latOpt = nullableToOption geoInfo.Latitude
        let lngOpt = nullableToOption geoInfo.Longitude

        match latOpt, lngOpt with
        | Some userLat, Some userLng ->
            let distance =
                calculateHaversineDistance userLat userLng targeting.Latitude targeting.Longitude

            let radiusFloat = float targeting.RadiusMeters

            if distance <= radiusFloat then
                // 根据距离计算精确度评分
                let distanceRatio = distance / radiusFloat
                let accuracyScore = 1.0m - decimal distanceRatio * 0.1m
                let finalScore = max 0.85m accuracyScore

                (true, $"用户位置在圆形围栏内（距离: {distance:F1}米，半径: {targeting.RadiusMeters}米）", finalScore)
            else
                (false, $"用户位置不在圆形围栏内（距离: {distance:F1}米，半径: {targeting.RadiusMeters}米）", 0.0m)
        | _ -> (false, "用户位置信息不完整，无法进行圆形围栏匹配", 0.0m)

    /// <summary>
    /// 多边形地理围栏匹配（优化版）
    /// 使用优化的射线法和空间索引
    /// </summary>
    let matchPolygonGeoFence (geoInfo: GeoInfo) (targeting: PolygonGeoFenceTargeting) : (bool * string * decimal) =
        let latOpt = nullableToOption geoInfo.Latitude
        let lngOpt = nullableToOption geoInfo.Longitude

        match latOpt, lngOpt with
        | Some userLat, Some userLng ->
            if targeting.Points.Count >= 3 then
                // 尝试从边界缓存获取预计算结果
                let boundsKey = $"polygon_{targeting.GetHashCode()}"

                let (minLat, maxLat, minLng, maxLng) =
                    match BoundaryCache.TryGetValue(boundsKey) with
                    | true, bounds -> bounds
                    | false, _ ->
                        let bounds = getPolygonBounds targeting.Points
                        BoundaryCache.[boundsKey] <- bounds
                        bounds

                // 快速边界框检查
                if userLat < minLat || userLat > maxLat || userLng < minLng || userLng > maxLng then
                    (false, $"用户位置不在多边形围栏边界框内（{targeting.Points.Count}个顶点）", 0.0m)
                else
                    let isInside = isPointInPolygon userLat userLng targeting.Points

                    if isInside then
                        (true, $"用户位置在多边形围栏内（{targeting.Points.Count}个顶点）", 0.95m)
                    else
                        (false, $"用户位置不在多边形围栏内（{targeting.Points.Count}个顶点）", 0.0m)
            else
                (false, "多边形围栏顶点数量不足，至少需要3个顶点", 0.0m)
        | _ -> (false, "用户位置信息不完整，无法进行多边形围栏匹配", 0.0m)

    interface ITargetingMatcher with
        member _.MatcherId = "GeoLocationMatcher"

        member _.MatcherName = "地理位置定向匹配器"

        member _.Version = "1.0.0"

        member _.MatcherType = "GeoLocation"

        member _.Priority = 2

        member _.IsEnabled = true

        member _.ExpectedExecutionTime = TimeSpan.FromMilliseconds(10.0) // 优化后更快

        member _.CanRunInParallel = true

        member _.CalculateMatchScoreAsync
            (
                context: ITargetingContext,
                criteria: ITargetingCriteria,
                callbackProvider: ICallbackProvider,
                cancellationToken: CancellationToken
            ) =
            Task.Run(fun () ->
                let startTime = DateTime.UtcNow
                let timeoutThreshold = TimeSpan.FromMilliseconds(50.0) // 50ms超时阈值

                try
                    // 生成缓存键
                    let currentGeo =
                        match context with
                        | :? GeoInfo as geo -> geo
                        | _ -> GeoInfo()

                    let cacheKey =
                        generateCacheKey currentGeo (criteria.CriteriaId.ToString()) (criteria.GetType().Name)

                    // 尝试从多级缓存获取结果
                    match getMultiLevelCachedResult cacheKey with
                    | Some(isMatch, reason, score) ->
                        let executionTime = DateTime.UtcNow - startTime

                        if isMatch then
                            MatchResult.CreateMatch(
                                "GeoLocation",
                                ensureValidCriteriaId criteria,
                                score,
                                $"{reason} (多级缓存)",
                                executionTime
                            )
                        else
                            MatchResult.CreateNoMatch(
                                "GeoLocation",
                                ensureValidCriteriaId criteria,
                                $"{reason} (多级缓存)",
                                executionTime
                            )
                    | None ->
                        // 提取地理位置上下文
                        let geoContext =
                            match context with
                            | :? GeoInfo as geoInfo -> Some geoInfo
                            | _ ->
                                // 尝试从通用上下文中提取地理信息
                                let countryCode = context.GetPropertyValue<string>("CountryCode")
                                let provinceCode = context.GetPropertyValue<string>("ProvinceCode")
                                let cityName = context.GetPropertyValue<string>("CityName")
                                let latitude = context.GetPropertyValue<Nullable<decimal>>("Latitude")
                                let longitude = context.GetPropertyValue<Nullable<decimal>>("Longitude")
                                let dataSource = context.GetPropertyValue<string>("DataSource")

                                if not (isNull countryCode) || latitude.HasValue then
                                    Some(
                                        GeoInfo(
                                            countryCode = countryCode,
                                            provinceCode = provinceCode,
                                            cityName = cityName,
                                            latitude =
                                                (if latitude.HasValue then
                                                     Nullable<decimal>(latitude.Value)
                                                 else
                                                     Nullable<decimal>()),
                                            longitude =
                                                (if longitude.HasValue then
                                                     Nullable<decimal>(longitude.Value)
                                                 else
                                                     Nullable<decimal>()),
                                            dataSource = dataSource
                                        )
                                    )
                                else
                                    // IP地址降级机制
                                    tryIPLocationFallback context callbackProvider

                        match geoContext with
                        | None ->
                            let executionTime = DateTime.UtcNow - startTime

                            let result =
                                MatchResult.CreateNoMatch(
                                    "GeoLocation",
                                    ensureValidCriteriaId criteria,
                                    "无法获取用户地理位置信息，IP降级也失败",
                                    executionTime
                                )

                            // 缓存失败结果（短时间）
                            cacheMatchResultMultiLevel
                                cacheKey
                                false
                                "无法获取用户地理位置信息"
                                0.0m
                                (TimeSpan.FromMinutes(5.0))
                                (GeoInfo())

                            result
                        | Some geoInfo ->
                            // 检查超时和降级处理
                            match handleTimeoutAndFallback geoInfo criteria startTime timeoutThreshold with
                            | Some(isTimeoutMatch, timeoutReason, timeoutScore) ->
                                let executionTime = DateTime.UtcNow - startTime

                                // 缓存超时降级结果
                                cacheMatchResultMultiLevel
                                    cacheKey
                                    isTimeoutMatch
                                    timeoutReason
                                    timeoutScore
                                    (TimeSpan.FromMinutes(10.0))
                                    geoInfo

                                if isTimeoutMatch then
                                    MatchResult.CreateMatch(
                                        "GeoLocation",
                                        ensureValidCriteriaId criteria,
                                        timeoutScore,
                                        timeoutReason,
                                        executionTime
                                    )
                                else
                                    MatchResult.CreateNoMatch(
                                        "GeoLocation",
                                        ensureValidCriteriaId criteria,
                                        timeoutReason,
                                        executionTime
                                    )
                            | None ->
                                // 检查取消请求
                                if cancellationToken.IsCancellationRequested then
                                    let executionTime = DateTime.UtcNow - startTime

                                    MatchResult.CreateNoMatch(
                                        "GeoLocation",
                                        ensureValidCriteriaId criteria,
                                        "地理位置匹配计算被取消",
                                        executionTime
                                    )
                                else
                                    // 根据位置精度选择匹配策略
                                    let strategy = selectMatchingStrategy geoInfo criteria

                                    let (isMatch, reason, score) =
                                        match criteria with
                                        | :? CircularGeoFenceTargeting as circular ->
                                            matchCircularGeoFence geoInfo circular
                                        | :? PolygonGeoFenceTargeting as polygon ->
                                            matchPolygonGeoFence geoInfo polygon
                                        | :? AdministrativeGeoTargeting as admin ->
                                            matchAdministrativeGeo geoInfo admin
                                        | _ -> (false, "不支持的地理定向条件类型", 0.0m)

                                    let executionTime = DateTime.UtcNow - startTime

                                    // 使用多级缓存策略缓存结果
                                    let cacheExpiration = getCacheExpirationTime geoInfo

                                    cacheMatchResultMultiLevel
                                        cacheKey
                                        isMatch
                                        $"{reason} (策略: {strategy})"
                                        score
                                        cacheExpiration
                                        geoInfo

                                    if isMatch then
                                        MatchResult.CreateMatch(
                                            "GeoLocation",
                                            ensureValidCriteriaId criteria,
                                            score,
                                            $"{reason} (策略: {strategy})",
                                            executionTime
                                        )
                                    else
                                        MatchResult.CreateNoMatch(
                                            "GeoLocation",
                                            ensureValidCriteriaId criteria,
                                            $"{reason} (策略: {strategy})",
                                            executionTime
                                        )

                with ex ->
                    let executionTime = DateTime.UtcNow - startTime

                    MatchResult.CreateNoMatch(
                        "GeoLocation",
                        ensureValidCriteriaId criteria,
                        $"地理位置匹配执行异常: {ex.Message}",
                        executionTime
                    ))

        member _.IsSupported(criteriaType: string) =
            match criteriaType.ToLowerInvariant() with
            | "circulargeofence"
            | "circular_geofence"
            | "polygongeofence"
            | "polygon_geofence"
            | "administrativegeo"
            | "administrative_geo"
            | "geolocation"
            | "geo" -> true
            | _ -> false

        member _.ValidateCriteria(criteria: ITargetingCriteria) =
            try
                match criteria with
                | :? CircularGeoFenceTargeting as circular ->
                    if circular.RadiusMeters <= 0 then
                        ValidationResult.Failure(
                            [ ValidationError(Message = "圆形围栏半径必须大于0", Field = "RadiusMeters") ],
                            "圆形地理围栏验证失败"
                        )
                    elif abs (circular.Latitude) > 90m then
                        ValidationResult.Failure(
                            [ ValidationError(Message = "纬度必须在-90到90之间", Field = "Latitude") ],
                            "圆形地理围栏验证失败"
                        )
                    elif abs (circular.Longitude) > 180m then
                        ValidationResult.Failure(
                            [ ValidationError(Message = "经度必须在-180到180之间", Field = "Longitude") ],
                            "圆形地理围栏验证失败"
                        )
                    else
                        ValidationResult.Success("圆形地理围栏验证通过")

                | :? PolygonGeoFenceTargeting as polygon ->
                    if polygon.Points.Count < 3 then
                        ValidationResult.Failure(
                            [ ValidationError(Message = "多边形围栏至少需要3个顶点", Field = "Points") ],
                            "多边形地理围栏验证失败"
                        )
                    else
                        let invalidPoints =
                            polygon.Points
                            |> Seq.mapi (fun i point -> (i, point))
                            |> Seq.filter (fun (i, point) -> abs (point.Latitude) > 90m || abs (point.Longitude) > 180m)
                            |> Seq.toList

                        if invalidPoints.Length > 0 then
                            let errors =
                                invalidPoints
                                |> List.map (fun (i, _) ->
                                    ValidationError(Message = "坐标值超出有效范围", Field = $"Points[{i}]"))

                            ValidationResult.Failure(errors, "多边形地理围栏验证失败")
                        else
                            ValidationResult.Success("多边形地理围栏验证通过")

                | :? AdministrativeGeoTargeting as admin ->
                    if admin.IncludedLocations.Count = 0 && admin.ExcludedLocations.Count = 0 then
                        ValidationResult.Failure(
                            [ ValidationError(Message = "包含或排除的地理位置列表不能同时为空", Field = "Locations") ],
                            "行政地理定向验证失败"
                        )
                    else
                        ValidationResult.Success("行政地理定向验证通过")

                | _ ->
                    ValidationResult.Failure(
                        [ ValidationError(Message = "不支持的地理定向条件类型", Field = "CriteriaType") ],
                        "地理定向条件验证失败"
                    )

            with ex ->
                ValidationResult.Failure([ ValidationError(Message = ex.Message, Field = "Exception") ], "地理定向条件验证异常")

        member _.GetMetadata() =
            TargetingMatcherMetadata(
                MatcherId = "GeoLocationMatcher",
                Name = "地理位置定向匹配器",
                Description = "支持圆形地理围栏、多边形地理围栏和行政地理定向的匹配器，使用高精度算法进行地理位置判断",
                Version = "1.0.0",
                MatcherType = "GeoLocation",
                Author = "Lorn.ADSP Team",
                SupportedCriteriaTypes =
                    [ "CircularGeoFence"
                      "PolygonGeoFence"
                      "AdministrativeGeo"
                      "GeoLocation"
                      "Geo" ],
                SupportsParallelExecution = true,
                ExpectedExecutionTime = TimeSpan.FromMilliseconds(15.0),
                ResourceRequirement =
                    ResourceRequirement(
                        ExpectedMemoryUsageMB = 10.0,
                        MaxMemoryUsageMB = 50.0,
                        IsCpuIntensive = false,
                        RequiresNetworkAccess = false
                    ),
                Tags = [ "geo"; "location"; "targeting"; "fence"; "administrative" ]
            )
