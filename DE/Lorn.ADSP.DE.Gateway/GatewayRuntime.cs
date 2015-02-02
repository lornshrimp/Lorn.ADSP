using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.Interfaces;
using System.ComponentModel.Composition;
using System.Timers;

namespace Lorn.ADSP.DE.Gateway
{
    [Export(typeof(IRequestGateway))]
    public class GatewayRuntime : IRequestGateway, IDisposable
    {
        protected IDictionary<Guid, Guid> mediaSecureKeys;

        protected Timer timer;
        protected bool readingCache;

        [Import(typeof(ICacheMediaSecureKeys), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICacheMediaSecureKeys MediaScureKeysCacher { get; set; }

        [ImportMany(typeof(IRequestInformationServiceAgent), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ICollection<Lazy<IRequestInformationServiceAgent, IRequestInformationServiceAgentMetadata>> InformationServiceAgents { get; set; }
        [Import(typeof(ISerializeRequestedServiceInformations), RequiredCreationPolicy = CreationPolicy.Shared)]
        public ISerializeRequestedServiceInformations RequestedInformationsSerializer { get; set; }
        #region IRequestGateway 成员

        public GatewayRuntime()
        {
            timer = new Timer();
            timer.AutoReset = true;
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 300000;
            timer.Start();
            ReadCache();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (readingCache == false)
            {
                ReadCache();
            }
        }
        protected void ReadCache()
        {
            readingCache = true;
            var keys = this.MediaScureKeysCacher.GetMediaSecureKeys();
            if (keys != null)
            {
                this.mediaSecureKeys = keys;
            }
            readingCache = false;
        }

        public string RequestGateway(string requestUrl, ICollection<string> serviceNames, Guid mediaId, Guid cookieId, Guid sessionId, Guid viewId, Guid requestId, IDictionary<string, string> parameters, string requestTime = null, string hash = null)
        {
            if (this.mediaSecureKeys.ContainsKey(mediaId))
            {
                var datas = new Dictionary<string, string>();
                var returnDatas = new Dictionary<string, string>();
                foreach (var serviceName in serviceNames)
                {
                    if (!datas.ContainsKey(serviceName))
                    {
                        RequestInformationService(datas, serviceName, requestUrl, mediaId, cookieId, sessionId, viewId, requestId, parameters);
                    }
                    returnDatas[serviceName] = datas[serviceName];
                }

                return RequestedInformationsSerializer.SerialzeRequestedServiceInformations(returnDatas);
            }
            return string.Empty;
        }

        protected void RequestInformationService(IDictionary<string, string> returnedDatas, string serviceName, string requestUrl, Guid mediaId, Guid cookieId, Guid sessionId, Guid viewId, Guid requestId, IDictionary<string, string> parameters)
        {
            var agent = this.InformationServiceAgents.First(o => o.Metadata.ServiceName == serviceName);
            if (agent.Metadata.PreServiceNames == null)
            {
                returnedDatas[serviceName] = agent.Value.RequestInformationService(requestUrl, mediaId, cookieId, sessionId, viewId, requestId, parameters);
            }
            else
            {
                foreach (var preServiceName in agent.Metadata.PreServiceNames)
                {
                    if (!returnedDatas.ContainsKey(preServiceName))
                    {
                        RequestInformationService(returnedDatas, preServiceName, requestUrl, mediaId, cookieId, sessionId, viewId, requestId, parameters);
                    }
                }
                returnedDatas[serviceName] = agent.Value.RequestInformationService(requestUrl, mediaId, cookieId, sessionId, viewId, requestId, parameters, returnedDatas);
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool ok)
        {
            timer.Dispose();
        }

        #endregion
    }
}
