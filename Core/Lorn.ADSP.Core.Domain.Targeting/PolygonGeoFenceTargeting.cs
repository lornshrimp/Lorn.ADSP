using Lorn.ADSP.Core.Domain.Targeting;

namespace Lorn.ADSP.Core.Domain.ValueObjects.Targeting
{
    /// <summary>
    /// 多边形地理围栏定向条件
    /// 实现基于复杂多边形区域的精确地理围栏定向规则配置
    /// 适用场景：复杂地形区域定向、不规则商业区域、行政边界精确匹配、特殊形状区域定向
    /// 特点：定向精度最高，支持复杂形状，适合对定向精度要求极高的高价值广告投放
    /// </summary>
    public class PolygonGeoFenceTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public override string CriteriaName => "多边形地理围栏定向";

        /// <summary>
        /// 条件类型标识
        /// </summary>
        public override string CriteriaType => "PolygonGeoFence";

        /// <summary>
        /// 多边形顶点坐标集合（纬度、经度坐标对）
        /// 顶点按顺序连接形成闭合多边形，最少需要3个顶点
        /// </summary>
        public IReadOnlyList<GeoPoint> Points => GetRule<List<GeoPoint>>("Points") ?? new List<GeoPoint>();

        /// <summary>
        /// 围栏业务名称
        /// 用于标识围栏的业务含义，如"CBD核心区域"、"工业园区"等
        /// </summary>
        public string? Name => GetRule<string?>("Name");

        /// <summary>
        /// 围栏类别
        /// 用于分类管理不同类型的多边形围栏
        /// </summary>
        public GeoFenceCategory Category => GetRule<GeoFenceCategory>("Category", GeoFenceCategory.Custom);

        /// <summary>
        /// 缓冲区距离（米）
        /// 在多边形边界外增加的缓冲区域，用于处理GPS精度误差
        /// </summary>
        public int? BufferMeters => GetRule<int?>("BufferMeters");

        /// <summary>
        /// 多边形复杂度等级
        /// 影响点包含判断的算法选择和性能优化策略
        /// </summary>
        public PolygonComplexity Complexity => GetRule<PolygonComplexity>("Complexity", PolygonComplexity.Medium);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="points">多边形顶点</param>
        /// <param name="name">围栏名称</param>
        /// <param name="category">围栏类别</param>
        /// <param name="bufferMeters">缓冲区距离（米）</param>
        /// <param name="weight">权重</param>
        /// <param name="isEnabled">是否启用</param>
        public PolygonGeoFenceTargeting(
            IReadOnlyList<GeoPoint> points,
            string? name = null,
            GeoFenceCategory category = GeoFenceCategory.Custom,
            int? bufferMeters = null,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(points, name, category, bufferMeters), weight, isEnabled)
        {
        }

        /// <summary>
        /// 创建规则字典
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IReadOnlyList<GeoPoint> points,
            string? name,
            GeoFenceCategory category,
            int? bufferMeters)
        {
            ValidateInput(points, bufferMeters);

            var rules = new List<TargetingRule>();

            // 添加点列表
            var pointsList = points.ToList();
            var pointsRule = new TargetingRule("Points", string.Empty, "Json").WithValue(pointsList);
            rules.Add(pointsRule);

            // 添加分类
            var categoryRule = new TargetingRule("Category", string.Empty, "Enum").WithValue(category);
            rules.Add(categoryRule);

            // 添加复杂性
            var complexity = DetermineComplexity(points);
            var complexityRule = new TargetingRule("Complexity", string.Empty, "Enum").WithValue(complexity);
            rules.Add(complexityRule);

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
        /// 创建多边形地理围栏定向条件
        /// </summary>
        public static PolygonGeoFenceTargeting Create(
            IReadOnlyList<GeoPoint> points,
            string? name = null,
            GeoFenceCategory category = GeoFenceCategory.Custom,
            int? bufferMeters = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new PolygonGeoFenceTargeting(points, name, category, bufferMeters, weight, isEnabled);
        }

        /// <summary>
        /// 基于矩形区域创建多边形围栏
        /// </summary>
        /// <param name="minLatitude">最小纬度</param>
        /// <param name="minLongitude">最小经度</param>
        /// <param name="maxLatitude">最大纬度</param>
        /// <param name="maxLongitude">最大经度</param>
        /// <param name="name">围栏名称</param>
        /// <param name="category">围栏类别</param>
        /// <returns>矩形多边形围栏实例</returns>
        public static PolygonGeoFenceTargeting CreateRectangle(
            decimal minLatitude, decimal minLongitude,
            decimal maxLatitude, decimal maxLongitude,
            string? name = null,
            GeoFenceCategory category = GeoFenceCategory.Custom)
        {
            var points = new List<GeoPoint>
            {
                GeoPoint.Create(minLatitude, minLongitude),
                GeoPoint.Create(minLatitude, maxLongitude),
                GeoPoint.Create(maxLatitude, maxLongitude),
                GeoPoint.Create(maxLatitude, minLongitude)
            };

            return Create(points, name ?? "矩形区域", category);
        }

        /// <summary>
        /// 基于现有圆形围栏创建多边形围栏（圆形逼近）
        /// </summary>
        /// <param name="center">圆心</param>
        /// <param name="radiusMeters">半径（米）</param>
        /// <param name="segments">分段数（多边形边数）</param>
        /// <param name="name">围栏名称</param>
        /// <param name="category">围栏类别</param>
        /// <returns>多边形围栏实例</returns>
        public static PolygonGeoFenceTargeting CreateFromCircle(
            GeoPoint center, int radiusMeters, int segments = 12,
            string? name = null, GeoFenceCategory category = GeoFenceCategory.Custom)
        {
            if (segments < 3)
                throw new ArgumentException("分段数必须至少为3", nameof(segments));

            var points = new List<GeoPoint>();
            var earthRadius = 6371000.0; // 地球半径（米）

            for (int i = 0; i < segments; i++)
            {
                var angle = 2 * Math.PI * i / segments;
                var deltaLat = radiusMeters * Math.Cos(angle) / earthRadius * (180 / Math.PI);
                var deltaLon = radiusMeters * Math.Sin(angle) / (earthRadius * Math.Cos((double)center.Latitude * Math.PI / 180)) * (180 / Math.PI);

                var lat = center.Latitude + (decimal)deltaLat;
                var lon = center.Longitude + (decimal)deltaLon;

                points.Add(GeoPoint.Create(lat, lon));
            }

            return Create(points, name ?? $"圆形逼近{segments}边形", category);
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
                if (bufferMeters.Value < 0 || bufferMeters.Value > 10000)
                    throw new ArgumentException("缓冲区距离必须在0到10,000米之间", nameof(bufferMeters));

                SetRule("BufferMeters", bufferMeters.Value);
            }
            else
            {
                RemoveRule("BufferMeters");
            }
        }

        /// <summary>
        /// 更新多边形顶点
        /// </summary>
        /// <param name="points">新的顶点列表</param>
        public void UpdatePoints(IReadOnlyList<GeoPoint> points)
        {
            ValidateInput(points, BufferMeters);
            SetRule("Points", points.ToList());
            SetRule("Complexity", DetermineComplexity(points));
        }

        /// <summary>
        /// 添加顶点
        /// </summary>
        /// <param name="point">新顶点</param>
        public void AddPoint(GeoPoint point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            var currentPoints = Points.ToList();
            if (currentPoints.Count >= 1000)
                throw new InvalidOperationException("多边形顶点数量不能超过1000个");

            currentPoints.Add(point);
            SetRule("Points", currentPoints);
            SetRule("Complexity", DetermineComplexity(currentPoints));
        }

        /// <summary>
        /// 插入顶点到指定位置
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="point">新顶点</param>
        public void InsertPoint(int index, GeoPoint point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            var currentPoints = Points.ToList();
            if (index < 0 || index > currentPoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (currentPoints.Count >= 1000)
                throw new InvalidOperationException("多边形顶点数量不能超过1000个");

            currentPoints.Insert(index, point);
            SetRule("Points", currentPoints);
            SetRule("Complexity", DetermineComplexity(currentPoints));
        }

        /// <summary>
        /// 移除顶点
        /// </summary>
        /// <param name="index">顶点索引</param>
        public void RemovePoint(int index)
        {
            var currentPoints = Points.ToList();
            if (index < 0 || index >= currentPoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (currentPoints.Count <= 3)
                throw new InvalidOperationException("多边形至少需要3个顶点，无法继续移除");

            currentPoints.RemoveAt(index);
            SetRule("Points", currentPoints);
            SetRule("Complexity", DetermineComplexity(currentPoints));
        }

        /// <summary>
        /// 检查点是否在多边形内（使用射线法，含缓冲区）
        /// </summary>
        /// <param name="latitude">检查点纬度</param>
        /// <param name="longitude">检查点经度</param>
        /// <returns>是否在围栏内</returns>
        public bool Contains(decimal latitude, decimal longitude)
        {
            if (Points.Count < 3)
                return false;

            // 如果有缓冲区，需要检查点到多边形边界的距离
            if (BufferMeters.HasValue && BufferMeters.Value > 0)
            {
                return IsWithinPolygonBuffer(latitude, longitude, BufferMeters.Value);
            }

            // 标准点在多边形内判断（射线法）
            return IsPointInPolygon(latitude, longitude);
        }

        /// <summary>
        /// 检查点是否在多边形内（含缓冲区）
        /// </summary>
        /// <param name="point">地理坐标点</param>
        /// <returns>是否在围栏内</returns>
        public bool Contains(GeoPoint point)
        {
            return Contains(point.Latitude, point.Longitude);
        }

        /// <summary>
        /// 检查点是否在核心多边形内（不含缓冲区）
        /// </summary>
        /// <param name="latitude">检查点纬度</param>
        /// <param name="longitude">检查点经度</param>
        /// <returns>是否在核心多边形内</returns>
        public bool IsInCoreArea(decimal latitude, decimal longitude)
        {
            return IsPointInPolygon(latitude, longitude);
        }

        /// <summary>
        /// 检查点是否在缓冲区内（但不在核心多边形内）
        /// </summary>
        /// <param name="latitude">检查点纬度</param>
        /// <param name="longitude">检查点经度</param>
        /// <returns>是否在缓冲区内</returns>
        public bool IsInBufferZone(decimal latitude, decimal longitude)
        {
            if (!BufferMeters.HasValue || BufferMeters.Value <= 0)
                return false;

            return !IsPointInPolygon(latitude, longitude) &&
                   IsWithinPolygonBuffer(latitude, longitude, BufferMeters.Value);
        }

        /// <summary>
        /// 计算多边形的边界框
        /// </summary>
        /// <returns>边界框（最小纬度、最小经度、最大纬度、最大经度）</returns>
        public (decimal MinLat, decimal MinLon, decimal MaxLat, decimal MaxLon) GetBoundingBox()
        {
            if (!Points.Any())
                return (0, 0, 0, 0);

            var minLat = Points.Min(p => p.Latitude);
            var maxLat = Points.Max(p => p.Latitude);
            var minLon = Points.Min(p => p.Longitude);
            var maxLon = Points.Max(p => p.Longitude);

            return (minLat, minLon, maxLat, maxLon);
        }

        /// <summary>
        /// 计算多边形的几何中心点（重心）
        /// </summary>
        /// <returns>中心点坐标</returns>
        public GeoPoint GetCentroid()
        {
            if (!Points.Any())
                return GeoPoint.Create(0, 0);

            var avgLat = Points.Average(p => p.Latitude);
            var avgLon = Points.Average(p => p.Longitude);

            return GeoPoint.Create(avgLat, avgLon);
        }

        /// <summary>
        /// 计算多边形周长（米，近似值）
        /// </summary>
        /// <returns>周长（米）</returns>
        public double CalculatePerimeter()
        {
            if (Points.Count < 3)
                return 0;

            double perimeter = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                var current = Points[i];
                var next = Points[(i + 1) % Points.Count];
                perimeter += CalculateDistance(current.Latitude, current.Longitude, next.Latitude, next.Longitude);
            }

            return perimeter;
        }

        /// <summary>
        /// 计算多边形面积（平方米，近似值）
        /// </summary>
        /// <returns>面积（平方米）</returns>
        public double CalculateApproximateArea()
        {
            if (Points.Count < 3)
                return 0;

            // 使用Shoelace公式计算多边形面积（球面坐标简化计算）
            const double earthRadius = 6371000; // 地球半径（米）

            double area = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                var j = (i + 1) % Points.Count;
                var pi = Points[i];
                var pj = Points[j];

                // 转换为弧度
                var lat1 = (double)pi.Latitude * Math.PI / 180;
                var lon1 = (double)pi.Longitude * Math.PI / 180;
                var lat2 = (double)pj.Latitude * Math.PI / 180;
                var lon2 = (double)pj.Longitude * Math.PI / 180;

                area += (lon2 - lon1) * (2 + Math.Sin(lat1) + Math.Sin(lat2));
            }

            area = Math.Abs(area) * earthRadius * earthRadius / 2;
            return area;
        }

        /// <summary>
        /// 获取多边形的外接圆半径（近似值）
        /// </summary>
        /// <returns>外接圆半径（米）</returns>
        public double GetCircumscribedRadius()
        {
            var centroid = GetCentroid();
            double maxDistance = 0;

            foreach (var point in Points)
            {
                var distance = CalculateDistance(centroid.Latitude, centroid.Longitude, point.Latitude, point.Longitude);
                maxDistance = Math.Max(maxDistance, distance);
            }

            return maxDistance;
        }

        /// <summary>
        /// 检查是否与另一个多边形围栏重叠（边界框快速检查）
        /// </summary>
        /// <param name="other">另一个多边形围栏</param>
        /// <returns>是否可能重叠</returns>
        public bool MayOverlapWith(PolygonGeoFenceTargeting other)
        {
            if (other == null)
                return false;

            var thisBounds = GetBoundingBox();
            var otherBounds = other.GetBoundingBox();

            return thisBounds.MinLat <= otherBounds.MaxLat &&
                   thisBounds.MaxLat >= otherBounds.MinLat &&
                   thisBounds.MinLon <= otherBounds.MaxLon &&
                   thisBounds.MaxLon >= otherBounds.MinLon;
        }

        /// <summary>
        /// 简化多边形（减少顶点数量）
        /// </summary>
        /// <param name="tolerance">简化容差（米）</param>
        /// <returns>简化后的多边形</returns>
        public PolygonGeoFenceTargeting Simplify(double tolerance = 10.0)
        {
            if (Points.Count <= 3)
                return this;

            var simplifiedPoints = SimplifyPolygon(Points.ToList(), tolerance);
            return Create(simplifiedPoints, Name, Category, BufferMeters, Weight, IsEnabled);
        }

        /// <summary>
        /// 验证多边形地理围栏特定规则的有效性
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            try
            {
                // 验证至少有3个顶点
                if (Points.Count < 3)
                    return false;

                // 验证顶点数量不能过多
                if (Points.Count > 1000)
                    return false;

                // 验证所有顶点坐标的有效性
                foreach (var point in Points)
                {
                    if (point == null)
                        return false;

                    if (point.Latitude < -90 || point.Latitude > 90)
                        return false;

                    if (point.Longitude < -180 || point.Longitude > 180)
                        return false;
                }

                // 验证缓冲区距离
                if (BufferMeters.HasValue && (BufferMeters.Value < 0 || BufferMeters.Value > 10000))
                    return false;

                // 验证多边形不能自相交（简单检查）
                if (HasObviousSelfIntersection())
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
            var centroid = GetCentroid();
            var area = CalculateApproximateArea();
            var perimeter = CalculatePerimeter();

            var details = $"Type: Polygon, Points: {Points.Count}, Centroid: ({centroid.Latitude:F6}, {centroid.Longitude:F6}), " +
                         $"Area: ~{area / 1000000:F2}km², Perimeter: ~{perimeter / 1000:F2}km, Complexity: {Complexity}";

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
        private static void ValidateInput(IReadOnlyList<GeoPoint> points, int? bufferMeters)
        {
            if (points == null || points.Count < 3)
                throw new ArgumentException("多边形至少需要3个顶点", nameof(points));

            if (points.Count > 1000)
                throw new ArgumentException("多边形顶点数量不能超过1000个", nameof(points));

            foreach (var point in points)
            {
                if (point == null)
                    throw new ArgumentException("顶点不能为null", nameof(points));
            }

            if (bufferMeters.HasValue && (bufferMeters.Value < 0 || bufferMeters.Value > 10000))
                throw new ArgumentException("缓冲区距离必须在0到10,000米之间", nameof(bufferMeters));
        }

        /// <summary>
        /// 确定多边形复杂度
        /// </summary>
        private static PolygonComplexity DetermineComplexity(IReadOnlyList<GeoPoint> points)
        {
            var count = points.Count;
            return count switch
            {
                <= 10 => PolygonComplexity.Low,
                <= 50 => PolygonComplexity.Medium,
                <= 200 => PolygonComplexity.High,
                _ => PolygonComplexity.VeryHigh
            };
        }

        /// <summary>
        /// 使用射线法判断点是否在多边形内
        /// </summary>
        private bool IsPointInPolygon(decimal latitude, decimal longitude)
        {
            var intersections = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                var j = (i + 1) % Points.Count;
                var pi = Points[i];
                var pj = Points[j];

                if (((pi.Latitude > latitude) != (pj.Latitude > latitude)) &&
                    (longitude < (pj.Longitude - pi.Longitude) * (latitude - pi.Latitude) / (pj.Latitude - pi.Latitude) + pi.Longitude))
                {
                    intersections++;
                }
            }

            return intersections % 2 == 1;
        }

        /// <summary>
        /// 检查点是否在多边形的缓冲区内
        /// </summary>
        private bool IsWithinPolygonBuffer(decimal latitude, decimal longitude, int bufferMeters)
        {
            // 先检查是否在多边形内
            if (IsPointInPolygon(latitude, longitude))
                return true;

            // 计算点到多边形边界的最短距离
            var minDistance = GetMinDistanceToPolygonEdges(latitude, longitude);
            return minDistance <= bufferMeters;
        }

        /// <summary>
        /// 计算点到多边形边界的最短距离
        /// </summary>
        private double GetMinDistanceToPolygonEdges(decimal latitude, decimal longitude)
        {
            double minDistance = double.MaxValue;

            for (int i = 0; i < Points.Count; i++)
            {
                var j = (i + 1) % Points.Count;
                var distance = GetDistanceToLineSegment(latitude, longitude, Points[i], Points[j]);
                minDistance = Math.Min(minDistance, distance);
            }

            return minDistance;
        }

        /// <summary>
        /// 计算点到线段的最短距离
        /// </summary>
        private static double GetDistanceToLineSegment(decimal pointLat, decimal pointLon, GeoPoint lineStart, GeoPoint lineEnd)
        {
            var A = (double)pointLat - (double)lineStart.Latitude;
            var B = (double)pointLon - (double)lineStart.Longitude;
            var C = (double)lineEnd.Latitude - (double)lineStart.Latitude;
            var D = (double)lineEnd.Longitude - (double)lineStart.Longitude;

            var dot = A * C + B * D;
            var lenSq = C * C + D * D;

            if (lenSq == 0)
            {
                // 线段退化为点
                return CalculateDistance((decimal)lineStart.Latitude, lineStart.Longitude, pointLat, pointLon);
            }

            var param = dot / lenSq;

            decimal xx, yy;
            if (param < 0)
            {
                xx = lineStart.Latitude;
                yy = lineStart.Longitude;
            }
            else if (param > 1)
            {
                xx = lineEnd.Latitude;
                yy = lineEnd.Longitude;
            }
            else
            {
                xx = lineStart.Latitude + (decimal)(param * C);
                yy = lineStart.Longitude + (decimal)(param * D);
            }

            return CalculateDistance(pointLat, pointLon, xx, yy);
        }

        /// <summary>
        /// 检查多边形是否有明显的自相交
        /// </summary>
        private bool HasObviousSelfIntersection()
        {
            // 简单检查：如果边界框的对角线长度远小于周长，可能有自相交
            var bounds = GetBoundingBox();
            var diagonalLength = CalculateDistance(bounds.MinLat, bounds.MinLon, bounds.MaxLat, bounds.MaxLon);
            var perimeter = CalculatePerimeter();

            return perimeter > diagonalLength * 10; // 经验阈值
        }

        /// <summary>
        /// 使用Douglas-Peucker算法简化多边形
        /// </summary>
        private static List<GeoPoint> SimplifyPolygon(List<GeoPoint> points, double tolerance)
        {
            if (points.Count <= 3)
                return points;

            // 实现Douglas-Peucker算法的简化版本
            var simplified = new List<GeoPoint> { points[0] };

            for (int i = 1; i < points.Count - 1; i++)
            {
                var distance = GetDistanceToLineSegment(points[i].Latitude, points[i].Longitude, points[i - 1], points[i + 1]);
                if (distance > tolerance)
                {
                    simplified.Add(points[i]);
                }
            }

            simplified.Add(points[^1]);
            return simplified;
        }

        /// <summary>
        /// 计算两点间距离（米）
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
    /// 多边形复杂度枚举
    /// </summary>
    public enum PolygonComplexity
    {
        /// <summary>
        /// 低复杂度（3-10个顶点）
        /// </summary>
        Low = 1,

        /// <summary>
        /// 中等复杂度（11-50个顶点）
        /// </summary>
        Medium = 2,

        /// <summary>
        /// 高复杂度（51-200个顶点）
        /// </summary>
        High = 3,

        /// <summary>
        /// 极高复杂度（200+个顶点）
        /// </summary>
        VeryHigh = 4
    }
}
