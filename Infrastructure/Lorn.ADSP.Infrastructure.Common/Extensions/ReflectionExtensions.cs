using System.Reflection;

namespace Lorn.ADSP.Infrastructure.Common.Extensions;

/// <summary>
/// 反射扩展方法
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// 获取程序集中所有的具体类类型
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <returns>具体类类型列表</returns>
    public static IEnumerable<Type> GetConcreteTypes(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes().Where(t => t.IsConcreteClass());
        }
        catch (ReflectionTypeLoadException ex)
        {
            // 处理类型加载异常，返回成功加载的类型
            return ex.Types.Where(t => t != null && t.IsConcreteClass())!;
        }
    }

    /// <summary>
    /// 获取程序集中实现指定接口的所有类型
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <param name="interfaceType">接口类型</param>
    /// <returns>实现接口的类型列表</returns>
    public static IEnumerable<Type> GetTypesImplementing(this Assembly assembly, Type interfaceType)
    {
        return assembly.GetConcreteTypes().Where(t => t.ImplementsInterface(interfaceType));
    }

    /// <summary>
    /// 获取程序集中实现指定接口名称的所有类型
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <param name="interfaceName">接口名称</param>
    /// <returns>实现接口的类型列表</returns>
    public static IEnumerable<Type> GetTypesImplementing(this Assembly assembly, string interfaceName)
    {
        return assembly.GetConcreteTypes().Where(t => t.ImplementsInterface(interfaceName));
    }

    /// <summary>
    /// 获取程序集中继承指定基类的所有类型
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <param name="baseType">基类类型</param>
    /// <returns>继承基类的类型列表</returns>
    public static IEnumerable<Type> GetTypesInheriting(this Assembly assembly, Type baseType)
    {
        return assembly.GetConcreteTypes().Where(t => baseType.IsAssignableFrom(t) && t != baseType);
    }

    /// <summary>
    /// 获取程序集中带有指定特性的所有类型
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <param name="attributeType">特性类型</param>
    /// <returns>带有特性的类型列表</returns>
    public static IEnumerable<Type> GetTypesWithAttribute(this Assembly assembly, Type attributeType)
    {
        return assembly.GetConcreteTypes().Where(t => t.GetCustomAttribute(attributeType) != null);
    }

    /// <summary>
    /// 获取程序集中带有指定特性的所有类型
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="assembly">程序集</param>
    /// <returns>带有特性的类型列表</returns>
    public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(this Assembly assembly)
        where TAttribute : Attribute
    {
        return assembly.GetTypesWithAttribute(typeof(TAttribute));
    }

    /// <summary>
    /// 安全地获取类型的自定义特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="type">类型</param>
    /// <returns>特性实例，如果不存在则返回null</returns>
    public static TAttribute? GetCustomAttributeSafe<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        try
        {
            return type.GetCustomAttribute<TAttribute>();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 安全地获取类型的所有自定义特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="type">类型</param>
    /// <returns>特性实例列表</returns>
    public static IEnumerable<TAttribute> GetCustomAttributesSafe<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        try
        {
            return type.GetCustomAttributes<TAttribute>();
        }
        catch
        {
            return Enumerable.Empty<TAttribute>();
        }
    }

    /// <summary>
    /// 创建类型的实例
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="args">构造函数参数</param>
    /// <returns>类型实例</returns>
    public static object? CreateInstance(this Type type, params object[] args)
    {
        try
        {
            return Activator.CreateInstance(type, args);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 安全地创建类型的实例
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="type">类型</param>
    /// <param name="args">构造函数参数</param>
    /// <returns>类型实例</returns>
    public static T? CreateInstance<T>(this Type type, params object[] args)
        where T : class
    {
        return CreateInstance(type, args) as T;
    }
}