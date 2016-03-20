using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Iot
{
    /// <summary>
    /// Interface which defines operation which IoT transports have to implement.
    /// </summary>
    public interface IIotApi
    {
        /// <summary>
        /// Performs initialization of transport implementation.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task Open(Dictionary<string, object> args);

        /// <summary>
        /// Gets the name of transport library.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Invoked by transport when a message is received.
        /// </summary>
        /// <param name="onReceiveMsg">The method which is invoked when the message is received.</param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        void OnMessage(Action<object> onReceiveMsg, Dictionary<string, object> args = null);

        /// <summary>
        /// Receievs the message.
        /// </summary>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        /// <returns></returns>
        object Receive(Dictionary<string, object> args = null);

        /// <summary>
        /// After the message is receieved the business code, whic huses this transport,
        /// SHOULD/MUST (depends on concrete implementation) invoke this method to commit receiving of the message. 
        /// If the transport does not support commit of the message, then implementation should 
        /// return without any action.
        /// </summary>
        /// <param name="msgId">Identifier of the message, which should be acknowledged.</param>
        /// <param name="error">If no error ocurred this MUST BE null. If no error ocurred, message can be commited.
        /// Otherwise this argument is instance of exception which has ocurred. In that case message is abandoned and can be
        /// resent again. Exact behavior depends on the concrete implementation.</param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        void AcknowledgeReceivedMessage(string msgId, Exception error = null, Dictionary<string, object> args = null);


        void RegisterAcknowledge(Action<string, Exception> onAcknowledgeReceived);


        /// <summary>
        /// Sends the message to service or device. 
        /// </summary>
        /// <param name="sensorMessage">The message to be sent.</param>
        /// <param name="onSuccess">On success delegate method (promise).</param>
        /// <param name="onError">>On error delegate method (promise).</param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        /// <returns>Task</returns>
        /// <remarks>
        /// Note, the exact behavior of sending of the
        /// message is defined by concrete implementation.
        /// </remarks>

        Task SendAsync(object sensorMessage,
                                  Action<IList<object>> onSuccess = null,
                                  Action<IList<object>, Exception> onError = null,
                                  Dictionary<string, object> args = null);



        /// <summary>
        /// Sends the message to service or device. 
        /// </summary>
        /// <param name="sensorMessages">Batch pof messages.</param>
        /// <param name="onSuccess">On success delegate method (promise).</param>
        /// <param name="onError">>On error delegate method (promise).</param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        /// <returns>Task</returns>
        /// <remarks>
        /// Note, the exact behavior of sending of the
        /// message is defined by concrete implementation.
        /// </remarks>
        Task SendAsync(IList<object> sensorMessages,
           Action<IList<object>> onSuccess = null,
           Action<IList<object>, Exception> onError = null,
           Dictionary<string, object> args = null);
    }
}
