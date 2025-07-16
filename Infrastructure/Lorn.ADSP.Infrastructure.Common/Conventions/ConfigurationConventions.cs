namespace Lorn.ADSP.Infrastructure.Common.Conventions;

/// <summary>
/// 配置约定规范
/// </summary>
public static class ConfigurationConventions
{
    /// <summary>
    /// 配置节名称定义
    /// </summary>
    public static class SectionNames
    {
        public const string AdEngine = "AdEngine";
        public const string DataAccess = "DataAccess";
        public const string Strategies = "Strategies";
        public const string DataProviders = "DataProviders";
        public const string Targeting = "Targeting";
        public const string Bidding = "Bidding";
        public const string Creative = "Creative";
        public const string Monitoring = "Monitoring";
        public const string Security = "Security";
        public const string Performance = "Performance";
        public const string Services = "Services";
        public const string Managers = "Managers";
        public const string Providers = "Providers";
        public const string CallbackProviders = "CallbackProviders";
        public const string Matchers = "Matchers";
        public const string Calculators = "Calculators";
        public const string Processors = "Processors";
    }

    /// <summary>
    /// 通用配置属性名称
    /// </summary>
    public static class CommonPropertyNames
    {
        public const string IsEnabled = "IsEnabled";
        public const string Priority = "Priority";
        public const string Timeout = "Timeout";
        public const string MaxRetries = "MaxRetries";
        public const string RetryInterval = "RetryInterval";
        public const string CacheExpiration = "CacheExpiration";
        public const string ConnectionString = "ConnectionString";
        public const string ApiEndpoint = "ApiEndpoint";
        public const string LogLevel = "LogLevel";
    }

    /// <summary>
    /// 根据组件类型获取配置节路径
    /// </summary>
    /// <param name="componentType">组件类型</param>
    /// <returns>配置节路径</returns>
    public static string GetConfigurationPath(Type componentType)
    {
        var typeName = componentType.Name;
        
        if (typeName.EndsWith("Strategy"))
        {
            var strategyName = typeName.Substring(0, typeName.Length - "Strategy".Length);
            return $"{SectionNames.Strategies}:{strategyName}";
        }
        
        if (typeName.EndsWith("Provider") && !typeName.EndsWith("CallbackProvider"))
        {
            var providerName = typeName.Substring(0, typeName.Length - "Provider".Length);
            return $"{SectionNames.DataProviders}:{providerName}";
        }
        
        if (typeName.EndsWith("CallbackProvider"))
        {
            var providerName = typeName.Substring(0, typeName.Length - "CallbackProvider".Length);
            return $"{SectionNames.CallbackProviders}:{providerName}";
        }
        
        if (typeName.EndsWith("Service"))
        {
            var serviceName = typeName.Substring(0, typeName.Length - "Service".Length);
            return $"{SectionNames.Services}:{serviceName}";
        }
        
        if (typeName.EndsWith("Manager"))
        {
            var managerName = typeName.Substring(0, typeName.Length - "Manager".Length);
            return $"{SectionNames.Managers}:{managerName}";
        }
        
        if (typeName.EndsWith("Matcher"))
        {
            var matcherName = typeName.Substring(0, typeName.Length - "Matcher".Length);
            return $"{SectionNames.Matchers}:{matcherName}";
        }
        
        if (typeName.EndsWith("Calculator"))
        {
            var calculatorName = typeName.Substring(0, typeName.Length - "Calculator".Length);
            return $"{SectionNames.Calculators}:{calculatorName}";
        }
        
        if (typeName.EndsWith("Processor"))
        {
            var processorName = typeName.Substring(0, typeName.Length - "Processor".Length);
            return $"{SectionNames.Processors}:{processorName}";
        }
        
        // 默认使用类型名称
        return NamingConventions.GetConfigurationSectionName(typeName);
    }

    /// <summary>
    /// 检查配置属性是否为通用属性
    /// </summary>
    /// <param name="propertyName">属性名称</param>
    /// <returns>是否为通用属性</returns>
    public static bool IsCommonProperty(string propertyName)
    {
        return propertyName == CommonPropertyNames.IsEnabled ||
               propertyName == CommonPropertyNames.Priority ||
               propertyName == CommonPropertyNames.Timeout ||
               propertyName == CommonPropertyNames.MaxRetries ||
               propertyName == CommonPropertyNames.RetryInterval ||
               propertyName == CommonPropertyNames.CacheExpiration ||
               propertyName == CommonPropertyNames.ConnectionString ||
               propertyName == CommonPropertyNames.ApiEndpoint ||
               propertyName == CommonPropertyNames.LogLevel;
    }
}