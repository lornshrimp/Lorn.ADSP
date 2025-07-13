using Lorn.ADSP.Core.Domain.Enums;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 优化建议
    /// </summary>
    public class OptimizationRecommendation
    {
        public OptimizationType Type { get; set; }
        public string? CriteriaType { get; set; }
        public decimal? NewWeight { get; set; }
        public string? ParameterKey { get; set; }
        public object? ParameterValue { get; set; }
        public string? Reason { get; set; }
    }
}
