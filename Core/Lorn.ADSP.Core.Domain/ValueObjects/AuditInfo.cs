using Lorn.ADSP.Core.Domain.Common;
using Lorn.ADSP.Core.Shared.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects;

/// <summary>
/// 审核信息值对象
/// </summary>
public class AuditInfo : ValueObject
{
    /// <summary>
    /// 审核状态
    /// </summary>
    public AuditStatus Status { get; private set; }

    /// <summary>
    /// 审核反馈信息
    /// </summary>
    public string? Feedback { get; private set; }

    /// <summary>
    /// 最后审核时间
    /// </summary>
    public DateTime LastAuditTime { get; private set; }

    /// <summary>
    /// 审核员ID
    /// </summary>
    public string? AuditorId { get; private set; }

    /// <summary>
    /// 审核员姓名
    /// </summary>
    public string? AuditorName { get; private set; }

    /// <summary>
    /// 修正建议
    /// </summary>
    public string? CorrectionSuggestion { get; private set; }

    /// <summary>
    /// 审核批次号
    /// </summary>
    public string? AuditBatchId { get; private set; }

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private AuditInfo() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    public AuditInfo(
        AuditStatus status,
        string? feedback = null,
        string? auditorId = null,
        string? auditorName = null,
        string? correctionSuggestion = null,
        string? auditBatchId = null)
    {
        Status = status;
        Feedback = feedback;
        LastAuditTime = DateTime.UtcNow;
        AuditorId = auditorId;
        AuditorName = auditorName;
        CorrectionSuggestion = correctionSuggestion;
        AuditBatchId = auditBatchId;
    }

    /// <summary>
    /// 创建待审核状态
    /// </summary>
    public static AuditInfo CreatePending()
    {
        return new AuditInfo(AuditStatus.Pending);
    }

    /// <summary>
    /// 创建审核中状态
    /// </summary>
    public static AuditInfo CreateInProgress(string? auditorId = null, string? auditorName = null)
    {
        return new AuditInfo(AuditStatus.InProgress, auditorId: auditorId, auditorName: auditorName);
    }

    /// <summary>
    /// 创建审核通过状态
    /// </summary>
    public static AuditInfo CreateApproved(string? auditorId = null, string? auditorName = null, string? feedback = null)
    {
        return new AuditInfo(AuditStatus.Approved, feedback, auditorId, auditorName);
    }

    /// <summary>
    /// 创建审核拒绝状态
    /// </summary>
    public static AuditInfo CreateRejected(string feedback, string? auditorId = null, string? auditorName = null)
    {
        return new AuditInfo(AuditStatus.Rejected, feedback, auditorId, auditorName);
    }

    /// <summary>
    /// 创建需要修改状态
    /// </summary>
    public static AuditInfo CreateRequiresChanges(string correctionSuggestion, string? auditorId = null, string? auditorName = null)
    {
        return new AuditInfo(AuditStatus.RequiresChanges, correctionSuggestion: correctionSuggestion, auditorId: auditorId, auditorName: auditorName);
    }

    /// <summary>
    /// 是否已审核通过
    /// </summary>
    public bool IsApproved => Status == AuditStatus.Approved;

    /// <summary>
    /// 是否被拒绝
    /// </summary>
    public bool IsRejected => Status == AuditStatus.Rejected;

    /// <summary>
    /// 是否需要修改
    /// </summary>
    public bool RequiresChanges => Status == AuditStatus.RequiresChanges;

    /// <summary>
    /// 是否已过期
    /// </summary>
    public bool IsExpired => Status == AuditStatus.Expired;

    /// <summary>
    /// 是否可以投放
    /// </summary>
    public bool CanDeliver => Status == AuditStatus.Approved;

    /// <summary>
    /// 获取相等性比较的组件
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Status;
        yield return Feedback ?? string.Empty;
        yield return LastAuditTime;
        yield return AuditorId ?? string.Empty;
        yield return AuditorName ?? string.Empty;
        yield return CorrectionSuggestion ?? string.Empty;
        yield return AuditBatchId ?? string.Empty;
    }
}