namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// ���۲���ö��
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
    /// Ŀ��CPA����
    /// </summary>
    TargetCPA = 3,

    /// <summary>
    /// Ŀ��ROAS����
    /// </summary>
    TargetROAS = 4,

    /// <summary>
    /// ��󻯵����
    /// </summary>
    MaximizeClicks = 5,

    /// <summary>
    /// ���ת����
    /// </summary>
    MaximizeConversions = 6
}