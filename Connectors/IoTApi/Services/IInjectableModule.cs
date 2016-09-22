using Daenet.Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.IoT.Services
{
    public interface IInjectableModule
    {
        IInjectableModule NextModule { set; get; }

        //Task Open(IIotApi connector, IInjectableModule nextModule, Dictionary<string, object> args = null);

       Task SendAsync(object sensorMessage,
       Action<IList<object>> onSuccess = null,
       Action<IList<object>, Exception> onError = null,
       Dictionary<string, object> args = null);
    }




    public interface IBeforeReceive : IInjectableModule
    {
        bool BeforeReceive(IIotApi connector, object sensorMessage, Dictionary<string, object> args = null);
    }

    public interface IAfterReceive : IInjectableModule
    {
        bool AfterReceive(IIotApi connector, object sensorMessage, Dictionary<string, object> args = null);
    }
}
