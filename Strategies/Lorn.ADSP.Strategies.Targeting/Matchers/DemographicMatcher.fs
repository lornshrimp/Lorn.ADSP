namespace Lorn.ADSP.Strategies.Targeting.Matchers

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open FSharp.Control
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Domain.Targeting
open Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces
open Lorn.ADSP.Core.Shared.Enums
open Lorn.ADSP.Strategies.Targeting.Core.MatchingAlgorithms
// Utils modules will be referenced by full module name

/// <summary>
/// 人口属性定向匹配器
/// 实现年龄、性别、教育程度、职业、收入、婚恋状况的并行匹配逻辑
/// 使用F#模式匹配处理不同属性类型的分支逻辑
/// </summary>
module DemographicMatcher =

    // 使用 Core.Shared 中定义的枚举类型
    // EducationLevel, MaritalStatus, OccupationType, IncomeLevel 已在 Core.Shared.Enums 中定义

    /// <summary>
    /// 人口属性匹配上下文
    /// </summary>
    type DemographicContext =
        { Age: int option
          Gender: Gender option
          Education: Lorn.ADSP.Core.Shared.Enums.EducationLevel option
          Occupation: Lorn.ADSP.Core.Shared.Enums.OccupationType option
          Income: Lorn.ADSP.Core.Shared.Enums.IncomeLevel option
          MaritalStatus: Lorn.ADSP.Core.Shared.Enums.MaritalStatus option }

    /// <summary>
    /// 人口属性匹配条件
    /// </summary>
    type DemographicCriteria =
        { AgeRange: (int * int) option
          TargetGenders: Gender list
          TargetEducations: Lorn.ADSP.Core.Shared.Enums.EducationLevel list
          TargetOccupations: Lorn.ADSP.Core.Shared.Enums.OccupationType list
          TargetIncomes: Lorn.ADSP.Core.Shared.Enums.IncomeLevel list
          TargetMaritalStatuses: Lorn.ADSP.Core.Shared.Enums.MaritalStatus list
          RequireAllMatch: bool }

    /// <summary>
    /// 单个属性匹配结果
    /// </summary>
    type AttributeMatchResult =
        { AttributeName: string
          IsMatch: bool
          Score: decimal
          Reason: string
          Weight: decimal }

    /// <summary>
    /// 从用户画像中提取人口属性信息（增强版，包含数据验证和推断）
    /// </summary>
    let extractDemographicContext (userProfile: ITargetingContext) : DemographicContext =
        let demographics = userProfile.GetPropertiesByCategory("Demographics") |> List.ofSeq

        // 应用数据验证和推断规则
        let (enhancedProperties, completenessResult) =
            Lorn.ADSP.Strategies.Targeting.Utils.DataValidation.validateAndInferDemographicData demographics

        let getProperty (key: string) (parseFunc: string -> 'T option) =
            enhancedProperties
            |> List.tryFind (fun p -> p.PropertyKey = key)
            |> Option.bind (fun p ->
                if String.IsNullOrEmpty(p.PropertyValue) then
                    None
                else
                    parseFunc p.PropertyValue)

        let parseAge (value: string) =
            match Int32.TryParse(value) with
            | (true, age) when age > 0 && age < 150 -> Some age
            | _ -> None

        let parseGender (value: string) =
            let mutable result = Gender.Unknown

            if Enum.TryParse<Gender>(value, true, &result) then
                Some result
            else
                None

        let parseEducation (value: string) =
            let mutable result = Lorn.ADSP.Core.Shared.Enums.EducationLevel.Unknown

            if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.EducationLevel>(value, true, &result) then
                Some result
            else
                None

        let parseOccupation (value: string) =
            let mutable result = Lorn.ADSP.Core.Shared.Enums.OccupationType.Unknown

            if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.OccupationType>(value, true, &result) then
                Some result
            else
                None

        let parseIncome (value: string) =
            let mutable result = Lorn.ADSP.Core.Shared.Enums.IncomeLevel.Unknown

            if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.IncomeLevel>(value, true, &result) then
                Some result
            else
                None

        let parseMaritalStatus (value: string) =
            let mutable result = Lorn.ADSP.Core.Shared.Enums.MaritalStatus.Unknown

            if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.MaritalStatus>(value, true, &result) then
                Some result
            else
                None

        { Age = getProperty "Age" parseAge
          Gender = getProperty "Gender" parseGender
          Education = getProperty "Education" parseEducation
          Occupation = getProperty "Occupation" parseOccupation
          Income = getProperty "Income" parseIncome
          MaritalStatus = getProperty "MaritalStatus" parseMaritalStatus }

    /// <summary>
    /// 从定向条件中提取人口属性匹配条件
    /// </summary>
    let extractDemographicCriteria (criteria: ITargetingCriteria) : DemographicCriteria =
        let parseAgeRange (value: string) =
            match value.Split('-') with
            | [| minStr; maxStr |] ->
                match Int32.TryParse(minStr), Int32.TryParse(maxStr) with
                | (true, minAge), (true, maxAge) when minAge <= maxAge -> Some(minAge, maxAge)
                | _ -> None
            | _ -> None

        let parseGenderList (value: string) =
            value.Split(',')
            |> Array.map (fun s -> s.Trim())
            |> Array.choose (fun s ->
                let mutable result = Gender.Unknown

                if Enum.TryParse<Gender>(s, true, &result) then
                    Some result
                else
                    None)
            |> List.ofArray

        let parseEducationList (value: string) =
            value.Split(',')
            |> Array.map (fun s -> s.Trim())
            |> Array.choose (fun s ->
                let mutable result = Lorn.ADSP.Core.Shared.Enums.EducationLevel.Unknown

                if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.EducationLevel>(s, true, &result) then
                    Some result
                else
                    None)
            |> List.ofArray

        let parseOccupationList (value: string) =
            value.Split(',')
            |> Array.map (fun s -> s.Trim())
            |> Array.choose (fun s ->
                let mutable result = Lorn.ADSP.Core.Shared.Enums.OccupationType.Unknown

                if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.OccupationType>(s, true, &result) then
                    Some result
                else
                    None)
            |> List.ofArray

        let parseIncomeList (value: string) =
            value.Split(',')
            |> Array.map (fun s -> s.Trim())
            |> Array.choose (fun s ->
                let mutable result = Lorn.ADSP.Core.Shared.Enums.IncomeLevel.Unknown

                if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.IncomeLevel>(s, true, &result) then
                    Some result
                else
                    None)
            |> List.ofArray

        let parseMaritalStatusList (value: string) =
            value.Split(',')
            |> Array.map (fun s -> s.Trim())
            |> Array.choose (fun s ->
                let mutable result = Lorn.ADSP.Core.Shared.Enums.MaritalStatus.Unknown

                if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.MaritalStatus>(s, true, &result) then
                    Some result
                else
                    None)
            |> List.ofArray

        { AgeRange =
            criteria.GetRule<string>("AgeRange")
            |> Option.ofObj
            |> Option.bind parseAgeRange

          TargetGenders =
            criteria.GetRule<string>("Gender")
            |> Option.ofObj
            |> Option.map parseGenderList
            |> Option.defaultValue []

          TargetEducations =
            criteria.GetRule<string>("Education")
            |> Option.ofObj
            |> Option.map parseEducationList
            |> Option.defaultValue []

          TargetOccupations =
            criteria.GetRule<string>("Occupation")
            |> Option.ofObj
            |> Option.map parseOccupationList
            |> Option.defaultValue []

          TargetIncomes =
            criteria.GetRule<string>("Income")
            |> Option.ofObj
            |> Option.map parseIncomeList
            |> Option.defaultValue []

          TargetMaritalStatuses =
            criteria.GetRule<string>("MaritalStatus")
            |> Option.ofObj
            |> Option.map parseMaritalStatusList
            |> Option.defaultValue []

          RequireAllMatch = criteria.GetRule<bool>("RequireAllMatch", false) }

    /// <summary>
    /// 年龄匹配逻辑（增强版，支持动态权重）
    /// </summary>
    let matchAge (userAge: int option) (ageRange: (int * int) option) (dynamicWeight: decimal) : AttributeMatchResult =
        match userAge, ageRange with
        | Some age, Some(minAge, maxAge) ->
            let isMatch = age >= minAge && age <= maxAge
            let score = if isMatch then 1.0m else 0.0m

            let reason =
                if isMatch then
                    $"用户年龄 {age} 在目标范围 {minAge}-{maxAge} 内"
                else
                    $"用户年龄 {age} 不在目标范围 {minAge}-{maxAge} 内"

            { AttributeName = "Age"
              IsMatch = isMatch
              Score = score
              Reason = reason
              Weight = dynamicWeight }

        | None, Some(minAge, maxAge) ->
            { AttributeName = "Age"
              IsMatch = false
              Score = 0.0m
              Reason = $"用户年龄信息缺失，无法匹配目标范围 {minAge}-{maxAge}"
              Weight = dynamicWeight }

        | Some age, None ->
            { AttributeName = "Age"
              IsMatch = true
              Score = 1.0m
              Reason = $"无年龄限制，用户年龄 {age} 通过匹配"
              Weight = dynamicWeight }

        | None, None ->
            { AttributeName = "Age"
              IsMatch = true
              Score = 1.0m
              Reason = "无年龄信息和限制，默认匹配"
              Weight = dynamicWeight }

    /// <summary>
    /// 性别匹配逻辑（增强版，支持动态权重）
    /// </summary>
    let matchGender
        (userGender: Gender option)
        (targetGenders: Gender list)
        (dynamicWeight: decimal)
        : AttributeMatchResult =
        match userGender, targetGenders with
        | Some gender, genders when not (List.isEmpty genders) ->
            let isMatch = List.contains gender genders
            let score = if isMatch then 1.0m else 0.0m
            let genderNames = genders |> List.map string |> String.concat ", "

            let reason =
                if isMatch then
                    $"用户性别 {gender} 在目标性别 [{genderNames}] 中"
                else
                    $"用户性别 {gender} 不在目标性别 [{genderNames}] 中"

            { AttributeName = "Gender"
              IsMatch = isMatch
              Score = score
              Reason = reason
              Weight = dynamicWeight }

        | None, genders when not (List.isEmpty genders) ->
            let genderNames = genders |> List.map string |> String.concat ", "

            { AttributeName = "Gender"
              IsMatch = false
              Score = 0.0m
              Reason = $"用户性别信息缺失，无法匹配目标性别 [{genderNames}]"
              Weight = dynamicWeight }

        | Some gender, [] ->
            { AttributeName = "Gender"
              IsMatch = true
              Score = 1.0m
              Reason = $"无性别限制，用户性别 {gender} 通过匹配"
              Weight = dynamicWeight }

        | None, [] ->
            { AttributeName = "Gender"
              IsMatch = true
              Score = 1.0m
              Reason = "无性别信息和限制，默认匹配"
              Weight = dynamicWeight }

    /// <summary>
    /// 教育程度匹配逻辑（增强版，支持动态权重）
    /// </summary>
    let matchEducation
        (userEducation: Lorn.ADSP.Core.Shared.Enums.EducationLevel option)
        (targetEducations: Lorn.ADSP.Core.Shared.Enums.EducationLevel list)
        (dynamicWeight: decimal)
        : AttributeMatchResult =
        match userEducation, targetEducations with
        | Some education, educations when not (List.isEmpty educations) ->
            let isMatch = List.contains education educations
            let score = if isMatch then 1.0m else 0.0m
            let educationNames = educations |> List.map string |> String.concat ", "

            let reason =
                if isMatch then
                    $"用户教育程度 {education} 在目标教育程度 [{educationNames}] 中"
                else
                    $"用户教育程度 {education} 不在目标教育程度 [{educationNames}] 中"

            { AttributeName = "Education"
              IsMatch = isMatch
              Score = score
              Reason = reason
              Weight = dynamicWeight }

        | None, educations when not (List.isEmpty educations) ->
            let educationNames = educations |> List.map string |> String.concat ", "

            { AttributeName = "Education"
              IsMatch = false
              Score = 0.0m
              Reason = $"用户教育程度信息缺失，无法匹配目标教育程度 [{educationNames}]"
              Weight = dynamicWeight }

        | Some education, [] ->
            { AttributeName = "Education"
              IsMatch = true
              Score = 1.0m
              Reason = $"无教育程度限制，用户教育程度 {education} 通过匹配"
              Weight = dynamicWeight }

        | None, [] ->
            { AttributeName = "Education"
              IsMatch = true
              Score = 1.0m
              Reason = "无教育程度信息和限制，默认匹配"
              Weight = dynamicWeight }

    /// <summary>
    /// 职业匹配逻辑（增强版，支持动态权重）
    /// </summary>
    let matchOccupation
        (userOccupation: Lorn.ADSP.Core.Shared.Enums.OccupationType option)
        (targetOccupations: Lorn.ADSP.Core.Shared.Enums.OccupationType list)
        (dynamicWeight: decimal)
        : AttributeMatchResult =
        match userOccupation, targetOccupations with
        | Some occupation, occupations when not (List.isEmpty occupations) ->
            let isMatch = List.contains occupation occupations
            let score = if isMatch then 1.0m else 0.0m
            let occupationNames = occupations |> List.map string |> String.concat ", "

            let reason =
                if isMatch then
                    $"用户职业 {occupation} 在目标职业 [{occupationNames}] 中"
                else
                    $"用户职业 {occupation} 不在目标职业 [{occupationNames}] 中"

            { AttributeName = "Occupation"
              IsMatch = isMatch
              Score = score
              Reason = reason
              Weight = dynamicWeight }

        | None, occupations when not (List.isEmpty occupations) ->
            let occupationNames = occupations |> List.map string |> String.concat ", "

            { AttributeName = "Occupation"
              IsMatch = false
              Score = 0.0m
              Reason = $"用户职业信息缺失，无法匹配目标职业 [{occupationNames}]"
              Weight = dynamicWeight }

        | Some occupation, [] ->
            { AttributeName = "Occupation"
              IsMatch = true
              Score = 1.0m
              Reason = $"无职业限制，用户职业 {occupation} 通过匹配"
              Weight = dynamicWeight }

        | None, [] ->
            { AttributeName = "Occupation"
              IsMatch = true
              Score = 1.0m
              Reason = "无职业信息和限制，默认匹配"
              Weight = dynamicWeight }

    /// <summary>
    /// 收入匹配逻辑（增强版，支持动态权重）
    /// </summary>
    let matchIncome
        (userIncome: Lorn.ADSP.Core.Shared.Enums.IncomeLevel option)
        (targetIncomes: Lorn.ADSP.Core.Shared.Enums.IncomeLevel list)
        (dynamicWeight: decimal)
        : AttributeMatchResult =
        match userIncome, targetIncomes with
        | Some income, incomes when not (List.isEmpty incomes) ->
            let isMatch = List.contains income incomes
            let score = if isMatch then 1.0m else 0.0m
            let incomeNames = incomes |> List.map string |> String.concat ", "

            let reason =
                if isMatch then
                    $"用户收入水平 {income} 在目标收入水平 [{incomeNames}] 中"
                else
                    $"用户收入水平 {income} 不在目标收入水平 [{incomeNames}] 中"

            { AttributeName = "Income"
              IsMatch = isMatch
              Score = score
              Reason = reason
              Weight = dynamicWeight }

        | None, incomes when not (List.isEmpty incomes) ->
            let incomeNames = incomes |> List.map string |> String.concat ", "

            { AttributeName = "Income"
              IsMatch = false
              Score = 0.0m
              Reason = $"用户收入信息缺失，无法匹配目标收入水平 [{incomeNames}]"
              Weight = dynamicWeight }

        | Some income, [] ->
            { AttributeName = "Income"
              IsMatch = true
              Score = 1.0m
              Reason = $"无收入限制，用户收入水平 {income} 通过匹配"
              Weight = dynamicWeight }

        | None, [] ->
            { AttributeName = "Income"
              IsMatch = true
              Score = 1.0m
              Reason = "无收入信息和限制，默认匹配"
              Weight = dynamicWeight }

    /// <summary>
    /// 婚恋状况匹配逻辑（增强版，支持动态权重）
    /// </summary>
    let matchMaritalStatus
        (userMaritalStatus: Lorn.ADSP.Core.Shared.Enums.MaritalStatus option)
        (targetMaritalStatuses: Lorn.ADSP.Core.Shared.Enums.MaritalStatus list)
        (dynamicWeight: decimal)
        : AttributeMatchResult =
        match userMaritalStatus, targetMaritalStatuses with
        | Some maritalStatus, statuses when not (List.isEmpty statuses) ->
            let isMatch = List.contains maritalStatus statuses
            let score = if isMatch then 1.0m else 0.0m
            let statusNames = statuses |> List.map string |> String.concat ", "

            let reason =
                if isMatch then
                    $"用户婚恋状况 {maritalStatus} 在目标婚恋状况 [{statusNames}] 中"
                else
                    $"用户婚恋状况 {maritalStatus} 不在目标婚恋状况 [{statusNames}] 中"

            { AttributeName = "MaritalStatus"
              IsMatch = isMatch
              Score = score
              Reason = reason
              Weight = dynamicWeight }

        | None, statuses when not (List.isEmpty statuses) ->
            let statusNames = statuses |> List.map string |> String.concat ", "

            { AttributeName = "MaritalStatus"
              IsMatch = false
              Score = 0.0m
              Reason = $"用户婚恋状况信息缺失，无法匹配目标婚恋状况 [{statusNames}]"
              Weight = dynamicWeight }

        | Some maritalStatus, [] ->
            { AttributeName = "MaritalStatus"
              IsMatch = true
              Score = 1.0m
              Reason = $"无婚恋状况限制，用户婚恋状况 {maritalStatus} 通过匹配"
              Weight = dynamicWeight }

        | None, [] ->
            { AttributeName = "MaritalStatus"
              IsMatch = true
              Score = 1.0m
              Reason = "无婚恋状况信息和限制，默认匹配"
              Weight = dynamicWeight }

    /// <summary>
    /// 并行执行所有人口属性匹配（增强版，支持动态权重和缓存）
    /// </summary>
    let executeParallelMatching
        (context: DemographicContext)
        (criteria: DemographicCriteria)
        (properties: ContextProperty list)
        (userId: string)
        : Async<AttributeMatchResult list> =
        async {
            // 计算缓存键
            let criteriaHash =
                Lorn.ADSP.Strategies.Targeting.Utils.CacheHelpers.calculateDataHash properties

            // 尝试从缓存获取结果
            match
                Lorn.ADSP.Strategies.Targeting.Utils.CacheHelpers.getCachedDemographicMatchResult<
                    AttributeMatchResult list
                 >
                    userId
                    criteriaHash
            with
            | Some cachedResults -> return cachedResults
            | None ->
                // 计算动态权重
                let weightResults =
                    Lorn.ADSP.Strategies.Targeting.Utils.WeightConfiguration.calculateBatchWeights properties

                let getWeight attributeName =
                    match weightResults.TryFind attributeName with
                    | Some result -> result.FinalWeight
                    | None -> 1.0m

                // 创建所有匹配任务
                let matchingTasks =
                    [ async { return matchAge context.Age criteria.AgeRange (getWeight "Age") }
                      async { return matchGender context.Gender criteria.TargetGenders (getWeight "Gender") }
                      async {
                          return matchEducation context.Education criteria.TargetEducations (getWeight "Education")
                      }
                      async {
                          return matchOccupation context.Occupation criteria.TargetOccupations (getWeight "Occupation")
                      }
                      async { return matchIncome context.Income criteria.TargetIncomes (getWeight "Income") }
                      async {
                          return
                              matchMaritalStatus
                                  context.MaritalStatus
                                  criteria.TargetMaritalStatuses
                                  (getWeight "MaritalStatus")
                      } ]

                // 并行执行所有匹配任务
                let! results = Async.Parallel matchingTasks
                let resultList = List.ofArray results

                // 缓存结果
                Lorn.ADSP.Strategies.Targeting.Utils.CacheHelpers.cacheDemographicMatchResult
                    userId
                    criteriaHash
                    resultList
                    properties

                return resultList
        }

    /// <summary>
    /// 计算综合匹配结果
    /// </summary>
    let calculateOverallResult (attributeResults: AttributeMatchResult list) (requireAllMatch: bool) : MatchingResult =
        let matchedResults = attributeResults |> List.filter (fun r -> r.IsMatch)
        let totalWeight = attributeResults |> List.sumBy (fun r -> r.Weight)

        let isOverallMatch =
            if requireAllMatch then
                attributeResults |> List.forall (fun r -> r.IsMatch)
            else
                matchedResults.Length > 0

        let weightedScore =
            if totalWeight > 0.0m then
                attributeResults
                |> List.map (fun r -> r.Score * r.Weight)
                |> List.sum
                |> fun sum -> sum / totalWeight
            else
                0.0m

        let reason =
            if isOverallMatch then
                let matchedAttributes =
                    matchedResults |> List.map (fun r -> r.AttributeName) |> String.concat ", "

                $"人口属性匹配成功，匹配属性: [{matchedAttributes}]"
            else
                let unmatchedResults = attributeResults |> List.filter (fun r -> not r.IsMatch)

                let unmatchedAttributes =
                    unmatchedResults |> List.map (fun r -> r.AttributeName) |> String.concat ", "

                $"人口属性匹配失败，不匹配属性: [{unmatchedAttributes}]"

        let notMatchReason =
            if not isOverallMatch then
                attributeResults
                |> List.filter (fun r -> not r.IsMatch)
                |> List.map (fun r -> r.Reason)
                |> String.concat "; "
            else
                ""

        let details = Dictionary<string, obj>()

        attributeResults
        |> List.iteri (fun i result ->
            details.Add($"Attribute_{i}_{result.AttributeName}_IsMatch", box result.IsMatch)
            details.Add($"Attribute_{i}_{result.AttributeName}_Score", box result.Score)
            details.Add($"Attribute_{i}_{result.AttributeName}_Reason", box result.Reason))

        details.Add("RequireAllMatch", box requireAllMatch)
        details.Add("MatchedCount", box matchedResults.Length)
        details.Add("TotalCount", box attributeResults.Length)

        { IsMatch = isOverallMatch
          Score = weightedScore
          Reason = reason
          NotMatchReason = notMatchReason
          Details = details
          ExecutionTime = TimeSpan.Zero // 将在调用方设置
          Weight = 1.0m
          Priority = 0
          IsRequired = false }

    /// <summary>
    /// 核心人口属性匹配算法（增强版，包含数据验证、推断、动态权重和缓存）
    /// 实现年龄、性别、教育程度、职业、收入、婚恋状况的并行匹配
    /// </summary>
    let executeMatching (matchingContext: MatchingContext) : Async<MatchingResult> =
        async {
            let startTime = DateTime.UtcNow

            try
                // 提取用户人口属性信息（包含数据验证和推断）
                let demographicContext = extractDemographicContext matchingContext.TargetingContext

                // 提取定向匹配条件
                let demographicCriteria = extractDemographicCriteria matchingContext.Criteria

                // 获取增强后的属性列表（用于权重计算和缓存）
                let demographics =
                    matchingContext.TargetingContext.GetPropertiesByCategory("Demographics")
                    |> List.ofSeq

                let (enhancedProperties, _) =
                    Lorn.ADSP.Strategies.Targeting.Utils.DataValidation.validateAndInferDemographicData demographics

                // 生成用户ID（用于缓存键）
                let userId =
                    match matchingContext.TargetingContext.GetProperty("UserId") with
                    | null -> "anonymous"
                    | prop -> prop.PropertyValue

                // 并行执行所有属性匹配（使用动态权重和缓存）
                let! attributeResults =
                    executeParallelMatching demographicContext demographicCriteria enhancedProperties userId

                // 计算综合匹配结果
                let overallResult =
                    calculateOverallResult attributeResults demographicCriteria.RequireAllMatch

                let executionTime = DateTime.UtcNow - startTime

                return
                    { overallResult with
                        ExecutionTime = executionTime }

            with ex ->
                let executionTime = DateTime.UtcNow - startTime
                let errorDetails = Dictionary<string, obj>()
                errorDetails.Add("Exception", box ex.Message)
                errorDetails.Add("StackTrace", box ex.StackTrace)

                return createNoMatchResult $"人口属性匹配执行异常: {ex.Message}" errorDetails executionTime 1.0m 0 false
        }

    /// <summary>
    /// 验证人口属性定向条件的有效性
    /// </summary>
    let validateCriteria (criteria: ITargetingCriteria) : bool =
        try
            let demographicCriteria = extractDemographicCriteria criteria

            // 验证年龄范围
            let ageRangeValid =
                match demographicCriteria.AgeRange with
                | Some(minAge, maxAge) -> minAge >= 0 && maxAge <= 150 && minAge <= maxAge
                | None -> true

            // 验证其他枚举值是否有效
            let gendersValid =
                demographicCriteria.TargetGenders
                |> List.forall (fun g -> Enum.IsDefined(typeof<Gender>, g))

            let educationsValid =
                demographicCriteria.TargetEducations
                |> List.forall (fun e -> Enum.IsDefined(typeof<Lorn.ADSP.Core.Shared.Enums.EducationLevel>, e))

            let occupationsValid =
                demographicCriteria.TargetOccupations
                |> List.forall (fun o -> Enum.IsDefined(typeof<Lorn.ADSP.Core.Shared.Enums.OccupationType>, o))

            let incomesValid =
                demographicCriteria.TargetIncomes
                |> List.forall (fun i -> Enum.IsDefined(typeof<Lorn.ADSP.Core.Shared.Enums.IncomeLevel>, i))

            let maritalStatusesValid =
                demographicCriteria.TargetMaritalStatuses
                |> List.forall (fun m -> Enum.IsDefined(typeof<Lorn.ADSP.Core.Shared.Enums.MaritalStatus>, m))

            ageRangeValid
            && gendersValid
            && educationsValid
            && occupationsValid
            && incomesValid
            && maritalStatusesValid

        with _ ->
            false

    /// <summary>
    /// 获取支持的定向条件类型
    /// </summary>
    let getSupportedCriteriaTypes () =
        [ "Demographic"; "Demographics"; "UserDemographic" ]

    /// <summary>
    /// 检查是否支持指定的定向条件类型
    /// </summary>
    let isSupported (criteriaType: string) : bool =
        getSupportedCriteriaTypes () |> List.contains criteriaType

    /// <summary>
    /// 获取匹配器元数据信息
    /// </summary>
    let getMatcherMetadata () =
        let supportedAttributes =
            [ "Age - 年龄范围匹配"
              "Gender - 性别匹配"
              "Education - 教育程度匹配"
              "Occupation - 职业类型匹配"
              "Income - 收入水平匹配"
              "MaritalStatus - 婚恋状况匹配" ]

        let metadata = Dictionary<string, obj>()
        metadata.Add("MatcherType", box "Demographic")
        metadata.Add("Version", box "1.0.0")
        metadata.Add("SupportedAttributes", box supportedAttributes)
        metadata.Add("SupportsCriteriaTypes", box (getSupportedCriteriaTypes ()))
        metadata.Add("SupportsParallelExecution", box true)
        metadata.Add("ExpectedExecutionTimeMs", box 10)
        metadata.Add("Description", box "人口属性定向匹配器，支持年龄、性别、教育程度、职业、收入、婚恋状况的并行匹配")

        metadata
