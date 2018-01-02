using Daenet.Azure.Devices.Gateway;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartUpModule
{
    public class StartUp : IGatewayModule, IGatewayModuleStart
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
            
        }

        public void Start()
        {
            Dictionary<string, string> property = new Dictionary<string, string>();
            property.Add("name", "daenet");
           
            int a = 1;
            while (true)
            {
                Console.WriteLine("Enter message");
                var mgs = Console.ReadLine();
                m_Brocker.Publish(new Message($"{mgs}, Message id: {a++}", property));
                
            }
        }
    }
}
