namespace Lorn.ADSP.Strategies.Targeting.Tests

/// <summary>
/// F#策略测试项目的入口模块
/// 提供测试运行和管理功能
/// </summary>
module TestRunner =

    /// <summary>
    /// 获取测试项目版本信息
    /// </summary>
    let getVersion () = "1.0.0"

    /// <summary>
    /// 初始化测试环境
    /// </summary>
    let initialize () =
        printfn "F#策略测试项目已初始化"
        true
