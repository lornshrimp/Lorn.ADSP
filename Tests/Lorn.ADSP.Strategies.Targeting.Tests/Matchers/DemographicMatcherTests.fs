namespace Lorn.ADSP.Strategies.Targeting.Tests.Matchers

open System
open System.Collections.Generic
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Core.Shared.Enums
open Lorn.ADSP.Strategies.Targeting.Matchers.DemographicMatcher

/// <summary>
/// 人口属性定向匹配器单元测试
/// 测试各种人口属性组合的匹配逻辑，验证边界条件和异常情况处理
/// 遵循需求1.1-1.6和13.1-13.3的验收标准
/// </summary>
module DemographicMatcherTests =

    // ==================== 需求1.1测试：人口属性数据结构测试 ====================

    [<Fact>]
    let ``DemographicContext应该正确存储所有人口属性`` () =
        // Arrange & Act
        let context =
            { Age = Some 25
              Gender = Some Gender.Female
              Education = Some EducationLevel.Bachelor
              Occupation = Some OccupationType.Professional
              Income = Some IncomeLevel.UpperMiddle
              MaritalStatus = Some MaritalStatus.Single }

        // Assert - 验证所有属性都被正确存储
        context.Age |> should equal (Some 25)
        context.Gender |> should equal (Some Gender.Female)
        context.Education |> should equal (Some EducationLevel.Bachelor)
        context.Occupation |> should equal (Some OccupationType.Professional)
        context.Income |> should equal (Some IncomeLevel.UpperMiddle)
        context.MaritalStatus |> should equal (Some MaritalStatus.Single)

    [<Fact>]
    let ``DemographicContext应该支持部分属性缺失`` () =
        // Arrange & Act
        let context =
            { Age = Some 30
              Gender = Some Gender.Male
              Education = None
              Occupation = None
              Income = None
              MaritalStatus = None }

        // Assert - 验证部分属性可以为None
        context.Age |> should equal (Some 30)
        context.Gender |> should equal (Some Gender.Male)
        context.Education |> should equal None
        context.Occupation |> should equal None
        context.Income |> should equal None
        context.MaritalStatus |> should equal None

    [<Fact>]
    let ``DemographicCriteria应该正确存储匹配条件`` () =
        // Arrange & Act
        let criteria =
            { AgeRange = Some(20, 30)
              TargetGenders = [ Gender.Female; Gender.Male ]
              TargetEducations = [ EducationLevel.Bachelor; EducationLevel.Master ]
              TargetOccupations = [ OccupationType.Professional ]
              TargetIncomes = [ IncomeLevel.UpperMiddle; IncomeLevel.High ]
              TargetMaritalStatuses = [ MaritalStatus.Single ]
              RequireAllMatch = true }

        // Assert
        criteria.AgeRange |> should equal (Some(20, 30))
        criteria.TargetGenders |> should equal [ Gender.Female; Gender.Male ]

        criteria.TargetEducations
        |> should equal [ EducationLevel.Bachelor; EducationLevel.Master ]

        criteria.TargetOccupations |> should equal [ OccupationType.Professional ]

        criteria.TargetIncomes
        |> should equal [ IncomeLevel.UpperMiddle; IncomeLevel.High ]

        criteria.TargetMaritalStatuses |> should equal [ MaritalStatus.Single ]
        criteria.RequireAllMatch |> should equal true

    // ==================== 需求1.4测试：年龄匹配逻辑 ====================

    [<Fact>]
    let ``matchAge应该在年龄范围内时返回匹配成功`` () =
        // Arrange
        let userAge = Some 25
        let ageRange = Some(20, 30)
        let weight = 1.0m

        // Act
        let result = matchAge userAge ageRange weight

        // Assert
        result.AttributeName |> should equal "Age"
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m
        result.Weight |> should equal weight
        result.Reason.Contains("25") |> should equal true
        result.Reason.Contains("20-30") |> should equal true

    [<Fact>]
    let ``matchAge应该在年龄超出范围时返回匹配失败`` () =
        // Arrange
        let userAge = Some 35
        let ageRange = Some(20, 30)
        let weight = 1.0m

        // Act
        let result = matchAge userAge ageRange weight

        // Assert
        result.AttributeName |> should equal "Age"
        result.IsMatch |> should equal false
        result.Score |> should equal 0.0m
        result.Weight |> should equal weight
        result.Reason.Contains("35") |> should equal true
        result.Reason.Contains("不在") |> should equal true

    [<Fact>]
    let ``matchAge应该在年龄边界值时正确匹配`` () =
        // Arrange & Act & Assert
        let testCases =
            [ (Some 20, Some(20, 30), true) // 下边界
              (Some 30, Some(20, 30), true) // 上边界
              (Some 19, Some(20, 30), false) // 下边界外
              (Some 31, Some(20, 30), false) ] // 上边界外

        for (userAge, ageRange, expectedMatch) in testCases do
            let result = matchAge userAge ageRange 1.0m
            result.IsMatch |> should equal expectedMatch

    [<Fact>]
    let ``matchAge应该在用户年龄缺失时返回匹配失败`` () =
        // Arrange
        let userAge = None
        let ageRange = Some(20, 30)
        let weight = 1.0m

        // Act
        let result = matchAge userAge ageRange weight

        // Assert
        result.IsMatch |> should equal false
        result.Score |> should equal 0.0m
        result.Reason.Contains("缺失") |> should equal true

    [<Fact>]
    let ``matchAge应该在无年龄限制时返回匹配成功`` () =
        // Arrange
        let userAge = Some 25
        let ageRange = None
        let weight = 1.0m

        // Act
        let result = matchAge userAge ageRange weight

        // Assert
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m
        result.Reason.Contains("无年龄限制") |> should equal true

    // ==================== 需求1.4测试：性别匹配逻辑 ====================

    [<Fact>]
    let ``matchGender应该在性别匹配时返回成功`` () =
        // Arrange
        let userGender = Some Gender.Female
        let targetGenders = [ Gender.Female; Gender.Male ]
        let weight = 1.0m

        // Act
        let result = matchGender userGender targetGenders weight

        // Assert
        result.AttributeName |> should equal "Gender"
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m
        result.Weight |> should equal weight
        result.Reason.Contains("Female") |> should equal true

    [<Fact>]
    let ``matchGender应该在性别不匹配时返回失败`` () =
        // Arrange
        let userGender = Some Gender.Male
        let targetGenders = [ Gender.Female ]
        let weight = 1.0m

        // Act
        let result = matchGender userGender targetGenders weight

        // Assert
        result.IsMatch |> should equal false
        result.Score |> should equal 0.0m
        result.Reason.Contains("Male") |> should equal true
        result.Reason.Contains("不在") |> should equal true

    [<Fact>]
    let ``matchGender应该在用户性别缺失时返回失败`` () =
        // Arrange
        let userGender = None
        let targetGenders = [ Gender.Female ]
        let weight = 1.0m

        // Act
        let result = matchGender userGender targetGenders weight

        // Assert
        result.IsMatch |> should equal false
        result.Reason.Contains("缺失") |> should equal true

    [<Fact>]
    let ``matchGender应该在无性别限制时返回成功`` () =
        // Arrange
        let userGender = Some Gender.Female
        let targetGenders = []
        let weight = 1.0m

        // Act
        let result = matchGender userGender targetGenders weight

        // Assert
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m
        result.Reason.Contains("无性别限制") |> should equal true

    // ==================== 需求1.4测试：教育程度匹配逻辑 ====================

    [<Fact>]
    let ``matchEducation应该在教育程度匹配时返回成功`` () =
        // Arrange
        let userEducation = Some EducationLevel.Bachelor
        let targetEducations = [ EducationLevel.Bachelor; EducationLevel.Master ]
        let weight = 1.0m

        // Act
        let result = matchEducation userEducation targetEducations weight

        // Assert
        result.AttributeName |> should equal "Education"
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m
        result.Reason.Contains("Bachelor") |> should equal true

    [<Fact>]
    let ``matchEducation应该在教育程度不匹配时返回失败`` () =
        // Arrange
        let userEducation = Some EducationLevel.Doctorate
        let targetEducations = [ EducationLevel.Bachelor; EducationLevel.Master ]
        let weight = 1.0m

        // Act
        let result = matchEducation userEducation targetEducations weight

        // Assert
        result.IsMatch |> should equal false
        result.Score |> should equal 0.0m
        result.Reason.Contains("Doctorate") |> should equal true
        result.Reason.Contains("不在") |> should equal true

    // ==================== 需求1.4测试：职业匹配逻辑 ====================

    [<Fact>]
    let ``matchOccupation应该在职业匹配时返回成功`` () =
        // Arrange
        let userOccupation = Some OccupationType.Professional
        let targetOccupations = [ OccupationType.Professional; OccupationType.Manager ]
        let weight = 1.0m

        // Act
        let result = matchOccupation userOccupation targetOccupations weight

        // Assert
        result.AttributeName |> should equal "Occupation"
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m
        result.Reason.Contains("Professional") |> should equal true

    [<Fact>]
    let ``matchOccupation应该在职业不匹配时返回失败`` () =
        // Arrange
        let userOccupation = Some OccupationType.Student
        let targetOccupations = [ OccupationType.Professional; OccupationType.Manager ]
        let weight = 1.0m

        // Act
        let result = matchOccupation userOccupation targetOccupations weight

        // Assert
        result.IsMatch |> should equal false
        result.Score |> should equal 0.0m
        result.Reason.Contains("Student") |> should equal true
        result.Reason.Contains("不在") |> should equal true

    // ==================== 需求1.4测试：收入匹配逻辑 ====================

    [<Fact>]
    let ``matchIncome应该在收入匹配时返回成功`` () =
        // Arrange
        let userIncome = Some IncomeLevel.UpperMiddle
        let targetIncomes = [ IncomeLevel.UpperMiddle; IncomeLevel.High ]
        let weight = 1.0m

        // Act
        let result = matchIncome userIncome targetIncomes weight

        // Assert
        result.AttributeName |> should equal "Income"
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m
        result.Reason.Contains("UpperMiddle") |> should equal true

    [<Fact>]
    let ``matchIncome应该在收入不匹配时返回失败`` () =
        // Arrange
        let userIncome = Some IncomeLevel.Low
        let targetIncomes = [ IncomeLevel.UpperMiddle; IncomeLevel.High ]
        let weight = 1.0m

        // Act
        let result = matchIncome userIncome targetIncomes weight

        // Assert
        result.IsMatch |> should equal false
        result.Score |> should equal 0.0m
        result.Reason.Contains("Low") |> should equal true
        result.Reason.Contains("不在") |> should equal true

    // ==================== 需求1.4测试：婚恋状况匹配逻辑 ====================

    [<Fact>]
    let ``matchMaritalStatus应该在婚恋状况匹配时返回成功`` () =
        // Arrange
        let userMaritalStatus = Some MaritalStatus.Single
        let targetMaritalStatuses = [ MaritalStatus.Single; MaritalStatus.Married ]
        let weight = 1.0m

        // Act
        let result = matchMaritalStatus userMaritalStatus targetMaritalStatuses weight

        // Assert
        result.AttributeName |> should equal "MaritalStatus"
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m
        result.Reason.Contains("Single") |> should equal true

    [<Fact>]
    let ``matchMaritalStatus应该在婚恋状况不匹配时返回失败`` () =
        // Arrange
        let userMaritalStatus = Some MaritalStatus.Divorced
        let targetMaritalStatuses = [ MaritalStatus.Single; MaritalStatus.Married ]
        let weight = 1.0m

        // Act
        let result = matchMaritalStatus userMaritalStatus targetMaritalStatuses weight

        // Assert
        result.IsMatch |> should equal false
        result.Score |> should equal 0.0m
        result.Reason.Contains("Divorced") |> should equal true
        result.Reason.Contains("不在") |> should equal true

    // ==================== 需求1.6测试：综合匹配结果计算 ====================

    [<Fact>]
    let ``calculateOverallResult应该在所有属性匹配时返回成功`` () =
        // Arrange
        let attributeResults =
            [ { AttributeName = "Age"
                IsMatch = true
                Score = 1.0m
                Reason = "匹配"
                Weight = 1.0m }
              { AttributeName = "Gender"
                IsMatch = true
                Score = 1.0m
                Reason = "匹配"
                Weight = 1.0m }
              { AttributeName = "Education"
                IsMatch = true
                Score = 1.0m
                Reason = "匹配"
                Weight = 1.0m } ]

        let requireAllMatch = true

        // Act
        let result = calculateOverallResult attributeResults requireAllMatch

        // Assert
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m
        result.Reason.Contains("人口属性匹配成功") |> should equal true

    [<Fact>]
    let ``calculateOverallResult应该在要求全部匹配但有失败时返回失败`` () =
        // Arrange
        let attributeResults =
            [ { AttributeName = "Age"
                IsMatch = true
                Score = 1.0m
                Reason = "匹配"
                Weight = 1.0m }
              { AttributeName = "Gender"
                IsMatch = false
                Score = 0.0m
                Reason = "不匹配"
                Weight = 1.0m }
              { AttributeName = "Education"
                IsMatch = true
                Score = 1.0m
                Reason = "匹配"
                Weight = 1.0m } ]

        let requireAllMatch = true

        // Act
        let result = calculateOverallResult attributeResults requireAllMatch

        // Assert
        result.IsMatch |> should equal false
        result.Score |> should equal 0.0m
        result.Reason.Contains("人口属性匹配失败") |> should equal true
        result.NotMatchReason.Contains("Gender") |> should equal true

    [<Fact>]
    let ``calculateOverallResult应该在不要求全部匹配且有部分成功时返回成功`` () =
        // Arrange
        let attributeResults =
            [ { AttributeName = "Age"
                IsMatch = true
                Score = 1.0m
                Reason = "匹配"
                Weight = 1.0m }
              { AttributeName = "Gender"
                IsMatch = false
                Score = 0.0m
                Reason = "不匹配"
                Weight = 1.0m }
              { AttributeName = "Education"
                IsMatch = true
                Score = 1.0m
                Reason = "匹配"
                Weight = 1.0m } ]

        let requireAllMatch = false

        // Act
        let result = calculateOverallResult attributeResults requireAllMatch

        // Assert
        result.IsMatch |> should equal true
        result.Score |> should be (greaterThan 0.0m)
        result.Score |> should be (lessThan 1.0m)

    [<Fact>]
    let ``calculateOverallResult应该正确计算加权分数`` () =
        // Arrange
        let attributeResults =
            [ { AttributeName = "Age"
                IsMatch = true
                Score = 1.0m
                Reason = "匹配"
                Weight = 2.0m }
              { AttributeName = "Gender"
                IsMatch = true
                Score = 1.0m
                Reason = "匹配"
                Weight = 1.0m } ]

        let requireAllMatch = false

        // Act
        let result = calculateOverallResult attributeResults requireAllMatch

        // Assert
        result.IsMatch |> should equal true
        // 加权分数应该是 (1.0*2.0 + 1.0*1.0) / (2.0 + 1.0) = 3.0/3.0 = 1.0
        result.Score |> should equal 1.0m

    // ==================== 匹配器元数据测试 ====================

    [<Fact>]
    let ``getSupportedCriteriaTypes应该返回正确的支持类型`` () =
        // Act
        let supportedTypes = getSupportedCriteriaTypes ()

        // Assert
        supportedTypes |> should contain "Demographic"
        supportedTypes |> should contain "Demographics"
        supportedTypes |> should contain "UserDemographic"

    [<Fact>]
    let ``isSupported应该正确验证支持的类型`` () =
        // Act & Assert
        isSupported "Demographic" |> should equal true
        isSupported "Demographics" |> should equal true
        isSupported "UserDemographic" |> should equal true
        isSupported "InvalidType" |> should equal false
        isSupported "" |> should equal false

    [<Fact>]
    let ``getMatcherMetadata应该返回正确的元数据信息`` () =
        // Act
        let metadata = getMatcherMetadata ()

        // Assert
        metadata.["MatcherType"] |> should equal "Demographic"
        metadata.["Version"] |> should equal "1.0.0"
        metadata.["SupportsParallelExecution"] |> should equal true
        metadata.["ExpectedExecutionTimeMs"] |> should equal 10
        metadata.ContainsKey("SupportedAttributes") |> should equal true
        metadata.ContainsKey("Description") |> should equal true

    // ==================== 边界条件和异常情况测试 ====================

    [<Fact>]
    let ``AttributeMatchResult应该正确存储匹配结果信息`` () =
        // Arrange & Act
        let result =
            { AttributeName = "TestAttribute"
              IsMatch = true
              Score = 0.8m
              Reason = "测试匹配成功"
              Weight = 1.5m }

        // Assert
        result.AttributeName |> should equal "TestAttribute"
        result.IsMatch |> should equal true
        result.Score |> should equal 0.8m
        result.Reason |> should equal "测试匹配成功"
        result.Weight |> should equal 1.5m

    [<Fact>]
    let ``匹配函数应该处理空列表条件`` () =
        // Arrange
        let userGender = Some Gender.Female
        let emptyGenders = []
        let weight = 1.0m

        // Act
        let result = matchGender userGender emptyGenders weight

        // Assert - 空列表应该表示无限制，匹配成功
        result.IsMatch |> should equal true
        result.Score |> should equal 1.0m

        (result.Reason.Contains("无") && result.Reason.Contains("限制"))
        |> should equal true

    [<Fact>]
    let ``匹配函数应该处理零权重`` () =
        // Arrange
        let userAge = Some 25
        let ageRange = Some(20, 30)
        let zeroWeight = 0.0m

        // Act
        let result = matchAge userAge ageRange zeroWeight

        // Assert
        result.IsMatch |> should equal true
        result.Weight |> should equal 0.0m

    [<Fact>]
    let ``匹配函数应该处理负权重`` () =
        // Arrange
        let userAge = Some 25
        let ageRange = Some(20, 30)
        let negativeWeight = -1.0m

        // Act
        let result = matchAge userAge ageRange negativeWeight

        // Assert
        result.IsMatch |> should equal true
        result.Weight |> should equal -1.0m
