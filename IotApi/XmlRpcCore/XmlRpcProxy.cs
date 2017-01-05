using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace XmlRpcCore
{
    /// <summary>
    /// Provides all methods that used to make a XML-RPC connection
    /// </summary>
    public class XmlRpcProxy
    {
        /// <summary>
        /// Uri of server
        /// </summary>
        private Uri m_Uri;

        /// <summary>
        /// Duration of time out
        /// </summary>
        private TimeSpan m_TimeOut;

        /// <summary>
        /// Test object
        /// </summary>
        private bool mockTest = false;

        public XmlRpcProxy(Uri requestUri, TimeSpan setTimeOut, bool mock)
        {
            this.m_Uri = requestUri;
            this.m_TimeOut = setTimeOut;
            mockTest = mock;
        }

        /// <summary>
        /// Creates a proxy to connect server
        /// </summary>
        /// <param name="requestUri">Uri of server</param>
        /// <param name="setTimeOut">Duration of time out</param>
        public XmlRpcProxy(string requestUri, TimeSpan setTimeOut, bool mock)
        {
            this.m_Uri = new Uri(requestUri);
            this.m_TimeOut = setTimeOut;
            mockTest = mock;
        }

        /// <summary>
        /// Serializes request as Method Call object into XML-type
        /// </summary>
        /// <param name="methodName">name of method</param>
        /// <param name="methodParams">parameters of method in object type</param>
        /// <returns>serialized string</returns>
        public string SerializeRequest(string methodName, List<object> methodParams)
        {
            MethodCall request = new MethodCall(methodName, methodParams);
            return SerializeRequest(request);
        }

        /// <summary>
        /// Serializes request as Method Call object into XML-type
        /// </summary>
        /// <param name="methodName">name of method</param>
        /// <param name="methodParams">parameters of method in Param type</param>
        /// <returns>serialized string</returns>
        public string SerializeRequest(string methodName, List<Param> methodParams)
        {
            MethodCall request = new MethodCall(methodName, methodParams);
            return SerializeRequest(request);
        }

        /// <summary>
        /// Serializes request as Method Call object into XML-type
        /// </summary>
        /// <param name="request">MethodCall object</param>
        /// <returns>serialized string</returns>
        public string SerializeRequest(MethodCall request)
        {
            return XmlRpcSerialization.XmlRpcSerialize(request);
        }

        /// <summary>
        /// Sends request as serialized string to server via HTTP
        /// </summary>
        /// <param name="serializedRequest">string of serialized request</param>
        /// <returns>response string</returns>
        public async Task<string> SendRequest(string serializedRequest)
        {
            string responseString;
            responseString = await postHttp(serializedRequest);

            return responseString;
        }

        /// <summary>
        /// Sends request as Method Call object to server via HTTP
        /// </summary>
        /// <param name="request">MethodCall object as request</param>
        /// <returns>response string</returns>
        public async Task<string> SendRequest(MethodCall request)
        {
            if (m_Uri != null)
            {
                string xmlRpcString = XmlRpcSerialization.XmlRpcSerialize(request);
                string responseString;

                responseString = await this.postHttp(xmlRpcString);

                return responseString;
            }
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// Sends request to server via HTTP and deserialize string response
        /// </summary>
        /// <param name="request">MethodCall object</param>
        /// <returns>MethodResponse object</returns>
        public async Task<MethodResponse> SendRequestAndDeserialize(MethodCall request)
        {
            if (m_Uri != null)
            {

                string xmlRpcString = XmlRpcSerialization.XmlRpcSerialize(request);
                string responseString = await this.postHttp(xmlRpcString);
                MethodResponse response = XmlRpcSerialization.XmlRpcDeserialize(responseString);

                return response;
            }
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// Deserializes Response from string
        /// </summary>
        /// <param name="responseString">response string</param>
        /// <returns>MethodResponse object</returns>
        public MethodResponse DeserializeResponse(string responseString)
        {
            return XmlRpcSerialization.XmlRpcDeserialize(responseString);
        }

        /// <summary>
        /// Using HTTP to send string content
        /// </summary>
        /// <param name="xmlString">content string</param>
        /// <returns>response string</returns>
        public async Task<string> postHttp(string xmlString)
        {
            string responseString;

            if (m_Uri != null)
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = m_Uri;

                    if (m_TimeOut != new TimeSpan())
                        httpClient.Timeout = m_TimeOut;

                    HttpContent httpContent = new StringContent(xmlString);
                    
                    if (!mockTest)
                    {
                        var result = await httpClient.PostAsync(m_Uri, httpContent);
                        result.EnsureSuccessStatusCode();
                        responseString = await result.Content.ReadAsStringAsync();
                    }
                    else responseString = @"<methodResponse><params><param><value><boolean>1</boolean></value></param></params></methodResponse>";

                    return responseString;
                }
            }
            else throw new NotImplementedException();

        }
    }
}
