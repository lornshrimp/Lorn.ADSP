using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lorn.ADSP.DE.DataModels;
using Lorn.ADSP.DE.Interfaces;
using Lorn.ADSP.DE.AdFilters;
using System.Collections.Generic;
using System.Linq;

namespace Lorn.ADSP.DE.AdFilters.Tests
{
    [TestClass]
    public class TestAdPositionAdFilter
    {
        private TestContext testContextInstance;
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

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", @"AdPositionAdFilterTests.csv", "AdPositionAdFilterTests#csv", DataAccessMethod.Sequential), TestMethod()]
        public void TestAdPositionAdFilterFilterAd()
        {
            IFilterAd filter = new AdPositionAdFilter();
            var reqPositionId = testContextInstance.DataRow["ReqPositionId"].ToString();
            var parameters = new Dictionary<string, string>();
            AdPosition position = new AdPosition() { AdPositionId = new Guid(reqPositionId) };
            var additionalParameters = new Dictionary<string, object>();
            additionalParameters["AdPosition"] = position;
            var Ads = testContextInstance.DataRow["Ads"].ToString().Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var ads = new List<Ad>();
            foreach (var item in Ads)
            {
                var ids = item.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                ads.Add(new AdSpotPlan() {AdSpotPlanId = new Guid(ids[0]), AdPositionId = new Guid(ids[1]) });
            }
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
