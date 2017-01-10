using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iot
{
    /// <summary>
    /// Defines base interface for all injectable modules.
    /// </summary>
    public interface IInjectableModule
    {
        /// <summary>
        /// Performs initialization of transport implementation.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        void Open(Dictionary<string, object> args);


    }


    /// <summary>
    /// Modules which implements this interface will be invoked on Send Operation.
    /// </summary>
    public interface ISendModule : IInjectableModule
    {
        ISendModule NextSendModule { set; get; }

        /// <summary>
        /// Sends the message to remote endpoint by using of JAVA Script API style.
        /// </summary>
        /// <param name="sensorMessage">The message to be sent.</param>
        /// <param name="onSuccess">Callback function invoked after th emessage has been successfully
        /// sent to endpoint.</param>
        /// <param name="onError">Callback error function invoked if the message transfer ha failed.</param>
        /// <param name="args">Any protocol required parameters.</param>
        /// <returns>Task</returns>
        Task SendAsync(object sensorMessage,
                        Action<IList<object>> onSuccess = null,
                        Action<IList<object>, Exception> onError = null,
                        Dictionary<string, object> args = null);

        /// <summary>
        /// Sends the batch of messages to remote endpoint by using of JAVA Script API style.
        /// </summary>
        /// <param name="sensorMessages">List of messages to be sent.</param>
        /// <param name="onSuccess">Callback function invoked after th emessage has been successfully
        /// sent to endpoint.</param>
        /// <param name="onError">Callback error function invoked if the message transfer ha failed.</param>
        /// <param name="args">Any protocol required parameters.</param>
        /// <returns>Task</returns>
        Task SendAsync(IList<object> sensorMessages,
        Action<IList<object>> onSuccess = null,
                      Action<IList<object>, Exception> onError = null,
                      Dictionary<string, object> args = null);
    }


    /// <summary>
    /// Modules which implements this interface will be invoked on Receive Operation.
    /// </summary>
    public interface IReceiveModule : IInjectableModule
    {
        /// <summary>
        /// Next receive module in the pipeline.
        /// </summary>
        IReceiveModule NextReceiveModule { set; get; }

        /// <summary>
        /// Receives the message by using of JAVA Script API style.
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task ReceiveAsync(
                   Action<IList<object>> onSuccess,
                   Action<IList<object>, Exception> onError = null,
                   Dictionary<string, object> args = null);

        /// <summary>
        /// Receives the message.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<object> ReceiveAsync(Dictionary<string, object> args = null);
    }
    
    /// <summary>
    /// Modules which implements this interface will be invoked when acknowledge messages are sent.
    /// Some services and devices provide reliable messaging, which require invoking
    /// of 
    /// </summary>
    public interface IAcknowledgeModule : IInjectableModule
    {
        /// <summary>
        /// Next acknowledge module in the pipeline.
        /// </summary>
        ISendModule NextAcknowledgeModule { set; get; }


        /// <summary>
        /// Completes the message to remote endpoint by using of JAVA Script API style.
        /// </summary>
        /// <param name="sensorMessage">The message to be sent.</param>
        /// <param name="onSuccess">Callback function invoked after th emessage has been successfully
        /// sent to endpoint.</param>
        /// <param name="onError">Callback error function invoked if the message transfer ha failed.</param>
        /// <param name="args">Any protocol required parameters.</param>
        /// <returns>Task</returns>
        Task CommitAsync(object sensorMessage,
                        Action<IList<object>> onSuccess = null,
                        Action<IList<object>, Exception> onError = null,
                        Dictionary<string, object> args = null);


        /// <summary>
        /// Abandons the message to remote endpoint by using of JAVA Script API style.
        /// </summary>
        /// <param name="sensorMessage">The message to be sent.</param>
        /// <param name="onSuccess">Callback function invoked after th emessage has been successfully
        /// sent to endpoint.</param>
        /// <param name="onError">Callback error function invoked if the message transfer ha failed.</param>
        /// <param name="args">Any protocol required parameters.</param>
        /// <returns>Task</returns>
        Task AbandonAsync(object sensorMessage,
                        Action<IList<object>> onSuccess = null,
                        Action<IList<object>, Exception> onError = null,
                        Dictionary<string, object> args = null);
    }
}
