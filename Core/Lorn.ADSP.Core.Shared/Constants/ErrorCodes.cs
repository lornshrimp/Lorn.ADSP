namespace Lorn.ADSP.Core.Shared.Constants;

/// <summary>
/// �����볣��
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// ͨ�ô�����
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// δ֪����
        /// </summary>
        public const string UnknownError = "E0001";

        /// <summary>
        /// ������Ч
        /// </summary>
        public const string InvalidParameter = "E0002";

        /// <summary>
        /// ���ݲ�����
        /// </summary>
        public const string DataNotFound = "E0003";

        /// <summary>
        /// �����ظ�
        /// </summary>
        public const string DataDuplicate = "E0004";

        /// <summary>
        /// Ȩ�޲���
        /// </summary>
        public const string InsufficientPermission = "E0005";

        /// <summary>
        /// ����ʱ
        /// </summary>
        public const string RequestTimeout = "E0006";

        /// <summary>
        /// ϵͳ��æ
        /// </summary>
        public const string SystemBusy = "E0007";
    }

    /// <summary>
    /// �����ش�����
    /// </summary>
    public static class Advertisement
    {
        /// <summary>
        /// ��治����
        /// </summary>
        public const string AdNotFound = "E1001";

        /// <summary>
        /// ����ѹ���
        /// </summary>
        public const string AdExpired = "E1002";

        /// <summary>
        /// ���δ���ͨ��
        /// </summary>
        public const string AdNotApproved = "E1003";

        /// <summary>
        /// ���Ԥ�㲻��
        /// </summary>
        public const string InsufficientBudget = "E1004";

        /// <summary>
        /// ��涨��ƥ��
        /// </summary>
        public const string TargetingMismatch = "E1005";

        /// <summary>
        /// ���Ƶ�γ���
        /// </summary>
        public const string FrequencyCapExceeded = "E1006";

        /// <summary>
        /// ����ز���Ч
        /// </summary>
        public const string InvalidCreative = "E1007";
    }

    /// <summary>
    /// ������ش�����
    /// </summary>
    public static class Bidding
    {
        /// <summary>
        /// ����������Ч
        /// </summary>
        public const string InvalidBidRequest = "E2001";

        /// <summary>
        /// ���ۼ۸����
        /// </summary>
        public const string BidPriceTooLow = "E2002";

        /// <summary>
        /// ���ۼ۸����
        /// </summary>
        public const string BidPriceTooHigh = "E2003";

        /// <summary>
        /// ���۳�ʱ
        /// </summary>
        public const string BidTimeout = "E2004";

        /// <summary>
        /// ����Ч����
        /// </summary>
        public const string NoBidAvailable = "E2005";

        /// <summary>
        /// ����ʧ��
        /// </summary>
        public const string BidFailed = "E2006";
    }

    /// <summary>
    /// �û���ش�����
    /// </summary>
    public static class User
    {
        /// <summary>
        /// �û�������
        /// </summary>
        public const string UserNotFound = "E3001";

        /// <summary>
        /// �û��Ѵ���
        /// </summary>
        public const string UserAlreadyExists = "E3002";

        /// <summary>
        /// �û�δ����
        /// </summary>
        public const string UserNotActivated = "E3003";

        /// <summary>
        /// �û��ѱ�����
        /// </summary>
        public const string UserDisabled = "E3004";

        /// <summary>
        /// �û���֤ʧ��
        /// </summary>
        public const string AuthenticationFailed = "E3005";

        /// <summary>
        /// �û���Ȩʧ��
        /// </summary>
        public const string AuthorizationFailed = "E3006";
    }

    /// <summary>
    /// ���ݷ��ʴ�����
    /// </summary>
    public static class DataAccess
    {
        /// <summary>
        /// ���ݿ�����ʧ��
        /// </summary>
        public const string DatabaseConnectionFailed = "E4001";

        /// <summary>
        /// ���ݿ��ѯʧ��
        /// </summary>
        public const string DatabaseQueryFailed = "E4002";

        /// <summary>
        /// ���ݿ����ʧ��
        /// </summary>
        public const string DatabaseUpdateFailed = "E4003";

        /// <summary>
        /// �������ʧ��
        /// </summary>
        public const string CacheAccessFailed = "E4004";

        /// <summary>
        /// �������л�ʧ��
        /// </summary>
        public const string DataSerializationFailed = "E4005";

        /// <summary>
        /// ���ݷ����л�ʧ��
        /// </summary>
        public const string DataDeserializationFailed = "E4006";
    }

    /// <summary>
    /// �ⲿ���������
    /// </summary>
    public static class ExternalService
    {
        /// <summary>
        /// �ⲿ���񲻿���
        /// </summary>
        public const string ServiceUnavailable = "E5001";

        /// <summary>
        /// �ⲿ������Ӧ��ʱ
        /// </summary>
        public const string ServiceTimeout = "E5002";

        /// <summary>
        /// �ⲿ������Ӧ��ʽ����
        /// </summary>
        public const string InvalidServiceResponse = "E5003";

        /// <summary>
        /// �ⲿ������֤ʧ��
        /// </summary>
        public const string ServiceAuthenticationFailed = "E5004";

        /// <summary>
        /// �ⲿ��������
        /// </summary>
        public const string ServiceRateLimited = "E5005";
    }
}