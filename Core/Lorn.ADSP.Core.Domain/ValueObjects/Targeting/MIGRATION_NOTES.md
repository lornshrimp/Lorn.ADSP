# ֵ����Ǩ�������������Ļ���

## ����

��������ĵ�Ҫ���ѽ������ĸ�ֵ�������޸�Ϊ�̳� `TargetingContextBase`��ʹ�����ܹ���Ϊ�����������ڹ��������ʹ�ã�

- `DemographicInfo` - �˿�ͳ��ѧ��Ϣ����������
- `GeoInfo` - ����λ����Ϣ����������  
- `TimeRange` - ʱ�䷶Χ����������
- `DeviceInfo` - �豸��Ϣ����������

## �޸�����

### 1. DemographicInfo.cs
**�������**���̳й�ϵ���
- **֮ǰ**��`public class DemographicInfo : ValueObject`
- **֮��**��`public class DemographicInfo : TargetingContextBase`

**��Ҫ���**��
- ���캯���޸�Ϊ���û���� `TargetingContextBase("Demographic", properties, dataSource)`
- ���Է��ʸ�Ϊͨ�� `GetProperty<T>()` ����
- ����� `CreateProperties` ��̬���������������ֵ�
- ��д�� `GetDebugInfo()` �� `IsValid()` ����
- ���й������������� `dataSource` ����

### 2. GeoInfo.cs
**�������**���̳й�ϵ���
- **֮ǰ**��`public class GeoInfo : ValueObject`
- **֮��**��`public class GeoInfo : TargetingContextBase`

**��Ҫ���**��
- ���캯���޸�Ϊ���û���� `TargetingContextBase("Geo", properties, dataSource)`
- �������Ը�Ϊͨ�� `GetProperty<T>()` ����
- ����� `GetLocationDescription()` ����
- ��д�� `GetDebugInfo()` �� `IsValid()` ����
- ��������֧�� `dataSource` ����

### 3. TimeRange.cs
**�������**���̳й�ϵ���
- **֮ǰ**��`public class TimeRange : ValueObject`
- **֮��**��`public class TimeRange : TargetingContextBase`

**��Ҫ���**��
- ���캯���޸�Ϊ���û���� `TargetingContextBase("TimeRange", properties, dataSource)`
- ���Է��ʸ�Ϊͨ�� `GetProperty<T>()` ����
- ԭ `IsValid` ����������Ϊ `IsValidRange` �Ա�������෽����ͻ
- �����ʱ�䷶Χ��صı�ݷ���
- ��д�� `GetDebugInfo()` �� `IsValid()` ����

### 4. DeviceInfo.cs
**�������**���̳й�ϵ���
- **֮ǰ**��`public class DeviceInfo : ValueObject`
- **֮��**��`public class DeviceInfo : TargetingContextBase`

**��Ҫ���**��
- ���캯���޸�Ϊ���û���� `TargetingContextBase("Device", properties, dataSource)`
- �������Ը�Ϊͨ�� `GetProperty<T>()` ���ʣ������ʵ���Ĭ��ֵ
- ������豸��صı�ݷ���������
- ��д�� `GetDebugInfo()` �� `IsValid()` ����
- ��������֧�� `dataSource` ����

## ��������

�����޸ĺ�������ڶ��߱����¶��������Ĺ��ܣ�

### �̳еĻ�������
- **�����Ĺ���**��`ContextType`��`ContextId`��`DataSource`��`Timestamp`
- **���Է���**�����Ͱ�ȫ�� `GetProperty<T>()`��`HasProperty()`��`GetPropertyKeys()`
- **�����Ĳ���**��`CreateLightweightCopy()`��`Merge()`��`SetProperty()`
- **��֤�͵���**��`IsValid()`��`IsExpired()`��`GetDebugInfo()`��`GetMetadata()`

### �ض�������
ÿ���඼��������ԭ�е������ض�������ͬʱ��ǿ��������������

- **DemographicInfo**���˿�ͳ��ѧ���ݷ��ʺ�����������
- **GeoInfo**������λ�ü����λ������  
- **TimeRange**��ʱ�䷶Χ��������֤
- **DeviceInfo**���豸����ʶ����������

## ʹ��ʾ��

### ��������������

```csharp
// ��������λ��������
var geoContext = GeoInfo.Create(
    countryCode: "CN",
    countryName: "�й�", 
    cityName: "����",
    dataSource: "IPService");

// �����豸������
var deviceContext = DeviceInfo.CreateMobile(
    operatingSystem: "iOS",
    brand: "Apple",
    model: "iPhone 14",
    dataSource: "UserAgent");

// ����ʱ�䷶Χ������
var timeContext = TimeRange.Today("SystemTime");
```

### ʹ�������Ĺ���

```csharp
// �����������Ч��
if (geoContext.IsValid() && !geoContext.IsExpired(TimeSpan.FromHours(1)))
{
    // ��ȡλ������
    var location = geoContext.GetLocationDescription();
    
    // �����ض�����
    var country = geoContext.GetProperty<string>("CountryCode");
    
    // ��ȡ������Ϣ
    var debugInfo = geoContext.GetDebugInfo();
}

// �ϲ����������
var mergedContext = geoContext.Merge(deviceContext);

// ��������������
var lightContext = deviceContext.CreateLightweightCopy(new[] { "DeviceType", "OperatingSystem" });
```

## ��������

��Ȼ��ļ̳й�ϵ�����˱仯�������й���API�������������ݣ�

- ����ԭ�е����Ժͷ�����Ȼ����
- ��������ǩ�����ֲ��䣨����������ѡ�� `dataSource` ������
- ҵ���߼��������ܲ���

## Ӱ�����

### ����Ӱ��
1. **ͳһ�Ķ���ӿ�**�������������������ڶ�ʵ�� `ITargetingContext` �ӿ�
2. **��ǿ�Ĺ���**������������Ĺ����ϲ������Եȸ߼�����
3. **���õļ���**�������붨�������޷켯��
4. **���Ͱ�ȫ**�����Է��ʸ������Ͱ�ȫ��֧��Ĭ��ֵ

### Ǳ��Ӱ��
1. **���ܿ���**�����Է�������ͨ���ֵ���ң���������΢����Ӱ��
2. **�ڴ�ʹ��**��ÿ��ʵ�����ڰ��������������Ԫ����
3. **���л�**�����л���ʽ���ܷ����仯����Ҫע�������

## ���Խ���

1. **��Ԫ����**��ȷ������ԭ�й�����������
2. **���ɲ���**����֤�붨������ļ���
3. **���ܲ���**����������Ӱ���Ƿ��ڿɽ��ܷ�Χ��
4. **�����Բ���**��ȷ�����л�/�����л�������

## �ܽ�

�˴��޸ĳɹ����ĸ�����ֵ����ת��Ϊ�����������࣬Ϊ�������Ķ������ṩ��ͳһ�ĳ���㡣�����޸Ķ��������������ԣ�ͬʱ������ǿ��ϵͳ�Ķ�����������չ�ԡ�