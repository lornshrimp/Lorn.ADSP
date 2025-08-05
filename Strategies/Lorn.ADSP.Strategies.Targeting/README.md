# Lorn.ADSP.Strategies.Targeting

## 项目概述

这是Lorn.ADSP广告召回逻辑引擎的F#定向匹配策略实现项目。该项目使用F#函数式编程语言实现高性能、高并发的定向匹配算法，为广告投放引擎提供精准的定向筛选能力。

## 项目结构

```
Lorn.ADSP.Strategies.Targeting/
├── Core/                          # 核心类型定义和算法
│   ├── TargetingTypes.fs          # F#核心类型定义
│   └── MatchingAlgorithms.fs      # 通用匹配算法
├── Matchers/                      # 各种定向匹配器实现
│   └── (待实现的匹配器文件)
├── Utils/                         # 工具函数模块
│   ├── DataValidation.fs          # 数据验证工具
│   ├── CacheHelpers.fs            # 缓存辅助函数
│   └── MetricsCollector.fs        # 指标收集工具
├── Interop/                       # C#互操作层
│   ├── CSharpInterop.fs           # C#互操作功能
│   └── TypeConverters.fs          # 类型转换器
└── Library.fs                     # 主入口模块
```

## 技术特性

- **函数式编程**: 使用F#的函数式编程特性实现纯函数算法
- **高性能**: 利用F#的并行计算和异步工作流提升性能
- **类型安全**: 使用F#的强类型系统确保代码安全性
- **互操作性**: 与现有C#代码库无缝集成

## 依赖项目

- `Lorn.ADSP.Core.AdEngine.Abstractions`: 广告引擎抽象层接口
- `Lorn.ADSP.Core.Domain.Targeting`: 定向领域模型
- `Lorn.ADSP.Core.Shared`: 共享组件和工具

## NuGet包依赖

- `FSharp.Collections.ParallelSeq`: 并行集合处理
- `FSharp.Data`: 数据访问和序列化
- `FSharp.Control.AsyncSeq`: 异步序列处理
- `MathNet.Numerics.FSharp`: 数值计算

## 构建和运行

```bash
# 构建项目
dotnet build Strategies/Lorn.ADSP.Strategies.Targeting/Lorn.ADSP.Strategies.Targeting.fsproj

# 运行测试（待实现）
dotnet test

# 发布项目
dotnet publish -c Release
```

## 开发状态

当前项目处于基础架构搭建阶段，已完成：

- [x] 项目结构创建
- [x] 基础依赖配置
- [x] 核心模块占位符
- [ ] 具体匹配器实现
- [ ] 单元测试
- [ ] 集成测试
- [ ] 性能测试

## 后续开发计划

1. 实现核心类型定义（TargetingTypes.fs）
2. 实现通用匹配算法（MatchingAlgorithms.fs）
3. 实现各种定向匹配器
4. 完善C#互操作层
5. 添加单元测试和集成测试
6. 性能优化和监控集成

## 贡献指南

请遵循项目的编码规范和中文注释要求：

- 所有F#代码注释必须使用中文
- 函数和类型命名使用英文，但提供中文注释说明
- 遵循F#函数式编程最佳实践
- 确保代码的类型安全和性能优化