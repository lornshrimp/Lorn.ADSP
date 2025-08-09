namespace Lorn.ADSP.Strategies.Targeting.Utils

open System
open System.Collections.Concurrent

/// 预计算时间片键
[<Struct>]
type TimeKey = { Date: DateTime; Hour: int }

/// 预计算条目
[<CLIMutable>]
type TimePrecomputeEntry =
    { Key: TimeKey
      Season: string
      DayOfWeekNorm: int }

/// 预计算与缓存（简单内存实现，后续可接入分布式缓存）
module TimePrecomputeCache =
    let private cache = ConcurrentDictionary<TimeKey, TimePrecomputeEntry>()

    let private compute (dt: DateTime) =
        let season =
            match dt.Month with
            | 3
            | 4
            | 5 -> "Spring"
            | 6
            | 7
            | 8 -> "Summer"
            | 9
            | 10
            | 11 -> "Autumn"
            | _ -> "Winter"

        let dow =
            let raw = int dt.DayOfWeek // Sunday=0
            raw

        { Key = { Date = dt.Date; Hour = dt.Hour }
          Season = season
          DayOfWeekNorm = dow }

    let get (dt: DateTime) =
        let key = { Date = dt.Date; Hour = dt.Hour }
        cache.GetOrAdd(key, fun _ -> compute dt)

    /// 预热指定日期范围
    let prewarm (startDate: DateTime) (endDate: DateTime) =
        let mutable cursor = startDate

        while cursor <= endDate do
            ignore (get cursor)
            cursor <- cursor.AddHours(1.)
