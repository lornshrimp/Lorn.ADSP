namespace Lorn.ADSP.Core.Shared.Constants;

/// <summary>
/// 广告尺寸常量
/// </summary>
public static class AdSizeConstants
{
    /// <summary>
    /// 横幅广告标准尺寸
    /// </summary>
    public static class Banner
    {
        /// <summary>
        /// 320x50 - 移动横幅
        /// </summary>
        public static readonly (int Width, int Height) MobileBanner = (320, 50);

        /// <summary>
        /// 728x90 - 排行榜广告位
        /// </summary>
        public static readonly (int Width, int Height) Leaderboard = (728, 90);

        /// <summary>
        /// 300x250 - 中等矩形
        /// </summary>
        public static readonly (int Width, int Height) MediumRectangle = (300, 250);

        /// <summary>
        /// 336x280 - 大矩形
        /// </summary>
        public static readonly (int Width, int Height) LargeRectangle = (336, 280);

        /// <summary>
        /// 160x600 - 宽摩天大楼
        /// </summary>
        public static readonly (int Width, int Height) WideSkyscraper = (160, 600);

        /// <summary>
        /// 120x600 - 摩天大楼
        /// </summary>
        public static readonly (int Width, int Height) Skyscraper = (120, 600);
    }

    /// <summary>
    /// 视频广告标准尺寸
    /// </summary>
    public static class Video
    {
        /// <summary>
        /// 640x480 - 标清
        /// </summary>
        public static readonly (int Width, int Height) SD = (640, 480);

        /// <summary>
        /// 1280x720 - 高清
        /// </summary>
        public static readonly (int Width, int Height) HD = (1280, 720);

        /// <summary>
        /// 1920x1080 - 全高清
        /// </summary>
        public static readonly (int Width, int Height) FullHD = (1920, 1080);

        /// <summary>
        /// 3840x2160 - 4K超高清
        /// </summary>
        public static readonly (int Width, int Height) UHD = (3840, 2160);
    }

    /// <summary>
    /// 原生广告标准尺寸
    /// </summary>
    public static class Native
    {
        /// <summary>
        /// 1200x627 - 社交媒体封面
        /// </summary>
        public static readonly (int Width, int Height) SocialCover = (1200, 627);

        /// <summary>
        /// 1080x1080 - 正方形
        /// </summary>
        public static readonly (int Width, int Height) Square = (1080, 1080);

        /// <summary>
        /// 1080x1350 - 纵向
        /// </summary>
        public static readonly (int Width, int Height) Portrait = (1080, 1350);
    }
}