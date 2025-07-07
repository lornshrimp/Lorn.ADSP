namespace Lorn.ADSP.Core.Shared.Enums;

/// <summary>
/// 请求来源枚举
/// </summary>
public enum RequestSource
{
    /// <summary>
    /// 网站
    /// </summary>
    Website = 1,

    /// <summary>
    /// 移动应用
    /// </summary>
    MobileApp = 2,

    /// <summary>
    /// 桌面应用
    /// </summary>
    DesktopApp = 3,

    /// <summary>
    /// 微信小程序
    /// </summary>
    WechatMiniProgram = 4,

    /// <summary>
    /// 第三方平台
    /// </summary>
    ThirdPartyPlatform = 5,

    /// <summary>
    /// API直接调用
    /// </summary>
    ApiDirect = 6,

    /// <summary>
    /// 其他
    /// </summary>
    Other = 99
}
