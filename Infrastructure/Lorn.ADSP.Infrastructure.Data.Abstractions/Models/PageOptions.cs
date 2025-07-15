namespace Lorn.ADSP.Infrastructure.Data.Abstractions.Models
{
    /// <summary>
    /// 分页选项
    /// </summary>
    public record PageOptions
    {
        /// <summary>
        /// 页索引（从0开始）
        /// </summary>
        public int PageIndex { get; init; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// 最大页大小限制
        /// </summary>
        public static int MaxPageSize { get; } = 1000;

        /// <summary>
        /// 验证分页选项
        /// </summary>
        /// <returns>验证结果</returns>
        public ValidationResult Validate()
        {
            var errors = new List<string>();

            if (PageIndex < 0)
            {
                errors.Add("Page index cannot be negative");
            }

            if (PageSize <= 0)
            {
                errors.Add("Page size must be greater than 0");
            }

            if (PageSize > MaxPageSize)
            {
                errors.Add($"Page size cannot exceed {MaxPageSize}");
            }

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors);
        }
    }
}
