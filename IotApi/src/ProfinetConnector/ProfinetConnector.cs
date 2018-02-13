using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dacs7;
using Dacs7.Domain;

namespace ProfinetConnector
{
    public class ProfinetConnector : ISendModule, IReceiveModule

    {
        private Dacs7Client m_Client;
        private string m_ConnectionString;
        public IReceiveModule NextReceiveModule { get; set; }
        public ISendModule NextSendModule { get; set; }


        /// <summary>
        /// Open connection 
        /// </summary>
        /// <param name="args"></param>
        public void Open(Dictionary<string, object> args)
        {
            if (!String.IsNullOrEmpty(m_ConnectionString))
            {
                try
                {
                    m_Client.Connect(m_ConnectionString);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }


        /// <summary>
        /// Create instance of ProfinetConnector 
        /// </summary>
        /// <param name="connStr">End point connection string</param>
        public ProfinetConnector(string connStr)
        {
            this.m_ConnectionString = connStr;
            this.m_Client = new Dacs7Client();
        }


        /// <summary>
        /// Receive message from end point 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async  Task<object> ReceiveAsync(Dictionary<string, object> args)
        {
            if (args != null)
            {
                var value = args.Where(d => d.Value.GetType() == typeof(IReadMessage)).FirstOrDefault();

                if (value.Value is IReadMessage)
                {
                    var iRmgs = value.Value as IReadMessage;

                    var result = await m_Client.ReadAnyAsync(iRmgs.Area, iRmgs.Offset, iRmgs.Type, iRmgs.Arg);

                    return result;
                }
                else
                {
                    throw new Exception("");
                }
            }else
            {
                throw new Exception("Argument should not be null.");
            }

        }


        /// <summary>
        /// Receive message from end point
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task ReceiveAsync(Action<IList<object>> onSuccess, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            try
            {
                onSuccess?.Invoke(new List<object>() { await this.ReceiveAsync(args) });
            }
            catch (Exception ex)
            {
                onError?.Invoke(new List<object>() { args }, ex);
            }
        }


        /// <summary>
        /// Send messages to endpoint 
        /// </summary>
        /// <param name="sensorMessages"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            var listOnSucc = new List<object>();
            var listErr = new List<IotApiException>();

            foreach (var mgs in sensorMessages)
            {
                await this.SendAsync(mgs, (succ) =>
                {
                    listOnSucc.Add(succ);
                }, (err) =>
                {
                    listErr.Add(err);
                }, args);
            }

            onSuccess?.Invoke(listOnSucc);
            onError?.Invoke(listErr);

        }


        /// <summary>
        /// Send message to endpint 
        /// </summary>
        /// <param name="sensorMessage"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            if (sensorMessage is ISensorMessage)
            {
                var iSMgs = sensorMessage as ISensorMessage;

                //Write an array of bytes to the PLC. 
                await m_Client.WriteAnyAsync(iSMgs.Area, iSMgs.Offset, iSMgs.Value, iSMgs.Args);
            }
            else
            {
                onError?.Invoke(new ProfinetException($"You have to send '{nameof(ISensorMessage)}'."));
            }
        }
    }
}
