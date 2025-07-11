using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.Requests;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// ����������ݷ��ʻص��ӿ�
/// </summary>
public interface IAdEngineDataCallback : IAdEngineCallback
{
    /// <summary>
    /// ��ȡ�û���������
    /// </summary>
    /// <param name="userId">�û�ID</param>
    /// <param name="fields">��Ҫ��ȡ���ֶ��б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�û���������</returns>
    Task<UserProfile?> GetUserProfileAsync(
        string userId,
        IReadOnlyList<string>? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ����ѡ����
    /// </summary>
    /// <param name="request">����ѡ����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����ѡ�б�</returns>
    Task<IReadOnlyList<AdCandidate>> GetAdCandidatesAsync(
        AdCandidateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡʵʱ��������
    /// </summary>
    /// <param name="request">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task<FeatureVector> GetRealTimeFeatureAsync(
        FeatureRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��ʷ����
    /// </summary>
    /// <param name="request">��ʷ��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��ʷ����</returns>
    Task<HistoricalData> GetHistoricalDataAsync(
        HistoricalDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������ȡ�û���������
    /// </summary>
    /// <param name="userIds">�û�ID�б�</param>
    /// <param name="fields">��Ҫ��ȡ���ֶ��б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�û����������ֵ�</returns>
    Task<IReadOnlyDictionary<string, UserProfile>> GetUserProfilesBatchAsync(
        IReadOnlyList<string> userIds,
        IReadOnlyList<string>? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�豸��Ϣ
    /// </summary>
    /// <param name="deviceId">�豸ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�豸��Ϣ</returns>
    Task<DeviceInfo?> GetDeviceInfoAsync(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ����λ����Ϣ
    /// </summary>
    /// <param name="ipAddress">IP��ַ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����λ����Ϣ</returns>
    Task<GeoInfo?> GetGeoInfoAsync(
        string ipAddress,
        CancellationToken cancellationToken = default);
}