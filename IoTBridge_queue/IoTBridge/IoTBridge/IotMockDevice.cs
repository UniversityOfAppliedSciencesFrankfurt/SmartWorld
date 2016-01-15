using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotBridge
{
    /// <summary>
    /// Mock device which supports specific protocol.This device is very useful for testing of protocols.
    /// SCENARIO 'Send Command to device': You can send commands to device.
    /// For every received command, device will trace out the message id. If you want to trace out some other 
    /// part of the message please override method <see cref="ReadMessage"/>.
    /// SCENARIO 'Send Telemetry Data'.
    /// If you want to enable device for sending of telemetry data you can send a message with property 'Start'
    /// of type int. This property will set in millisecopnds how often the device shoud send telemetry data.
    /// If you send a message with property 'Stop' (any type and any value) device will stop sending of telemetry data.
    /// </summary>
    public class IotMockDevice
    {
        private IBridgeTransport m_Transport;
        private Dictionary<string, object> m_RcvAckArgs;
        private Dictionary<string, object> m_OnMsgRcvArg;
        private Dictionary<string, object> m_SndAckArgs;
        private Dictionary<string, object> m_SndArgs;
        private Action<string> m_TraceMethod;

        private bool m_IsStopRequested = false;

        private Task m_TelemetryTask;

        private string m_Id = "MockDevice" + new Guid().ToString();

        public IotMockDevice(IBridgeTransport deviceTransport,
            Dictionary<string, object> rcvAckArgs,
            Dictionary<string, object> onMsgRcvArgs,
            Dictionary<string, object> sndArgs,
            Dictionary<string, object> sndAckArgs,
            Action<string> traceMethod)
        {
            if (m_Transport == null)
                throw new ArgumentNullException();

            m_Transport = deviceTransport;
            m_TraceMethod = traceMethod;
            m_OnMsgRcvArg = onMsgRcvArgs;
            m_RcvAckArgs = rcvAckArgs;

            m_SndAckArgs = sndAckArgs;
            m_SndAckArgs = sndAckArgs;
        }

        public Task RunAsync()
        {
            Task t = new Task(() =>
            {

                m_Transport.OnMessage((msg) =>
                {
                    m_TraceMethod(String.Format("Message: {0}", msg.MessageId));

                    m_Transport.SendReceiveAckonwledgeResult(msg.MessageId, null, m_RcvAckArgs);

                    if (msg.Properties.ContainsKey("Start"))
                    {
                        startTelemetryLoop(msg);
                    }
                    else if (msg.Properties.ContainsKey("Stop"))
                    {
                        stopTelemetryLoop();
                    }

                }, m_OnMsgRcvArg);
            });

            t.Start();

            return t;
        }

        private void stopTelemetryLoop()
        {
            if (m_TelemetryTask != null)
            {
                m_IsStopRequested = true;

                m_TelemetryTask.Wait();

                m_TelemetryTask = null;
            }
        }

        private void startTelemetryLoop(Message msg)
        {
            m_TelemetryTask = new Task(() =>
            {
                m_Transport.OnSendAcknowledgeResult((msgId, err) =>
                {
                    m_TraceMethod(String.Format("Ackowledged send of telemetry data for MessageId: {0}", msgId));
                });

                while (m_IsStopRequested == false)
                {
                    Task.Delay((int)msg.Properties["Start"]);

                    TelemetryData data = new TelemetryData()
                    {
                        Device = m_Id,
                        Temperature = DateTime.Now.Minute,
                    };

                    Message telemetryData = new Message(data);

                    m_Transport.Send(telemetryData, m_SndAckArgs);
                }

                m_IsStopRequested = false;
            });

            m_TelemetryTask.Start();
        }

        /// <summary>
        /// You can override this method if you want to trace out any other part of the message.
        /// By defaulot this method returns message identifier.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual string ReadMessage(Message msg)
        {
            return msg.MessageId;
        }
    }

    public class TelemetryData
    {
        public double Temperature { get; set; }

        public string Device { get; set; }
    }
}
