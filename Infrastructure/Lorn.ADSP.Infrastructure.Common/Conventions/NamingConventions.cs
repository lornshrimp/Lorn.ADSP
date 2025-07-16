namespace Lorn.ADSP.Infrastructure.Common.Conventions;

/// <summary>
/// 命名约定规范
/// </summary>
public static class NamingConventions
{
    /// <summary>
    /// 组件命名后缀定义
    /// </summary>
    public static class ComponentSuffixes
    {
        public const string Strategy = "Strategy";
        public const string Service = "Service";
        public const string Manager = "Manager";
        public const string Provider = "Provider";
        public const string CallbackProvider = "CallbackProvider";
        public const string Matcher = "Matcher";
        public const string Calculator = "Calculator";
        public const string Processor = "Processor";
        public const string Handler = "Handler";
        public const string Factory = "Factory";
        public const string Repository = "Repository";
        public const string Controller = "Controller";
    }

    /// <summary>
    /// 配置类命名后缀
    /// </summary>
    public static class ConfigurationSuffixes
    {
        public const string Options = "Options";
        public const string Settings = "Settings";
        public const string Configuration = "Configuration";
    }

    /// <summary>
    /// 接口命名前缀
    /// </summary>
    public static class InterfacePrefixes
    {
        public const string Interface = "I";
    }

    /// <summary>
    /// 检查类型名称是否符合组件命名约定
    /// </summary>
    /// <param name="typeName">类型名称</param>
    /// <returns>是否符合约定</returns>
    public static bool IsComponentType(string typeName)
    {
        return typeName.EndsWith(ComponentSuffixes.Strategy) ||
               typeName.EndsWith(ComponentSuffixes.Service) ||
               typeName.EndsWith(ComponentSuffixes.Manager) ||
               typeName.EndsWith(ComponentSuffixes.Provider) ||
               typeName.EndsWith(ComponentSuffixes.CallbackProvider) ||
               typeName.EndsWith(ComponentSuffixes.Matcher) ||
               typeName.EndsWith(ComponentSuffixes.Calculator) ||
               typeName.EndsWith(ComponentSuffixes.Processor) ||
               typeName.EndsWith(ComponentSuffixes.Handler) ||
               typeName.EndsWith(ComponentSuffixes.Factory) ||
               typeName.EndsWith(ComponentSuffixes.Repository) ||
               typeName.EndsWith(ComponentSuffixes.Controller);
    }

    /// <summary>
    /// 检查类型名称是否符合配置类命名约定
    /// </summary>
    /// <param name="typeName">类型名称</param>
    /// <returns>是否符合约定</returns>
    public static bool IsConfigurationType(string typeName)
    {
        return typeName.EndsWith(ConfigurationSuffixes.Options) ||
               typeName.EndsWith(ConfigurationSuffixes.Settings) ||
               typeName.EndsWith(ConfigurationSuffixes.Configuration);
    }

    /// <summary>
    /// 根据组件类型名称获取对应的接口名称
    /// </summary>
    /// <param name="componentTypeName">组件类型名称</param>
    /// <returns>接口名称</returns>
    public static string GetInterfaceName(string componentTypeName)
    {
        return $"{InterfacePrefixes.Interface}{componentTypeName}";
    }

    /// <summary>
    /// 根据配置类名称获取配置节名称
    /// </summary>
    /// <param name="configurationTypeName">配置类名称</param>
    /// <returns>配置节名称</returns>
    public static string GetConfigurationSectionName(string configurationTypeName)
    {
        if (configurationTypeName.EndsWith(ConfigurationSuffixes.Options))
            return configurationTypeName.Substring(0, configurationTypeName.Length - ConfigurationSuffixes.Options.Length);
        
        if (configurationTypeName.EndsWith(ConfigurationSuffixes.Settings))
            return configurationTypeName.Substring(0, configurationTypeName.Length - ConfigurationSuffixes.Settings.Length);
        
        if (configurationTypeName.EndsWith(ConfigurationSuffixes.Configuration))
            return configurationTypeName.Substring(0, configurationTypeName.Length - ConfigurationSuffixes.Configuration.Length);
        
        return configurationTypeName;
    }
}