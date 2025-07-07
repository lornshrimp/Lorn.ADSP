using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.BusinessRules;
using Lorn.ADSP.Core.Shared.Entities;

namespace Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// 广告引擎配置管理回调接口
/// </summary>
public interface IAdEngineConfigCallback : IAdEngineCallback
{
    /// <summary>
    /// 获取策略配置
    /// </summary>
    /// <param name="strategyId">策略ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>策略配置</returns>
    Task<StrategyConfig?> GetStrategyConfigAsync(
        string strategyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取业务规则配置
    /// </summary>
    /// <param name="ruleType">规则类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>业务规则</returns>
    Task<BusinessRules> GetBusinessRulesAsync(
        string ruleType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取阈值参数
    /// </summary>
    /// <param name="paramType">参数类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>阈值参数字典</returns>
    Task<IReadOnlyDictionary<string, object>> GetThresholdParametersAsync(
        string paramType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取A/B测试配置
    /// </summary>
    /// <param name="experimentId">实验ID</param>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>A/B测试配置</returns>
    Task<ABTestConfig?> GetABTestConfigAsync(
        string experimentId,
        string? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取特征配置
    /// </summary>
    /// <param name="featureKey">特征键</param>
    /// <param name="defaultValue">默认值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>特征值</returns>
    Task<T> GetFeatureFlagAsync<T>(
        string featureKey,
        T defaultValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取策略配置
    /// </summary>
    /// <param name="strategyIds">策略ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>策略配置字典</returns>
    Task<IReadOnlyDictionary<string, StrategyConfig>> GetStrategyConfigsBatchAsync(
        IReadOnlyList<string> strategyIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取动态参数
    /// </summary>
    /// <param name="section">配置节</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>动态参数配置</returns>
    Task<DynamicParameters> GetDynamicParametersAsync(
        string section,
        CancellationToken cancellationToken = default);
}