using Lorn.ADSP.Infrastructure.Common.Models;

namespace Lorn.ADSP.Infrastructure.DependencyInjection.Attributes;

/// <summary>
/// 依赖声明特性
/// 用于显式声明组件的依赖关系
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependencyAttribute : Attribute
{
    /// <summary>
    /// 依赖的服务类型
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    /// 是否为可选依赖
    /// </summary>
    public bool IsOptional { get; set; } = false;

    /// <summary>
    /// 依赖的生命周期要求
    /// </summary>
    public ServiceLifetime? RequiredLifetime { get; set; }

    /// <summary>
    /// 依赖描述
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// 初始化依赖声明特性
    /// </summary>
    /// <param name="serviceType">依赖的服务类型</param>
    public DependencyAttribute(Type serviceType)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
    }
}