using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 监测服务状态接口
    /// </summary>
    public interface IMonitorServiceStatus
    {
        /// <summary>
        /// 服务初始化监测
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serverId">服务器Id</param>
        void InitService(string serviceName, string serverId, int resourceNumber = 100);
        /// <summary>
        /// 服务初始化完成
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="ServerId">服务器Id</param>
        void InitServiceComplete(string serviceName, string ServerId);
        /// <summary>
        /// 服务心跳监测
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serverId">服务器id</param>
        /// <param name="resourceNumber">资源数量</param>
        void ReportStatus(string serviceName, string serverId, int resourceNumber = 100);
        /// <summary>
        /// 获取服务状态
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务状态：服务器Id，服务状体，资源数量</returns>
        Dictionary<string, KeyValuePair<ServiceStatuses,int?>> GetServerStatuses(string serviceName);
    }
}
