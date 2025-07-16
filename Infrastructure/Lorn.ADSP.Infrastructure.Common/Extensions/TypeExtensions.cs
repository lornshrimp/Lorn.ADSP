using System.Reflection;

namespace Lorn.ADSP.Infrastructure.Common.Extensions;

/// <summary>
/// 类型扩展方法
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// 获取类型的友好名称
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>友好名称</returns>
    public static string GetFriendlyName(this Type type)
    {
        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            var typeName = type.Name.Substring(0, type.Name.IndexOf('`'));
            var genericArgNames = string.Join(",", genericArgs.Select(x => x.GetFriendlyName()));
            return $"{typeName}<{genericArgNames}>";
        }
        
        return type.Name;
    }

    /// <summary>
    /// 检查类型是否实现了指定的接口
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="interfaceType">接口类型</param>
    /// <returns>是否实现接口</returns>
    public static bool ImplementsInterface(this Type type, Type interfaceType)
    {
        if (!interfaceType.IsInterface)
            return false;

        return interfaceType.IsAssignableFrom(type);
    }

    /// <summary>
    /// 检查类型是否实现了指定名称的接口
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="interfaceName">接口名称</param>
    /// <returns>是否实现接口</returns>
    public static bool ImplementsInterface(this Type type, string interfaceName)
    {
        return type.GetInterfaces().Any(i => i.Name == interfaceName);
    }

    /// <summary>
    /// 检查类型是否为具体类（非抽象、非接口、非泛型定义）
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>是否为具体类</returns>
    public static bool IsConcreteClass(this Type type)
    {
        return type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition;
    }

    /// <summary>
    /// 获取类型实现的所有接口（包括继承的接口）
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>接口列表</returns>
    public static IEnumerable<Type> GetAllInterfaces(this Type type)
    {
        var interfaces = new HashSet<Type>();
        
        foreach (var iface in type.GetInterfaces())
        {
            interfaces.Add(iface);
            foreach (var baseIface in iface.GetInterfaces())
            {
                interfaces.Add(baseIface);
            }
        }
        
        return interfaces;
    }

    /// <summary>
    /// 获取类型的默认构造函数
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>默认构造函数</returns>
    public static ConstructorInfo? GetDefaultConstructor(this Type type)
    {
        return type.GetConstructor(Type.EmptyTypes);
    }

    /// <summary>
    /// 检查类型是否有默认构造函数
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>是否有默认构造函数</returns>
    public static bool HasDefaultConstructor(this Type type)
    {
        return GetDefaultConstructor(type) != null;
    }

    /// <summary>
    /// 获取类型的所有公共构造函数
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>构造函数列表</returns>
    public static IEnumerable<ConstructorInfo> GetPublicConstructors(this Type type)
    {
        return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
    }
}