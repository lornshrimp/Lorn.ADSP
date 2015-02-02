using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 广告请求接口
    /// </summary>
    public interface IRequestAd
    {
        /// <summary>
        /// 请求广告
        /// </summary>
        /// <param name="reuqestUrl">请求的原始URL</param>
        /// <param name="adPositionOrAdPositionGroupCodes">广告位或广告位组Code</param>
        /// <param name="cookieId"></param>
        /// <param name="sessionId"></param>
        /// <param name="requestId"></param>
        /// <param name="parameters"></param>
        /// <returns>序列化后的广告返回信息</returns>
        string RequestAd(string reuqestUrl,ICollection<string> adPositionOrAdPositionGroupCodes,Guid meidiaId, Guid cookieId, Guid sessionId, Guid viewId, Guid requestId, IDictionary<string, string> parameters); 
    }
}
