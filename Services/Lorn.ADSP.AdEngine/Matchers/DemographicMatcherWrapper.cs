using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lorn.ADSP.Core.AdEngine.Abstractions.Interfaces;
using Lorn.ADSP.Core.AdEngine.Abstractions.Models;
using Lorn.ADSP.Core.Domain.Targeting;
using Lorn.ADSP.Core.Domain.ValueObjects;
using Lorn.ADSP.Core.Shared.Entities;
using static Lorn.ADSP.Strategies.Targeting.Core.MatchingAlgorithms;
using static Lorn.ADSP.Strategies.Targeting.Matchers.DemographicMatcher;

namespace Lorn.ADSP.AdEngine.Matchers
{
    /// <summary>
    /// 人口属性匹配器C#包装器
    /// 实现ITargetingMatcher接口，封装F#人口属性匹配逻辑
    /// </summary>
    public class DemographicMatcherWrapper : ITargetingMatcher
    {
        /// <summary>
        /// 匹配器唯一标识符
        /// </summary>
        public string MatcherId { get; } = "demographic-matcher-v1";

        /// <summary>
        /// 匹配器名称
        /// </summary>
        public string MatcherName { get; } = "人口属性定向匹配器";

        /// <summary>
        /// 匹配器版本
        /// </summary>
        public string Version { get; } = "1.0.0";

        /// <summary>
        /// 匹配器类型
        /// </summary>
        public string MatcherType { get; } = "Demographic";

        /// <summary>
        /// 执行优先级（数值越小优先级越高）
        /// </summary>
        public int Priority { get; } = 100;

        /// <summary>
        /// 匹配器是否启用
        /// </summary>
        public bool IsEnabled { get; } = true;

        /// <summary>
        /// 预期执行时间
        /// </summary>
        public TimeSpan ExpectedExecutionTime { get; } = TimeSpan.FromMilliseconds(10);

        /// <summary>
        /// 是否支持并行执行
        /// </summary>
        public bool CanRunInParallel { get; } = true;

        /// <summary>
        /// 计算匹配评分
        /// 根据用户上下文和定向条件计算匹配度评分
        /// </summary>
        /// <param name="context">定向上下文，包含用户画像信息</param>
        /// <param name="criteria">定向条件</param>
        /// <param name="callbackProvider">回调提供者，用于获取额外数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>匹配结果，包含评分、匹配状态、执行时间等信息</returns>
        public async Task<MatchResult> CalculateMatchScoreAsync(
            ITargetingContext context,
            ITargetingCriteria criteria,
            ICallbackProvider callbackProvider,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // 创建F#匹配上下文
                var matchingContext = createMatchingContext(
                    context,
                    criteria,
                    callbackProvider,
                    cancellationToken,
                    MatcherId,
                    MatcherType
                );

                // 调用F#匹配算法
                var fsharpResult = await Microsoft.FSharp.Control.FSharpAsync.StartAsTask(
                    executeMatching(matchingContext),
                    null,
                    cancellationToken
                );

                // 转换F#结果为C# MatchResult
                var matchResult = convertToMatchResult(
                    criteria.CriteriaType,
                    criteria.CriteriaId,
                    fsharpResult
                );

                return matchResult;
            }
            catch (OperationCanceledException)
            {
                // 处理取消操作
                return MatchResult.CreateNoMatch(
                    criteria.CriteriaType,
                    criteria.CriteriaId,
                    "人口属性匹配被取消",
                    TimeSpan.Zero,
                    Priority,
                    1.0m,
                    false,
                    new List<ContextProperty>()
                );
            }
            catch (Exception ex)
            {
                // 处理异常情况
                var errorProperties = new List<ContextProperty>
                {
                    new ContextProperty("Exception", ex.Message, "String", "Error", false, 1.0m, null, MatcherId),
                    new ContextProperty("StackTrace", ex.StackTrace ?? "", "String", "Error", false, 1.0m, null, MatcherId)
                };

                return MatchResult.CreateNoMatch(
                    criteria.CriteriaType,
                    criteria.CriteriaId,
                    $"人口属性匹配执行异常: {ex.Message}",
                    TimeSpan.Zero,
                    Priority,
                    1.0m,
                    false,
                    errorProperties
                );
            }
        }

        /// <summary>
        /// 检查是否支持指定的定向条件类型
        /// </summary>
        /// <param name="criteriaType">定向条件类型</param>
        /// <returns>是否支持该类型的定向条件</returns>
        public bool IsSupported(string criteriaType)
        {
            return isSupported(criteriaType);
        }

        /// <summary>
        /// 验证定向条件的有效性
        /// </summary>
        /// <param name="criteria">定向条件</param>
        /// <returns>验证结果</returns>
        public ValidationResult ValidateCriteria(ITargetingCriteria criteria)
        {
            try
            {
                var isValid = validateCriteria(criteria);

                if (isValid)
                {
                    return ValidationResult.Success("人口属性定向条件验证通过");
                }
                else
                {
                    var errors = new List<ValidationError>
                    {
                        new ValidationError { Message = "人口属性定向条件验证失败：条件格式不正确或包含无效值" }
                    };
                    return ValidationResult.Failure(errors, "人口属性定向条件验证失败");
                }
            }
            catch (Exception ex)
            {
                var errors = new List<ValidationError>
                {
                    new ValidationError { Message = $"人口属性定向条件验证异常: {ex.Message}" }
                };
                return ValidationResult.Failure(errors, "人口属性定向条件验证异常");
            }
        }

        /// <summary>
        /// 获取匹配器元数据信息
        /// </summary>
        /// <returns>匹配器元数据</returns>
        public TargetingMatcherMetadata GetMetadata()
        {
            var fsharpMetadata = getMatcherMetadata();

            var metadata = new TargetingMatcherMetadata
            {
                MatcherId = MatcherId,
                Name = MatcherName,
                MatcherType = MatcherType,
                Version = Version,
                Description = fsharpMetadata.ContainsKey("Description") ?
                    fsharpMetadata["Description"].ToString() ?? "人口属性定向匹配器" :
                    "人口属性定向匹配器",
                SupportedCriteriaTypes = getSupportedCriteriaTypes(),
                ExpectedExecutionTime = ExpectedExecutionTime,
                SupportsParallelExecution = CanRunInParallel,
                SupportsCaching = true,
                SupportsBatchProcessing = false,
                MaxExecutionTime = TimeSpan.FromMilliseconds(50),
                ResourceRequirement = new ResourceRequirement
                {
                    ExpectedMemoryUsageMB = 5.0,
                    MaxMemoryUsageMB = 20.0,
                    ExpectedCpuUsagePercent = 2.0,
                    MaxCpuUsagePercent = 10.0,
                    RequiresCacheAccess = true,
                    ConcurrencyCapability = 1000,
                    UsagePattern = ResourceUsagePattern.Steady
                },
                Tags = new List<string> { "demographic", "targeting", "user-profile" },
                ExtendedProperties = new Dictionary<string, object>(fsharpMetadata)
            };

            return metadata;
        }

        /// <summary>
        /// 预热匹配器（可选实现）
        /// 用于预加载数据、初始化缓存等操作
        /// </summary>
        /// <param name="callbackProvider">回调提供者</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>预热任务</returns>
        public Task WarmUpAsync(ICallbackProvider callbackProvider, CancellationToken cancellationToken = default)
        {
            // 人口属性匹配器无需特殊预热操作
            return Task.CompletedTask;
        }

        /// <summary>
        /// 清理匹配器资源（可选实现）
        /// </summary>
        /// <returns>清理任务</returns>
        public Task CleanupAsync()
        {
            // 人口属性匹配器无需特殊清理操作
            return Task.CompletedTask;
        }
    }
}