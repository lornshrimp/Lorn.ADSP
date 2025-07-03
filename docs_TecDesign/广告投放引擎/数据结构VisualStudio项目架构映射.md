## 10. Visual Studio项目架构映射

### 10.1 领域模型层项目映射

#### 10.1.1 核心实体项目分布

| 实体名称       | 所属项目                | 项目类型             | 具体类文件路径                  | 实现要点                   |
| -------------- | ----------------------- | -------------------- | ------------------------------- | -------------------------- |
| Advertisement  | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/Entities/Advertisement.cs`    | 实体基类继承、领域逻辑封装 |
| Campaign       | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/Entities/Campaign.cs`         | 聚合根设计、业务规则验证   |
| Advertiser     | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/Entities/Advertiser.cs`       | 实体标识、业务状态管理     |
| MediaResource  | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/Entities/MediaResource.cs`    | 媒体属性建模、配置管理     |
| DeliveryRecord | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/Aggregates/DeliveryRecord.cs` | 聚合设计、事件发布         |

```csharp
// 示例：Advertisement实体实现指导
namespace Lorn.ADSP.Core.Domain.Entities
{
    public class Advertisement : EntityBase<int>, IAggregateRoot
    {
        // 基础属性
        public string Title { get; private set; }
        public AdStatus Status { get; private set; }
        public TargetingPolicy TargetingPolicy { get; private set; }
        
        // 领域行为
        public void SubmitForReview()
        {
            // 业务逻辑验证
            // 状态变更
            // 领域事件发布
        }
        
        // 工厂方法
        public static Advertisement Create(string title, int advertiserId)
        {
            // 创建逻辑
        }
    }
}
```

#### 10.1.2 值对象项目分布

| 值对象名称      | 所属项目                | 项目类型             | 具体类文件路径                     | 实现要点             |
| --------------- | ----------------------- | -------------------- | ---------------------------------- | -------------------- |
| TargetingPolicy | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/ValueObjects/TargetingPolicy.cs` | 不可变性、相等性比较 |
| DeliveryPolicy  | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/ValueObjects/DeliveryPolicy.cs`  | 结构化数据、验证逻辑 |
| AuditInfo       | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/ValueObjects/AuditInfo.cs`       | 审核状态封装、状态机 |
| GeoLocation     | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/ValueObjects/GeoLocation.cs`     | 地理坐标、距离计算   |
| BudgetInfo      | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/ValueObjects/BudgetInfo.cs`      | 预算计算、约束验证   |

```csharp
// 示例：TargetingPolicy值对象实现指导
namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    public class TargetingPolicy : ValueObject
    {
        public GeoTargeting GeoTargeting { get; }
        public DemographicTargeting DemographicTargeting { get; }
        public DeviceTargeting DeviceTargeting { get; }
        public TimeTargeting TimeTargeting { get; }
        
        public TargetingPolicy(
            GeoTargeting geoTargeting,
            DemographicTargeting demographicTargeting,
            DeviceTargeting deviceTargeting,
            TimeTargeting timeTargeting)
        {
            // 验证逻辑
            // 不可变性保证
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return GeoTargeting;
            yield return DemographicTargeting;
            yield return DeviceTargeting;
            yield return TimeTargeting;
        }
    }
}
```

#### 10.1.3 领域事件项目分布

| 事件名称             | 所属项目                | 项目类型             | 具体类文件路径                    | 实现要点           |
| -------------------- | ----------------------- | -------------------- | --------------------------------- | ------------------ |
| AdCreatedEvent       | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/Events/AdCreatedEvent.cs`       | 事件数据、时间戳   |
| AdApprovedEvent      | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/Events/AdApprovedEvent.cs`      | 审核结果、操作人   |
| BudgetExhaustedEvent | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/Events/BudgetExhaustedEvent.cs` | 预算信息、告警级别 |
| DeliverySuccessEvent | `Lorn.ADSP.Core.Domain` | .NET 9 Class Library | `/Events/DeliverySuccessEvent.cs` | 投放结果、统计数据 |

### 10.2 数据传输层项目映射

#### 10.2.1 API传输对象分布

| DTO类型            | 所属项目              | 项目类型             | 具体类文件路径                         | 实现要点             |
| ------------------ | --------------------- | -------------------- | -------------------------------------- | -------------------- |
| AdRequestDTO       | `Lorn.ADSP.AdEngine`  | ASP.NET Core Web API | `/DTOs/Requests/AdRequestDTO.cs`       | 数据验证、格式转换   |
| AdResponseDTO      | `Lorn.ADSP.AdEngine`  | ASP.NET Core Web API | `/DTOs/Responses/AdResponseDTO.cs`     | 序列化优化、字段筛选 |
| BidRequestDTO      | `Lorn.ADSP.Bidding`   | ASP.NET Core Web API | `/DTOs/Requests/BidRequestDTO.cs`      | OpenRTB兼容性        |
| CampaignCommandDTO | `Lorn.ADSP.Campaign`  | ASP.NET Core Web API | `/DTOs/Commands/CampaignCommandDTO.cs` | 命令验证、业务规则   |
| TargetingQueryDTO  | `Lorn.ADSP.Targeting` | ASP.NET Core Web API | `/DTOs/Queries/TargetingQueryDTO.cs`   | 查询优化、索引友好   |

```csharp
// 示例：AdRequestDTO实现指导
namespace Lorn.ADSP.AdEngine.DTOs.Requests
{
    public class AdRequestDTO
    {
        [Required]
        [StringLength(50)]
        public string PlacementId { get; set; }
        
        [Required]
        public DeviceInfoDTO Device { get; set; }
        
        [Required]
        public GeoLocationDTO GeoLocation { get; set; }
        
        public UserProfileDTO UserProfile { get; set; }
        
        // 验证方法
        public bool IsValid(out List<string> errors)
        {
            errors = new List<string>();
            // 业务验证逻辑
            return errors.Count == 0;
        }
        
        // 转换方法
        public AdRequest ToAdRequest()
        {
            // DTO到领域对象转换
        }
    }
}
```

#### 10.2.2 服务间通信对象分布

| 通信对象             | 所属项目                | 项目类型             | 具体类文件路径                      | 实现要点               |
| -------------------- | ----------------------- | -------------------- | ----------------------------------- | ---------------------- |
| BiddingMessage       | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Messages/BiddingMessage.cs`       | Protocol Buffers序列化 |
| TargetingMessage     | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Messages/TargetingMessage.cs`     | 轻量级数据结构         |
| DeliveryNotification | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Messages/DeliveryNotification.cs` | 事件通知格式           |
| StatisticsMessage    | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Messages/StatisticsMessage.cs`    | 批量数据传输           |

### 10.3 外部协议对象项目映射

#### 10.3.1 OpenRTB协议对象分布

| OpenRTB对象 | 所属项目                     | 项目类型             | 具体类文件路径           | 实现要点            |
| ----------- | ---------------------------- | -------------------- | ------------------------ | ------------------- |
| BidRequest  | `Lorn.ADSP.External.OpenRTB` | .NET 9 Class Library | `/Models/BidRequest.cs`  | OpenRTB 2.5标准实现 |
| BidResponse | `Lorn.ADSP.External.OpenRTB` | .NET 9 Class Library | `/Models/BidResponse.cs` | JSON序列化优化      |
| Impression  | `Lorn.ADSP.External.OpenRTB` | .NET 9 Class Library | `/Models/Impression.cs`  | 扩展字段支持        |
| User        | `Lorn.ADSP.External.OpenRTB` | .NET 9 Class Library | `/Models/User.cs`        | 隐私保护实现        |
| Device      | `Lorn.ADSP.External.OpenRTB` | .NET 9 Class Library | `/Models/Device.cs`      | 设备识别优化        |

```csharp
// 示例：BidRequest OpenRTB对象实现指导
namespace Lorn.ADSP.External.OpenRTB.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BidRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("imp")]
        public List<Impression> Impressions { get; set; }
        
        [JsonProperty("user")]
        public User User { get; set; }
        
        [JsonProperty("device")]
        public Device Device { get; set; }
        
        [JsonProperty("ext")]
        public Dictionary<string, object> Extensions { get; set; }
        
        // 验证方法
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Id) && 
                   Impressions?.Any() == true;
        }
        
        // 转换方法
        public AdRequest ToInternalAdRequest()
        {
            // 外部协议到内部对象转换
        }
    }
}
```

#### 10.3.2 VAST协议对象分布

| VAST对象       | 所属项目                  | 项目类型             | 具体类文件路径              | 实现要点           |
| -------------- | ------------------------- | -------------------- | --------------------------- | ------------------ |
| VASTDocument   | `Lorn.ADSP.External.VAST` | .NET 9 Class Library | `/Models/VASTDocument.cs`   | XML序列化、DOM解析 |
| AdSystem       | `Lorn.ADSP.External.VAST` | .NET 9 Class Library | `/Models/AdSystem.cs`       | 系统标识、版本管理 |
| Creative       | `Lorn.ADSP.External.VAST` | .NET 9 Class Library | `/Models/Creative.cs`       | 多媒体资源管理     |
| MediaFile      | `Lorn.ADSP.External.VAST` | .NET 9 Class Library | `/Models/MediaFile.cs`      | 编码格式、质量适配 |
| TrackingEvents | `Lorn.ADSP.External.VAST` | .NET 9 Class Library | `/Models/TrackingEvents.cs` | 事件监测、URL管理  |

### 10.4 数据访问层项目映射

#### 10.4.1 实体框架映射对象分布

| 映射对象             | 所属项目                         | 项目类型             | 具体类文件路径                      | 实现要点               |
| -------------------- | -------------------------------- | -------------------- | ----------------------------------- | ---------------------- |
| AdvertisementEntity  | `Lorn.ADSP.Data.EntityFramework` | .NET 9 Class Library | `/Entities/AdvertisementEntity.cs`  | EF Core映射、外键关系  |
| CampaignEntity       | `Lorn.ADSP.Data.EntityFramework` | .NET 9 Class Library | `/Entities/CampaignEntity.cs`       | 复杂类型映射、索引设计 |
| DeliveryRecordEntity | `Lorn.ADSP.Data.EntityFramework` | .NET 9 Class Library | `/Entities/DeliveryRecordEntity.cs` | 时间分区、批量操作     |
| UserProfileEntity    | `Lorn.ADSP.Data.EntityFramework` | .NET 9 Class Library | `/Entities/UserProfileEntity.cs`    | JSON列、全文索引       |

```csharp
// 示例：AdvertisementEntity EF映射实现指导
namespace Lorn.ADSP.Data.EntityFramework.Entities
{
    [Table("Advertisements")]
    public class AdvertisementEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        [Required]
        public int AdvertiserId { get; set; }
        
        [Column(TypeName = "nvarchar(50)")]
        public AdStatus Status { get; set; }
        
        // JSON列映射
        [Column(TypeName = "nvarchar(max)")]
        public string TargetingPolicyJson { get; set; }
        
        // 导航属性
        public AdvertiserEntity Advertiser { get; set; }
        public ICollection<CampaignEntity> Campaigns { get; set; }
        
        // 转换方法
        public Advertisement ToDomainEntity()
        {
            var targetingPolicy = JsonSerializer.Deserialize<TargetingPolicy>(TargetingPolicyJson);
            return Advertisement.Load(Id, Title, AdvertiserId, Status, targetingPolicy);
        }
        
        public static AdvertisementEntity FromDomainEntity(Advertisement advertisement)
        {
            // 领域对象到数据实体转换
        }
    }
}
```

#### 10.4.2 缓存对象分布

| 缓存对象         | 所属项目               | 项目类型             | 具体类文件路径                | 实现要点           |
| ---------------- | ---------------------- | -------------------- | ----------------------------- | ------------------ |
| AdCacheModel     | `Lorn.ADSP.Data.Redis` | .NET 9 Class Library | `/Models/AdCacheModel.cs`     | MessagePack序列化  |
| UserProfileCache | `Lorn.ADSP.Data.Redis` | .NET 9 Class Library | `/Models/UserProfileCache.cs` | 压缩存储、过期策略 |
| BudgetCache      | `Lorn.ADSP.Data.Redis` | .NET 9 Class Library | `/Models/BudgetCache.cs`      | 原子操作、分布式锁 |
| StatisticsCache  | `Lorn.ADSP.Data.Redis` | .NET 9 Class Library | `/Models/StatisticsCache.cs`  | 滑动窗口、聚合计算 |

#### 10.4.3 消息队列对象分布

| 消息对象                | 所属项目                      | 项目类型             | 具体类文件路径                         | 实现要点             |
| ----------------------- | ----------------------------- | -------------------- | -------------------------------------- | -------------------- |
| DeliveryEventMessage    | `Lorn.ADSP.Data.MessageQueue` | .NET 9 Class Library | `/Messages/DeliveryEventMessage.cs`    | 事件序列化、顺序保证 |
| BudgetUpdateMessage     | `Lorn.ADSP.Data.MessageQueue` | .NET 9 Class Library | `/Messages/BudgetUpdateMessage.cs`     | 幂等性、重试机制     |
| AuditLogMessage         | `Lorn.ADSP.Data.MessageQueue` | .NET 9 Class Library | `/Messages/AuditLogMessage.cs`         | 安全传输、完整性校验 |
| StatisticsUpdateMessage | `Lorn.ADSP.Data.MessageQueue` | .NET 9 Class Library | `/Messages/StatisticsUpdateMessage.cs` | 批量处理、压缩传输   |

### 10.5 共享组件项目映射

#### 10.5.1 枚举和常量分布

| 枚举/常量类型   | 所属项目                | 项目类型             | 具体类文件路径                  | 实现要点             |
| --------------- | ----------------------- | -------------------- | ------------------------------- | -------------------- |
| AdStatus        | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Enums/AdStatus.cs`            | 状态机实现、转换验证 |
| CampaignStatus  | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Enums/CampaignStatus.cs`      | 生命周期管理         |
| BiddingStrategy | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Enums/BiddingStrategy.cs`     | 竞价算法标识         |
| AdSizeConstants | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Constants/AdSizeConstants.cs` | IAB标准尺寸          |
| ErrorCodes      | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Constants/ErrorCodes.cs`      | 统一错误编码         |

```csharp
// 示例：AdStatus枚举实现指导
namespace Lorn.ADSP.Core.Shared.Enums
{
    public enum AdStatus
    {
        [Description("草稿状态")]
        Draft = 0,
        
        [Description("待审核")]
        PendingReview = 1,
        
        [Description("审核通过")]
        Approved = 2,
        
        [Description("审核拒绝")]
        Rejected = 3,
        
        [Description("投放中")]
        Active = 4,
        
        [Description("暂停")]
        Paused = 5,
        
        [Description("已结束")]
        Completed = 6
    }
    
    public static class AdStatusExtensions
    {
        public static bool CanTransitionTo(this AdStatus from, AdStatus to)
        {
            // 状态转换验证逻辑
            return from switch
            {
                AdStatus.Draft => to is AdStatus.PendingReview,
                AdStatus.PendingReview => to is AdStatus.Approved or AdStatus.Rejected,
                AdStatus.Approved => to is AdStatus.Active or AdStatus.Paused,
                AdStatus.Active => to is AdStatus.Paused or AdStatus.Completed,
                AdStatus.Paused => to is AdStatus.Active or AdStatus.Completed,
                _ => false
            };
        }
        
        public static string GetDescription(this AdStatus status)
        {
            // 获取状态描述
        }
    }
}
```

#### 10.5.2 扩展方法分布

| 扩展方法类           | 所属项目                | 项目类型             | 具体类文件路径                        | 实现要点           |
| -------------------- | ----------------------- | -------------------- | ------------------------------------- | ------------------ |
| DateTimeExtensions   | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Extensions/DateTimeExtensions.cs`   | 时区处理、格式化   |
| CollectionExtensions | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Extensions/CollectionExtensions.cs` | 分页、过滤、聚合   |
| StringExtensions     | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Extensions/StringExtensions.cs`     | 验证、格式化、编码 |
| JsonExtensions       | `Lorn.ADSP.Core.Shared` | .NET 9 Class Library | `/Extensions/JsonExtensions.cs`       | 序列化、安全解析   |

### 10.6 算法和服务对象项目映射

#### 10.6.1 算法实现对象分布

| 算法对象          | 所属项目              | 项目类型             | 具体类文件路径                     | 实现要点           |
| ----------------- | --------------------- | -------------------- | ---------------------------------- | ------------------ |
| TargetingMatcher  | `Lorn.ADSP.Targeting` | ASP.NET Core Web API | `/Algorithms/TargetingMatcher.cs`  | 匹配算法、性能优化 |
| BiddingCalculator | `Lorn.ADSP.Bidding`   | ASP.NET Core Web API | `/Algorithms/BiddingCalculator.cs` | 竞价算法、质量评估 |
| BudgetController  | `Lorn.ADSP.Campaign`  | ASP.NET Core Web API | `/Algorithms/BudgetController.cs`  | 预算控制、令牌桶   |
| RecallEngine      | `Lorn.ADSP.AdEngine`  | ASP.NET Core Web API | `/Algorithms/RecallEngine.cs`      | 召回算法、多路召回 |

#### 10.6.2 服务接口和实现分布

| 服务接口           | 接口项目                     | 实现项目              | 接口文件路径                      | 实现文件路径                     |
| ------------------ | ---------------------------- | --------------------- | --------------------------------- | -------------------------------- |
| IAdDeliveryService | `Lorn.ADSP.Core.Application` | `Lorn.ADSP.AdEngine`  | `/Services/IAdDeliveryService.cs` | `/Services/AdDeliveryService.cs` |
| ITargetingService  | `Lorn.ADSP.Core.Application` | `Lorn.ADSP.Targeting` | `/Services/ITargetingService.cs`  | `/Services/TargetingService.cs`  |
| IBiddingService    | `Lorn.ADSP.Core.Application` | `Lorn.ADSP.Bidding`   | `/Services/IBiddingService.cs`    | `/Services/BiddingService.cs`    |
| ICampaignService   | `Lorn.ADSP.Core.Application` | `Lorn.ADSP.Campaign`  | `/Services/ICampaignService.cs`   | `/Services/CampaignService.cs`   |

### 10.7 配置和设置对象项目映射

#### 10.7.1 配置对象分布

| 配置对象        | 所属项目                           | 项目类型             | 具体类文件路径                      | 实现要点             |
| --------------- | ---------------------------------- | -------------------- | ----------------------------------- | -------------------- |
| AdEngineOptions | `Lorn.ADSP.AdEngine`               | ASP.NET Core Web API | `/Configuration/AdEngineOptions.cs` | 强类型配置、验证     |
| BiddingOptions  | `Lorn.ADSP.Bidding`                | ASP.NET Core Web API | `/Configuration/BiddingOptions.cs`  | 算法参数、策略配置   |
| CacheOptions    | `Lorn.ADSP.Infrastructure.Caching` | .NET 9 Class Library | `/Configuration/CacheOptions.cs`    | 缓存策略、过期设置   |
| DatabaseOptions | `Lorn.ADSP.Data.EntityFramework`   | .NET 9 Class Library | `/Configuration/DatabaseOptions.cs` | 连接字符串、性能参数 |

```csharp
// 示例：AdEngineOptions配置对象实现指导
namespace Lorn.ADSP.AdEngine.Configuration
{
    public class AdEngineOptions
    {
        public const string ConfigSection = "AdEngine";
        
        [Required]
        [Range(1, 10000)]
        public int MaxConcurrentRequests { get; set; } = 5000;
        
        [Required]
        [Range(50, 5000)]
        public int RequestTimeoutMs { get; set; } = 100;
        
        [Required]
        public string[] SupportedFormats { get; set; } = { "banner", "video", "native" };
        
        public BudgetControlOptions BudgetControl { get; set; } = new();
        public PerformanceOptions Performance { get; set; } = new();
        
        // 验证方法
        public bool IsValid(out List<string> errors)
        {
            errors = new List<string>();
            
            if (MaxConcurrentRequests <= 0)
                errors.Add("MaxConcurrentRequests must be greater than 0");
                
            if (RequestTimeoutMs <= 0)
                errors.Add("RequestTimeoutMs must be greater than 0");
                
            return errors.Count == 0;
        }
    }
    
    public class BudgetControlOptions
    {
        public bool EnableRealTimeCheck { get; set; } = true;
        public int CheckIntervalMs { get; set; } = 1000;
        public double SafetyMarginPercent { get; set; } = 5.0;
    }
    
    public class PerformanceOptions
    {
        public bool EnableCaching { get; set; } = true;
        public int CacheTtlSeconds { get; set; } = 300;
        public bool EnableCompression { get; set; } = true;
    }
}
```

### 10.8 数据库映射和迁移项目配置

#### 10.8.1 DbContext配置

| DbContext          | 所属项目                         | 配置文件路径                      | 实现要点           |
| ------------------ | -------------------------------- | --------------------------------- | ------------------ |
| AdSystemDbContext  | `Lorn.ADSP.Data.EntityFramework` | `/Contexts/AdSystemDbContext.cs`  | 实体配置、性能优化 |
| AnalyticsDbContext | `Lorn.ADSP.Data.EntityFramework` | `/Contexts/AnalyticsDbContext.cs` | 读写分离、分区表   |
| AuditDbContext     | `Lorn.ADSP.Data.EntityFramework` | `/Contexts/AuditDbContext.cs`     | 审计日志、只写模式 |

#### 10.8.2 数据库迁移管理

| 迁移类型 | 所属项目                         | 迁移文件路径             | 管理策略             |
| -------- | -------------------------------- | ------------------------ | -------------------- |
| 结构迁移 | `Lorn.ADSP.Data.EntityFramework` | `/Migrations/Structure/` | 版本化管理、自动执行 |
| 数据迁移 | `Lorn.ADSP.Tools.DataMigration`  | `/Migrations/Data/`      | 手动执行、数据验证   |
| 索引迁移 | `Lorn.ADSP.Data.EntityFramework` | `/Migrations/Indexes/`   | 性能监控、在线执行   |
