using FluentAssertions;
using Lorn.ADSP.Infrastructure.Common.Extensions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Lorn.ADSP.Infrastructure.Common.Tests.Extensions;

/// <summary>
/// ValidationExtensions 扩展方法的单元测试
/// 验证验证扩展功能的正确性
/// </summary>
public class ValidationExtensionsTests
{
    /// <summary>
    /// 测试ValidateObject方法对有效对象的验证
    /// </summary>
    [Fact]
    public void ValidateObject_Should_Return_Valid_Result_For_Valid_Object()
    {
        // Arrange
        var validObject = new TestValidationModel
        {
            RequiredProperty = "ValidValue",
            RangeProperty = 50,
            StringLengthProperty = "ValidLength"
        };

        // Act
        var result = validObject.ValidateObject();

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// 测试ValidateObject方法对无效对象的验证
    /// </summary>
    [Fact]
    public void ValidateObject_Should_Return_Invalid_Result_For_Invalid_Object()
    {
        // Arrange
        var invalidObject = new TestValidationModel
        {
            RequiredProperty = null, // 违反Required验证
            RangeProperty = 150, // 违反Range验证
            StringLengthProperty = "ThisStringIsTooLongForValidation" // 违反StringLength验证
        };

        // Act
        var result = invalidObject.ValidateObject();

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCountGreaterThan(0);
    }

    /// <summary>
    /// 测试ValidateObject方法对部分无效对象的验证
    /// </summary>
    [Fact]
    public void ValidateObject_Should_Return_Specific_Errors_For_Invalid_Properties()
    {
        // Arrange
        var partiallyInvalidObject = new TestValidationModel
        {
            RequiredProperty = "ValidValue",
            RangeProperty = 150, // 违反Range验证
            StringLengthProperty = "Valid"
        };

        // Act
        var result = partiallyInvalidObject.ValidateObject();

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain(error => error.Message.Contains("range"));
    }

    /// <summary>
    /// 测试ValidateProperty方法对有效属性的验证
    /// </summary>
    [Fact]
    public void ValidateProperty_Should_Return_Valid_Result_For_Valid_Property()
    {
        // Arrange
        var testObject = new TestValidationModel();
        var validValue = "ValidValue";

        // Act
        var result = testObject.ValidateProperty(nameof(TestValidationModel.RequiredProperty), validValue);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// 测试ValidateProperty方法对无效属性的验证
    /// </summary>
    [Fact]
    public void ValidateProperty_Should_Return_Invalid_Result_For_Invalid_Property()
    {
        // Arrange
        var testObject = new TestValidationModel();
        var invalidValue = (string?)null;

        // Act
        var result = testObject.ValidateProperty(nameof(TestValidationModel.RequiredProperty), invalidValue);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(error => error.Message.Contains("required"));
    }

    /// <summary>
    /// 测试ValidateProperty方法对范围属性的验证
    /// </summary>
    [Theory]
    [InlineData(0, true)]
    [InlineData(50, true)]
    [InlineData(100, true)]
    [InlineData(-1, false)]
    [InlineData(101, false)]
    public void ValidateProperty_Should_Validate_Range_Property_Correctly(int value, bool expectedValid)
    {
        // Arrange
        var testObject = new TestValidationModel();

        // Act
        var result = testObject.ValidateProperty(nameof(TestValidationModel.RangeProperty), value);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().Be(expectedValid);

        if (!expectedValid)
        {
            result.Errors.Should().NotBeEmpty();
        }
        else
        {
            result.Errors.Should().BeEmpty();
        }
    }

    /// <summary>
    /// 测试ValidateProperty方法对字符串长度属性的验证
    /// </summary>
    [Theory]
    [InlineData("", false)] // 太短
    [InlineData("Valid", true)]
    [InlineData("ValidLength", true)]
    [InlineData("ThisStringIsTooLong", false)] // 太长
    public void ValidateProperty_Should_Validate_StringLength_Property_Correctly(string value, bool expectedValid)
    {
        // Arrange
        var testObject = new TestValidationModel();

        // Act
        var result = testObject.ValidateProperty(nameof(TestValidationModel.StringLengthProperty), value);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().Be(expectedValid);

        if (!expectedValid)
        {
            result.Errors.Should().NotBeEmpty();
        }
        else
        {
            result.Errors.Should().BeEmpty();
        }
    }

    /// <summary>
    /// 测试ValidateObjects方法对多个对象的验证
    /// </summary>
    [Fact]
    public void ValidateObjects_Should_Validate_Multiple_Objects()
    {
        // Arrange
        var validObject = new TestValidationModel
        {
            RequiredProperty = "ValidValue",
            RangeProperty = 50,
            StringLengthProperty = "ValidLength"
        };

        var invalidObject = new TestValidationModel
        {
            RequiredProperty = null,
            RangeProperty = 150,
            StringLengthProperty = "TooLong"
        };

        var objects = new object[] { validObject, invalidObject };

        // Act
        var results = objects.ValidateObjects();

        // Assert
        results.Should().HaveCount(2);
        results[validObject].IsValid.Should().BeTrue();
        results[invalidObject].IsValid.Should().BeFalse();
    }

    /// <summary>
    /// 测试AllValid方法对验证结果的检查
    /// </summary>
    [Fact]
    public void AllValid_Should_Return_True_When_All_Results_Are_Valid()
    {
        // Arrange
        var validObject1 = new TestValidationModel
        {
            RequiredProperty = "ValidValue1",
            RangeProperty = 25,
            StringLengthProperty = "Valid1"
        };

        var validObject2 = new TestValidationModel
        {
            RequiredProperty = "ValidValue2",
            RangeProperty = 75,
            StringLengthProperty = "Valid2"
        };

        var objects = new object[] { validObject1, validObject2 };
        var results = objects.ValidateObjects();

        // Act
        var allValid = results.AllValid();

        // Assert
        allValid.Should().BeTrue();
    }

    /// <summary>
    /// 测试AllValid方法对包含无效结果的检查
    /// </summary>
    [Fact]
    public void AllValid_Should_Return_False_When_Any_Result_Is_Invalid()
    {
        // Arrange
        var validObject = new TestValidationModel
        {
            RequiredProperty = "ValidValue",
            RangeProperty = 50,
            StringLengthProperty = "Valid"
        };

        var invalidObject = new TestValidationModel
        {
            RequiredProperty = null,
            RangeProperty = 150,
            StringLengthProperty = "TooLong"
        };

        var objects = new object[] { validObject, invalidObject };
        var results = objects.ValidateObjects();

        // Act
        var allValid = results.AllValid();

        // Assert
        allValid.Should().BeFalse();
    }

    /// <summary>
    /// 测试GetAllErrors方法获取所有验证错误
    /// </summary>
    [Fact]
    public void GetAllErrors_Should_Return_All_Validation_Errors()
    {
        // Arrange
        var invalidObject1 = new TestValidationModel
        {
            RequiredProperty = null, // 1个错误
            RangeProperty = 50,
            StringLengthProperty = "Valid"
        };

        var invalidObject2 = new TestValidationModel
        {
            RequiredProperty = "Valid",
            RangeProperty = 150, // 1个错误
            StringLengthProperty = "ThisStringIsTooLongForValidation" // 1个错误
        };

        var objects = new object[] { invalidObject1, invalidObject2 };
        var results = objects.ValidateObjects();

        // Act
        var allErrors = results.GetAllErrors();

        // Assert
        allErrors.Should().NotBeEmpty();
        allErrors.Should().HaveCount(3); // 总共3个错误
        allErrors.Should().Contain(error => error.Contains("required"));
        allErrors.Should().Contain(error => error.Contains("range"));
        allErrors.Should().Contain(error => error.Contains("length"));
    }

    /// <summary>
    /// 测试GetAllErrors方法对有效对象的处理
    /// </summary>
    [Fact]
    public void GetAllErrors_Should_Return_Empty_List_For_Valid_Objects()
    {
        // Arrange
        var validObject1 = new TestValidationModel
        {
            RequiredProperty = "ValidValue1",
            RangeProperty = 25,
            StringLengthProperty = "Valid1"
        };

        var validObject2 = new TestValidationModel
        {
            RequiredProperty = "ValidValue2",
            RangeProperty = 75,
            StringLengthProperty = "Valid2"
        };

        var objects = new object[] { validObject1, validObject2 };
        var results = objects.ValidateObjects();

        // Act
        var allErrors = results.GetAllErrors();

        // Assert
        allErrors.Should().BeEmpty();
    }

    /// <summary>
    /// 测试验证扩展方法的性能特征
    /// </summary>
    [Fact]
    public void ValidationExtensions_Should_Execute_Efficiently()
    {
        // Arrange
        var testObjects = Enumerable.Range(0, 100)
                                   .Select(i => new TestValidationModel
                                   {
                                       RequiredProperty = $"Value{i}",
                                       RangeProperty = i % 101, // 0-100范围内
                                       StringLengthProperty = $"Valid{i}"
                                   })
                                   .Cast<object>()
                                   .ToArray();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 10; i++)
        {
            var results = testObjects.ValidateObjects();
            var allValid = results.AllValid();
            var allErrors = results.GetAllErrors();
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 应在1秒内完成
    }

    /// <summary>
    /// 测试验证扩展方法的线程安全性
    /// </summary>
    [Fact]
    public void ValidationExtensions_Should_Be_Thread_Safe()
    {
        // Arrange
        var testObject = new TestValidationModel
        {
            RequiredProperty = "ValidValue",
            RangeProperty = 50,
            StringLengthProperty = "Valid"
        };

        // Act
        var tasks = Enumerable.Range(0, 10)
                             .Select(_ => Task.Run(() =>
                             {
                                 var objectResult = testObject.ValidateObject();
                                 var propertyResult = testObject.ValidateProperty(nameof(TestValidationModel.RequiredProperty), "TestValue");
                                 var multipleResults = new object[] { testObject }.ValidateObjects();
                                 var allValid = multipleResults.AllValid();
                                 var allErrors = multipleResults.GetAllErrors();

                                 return new
                                 {
                                     ObjectValid = objectResult.IsValid,
                                     PropertyValid = propertyResult.IsValid,
                                     AllValid = allValid,
                                     ErrorCount = allErrors.Count
                                 };
                             }))
                             .ToArray();

        var results = Task.WhenAll(tasks).Result;

        // Assert
        results.Should().AllSatisfy(result =>
        {
            result.ObjectValid.Should().BeTrue();
            result.PropertyValid.Should().BeTrue();
            result.AllValid.Should().BeTrue();
            result.ErrorCount.Should().Be(0);
        });
    }

    /// <summary>
    /// 测试验证扩展方法对复杂对象的处理
    /// </summary>
    [Fact]
    public void ValidationExtensions_Should_Handle_Complex_Objects()
    {
        // Arrange
        var complexObject = new ComplexValidationModel
        {
            SimpleProperty = new TestValidationModel
            {
                RequiredProperty = "ValidValue",
                RangeProperty = 50,
                StringLengthProperty = "Valid"
            },
            ListProperty = new List<TestValidationModel>
            {
                new TestValidationModel
                {
                    RequiredProperty = "Item1",
                    RangeProperty = 25,
                    StringLengthProperty = "Valid1"
                },
                new TestValidationModel
                {
                    RequiredProperty = "Item2",
                    RangeProperty = 75,
                    StringLengthProperty = "Valid2"
                }
            }
        };

        // Act
        var result = complexObject.ValidateObject();

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}

/// <summary>
/// 测试用的验证模型
/// </summary>
public class TestValidationModel
{
    [Required(ErrorMessage = "RequiredProperty is required")]
    public string? RequiredProperty { get; set; }

    [Range(0, 100, ErrorMessage = "RangeProperty must be between 0 and 100")]
    public int RangeProperty { get; set; }

    [StringLength(15, MinimumLength = 3, ErrorMessage = "StringLengthProperty must be between 3 and 15 characters")]
    public string StringLengthProperty { get; set; } = "";
}

/// <summary>
/// 测试用的复杂验证模型
/// </summary>
public class ComplexValidationModel
{
    [Required]
    public TestValidationModel? SimpleProperty { get; set; }

    [Required]
    public List<TestValidationModel> ListProperty { get; set; } = new();
}