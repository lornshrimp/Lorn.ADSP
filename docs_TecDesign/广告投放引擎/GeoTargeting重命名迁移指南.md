# GeoTargeting��������Ǩ��ָ��

## ����

Ϊ�˸���ȷ�����ֲ�ͬ���͵ĵ������ܣ����ǶԵ������������������������ع�����ָ�Ͻ��������˽������ݲ���ɴ���Ǩ�ơ�

## ���ժҪ

### �������

| ������ | ������ | ����˵�� |
|--------|--------|----------|
| `GeoTargeting` | `AdministrativeGeoTargeting` | �������������򣨹��ҡ�ʡ�ݡ����У� |
| - | `CircularGeoFenceTargeting` | Բ�ε���Χ�������Ѵ��ڣ��ޱ���� |
| - | `PolygonGeoFenceTargeting` | ����ε���Χ�������Ѵ��ڣ��ޱ���� |

### �ļ�·�����

| ���ļ�·�� | ���ļ�·�� |
|------------|------------|
| `Core\Lorn.ADSP.Core.Domain\ValueObjects\Targeting\GeoTargeting.cs` | `Core\Lorn.ADSP.Core.Domain\ValueObjects\Targeting\AdministrativeGeoTargeting.cs` |

## ��ϸ�������

### 1. AdministrativeGeoTargeting ��

**���ǰ��**
```csharp
public class GeoTargeting : TargetingCriteriaBase
{
    public override string CriteriaType => "Geo";
    // ... ������Ա
}
```

**�����**
```csharp
public class AdministrativeGeoTargeting : TargetingCriteriaBase
{
    public override string CriteriaType => "AdministrativeGeo";
    // ... ������Ա�����ܲ��䣩
}
```

### 2. TargetingConfig �����

**���ǰ��**
```csharp
public class TargetingConfig : ValueObject
{
    public GeoTargeting? GeoTargeting { get; private set; }
    
    public static TargetingConfig Create(
        GeoTargeting? geoTargeting = null,
        // ... ��������
    )
}
```

**�����**
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
        // ... ��������
    )
    
    // ������ݷ���
    public IEnumerable<ITargetingCriteria> GetEnabledGeoTargetingCriteria()
    public bool HasGeoTargeting()
}
```

### 3. TargetingPolicy �����

**���ǰ��**
```csharp
public GeoTargeting? GeoTargeting => GetCriteria<GeoTargeting>("Geo");
```

**�����**
```csharp
public AdministrativeGeoTargeting? AdministrativeGeoTargeting => GetCriteria<AdministrativeGeoTargeting>("AdministrativeGeo");
public CircularGeoFenceTargeting? CircularGeoFenceTargeting => GetCriteria<CircularGeoFenceTargeting>("CircularGeoFence");
public PolygonGeoFenceTargeting? PolygonGeoFenceTargeting => GetCriteria<PolygonGeoFenceTargeting>("PolygonGeoFence");

// ������ݷ���
public IEnumerable<ITargetingCriteria> GetGeoTargetingCriteria()
public bool HasGeoTargeting()
```

## Ǩ�Ʋ���

### ���� 1����������

������ʹ�� `GeoTargeting` �Ĵ����ļ��У�

1. �� `GeoTargeting` �滻Ϊ `AdministrativeGeoTargeting`
2. ���ʹ�� `CriteriaType` ����ƥ�䣬�� `"Geo"` �滻Ϊ `"AdministrativeGeo"`

### ���� 2�����´�������

**���ǰ��**
```csharp
var geoTargeting = GeoTargeting.Create(
    includedLocations: cities,
    mode: GeoTargetingMode.Include
);

var config = TargetingConfig.Create(
    geoTargeting: geoTargeting
);
```

**�����**
```csharp
var adminGeoTargeting = AdministrativeGeoTargeting.Create(
    includedLocations: cities,
    mode: GeoTargetingMode.Include
);

var config = TargetingConfig.Create(
    administrativeGeoTargeting: adminGeoTargeting
);
```

### ���� 3�����·��ʴ���

**���ǰ��**
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

**�����**
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

### ���� 4�����Ƕ��ֵ��������

�¼ܹ�֧��ͬʱʹ�ö��ֵ��������ͣ�

```csharp
var config = TargetingConfig.Create(
    administrativeGeoTargeting: adminTargeting,     // ����������ɸ
    circularGeoFenceTargeting: circularTargeting,   // ��Ȧ��׼����
    polygonGeoFenceTargeting: polygonTargeting      // ����������
);

// ����Ƿ��е�����
if (config.HasGeoTargeting())
{
    var geoTargetingTypes = config.GetEnabledGeoTargetingCriteria()
        .Select(c => c.CriteriaType);
    Console.WriteLine($"���õĵ���������: {string.Join(", ", geoTargetingTypes)}");
}
```

## ������˵��

### ���ݿ�Ǩ��

����������ݿ��д洢�����л��Ķ������ã�������Ҫ��������Ǩ�ƣ�

1. **CriteriaType ����**�����洢�� `"Geo"` ����Ϊ `"AdministrativeGeo"`
2. **��������**�������л������е������� `GeoTargeting` ����Ϊ `AdministrativeGeoTargeting`

### API ������

������ж����API�ӿڣ����飺

1. �����������ԣ�ͬʱ֧���¾��ֶ���
2. ��API�ĵ��б�Ǿ��ֶ�Ϊ������
3. �ṩǨ��ʱ���

## ��֤Ǩ��

���Ǩ�ƺ������������֤��

1. **������֤**��ȷ��������Ŀ���ܳɹ�����
```bash
dotnet build Lorn.ADSP.sln
```

2. **��Ԫ������֤**��������صĵ�Ԫ����
```bash
dotnet test Lorn.ADSP.sln --filter="Category=GeoTargeting"
```

3. **������֤**��ȷ������������������
   - ���������������
   - Բ��Χ���������
   - �����Χ���������

## ��������

### Q: ΪʲôҪ���������������

A: ԭ���� `GeoTargeting` ���ƹ��ڿ����޷���ȷ��ʾ����幦�ܡ�������Ϊ `AdministrativeGeoTargeting` �󣬿�������ر������ǻ������������ĵ���������������Χ���������ֿ�����

### Q: �ɵĹ��ܻ��б仯��

A: ���ᡣ`AdministrativeGeoTargeting` �����й��ܶ���ԭ���� `GeoTargeting` ��ȫ��ͬ��ֻ�������� CriteriaType �����˱����

### Q: ���ѡ����ʵĵ��������ͣ�

A: 
- **AdministrativeGeoTargeting**���ʺϰ����С�ʡ�ݡ����ҽ��еĴ�Χ����
- **CircularGeoFenceTargeting**���ʺ���Ȧ��POI�ܱߵľ�׼����
- **PolygonGeoFenceTargeting**���ʺϸ�����״����ľ�ȷ����

### Q: ����ͬʱʹ�ö��ֵ�������

A: ���ԡ��¼ܹ�֧����ͬһ�� `TargetingConfig` ��ͬʱ���ö��ֵ��������ͣ�ϵͳ�ᰴ�ո��ԵĹ������ƥ�䡣

## ����֧��

�����Ǩ�ƹ������������⣬�룺

1. �鿴��Ǩ��ָ��
2. �ο� `docs_TecDesign/������ϵͳ���˵��.md`
3. ��ϵ�����Ŷӻ�ȡ֧��

---

**��Ҫ����**������������������ǰ���ڲ��Ի����г����֤Ǩ�ƽ����