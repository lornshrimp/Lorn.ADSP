using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lorn.ADSP.DE.Serializers;
using Lorn.ADSP.DE.Interfaces;
using System.Collections.Generic;

namespace Lorn.ADSP.DE.Serializers.Tests
{
    [TestClass]
    public class TestNonSerializeRequestedInformationSerializer
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

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", @"NonSerializeRequestedInformationSerializerTests.csv", "NonSerializeRequestedInformationSerializerTests#csv", DataAccessMethod.Sequential), TestMethod()]
        public void TestNonSerializeRequestedInformationSerializerSerialzeRequestedServiceInformations()
        {
            ISerializeRequestedServiceInformations serializer = new NonSerializeRequestedInformationSerializer();
            var inputs = testContextInstance.DataRow["Inputs"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var inputDictory = new Dictionary<string, string>();
            foreach (var item in inputs)
            {
                var dic = item.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                inputDictory[dic[0]] = dic[1];
            }
            var outputs = testContextInstance.DataRow["Outputs "].ToString();
            Assert.AreEqual(outputs, serializer.SerialzeRequestedServiceInformations(inputDictory));

        }
    }
}
