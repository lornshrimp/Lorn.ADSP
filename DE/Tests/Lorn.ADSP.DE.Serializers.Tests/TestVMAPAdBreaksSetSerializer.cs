using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lorn.ADSP.DE.Serializers;
using Lorn.ADSP.DE.Interfaces;

namespace Lorn.ADSP.DE.Serializers.Tests
{
    /// <summary>
    /// TestVMAPAdBreaksSerializer 的摘要说明
    /// </summary>
    [TestClass]
    public class TestVMAPAdBreaksSetSerializer
    {
        public TestVMAPAdBreaksSetSerializer()
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

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", @"VMAPAdBreaksSetSerializerTests.csv", "VMAPAdBreaksSetSerializerTests#csv", DataAccessMethod.Sequential), TestMethod()]
        public void TestVMAPAdBreaksSetSerializerSerializeAdBreaksSet()
        {
            ISerializeAdBreaksSet vmapSerializer = new VMAPAdBreaksSetSerializer();
            var timeOffsets = testContextInstance.DataRow["TimeOffsets"].ToString().Split(new string[] { "|" }, StringSplitOptions.None);
            var positionTypes = testContextInstance.DataRow["PositionTypes"].ToString().Split(new string[] { "|" }, StringSplitOptions.None);
            var positionIds = testContextInstance.DataRow["PositionIds"].ToString().Split(new string[] { "|" }, StringSplitOptions.None);
            var vastVersions = testContextInstance.DataRow["VASTVersions"].ToString().Split(new string[] { "|" }, StringSplitOptions.None);
            var inputDatas = testContextInstance.DataRow["InputDatas"].ToString().Split(new string[] { "|" }, StringSplitOptions.None);
            var events = testContextInstance.DataRow["events"].ToString().Split(new string[] { "|" }, StringSplitOptions.None);

            var extensionStrings = testContextInstance.DataRow["Extensions"].ToString().Split(new string[] { "|" }, StringSplitOptions.None);

            var adBreaks = new Dictionary<DataModels.AdPositionReleaseInfo, string>();
            for (int i = 0; i < timeOffsets.Length; i++)
            {
                var timeOffset = timeOffsets[i];
                var positionType = positionTypes[i];
                var positionId = positionIds[i];
                var vastVersion = vastVersions[i];
                var inputData = inputDatas[i];
                var trackingEventStrings = events[i].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var trackingEvents = new Dictionary<string, string>();
                foreach (var trackingEventString in trackingEventStrings)
                {
                    var keyValuePair = trackingEventString.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    trackingEvents[keyValuePair[0]] = keyValuePair[1];
                }

                adBreaks.Add(new DataModels.AdPositionReleaseInfo() { TimeOffset = timeOffset, PositionType = (Lorn.ADSP.Common.DataModels.AdPositionType)int.Parse(positionType), AdPositionId = new Guid(positionId), VASTVersion = float.Parse(vastVersion), TrackingEvents = trackingEvents }, inputData);
            }

            var extensions = new Dictionary<string, string>();
            foreach (var extensionString in extensionStrings)
            {
                var keyValuePair = extensionString.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                extensions[keyValuePair[0]] = keyValuePair[1];
            }

            var output = vmapSerializer.SerializeAdBreaksSet(adBreaks, extensions);
            Assert.AreEqual(testContextInstance.DataRow["Output"], output);

        }
    }
}
