using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotBridge
{
    /// <summary>
    /// Interface which defines operation which IoT transports have to implement.
    /// </summary>
    public interface IBridgeTransport
    {
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
        void OnMessage(Action<Message> onReceiveMsg, Dictionary<string, object> args = null);

        /// <summary>
        /// Receievs the message.
        /// </summary>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        /// <returns></returns>
        Message Receive(Dictionary<string, object> args = null);

        /// <summary>
        /// After the message is receieved the business code, whic h uses this transport,
        /// MUST invoke this method to commit receiving of the message. 
        /// If the transport does not support commit of the message, then implementation should 
        /// return without any action.
        /// </summary>
        /// <param name="msgId">Identifier of the message, which should be commited.</param>
        /// <param name="error">If no error ocurred this MUST BE null.
        /// Otherwise this argument is instance of exception which has ocurred.</param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        void SendReceiveAckonwledgeResult(string msgId, Exception error, Dictionary<string, object> args = null);

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="msg">Message which will be sent.</param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        void Send(Message msg, Dictionary<string, object> args = null);

        /// <summary>
        /// After sending of the message by using <see cref="Send"/> method, the transport will invoke
        /// this method to notify business code about result of sending of the message.
        /// If the transport does not support reliable messaging it should invoke this method without providing an exception.
        /// First argument of type <see cref="string"/> specifyies the message identifier (used for correlation) and
        /// second argument describes the error if some ocurred. If nmo error ocurred, second argument is null.
        /// </summary>
        /// <param name="onMsgSendResult"></param>
        /// <param name="args">List of arguments which can be internally used by transports.
        /// Because transports will use different argumens
        /// this parameter provides a generic dictionary of arguments.</param>
        void OnSendAcknowledgeResult(Action<string, Exception> onMsgSendResult, Dictionary<string, object> args = null);
    }
}
