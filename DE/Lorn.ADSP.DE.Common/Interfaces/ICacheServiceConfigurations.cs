using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 缓存服务配置信息
    /// </summary>
    public interface ICacheServiceConfigurations
    {
        /// <summary>
        /// 缓存服务配置信息
        /// </summary>
        /// <param name="serviceConfigurations">服务名称、服务器Id、配置项、配置值</param>
        void SetServiceConfigurations(IDictionary<string, IDictionary<string,IDictionary<string, object>>> serviceConfigurations);
        /// <summary>
        /// 读取缓存的服务配置信息
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serverId">服务器Id</param>
        /// <returns>配置项、配置值</returns>
        IDictionary<string, object> GetServiceConfiguration(string serviceName,string serverId);
    }
}
