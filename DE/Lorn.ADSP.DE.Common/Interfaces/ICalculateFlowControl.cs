using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 流量控制曲线计算接口
    /// </summary>
    public interface ICalculateFlowControl
    {
        /// <summary>
        /// 计算流量控制曲线
        /// </summary>
        /// <param name="RedirctConditions">定向条件</param>
        /// <returns>流量控制曲线</returns>
        IDictionary<DateTime, long> CalculateFlowControl(IDictionary<Guid, RedirectCondition> RedirctConditions, FlowData flowData);
    }
}
