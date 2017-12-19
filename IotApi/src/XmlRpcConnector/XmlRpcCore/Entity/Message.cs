using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlRpcCore
{
    /// <summary>
    /// Define properties of message transported inside the bridge
    /// </summary>
    public class Message
    {
        // readonly MessagingClient client;
        private object Body { get; set; }
        private Stream BodyStream { get; set; }
        private Type BodyType { get; set; }
        private string ContentType { get; set; }
        public string MessageId { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public long Size { get; internal set; }
        public TimeSpan TimeToLive { get; set; }

        public Message()
        {
            initializeMessage();
        }
        public Message(object serializableObject)
        {
            initializeMessage();
            this.Body = serializableObject;
            this.ContentType = serializableObject.GetType().Name;
            this.BodyType = serializableObject.GetType();

        }

        public Message(Stream BodyStream)
        {
            initializeMessage();
            this.BodyStream = BodyStream;
            this.ContentType = BodyStream.GetType().Name.ToLower();
        }

        /// <summary>
        /// Initialize Message content
        /// </summary>
        private void initializeMessage()
        {
            Body = null;
            BodyStream = null;
            ContentType = null;
            MessageId = null;
            Properties = new Dictionary<string, object>();
            Size = 0;

            TimeToLive = new TimeSpan();

        }

        /// <summary>
        /// Get body of message
        /// </summary>
        /// <typeparam name="T">Type of Message</typeparam>
        /// <returns>content of message</returns>
        public T GetBody<T>()
        {

            if (this.Body != null)
            {
                return (T)Body;
            }
            else if (BodyStream != null && typeof(T) == typeof(Stream))
            {
                return (T)(object)BodyStream;
            }
            else
            {
                return default(T);
            }

        }

        /// <summary>
        /// Get type of message body
        /// </summary>
        /// <returns>Type of message body in string</returns>
        public string GetContentType()
        {
            return this.ContentType;
        }

        /// <summary>
        /// Get type of message body
        /// </summary>
        /// <returns>Type of message body in Type</returns>
        public Type GetBodyType()
        {
            return this.BodyType;
        }
    }
}
