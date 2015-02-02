using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 用于信息服务网关的请求接口
    /// </summary>
    public interface IRequestGateway
    {
        /// <summary>
        /// 请求信息服务网关
        /// </summary>
        /// <param name="requestUrl">请求的原始URL</param>
        /// <param name="serviceNames">要请求的服务名称</param>
        /// <param name="parameters">请求参数</param>
        /// <returns></returns>
        string RequestGateway(string requestUrl, ICollection<string> serviceNames, Guid mediaId, Guid cookieId, Guid sessionId, Guid viewId, Guid requestId, IDictionary<string, string> parameters, string requestTime = null, string hash = null);
    }
}
