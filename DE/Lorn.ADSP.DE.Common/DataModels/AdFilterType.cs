using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    /// <summary>
    /// 广告过滤器类型
    /// </summary>
    public enum AdFilterType
    {
        /// <summary>
        /// 排除过滤
        /// </summary>
        ExcludeFilter,
        /// <summary>
        /// 优选过滤
        /// </summary>
        PreferenceFilter,
        /// <summary>
        /// 随机过滤
        /// </summary>
        RandomFilter
    }
}
