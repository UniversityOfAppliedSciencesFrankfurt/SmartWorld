using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iot
{
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
        IReceiveModule NextReceiveModule { set; get; }

        Task<object> ReceiveAsync(
                   Action<IList<object>> onSuccess = null,
                   Action<IList<object>, Exception> onError = null,
                   Dictionary<string, object> args = null);
    }
}
