using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;
using System.Data;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// 数据提供程序接口
/// 抽象不同数据库的连接和操作
/// </summary>
public interface IDataProvider
{
    /// <summary>
    /// 提供程序名称
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// 连接字符串
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// 数据库类型
    /// </summary>
    DatabaseType DatabaseType { get; }

    /// <summary>
    /// 创建数据库连接
    /// </summary>
    /// <returns>数据库连接</returns>
    IDbConnection CreateConnection();

    /// <summary>
    /// 创建数据库命令
    /// </summary>
    /// <returns>数据库命令</returns>
    IDbCommand CreateCommand();

    /// <summary>
    /// 创建查询构建器
    /// </summary>
    /// <returns>查询构建器</returns>
    IQueryBuilder CreateQueryBuilder();

    /// <summary>
    /// 获取SQL方言
    /// </summary>
    /// <returns>SQL方言</returns>
    ISqlDialect GetDialect();

    /// <summary>
    /// 检查是否支持特定功能
    /// </summary>
    /// <param name="feature">数据库功能</param>
    /// <returns>是否支持</returns>
    bool SupportsFeature(DatabaseFeature feature);

    /// <summary>
    /// 测试连接
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>连接是否成功</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数据库版本信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>版本信息</returns>
    Task<DatabaseVersion> GetVersionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 云数据提供程序接口
/// 支持云平台特定的数据库服务
/// </summary>
public interface ICloudDataProvider
{
    /// <summary>
    /// 云平台类型
    /// </summary>
    CloudPlatform Platform { get; }

    /// <summary>
    /// 云区域
    /// </summary>
    string Region { get; }

    /// <summary>
    /// 云连接信息
    /// </summary>
    CloudConnectionInfo ConnectionInfo { get; }

    /// <summary>
    /// 创建数据提供程序
    /// </summary>
    /// <param name="dbType">数据库类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据提供程序</returns>
    Task<IDataProvider> CreateProviderAsync(DatabaseType dbType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试云连接
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>连接是否成功</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取连接健康状态
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>健康状态</returns>
    Task<ConnectionHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建数据库实例
    /// </summary>
    /// <param name="instanceOptions">实例选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据库实例信息</returns>
    Task<DatabaseInstanceInfo> CreateDatabaseInstanceAsync(DatabaseInstanceOptions instanceOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// 配置数据库安全
    /// </summary>
    /// <param name="securityOptions">安全选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>配置是否成功</returns>
    Task<bool> ConfigureSecurityAsync(DatabaseSecurityOptions securityOptions, CancellationToken cancellationToken = default);
}

/// <summary>
/// 查询构建器接口
/// 用于构建数据库特定的查询语句
/// </summary>
public interface IQueryBuilder
{
    /// <summary>
    /// 构建Select查询
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="columns">列名列表</param>
    /// <returns>查询构建器</returns>
    IQueryBuilder Select(string tableName, params string[] columns);

    /// <summary>
    /// 添加Where条件
    /// </summary>
    /// <param name="condition">查询条件</param>
    /// <returns>查询构建器</returns>
    IQueryBuilder Where(string condition);

    /// <summary>
    /// 添加Join连接
    /// </summary>
    /// <param name="joinType">连接类型</param>
    /// <param name="tableName">表名</param>
    /// <param name="condition">连接条件</param>
    /// <returns>查询构建器</returns>
    IQueryBuilder Join(JoinType joinType, string tableName, string condition);

    /// <summary>
    /// 添加排序
    /// </summary>
    /// <param name="columnName">列名</param>
    /// <param name="direction">排序方向</param>
    /// <returns>查询构建器</returns>
    IQueryBuilder OrderBy(string columnName, OrderDirection direction = OrderDirection.Ascending);

    /// <summary>
    /// 添加分页
    /// </summary>
    /// <param name="offset">偏移量</param>
    /// <param name="limit">限制数量</param>
    /// <returns>查询构建器</returns>
    IQueryBuilder Paging(int offset, int limit);

    /// <summary>
    /// 构建SQL语句
    /// </summary>
    /// <returns>SQL语句</returns>
    string Build();

    /// <summary>
    /// 构建参数化SQL语句
    /// </summary>
    /// <param name="parameters">输出参数</param>
    /// <returns>SQL语句</returns>
    string BuildParameterized(out IDictionary<string, object> parameters);
}

/// <summary>
/// SQL方言接口
/// 处理不同数据库的SQL语法差异
/// </summary>
public interface ISqlDialect
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    DatabaseType DatabaseType { get; }

    /// <summary>
    /// 转义标识符
    /// </summary>
    /// <param name="identifier">标识符</param>
    /// <returns>转义后的标识符</returns>
    string EscapeIdentifier(string identifier);

    /// <summary>
    /// 格式化参数名
    /// </summary>
    /// <param name="parameterName">参数名</param>
    /// <returns>格式化后的参数名</returns>
    string FormatParameterName(string parameterName);

    /// <summary>
    /// 获取分页SQL
    /// </summary>
    /// <param name="sql">基础SQL</param>
    /// <param name="offset">偏移量</param>
    /// <param name="limit">限制数量</param>
    /// <returns>分页SQL</returns>
    string GetPagingSql(string sql, int offset, int limit);

    /// <summary>
    /// 获取存在性检查SQL
    /// </summary>
    /// <param name="sql">基础SQL</param>
    /// <returns>存在性检查SQL</returns>
    string GetExistsSql(string sql);

    /// <summary>
    /// 获取计数SQL
    /// </summary>
    /// <param name="sql">基础SQL</param>
    /// <returns>计数SQL</returns>
    string GetCountSql(string sql);

    /// <summary>
    /// 获取当前时间SQL表达式
    /// </summary>
    /// <returns>当前时间SQL</returns>
    string GetCurrentTimestampSql();

    /// <summary>
    /// 获取UUID生成SQL表达式
    /// </summary>
    /// <returns>UUID生成SQL</returns>
    string GetNewUuidSql();
}