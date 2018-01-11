using Daenet.Azure.Devices.Gateway;
using System;
using System.Text;

namespace PrintMdule
{
    public class PrintModule : IGatewayModule
    {
        private Broker m_Brocker;
        private string m_Config;
        public void Create(Broker broker, byte[] configuration)
        {
            this.m_Brocker = broker;
            this.m_Config = Encoding.UTF8.GetString(configuration);
        }

        public void Destroy()
        {
            
        }

        public void Receive(Message received_message)
        {
                Console.WriteLine($"Print Module: {Encoding.UTF8.GetString(received_message.Content)}");
        }
    }
}
