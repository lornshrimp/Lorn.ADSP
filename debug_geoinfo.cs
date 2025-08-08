using System;
using Lorn.ADSP.Core.Domain.Targeting;

var geoInfo = new GeoInfo(
    countryCode: "CN",
    provinceCode: "11",
    cityName: "北京市",
    latitude: 39.9042m,
    longitude: 116.4074m,
    dataSource: "Manual"
);

Console.WriteLine("GeoInfo created");
Console.WriteLine($"Latitude from property: {geoInfo.Latitude}");
Console.WriteLine($"Longitude from property: {geoInfo.Longitude}");
Console.WriteLine($"Latitude HasValue: {geoInfo.Latitude.HasValue}");
Console.WriteLine($"Longitude HasValue: {geoInfo.Longitude.HasValue}");

// 直接测试属性获取
var latProp = geoInfo.GetProperty("Latitude");
var lngProp = geoInfo.GetProperty("Longitude");

Console.WriteLine($"Latitude property exists: {latProp != null}");
Console.WriteLine($"Longitude property exists: {lngProp != null}");

if (latProp != null)
{
    Console.WriteLine($"Latitude property value: {latProp.PropertyValue}");
    Console.WriteLine($"Latitude property data type: {latProp.DataType}");
}

if (lngProp != null)
{
    Console.WriteLine($"Longitude property value: {lngProp.PropertyValue}");
    Console.WriteLine($"Longitude property data type: {lngProp.DataType}");
}
