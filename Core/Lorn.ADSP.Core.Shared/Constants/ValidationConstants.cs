namespace Lorn.ADSP.Core.Shared.Constants;

/// <summary>
/// 验证规则常量
/// </summary>
public static class ValidationConstants
{
    /// <summary>
    /// 字符串长度限制
    /// </summary>
    public static class StringLength
    {
        /// <summary>
        /// 广告ID最大长度
        /// </summary>
        public const int AdIdMaxLength = 50;

        /// <summary>
        /// 用户ID最大长度
        /// </summary>
        public const int UserIdMaxLength = 100;

        /// <summary>
        /// 域名最大长度
        /// </summary>
        public const int DomainMaxLength = 255;

        /// <summary>
        /// 关键词最大长度
        /// </summary>
        public const int KeywordsMaxLength = 1000;

        /// <summary>
        /// 广告标题最大长度
        /// </summary>
        public const int AdTitleMaxLength = 100;

        /// <summary>
        /// 广告描述最大长度
        /// </summary>
        public const int AdDescriptionMaxLength = 500;

        /// <summary>
        /// URL最大长度
        /// </summary>
        public const int UrlMaxLength = 2048;
    }

    /// <summary>
    /// 数值范围限制
    /// </summary>
    public static class NumberRange
    {
        /// <summary>
        /// 最小出价价格 (分)
        /// </summary>
        public const decimal MinBidPrice = 0.01m;

        /// <summary>
        /// 最大出价价格 (元)
        /// </summary>
        public const decimal MaxBidPrice = 10000.00m;

        /// <summary>
        /// 最小预算金额 (元)
        /// </summary>
        public const decimal MinBudget = 1.00m;

        /// <summary>
        /// 最大预算金额 (元)
        /// </summary>
        public const decimal MaxBudget = 1000000.00m;

        /// <summary>
        /// 最小年龄
        /// </summary>
        public const int MinAge = 13;

        /// <summary>
        /// 最大年龄
        /// </summary>
        public const int MaxAge = 100;

        /// <summary>
        /// 最小广告宽度
        /// </summary>
        public const int MinAdWidth = 50;

        /// <summary>
        /// 最大广告宽度
        /// </summary>
        public const int MaxAdWidth = 4096;

        /// <summary>
        /// 最小广告高度
        /// </summary>
        public const int MinAdHeight = 50;

        /// <summary>
        /// 最大广告高度
        /// </summary>
        public const int MaxAdHeight = 4096;
    }

    /// <summary>
    /// 集合大小限制
    /// </summary>
    public static class CollectionSize
    {
        /// <summary>
        /// 单次请求最大广告位数量
        /// </summary>
        public const int MaxImpressionsPerRequest = 10;

        /// <summary>
        /// 最大关键词数量
        /// </summary>
        public const int MaxKeywordsCount = 100;

        /// <summary>
        /// 最大域名数量
        /// </summary>
        public const int MaxDomainsCount = 50;

        /// <summary>
        /// 最大分类数量
        /// </summary>
        public const int MaxCategoriesCount = 20;

        /// <summary>
        /// 最大属性数量
        /// </summary>
        public const int MaxAttributesCount = 50;
    }

    /// <summary>
    /// 正则表达式模式
    /// </summary>
    public static class RegexPatterns
    {
        /// <summary>
        /// UUID格式验证
        /// </summary>
        public const string UuidPattern = @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";

        /// <summary>
        /// 域名格式验证
        /// </summary>
        public const string DomainPattern = @"^[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?(\.[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?)*$";

        /// <summary>
        /// 邮箱格式验证
        /// </summary>
        public const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        /// <summary>
        /// IP地址格式验证
        /// </summary>
        public const string IpAddressPattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        /// <summary>
        /// URL格式验证
        /// </summary>
        public const string UrlPattern = @"^https?://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,}(/.*)?$";
    }
}