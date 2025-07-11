using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.Requests;
using Lorn.ADSP.Core.Domain.ValueObjects.Targeting;
using Lorn.ADSP.Core.Shared.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 广告引擎数据访问回调接口
/// </summary>
public interface IAdEngineDataCallback : IAdEngineCallback
{
    /// <summary>
    /// 获取用户画像数据
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="fields">需要获取的字段列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户画像数据</returns>
    Task<UserProfile?> GetUserProfileAsync(
        string userId,
        IReadOnlyList<string>? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取广告候选集合
    /// </summary>
    /// <param name="request">广告候选请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>广告候选列表</returns>
    Task<IReadOnlyList<AdCandidate>> GetAdCandidatesAsync(
        AdCandidateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实时特征数据
    /// </summary>
    /// <param name="request">特征请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>特征向量</returns>
    Task<FeatureVector> GetRealTimeFeatureAsync(
        FeatureRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取历史数据
    /// </summary>
    /// <param name="request">历史数据请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>历史数据</returns>
    Task<HistoricalData> GetHistoricalDataAsync(
        HistoricalDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取用户画像数据
    /// </summary>
    /// <param name="userIds">用户ID列表</param>
    /// <param name="fields">需要获取的字段列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户画像数据字典</returns>
    Task<IReadOnlyDictionary<string, UserProfile>> GetUserProfilesBatchAsync(
        IReadOnlyList<string> userIds,
        IReadOnlyList<string>? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备信息</returns>
    Task<DeviceInfo?> GetDeviceInfoAsync(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取地理位置信息
    /// </summary>
    /// <param name="ipAddress">IP地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>地理位置信息</returns>
    Task<GeoInfo?> GetGeoInfoAsync(
        string ipAddress,
        CancellationToken cancellationToken = default);
}