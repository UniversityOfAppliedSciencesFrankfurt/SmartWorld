using Iot;
using XmlRpcCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmlRpcUnitTests
{
    [TestClass]
    public class UnitTest
    {
        /// <summary>
        /// Check door status 
        /// </summary>
        [TestMethod]
        public void DoorStatus()
        {
            var api = getApi();
            var result = api.SendAsync(new MethodCall()
            {
                MethodName = "getValue",
                SendParams = new System.Collections.Generic.List<Param>() {
                    new Param()
                    {
                        Value = "LEQ1335713:1"
                    },
                    new Param()
                    {
                        Value = "STATE"
                    }
                }
            }).Result;
        }

        /// <summary>
        /// Open the door
        /// </summary>
        [TestMethod]
        public void OpenDoor()
        {
            var api = getApi();
            var result = api.SendAsync(new MethodCall()
            {
                MethodName = "setValue",
                SendParams = new System.Collections.Generic.List<Param>() {
                    new Param()
                    {
                        Value = "LEQ1335713:1"
                    },
                    new Param()
                    {
                        Value = "STATE"
                    },
                    new Param()
                    {
                        Value = true
                    }
                }
            }).Result;    
        }

        /// <summary>
        /// Close the door 
        /// </summary>
        [TestMethod]
        public void CloseDoor()
        {
            var api = getApi();
            var result = api.SendAsync(new MethodCall()
            {
                MethodName = "setValue",
                SendParams = new System.Collections.Generic.List<Param>() {
                    new Param()
                    {
                        Value = "LEQ1335713:1"
                    },
                    new Param()
                    {
                        Value = "STATE"
                    },
                    new Param()
                    {
                        Value = false
                    }
                }
            }).Result;
        }

        /// <summary>
        /// Get api
        /// </summary>
        /// <returns></returns>
        private IotApi getApi()
        {
            var api = new IotApi()
                .UseXmlRpc("http://192.168.0.222:2001");
            api.Open();

            return api;
        }
    }
}
