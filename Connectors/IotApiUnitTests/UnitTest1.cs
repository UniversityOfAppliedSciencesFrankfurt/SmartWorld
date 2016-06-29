using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daenet.Iot;
using Daenet.IoTApi;

namespace IotApiUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        public IIotApi getConnector()
        {
            return new SampleConnector();

        }
        [TestMethod]
        public void TestInitialization()
        {
        //    IotApi api = new IotApi(getConnector());

        //    go(1, testMethod);
       }

        private void testMethod(int a)
        {

        }

        public void go(dynamic a, dynamic b)
        {

        }
    }
}
