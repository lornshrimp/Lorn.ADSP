using Lorn.ADSP.Infrastructure.Data.Abstractions.Enums;
using Lorn.ADSP.Infrastructure.Data.Abstractions.Interfaces;

namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 批量操作定义
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public record BatchOperationDefinition<T> where T : class
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public required BatchOperationType OperationType { get; init; }

        /// <summary>
        /// 数据访问上下文
        /// </summary>
        public required DataAccessContext Context { get; init; }

        /// <summary>
        /// 数据项（用于写操作）
        /// </summary>
        public IEnumerable<T>? Items { get; init; }

        /// <summary>
        /// 查询规格（用于查询操作）
        /// </summary>
        public IQuerySpecification<T>? QuerySpecification { get; init; }

        /// <summary>
        /// 操作选项
        /// </summary>
        public Dictionary<string, object> Options { get; init; } = new();
    }
}
