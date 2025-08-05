namespace Lorn.ADSP.Strategies.Targeting.Utils

open System
open System.Collections.Generic
open Lorn.ADSP.Core.Domain.ValueObjects
open Lorn.ADSP.Core.Shared.Enums

/// <summary>
/// 数据验证工具模块
/// 提供人口属性数据验证和完整性检查功能
/// </summary>
module DataValidation =

    /// <summary>
    /// 数据完整性检查结果
    /// </summary>
    type DataCompletenessResult =
        { IsComplete: bool
          MissingAttributes: string list
          InvalidAttributes: string list
          CompletionScore: decimal
          Recommendations: string list }

    /// <summary>
    /// 属性推断规则
    /// </summary>
    type InferenceRule =
        { AttributeName: string
          SourceAttributes: string list
          InferenceLogic: Map<string, string> -> string option
          Confidence: decimal
          Description: string }

    /// <summary>
    /// 验证年龄数据的有效性
    /// </summary>
    let validateAge (ageValue: string) : bool * string option =
        if String.IsNullOrWhiteSpace(ageValue) then
            (false, Some "年龄信息缺失")
        else
            match Int32.TryParse(ageValue) with
            | (true, age) when age >= 0 && age <= 150 -> (true, None)
            | (true, age) -> (false, Some $"年龄值 {age} 超出合理范围 (0-150)")
            | _ -> (false, Some $"年龄值 '{ageValue}' 格式无效")

    /// <summary>
    /// 验证性别数据的有效性
    /// </summary>
    let validateGender (genderValue: string) : bool * string option =
        if String.IsNullOrWhiteSpace(genderValue) then
            (false, Some "性别信息缺失")
        else
            let mutable result = Gender.Unknown

            if Enum.TryParse<Gender>(genderValue, true, &result) && result <> Gender.Unknown then
                (true, None)
            else
                (false, Some $"性别值 '{genderValue}' 无效")

    /// <summary>
    /// 验证教育程度数据的有效性
    /// </summary>
    let validateEducation (educationValue: string) : bool * string option =
        if String.IsNullOrWhiteSpace(educationValue) then
            (false, Some "教育程度信息缺失")
        else
            let mutable result = Lorn.ADSP.Core.Shared.Enums.EducationLevel.Unknown

            if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.EducationLevel>(educationValue, true, &result) then
                (true, None)
            else
                (false, Some $"教育程度值 '{educationValue}' 无效")

    /// <summary>
    /// 验证职业数据的有效性
    /// </summary>
    let validateOccupation (occupationValue: string) : bool * string option =
        if String.IsNullOrWhiteSpace(occupationValue) then
            (false, Some "职业信息缺失")
        else
            let mutable result = Lorn.ADSP.Core.Shared.Enums.OccupationType.Unknown

            if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.OccupationType>(occupationValue, true, &result) then
                (true, None)
            else
                (false, Some $"职业值 '{occupationValue}' 无效")

    /// <summary>
    /// 验证收入数据的有效性
    /// </summary>
    let validateIncome (incomeValue: string) : bool * string option =
        if String.IsNullOrWhiteSpace(incomeValue) then
            (false, Some "收入信息缺失")
        else
            let mutable result = Lorn.ADSP.Core.Shared.Enums.IncomeLevel.Unknown

            if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.IncomeLevel>(incomeValue, true, &result) then
                (true, None)
            else
                (false, Some $"收入水平值 '{incomeValue}' 无效")

    /// <summary>
    /// 验证婚恋状况数据的有效性
    /// </summary>
    let validateMaritalStatus (maritalValue: string) : bool * string option =
        if String.IsNullOrWhiteSpace(maritalValue) then
            (false, Some "婚恋状况信息缺失")
        else
            let mutable result = Lorn.ADSP.Core.Shared.Enums.MaritalStatus.Unknown

            if Enum.TryParse<Lorn.ADSP.Core.Shared.Enums.MaritalStatus>(maritalValue, true, &result) then
                (true, None)
            else
                (false, Some $"婚恋状况值 '{maritalValue}' 无效")

    /// <summary>
    /// 基于年龄推断其他属性的规则
    /// </summary>
    let inferFromAge (age: int) : Map<string, string> =
        let inferences = Dictionary<string, string>()

        // 基于年龄推断教育程度
        match age with
        | a when a >= 6 && a <= 12 -> inferences.Add("Education", "Elementary")
        | a when a >= 13 && a <= 15 -> inferences.Add("Education", "MiddleSchool")
        | a when a >= 16 && a <= 18 -> inferences.Add("Education", "HighSchool")
        | a when a >= 19 && a <= 22 -> inferences.Add("Education", "Bachelor")
        | a when a >= 23 && a <= 25 -> inferences.Add("Education", "Master")
        | _ -> ()

        // 基于年龄推断职业状态
        match age with
        | a when a >= 6 && a <= 22 -> inferences.Add("Occupation", "Student")
        | a when a >= 23 && a <= 65 -> inferences.Add("Occupation", "Employee")
        | a when a > 65 -> inferences.Add("Occupation", "Retired")
        | _ -> ()

        // 基于年龄推断收入水平
        match age with
        | a when a >= 18 && a <= 25 -> inferences.Add("Income", "Low")
        | a when a >= 26 && a <= 35 -> inferences.Add("Income", "LowerMiddle")
        | a when a >= 36 && a <= 50 -> inferences.Add("Income", "Middle")
        | a when a >= 51 && a <= 65 -> inferences.Add("Income", "UpperMiddle")
        | _ -> ()

        Map.ofSeq (inferences |> Seq.map (fun kvp -> kvp.Key, kvp.Value))

    /// <summary>
    /// 基于教育程度推断其他属性的规则
    /// </summary>
    let inferFromEducation (education: string) : Map<string, string> =
        let inferences = Dictionary<string, string>()

        // 基于教育程度推断收入水平
        match education with
        | "Elementary"
        | "MiddleSchool" -> inferences.Add("Income", "Low")
        | "HighSchool"
        | "College" -> inferences.Add("Income", "LowerMiddle")
        | "Bachelor" -> inferences.Add("Income", "Middle")
        | "Master" -> inferences.Add("Income", "UpperMiddle")
        | "Doctorate" -> inferences.Add("Income", "High")
        | _ -> ()

        // 基于教育程度推断职业类型
        match education with
        | "Elementary"
        | "MiddleSchool"
        | "HighSchool" -> inferences.Add("Occupation", "Student")
        | "Bachelor"
        | "College" -> inferences.Add("Occupation", "Employee")
        | "Master"
        | "Doctorate" -> inferences.Add("Occupation", "Professional")
        | _ -> ()

        Map.ofSeq (inferences |> Seq.map (fun kvp -> kvp.Key, kvp.Value))

    /// <summary>
    /// 基于职业推断其他属性的规则
    /// </summary>
    let inferFromOccupation (occupation: string) : Map<string, string> =
        let inferences = Dictionary<string, string>()

        // 基于职业推断收入水平
        match occupation with
        | "Student"
        | "Unemployed" -> inferences.Add("Income", "Low")
        | "Employee" -> inferences.Add("Income", "LowerMiddle")
        | "Manager"
        | "Professional" -> inferences.Add("Income", "Middle")
        | "Entrepreneur" -> inferences.Add("Income", "UpperMiddle")
        | "Retired" -> inferences.Add("Income", "Low")
        | _ -> ()

        // 基于职业推断教育程度
        match occupation with
        | "Professional"
        | "Manager" -> inferences.Add("Education", "Bachelor")
        | "Entrepreneur" -> inferences.Add("Education", "Bachelor")
        | "Student" -> inferences.Add("Education", "HighSchool")
        | _ -> ()

        Map.ofSeq (inferences |> Seq.map (fun kvp -> kvp.Key, kvp.Value))

    /// <summary>
    /// 执行属性推断逻辑
    /// </summary>
    let executeInference (properties: ContextProperty list) : Map<string, string> =
        let propertyMap =
            properties |> List.map (fun p -> p.PropertyKey, p.PropertyValue) |> Map.ofList

        let allInferences = Dictionary<string, string>()

        // 基于年龄进行推断
        match propertyMap.TryFind "Age" with
        | Some ageStr when not (String.IsNullOrWhiteSpace(ageStr)) ->
            match Int32.TryParse(ageStr) with
            | (true, age) ->
                let ageInferences = inferFromAge age

                for kvp in ageInferences do
                    if not (allInferences.ContainsKey(kvp.Key)) then
                        allInferences.Add(kvp.Key, kvp.Value)
            | _ -> ()
        | _ -> ()

        // 基于教育程度进行推断
        match propertyMap.TryFind "Education" with
        | Some education when not (String.IsNullOrWhiteSpace(education)) ->
            let educationInferences = inferFromEducation education

            for kvp in educationInferences do
                if not (allInferences.ContainsKey(kvp.Key)) then
                    allInferences.Add(kvp.Key, kvp.Value)
        | _ -> ()

        // 基于职业进行推断
        match propertyMap.TryFind "Occupation" with
        | Some occupation when not (String.IsNullOrWhiteSpace(occupation)) ->
            let occupationInferences = inferFromOccupation occupation

            for kvp in occupationInferences do
                if not (allInferences.ContainsKey(kvp.Key)) then
                    allInferences.Add(kvp.Key, kvp.Value)
        | _ -> ()

        Map.ofSeq (allInferences |> Seq.map (fun kvp -> kvp.Key, kvp.Value))

    /// <summary>
    /// 检查人口属性数据的完整性
    /// </summary>
    let checkDataCompleteness (properties: ContextProperty list) : DataCompletenessResult =
        let requiredAttributes =
            [ "Age"; "Gender"; "Education"; "Occupation"; "Income"; "MaritalStatus" ]

        let propertyMap =
            properties |> List.map (fun p -> p.PropertyKey, p.PropertyValue) |> Map.ofList

        let missingAttributes = ResizeArray<string>()
        let invalidAttributes = ResizeArray<string>()
        let recommendations = ResizeArray<string>()

        // 检查每个必需属性
        for attr in requiredAttributes do
            match propertyMap.TryFind attr with
            | Some value when not (String.IsNullOrWhiteSpace(value)) ->
                // 验证属性值的有效性
                let (isValid, errorMsg) =
                    match attr with
                    | "Age" -> validateAge value
                    | "Gender" -> validateGender value
                    | "Education" -> validateEducation value
                    | "Occupation" -> validateOccupation value
                    | "Income" -> validateIncome value
                    | "MaritalStatus" -> validateMaritalStatus value
                    | _ -> (true, None)

                if not isValid then
                    invalidAttributes.Add(attr)

                    match errorMsg with
                    | Some msg -> recommendations.Add($"修复 {attr}: {msg}")
                    | None -> ()
            | _ ->
                missingAttributes.Add(attr)
                recommendations.Add($"补充缺失的 {attr} 信息")

        // 计算完整性评分
        let totalAttributes = requiredAttributes.Length

        let validAttributes =
            totalAttributes - missingAttributes.Count - invalidAttributes.Count

        let completionScore = decimal validAttributes / decimal totalAttributes

        // 添加推断建议
        if missingAttributes.Count > 0 then
            recommendations.Add("可以基于现有属性进行智能推断来补充缺失信息")

        { IsComplete = missingAttributes.Count = 0 && invalidAttributes.Count = 0
          MissingAttributes = List.ofSeq missingAttributes
          InvalidAttributes = List.ofSeq invalidAttributes
          CompletionScore = completionScore
          Recommendations = List.ofSeq recommendations }

    /// <summary>
    /// 应用默认推断规则补充缺失的属性
    /// </summary>
    let applyDefaultInferenceRules (properties: ContextProperty list) : ContextProperty list =
        let inferences = executeInference properties
        let existingKeys = properties |> List.map (fun p -> p.PropertyKey) |> Set.ofList

        let inferredProperties =
            inferences
            |> Map.toList
            |> List.filter (fun (key, _) -> not (Set.contains key existingKeys))
            |> List.map (fun (key, value) ->
                ContextProperty(
                    propertyKey = key,
                    propertyValue = value,
                    dataType = "String",
                    category = "Demographics",
                    isSensitive = false,
                    weight = 0.5m, // 推断属性权重较低
                    expiresAt = Nullable(DateTime.UtcNow.AddDays(7.0)), // 推断属性7天后过期
                    dataSource = "Inference"
                ))

        properties @ inferredProperties

    /// <summary>
    /// 验证人口属性数据并应用推断规则
    /// </summary>
    let validateAndInferDemographicData
        (properties: ContextProperty list)
        : ContextProperty list * DataCompletenessResult =
        // 首先检查数据完整性
        let completenessResult = checkDataCompleteness properties

        // 应用推断规则补充缺失属性
        let enhancedProperties = applyDefaultInferenceRules properties

        // 重新检查完整性（包含推断的属性）
        let finalCompletenessResult = checkDataCompleteness enhancedProperties

        (enhancedProperties, finalCompletenessResult)

    /// <summary>
    /// 获取模块版本信息
    /// </summary>
    let getModuleVersion () = "2.0.0"
