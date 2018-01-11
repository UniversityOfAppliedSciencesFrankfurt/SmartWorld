using Daenet.Azure.Devices.Gateway;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StartUpModule
{
    public class StartUp : IGatewayModule, IGatewayModuleStart
    {
        private Broker m_Brocker;
        private string m_Config;
        private int a = 1;
        private Task m_Task;
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
            //Runs the Ble Module receiver on a new thread
            m_Task = new Task(new Action(this.bleReceiverLoop));

            m_Task.Start();

            m_Task.ContinueWith((r) =>
            {
                var i = r;
            });
            //Dictionary<string, string> property = new Dictionary<string, string>();
            //property.Add("name", "daenet");

            //int a = 1;
            //while (true)
            //{
            //    Console.WriteLine("Enter message");
            //    var mgs = Console.ReadLine();
            //    m_Brocker.Publish(new Message($"{mgs}, Message id: {a++}", property));

            //}
            //int a = 10;
            //while (a > 0)
            //{
            //    bleReceiverLoop();
            //    a--;
            //}
        }

        private void bleReceiverLoop()
        {
            Dictionary<string, string> property = new Dictionary<string, string>();
            property.Add("name", "daenet");
            Console.WriteLine("Star up module");
                m_Brocker.Publish(new Message($"{DateTime.Now}, Message id: {a++}", property));
        }
    }
}
