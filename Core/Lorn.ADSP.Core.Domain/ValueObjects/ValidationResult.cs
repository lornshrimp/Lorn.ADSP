namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResult
    {
        private readonly List<string> _errors = new();
        private readonly List<string> _warnings = new();

        public bool IsValid => !_errors.Any();
        public IReadOnlyList<string> Errors => _errors.AsReadOnly();
        public IReadOnlyList<string> Warnings => _warnings.AsReadOnly();

        public void AddError(string error)
        {
            _errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            _warnings.Add(warning);
        }
    }
}
