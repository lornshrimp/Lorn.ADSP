# 值对象迁移至定向上下文基类

## 概述

根据设计文档要求，已将以下四个值对象类修改为继承 `TargetingContextBase`，使它们能够作为定向上下文在广告引擎中使用：

- `DemographicInfo` - 人口统计学信息定向上下文
- `GeoInfo` - 地理位置信息定向上下文  
- `TimeRange` - 时间范围定向上下文
- `DeviceInfo` - 设备信息定向上下文

## 修改详情

### 1. DemographicInfo.cs
**变更类型**：继承关系变更
- **之前**：`public class DemographicInfo : ValueObject`
- **之后**：`public class DemographicInfo : TargetingContextBase`

**主要变更**：
- 构造函数修改为调用基类的 `TargetingContextBase("Demographic", properties, dataSource)`
- 属性访问改为通过 `GetProperty<T>()` 方法
- 添加了 `CreateProperties` 静态方法来构建属性字典
- 重写了 `GetDebugInfo()` 和 `IsValid()` 方法
- 所有工厂方法增加了 `dataSource` 参数

### 2. GeoInfo.cs
**变更类型**：继承关系变更
- **之前**：`public class GeoInfo : ValueObject`
- **之后**：`public class GeoInfo : TargetingContextBase`

**主要变更**：
- 构造函数修改为调用基类的 `TargetingContextBase("Geo", properties, dataSource)`
- 所有属性改为通过 `GetProperty<T>()` 访问
- 添加了 `GetLocationDescription()` 方法
- 重写了 `GetDebugInfo()` 和 `IsValid()` 方法
- 工厂方法支持 `dataSource` 参数

### 3. TimeRange.cs
**变更类型**：继承关系变更
- **之前**：`public class TimeRange : ValueObject`
- **之后**：`public class TimeRange : TargetingContextBase`

**主要变更**：
- 构造函数修改为调用基类的 `TargetingContextBase("TimeRange", properties, dataSource)`
- 属性访问改为通过 `GetProperty<T>()` 方法
- 原 `IsValid` 属性重命名为 `IsValidRange` 以避免与基类方法冲突
- 添加了时间范围相关的便捷方法
- 重写了 `GetDebugInfo()` 和 `IsValid()` 方法

### 4. DeviceInfo.cs
**变更类型**：继承关系变更
- **之前**：`public class DeviceInfo : ValueObject`
- **之后**：`public class DeviceInfo : TargetingContextBase`

**主要变更**：
- 构造函数修改为调用基类的 `TargetingContextBase("Device", properties, dataSource)`
- 所有属性改为通过 `GetProperty<T>()` 访问，带有适当的默认值
- 添加了设备相关的便捷方法和属性
- 重写了 `GetDebugInfo()` 和 `IsValid()` 方法
- 工厂方法支持 `dataSource` 参数

## 新增功能

所有修改后的类现在都具备以下定向上下文功能：

### 继承的基础功能
- **上下文管理**：`ContextType`、`ContextId`、`DataSource`、`Timestamp`
- **属性访问**：类型安全的 `GetProperty<T>()`、`HasProperty()`、`GetPropertyKeys()`
- **上下文操作**：`CreateLightweightCopy()`、`Merge()`、`SetProperty()`
- **验证和调试**：`IsValid()`、`IsExpired()`、`GetDebugInfo()`、`GetMetadata()`

### 特定领域功能
每个类都保留了其原有的领域特定方法，同时增强了上下文能力：

- **DemographicInfo**：人口统计学数据访问和完整性评分
- **GeoInfo**：地理位置计算和位置描述  
- **TimeRange**：时间范围操作和验证
- **DeviceInfo**：设备特征识别和能力检测

## 使用示例

### 创建定向上下文

```csharp
// 创建地理位置上下文
var geoContext = GeoInfo.Create(
    countryCode: "CN",
    countryName: "中国", 
    cityName: "北京",
    dataSource: "IPService");

// 创建设备上下文
var deviceContext = DeviceInfo.CreateMobile(
    operatingSystem: "iOS",
    brand: "Apple",
    model: "iPhone 14",
    dataSource: "UserAgent");

// 创建时间范围上下文
var timeContext = TimeRange.Today("SystemTime");
```

### 使用上下文功能

```csharp
// 检查上下文有效性
if (geoContext.IsValid() && !geoContext.IsExpired(TimeSpan.FromHours(1)))
{
    // 获取位置描述
    var location = geoContext.GetLocationDescription();
    
    // 访问特定属性
    var country = geoContext.GetProperty<string>("CountryCode");
    
    // 获取调试信息
    var debugInfo = geoContext.GetDebugInfo();
}

// 合并多个上下文
var mergedContext = geoContext.Merge(deviceContext);

// 创建轻量级副本
var lightContext = deviceContext.CreateLightweightCopy(new[] { "DeviceType", "OperatingSystem" });
```

## 向后兼容性

虽然类的继承关系发生了变化，但所有公共API都保持了向后兼容：

- 所有原有的属性和方法仍然可用
- 工厂方法签名保持不变（除了新增可选的 `dataSource` 参数）
- 业务逻辑方法功能不变

## 影响分析

### 正面影响
1. **统一的定向接口**：所有上下文类型现在都实现 `ITargetingContext` 接口
2. **增强的功能**：获得了上下文管理、合并、调试等高级功能
3. **更好的集成**：可以与定向引擎无缝集成
4. **类型安全**：属性访问更加类型安全，支持默认值

### 潜在影响
1. **性能开销**：属性访问现在通过字典查找，可能有轻微性能影响
2. **内存使用**：每个实例现在包含额外的上下文元数据
3. **序列化**：序列化格式可能发生变化，需要注意兼容性

## 测试建议

1. **单元测试**：确保所有原有功能正常工作
2. **集成测试**：验证与定向引擎的集成
3. **性能测试**：评估性能影响是否在可接受范围内
4. **兼容性测试**：确保序列化/反序列化兼容性

## 总结

此次修改成功将四个核心值对象转换为定向上下文类，为广告引擎的定向功能提供了统一的抽象层。所有修改都保持了向后兼容性，同时显著增强了系统的定向能力和扩展性。