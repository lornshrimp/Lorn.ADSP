namespace Lorn.ADSP.Strategies.Targeting.Utils

/// <summary>
/// DeviceNormalization / VersionSemver / DeviceCapability
/// 提供设备匹配过程所需的: 品牌别名规范化、型号清洗、分辨率等级分类、语义化版本解析比较、能力(Tier)与价格档推断。
/// 设计原则:
///  - 纯函数/无副作用, 便于单元测试与缓存。
///  - 别名映射后续可替换为外部配置/数据库/动态刷新。
///  - VersionSemver 宽松解析 (非严格 SemVer 输入仍可获得主次补丁数字)。
///  - Capability 推断策略可插拔, 现为启发式规则。
/// 未来扩展建议:
///  - 引入设备硬件基准数据(GPU, RAM)增强 Tier 精度。
///  - 根据地区市场价或 MSRP 动态计算价格区间。
///  - 维护热路径缓存 (brand+model -> tier) 并暴露命中指标。
///  - 语义版本支持构建/预发布标签排序策略。
/// </summary>

open System
open System.Text.RegularExpressions

/// 设备品牌与型号标准化工具
module DeviceNormalization =
    // 品牌别名映射（示例，可扩展至外部配置）
    let private brandAliases: (string * string list) list =
        [ "Apple", [ "Apple"; "苹果"; "蘋果"; "iPhone" ]
          "Samsung", [ "Samsung"; "三星" ]
          "Huawei", [ "Huawei"; "华为" ]
          "Xiaomi", [ "Xiaomi"; "Mi"; "小米" ]
          "OPPO", [ "OPPO"; "Oppo" ]
          "Vivo", [ "Vivo"; "vivo" ]
          "OnePlus", [ "OnePlus"; "一加" ] ]

    let private brandIndex =
        brandAliases
        |> List.collect (fun (canon, alts) -> alts |> List.map (fun a -> a.ToLowerInvariant(), canon))
        |> dict

    /// 标准化品牌名称，返回规范品牌（保持首字母大写）
    let normalizeBrand (brand: string option) =
        match brand with
        | None -> None
        | Some b when String.IsNullOrWhiteSpace b -> None
        | Some b ->
            let key = b.Trim().ToLowerInvariant()

            if brandIndex.ContainsKey key then
                Some brandIndex[key]
            else
                Some(b.Trim())

    /// 型号规范化：去除多余空格、统一大小写策略（保留字母数字顺序，首字母大写其余原样）
    let normalizeModel (model: string option) =
        match model with
        | None -> None
        | Some m when String.IsNullOrWhiteSpace m -> None
        | Some m ->
            let cleaned = Regex.Replace(m.Trim(), "\s+", " ")
            Some cleaned

    /// 解析屏幕规格 (width,height) -> 分辨率等级分类
    let classifyResolution (width: int option) (height: int option) =
        match width, height with
        | Some w, Some h ->
            let minDim = min w h

            if minDim >= 1440 then "UHD"
            elif minDim >= 1080 then "FHD"
            elif minDim >= 768 then "HD"
            else "SD"
        | _ -> "Unknown"

/// 语义化版本处理
module VersionSemver =
    open System

    type SemVersion =
        { Major: int
          Minor: int
          Patch: int
          Suffix: string option }

    let private parseInt (s: string) =
        match Int32.TryParse s with
        | true, v -> v
        | _ -> 0

    /// 解析版本字符串，非严格 SemVer 也尽量宽松（提取前三段数字）
    let parse (input: string option) =
        match input with
        | None -> None
        | Some v when String.IsNullOrWhiteSpace v -> None
        | Some v ->
            let core, suffix =
                let idx = v.IndexOf('-')

                if idx > 0 then
                    v.Substring(0, idx), Some(v.Substring(idx + 1))
                else
                    v, None

            let parts = core.Split('.')
            let major = if parts.Length > 0 then parseInt parts[0] else 0
            let minor = if parts.Length > 1 then parseInt parts[1] else 0
            let patch = if parts.Length > 2 then parseInt parts[2] else 0

            Some
                { Major = major
                  Minor = minor
                  Patch = patch
                  Suffix = suffix }

    /// 比较版本，返回 -1 / 0 / 1
    let compare (a: SemVersion) (b: SemVersion) =
        if a.Major <> b.Major then compare a.Major b.Major
        elif a.Minor <> b.Minor then compare a.Minor b.Minor
        elif a.Patch <> b.Patch then compare a.Patch b.Patch
        else 0

    /// 是否满足最小版本要求（含等于）
    let satisfiesMin (current: string option) (minRequired: string option) =
        match parse current, parse minRequired with
        | Some c, Some m -> compare c m >= 0
        | Some _, None -> true
        | _ -> false

    /// 大版本兼容：要求 major 相同（或策略允许更高大版本）
    let isBackwardCompatible (current: string option) (target: string option) =
        match parse current, parse target with
        | Some c, Some t -> c.Major = t.Major && compare c t >= 0
        | _ -> false

/// 设备能力推断（简单静态映射，可扩展为外部数据源）
module DeviceCapability =
    type Tier =
        | Entry
        | Mid
        | High
        | Ultra

    /// 基于品牌 + 型号/分辨率推断
    let inferTier (brand: string option) (model: string option) (resolutionClass: string) =
        let b = brand |> Option.map (fun s -> s.ToLowerInvariant())
        let m = model |> Option.map (fun s -> s.ToLowerInvariant())

        match b, m, resolutionClass with
        | Some "apple", Some mm, ("FHD" | "UHD") when mm.StartsWith("iphone 14") || mm.StartsWith("iphone 15") -> High
        | Some "apple", _, ("UHD") -> High
        | Some "apple", _, ("FHD") -> Mid
        | Some "huawei", _, ("UHD" | "FHD") -> Mid
        | Some "samsung", _, ("UHD") -> High
        | _, _, "UHD" -> High
        | _, _, "FHD" -> Mid
        | _, _, "HD" -> Mid
        | _ -> Entry

    /// 简单价格区间映射
    let priceBand (tier: Tier) =
        match tier with
        | Entry -> "Low"
        | Mid -> "Medium"
        | High -> "High"
        | Ultra -> "Premium"
