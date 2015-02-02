using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 广告排序得分计算器接口
    /// </summary>
    public interface ICalculateAdScore
    {
        /// <summary>
        /// 计算广告得分
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="parameters">请求参数</param>
        /// <param name="ad">要计算的广告</param>
        /// <param name="filteredAds">已经筛选出来的广告：广告的结构化信息和已经序列化的数据</param>
        /// <param name="additionalParameters">附加参数：键值对</param>
        /// <param name="calculatorConfiguration">计算器配置</param>
        /// <param name="redirectConditionDefinitions">定向条件定义</param>
        /// <returns>计算出来的匹配度得分</returns>
        int CalculateAdScore(UserMediaCookie cookie, Dictionary<string, string> parameters, Ad ad, AdProcesserConfiguration calculatorConfiguration, IDictionary<Guid, RedirectDimension> redirectConditionDefinitions, IDictionary<Guid, Queue<AdMaterialReleaseInfo>> filteredAds = null, Dictionary<string, object> additionalParameters = null);
    }
    public interface IProcesserMetadata
    {
        Guid ProcesserId { get; }
        float Version { get; }
        string ProcesserName { get; }
        string Description { get; }
        /// <summary>
        /// 需求的附加参数列表
        /// </summary>
        string[] RequiredAdditionalParameters { get; }
        /// <summary>
        /// 需求的配置参数列表
        /// </summary>
        string[] RequiredConfigParameters { get; }
    }
    public interface IRedirectDimensionProcesserMetada:IProcesserMetadata
    {
        /// <summary>
        /// 定向维度Id
        /// </summary>
        Guid? RedirectDimensionId { get; }
    }
}
