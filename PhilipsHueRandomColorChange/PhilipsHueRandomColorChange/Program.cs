using Iot;
using PhilipsHueConnector;
using PhilipsHueConnector.Entities;
using System;
using System.Threading.Tasks;

namespace PhilipsHueRandomColorChange
{
    class Program
    {
        private static string m_GtwUri;// = "http://192.168.0.99";
        private static string m_UserName;// = "gusp-xLeBhYznPCkz0ZQBnuZ25f3cOwRpW3tiQ8k";
        private static string m_DeviceId;// = "4";

        static void Main(string[] args)
        {
            try
            {

                setGtwUriUserNameAndDeviceId();
                randomColorChange();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void randomColorChange()
        {
            Console.WriteLine("Initializing IotApi and send to endpoint.");

            IotApi iotApi = new IotApi();

            iotApi.UsePhilpsQueueRest(m_GtwUri, m_UserName);

            iotApi.Open();

            while (true)
            {
                var result = iotApi.SendAsync(new SetLightStates()
                {
                    Id = m_DeviceId,

                    Body = new State()
                    {
                        on = true,
                        bri = new Random().Next(120, 253),
                        hue = new Random().Next(1, 65534)
                    },

                }).Result;

                Task.Delay(3000).Wait();
            }
        }

        private static void setGtwUriUserNameAndDeviceId()
        {
            Console.WriteLine("Initializing environment variables");

            if (environmentContains("GTWURI"))
            {
                m_GtwUri = getEnvValue<string>("GTWURI");
            }
            else
            {
                throw new ArgumentException("Error when receiving desired property: GTWURI");
            }

            if (environmentContains("USERNAME"))
            {
                m_UserName = getEnvValue<string>("USERNAME");
            }
            else
            {
                throw new ArgumentException("Error when receiving desired property: USERNAME");
            }

            if (environmentContains("DEVICEID"))
            {
                m_DeviceId = getEnvValue<string>("DEVICEID");
            }
            else
            {
                throw new ArgumentException("Error when receiving desired property: DeviceId");
            }
        }

        private static T getEnvValue<T>(string propertyName)
        {
            var value = Environment.GetEnvironmentVariable(propertyName);
            Console.WriteLine($"Property: {propertyName}, Value: {value}");
            return (T)Convert.ChangeType(value, typeof(T));
        }

        private static bool environmentContains(string key)
        {
            return Environment.GetEnvironmentVariables().Contains(key);
        }
    }
}

