using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lorn.ADSP.DE.Interfaces;
using System.ComponentModel.Composition;

namespace Lorn.ADSP.DE.GatewayServices
{
    /// <summary>
    /// 网关处理器
    /// </summary>
    public class GatewayHandler : IHttpHandler
    {
        [Import(typeof(IRequestGateway),RequiredCreationPolicy=CreationPolicy.Shared)]
        public Lazy<IRequestGateway> gateway;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var serviceNames = context.Request.Params["Services"].Split((".").ToCharArray());
            var cookieId = new Guid(context.Request.Cookies["CookieId"].ToString());
           
            var parameters = new Dictionary<string, string>();
            foreach (var item in context.Request.Params.AllKeys)
            {
                parameters[item] = context.Request.Params[item];
            }
            var sessionId = new Guid(parameters["SessionId"]);
            var requestId = new Guid(parameters["RequestId"]);
            var viewId = new Guid(parameters["ViewId"]);
            var mediaId = new Guid(parameters["MediaId"]);
            var data = this.gateway.Value.RequestGateway(context.Request.Url.ToString(), serviceNames, mediaId, cookieId, sessionId, viewId, requestId, parameters);
            context.Response.Write(data);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}