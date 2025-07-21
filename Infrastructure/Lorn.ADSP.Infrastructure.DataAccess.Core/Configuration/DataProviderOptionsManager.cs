using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lorn.ADSP.Infrastructure.DataAccess.Core.Configuration;

/// <summary>
/// 数据提供者配置选项管理器
/// 负责管理各个数据提供者的配置选项
/// </summary>
public class DataProviderOptionsManager
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DataProviderOptionsManager> _logger;
    private readonly Dictionary<string, object> _optionsCache = new();
    private readonly object _cacheLock = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="configuration">配置服务</param>
    /// <param name="logger">日志记录器</param>
    public DataProviderOptionsManager(
        IConfiguration configuration,
        ILogger<DataProviderOptionsManager> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 获取提供者配置选项
    /// </summary>
    /// <typeparam name="TOptions">配置选项类型</typeparam>
    /// <param name="sectionName">配置节名称</param>
    /// <returns>配置选项实例</returns>
    public TOptions GetOptions<TOptions>(string sectionName) where TOptions : class, new()
    {
        if (string.IsNullOrWhiteSpace(sectionName))
            throw new ArgumentException("Section name cannot be null or empty", nameof(sectionName));

        var cacheKey = $"{typeof(TOptions).FullName}_{sectionName}";

        lock (_cacheLock)
        {
            if (_optionsCache.TryGetValue(cacheKey, out var cachedOptions))
            {
                _logger.LogTrace("Retrieved cached options for section '{SectionName}'", sectionName);
                return (TOptions)cachedOptions;
            }

            var options = CreateOptions<TOptions>(sectionName);
            _optionsCache[cacheKey] = options;

            _logger.LogDebug("Created and cached options for section '{SectionName}' (Type: {OptionsType})",
                sectionName, typeof(TOptions).Name);

            return options;
        }
    }

    /// <summary>
    /// 获取提供者配置选项（异步）
    /// </summary>
    /// <typeparam name="TOptions">配置选项类型</typeparam>
    /// <param name="sectionName">配置节名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>配置选项实例</returns>
    public Task<TOptions> GetOptionsAsync<TOptions>(string sectionName, CancellationToken cancellationToken = default)
        where TOptions : class, new()
    {
        return Task.FromResult(GetOptions<TOptions>(sectionName));
    }

    /// <summary>
    /// 检查配置节是否存在
    /// </summary>
    /// <param name="sectionName">配置节名称</param>
    /// <returns>如果存在则返回true</returns>
    public bool SectionExists(string sectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
            return false;

        var section = _configuration.GetSection(sectionName);
        return section.Exists();
    }

    /// <summary>
    /// 获取配置节值
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>配置值</returns>
    public string? GetValue(string key)
    {
        return _configuration[key];
    }

    /// <summary>
    /// 获取配置节值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>配置值</returns>
    public T GetValue<T>(string key, T defaultValue = default!)
    {
        return _configuration.GetValue(key, defaultValue);
    }

    /// <summary>
    /// 验证配置选项
    /// </summary>
    /// <typeparam name="TOptions">配置选项类型</typeparam>
    /// <param name="options">配置选项实例</param>
    /// <param name="sectionName">配置节名称</param>
    /// <returns>验证结果</returns>
    public ValidationResult ValidateOptions<TOptions>(TOptions options, string sectionName)
        where TOptions : class
    {
        if (options == null)
            return new ValidationResult(false, "Options cannot be null");

        try
        {
            // 使用数据注解验证
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(options);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                options, validationContext, validationResults, true);

            if (!isValid)
            {
                var errors = validationResults.Select(r => r.ErrorMessage).ToArray();
                var errorMessage = string.Join("; ", errors);

                _logger.LogWarning("Configuration validation failed for section '{SectionName}': {Errors}",
                    sectionName, errorMessage);

                return new ValidationResult(false, errorMessage);
            }

            _logger.LogDebug("Configuration validation passed for section '{SectionName}'", sectionName);
            return new ValidationResult(true, "Validation passed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during configuration validation for section '{SectionName}'", sectionName);
            return new ValidationResult(false, $"Validation error: {ex.Message}");
        }
    }

    /// <summary>
    /// 重新加载配置选项
    /// </summary>
    /// <param name="sectionName">配置节名称</param>
    public void ReloadOptions(string sectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
            return;

        lock (_cacheLock)
        {
            var keysToRemove = _optionsCache.Keys
                .Where(key => key.EndsWith($"_{sectionName}"))
                .ToList();

            foreach (var key in keysToRemove)
            {
                _optionsCache.Remove(key);
            }

            _logger.LogInformation("Reloaded configuration options for section '{SectionName}'", sectionName);
        }
    }

    /// <summary>
    /// 清空所有缓存的配置选项
    /// </summary>
    public void ClearCache()
    {
        lock (_cacheLock)
        {
            var count = _optionsCache.Count;
            _optionsCache.Clear();

            _logger.LogInformation("Cleared all cached configuration options ({Count} items)", count);
        }
    }

    /// <summary>
    /// 获取所有配置节名称
    /// </summary>
    /// <returns>配置节名称集合</returns>
    public IEnumerable<string> GetAllSectionNames()
    {
        var sections = new List<string>();

        try
        {
            foreach (var section in _configuration.GetChildren())
            {
                sections.Add(section.Key);
                sections.AddRange(GetChildSectionNames(section, section.Key));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get all section names from configuration");
        }

        return sections.Distinct();
    }

    /// <summary>
    /// 导出配置为字典
    /// </summary>
    /// <param name="sectionName">配置节名称</param>
    /// <returns>配置字典</returns>
    public Dictionary<string, string?> ExportSectionAsDict(string sectionName)
    {
        var result = new Dictionary<string, string?>();

        if (string.IsNullOrWhiteSpace(sectionName))
            return result;

        try
        {
            var section = _configuration.GetSection(sectionName);

            foreach (var child in section.GetChildren())
            {
                FlattenSection(child, child.Key, result);
            }

            _logger.LogDebug("Exported configuration section '{SectionName}' as dictionary with {Count} entries",
                sectionName, result.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to export configuration section '{SectionName}' as dictionary", sectionName);
        }

        return result;
    }

    /// <summary>
    /// 创建配置选项实例
    /// </summary>
    /// <typeparam name="TOptions">配置选项类型</typeparam>
    /// <param name="sectionName">配置节名称</param>
    /// <returns>配置选项实例</returns>
    private TOptions CreateOptions<TOptions>(string sectionName) where TOptions : class, new()
    {
        var options = new TOptions();
        var section = _configuration.GetSection(sectionName);

        if (section.Exists())
        {
            section.Bind(options);
            _logger.LogTrace("Bound configuration section '{SectionName}' to options type '{OptionsType}'",
                sectionName, typeof(TOptions).Name);
        }
        else
        {
            _logger.LogWarning("Configuration section '{SectionName}' not found, using default options", sectionName);
        }

        return options;
    }

    /// <summary>
    /// 获取子配置节名称
    /// </summary>
    /// <param name="section">配置节</param>
    /// <param name="prefix">前缀</param>
    /// <returns>子配置节名称集合</returns>
    private IEnumerable<string> GetChildSectionNames(IConfigurationSection section, string prefix)
    {
        var childNames = new List<string>();

        foreach (var child in section.GetChildren())
        {
            var childPath = $"{prefix}:{child.Key}";
            childNames.Add(childPath);
            childNames.AddRange(GetChildSectionNames(child, childPath));
        }

        return childNames;
    }

    /// <summary>
    /// 扁平化配置节
    /// </summary>
    /// <param name="section">配置节</param>
    /// <param name="prefix">前缀</param>
    /// <param name="result">结果字典</param>
    private void FlattenSection(IConfigurationSection section, string prefix, Dictionary<string, string?> result)
    {
        if (section.Value != null)
        {
            result[prefix] = section.Value;
        }

        foreach (var child in section.GetChildren())
        {
            FlattenSection(child, $"{prefix}:{child.Key}", result);
        }
    }
}

/// <summary>
/// 验证结果
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="isValid">是否有效</param>
    /// <param name="message">验证消息</param>
    public ValidationResult(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// 验证消息
    /// </summary>
    public string Message { get; }
}
