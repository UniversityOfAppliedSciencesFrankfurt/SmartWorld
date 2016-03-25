using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        /// <param name="onReceiveMsg">The method which is invoked when the message is received.
        /// Return value shoul d be set on true if message needs to be acknowledged.</param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        Task OnMessage(Func<object, bool> onReceiveMsg, CancellationToken cancelationToken, Dictionary<string, object> args = null);

        /// <summary>
        /// Receievs the message.
        /// </summary>
        /// <param name="onSuccess">On success delegate method (promise).</param>
        /// <param name="onError">>On error delegate method (promise).</param>
        /// <param name="autoComplete">True if the message should be completed automatically.
        /// In that case it is completted as successful after successful invoke of onSuccess callback function or it 
        /// is completed as failed if onSuccess callback failes.
        /// If this is set on NULL, <see cref="AcknowledgeReceivedMessage" /> should explicitelly be called.
        /// If some APIs do not support message completion, this parameter can be ignored.</param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        /// <returns>List of messages.</returns>
        Task ReceiveAsync(Func<object, bool> onSuccess = null,
            Func<Exception, bool> onError = null,
            int timeout = 60000,
            Dictionary<string, object> args = null);

        /// <summary>
        /// After the message is receieved the business code, 
        /// SHOULD/MUST/CAN (depends on concrete implementation) invoke this method to acknowledge receiving of the message. 
        /// If the transport does not support acknowledge of the message, then implementation should 
        /// return without any action.
        /// </summary>
        /// <param name="msg">Message, which should be acknowledged.</param>
        /// <param name="error">If no error ocurred this MUST BE null. If no error ocurred, message will be acknowledged (completed)
        /// as successful.
        /// Otherwise this argument is instance of exception which has ocurred. In that case message is acknowledged as failed (abandoned)
        /// and can be resent again. 
        /// Exact behavior depends on the concrete implementation.</param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        //Task AcknowledgeReceivedMessageAsync(object msg, Exception error = null, Dictionary<string, object> args = null);


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
