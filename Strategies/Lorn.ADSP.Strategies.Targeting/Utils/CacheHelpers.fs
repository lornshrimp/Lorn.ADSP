namespace Lorn.ADSP.Strategies.Targeting.Utils

open System
open System.Collections.Concurrent
open System.Collections.Generic
open Lorn.ADSP.Core.Domain.ValueObjects

/// <summary>
/// 缓存辅助函数模块
/// 提供人口属性匹配结果的缓存操作和智能失效策略
/// </summary>
module CacheHelpers =

    /// <summary>
    /// 缓存项结构
    /// </summary>
    type CacheItem<'T> =
        { Value: 'T
          CreatedAt: DateTime
          ExpiresAt: DateTime
          AccessCount: int
          LastAccessedAt: DateTime
          Weight: decimal
          DataSource: string }

    /// <summary>
    /// 缓存统计信息
    /// </summary>
    type CacheStatistics =
        { TotalItems: int
          HitCount: int64
          MissCount: int64
          HitRate: decimal
          EvictionCount: int64
          TotalMemoryUsage: int64 }

    /// <summary>
    /// 内存缓存存储（线程安全）
    /// </summary>
    let private memoryCache = ConcurrentDictionary<string, CacheItem<obj>>()

    /// <summary>
    /// 缓存统计计数器
    /// </summary>
    let mutable private hitCount = 0L
    let mutable private missCount = 0L
    let mutable private evictionCount = 0L

    /// <summary>
    /// 生成人口属性匹配的缓存键
    /// </summary>
    let generateDemographicCacheKey (userId: string) (criteriaHash: string) : string =
        $"demographic_match:{userId}:{criteriaHash}"

    /// <summary>
    /// 生成属性权重配置的缓存键
    /// </summary>
    let generateWeightConfigCacheKey (configVersion: string) : string = $"weight_config:{configVersion}"

    /// <summary>
    /// 计算属性数据的哈希值（用于缓存键）
    /// </summary>
    let calculateDataHash (properties: ContextProperty list) : string =
        let sortedData =
            properties
            |> List.sortBy (fun p -> p.PropertyKey)
            |> List.map (fun p -> $"{p.PropertyKey}={p.PropertyValue}")
            |> String.concat "|"

        // 简单哈希算法（生产环境建议使用更强的哈希算法）
        let hash = sortedData.GetHashCode()
        hash.ToString("X8")

    /// <summary>
    /// 根据属性特征计算智能过期时间
    /// </summary>
    let calculateIntelligentExpiration (properties: ContextProperty list) : DateTime =
        let baseExpirationMinutes = 60.0 // 基础过期时间1小时

        // 根据属性稳定性调整过期时间
        let stabilityFactor =
            properties
            |> List.map (fun p ->
                match p.PropertyKey with
                | "Age" -> 24.0 * 60.0 // 年龄相对稳定，24小时
                | "Gender" -> 24.0 * 60.0 * 30.0 // 性别非常稳定，30天
                | "Education" -> 24.0 * 60.0 * 7.0 // 教育程度较稳定，7天
                | "Occupation" -> 24.0 * 60.0 * 3.0 // 职业中等稳定，3天
                | "Income" -> 24.0 * 60.0 * 1.0 // 收入变化较快，1天
                | "MaritalStatus" -> 24.0 * 60.0 * 7.0 // 婚恋状况较稳定，7天
                | _ -> baseExpirationMinutes)
            |> List.average

        // 根据数据来源调整过期时间
        let sourceAdjustment =
            properties
            |> List.map (fun p ->
                match p.DataSource with
                | "Inference" -> 0.5 // 推断数据过期时间减半
                | "ThirdParty" -> 0.8 // 第三方数据过期时间减少20%
                | "UserInput" -> 1.2 // 用户输入数据过期时间增加20%
                | _ -> 1.0)
            |> List.average

        let finalExpirationMinutes = stabilityFactor * sourceAdjustment
        DateTime.UtcNow.AddMinutes(finalExpirationMinutes)

    /// <summary>
    /// 存储缓存项
    /// </summary>
    let setCacheItem<'T>
        (key: string)
        (value: 'T)
        (weight: decimal)
        (dataSource: string)
        (customExpiration: DateTime option)
        : unit =
        let expiresAt =
            match customExpiration with
            | Some expiration -> expiration
            | None -> DateTime.UtcNow.AddHours(1.0) // 默认1小时过期

        let cacheItem =
            { Value = box value
              CreatedAt = DateTime.UtcNow
              ExpiresAt = expiresAt
              AccessCount = 0
              LastAccessedAt = DateTime.UtcNow
              Weight = weight
              DataSource = dataSource }

        memoryCache.AddOrUpdate(key, cacheItem, fun _ _ -> cacheItem) |> ignore

    /// <summary>
    /// 获取缓存项
    /// </summary>
    let getCacheItem<'T> (key: string) : 'T option =
        match memoryCache.TryGetValue(key) with
        | (true, cacheItem) ->
            // 检查是否过期
            if DateTime.UtcNow > cacheItem.ExpiresAt then
                memoryCache.TryRemove(key) |> ignore
                System.Threading.Interlocked.Increment(&missCount) |> ignore
                None
            else
                // 更新访问统计
                let updatedItem =
                    { cacheItem with
                        AccessCount = cacheItem.AccessCount + 1
                        LastAccessedAt = DateTime.UtcNow }

                memoryCache.TryUpdate(key, updatedItem, cacheItem) |> ignore
                System.Threading.Interlocked.Increment(&hitCount) |> ignore

                try
                    Some(unbox<'T> cacheItem.Value)
                with _ ->
                    None
        | _ ->
            System.Threading.Interlocked.Increment(&missCount) |> ignore
            None

    /// <summary>
    /// 缓存人口属性匹配结果
    /// </summary>
    let cacheDemographicMatchResult
        (userId: string)
        (criteriaHash: string)
        (matchResult: obj)
        (properties: ContextProperty list)
        : unit =
        let cacheKey = generateDemographicCacheKey userId criteriaHash
        let intelligentExpiration = calculateIntelligentExpiration properties
        let weight = 1.0m // 匹配结果权重

        setCacheItem cacheKey matchResult weight "MatchingEngine" (Some intelligentExpiration)

    /// <summary>
    /// 获取缓存的人口属性匹配结果
    /// </summary>
    let getCachedDemographicMatchResult<'T> (userId: string) (criteriaHash: string) : 'T option =
        let cacheKey = generateDemographicCacheKey userId criteriaHash
        getCacheItem<'T> cacheKey

    /// <summary>
    /// 缓存属性权重配置
    /// </summary>
    let cacheWeightConfiguration (configVersion: string) (weightConfig: Map<string, decimal>) : unit =
        let cacheKey = generateWeightConfigCacheKey configVersion
        let expiration = DateTime.UtcNow.AddHours(24.0) // 权重配置24小时过期

        setCacheItem cacheKey weightConfig 2.0m "Configuration" (Some expiration)

    /// <summary>
    /// 获取缓存的属性权重配置
    /// </summary>
    let getCachedWeightConfiguration (configVersion: string) : Map<string, decimal> option =
        let cacheKey = generateWeightConfigCacheKey configVersion
        getCacheItem<Map<string, decimal>> cacheKey

    /// <summary>
    /// 清理过期的缓存项
    /// </summary>
    let cleanupExpiredItems () : int =
        let currentTime = DateTime.UtcNow
        let expiredKeys = ResizeArray<string>()

        for kvp in memoryCache do
            if currentTime > kvp.Value.ExpiresAt then
                expiredKeys.Add(kvp.Key)

        let removedCount =
            expiredKeys
            |> Seq.map (fun key ->
                match memoryCache.TryRemove(key) with
                | (true, _) -> 1
                | _ -> 0)
            |> Seq.sum

        System.Threading.Interlocked.Add(&evictionCount, int64 removedCount) |> ignore
        removedCount

    /// <summary>
    /// 基于LRU策略清理缓存
    /// </summary>
    let cleanupLRUItems (maxItems: int) : int =
        if memoryCache.Count <= maxItems then
            0
        else
            let itemsToRemove = memoryCache.Count - maxItems

            let lruItems =
                memoryCache
                |> Seq.sortBy (fun kvp -> kvp.Value.LastAccessedAt)
                |> Seq.take itemsToRemove
                |> Seq.map (fun kvp -> kvp.Key)
                |> List.ofSeq

            let removedCount =
                lruItems
                |> List.map (fun key ->
                    match memoryCache.TryRemove(key) with
                    | (true, _) -> 1
                    | _ -> 0)
                |> List.sum

            System.Threading.Interlocked.Add(&evictionCount, int64 removedCount) |> ignore
            removedCount

    /// <summary>
    /// 基于权重清理低价值缓存项
    /// </summary>
    let cleanupLowWeightItems (minWeight: decimal) : int =
        let lowWeightKeys =
            memoryCache
            |> Seq.filter (fun kvp -> kvp.Value.Weight < minWeight)
            |> Seq.map (fun kvp -> kvp.Key)
            |> List.ofSeq

        let removedCount =
            lowWeightKeys
            |> List.map (fun key ->
                match memoryCache.TryRemove(key) with
                | (true, _) -> 1
                | _ -> 0)
            |> List.sum

        System.Threading.Interlocked.Add(&evictionCount, int64 removedCount) |> ignore
        removedCount

    /// <summary>
    /// 获取缓存统计信息
    /// </summary>
    let getCacheStatistics () : CacheStatistics =
        let totalHits = System.Threading.Interlocked.Read(&hitCount)
        let totalMisses = System.Threading.Interlocked.Read(&missCount)
        let totalRequests = totalHits + totalMisses

        let hitRate =
            if totalRequests > 0L then
                decimal totalHits / decimal totalRequests
            else
                0.0m

        { TotalItems = memoryCache.Count
          HitCount = totalHits
          MissCount = totalMisses
          HitRate = hitRate
          EvictionCount = System.Threading.Interlocked.Read(&evictionCount)
          TotalMemoryUsage = int64 memoryCache.Count * 1024L } // 估算内存使用

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    let clearAllCache () : unit =
        memoryCache.Clear()
        System.Threading.Interlocked.Exchange(&hitCount, 0L) |> ignore
        System.Threading.Interlocked.Exchange(&missCount, 0L) |> ignore
        System.Threading.Interlocked.Exchange(&evictionCount, 0L) |> ignore

    /// <summary>
    /// 执行智能缓存维护
    /// </summary>
    let performIntelligentMaintenance (maxItems: int) (minWeight: decimal) : unit =
        // 1. 清理过期项
        cleanupExpiredItems () |> ignore

        // 2. 如果仍然超过限制，清理低权重项
        if memoryCache.Count > maxItems then
            cleanupLowWeightItems minWeight |> ignore

        // 3. 如果仍然超过限制，使用LRU策略
        if memoryCache.Count > maxItems then
            cleanupLRUItems maxItems |> ignore

    /// <summary>
    /// 获取模块版本信息
    /// </summary>
    let getModuleVersion () = "2.0.0"
