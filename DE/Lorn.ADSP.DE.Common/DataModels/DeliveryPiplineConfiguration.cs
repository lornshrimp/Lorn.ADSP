using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorn.ADSP.DE.DataModels
{
    /// <summary>
    /// 投放处理管线配置
    /// </summary>
    public struct DeliveryPiplineConfiguration
    {
        /// <summary>
        /// 管线配置Id
        /// </summary>
        public Guid DeliveryPiplineConfigurationId { get; set; }
        /// <summary>
        /// 管线配置名称
        /// </summary>
        public string DeliveryPiplineConfigurationName { get; set; }
       
        /// <summary>
        /// 版本号
        /// </summary>
        public float Version { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 投放策略
        /// </summary>
        public AdProcesserConfiguration AdDeliveryPolicy { get; set; }
        /// <summary>
        /// 排除过滤器
        /// </summary>
        public Queue<AdProcesserConfiguration> ExcludeFilters { get; set; }
        /// <summary>
        /// 优选过滤器
        /// </summary>
        public Queue<AdProcesserConfiguration> PreferredFilters { get; set; }
        /// <summary>
        /// 随机过滤器
        /// </summary>
        public AdProcesserConfiguration RandomFilter { get; set; }
        /// <summary>
        /// 物料过滤器
        /// </summary>
        public AdProcesserConfiguration AdMaterialFilter { get; set; }
        
    }
    /// <summary>
    /// 处理器参数
    /// </summary>
    public class AdProcesserConfiguration
    {
        protected IDictionary<string, object> StaticParameters;
        /// <summary>
        /// 处理器Id
        /// </summary>
        public Guid ProcesserId { get; set; }
        public float ProcesserMinVersion { get; set; }
        /// <summary>
        /// 获取静态参数
        /// </summary>
        public T GetStaticParameter<T>(string key)
        {
            return (T)this.StaticParameters[key];
        }
        /// <summary>
        /// 附加处理器
        /// </summary>
        public ICollection<AdProcesserConfiguration> ExtendedProcessers { get; set; }
    }
}
