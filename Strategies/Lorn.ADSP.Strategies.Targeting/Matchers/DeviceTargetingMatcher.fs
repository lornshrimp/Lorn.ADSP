namespace Lorn.ADSP.Strategies.Targeting.Matchers

/// <summary>
/// DeviceTargetingMatcher
/// 实现多属性(操作系统/品牌/型号/浏览器/网络/运营商/分辨率能力/版本)并行匹配与评分。
/// 特性:
/// 1. 关键属性(OS)快速失败降低无效计算 (需求7.1性能, 7.2并发, 4.1-4.6).
/// 2. 设备品牌 & 型号标准化 (品牌别名映射, 型号清洗) —— 减少规则配置冗余.
/// 3. 语义化版本比较 + 向后兼容检查 (MinOSVersion / TargetOSVersion).
/// 4. 基于分辨率与品牌/型号的能力(Tier)推断, 支持 CapabilityTiers 维度.
/// 5. 评分 = 匹配成功的激活维度 / 激活维度总数; 所有激活维度全部匹配才返回 IsMatch=true.
/// 6. 结构与 Time/Demographic 匹配器保持一致, 便于统一聚合.
/// 未来可扩展:
///   - GPU/CPU 基准库接入形成精细性能档位
///   - 实际价格 API/离线表推断价格区间与付费能力
///   - 通过设备指纹缓存标准化结果 (提高重复请求命中率)
///   - 批量匹配接口: 预标准化 criteria 后对多 context 复用
///   - 规则热更新后清空标准化缓存.
/// </summary>

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.AdEngine.Abstractions.Models
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.Shared.Entities
open Lorn.ADSP.Core.Shared.Enums
open Lorn.ADSP.Strategies.Targeting.Utils
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.AdEngine.Abstractions.Models

// 内部匹配项结果
[<CLIMutable>]
type DeviceAttrMatch =
    { Name: string
      HasCondition: bool
      IsMatch: bool }

// 内部执行结果
[<CLIMutable>]
type DeviceExecResult =
    { IsMatch: bool
      Score: decimal
      MatchDetails: IReadOnlyList<ContextProperty>
      NotMatched: string list }

/// 设备定向匹配器 需求覆盖: 4.1-4.6, 7.1(性能-快速失败),7.2(并发特征),13.1(可聚合)
type DeviceTargetingMatcher() =
    interface ITargetingMatcher with
        member _.MatcherId = "device-matcher-v1"
        member _.MatcherName = "设备定向匹配器"
        member _.Version = "1.0.0"
        member _.MatcherType = "Device"
        member _.Priority = 110
        member _.IsEnabled = true
        member _.ExpectedExecutionTime = TimeSpan.FromMilliseconds 6.
        member _.CanRunInParallel = true

        member this.IsSupported(t: string) =
            [ "Device"; "DeviceInfo"; "UserDevice" ] |> List.contains t

        member this.ValidateCriteria(criteria) =
            try
                if isNull (box criteria) then
                    ValidationResult.Failure([ ValidationError(Message = "定向条件不能为空") ], "验证失败", TimeSpan.Zero)
                elif not ((this :> ITargetingMatcher).IsSupported(criteria.CriteriaType)) then
                    let msg = sprintf "不支持的定向条件类型: %s" criteria.CriteriaType
                    ValidationResult.Failure([ ValidationError(Message = msg) ], "验证失败", TimeSpan.Zero)
                else
                    let hasRule names = names |> List.exists criteria.HasRule

                    if
                        not (
                            hasRule
                                [ "DeviceTypes"
                                  "OperatingSystems"
                                  "Browsers"
                                  "Brands"
                                  "Models"
                                  "MinOSVersion"
                                  "RequiredCapabilities" ]
                        )
                    then
                        ValidationResult.Failure([ ValidationError(Message = "至少配置一个设备定向条件") ], "验证失败", TimeSpan.Zero)
                    else
                        ValidationResult.Success("设备定向条件验证通过", TimeSpan.Zero)
            with ex ->
                let msg = sprintf "验证异常: %s" ex.Message
                ValidationResult.Failure([ ValidationError(Message = msg) ], "验证异常", TimeSpan.Zero)

        member this.CalculateMatchScoreAsync
            (context: ITargetingContext, criteria: ITargetingCriteria, _callbackProvider, _ct: CancellationToken)
            : Task<MatchResult> =
            Task.Run(fun () ->
                let startUtc = DateTime.UtcNow

                try
                    if isNull (box criteria) then
                        MatchResult.CreateNoMatch("Device", Guid.NewGuid(), "定向条件为空", TimeSpan.Zero, 0, 0m, false)
                    elif isNull (box context) then
                        MatchResult.CreateNoMatch(
                            criteria.CriteriaType,
                            criteria.CriteriaId,
                            "用户上下文为空",
                            TimeSpan.Zero,
                            0,
                            0m,
                            false
                        )
                    elif not ((this :> ITargetingMatcher).IsSupported(criteria.CriteriaType)) then
                        let msg = sprintf "不支持的定向条件类型: %s" criteria.CriteriaType

                        MatchResult.CreateNoMatch(
                            criteria.CriteriaType,
                            criteria.CriteriaId,
                            msg,
                            TimeSpan.Zero,
                            0,
                            0m,
                            false
                        )
                    else
                        let exec = this.ExecuteLogic(context, criteria)
                        let elapsed = DateTime.UtcNow - startUtc

                        if exec.IsMatch then
                            MatchResult.CreateMatch(
                                criteria.CriteriaType,
                                criteria.CriteriaId,
                                exec.Score,
                                "设备属性匹配",
                                elapsed,
                                0,
                                criteria.Weight,
                                false,
                                exec.MatchDetails
                            )
                        else
                            let reason =
                                if exec.NotMatched.Length > 0 then
                                    sprintf "未匹配属性: %s" (String.Join(", ", exec.NotMatched))
                                else
                                    "存在未匹配的设备条件"

                            MatchResult.CreateNoMatch(
                                criteria.CriteriaType,
                                criteria.CriteriaId,
                                reason,
                                elapsed,
                                0,
                                criteria.Weight,
                                false,
                                exec.MatchDetails
                            )
                with ex ->
                    let elapsed = DateTime.UtcNow - startUtc
                    let msg = sprintf "匹配异常: %s" ex.Message
                    MatchResult.CreateNoMatch("Device", criteria.CriteriaId, msg, elapsed, 0, 0m, false))

        member _.GetMetadata() =
            TargetingMatcherMetadata(
                MatcherId = "device-matcher-v1",
                Name = "设备定向匹配器",
                Description = "支持操作系统/品牌/型号/网络/屏幕/运营商/版本/能力的复合匹配",
                Version = "1.0.0",
                MatcherType = "Device",
                Author = "Lorn.ADSP Team",
                SupportedCriteriaTypes = [ "Device"; "DeviceInfo"; "UserDevice" ],
                SupportedDimensions =
                    [ "OS"
                      "Brand"
                      "Model"
                      "Browser"
                      "DeviceType"
                      "OSVersion"
                      "Resolution"
                      "Network"
                      "Carrier"
                      "CapabilityTier" ],
                SupportsParallelExecution = true,
                SupportsCaching = true,
                SupportsBatchProcessing = true,
                ExpectedExecutionTime = TimeSpan.FromMilliseconds 6.,
                MaxExecutionTime = TimeSpan.FromMilliseconds 60.
            )

    // -------- 内部逻辑实现 --------
    member private _.GetList<'T> (criteria: ITargetingCriteria) name : List<'T> =
        let v = criteria.GetRule<List<'T>>(name)
        if isNull v then List<'T>() else v

    member private _.TryGet<'T> (criteria: ITargetingCriteria) name : 'T option =
        try
            let v = criteria.GetRule<'T>(name)
            if obj.ReferenceEquals(v, null) then None else Some v
        with _ ->
            None

    member private this.ExecuteLogic(context: ITargetingContext, criteria: ITargetingCriteria) : DeviceExecResult =
        // 单表达式结构：依次获取上下文 -> 规则 -> 快速失败 -> 逐属性匹配 -> 评分 -> 返回记录
        let getCtx name =
            if context.HasProperty name then
                Some(context.GetPropertyAsString name)
            else
                None

        let os = getCtx "OperatingSystem" |> Option.map (fun s -> s.ToLowerInvariant())
        let osVersion = getCtx "OSVersion"
        let brand = getCtx "Brand" |> DeviceNormalization.normalizeBrand
        let model = getCtx "Model" |> DeviceNormalization.normalizeModel
        let networkType = getCtx "NetworkType" |> Option.map (fun s -> s.ToLowerInvariant())
        let carrier = getCtx "Carrier" |> Option.map (fun s -> s.ToLowerInvariant())
        let browser = getCtx "Browser" |> Option.map (fun s -> s.ToLowerInvariant())

        let deviceType =
            if context.HasProperty "DeviceType" then
                context.GetPropertyAsString "DeviceType"
            else
                null

        let screenWidth =
            if context.HasProperty "ScreenWidth" then
                Some(context.GetPropertyValue<int>("ScreenWidth"))
            else
                None

        let screenHeight =
            if context.HasProperty "ScreenHeight" then
                Some(context.GetPropertyValue<int>("ScreenHeight"))
            else
                None

        let resolutionClass =
            DeviceNormalization.classifyResolution screenWidth screenHeight

        let tier = DeviceCapability.inferTier brand model resolutionClass
        let tierName = tier.ToString()

        // 规则集合 (全部转小写便于不区分大小写匹配)
        let toSet (lst: List<string>) =
            lst
            |> Seq.choose (fun s ->
                if String.IsNullOrWhiteSpace s then
                    None
                else
                    Some(s.ToLowerInvariant()))
            |> Set.ofSeq

        let osRules = this.GetList<string> criteria "OperatingSystems" |> toSet
        let brandRules = this.GetList<string> criteria "Brands" |> toSet
        let modelRules = this.GetList<string> criteria "Models" |> toSet
        let browserRules = this.GetList<string> criteria "Browsers" |> toSet
        let deviceTypeRules = this.GetList<string> criteria "DeviceTypes" |> toSet
        let networkRules = this.GetList<string> criteria "NetworkTypes" |> toSet
        let carrierRules = this.GetList<string> criteria "Carriers" |> toSet
        let tierRules = this.GetList<string> criteria "CapabilityTiers" |> toSet
        let minOSVersion = this.TryGet<string> criteria "MinOSVersion"
        let targetOSVersion = this.TryGet<string> criteria "TargetOSVersion"

        // 快速失败: OS 不匹配直接退出
        let quickFail =
            match osRules.Count > 0, os with
            | true, None -> Some "Unknown"
            | true, Some v when not (osRules.Contains v) -> Some v
            | _ -> None

        match quickFail with
        | Some osv ->
            let detail =
                [ ContextProperty("OS", osv, "String", "DeviceMatch", false, 1.0m, Nullable(), "DeviceTargetingMatcher") ]
                :> IReadOnlyList<_>

            { IsMatch = false
              Score = 0m
              MatchDetails = detail
              NotMatched = [ "OS" ] }
        | None ->
            let attr name hasCond matched =
                { Name = name
                  HasCondition = hasCond
                  IsMatch = matched }

            let osMatch = attr "OS" (osRules.Count > 0) true

            let brandMatch =
                if brandRules.Count = 0 then
                    attr "Brand" false true
                else
                    match brand with
                    | Some b -> attr "Brand" true (brandRules.Contains(b.ToLowerInvariant()))
                    | None -> attr "Brand" true false

            let modelMatch =
                if modelRules.Count = 0 then
                    attr "Model" false true
                else
                    match model with
                    | Some m -> attr "Model" true (modelRules.Contains(m.ToLowerInvariant()))
                    | None -> attr "Model" true false

            let browserMatch =
                if browserRules.Count = 0 then
                    attr "Browser" false true
                else
                    match browser with
                    | Some b -> attr "Browser" true (browserRules.Contains b)
                    | None -> attr "Browser" true false

            let deviceTypeMatch =
                if deviceTypeRules.Count = 0 then
                    attr "DeviceType" false true
                elif isNull deviceType then
                    attr "DeviceType" true false
                else
                    attr "DeviceType" true (deviceTypeRules.Contains(deviceType.ToLowerInvariant()))

            let networkMatch =
                if networkRules.Count = 0 then
                    attr "Network" false true
                else
                    match networkType with
                    | Some n -> attr "Network" true (networkRules.Contains n)
                    | None -> attr "Network" true false

            let carrierMatch =
                if carrierRules.Count = 0 then
                    attr "Carrier" false true
                else
                    match carrier with
                    | Some c -> attr "Carrier" true (carrierRules.Contains c)
                    | None -> attr "Carrier" true false

            let tierMatch =
                if tierRules.Count = 0 then
                    attr "CapabilityTier" false true
                else
                    attr "CapabilityTier" true (tierRules.Contains(tierName.ToLowerInvariant()))

            let versionAttr =
                let hasCond = minOSVersion.IsSome || targetOSVersion.IsSome
                let minOk = VersionSemver.satisfiesMin osVersion minOSVersion

                let backwardOk =
                    match targetOSVersion with
                    | None -> true
                    | Some t -> VersionSemver.isBackwardCompatible osVersion (Some t)

                attr "OSVersion" hasCond (minOk && backwardOk)

            let allAttrs =
                [ osMatch
                  brandMatch
                  modelMatch
                  browserMatch
                  deviceTypeMatch
                  networkMatch
                  carrierMatch
                  tierMatch
                  versionAttr ]

            let active = allAttrs |> List.filter (fun a -> a.HasCondition)
            let matched = active |> List.filter (fun a -> a.IsMatch)

            let score =
                if active.IsEmpty then
                    0m
                else
                    decimal matched.Length / decimal active.Length

            let allOk = (not active.IsEmpty) && matched.Length = active.Length

            let details: IReadOnlyList<ContextProperty> =
                allAttrs
                |> List.filter (fun a -> a.HasCondition)
                |> List.map (fun a ->
                    let status = if a.IsMatch then "匹配" else "不匹配"

                    ContextProperty(
                        a.Name,
                        status,
                        "String",
                        "DeviceMatch",
                        false,
                        1.0m,
                        Nullable(),
                        "DeviceTargetingMatcher"
                    ))
                |> List.toArray
                :> _

            let notMatched =
                active |> List.filter (fun a -> not a.IsMatch) |> List.map (fun a -> a.Name)

            { IsMatch = allOk
              Score = score
              MatchDetails = details
              NotMatched = notMatched }
