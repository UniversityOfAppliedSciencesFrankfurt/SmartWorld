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

        Task SendAsync(object sensorMessage,
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
