using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.Common.DataModels;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 广告调度计划计算接口
    /// </summary>
    public interface ICalculateAdDispatchPlan
    {

        /// <summary>
        /// 计算的时间片段时长
        /// </summary>
        TimeSpan CalculateTimeSpan { get; set; }
        /// <summary>
        /// 计算的时间片段数量
        /// </summary>
        int CalculateTimeSpanNumber { get; set; }
        /// <summary>
        /// 计算广告调度计划
        /// </summary>
        /// <param name="now">当前时点</param>
        /// <param name="planReleaseNumber">计划投放量</param>
        /// <param name="flowControl">流量控制数据</param>
        /// <returns>广告调度计划计算结果：时间点，投放量</returns>
        IDictionary<DateTime, long> CalculateAdDispatchPlan(DateTime now,long planReleaseNumber, IDictionary<DateTime, long> flowControl = null);
    }
    public interface ICalculateAdDispatchPlanMetadata
    {
        /// <summary>
        /// 计算器Id
        /// </summary>
        Guid CalculatorId { get; }
        int Version { get; }
        string CalculatorName { get; }
        string Description { get; }
        /// <summary>
        /// 计算针对的消耗方式
        /// </summary>
        ConsumeType ConsumeType { get; }
    }
}
