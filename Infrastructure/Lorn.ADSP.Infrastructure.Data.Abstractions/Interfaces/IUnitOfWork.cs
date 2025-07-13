using Lorn.ADSP.Core.Domain.Aggregates;
using Lorn.ADSP.Core.Domain.Entities;
using Lorn.ADSP.Core.Domain.ValueObjects;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

/// <summary>
/// ֻ��������Ԫ�ӿ�
/// �ṩֻ�����ݷ��ʵĲִ�����
/// </summary>
public interface IReadOnlyUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// ���ֻ���ִ�
    /// </summary>
    IReadOnlyRepository<Advertisement> Advertisements { get; }

    /// <summary>
    /// �ֻ���ִ�
    /// </summary>
    IReadOnlyRepository<Campaign> Campaigns { get; }

    /// <summary>
    /// Ͷ�ż�¼ֻ���ִ�
    /// </summary>
    IReadOnlyRepository<DeliveryRecord> DeliveryRecords { get; }

    /// <summary>
    /// ý����Դֻ���ִ�
    /// </summary>
    IReadOnlyRepository<MediaResource> MediaResources { get; }

    /// <summary>
    /// �����ֻ���ִ�
    /// </summary>
    IReadOnlyRepository<Advertiser> Advertisers { get; }

    /// <summary>
    /// �û�����ֻ���ִ�
    /// </summary>
    IReadOnlyRepository<UserProfile> UserProfiles { get; }

    /// <summary>
    /// ��������ֻ���ִ�
    /// </summary>
    IReadOnlyRepository<TargetingConfig> TargetingConfigs { get; }

    /// <summary>
    /// �������ֻ���ִ�
    /// </summary>
    IReadOnlyRepository<TargetingPolicy> TargetingPolicies { get; }

    /// <summary>
    /// ��ȡ����ֻ���ִ�
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <returns>ֻ���ִ�</returns>
    IReadOnlyRepository<T> GetReadOnlyRepository<T>() where T : class;
}

/// <summary>
/// ������Ԫ�ӿ�
/// �ṩ���������ݷ��ʲ������������
/// </summary>
public interface IUnitOfWork : IReadOnlyUnitOfWork
{
    /// <summary>
    /// ���ִ�
    /// </summary>
    new IRepository<Advertisement> Advertisements { get; }

    /// <summary>
    /// ��ִ�
    /// </summary>
    new IRepository<Campaign> Campaigns { get; }

    /// <summary>
    /// Ͷ�ż�¼�ִ�
    /// </summary>
    new IRepository<DeliveryRecord> DeliveryRecords { get; }

    /// <summary>
    /// ý����Դ�ִ�
    /// </summary>
    new IRepository<MediaResource> MediaResources { get; }

    /// <summary>
    /// ������ִ�
    /// </summary>
    new IRepository<Advertiser> Advertisers { get; }

    /// <summary>
    /// �û�����ִ�
    /// </summary>
    new IRepository<UserProfile> UserProfiles { get; }

    /// <summary>
    /// �������òִ�
    /// </summary>
    new IRepository<TargetingConfig> TargetingConfigs { get; }

    /// <summary>
    /// ������Բִ�
    /// </summary>
    new IRepository<TargetingPolicy> TargetingPolicies { get; }

    /// <summary>
    /// �������и���
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��Ӱ��ļ�¼��</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ʼ����
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// �ύ����
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ύ����</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// �ع�����
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�ع�����</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ���Ͳִ�
    /// </summary>
    /// <typeparam name="T">ʵ������</typeparam>
    /// <returns>�ִ�</returns>
    IRepository<T> GetRepository<T>() where T : class;

    /// <summary>
    /// ����Ƿ���δ�ύ�ĸ���
    /// </summary>
    /// <returns>�Ƿ���δ�ύ�ĸ���</returns>
    bool HasChanges();

    /// <summary>
    /// ����������״̬
    /// </summary>
    void ClearContext();
}