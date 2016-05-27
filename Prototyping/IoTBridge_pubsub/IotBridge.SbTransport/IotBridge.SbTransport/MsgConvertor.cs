using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotBridge.SbTransport
{
    internal static class MsgConvertor
    {
        public static Message ToMessage(this BrokeredMessage sbMsg)
        {
            //TODO. Impl. conversion.
            string text = sbMsg.GetBody<string>();
            return new Message(text);
        }

        public static BrokeredMessage FromMessage(this Message msg)
        {
            //TODO. Impl. conversion.
            var text = msg.GetBody<object>();
            return new BrokeredMessage(text);
        }
    }
}
