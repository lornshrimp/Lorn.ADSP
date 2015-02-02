using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 信息服务代理
    /// </summary>
    public interface IRequestInformationServiceAgent
    {
        /// <summary>
        /// 请求信息服务代理
        /// </summary>
        /// <param name="requestUrl">原始请求Url</param>
        /// <param name="cookieId"></param>
        /// <param name="sessionId"></param>
        /// <param name="viewId"></param>
        /// <param name="requestId"></param>
        /// <param name="parameters">请求参数列表</param>
        /// <param name="preServiceReturnDatas">前置服务返回的数据</param>
        /// <returns></returns>
        string RequestInformationService(string requestUrl,Guid mediaId, Guid cookieId, Guid sessionId, Guid viewId, Guid requestId, IDictionary<string, string> parameters, IDictionary<string, string> preServiceReturnDatas = null);
    }
    public interface IRequestInformationServiceAgentMetadata
    {
        string ServiceName { get; }
        float Version { get; }
        string Description { get; }
        string[] PreServiceNames { get; }
    }
}
