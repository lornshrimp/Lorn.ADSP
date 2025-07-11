# GeoTargeting类重命名迁移指南

## 概述

为了更明确地区分不同类型的地理定向功能，我们对地理定向相关类进行了重命名和重构。本指南将帮助您了解变更内容并完成代码迁移。

## 变更摘要

### 类名变更

| 旧类名 | 新类名 | 功能说明 |
|--------|--------|----------|
| `GeoTargeting` | `AdministrativeGeoTargeting` | 行政区划地理定向（国家、省份、城市） |
| - | `CircularGeoFenceTargeting` | 圆形地理围栏定向（已存在，无变更） |
| - | `PolygonGeoFenceTargeting` | 多边形地理围栏定向（已存在，无变更） |

### 文件路径变更

| 旧文件路径 | 新文件路径 |
|------------|------------|
| `Core\Lorn.ADSP.Core.Domain\ValueObjects\Targeting\GeoTargeting.cs` | `Core\Lorn.ADSP.Core.Domain\ValueObjects\Targeting\AdministrativeGeoTargeting.cs` |

## 详细变更内容

### 1. AdministrativeGeoTargeting 类

**变更前：**
```csharp
public class GeoTargeting : TargetingCriteriaBase
{
    public override string CriteriaType => "Geo";
    // ... 其他成员
}
```

**变更后：**
```csharp
public class AdministrativeGeoTargeting : TargetingCriteriaBase
{
    public override string CriteriaType => "AdministrativeGeo";
    // ... 其他成员（功能不变）
}
```

### 2. TargetingConfig 类更新

**变更前：**
```csharp
public class TargetingConfig : ValueObject
{
    public GeoTargeting? GeoTargeting { get; private set; }
    
    public static TargetingConfig Create(
        GeoTargeting? geoTargeting = null,
        // ... 其他参数
    )
}
```

**变更后：**
```csharp
public class TargetingConfig : ValueObject
{
    public AdministrativeGeoTargeting? AdministrativeGeoTargeting { get; private set; }
    public CircularGeoFenceTargeting? CircularGeoFenceTargeting { get; private set; }
    public PolygonGeoFenceTargeting? PolygonGeoFenceTargeting { get; private set; }
    
    public static TargetingConfig Create(
        AdministrativeGeoTargeting? administrativeGeoTargeting = null,
        CircularGeoFenceTargeting? circularGeoFenceTargeting = null,
        PolygonGeoFenceTargeting? polygonGeoFenceTargeting = null,
        // ... 其他参数
    )
    
    // 新增便捷方法
    public IEnumerable<ITargetingCriteria> GetEnabledGeoTargetingCriteria()
    public bool HasGeoTargeting()
}
```

### 3. TargetingPolicy 类更新

**变更前：**
```csharp
public GeoTargeting? GeoTargeting => GetCriteria<GeoTargeting>("Geo");
```

**变更后：**
```csharp
public AdministrativeGeoTargeting? AdministrativeGeoTargeting => GetCriteria<AdministrativeGeoTargeting>("AdministrativeGeo");
public CircularGeoFenceTargeting? CircularGeoFenceTargeting => GetCriteria<CircularGeoFenceTargeting>("CircularGeoFence");
public PolygonGeoFenceTargeting? PolygonGeoFenceTargeting => GetCriteria<PolygonGeoFenceTargeting>("PolygonGeoFence");

// 新增便捷方法
public IEnumerable<ITargetingCriteria> GetGeoTargetingCriteria()
public bool HasGeoTargeting()
```

## 迁移步骤

### 步骤 1：更新引用

在所有使用 `GeoTargeting` 的代码文件中：

1. 将 `GeoTargeting` 替换为 `AdministrativeGeoTargeting`
2. 如果使用 `CriteriaType` 进行匹配，将 `"Geo"` 替换为 `"AdministrativeGeo"`

### 步骤 2：更新创建代码

**变更前：**
```csharp
var geoTargeting = GeoTargeting.Create(
    includedLocations: cities,
    mode: GeoTargetingMode.Include
);

var config = TargetingConfig.Create(
    geoTargeting: geoTargeting
);
```

**变更后：**
```csharp
var adminGeoTargeting = AdministrativeGeoTargeting.Create(
    includedLocations: cities,
    mode: GeoTargetingMode.Include
);

var config = TargetingConfig.Create(
    administrativeGeoTargeting: adminGeoTargeting
);
```

### 步骤 3：更新访问代码

**变更前：**
```csharp
if (config.GeoTargeting != null)
{
    var mode = config.GeoTargeting.Mode;
}

if (policy.GeoTargeting != null)
{
    var locations = policy.GeoTargeting.IncludedLocations;
}
```

**变更后：**
```csharp
if (config.AdministrativeGeoTargeting != null)
{
    var mode = config.AdministrativeGeoTargeting.Mode;
}

if (policy.AdministrativeGeoTargeting != null)
{
    var locations = policy.AdministrativeGeoTargeting.IncludedLocations;
}
```

### 步骤 4：考虑多种地理定向组合

新架构支持同时使用多种地理定向类型：

```csharp
var config = TargetingConfig.Create(
    administrativeGeoTargeting: adminTargeting,     // 行政区划粗筛
    circularGeoFenceTargeting: circularTargeting,   // 商圈精准定向
    polygonGeoFenceTargeting: polygonTargeting      // 复杂区域定向
);

// 检查是否有地理定向
if (config.HasGeoTargeting())
{
    var geoTargetingTypes = config.GetEnabledGeoTargetingCriteria()
        .Select(c => c.CriteriaType);
    Console.WriteLine($"启用的地理定向类型: {string.Join(", ", geoTargetingTypes)}");
}
```

## 兼容性说明

### 数据库迁移

如果您的数据库中存储了序列化的定向配置，可能需要进行数据迁移：

1. **CriteriaType 更新**：将存储的 `"Geo"` 更新为 `"AdministrativeGeo"`
2. **类名更新**：将序列化数据中的类名从 `GeoTargeting` 更新为 `AdministrativeGeoTargeting`

### API 兼容性

如果您有对外的API接口，建议：

1. 保持向后兼容性，同时支持新旧字段名
2. 在API文档中标记旧字段为已弃用
3. 提供迁移时间表

## 验证迁移

完成迁移后，请进行以下验证：

1. **编译验证**：确保所有项目都能成功编译
```bash
dotnet build Lorn.ADSP.sln
```

2. **单元测试验证**：运行相关的单元测试
```bash
dotnet test Lorn.ADSP.sln --filter="Category=GeoTargeting"
```

3. **功能验证**：确保地理定向功能正常工作
   - 行政区划定向测试
   - 圆形围栏定向测试
   - 多边形围栏定向测试

## 常见问题

### Q: 为什么要进行这次重命名？

A: 原来的 `GeoTargeting` 名称过于宽泛，无法明确表示其具体功能。重命名为 `AdministrativeGeoTargeting` 后，可以清楚地表明这是基于行政区划的地理定向，与基于坐标的围栏定向区分开来。

### Q: 旧的功能会有变化吗？

A: 不会。`AdministrativeGeoTargeting` 的所有功能都与原来的 `GeoTargeting` 完全相同，只是类名和 CriteriaType 发生了变更。

### Q: 如何选择合适的地理定向类型？

A: 
- **AdministrativeGeoTargeting**：适合按城市、省份、国家进行的大范围定向
- **CircularGeoFenceTargeting**：适合商圈、POI周边的精准定向
- **PolygonGeoFenceTargeting**：适合复杂形状区域的精确定向

### Q: 可以同时使用多种地理定向吗？

A: 可以。新架构支持在同一个 `TargetingConfig` 中同时配置多种地理定向类型，系统会按照各自的规则进行匹配。

## 技术支持

如果在迁移过程中遇到问题，请：

1. 查看本迁移指南
2. 参考 `docs_TecDesign/地理定向系统设计说明.md`
3. 联系技术团队获取支持

---

**重要提醒**：请在生产环境部署前，在测试环境中充分验证迁移结果！