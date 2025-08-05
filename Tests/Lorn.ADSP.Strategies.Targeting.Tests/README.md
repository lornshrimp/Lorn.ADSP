# Lorn.ADSP.Strategies.Targeting.Tests

## 项目概述

这是F#定向匹配策略库的单元测试项目，使用xUnit和FsUnit框架进行测试。

## 项目结构

```
Tests/Lorn.ADSP.Strategies.Targeting.Tests/
├── Core/
│   └── MatchingAlgorithmsTests.fs          # F#通用匹配算法模块测试
├── TestRunner.fs                           # 测试运行器入口模块
├── Lorn.ADSP.Strategies.Targeting.Tests.fsproj  # F#测试项目文件
└── README.md                               # 项目说明文档
```

## 测试框架

- **xUnit**: 主要测试框架
- **FsUnit**: F#友好的断言库，提供流畅的测试语法
- **Microsoft.NET.Test.Sdk**: .NET测试SDK

## 运行测试

### 构建测试项目
```bash
dotnet build Tests/Lorn.ADSP.Strategies.Targeting.Tests/Lorn.ADSP.Strategies.Targeting.Tests.fsproj
```

### 运行所有测试
```bash
dotnet test Tests/Lorn.ADSP.Strategies.Targeting.Tests/Lorn.ADSP.Strategies.Targeting.Tests.fsproj
```

### 运行测试并显示详细输出
```bash
dotnet test Tests/Lorn.ADSP.Strategies.Targeting.Tests/Lorn.ADSP.Strategies.Targeting.Tests.fsproj --verbosity normal
```

### 运行测试并生成覆盖率报告
```bash
dotnet test Tests/Lorn.ADSP.Strategies.Targeting.Tests/Lorn.ADSP.Strategies.Targeting.Tests.fsproj --collect:"XPlat Code Coverage"
```

## 测试覆盖范围

### Core/MatchingAlgorithmsTests.fs

测试F#通用匹配算法模块的核心功能：

- ✅ 模块版本和摘要信息获取
- ✅ 默认配置验证
- ✅ 匹配结果创建（成功和失败）
- ✅ 加权分数计算
- ✅ 分数归一化算法
- ✅ 时间衰减计算
- ✅ 快速失败检查机制
- ✅ 匹配上下文验证
- ✅ 性能统计信息创建
- ✅ 多个匹配结果聚合

## 测试数据

测试使用模拟数据和边界条件来验证算法的正确性：

- 使用Dictionary<string, obj>模拟匹配详情
- 使用TimeSpan模拟执行时间
- 使用decimal类型进行精确的分数计算
- 测试各种边界条件和异常情况

## 依赖项目

- `Lorn.ADSP.Strategies.Targeting`: 被测试的F#策略库
- `Lorn.ADSP.Core.AdEngine.Abstractions`: 广告引擎抽象接口
- `Lorn.ADSP.Core.Domain.Targeting`: 定向领域模型
- `Lorn.ADSP.Core.Shared`: 共享组件

## 测试最佳实践

1. **命名规范**: 使用中文描述测试场景，格式为`被测试方法_应该_预期结果`
2. **AAA模式**: 遵循Arrange-Act-Assert模式组织测试代码
3. **独立性**: 每个测试方法独立运行，不依赖其他测试
4. **边界测试**: 包含正常情况、边界条件和异常情况的测试
5. **可读性**: 测试代码清晰易懂，便于维护和理解

## 持续集成

该测试项目已集成到解决方案的构建流程中，支持：

- 自动化构建验证
- 持续集成测试执行
- 代码覆盖率报告生成
- 测试结果报告