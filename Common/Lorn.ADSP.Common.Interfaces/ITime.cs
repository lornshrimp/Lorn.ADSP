using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.Common.Interfaces
{
    /// <summary>
    /// 时间接口，不取系统时间是为了方便进行模拟测试
    /// </summary>
    public interface ITime
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        DateTime Now { get; }
    }
}
