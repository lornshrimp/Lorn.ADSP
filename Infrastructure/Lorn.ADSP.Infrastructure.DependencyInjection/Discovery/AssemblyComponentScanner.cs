using Lorn.ADSP.Infrastructure.Common.Extensions;
using Lorn.ADSP.Infrastructure.Common.Models;
using System.Reflection;

namespace Lorn.ADSP.Infrastructure.DependencyInjection.Discovery;

/// <summary>
/// 程序集组件扫描器
/// 用于扫描程序集中的组件类型
/// </summary>
public class AssemblyComponentScanner
{
    private readonly List<Assembly> _assemblies = new();
    private readonly List<string> _includePatterns = new();
    private readonly List<string> _excludePatterns = new();

    /// <summary>
    /// 添加要扫描的程序集
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <returns>扫描器实例</returns>
    public AssemblyComponentScanner AddAssembly(Assembly assembly)
    {
        if (assembly != null && !_assemblies.Contains(assembly))
        {
            _assemblies.Add(assembly);
        }
        return this;
    }

    /// <summary>
    /// 添加多个要扫描的程序集
    /// </summary>
    /// <param name="assemblies">程序集列表</param>
    /// <returns>扫描器实例</returns>
    public AssemblyComponentScanner AddAssemblies(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            AddAssembly(assembly);
        }
        return this;
    }

    /// <summary>
    /// 添加程序集名称模式
    /// </summary>
    /// <param name="pattern">名称模式</param>
    /// <returns>扫描器实例</returns>
    public AssemblyComponentScanner AddAssemblyPattern(string pattern)
    {
        if (!string.IsNullOrEmpty(pattern))
        {
            _includePatterns.Add(pattern);
        }
        return this;
    }

    /// <summary>
    /// 排除程序集名称模式
    /// </summary>
    /// <param name="pattern">排除模式</param>
    /// <returns>扫描器实例</returns>
    public AssemblyComponentScanner ExcludeAssemblyPattern(string pattern)
    {
        if (!string.IsNullOrEmpty(pattern))
        {
            _excludePatterns.Add(pattern);
        }
        return this;
    }

    /// <summary>
    /// 扫描所有类型
    /// </summary>
    /// <returns>类型列表</returns>
    public IEnumerable<Type> ScanTypes()
    {
        var types = new List<Type>();
        var assemblies = GetAllAssemblies();

        foreach (var assembly in assemblies)
        {
            try
            {
                var assemblyTypes = assembly.GetConcreteTypes();
                types.AddRange(assemblyTypes);
            }
            catch (Exception)
            {
                // 忽略程序集加载错误
            }
        }

        return types;
    }

    /// <summary>
    /// 扫描组件类型
    /// </summary>
    /// <returns>组件类型列表</returns>
    public IEnumerable<Type> ScanComponentTypes()
    {
        return ScanTypes().Where(IsComponentType);
    }

    /// <summary>
    /// 扫描实现指定接口的类型
    /// </summary>
    /// <param name="interfaceType">接口类型</param>
    /// <returns>实现接口的类型列表</returns>
    public IEnumerable<Type> ScanTypesImplementing(Type interfaceType)
    {
        return ScanTypes().Where(t => t.ImplementsInterface(interfaceType));
    }

    /// <summary>
    /// 扫描带有指定特性的类型
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <returns>带有特性的类型列表</returns>
    public IEnumerable<Type> ScanTypesWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return ScanTypes().Where(t => t.GetCustomAttributeSafe<TAttribute>() != null);
    }

    /// <summary>
    /// 获取所有要扫描的程序集
    /// </summary>
    /// <returns>程序集列表</returns>
    private IEnumerable<Assembly> GetAllAssemblies()
    {
        var assemblies = new HashSet<Assembly>(_assemblies);

        // 如果没有手动添加程序集，则根据模式扫描
        if (!assemblies.Any())
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            if (_includePatterns.Any())
            {
                // 根据包含模式添加程序集
                foreach (var assembly in loadedAssemblies)
                {
                    if (ShouldIncludeAssembly(assembly))
                    {
                        assemblies.Add(assembly);
                    }
                }
            }
            else
            {
                // 默认添加所有相关程序集
                var defaultPatterns = new[] { "Lorn.ADSP.*" };
                foreach (var assembly in loadedAssemblies)
                {
                    if (MatchesPatterns(assembly.FullName, defaultPatterns))
                    {
                        assemblies.Add(assembly);
                    }
                }
            }
        }

        return assemblies;
    }

    /// <summary>
    /// 检查是否应该包含程序集
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <returns>是否包含</returns>
    private bool ShouldIncludeAssembly(Assembly assembly)
    {
        var assemblyName = assembly.FullName ?? assembly.GetName().Name ?? "";

        // 检查排除模式
        if (_excludePatterns.Any(pattern => MatchesPattern(assemblyName, pattern)))
        {
            return false;
        }

        // 检查包含模式
        return !_includePatterns.Any() || _includePatterns.Any(pattern => MatchesPattern(assemblyName, pattern));
    }

    /// <summary>
    /// 检查名称是否匹配模式列表
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="patterns">模式列表</param>
    /// <returns>是否匹配</returns>
    private bool MatchesPatterns(string? name, IEnumerable<string> patterns)
    {
        if (string.IsNullOrEmpty(name)) return false;
        
        return patterns.Any(pattern => MatchesPattern(name, pattern));
    }

    /// <summary>
    /// 检查名称是否匹配模式
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="pattern">模式</param>
    /// <returns>是否匹配</returns>
    private bool MatchesPattern(string name, string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return false;
        
        // 简单的通配符匹配
        if (pattern.EndsWith("*"))
        {
            var prefix = pattern.Substring(0, pattern.Length - 1);
            return name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
        
        if (pattern.StartsWith("*"))
        {
            var suffix = pattern.Substring(1);
            return name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }
        
        return name.Equals(pattern, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 检查类型是否为组件类型
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>是否为组件类型</returns>
    private bool IsComponentType(Type type)
    {
        if (type == null || !type.IsConcreteClass())
            return false;

        // 检查是否有组件特性
        if (type.GetCustomAttributeSafe<Attributes.ComponentAttribute>() != null)
            return true;

        // 检查是否符合组件约定
        return Lorn.ADSP.Infrastructure.Common.Conventions.ComponentConventions.IsComponent(type);
    }
}