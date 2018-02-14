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
                var value = args.Where(d => d.Value is IReadMessage).FirstOrDefault();

                if (value.Value is IReadMessage)
                {
                    var iRmgs = propertiesValidate(value.Value) as IReadMessage;

                    var result = await m_Client.ReadAnyAsync(iRmgs.Area, iRmgs.Offset, iRmgs.Type, iRmgs.Args);

                    return result;
                }
                else
                {
                    throw new Exception("You have to give 'IReadMessage' as value of 'args'");
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
                var iSMgs = propertiesValidate(sensorMessage)  as ISensorMessage;

                //Write an array of bytes to the PLC. 
                await m_Client.WriteAnyAsync(iSMgs.Area, iSMgs.Offset, iSMgs.Value, iSMgs.Args);
            }
            else
            {
                onError?.Invoke(new ProfinetException($"You have to send '{nameof(ISensorMessage)}'."));
            }
        }


        /// <summary>
        /// Validate properties of ISensorMessage and IReadMessage
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private object propertiesValidate(object obj)
        {
            if(obj is ISensorMessage)
            {
                var send = obj as ISensorMessage;

                if (send.Area == 0)
                    throw new ProfinetException("ISensorMessage.Area should not be null");
                if (send.Offset == 0)
                    throw new ProfinetException("ISensorMessage.Offset should not be null");
                if(send.Value == null)
                    throw new ProfinetException("ISensorMessage.Value should not be null");
                if (send.Args.Count() <= 0)
                    throw new ProfinetException("ISensorMessage.Args should not be null");
                
                return send;

            }
            else if(obj is IReadMessage)
            {
                var read = obj as IReadMessage;

                if (read.Area == 0)
                    throw new ProfinetException("IReadMessage.Area should not be null");
                if (read.Offset == 0)
                    throw new ProfinetException("IReadMessage.Offset should not be null");
                if (read.Type == null)
                    throw new ProfinetException("IReadMessage.Type should not be null");
                if (read.Args.Count() <= 0)
                    throw new ProfinetException("IReadMessage.Args should not be null");
                
                return read;
            }
            
            return null;
        }
    }
}
