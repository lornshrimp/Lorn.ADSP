using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    public interface ICacheRedirectConditionDefinitions
    {
        /// <summary>
        /// 缓存定向维度信息
        /// </summary>
        /// <param name="redirectConditionDefinitions">定向维度信息：媒体、定向维度Id，定向维度</param>
        void SetRedirectConditionDefinitions(IDictionary<Guid,IDictionary<Guid, RedirectDimension>> redirectConditionDefinitions);
        /// <summary>
        /// 获取缓存的定向维度信息
        /// </summary>
        /// <returns>媒体、定向维度Id，定向维度</returns>
        IDictionary<Guid,IDictionary<Guid, RedirectDimension>> GetRedirectConditionDefinitions();
        /// <summary>
        /// 缓存Ip库
        /// </summary>
        /// <param name="ipLibraries"></param>
        void SetIpLibraries(IDictionary<Guid,IDictionary<Guid,IpLibrary>> ipLibraries);
        /// <summary>
        /// 获取缓存的Ip库
        /// </summary>
        /// <returns></returns>
        IDictionary<Guid, IDictionary<Guid, IpLibrary>> GetIpLibraries();

    }
}
