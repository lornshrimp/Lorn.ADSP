namespace Lorn.ADSP.Core.Domain.Enums
{
    /// <summary>
    /// 规则评估策略
    /// </summary>
    public enum RuleEvaluationStrategy
    {
        /// <summary>
        /// 第一个匹配的规则
        /// </summary>
        FirstMatch = 1,

        /// <summary>
        /// 所有匹配的规则
        /// </summary>
        AllMatches = 2,

        /// <summary>
        /// 最高优先级的规则
        /// </summary>
        HighestPriority = 3,

        /// <summary>
        /// 加权评估
        /// </summary>
        WeightedEvaluation = 4
    }
}
