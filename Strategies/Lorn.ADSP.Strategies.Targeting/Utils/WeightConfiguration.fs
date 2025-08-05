namespace Lorn.ADSP.Strategies.Targeting.Utils

open System
open System.Collections.Generic
open Lorn.ADSP.Core.Domain.ValueObjects

/// <summary>
/// 属性权重配置和动态评分机制模块
/// 提供人口属性权重管理和动态评分计算功能
/// </summary>
module WeightConfiguration =

    /// <summary>
    /// 属性权重配置
    /// </summary>
    type AttributeWeightConfig =
        { AttributeName: string
          BaseWeight: decimal
          DataSourceWeights: Map<string, decimal>
          QualityFactors: Map<string, decimal>
          TimeDecayFactor: decimal
          MinWeight: decimal
          MaxWeight: decimal
          Description: string }

    /// <summary>
    /// 动态评分上下文
    /// </summary>
    type ScoringContext =
        { AttributeName: string
          AttributeValue: string
          DataSource: string
          CreatedAt: DateTime
          LastUpdatedAt: DateTime
          AccessCount: int
          ValidationScore: decimal
          UserConfidence: decimal option }

    /// <summary>
    /// 评分结果
    /// </summary>
    type ScoringResult =
        { FinalWeight: decimal
          BaseWeight: decimal
          DataSourceAdjustment: decimal
          QualityAdjustment: decimal
          TimeDecayAdjustment: decimal
          ConfidenceAdjustment: decimal
          Explanation: string list }

    /// <summary>
    /// 默认属性权重配置
    /// </summary>
    let getDefaultWeightConfiguration () : Map<string, AttributeWeightConfig> =
        let configs =
            [
              // 年龄权重配置
              ("Age",
               { AttributeName = "Age"
                 BaseWeight = 1.0m
                 DataSourceWeights =
                   Map.ofList
                       [ ("UserInput", 1.2m)
                         ("ThirdParty", 1.0m)
                         ("Inference", 0.6m)
                         ("System", 0.8m) ]
                 QualityFactors = Map.ofList [ ("Validated", 1.1m); ("Estimated", 0.9m); ("Outdated", 0.7m) ]
                 TimeDecayFactor = 0.95m // 每天衰减5%
                 MinWeight = 0.1m
                 MaxWeight = 2.0m
                 Description = "年龄属性权重配置，考虑数据来源和时效性" })

              // 性别权重配置
              ("Gender",
               { AttributeName = "Gender"
                 BaseWeight = 1.2m
                 DataSourceWeights =
                   Map.ofList
                       [ ("UserInput", 1.3m)
                         ("ThirdParty", 1.0m)
                         ("Inference", 0.5m)
                         ("System", 0.7m) ]
                 QualityFactors = Map.ofList [ ("Validated", 1.2m); ("Estimated", 0.8m); ("Outdated", 0.6m) ]
                 TimeDecayFactor = 0.99m // 性别相对稳定，衰减较慢
                 MinWeight = 0.2m
                 MaxWeight = 2.5m
                 Description = "性别属性权重配置，重要性较高" })

              // 教育程度权重配置
              ("Education",
               { AttributeName = "Education"
                 BaseWeight = 0.9m
                 DataSourceWeights =
                   Map.ofList
                       [ ("UserInput", 1.1m)
                         ("ThirdParty", 1.0m)
                         ("Inference", 0.7m)
                         ("System", 0.8m) ]
                 QualityFactors = Map.ofList [ ("Validated", 1.1m); ("Estimated", 0.9m); ("Outdated", 0.8m) ]
                 TimeDecayFactor = 0.98m // 教育程度相对稳定
                 MinWeight = 0.1m
                 MaxWeight = 1.8m
                 Description = "教育程度属性权重配置" })

              // 职业权重配置
              ("Occupation",
               { AttributeName = "Occupation"
                 BaseWeight = 1.1m
                 DataSourceWeights =
                   Map.ofList
                       [ ("UserInput", 1.2m)
                         ("ThirdParty", 1.0m)
                         ("Inference", 0.6m)
                         ("System", 0.8m) ]
                 QualityFactors = Map.ofList [ ("Validated", 1.1m); ("Estimated", 0.9m); ("Outdated", 0.7m) ]
                 TimeDecayFactor = 0.96m // 职业变化相对较快
                 MinWeight = 0.1m
                 MaxWeight = 2.2m
                 Description = "职业属性权重配置，考虑变化频率" })

              // 收入权重配置
              ("Income",
               { AttributeName = "Income"
                 BaseWeight = 1.3m
                 DataSourceWeights =
                   Map.ofList
                       [ ("UserInput", 1.1m) // 用户输入收入可能不准确
                         ("ThirdParty", 1.2m)
                         ("Inference", 0.5m)
                         ("System", 0.7m) ]
                 QualityFactors = Map.ofList [ ("Validated", 1.2m); ("Estimated", 0.8m); ("Outdated", 0.6m) ]
                 TimeDecayFactor = 0.93m // 收入变化较快
                 MinWeight = 0.1m
                 MaxWeight = 2.5m
                 Description = "收入属性权重配置，重要性高但变化快" })

              // 婚恋状况权重配置
              ("MaritalStatus",
               { AttributeName = "MaritalStatus"
                 BaseWeight = 0.8m
                 DataSourceWeights =
                   Map.ofList
                       [ ("UserInput", 1.2m)
                         ("ThirdParty", 1.0m)
                         ("Inference", 0.6m)
                         ("System", 0.8m) ]
                 QualityFactors = Map.ofList [ ("Validated", 1.1m); ("Estimated", 0.9m); ("Outdated", 0.8m) ]
                 TimeDecayFactor = 0.97m // 婚恋状况变化中等
                 MinWeight = 0.1m
                 MaxWeight = 1.5m
                 Description = "婚恋状况属性权重配置" }) ]

        Map.ofList configs

    /// <summary>
    /// 计算时间衰减因子
    /// </summary>
    let calculateTimeDecay (createdAt: DateTime) (timeDecayFactor: decimal) : decimal =
        let daysSinceCreation = (DateTime.UtcNow - createdAt).TotalDays
        let decayFactor = Math.Pow(float timeDecayFactor, daysSinceCreation)
        Math.Max(0.1, Math.Min(1.0, decayFactor)) |> decimal

    /// <summary>
    /// 计算数据源权重调整
    /// </summary>
    let calculateDataSourceAdjustment (dataSource: string) (dataSourceWeights: Map<string, decimal>) : decimal =
        match dataSourceWeights.TryFind dataSource with
        | Some weight -> weight
        | None -> 1.0m // 默认权重

    /// <summary>
    /// 计算质量因子调整
    /// </summary>
    let calculateQualityAdjustment (validationScore: decimal) (qualityFactors: Map<string, decimal>) : decimal =
        let qualityLevel =
            match validationScore with
            | score when score >= 0.9m -> "Validated"
            | score when score >= 0.7m -> "Estimated"
            | _ -> "Outdated"

        match qualityFactors.TryFind qualityLevel with
        | Some factor -> factor
        | None -> 1.0m

    /// <summary>
    /// 计算用户置信度调整
    /// </summary>
    let calculateConfidenceAdjustment (userConfidence: decimal option) : decimal =
        match userConfidence with
        | Some confidence when confidence >= 0.8m -> 1.1m
        | Some confidence when confidence >= 0.6m -> 1.0m
        | Some confidence when confidence >= 0.4m -> 0.9m
        | Some _ -> 0.8m
        | None -> 1.0m

    /// <summary>
    /// 执行动态评分计算
    /// </summary>
    let calculateDynamicScore (config: AttributeWeightConfig) (context: ScoringContext) : ScoringResult =
        let explanations = ResizeArray<string>()

        // 1. 基础权重
        let baseWeight = config.BaseWeight
        explanations.Add(sprintf "基础权重: %M" baseWeight)

        // 2. 数据源调整
        let dataSourceAdjustment =
            calculateDataSourceAdjustment context.DataSource config.DataSourceWeights

        explanations.Add(sprintf "数据源调整 (%s): %M" context.DataSource dataSourceAdjustment)

        // 3. 质量调整
        let qualityAdjustment =
            calculateQualityAdjustment context.ValidationScore config.QualityFactors

        explanations.Add(sprintf "质量调整 (验证分数: %M): %M" context.ValidationScore qualityAdjustment)

        // 4. 时间衰减调整
        let timeDecayAdjustment =
            calculateTimeDecay context.CreatedAt config.TimeDecayFactor

        explanations.Add(sprintf "时间衰减调整 (创建于: %s): %M" (context.CreatedAt.ToString("yyyy-MM-dd")) timeDecayAdjustment)

        // 5. 用户置信度调整
        let confidenceAdjustment = calculateConfidenceAdjustment context.UserConfidence

        match context.UserConfidence with
        | Some conf -> explanations.Add(sprintf "用户置信度调整: %M (置信度: %M)" confidenceAdjustment conf)
        | None -> explanations.Add(sprintf "用户置信度调整: %M (无置信度数据)" confidenceAdjustment)

        // 6. 计算最终权重
        let rawFinalWeight =
            baseWeight
            * dataSourceAdjustment
            * qualityAdjustment
            * timeDecayAdjustment
            * confidenceAdjustment

        let finalWeight =
            Math.Max(float config.MinWeight, Math.Min(float config.MaxWeight, float rawFinalWeight))
            |> decimal

        explanations.Add(sprintf "最终权重: %M (限制在 %M - %M 之间)" finalWeight config.MinWeight config.MaxWeight)

        { FinalWeight = finalWeight
          BaseWeight = baseWeight
          DataSourceAdjustment = dataSourceAdjustment
          QualityAdjustment = qualityAdjustment
          TimeDecayAdjustment = timeDecayAdjustment
          ConfidenceAdjustment = confidenceAdjustment
          Explanation = List.ofSeq explanations }

    /// <summary>
    /// 批量计算属性权重
    /// </summary>
    let calculateBatchWeights (properties: ContextProperty list) : Map<string, ScoringResult> =
        let weightConfigs = getDefaultWeightConfiguration ()
        let results = Dictionary<string, ScoringResult>()

        for property in properties do
            match weightConfigs.TryFind property.PropertyKey with
            | Some config ->
                let context =
                    { AttributeName = property.PropertyKey
                      AttributeValue = property.PropertyValue
                      DataSource = property.DataSource
                      CreatedAt = DateTime.UtcNow.AddDays(-1.0) // 假设创建时间
                      LastUpdatedAt = DateTime.UtcNow
                      AccessCount = 1
                      ValidationScore = if property.DataSource = "Inference" then 0.6m else 0.9m
                      UserConfidence = None }

                let scoringResult = calculateDynamicScore config context
                results.Add(property.PropertyKey, scoringResult)
            | None ->
                // 使用默认权重
                let defaultResult =
                    { FinalWeight = property.Weight
                      BaseWeight = property.Weight
                      DataSourceAdjustment = 1.0m
                      QualityAdjustment = 1.0m
                      TimeDecayAdjustment = 1.0m
                      ConfidenceAdjustment = 1.0m
                      Explanation = [ sprintf "使用属性默认权重: %M" property.Weight ] }

                results.Add(property.PropertyKey, defaultResult)

        Map.ofSeq (results |> Seq.map (fun kvp -> kvp.Key, kvp.Value))

    /// <summary>
    /// 更新属性权重配置
    /// </summary>
    let updateWeightConfiguration
        (attributeName: string)
        (newConfig: AttributeWeightConfig)
        : Map<string, AttributeWeightConfig> =
        let currentConfigs = getDefaultWeightConfiguration ()
        currentConfigs.Add(attributeName, newConfig)

    /// <summary>
    /// 获取属性权重统计信息
    /// </summary>
    let getWeightStatistics (scoringResults: Map<string, ScoringResult>) : Map<string, obj> =
        let stats = Dictionary<string, obj>()

        let weights =
            scoringResults |> Map.toList |> List.map (fun (_, result) -> result.FinalWeight)

        if not (List.isEmpty weights) then
            stats.Add("TotalAttributes", weights.Length)
            stats.Add("AverageWeight", List.average weights)
            stats.Add("MinWeight", List.min weights)
            stats.Add("MaxWeight", List.max weights)
            stats.Add("WeightSum", List.sum weights)

            // 按数据源分组统计
            let sourceGroups =
                scoringResults
                |> Map.toList
                |> List.groupBy (fun (_, result) ->
                    if result.DataSourceAdjustment > 1.0m then "High"
                    elif result.DataSourceAdjustment < 1.0m then "Low"
                    else "Normal")

            for (sourceType, group) in sourceGroups do
                stats.Add(sprintf "Count_%sSource" sourceType, group.Length)

        Map.ofSeq (stats |> Seq.map (fun kvp -> kvp.Key, kvp.Value))

    /// <summary>
    /// 验证权重配置的合理性
    /// </summary>
    let validateWeightConfiguration (config: AttributeWeightConfig) : bool * string list =
        let errors = ResizeArray<string>()

        // 检查基础权重范围
        if config.BaseWeight < 0.0m || config.BaseWeight > 5.0m then
            errors.Add(sprintf "基础权重 %M 超出合理范围 (0.0-5.0)" config.BaseWeight)

        // 检查最小最大权重
        if config.MinWeight >= config.MaxWeight then
            errors.Add(sprintf "最小权重 %M 必须小于最大权重 %M" config.MinWeight config.MaxWeight)

        // 检查时间衰减因子
        if config.TimeDecayFactor < 0.5m || config.TimeDecayFactor > 1.0m then
            errors.Add(sprintf "时间衰减因子 %M 超出合理范围 (0.5-1.0)" config.TimeDecayFactor)

        // 检查数据源权重
        for kvp in config.DataSourceWeights do
            if kvp.Value < 0.1m || kvp.Value > 3.0m then
                errors.Add(sprintf "数据源权重 %s:%M 超出合理范围 (0.1-3.0)" kvp.Key kvp.Value)

        (errors.Count = 0, List.ofSeq errors)

    /// <summary>
    /// 获取模块版本信息
    /// </summary>
    let getModuleVersion () = "1.0.0"
