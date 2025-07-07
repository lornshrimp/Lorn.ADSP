namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// ���۲���
/// </summary>
public enum BiddingStrategy
{
    /// <summary>
    /// �̶�����
    /// </summary>
    FixedBid = 1,

    /// <summary>
    /// �Զ�����
    /// </summary>
    AutoBid = 2,

    /// <summary>
    /// Ŀ��CPA
    /// </summary>
    TargetCpa = 3,

    /// <summary>
    /// Ŀ��CPC
    /// </summary>
    TargetCpc = 4,

    /// <summary>
    /// Ŀ��CPM
    /// </summary>
    TargetCpm = 5,

    /// <summary>
    /// Ŀ��ROAS
    /// </summary>
    TargetRoas = 6,

    /// <summary>
    /// ��󻯵��
    /// </summary>
    MaximizeClicks = 7,

    /// <summary>
    /// ���ת��
    /// </summary>
    MaximizeConversions = 8,

    /// <summary>
    /// �������
    /// </summary>
    MaximizeRevenue = 9,
    /// <summary>
    /// Ŀ��CPA
    /// </summary>
    TargetCPA = 10,
    /// <summary>
    /// Ŀ��ROAS
    /// </summary>
    TargetROAS = 11
}