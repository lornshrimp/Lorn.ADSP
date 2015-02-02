using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lorn.ADSP.DE.DataModels;
using Lorn.ADSP.DE.Interfaces;
using Lorn.ADSP.DE.AdFilters;
using System.Collections.Generic;
using System.Linq;

namespace Lorn.ADSP.DE.AdFilters.Tests
{
    /// <summary>
    /// AdLocationAdFilterTests 的摘要说明
    /// </summary>
    [TestClass]
    public class TestAdPositionSizeAdFilter
    {
        public TestAdPositionSizeAdFilter()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", @"AdPositionSizeAdFilterTests.csv", "AdPositionSizeAdFilterTests#csv", DataAccessMethod.Sequential), TestMethod()]
        public void TestAdPositionSizeAdFilterFilterAd()
        {
            IFilterAd filter = new AdPositionSizeAdFilter();
            var reqSizeId = testContextInstance.DataRow["ReqPositionSizeId"].ToString();
            var reqPositionId = Guid.NewGuid();
            var parameters = new Dictionary<string, string>();
            parameters["PositionId"] = reqPositionId.ToString();
            var Ads = testContextInstance.DataRow["Ads"].ToString().Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var ads = new List<Ad>();
            foreach (var item in Ads)
            {
                var ids = item.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                ads.Add(new AdSpotPlan() { AdSpotPlanId = new Guid(ids[0]), AdPositionSizeId = new Guid(ids[1]) });
            }
            AdPosition position = new AdPosition() { AdPositionId = reqPositionId, AdPositionSizeId = new Guid(reqSizeId) };
            var additionalParameters = new Dictionary<string, object>();
            additionalParameters["AdPosition"] = position;
            var outputs = TestContext.DataRow["Outputs"].ToString().Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var actual = filter.FilterAd(null, parameters, ads, null, null, null, additionalParameters);
            Assert.AreEqual(outputs.Length, actual.Count);
            foreach (var item in actual)
            {
                Assert.IsNotNull(outputs.FirstOrDefault(o => new Guid(o) == item.AdSpotPlanId));
            }
        }
    }
}
