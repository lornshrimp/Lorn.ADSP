namespace Lorn.ADSP.Strategies.Targeting.Tests.Core

open System
open Xunit
open FsUnit.Xunit
open Lorn.ADSP.Core.Shared.Enums

/// <summary>
/// 人口属性枚举测试
/// 验证重构后的枚举定义是否正确
/// </summary>
module DemographicEnumTests =

    [<Fact>]
    let ``验证教育程度枚举定义正确`` () =
        // Arrange & Act
        let educationLevel = EducationLevel.Bachelor

        // Assert
        int educationLevel |> should equal 5
        educationLevel.ToString() |> should equal "Bachelor"

    [<Fact>]
    let ``验证婚恋状况枚举定义正确`` () =
        // Arrange & Act
        let maritalStatus = MaritalStatus.Married

        // Assert
        int maritalStatus |> should equal 2
        maritalStatus.ToString() |> should equal "Married"

    [<Fact>]
    let ``验证职业类型枚举定义正确`` () =
        // Arrange & Act
        let occupationType = OccupationType.Professional

        // Assert
        int occupationType |> should equal 4
        occupationType.ToString() |> should equal "Professional"

    [<Fact>]
    let ``验证收入水平枚举定义正确`` () =
        // Arrange & Act
        let incomeLevel = IncomeLevel.UpperMiddle

        // Assert
        int incomeLevel |> should equal 4
        incomeLevel.ToString() |> should equal "UpperMiddle"

    [<Fact>]
    let ``验证性别枚举定义正确`` () =
        // Arrange & Act
        let gender = Gender.Female

        // Assert
        int gender |> should equal 2
        gender.ToString() |> should equal "Female"

    [<Fact>]
    let ``验证枚举解析功能正常`` () =
        // Arrange & Act & Assert
        let mutable educationResult = EducationLevel.Unknown

        Enum.TryParse<EducationLevel>("Bachelor", true, &educationResult)
        |> should be True

        educationResult |> should equal EducationLevel.Bachelor

        let mutable maritalResult = MaritalStatus.Unknown
        Enum.TryParse<MaritalStatus>("Single", true, &maritalResult) |> should be True
        maritalResult |> should equal MaritalStatus.Single

        let mutable occupationResult = OccupationType.Unknown

        Enum.TryParse<OccupationType>("Manager", true, &occupationResult)
        |> should be True

        occupationResult |> should equal OccupationType.Manager

        let mutable incomeResult = IncomeLevel.Unknown
        Enum.TryParse<IncomeLevel>("High", true, &incomeResult) |> should be True
        incomeResult |> should equal IncomeLevel.High
