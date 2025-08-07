namespace Lorn.ADSP.Strategies.Targeting.Matchers

open System
open System.Collections.Generic
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
/// </summary>
type GeoLocationMatcher() =

    /// <summary>
    /// 点是否在圆形区域内
    /// </summary>
    let isPointInCircle
        (userLat: decimal)
        (userLng: decimal)
        (centerLat: decimal)
        (centerLng: decimal)
        (radiusMeters: int)
        : bool =
        let userPoint = GeoPoint.Create(userLat, userLng)
        let centerPoint = GeoPoint.Create(centerLat, centerLng)
        let distance = centerPoint.CalculateDistanceTo(userPoint)
        distance <= float radiusMeters

    /// <summary>
    /// 点是否在多边形区域内（射线法）
    /// </summary>
    let isPointInPolygon (userLat: decimal) (userLng: decimal) (points: IReadOnlyList<GeoPoint>) : bool =
        if points.Count < 3 then
            false
        else
            let x = float userLng
            let y = float userLat

            let rec checkIntersections (vertices: GeoPoint list) (intersections: int) : bool =
                match vertices with
                | []
                | [ _ ] -> intersections % 2 = 1
                | v1 :: v2 :: rest ->
                    let x1 = float v1.Longitude
                    let y1 = float v1.Latitude
                    let x2 = float v2.Longitude
                    let y2 = float v2.Latitude

                    let newIntersections =
                        if ((y1 > y) <> (y2 > y)) && (x < (x2 - x1) * (y - y1) / (y2 - y1) + x1) then
                            intersections + 1
                        else
                            intersections

                    checkIntersections (v2 :: rest) newIntersections

            // 转换为F#列表并闭合多边形
            let pointsList = points |> Seq.toList
            let closedVertices = pointsList @ [ List.head pointsList ]
            checkIntersections closedVertices 0

    /// <summary>
    /// 处理Nullable类型转换为Option
    /// </summary>
    let nullableToOption (nullable: Nullable<'T>) : 'T option =
        if nullable.HasValue then Some nullable.Value else None

    /// <summary>
    /// 行政区划匹配
    /// </summary>
    let matchAdministrativeGeo (geoInfo: GeoInfo) (targeting: AdministrativeGeoTargeting) : (bool * string * decimal) =
        let includedMatches =
            targeting.IncludedLocations
            |> Seq.exists (fun location ->
                (location.CountryCode <> null
                 && geoInfo.CountryCode <> null
                 && location.CountryCode = geoInfo.CountryCode)
                || (location.ProvinceCode <> null
                    && geoInfo.ProvinceCode <> null
                    && location.ProvinceCode = geoInfo.ProvinceCode)
                || (location.CityName <> null
                    && geoInfo.CityName <> null
                    && location.CityName = geoInfo.CityName))

        let excludedMatches =
            targeting.ExcludedLocations
            |> Seq.exists (fun location ->
                (location.CountryCode <> null
                 && geoInfo.CountryCode <> null
                 && location.CountryCode = geoInfo.CountryCode)
                || (location.ProvinceCode <> null
                    && geoInfo.ProvinceCode <> null
                    && location.ProvinceCode = geoInfo.ProvinceCode)
                || (location.CityName <> null
                    && geoInfo.CityName <> null
                    && location.CityName = geoInfo.CityName))

        match targeting.Mode with
        | GeoTargetingMode.Include ->
            if includedMatches && not excludedMatches then
                (true, "用户位置在包含的地理区域内", 0.9m)
            else
                (false, "用户位置不在包含的地理区域内或在排除区域内", 0.0m)
        | GeoTargetingMode.Exclude ->
            if not excludedMatches then
                (true, "用户位置不在排除的地理区域内", 0.8m)
            else
                (false, "用户位置在排除的地理区域内", 0.0m)
        | _ -> (false, "未知的地理定向模式", 0.0m)

    /// <summary>
    /// 圆形地理围栏匹配
    /// </summary>
    let matchCircularGeoFence (geoInfo: GeoInfo) (targeting: CircularGeoFenceTargeting) : (bool * string * decimal) =
        let latOpt = nullableToOption geoInfo.Latitude
        let lngOpt = nullableToOption geoInfo.Longitude

        match latOpt, lngOpt with
        | Some userLat, Some userLng ->
            let isInside =
                isPointInCircle userLat userLng targeting.Latitude targeting.Longitude targeting.RadiusMeters

            if isInside then
                (true, $"用户位置在圆形围栏内（半径: {targeting.RadiusMeters}米）", 0.95m)
            else
                (false, $"用户位置不在圆形围栏内（半径: {targeting.RadiusMeters}米）", 0.0m)
        | _ -> (false, "用户位置信息不完整，无法进行圆形围栏匹配", 0.0m)

    /// <summary>
    /// 多边形地理围栏匹配
    /// </summary>
    let matchPolygonGeoFence (geoInfo: GeoInfo) (targeting: PolygonGeoFenceTargeting) : (bool * string * decimal) =
        let latOpt = nullableToOption geoInfo.Latitude
        let lngOpt = nullableToOption geoInfo.Longitude

        match latOpt, lngOpt with
        | Some userLat, Some userLng ->
            if targeting.Points.Count >= 3 then
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

        member _.ExpectedExecutionTime = TimeSpan.FromMilliseconds(15.0)

        member _.CanRunInParallel = true

        member _.CalculateMatchScoreAsync
            (
                context: ITargetingContext,
                criteria: ITargetingCriteria,
                callbackProvider: ICallbackProvider,
                cancellationToken: CancellationToken
            ) =
            task {
                let startTime = DateTime.UtcNow

                try
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

                            if countryCode <> null || latitude.HasValue then
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
                                                 Nullable<decimal>())
                                    )
                                )
                            else
                                None

                    match geoContext with
                    | None ->
                        let executionTime = DateTime.UtcNow - startTime

                        return
                            MatchResult.CreateNoMatch("GeoLocation", criteria.CriteriaId, "无法获取用户地理位置信息", executionTime)
                    | Some geoInfo ->
                        let (isMatch, reason, score) =
                            match criteria with
                            | :? CircularGeoFenceTargeting as circular -> matchCircularGeoFence geoInfo circular
                            | :? PolygonGeoFenceTargeting as polygon -> matchPolygonGeoFence geoInfo polygon
                            | :? AdministrativeGeoTargeting as admin -> matchAdministrativeGeo geoInfo admin
                            | _ -> (false, "不支持的地理定向条件类型", 0.0m)

                        let executionTime = DateTime.UtcNow - startTime

                        if isMatch then
                            return
                                MatchResult.CreateMatch(
                                    "GeoLocation",
                                    criteria.CriteriaId,
                                    score,
                                    reason,
                                    executionTime
                                )
                        else
                            return MatchResult.CreateNoMatch("GeoLocation", criteria.CriteriaId, reason, executionTime)

                with ex ->
                    let executionTime = DateTime.UtcNow - startTime

                    return
                        MatchResult.CreateNoMatch(
                            "GeoLocation",
                            criteria.CriteriaId,
                            $"地理位置匹配执行异常: {ex.Message}",
                            executionTime
                        )
            }

        member _.IsSupported(criteriaType: string) =
            match criteriaType.ToLowerInvariant() with
            | "circulargeoфence"
            | "polygongeofence"
            | "administrativegeo"
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
