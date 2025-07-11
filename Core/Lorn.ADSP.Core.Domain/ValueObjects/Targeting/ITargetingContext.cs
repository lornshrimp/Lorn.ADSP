namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// ���������ĳ���ӿ�
    /// ����ͳһ�����������ݷ��ʹ淶��֧�ֶ������͵Ķ�����Ϣ�洢�ͼ���
    /// </summary>
    public interface ITargetingContext
    {
        /// <summary>
        /// ���������ͱ�ʶ
        /// ���磺AdRequest��UserProfile��DeviceInfo��
        /// </summary>
        string ContextType { get; }

        /// <summary>
        /// ���������Լ���
        /// �洢���ֶ�����ص�������Ϣ����Ϊ�������ƣ�ֵΪ����ֵ
        /// </summary>
        IReadOnlyDictionary<string, object> Properties { get; }

        /// <summary>
        /// �����Ĵ���ʱ���
        /// ���ڻ�������������Ч�Լ��
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// �����ĵ�Ψһ��ʶ
        /// ���ڻ�������ɺ͵���׷��
        /// </summary>
        string ContextId { get; }

        /// <summary>
        /// ������������Դ
        /// ��ʶ���ݵ���Դϵͳ�����������������������
        /// </summary>
        string DataSource { get; }

        /// <summary>
        /// ��ȡָ�����͵�����ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ֵ������������򷵻�Ĭ��ֵ</returns>
        T? GetProperty<T>(string propertyKey);

        /// <summary>
        /// ��ȡָ�����͵�����ֵ������������򷵻�Ĭ��ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="propertyKey">���Լ�</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>����ֵ��Ĭ��ֵ</returns>
        T GetProperty<T>(string propertyKey, T defaultValue);

        /// <summary>
        /// ��ȡ����ֵ���ַ�����ʾ
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ֵ���ַ�����ʾ������������򷵻ؿ��ַ���</returns>
        string GetPropertyAsString(string propertyKey);

        /// <summary>
        /// ����Ƿ����ָ��������
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>�Ƿ����������</returns>
        bool HasProperty(string propertyKey);

        /// <summary>
        /// ��ȡ�������Լ�
        /// </summary>
        /// <returns>���Լ�����</returns>
        IReadOnlyCollection<string> GetPropertyKeys();

        /// <summary>
        /// ��������������Ƿ���Ч
        /// ����ʱ��������������Խ�����֤
        /// </summary>
        /// <returns>�Ƿ���Ч</returns>
        bool IsValid();

        /// <summary>
        /// ��������������Ƿ��ѹ���
        /// </summary>
        /// <param name="maxAge">�����Ч��</param>
        /// <returns>�Ƿ��ѹ���</returns>
        bool IsExpired(TimeSpan maxAge);

        /// <summary>
        /// ��ȡ�����ĵ�Ԫ������Ϣ
        /// ����������Դ������ʱ�䡢������������Ϣ
        /// </summary>
        /// <returns>Ԫ�����ֵ�</returns>
        IReadOnlyDictionary<string, object> GetMetadata();

        /// <summary>
        /// ��ȡ�����ĵĵ�����Ϣ
        /// ���������Ų����־��¼
        /// </summary>
        /// <returns>������Ϣ�ַ���</returns>
        string GetDebugInfo();

        /// <summary>
        /// ���������ĵ�����������
        /// ֻ����ָ�������Լ������������Ż�
        /// </summary>
        /// <param name="includeKeys">Ҫ���������Լ�</param>
        /// <returns>�����������ĸ���</returns>
        ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys);

        /// <summary>
        /// �ϲ���һ�������ĵ�����
        /// ������϶������Դ����������Ϣ
        /// </summary>
        /// <param name="other">Ҫ�ϲ���������</param>
        /// <param name="overwriteExisting">�Ƿ񸲸��Ѵ��ڵ�����</param>
        /// <returns>�ϲ������������</returns>
        ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false);
    }
}