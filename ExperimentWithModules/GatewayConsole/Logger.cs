using Daenet.IotGateway.Common.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayConsole
{
    class Logger : IGatewayLogger
    {
        public void Log<TState>(string moduleName, LogLevel logLevel, int eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

        }
    }
}
