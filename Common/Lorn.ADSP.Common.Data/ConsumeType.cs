using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.Common.DataModels
{
    /// <summary>
    /// 消耗方式
    /// </summary>
    public enum ConsumeType : int
    {
        /// <summary>
        /// 快速消耗
        /// </summary>
        FastConsume = 0,
        /// <summary>
        /// 平均消耗
        /// </summary>
        AverageConsume = 1,
        /// <summary>
        /// 平滑消耗
        /// </summary>
        SmoothConsume = 2,
        /// <summary>
        /// 百分比消耗
        /// </summary>
        PercentageConsume = 3
    }
}
