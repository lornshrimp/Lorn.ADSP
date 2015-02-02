using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.Common.DataModels
{
    /// <summary>
    /// 计划状态
    /// </summary>
    public enum PlanStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        Draft,
        /// <summary>
        /// 已发布
        /// </summary>
        Published,
        /// <summary>
        /// 暂停
        /// </summary>
        Suspend,
        /// <summary>
        /// 终止
        /// </summary>
        Stoped
    }
}
