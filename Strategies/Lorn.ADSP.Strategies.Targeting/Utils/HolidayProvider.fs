namespace Lorn.ADSP.Strategies.Targeting.Utils

open System
open System.Collections.Concurrent

/// 节假日信息
[<CLIMutable>]
type Holiday =
    { Date: DateTime // 以日期（本地日）为键，不含时间部分
      Country: string
      Region: string option
      Name: string }

/// 节假日提供接口（可后续用外部数据源实现）
type IHolidayProvider =
    abstract member GetHolidayName: date: DateTime * country: string * region: string option -> string option
    abstract member IsHoliday: date: DateTime * country: string * region: string option -> bool

/// 内存简单实现（可热更新）
type InMemoryHolidayProvider(initial: seq<Holiday>) =
    let index = ConcurrentDictionary<string, Holiday>()
    let normDate (d: DateTime) = d.Date

    let key (d: DateTime) (country: string) (region: string option) : string =
        let datePart: DateTime = normDate d
        let regionPart: string = region |> Option.defaultValue "*" |> (fun r -> r.ToUpper())
        System.String.Format("{0:yyyy-MM-dd}|{1}|{2}", datePart, country.ToUpper(), regionPart)

    do
        for h in initial do
            index[key h.Date h.Country h.Region] <- h

    member _.AddOrUpdate(holiday: Holiday) =
        index[key holiday.Date holiday.Country holiday.Region] <- holiday

    interface IHolidayProvider with
        member _.GetHolidayName(date, country, region) =
            match index.TryGetValue(key date country region) with
            | true, h -> Some h.Name
            | _ ->
                // 回退到无区域
                match index.TryGetValue(key date country None) with
                | true, h2 -> Some h2.Name
                | _ -> None

        member this.IsHoliday(date, country, region) =
            (this :> IHolidayProvider).GetHolidayName(date, country, region)
            |> Option.isSome

module DefaultHolidaySets =
    let sample: Holiday list =
        [ { Date = DateTime(2025, 1, 1)
            Country = "CN"
            Region = None
            Name = "NewYear" }
          { Date = DateTime(2025, 2, 1)
            Country = "CN"
            Region = None
            Name = "SpringFestival" }
          { Date = DateTime(2025, 12, 25)
            Country = "US"
            Region = None
            Name = "Christmas" } ]
