open System
open Lorn.ADSP.Core.Domain.Targeting

[<EntryPoint>]
let main argv =
    let geoInfo =
        GeoInfo(
            countryCode = "CN",
            provinceCode = "11",
            cityName = "北京市",
            latitude = Nullable<decimal>(39.9042m),
            longitude = Nullable<decimal>(116.4074m),
            dataSource = "Manual"
        )

    printfn "GeoInfo created"
    printfn "Latitude from property: %A" geoInfo.Latitude
    printfn "Longitude from property: %A" geoInfo.Longitude
    printfn "Latitude HasValue: %A" (geoInfo.Latitude.HasValue)
    printfn "Longitude HasValue: %A" (geoInfo.Longitude.HasValue)

    // 直接测试属性获取
    let latProp = geoInfo.GetProperty("Latitude")
    let lngProp = geoInfo.GetProperty("Longitude")

    printfn "Latitude property exists: %A" (latProp <> null)
    printfn "Longitude property exists: %A" (lngProp <> null)

    if latProp <> null then
        printfn "Latitude property value: %s" latProp.PropertyValue
        printfn "Latitude property data type: %s" latProp.DataType

    if lngProp <> null then
        printfn "Longitude property value: %s" lngProp.PropertyValue
        printfn "Longitude property data type: %s" lngProp.DataType

    0
