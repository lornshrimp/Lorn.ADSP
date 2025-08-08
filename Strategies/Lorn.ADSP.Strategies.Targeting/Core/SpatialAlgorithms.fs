namespace Lorn.ADSP.Strategies.Targeting.Core

open System
open System.Collections.Generic
open System.Collections.Concurrent
open Lorn.ADSP.Core.Domain.ValueObjects

/// <summary>
/// 四叉树节点定义
/// 用于空间索引优化，支持快速地理查询
/// 支持需求 2.5, 2.6: 创建空间索引优化机制
/// </summary>
type QuadTreeNode =
    { Bounds: decimal * decimal * decimal * decimal // minLat, maxLat, minLng, maxLng
      Points: GeoPoint list
      Children: QuadTreeNode option array // NW, NE, SW, SE
      IsLeaf: bool
      Depth: int }

/// <summary>
/// 四叉树空间索引实现
/// 用于优化大量地理点的查询性能
/// 支持需求 7.1, 7.2: 高性能空间查询优化
/// </summary>
module QuadTree =
    let MaxPointsPerNode = 10
    let MaxDepth = 8

    /// <summary>
    /// 创建空的四叉树节点
    /// </summary>
    let createNode (bounds: decimal * decimal * decimal * decimal) (depth: int) : QuadTreeNode =
        { Bounds = bounds
          Points = []
          Children = Array.create 4 None
          IsLeaf = true
          Depth = depth }

    /// <summary>
    /// 检查点是否在边界框内
    /// </summary>
    let isPointInBounds
        (lat: decimal)
        (lng: decimal)
        (minLat: decimal, maxLat: decimal, minLng: decimal, maxLng: decimal)
        : bool =
        lat >= minLat && lat <= maxLat && lng >= minLng && lng <= maxLng

    /// <summary>
    /// 分割边界框为四个子区域
    /// </summary>
    let splitBounds
        (minLat: decimal, maxLat: decimal, minLng: decimal, maxLng: decimal)
        : (decimal * decimal * decimal * decimal) array =
        let midLat = (minLat + maxLat) / 2m
        let midLng = (minLng + maxLng) / 2m

        [| (midLat, maxLat, minLng, midLng) // NW
           (midLat, maxLat, midLng, maxLng) // NE
           (minLat, midLat, minLng, midLng) // SW
           (minLat, midLat, midLng, maxLng) |] // SE

    /// <summary>
    /// 向四叉树插入点
    /// </summary>
    let rec insertPoint (node: QuadTreeNode) (point: GeoPoint) : QuadTreeNode =
        if not (isPointInBounds point.Latitude point.Longitude node.Bounds) then
            node // 点不在当前节点范围内
        elif node.IsLeaf then
            if node.Points.Length < MaxPointsPerNode || node.Depth >= MaxDepth then
                // 直接添加到叶子节点
                { node with
                    Points = point :: node.Points }
            else
                // 需要分割节点
                let childBounds = splitBounds node.Bounds

                let children =
                    Array.mapi (fun i bounds -> Some(createNode bounds (node.Depth + 1))) childBounds

                // 重新分配现有点到子节点
                let mutable updatedChildren = children

                for existingPoint in node.Points do
                    for i = 0 to 3 do
                        match updatedChildren.[i] with
                        | Some child when isPointInBounds existingPoint.Latitude existingPoint.Longitude child.Bounds ->
                            updatedChildren.[i] <- Some(insertPoint child existingPoint)
                        | _ -> ()

                // 插入新点到适当的子节点
                for i = 0 to 3 do
                    match updatedChildren.[i] with
                    | Some child when isPointInBounds point.Latitude point.Longitude child.Bounds ->
                        updatedChildren.[i] <- Some(insertPoint child point)
                    | _ -> ()

                { node with
                    Children = updatedChildren
                    IsLeaf = false
                    Points = [] }
        else
            // 非叶子节点，递归插入到适当的子节点
            let mutable updatedChildren = Array.copy node.Children

            for i = 0 to 3 do
                match updatedChildren.[i] with
                | Some child when isPointInBounds point.Latitude point.Longitude child.Bounds ->
                    updatedChildren.[i] <- Some(insertPoint child point)
                | _ -> ()

            { node with Children = updatedChildren }

    /// <summary>
    /// 在四叉树中查询指定范围内的点
    /// </summary>
    let rec queryRange (node: QuadTreeNode) (queryBounds: decimal * decimal * decimal * decimal) : GeoPoint list =
        let (qMinLat, qMaxLat, qMinLng, qMaxLng) = queryBounds
        let (nMinLat, nMaxLat, nMinLng, nMaxLng) = node.Bounds

        // 检查查询范围是否与节点边界相交
        let boundsIntersect =
            qMinLat <= nMaxLat
            && qMaxLat >= nMinLat
            && qMinLng <= nMaxLng
            && qMaxLng >= nMinLng

        if not boundsIntersect then
            [] // 不相交，返回空列表
        elif node.IsLeaf then
            // 叶子节点，过滤范围内的点
            node.Points
            |> List.filter (fun p -> isPointInBounds p.Latitude p.Longitude queryBounds)
        else
            // 非叶子节点，递归查询子节点
            node.Children
            |> Array.toList
            |> List.choose id
            |> List.collect (fun child -> queryRange child queryBounds)

/// <summary>
/// 地理哈希编码实现
/// 用于快速地理位置索引和邻近查询
/// 支持需求 2.5, 2.6: 地理数据预处理机制
/// </summary>
module GeoHash =
    let Base32 = "0123456789bcdefghjkmnpqrstuvwxyz"

    /// <summary>
    /// 计算地理哈希编码
    /// </summary>
    let encode (lat: decimal) (lng: decimal) (precision: int) : string =
        let mutable latMin = -90.0
        let mutable latMax = 90.0
        let mutable lngMin = -180.0
        let mutable lngMax = 180.0

        let latDouble = double lat
        let lngDouble = double lng

        let mutable bits = 0
        let mutable bit = 0
        let mutable ch = 0
        let mutable even = true
        let result = System.Text.StringBuilder()

        while result.Length < precision do
            if even then
                // 处理经度
                let mid = (lngMin + lngMax) / 2.0

                if lngDouble >= mid then
                    ch <- ch ||| (1 <<< (4 - bit))
                    lngMin <- mid
                else
                    lngMax <- mid
            else
                // 处理纬度
                let mid = (latMin + latMax) / 2.0

                if latDouble >= mid then
                    ch <- ch ||| (1 <<< (4 - bit))
                    latMin <- mid
                else
                    latMax <- mid

            even <- not even

            if bit < 4 then
                bit <- bit + 1
            else
                result.Append(Base32.[ch]) |> ignore
                bit <- 0
                ch <- 0

        result.ToString()

    /// <summary>
    /// 获取地理哈希的邻近编码
    /// 用于邻近区域查询
    /// </summary>
    let getNeighbors (geohash: string) : string list =
        if String.IsNullOrEmpty(geohash) then
            []
        else
            let neighbors =
                [
                  // 8个方向的邻近编码计算（简化实现）
                  geohash ] // 中心
            // 实际实现中需要根据地理哈希算法计算8个方向的邻近编码
            neighbors

/// <summary>
/// 空间索引管理器
/// 统一管理各种空间索引结构
/// 支持需求 2.5, 2.6: 空间索引优化和地理数据预处理机制
/// </summary>
type SpatialIndexManager() =
    let mutable quadTreeCache = ConcurrentDictionary<string, QuadTreeNode>()
    let mutable geoHashCache = ConcurrentDictionary<string, string>()

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
    /// 为多边形创建四叉树索引
    /// </summary>
    member _.CreateQuadTreeIndex (polygonId: string) (points: IReadOnlyList<GeoPoint>) : unit =
        if points.Count > 0 then
            let bounds = getPolygonBounds points
            let rootNode = QuadTree.createNode bounds 0

            let indexedNode = points |> Seq.fold QuadTree.insertPoint rootNode

            quadTreeCache.[polygonId] <- indexedNode

    /// <summary>
    /// 使用四叉树进行快速点查询
    /// </summary>
    member _.QueryPointsInRange
        (polygonId: string)
        (queryBounds: decimal * decimal * decimal * decimal)
        : GeoPoint list =
        match quadTreeCache.TryGetValue(polygonId) with
        | true, quadTree -> QuadTree.queryRange quadTree queryBounds
        | false, _ -> []

    /// <summary>
    /// 获取地理位置的哈希编码
    /// </summary>
    member _.GetGeoHash (lat: decimal) (lng: decimal) (precision: int) : string =
        let key = $"{lat}_{lng}_{precision}"

        match geoHashCache.TryGetValue(key) with
        | true, hash -> hash
        | false, _ ->
            let hash = GeoHash.encode lat lng precision
            geoHashCache.[key] <- hash
            hash

    /// <summary>
    /// 清理缓存
    /// </summary>
    member _.ClearCache() : unit =
        quadTreeCache.Clear()
        geoHashCache.Clear()

    /// <summary>
    /// 获取多边形边界框（公共方法）
    /// </summary>
    member _.GetPolygonBounds(points: IReadOnlyList<GeoPoint>) : (decimal * decimal * decimal * decimal) =
        getPolygonBounds points
