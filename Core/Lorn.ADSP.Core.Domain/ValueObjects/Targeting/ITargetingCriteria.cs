namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// ������������ӿ�
    /// ����ͳһ�Ķ����������ʹ淶��֧�ֲ�ͬ���͵Ķ����������
    /// </summary>
    public interface ITargetingCriteria
    {
        /// <summary>
        /// �������ͱ�ʶ
        /// ���磺Geo��Demographic��Device��Time��Behavior
        /// </summary>
        string CriteriaType { get; }

        /// <summary>
        /// ������򼯺�
        /// �洢���������͵ľ���������ã���Ϊ�������ƣ�ֵΪ����ֵ
        /// </summary>
        IReadOnlyDictionary<string, object> Rules { get; }

        /// <summary>
        /// ����Ȩ��
        /// �����ڶ���������ʱ�����Ȩ������Ĭ��Ϊ1.0
        /// </summary>
        decimal Weight { get; }

        /// <summary>
        /// �Ƿ����ø�����
        /// ������A/B���Ի�̬��������
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// ��������ʱ��
        /// ���ڰ汾����ͻ���ʧЧ
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// ����������ʱ��
        /// ����׷�����������ʷ
        /// </summary>
        DateTime UpdatedAt { get; }

        /// <summary>
        /// ��ȡָ�����͵Ĺ���ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="ruleKey">�����</param>
        /// <returns>����ֵ������������򷵻�Ĭ��ֵ</returns>
        T? GetRule<T>(string ruleKey);

        /// <summary>
        /// ��ȡָ�����͵Ĺ���ֵ������������򷵻�Ĭ��ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="ruleKey">�����</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>����ֵ��Ĭ��ֵ</returns>
        T GetRule<T>(string ruleKey, T defaultValue);

        /// <summary>
        /// ����Ƿ����ָ���Ĺ���
        /// </summary>
        /// <param name="ruleKey">�����</param>
        /// <returns>�Ƿ�����ù���</returns>
        bool HasRule(string ruleKey);

        /// <summary>
        /// ��ȡ���й����
        /// </summary>
        /// <returns>���������</returns>
        IReadOnlyCollection<string> GetRuleKeys();

        /// <summary>
        /// ��֤�������õ���Ч��
        /// </summary>
        /// <returns>��֤���</returns>
        bool IsValid();

        /// <summary>
        /// ��ȡ����������ժҪ��Ϣ
        /// ������־��¼�͵���
        /// </summary>
        /// <returns>����ժҪ</returns>
        string GetConfigurationSummary();
    }
}