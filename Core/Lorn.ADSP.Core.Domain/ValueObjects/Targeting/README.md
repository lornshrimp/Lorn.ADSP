# ITargetingCriteria 和 ITargetingContext 接口使用指南

## 概述

本文档介绍了在 `Lorn.ADSP.Core.Domain` 项目中新实现的 `ITargetingCriteria` 和 `ITargetingContext` 接口的使用方法。这两个接口为广告定向策略提供了统一的抽象层，支持灵活的定向条件配置和上下文数据管理。

## 接口设计

### ITargetingCriteria 接口

定向条件抽象接口，用于定义统一的定向规则配置：

```csharp
public interface ITargetingCriteria
{
    string CriteriaType { get; }
    IReadOnlyDictionary<string, object> Rules { get; }
    decimal Weight { get; }
    bool IsEnabled { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
    
    T? GetRule<T>(string ruleKey);
    T GetRule<T>(string ruleKey, T defaultValue);
    bool HasRule(string ruleKey);
    IReadOnlyCollection<string> GetRuleKeys();
    bool IsValid();
    string GetConfigurationSummary();
}
```

### ITargetingContext 接口

定向上下文抽象接口，用于统一上下文数据访问：

```csharp
public interface ITargetingContext
{
    string ContextType { get; }
    IReadOnlyDictionary<string, object> Properties { get; }
    DateTime Timestamp { get; }
    string ContextId { get; }
    string DataSource { get; }
    
    T? GetProperty<T>(string propertyKey);
    T GetProperty<T>(string propertyKey, T defaultValue);
    string GetPropertyAsString(string propertyKey);
    bool HasProperty(string propertyKey);
    IReadOnlyCollection<string> GetPropertyKeys();
    bool IsValid();
    bool IsExpired(TimeSpan maxAge);
    IReadOnlyDictionary<string, object> GetMetadata();
    string GetDebugInfo();
    ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys);
    ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false);
}
```

## 实现类

### 1. 地理定向条件 (GeoTargetingCriteria)

```csharp
// 创建地理定向条件
var geoLocations = new List<GeoInfo>
{
    GeoInfo.Create("CN", "中国", "北京"),
    GeoInfo.Create("CN", "中国", "上海")
};

var geoCriteria = GeoTargetingCriteria.Create(
    includedLocations: geoLocations,
    mode: GeoTargetingMode.Include,
    maxDistance: 50.0, // 50公里范围内
    weight: 1.0m,
    isEnabled: true
);

// 使用接口方法
Console.WriteLine($"条件类型: {geoCriteria.CriteriaType}");
Console.WriteLine($"配置摘要: {geoCriteria.GetConfigurationSummary()}");
Console.WriteLine($"最大距离: {geoCriteria.GetRule<double?>("MaxDistance")}");
```

### 2. 用户定向上下文 (UserTargetingContext)

```csharp
// 创建用户画像
var basicInfo = UserBasicInfo.CreateBasic("张三", Gender.Male, new DateTime(1990, 1, 1));
var userProfile = UserProfile.Create("user123", basicInfo);

// 创建地理位置信息
var geoInfo = GeoInfo.Create("CN", "中国", "北京");

// 创建设备信息
var deviceInfo = DeviceInfo.CreateMobile("Android", "Samsung", "Galaxy S21");

// 创建用户定向上下文
var userContext = UserTargetingContext.Create(
    userId: "user123",
    userProfile: userProfile,
    geoLocation: geoInfo,
    deviceInfo: deviceInfo,
    requestTime: DateTime.UtcNow,
    dataSource: "UserService"
);

// 使用接口方法
Console.WriteLine($"上下文类型: {userContext.ContextType}");
Console.WriteLine($"用户年龄: {userContext.GetUserAge()}");
Console.WriteLine($"是否移动设备: {userContext.IsMobileDevice()}");
Console.WriteLine($"位置描述: {userContext.GetLocationDescription()}");
```

### 3. 从 AdContext 转换

```csharp
// 从现有的 AdContext 创建用户定向上下文
public static UserTargetingContext ConvertFromAdContext(AdContext adContext)
{
    return UserTargetingContext.FromAdContext(adContext);
}

// 使用示例
var adContext = AdContext.Create(
    requestId: "req123",
    userId: "user456",
    placementId: "placement789",
    device: deviceInfo,
    geoLocation: geoInfo,
    userAgent: "Mozilla/5.0...",
    requestTime: DateTime.UtcNow,
    timeWindow: timeWindow,
    requestSource: RequestSource.Website,
    adSize: AdSize.CreateStandard(StandardAdSize.Banner)
);

var targetingContext = UserTargetingContext.FromAdContext(adContext);
```

## 扩展自定义实现

### 创建自定义定向条件

```csharp
public class InterestTargetingCriteria : TargetingCriteriaBase
{
    public override string CriteriaType => "Interest";
    
    public IReadOnlyList<string> TargetInterests => 
        GetRule<List<string>>("TargetInterests") ?? new List<string>();
    
    public decimal MinMatchThreshold => 
        GetRule<decimal>("MinMatchThreshold", 0.6m);
    
    public InterestTargetingCriteria(
        IList<string> targetInterests,
        decimal minMatchThreshold = 0.6m,
        decimal weight = 1.0m,
        bool isEnabled = true) 
        : base(CreateRules(targetInterests, minMatchThreshold), weight, isEnabled)
    {
    }
    
    private static Dictionary<string, object> CreateRules(
        IList<string> targetInterests,
        decimal minMatchThreshold)
    {
        return new Dictionary<string, object>
        {
            ["TargetInterests"] = targetInterests?.ToList() ?? new List<string>(),
            ["MinMatchThreshold"] = minMatchThreshold
        };
    }
    
    protected override bool ValidateSpecificRules()
    {
        return TargetInterests.Any() && 
               MinMatchThreshold >= 0 && 
               MinMatchThreshold <= 1;
    }
}
```

### 创建自定义上下文

```csharp
public class BehaviorTargetingContext : TargetingContextBase
{
    public IReadOnlyList<string> RecentActions => 
        GetProperty<List<string>>("RecentActions") ?? new List<string>();
    
    public TimeSpan SessionDuration => 
        GetProperty<TimeSpan>("SessionDuration", TimeSpan.Zero);
    
    public int PageViews => 
        GetProperty<int>("PageViews", 0);
    
    public BehaviorTargetingContext(
        IList<string> recentActions,
        TimeSpan sessionDuration,
        int pageViews,
        string dataSource = "BehaviorService") 
        : base("Behavior", CreateProperties(recentActions, sessionDuration, pageViews), dataSource)
    {
    }
    
    private static Dictionary<string, object> CreateProperties(
        IList<string> recentActions,
        TimeSpan sessionDuration,
        int pageViews)
    {
        return new Dictionary<string, object>
        {
            ["RecentActions"] = recentActions?.ToList() ?? new List<string>(),
            ["SessionDuration"] = sessionDuration,
            ["PageViews"] = pageViews
        };
    }
}
```

## 使用最佳实践

### 1. 类型安全访问

```csharp
// 推荐：使用类型安全的方法
var maxDistance = geoCriteria.GetRule<double?>("MaxDistance");
if (maxDistance.HasValue)
{
    // 处理距离逻辑
}

// 推荐：提供默认值
var threshold = criteria.GetRule<decimal>("Threshold", 0.5m);
```

### 2. 数据验证

```csharp
// 始终验证数据有效性
if (!criteria.IsValid())
{
    throw new InvalidOperationException("定向条件配置无效");
}

if (context.IsExpired(TimeSpan.FromMinutes(30)))
{
    // 刷新或重新获取上下文数据
}
```

### 3. 性能优化

```csharp
// 创建轻量级上下文副本
var lightweightContext = userContext.CreateLightweightCopy(new[] 
{
    "UserId", "GeoLocation", "DeviceInfo"
});

// 合并多个上下文
var mergedContext = userContext.Merge(behaviorContext, overwriteExisting: false);
```

### 4. 调试和日志

```csharp
// 获取调试信息
logger.LogDebug("定向条件: {Summary}", criteria.GetConfigurationSummary());
logger.LogDebug("上下文信息: {DebugInfo}", context.GetDebugInfo());

// 获取元数据
var metadata = context.GetMetadata();
foreach (var item in metadata)
{
    logger.LogTrace("元数据 {Key}: {Value}", item.Key, item.Value);
}
```

## 匹配结果处理

```csharp
// 创建匹配结果
var matchResult = TargetingMatchResult.Success(
    score: 0.85m,
    matcherType: "GeoMatcher",
    criteriaType: "Geo",
    executionTime: TimeSpan.FromMilliseconds(15),
    details: new Dictionary<string, object>
    {
        ["MatchedCity"] = "北京",
        ["Distance"] = 12.5
    }
);

// 处理匹配结果
if (matchResult.IsMatch && matchResult.Score > 0.7m)
{
    Console.WriteLine($"匹配成功: {matchResult}");
    var matchedCity = matchResult.GetDetail<string>("MatchedCity");
    Console.WriteLine($"匹配城市: {matchedCity}");
}
```

## 总结

这套接口设计提供了：

1. **统一的抽象层**：所有定向条件和上下文都实现统一接口
2. **类型安全**：强类型的数据访问方法
3. **可扩展性**：支持自定义实现各种定向类型
4. **性能优化**：轻量级上下文和高效的数据访问
5. **调试支持**：丰富的调试和元数据信息

通过这些接口，广告定向系统可以实现更灵活、更可维护的定向策略管理。