namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 优化上下文
    /// </summary>
    public class OptimizationContext
    {
        public PerformanceMetrics? PerformanceMetrics { get; set; }
        public List<OptimizationRecommendation>? OptimizationRecommendations { get; set; }
        public Dictionary<string, object>? AdditionalData { get; set; }
    }
}
