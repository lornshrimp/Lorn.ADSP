namespace Lorn.ADSP.Strategies.Targeting

/// <summary>
/// F#定向匹配策略库的入口模块
/// 提供定向匹配引擎的核心功能和接口实现
/// </summary>
module TargetingStrategies =
    
    /// <summary>
    /// 获取当前策略库的版本信息
    /// </summary>
    /// <returns>版本字符串</returns>
    let getVersion () = "1.0.0"
    
    /// <summary>
    /// 初始化定向匹配策略库
    /// </summary>
    /// <returns>初始化结果</returns>
    let initialize () = 
        printfn "F#定向匹配策略库已初始化"
        true