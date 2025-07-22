namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 圆形地理围栏定向条件
    /// 实现基于圆形区域的精确地理围栏定向规则配置
    /// 适用场景：商圈定向、POI周边定向、事件现场定向等需要精确圆形范围的营销活动
    /// 特点：定向精度高，计算效率优，适合大规模实时竞价场景
    /// </summary>
    public class CircularGeoFenceTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "CircularGeoFence";

        /// <summary>
        /// 围栏中心点纬度
        /// 取值范围：-90.0 到 90.0
        /// </summary>
        public decimal Latitude => GetRule<decimal>("Latitude");

        /// <summary>
        /// 围栏中心点经度
        /// 取值范围：-180.0 到 180.0
        /// </summary>
        public decimal Longitude => GetRule<decimal>("Longitude");

        /// <summary>
        /// 围栏半径（米）
        /// 取值范围：1 到 100,000 米（1米到100公里）
        /// </summary>
        public int RadiusMeters => GetRule<int>("RadiusMeters");

        /// <summary>
        /// 围栏业务名称
        /// 用于标识围栏的业务含义，如"万达广场商圈"、"机场周边"等
        /// </summary>
        public string? Name => GetRule<string?>("Name");

        /// <summary>
        /// 围栏类别
        /// 用于分类管理不同类型的圆形围栏
        /// </summary>
        public GeoFenceCategory Category => GetRule<GeoFenceCategory>("Category", GeoFenceCategory.Commercial);

        /// <summary>
        /// 缓冲区距离（米）
        /// 在围栏边界外增加的缓冲区域，用于处理GPS精度误差
        /// </summary>
        public int? BufferMeters => GetRule<int?>("BufferMeters");

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="latitude">中心点纬度</param>
        /// <param name="longitude">中心点经度</param>
        /// <param name="radiusMeters">半径（米）</param>
        /// <param name="name">围栏名称</param>
        /// <param name="category">围栏类别</param>
        /// <param name="bufferMeters">缓冲区距离（米）</param>
        /// <param name="weight">权重</param>
        /// <param name="isEnabled">是否启用</param>
        public CircularGeoFenceTargeting(
            decimal latitude,
            decimal longitude,
            int radiusMeters,
            string? name = null,
            GeoFenceCategory category = GeoFenceCategory.Commercial,
            int? bufferMeters = null,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(latitude, longitude, radiusMeters, name, category, bufferMeters), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则字典
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            decimal latitude,
            decimal longitude,
            int radiusMeters,
            string? name,
            GeoFenceCategory category,
            int? bufferMeters)
        {
            ValidateInput(latitude, longitude, radiusMeters, bufferMeters);

            var rules = new List<TargetingRule>();

            // 添加纬度
            var latitudeRule = new TargetingRule("Latitude", string.Empty, "Decimal").WithValue(latitude);
            rules.Add(latitudeRule);

            // 添加经度
            var longitudeRule = new TargetingRule("Longitude", string.Empty, "Decimal").WithValue(longitude);
            rules.Add(longitudeRule);

            // 添加半径
            var radiusRule = new TargetingRule("RadiusMeters", string.Empty, "Int32").WithValue(radiusMeters);
            rules.Add(radiusRule);

            // 添加分类
            var categoryRule = new TargetingRule("Category", string.Empty, "Enum").WithValue(category);
            rules.Add(categoryRule);

            // 添加名称（如果存在）
            if (!string.IsNullOrEmpty(name))
            {
                var nameRule = new TargetingRule("Name", string.Empty, "String").WithValue(name);
                rules.Add(nameRule);
            }

            // 添加缓冲区（如果存在）
            if (bufferMeters.HasValue)
            {
                var bufferRule = new TargetingRule("BufferMeters", string.Empty, "Int32").WithValue(bufferMeters.Value);
                rules.Add(bufferRule);
            }

            return rules;
        }

        /// <summary>
        /// 创建圆形地理围栏定向条件
        /// </summary>
        public static CircularGeoFenceTargeting Create(
            decimal latitude,
            decimal longitude,
            int radiusMeters,
            string? name = null,
            GeoFenceCategory category = GeoFenceCategory.Commercial,
            int? bufferMeters = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new CircularGeoFenceTargeting(latitude, longitude, radiusMeters, name, category, bufferMeters, weight, isEnabled);
        }

        /// <summary>
        /// 基于POI（兴趣点）创建圆形围栏
        /// </summary>
        /// <param name="poiLatitude">POI纬度</param>
        /// <param name="poiLongitude">POI经度</param>
        /// <param name="radiusMeters">围栏半径</param>
        /// <param name="poiName">POI名称</param>
        /// <param name="category">围栏类别</param>
        /// <returns>圆形地理围栏实例</returns>
        public static CircularGeoFenceTargeting CreateAroundPOI(
            decimal poiLatitude,
            decimal poiLongitude,
            int radiusMeters,
            string poiName,
            GeoFenceCategory category = GeoFenceCategory.Commercial)
        {
            return Create(poiLatitude, poiLongitude, radiusMeters, $"{poiName}周边{radiusMeters}米", category, radiusMeters / 10);
        }

        /// <summary>
        /// 设置围栏名称
        /// </summary>
        /// <param name="name">围栏名称</param>
        public void SetName(string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                SetRule("Name", name);
            }
            else
            {
                RemoveRule("Name");
            }
        }

        /// <summary>
        /// 设置围栏类别
        /// </summary>
        /// <param name="category">围栏类别</param>
        public void SetCategory(GeoFenceCategory category)
        {
            SetRule("Category", category);
        }

        /// <summary>
        /// 设置缓冲区距离
        /// </summary>
        /// <param name="bufferMeters">缓冲区距离（米）</param>
        public void SetBufferMeters(int? bufferMeters)
        {
            if (bufferMeters.HasValue)
            {
                if (bufferMeters.Value < 0 || bufferMeters.Value > RadiusMeters)
                    throw new ArgumentException("缓冲区距离必须在0到围栏半径之间", nameof(bufferMeters));

                SetRule("BufferMeters", bufferMeters.Value);
            }
            else
            {
                RemoveRule("BufferMeters");
            }
        }

        /// <summary>
        /// 更新围栏位置和半径
        /// </summary>
        /// <param name="latitude">新纬度</param>
        /// <param name="longitude">新经度</param>
        /// <param name="radiusMeters">新半径</param>
        public void UpdateLocation(decimal latitude, decimal longitude, int radiusMeters)
        {
            ValidateInput(latitude, longitude, radiusMeters, BufferMeters);

            SetRule("Latitude", latitude);
            SetRule("Longitude", longitude);
            SetRule("RadiusMeters", radiusMeters);
        }

        /// <summary>
        /// 检查点是否在围栏内（含缓冲区）
        /// </summary>
        /// <param name="latitude">检查点纬度</param>
        /// <param name="longitude">检查点经度</param>
        /// <returns>是否在围栏内</returns>
        public bool Contains(decimal latitude, decimal longitude)
        {
            var distance = CalculateDistance(Latitude, Longitude, latitude, longitude);
            var effectiveRadius = RadiusMeters + (BufferMeters ?? 0);
            return distance <= effectiveRadius;
        }

        /// <summary>
        /// 检查点是否在围栏内（含缓冲区）
        /// </summary>
        /// <param name="point">地理坐标点</param>
        /// <returns>是否在围栏内</returns>
        public bool Contains(GeoPoint point)
        {
            return Contains(point.Latitude, point.Longitude);
        }

        /// <summary>
        /// 检查点是否在核心围栏内（不含缓冲区）
        /// </summary>
        /// <param name="latitude">检查点纬度</param>
        /// <param name="longitude">检查点经度</param>
        /// <returns>是否在核心围栏内</returns>
        public bool IsInCoreArea(decimal latitude, decimal longitude)
        {
            var distance = CalculateDistance(Latitude, Longitude, latitude, longitude);
            return distance <= RadiusMeters;
        }

        /// <summary>
        /// 检查点是否在缓冲区内（但不在核心围栏内）
        /// </summary>
        /// <param name="latitude">检查点纬度</param>
        /// <param name="longitude">检查点经度</param>
        /// <returns>是否在缓冲区内</returns>
        public bool IsInBufferZone(decimal latitude, decimal longitude)
        {
            if (!BufferMeters.HasValue)
                return false;

            var distance = CalculateDistance(Latitude, Longitude, latitude, longitude);
            return distance > RadiusMeters && distance <= (RadiusMeters + BufferMeters.Value);
        }

        /// <summary>
        /// 计算到某点的距离（米）
        /// </summary>
        /// <param name="latitude">目标点纬度</param>
        /// <param name="longitude">目标点经度</param>
        /// <returns>距离（米）</returns>
        public double CalculateDistanceTo(decimal latitude, decimal longitude)
        {
            return CalculateDistance(Latitude, Longitude, latitude, longitude);
        }

        /// <summary>
        /// 计算到某点的距离（米）
        /// </summary>
        /// <param name="point">目标地理坐标点</param>
        /// <returns>距离（米）</returns>
        public double CalculateDistanceTo(GeoPoint point)
        {
            return CalculateDistanceTo(point.Latitude, point.Longitude);
        }

        /// <summary>
        /// 获取围栏的覆盖面积（平方米）
        /// </summary>
        /// <returns>面积（平方米）</returns>
        public double GetCoverageArea()
        {
            var effectiveRadius = RadiusMeters + (BufferMeters ?? 0);
            return Math.PI * effectiveRadius * effectiveRadius;
        }

        /// <summary>
        /// 获取围栏中心点
        /// </summary>
        /// <returns>中心点坐标</returns>
        public GeoPoint GetCenter()
        {
            return GeoPoint.Create(Latitude, Longitude);
        }

        /// <summary>
        /// 检查是否与另一个圆形围栏重叠
        /// </summary>
        /// <param name="other">另一个圆形围栏</param>
        /// <returns>是否重叠</returns>
        public bool OverlapsWith(CircularGeoFenceTargeting other)
        {
            if (other == null)
                return false;

            var distance = CalculateDistance(Latitude, Longitude, other.Latitude, other.Longitude);
            var thisEffectiveRadius = RadiusMeters + (BufferMeters ?? 0);
            var otherEffectiveRadius = other.RadiusMeters + (other.BufferMeters ?? 0);

            return distance < (thisEffectiveRadius + otherEffectiveRadius);
        }

        /// <summary>
        /// 验证圆形地理围栏特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            try
            {
                // 验证纬度
                if (Latitude < -90 || Latitude > 90)
                    return false;

                // 验证经度
                if (Longitude < -180 || Longitude > 180)
                    return false;

                // 验证半径
                if (RadiusMeters <= 0 || RadiusMeters > 100000)
                    return false;

                // 验证缓冲区
                if (BufferMeters.HasValue && (BufferMeters.Value < 0 || BufferMeters.Value > RadiusMeters))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var area = GetCoverageArea() / 1000000; // 转换为平方公里
            var details = $"Type: Circular, Center: ({Latitude:F6}, {Longitude:F6}), Radius: {RadiusMeters}m, Area: {area:F2}km²";

            if (BufferMeters.HasValue)
                details += $", Buffer: {BufferMeters}m";

            if (!string.IsNullOrEmpty(Name))
                details += $", Name: {Name}";

            details += $", Category: {Category}";

            return $"{summary} - {details}";
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInput(decimal latitude, decimal longitude, int radiusMeters, int? bufferMeters)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("纬度必须在-90到90之间", nameof(latitude));

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("经度必须在-180到180之间", nameof(longitude));

            if (radiusMeters <= 0 || radiusMeters > 100000)
                throw new ArgumentException("半径必须在1到100,000米之间", nameof(radiusMeters));

            if (bufferMeters.HasValue && (bufferMeters.Value < 0 || bufferMeters.Value > radiusMeters))
                throw new ArgumentException("缓冲区距离必须在0到围栏半径之间", nameof(bufferMeters));
        }

        /// <summary>
        /// 计算两点间距离（米）
        /// 使用Haversine公式计算球面距离
        /// </summary>
        private static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double earthRadius = 6371000; // 地球半径（米）

            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadius * c;
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }

    /// <summary>
    /// 地理围栏类别枚举
    /// </summary>
    public enum GeoFenceCategory
    {
        /// <summary>
        /// 商业区域
        /// </summary>
        Commercial = 1,

        /// <summary>
        /// 住宅区域
        /// </summary>
        Residential = 2,

        /// <summary>
        /// 交通枢纽
        /// </summary>
        Transportation = 3,

        /// <summary>
        /// 教育机构
        /// </summary>
        Educational = 4,

        /// <summary>
        /// 医疗机构
        /// </summary>
        Medical = 5,

        /// <summary>
        /// 娱乐场所
        /// </summary>
        Entertainment = 6,

        /// <summary>
        /// 办公区域
        /// </summary>
        Office = 7,

        /// <summary>
        /// 工业区域
        /// </summary>
        Industrial = 8,

        /// <summary>
        /// 旅游景点
        /// </summary>
        Tourism = 9,

        /// <summary>
        /// 体育场馆
        /// </summary>
        Sports = 10,

        /// <summary>
        /// 政府机构
        /// </summary>
        Government = 11,

        /// <summary>
        /// 自定义区域
        /// </summary>
        Custom = 99
    }
}
