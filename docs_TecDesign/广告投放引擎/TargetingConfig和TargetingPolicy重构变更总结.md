# TargetingConfig �� TargetingPolicy �ع�����ܽ�

## ����

��������ģ�ͷֲ�����ĵ���Ҫ�󣬶� `TargetingConfig` �� `TargetingPolicy` ����������������ع��������ϵͳ�Ŀ���չ�Ժ�����ԡ���Ҫ�������ʹ���ֵ�ṹ��������������ǿ��Ĺ��ܶ�λ����Ӷ�̬�Ż�������

## ��Ҫ�������

### 1. TargetingConfig ���ع�

#### 1.1 �ܹ�����

**���ǰ��**
- Ӳ������ֶ�����������
- ���޵���չ����
- ȱ����̬�Ż�����

**�����**
- ʹ���ֵ�ṹ `Dictionary<string, ITargetingCriteria>` ����������
- ���Ӷ�̬����֧�� `Dictionary<string, object>`
- ʵ���������������ڹ���

#### 1.2 ������������

```csharp
// �������ñ�ʶ�͹�����Ϣ
public string ConfigId { get; private set; }
public string AdvertisementId { get; private set; }
public string? SourcePolicyId { get; private set; }

// �ֵ�ṹ֧�ֿ���չ��
public IReadOnlyDictionary<string, ITargetingCriteria> Criteria => _criteria.AsReadOnly();
public IReadOnlyDictionary<string, object> DynamicParameters => _dynamicParameters.AsReadOnly();

// �������ڹ���
public DateTime CreatedAt { get; private set; }
public DateTime UpdatedAt { get; private set; }
public string CreatedFrom { get; private set; }
```

#### 1.3 �������ķ���

**����������**
- `CreateFromPolicy()` - �� TargetingPolicy ��������ʵ��
- `CreateFromScratch()` - ��ͷ��������ʵ��

**������������**
- `AddCriteria()` - ��Ӷ�������
- `UpdateCriteria()` - ���¶�������
- `RemoveCriteria()` - �Ƴ���������
- `GetCriteria<T>()` - ��ȡָ�����͵Ķ�������
- `HasCriteria()` - ����Ƿ����ָ������

**��̬��������**
- `SetDynamicParameter()` - ���ö�̬����
- `GetDynamicParameter<T>()` - ��ȡ��̬����

**�Ż����ܣ�**
- `ApplyDynamicOptimization()` - Ӧ�ö�̬�Ż�
- `ValidateConfig()` - ��֤������Ч��
- `Clone()` - ��¡����

### 2. TargetingPolicy ���ع�

#### 2.1 ���ܶ�λ��ȷ

**���ǰ��**
- ���ܶ�λģ��
- ȱ���汾����
- û��״̬����

**�����**
- ��ȷ��λΪ�ɸ��õĶ������ģ��
- ���������İ汾�����״̬����
- ֧�ֲ��Եķ������鵵��ʹ��ͳ��

#### 2.2 ������������

```csharp
// ���Ա�ʶ��Ԫ����
public string PolicyId { get; private set; }
public string Name { get; private set; }
public string? Description { get; private set; }
public int Version { get; private set; }
public string CreatedBy { get; private set; }

// ״̬�ͷ������
public PolicyStatus Status { get; private set; }
public string Category { get; private set; }
public bool IsPublic { get; private set; }

// ��ǩ��ģ�����
public IReadOnlyList<string> Tags => _tags.AsReadOnly();
public IReadOnlyDictionary<string, ITargetingCriteria> CriteriaTemplates => _criteriaTemplates.AsReadOnly();
```

#### 2.3 �������ķ���

**����������**
- `CreateEmpty()` - �����ղ���ģ��
- `CreateUnrestricted()` - ���������Ʋ���
- `CreateConfig()` - ���� TargetingConfig ʵ��

**ģ�����**
- `AddCriteriaTemplate()` - �������ģ��
- `RemoveCriteriaTemplate()` - �Ƴ�����ģ��
- `GetCriteriaTemplate<T>()` - ��ȡָ�����͵�����ģ��

**״̬����**
- `Publish()` - ��������
- `Archive()` - �鵵����
- `Clone()` - ��¡����

**��ǩ����**
- `AddTag()` - ��ӱ�ǩ
- `RemoveTag()` - �Ƴ���ǩ

**ʹ��ͳ�ƣ�**
- `GetUsageStatistics()` - ��ȡʹ��ͳ��

### 3. ֧����������

#### 3.1 ValidationResult ��

```csharp
public class ValidationResult
{
    public bool IsValid => !_errors.Any();
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();
    public IReadOnlyList<string> Warnings => _warnings.AsReadOnly();
    
    public void AddError(string error);
    public void AddWarning(string warning);
}
```

#### 3.2 OptimizationContext ��

```csharp
public class OptimizationContext
{
    public PerformanceMetrics? PerformanceMetrics { get; set; }
    public List<OptimizationRecommendation>? OptimizationRecommendations { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
}
```

#### 3.3 PolicyStatus ö��

```csharp
public enum PolicyStatus
{
    Draft = 1,      // �ݸ�״̬
    Published = 2,  // �ѷ���
    Archived = 3    // �ѹ鵵
}
```

#### 3.4 PolicyUsageStats ��

```csharp
public class PolicyUsageStats
{
    public string PolicyId { get; set; }
    public int TotalConfigs { get; set; }
    public int ActiveConfigs { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public decimal AveragePerformance { get; set; }
}
```

## �������

### 1. ����չ����ǿ

**�ֵ�ṹ����**
- ֧�ֶ�̬����µĶ�����������
- �����޸ĺ��Ĵ��뼴����չ����
- ��������ͨ���ַ��������й�����������

**��̬����֧�֣�**
- ֧������ʱ��̬��������
- Ϊ����ѧϰ�Ż�Ԥ���ӿ�
- ֧�� A/B ���Ժ͸��Ի��Ż�

### 2. ���������

**ģ���ʵ�����룺**
- TargetingPolicy ��Ϊ�ɸ���ģ��
- TargetingConfig ��Ϊ����ʱʵ��
- ֧�ִ�ģ�崴�����ã�ͬʱ������Ի�����

**״̬�������ƣ�**
- �������������ڹ���
- ֧�ְ汾���ƺ�״̬����
- �ṩʹ��ͳ�ƺ����ܷ���

### 3. ҵ���ֵ��ǿ

**��ӪЧ��������**
- ֧�ֲ���ģ��ĸ��ú͹���
- �ṩ����ʹ��ͳ�ƺ�Ч������
- ֧�ֲ��Եķ������ͱ�ǩ����

**�����Ż�������**
- ֧�ֻ�����ʷ���ֵĶ�̬�Ż�
- �ṩ�Ż�������Զ�����
- Ϊ����ѧϰ�������Ż�Ԥ���ӿ�

## �����Դ���

### 1. ������

**������ݷ��ʷ�����**
```csharp
// TargetingConfig �б����ı�ݷ���
public IEnumerable<ITargetingCriteria> GetEnabledGeoTargetingCriteria()
public bool HasGeoTargeting()

// TargetingPolicy �б����ı�ݷ���  
public AdministrativeGeoTargeting? AdministrativeGeoTargeting => GetCriteriaTemplate<AdministrativeGeoTargeting>("AdministrativeGeo");
public IEnumerable<ITargetingCriteria> GetGeoTargetingCriteriaTemplates()
```

### 2. Ǩ��ָ��

**���д���Ǩ�ƣ�**
1. ��ֱ�����Է��ʸ�Ϊ�ֵ����
2. ʹ���µĴ�������������캯��
3. ������������ʽ

**��������Ǩ�ƣ�**
1. ��Ӳ��������ת��Ϊ�ֵ�ṹ
2. ��ӱ�Ҫ��Ԫ������Ϣ
3. �������ݵ������Ժ�һ����

## Ӱ�췶Χ

### 1. ֱ��Ӱ��

**�޸����ļ���**
- `Core\Lorn.ADSP.Core.Domain\ValueObjects\TargetingConfig.cs` - ��ȫ�ع�
- `Core\Lorn.ADSP.Core.Domain\ValueObjects\TargetingPolicy.cs` - ��ȫ�ع�
- `Core\Lorn.ADSP.Core.Domain\Entities\Campaign.cs` - ���÷�������
- `Core\Lorn.ADSP.Core.Domain\Entities\Advertisement.cs` - ���÷�������

### 2. Ǳ��Ӱ��

**��Ҫ���������ģ�飺**
- ������Լ�����ʵ��
- ���Ͷ�����漯��
- ���ù���ͳ־û�
- API �ӿں����ݴ������

## ������������

### 1. ��������

1. **ʵ�����������** ���� `Clone()` �����е�����߼�
2. **��ӵ�Ԫ���ԣ�** Ϊ�¹��ܱ�дȫ��ĵ�Ԫ����
3. **�����ĵ���** ���� API �ĵ���ʹ��ָ��
4. **���ܲ��ԣ�** ��֤�ֵ�ṹ�����ܱ���

### 2. ��������

1. **���ɲ��ԣ�** ����Ͷ��������м��ɲ���
2. **����Ǩ�ƹ��ߣ�** �����������ݵ�Ǩ�ƹ���
3. **���ָ�꣺** ����¹��ܵļ�غͶ���ָ��
4. **�Ż��㷨��** ʵ�ֶ�̬�Ż��ľ����㷨

### 3. ���ڹ滮

1. **����ѧϰ���ɣ�** ���ɻ���ѧϰ�������Ż�
2. **���ӻ����ߣ�** �����������õĿ��ӻ�����
3. **�����Ƽ���** ����ʹ��ͳ�ƵĲ����Ƽ�
4. **A/B ���Կ�ܣ�** ���� A/B ����֧��

## �ܽ�

�˴��ع����������˶�������ϵͳ�Ŀ���չ�Ժ�����ԣ�ͨ���ֵ�ṹ��������������ȷ�� TargetingPolicy �� TargetingConfig ��ְ��ֹ���Ϊ�����Ĺ�����չ�������Ż��춨�˼�ʵ�������ع����ϵͳ���õ�֧����ҵ������Ŀ��ٱ仯�ͼ����ܹ��ĳ����ݽ���