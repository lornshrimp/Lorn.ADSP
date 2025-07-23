using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lorn.ADSP.Core.Domain.Common;

namespace Lorn.ADSP.Core.Domain.ValueObjects
{
    /// <summary>
    /// 年龄范围
    /// </summary>
    public class AgeRange : ValueObject
    {
        /// <summary>
        /// 最小年龄
        /// </summary>
        public int MinAge { get; private set; }

        /// <summary>
        /// 最大年龄
        /// </summary>
        public int MaxAge { get; private set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private AgeRange()
        {
            MinAge = 0;
            MaxAge = 0;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AgeRange(int minAge, int maxAge)
        {
            ValidateInput(minAge, maxAge);

            MinAge = minAge;
            MaxAge = maxAge;
        }

        /// <summary>
        /// 创建年龄范围
        /// </summary>
        public static AgeRange Create(int minAge, int maxAge)
        {
            return new AgeRange(minAge, maxAge);
        }

        /// <summary>
        /// 创建青少年年龄范围
        /// </summary>
        public static AgeRange CreateTeenager()
        {
            return new AgeRange(13, 19);
        }

        /// <summary>
        /// 创建成年人年龄范围
        /// </summary>
        public static AgeRange CreateAdult()
        {
            return new AgeRange(18, 65);
        }

        /// <summary>
        /// 检查年龄是否在范围内
        /// </summary>
        public bool Contains(int age)
        {
            return age >= MinAge && age <= MaxAge;
        }

        /// <summary>
        /// 扩展年龄范围
        /// </summary>
        public AgeRange Extend(int minExtension, int maxExtension)
        {
            return new AgeRange(
                Math.Max(0, MinAge - minExtension),
                MaxAge + maxExtension);
        }

        /// <summary>
        /// 获取年龄跨度
        /// </summary>
        public int Span => MaxAge - MinAge + 1;

        /// <summary>
        /// 获取等价性比较的组件
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MinAge;
            yield return MaxAge;
        }

        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static void ValidateInput(int minAge, int maxAge)
        {
            if (minAge < 0)
                throw new ArgumentException("最小年龄不能为负数", nameof(minAge));

            if (maxAge < 0)
                throw new ArgumentException("最大年龄不能为负数", nameof(maxAge));

            if (minAge > maxAge)
                throw new ArgumentException("最小年龄不能大于最大年龄");
        }
    }
}