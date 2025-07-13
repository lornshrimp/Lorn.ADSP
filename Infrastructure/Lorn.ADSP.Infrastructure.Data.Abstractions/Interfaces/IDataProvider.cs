using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Models;
using System.Data;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// �����ṩ����ӿ�
/// ����ͬ���ݿ�����ӺͲ���
/// </summary>
public interface IDataProvider
{
    /// <summary>
    /// �ṩ��������
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// �����ַ���
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// ���ݿ�����
    /// </summary>
    DatabaseType DatabaseType { get; }

    /// <summary>
    /// �������ݿ�����
    /// </summary>
    /// <returns>���ݿ�����</returns>
    IDbConnection CreateConnection();

    /// <summary>
    /// �������ݿ�����
    /// </summary>
    /// <returns>���ݿ�����</returns>
    IDbCommand CreateCommand();

    /// <summary>
    /// ������ѯ������
    /// </summary>
    /// <returns>��ѯ������</returns>
    IQueryBuilder CreateQueryBuilder();

    /// <summary>
    /// ��ȡSQL����
    /// </summary>
    /// <returns>SQL����</returns>
    ISqlDialect GetDialect();

    /// <summary>
    /// ����Ƿ�֧���ض�����
    /// </summary>
    /// <param name="feature">���ݿ⹦��</param>
    /// <returns>�Ƿ�֧��</returns>
    bool SupportsFeature(DatabaseFeature feature);

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ�ɹ�</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ���ݿ�汾��Ϣ
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�汾��Ϣ</returns>
    Task<DatabaseVersion> GetVersionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// �������ṩ����ӿ�
/// ֧����ƽ̨�ض������ݿ����
/// </summary>
public interface ICloudDataProvider
{
    /// <summary>
    /// ��ƽ̨����
    /// </summary>
    CloudPlatform Platform { get; }

    /// <summary>
    /// ������
    /// </summary>
    string Region { get; }

    /// <summary>
    /// ��������Ϣ
    /// </summary>
    CloudConnectionInfo ConnectionInfo { get; }

    /// <summary>
    /// ���������ṩ����
    /// </summary>
    /// <param name="dbType">���ݿ�����</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����ṩ����</returns>
    Task<IDataProvider> CreateProviderAsync(DatabaseType dbType, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ�ɹ�</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ���ӽ���״̬
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����״̬</returns>
    Task<ConnectionHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// �������ݿ�ʵ��
    /// </summary>
    /// <param name="instanceOptions">ʵ��ѡ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���ݿ�ʵ����Ϣ</returns>
    Task<DatabaseInstanceInfo> CreateDatabaseInstanceAsync(DatabaseInstanceOptions instanceOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// �������ݿⰲȫ
    /// </summary>
    /// <param name="securityOptions">��ȫѡ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����Ƿ�ɹ�</returns>
    Task<bool> ConfigureSecurityAsync(DatabaseSecurityOptions securityOptions, CancellationToken cancellationToken = default);
}

/// <summary>
/// ��ѯ�������ӿ�
/// ���ڹ������ݿ��ض��Ĳ�ѯ���
/// </summary>
public interface IQueryBuilder
{
    /// <summary>
    /// ����Select��ѯ
    /// </summary>
    /// <param name="tableName">����</param>
    /// <param name="columns">�����б�</param>
    /// <returns>��ѯ������</returns>
    IQueryBuilder Select(string tableName, params string[] columns);

    /// <summary>
    /// ���Where����
    /// </summary>
    /// <param name="condition">��ѯ����</param>
    /// <returns>��ѯ������</returns>
    IQueryBuilder Where(string condition);

    /// <summary>
    /// ���Join����
    /// </summary>
    /// <param name="joinType">��������</param>
    /// <param name="tableName">����</param>
    /// <param name="condition">��������</param>
    /// <returns>��ѯ������</returns>
    IQueryBuilder Join(JoinType joinType, string tableName, string condition);

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="columnName">����</param>
    /// <param name="direction">������</param>
    /// <returns>��ѯ������</returns>
    IQueryBuilder OrderBy(string columnName, OrderDirection direction = OrderDirection.Ascending);

    /// <summary>
    /// ��ӷ�ҳ
    /// </summary>
    /// <param name="offset">ƫ����</param>
    /// <param name="limit">��������</param>
    /// <returns>��ѯ������</returns>
    IQueryBuilder Paging(int offset, int limit);

    /// <summary>
    /// ����SQL���
    /// </summary>
    /// <returns>SQL���</returns>
    string Build();

    /// <summary>
    /// ����������SQL���
    /// </summary>
    /// <param name="parameters">�������</param>
    /// <returns>SQL���</returns>
    string BuildParameterized(out IDictionary<string, object> parameters);
}

/// <summary>
/// SQL���Խӿ�
/// ����ͬ���ݿ��SQL�﷨����
/// </summary>
public interface ISqlDialect
{
    /// <summary>
    /// ���ݿ�����
    /// </summary>
    DatabaseType DatabaseType { get; }

    /// <summary>
    /// ת���ʶ��
    /// </summary>
    /// <param name="identifier">��ʶ��</param>
    /// <returns>ת���ı�ʶ��</returns>
    string EscapeIdentifier(string identifier);

    /// <summary>
    /// ��ʽ��������
    /// </summary>
    /// <param name="parameterName">������</param>
    /// <returns>��ʽ����Ĳ�����</returns>
    string FormatParameterName(string parameterName);

    /// <summary>
    /// ��ȡ��ҳSQL
    /// </summary>
    /// <param name="sql">����SQL</param>
    /// <param name="offset">ƫ����</param>
    /// <param name="limit">��������</param>
    /// <returns>��ҳSQL</returns>
    string GetPagingSql(string sql, int offset, int limit);

    /// <summary>
    /// ��ȡ�����Լ��SQL
    /// </summary>
    /// <param name="sql">����SQL</param>
    /// <returns>�����Լ��SQL</returns>
    string GetExistsSql(string sql);

    /// <summary>
    /// ��ȡ����SQL
    /// </summary>
    /// <param name="sql">����SQL</param>
    /// <returns>����SQL</returns>
    string GetCountSql(string sql);

    /// <summary>
    /// ��ȡ��ǰʱ��SQL���ʽ
    /// </summary>
    /// <returns>��ǰʱ��SQL</returns>
    string GetCurrentTimestampSql();

    /// <summary>
    /// ��ȡUUID����SQL���ʽ
    /// </summary>
    /// <returns>UUID����SQL</returns>
    string GetNewUuidSql();
}