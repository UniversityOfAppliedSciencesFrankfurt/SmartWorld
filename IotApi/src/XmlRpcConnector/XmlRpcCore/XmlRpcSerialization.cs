using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlRpcCore
{
    /// <summary>
    /// Provides the Serialization and Deserialization operations for XMLRPC protocol
    /// </summary>
    public class XmlRpcSerialization : XmlObjectSerializer
    {
        public XmlRpcSerialization()
        {

        }

        /// <summary>
        /// Serializes MethodCall object into string
        /// </summary>
        /// <param name="methodCall">MethodCall object need to be serialized</param>
        /// <returns>serialized string</returns>
        public static string XmlRpcSerialize(MethodCall methodCall)
        {
            string data;

            using (MemoryStream ms = new MemoryStream())
            {
                XmlRpcSerialization serializer = new XmlRpcSerialization();
                serializer.WriteObject(ms, methodCall);

                ArraySegment<byte> buff;
                if (ms.TryGetBuffer(out buff))
                    data = Encoding.UTF8.GetString(buff.ToArray());
                else
                    throw new InvalidOperationException();
            }
            return data;
        }

        /// <summary>
        /// Deserializes string response to MethodResponse object. If error, it returns to FaultException object.
        /// </summary>
        /// <param name="xmlRpcResponse">string response after sending request</param>
        /// <returns>MethodResponse object</returns>
        public static MethodResponse XmlRpcDeserialize(string xmlRpcResponse)
        {
            using (var reader = XmlReader.Create(new StringReader(xmlRpcResponse)))
            {
                XmlRpcSerialization deserializer = new XmlRpcSerialization();
                MethodResponse responseCcu = (MethodResponse)deserializer.ReadObject(reader);
                return responseCcu;
            }
        }

        #region Parse Value
        /// <summary>
        /// Returns object from XML message after reading tag "value"
        /// </summary>
        /// <param name="reader">XML Reader</param>
        /// <returns>returned object</returns>
        private object ParseValue(XmlDictionaryReader reader)
        {
            if (reader.IsEmptyElement)
                throw new ArgumentException("Emty in 'value' ");
            object value = null;
            string valueType = null;
            string result = null;
            reader.ReadStartElement("value");
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement)
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    valueType = reader.Name.ToLower();
                    if ((valueType != "struct") && (valueType != "array")) reader.Read();
                }
                if ((reader.NodeType == XmlNodeType.Text) || (reader.NodeType == XmlNodeType.Whitespace))
                {
                    result = reader.ReadContentAsString();
                    value = result;
                }
                if (valueType != null)
                {
                    switch (valueType)
                    {
                        case "string":
                            value = result;
                            break;

                        case "int":
                        case "i4":
                            Int64 intVal;
                            if (Int64.TryParse(result, out intVal))
                                value = intVal;
                            else throw new FormatException(String.Format("The value cannot be parse as an integer."));
                            break;


                        case "boolean":
                        case "bool":
                            Boolean boolVal;
                            if (Boolean.TryParse(result, out boolVal))
                                value = boolVal;
                            else
                            {
                                Int16 boolIntVal;
                                if (Int16.TryParse(result, out boolIntVal))
                                    value = boolIntVal == 1;

                                else throw new FormatException(String.Format("The value cannot be parsed as a boolean."));
                            }
                            break;

                        case "double":
                        case "float":
                            Double doubleVal;
                            if (Double.TryParse(result, out doubleVal))
                                value = doubleVal;
                            else throw new FormatException(String.Format("The value cannot be parsed as a double."));
                            break;

                        case "dateTime.iso8601":
                            break;

                        case "array":
                            value = ParseArray(reader);
                            break;

                        case "struct":
                            value = ParseStruct(reader);
                            break;


                        default:
                            throw new ArgumentException();
                    }
                    reader.ReadEndElement();
                }


            }

            reader.ReadEndElement();
            return value;

            //if (value != null) return value;
            //else throw new NotImplementedException();

        }
        #endregion

        #region Parse Array
        /// <summary>
        /// Returns array type of object after reading "array" tag
        /// </summary>
        /// <param name="reader">XML Reader</param>
        /// <returns>list of objects stored in array</returns>
        public object[] ParseArray(XmlDictionaryReader reader)
        {
            object[] ListArray = null;
            List<object> listObject = new List<object>();

            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name.ToLower() == "array")
                {
                    reader.ReadStartElement("array");
                    reader.MoveToContent();
                    reader.ReadStartElement("data");
                    reader.MoveToContent();

                    while ((reader.NodeType != XmlNodeType.EndElement) && (reader.NodeType != XmlNodeType.None))
                    {
                        object obj;
                        if (reader.LocalName == "value")
                        {
                            obj = ParseValue(reader);
                            listObject.Add(obj);
                        }
                    }
                    reader.ReadEndElement();
                    ListArray = listObject.ToArray();
                }
            }

            return ListArray;
        }

        #endregion

        #region Parse Struct
        /// <summary>
        /// Returns XMLRPC Struct object
        /// </summary>
        /// <param name="xmlString">XML String</param>
        /// <returns>XMLRPC Struct object</returns>
        public XmlRpcStruct ParseStruct(string xmlString)
        {
            XmlDictionaryReader reader = XmlDictionaryReader.CreateDictionaryReader(XmlReader.Create(new StringReader(xmlString)));
            return ParseStruct(reader);
        }

        /// <summary>
        /// Returns XMLRPC Struct object
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>XMLRPC Struct object</returns>
        public XmlRpcStruct ParseStruct(XmlDictionaryReader reader)
        {
            MethodFaultResponse faultResponse = new MethodFaultResponse();
            XmlRpcStruct xmlRpcStruct = new XmlRpcStruct();
            List<StructMember> listMembers = new List<StructMember>();
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name.ToLower() == "struct")
                {
                    reader.ReadStartElement("struct");
                    reader.MoveToContent();

                    while ((reader.NodeType != XmlNodeType.EndElement) && (reader.NodeType != XmlNodeType.None))
                    {
                        StructMember mem;
                        if (reader.LocalName == "member")
                        {
                            mem = new StructMember();
                            mem = ParseStructMember(reader);
                            listMembers.Add(mem);
                        }

                    }
                    xmlRpcStruct.Member = listMembers.ToArray();
                }

            }
            return xmlRpcStruct;
        }

        /// <summary>
        /// Returns Struct-Member properties
        /// </summary>
        /// <param name="reader">XML Reader</param>
        /// <returns>StructMember object</returns>
        private StructMember ParseStructMember(XmlDictionaryReader reader)
        {
            if (reader.IsEmptyElement)
                throw new ArgumentException("Emty in 'member' ");

            string name = null;
            object value = null;
            reader.ReadStartElement("member");
            reader.MoveToContent();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                switch (reader.LocalName)
                {
                    case "value":
                        value = ParseValue(reader);
                        break;
                    case "name":
                        name = ParseMemberName(reader);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            reader.ReadEndElement();
            return new StructMember(name, value);
        }

        /// <summary>
        /// Returns name of Struct-Member
        /// </summary>
        /// <param name="reader">XML Reader</param>
        /// <returns>name of struct-member as string</returns>
        private string ParseMemberName(XmlDictionaryReader reader)
        {
            string name;
            if (reader.IsEmptyElement)
                throw new ArgumentException("Emty in 'name' ");
            reader.ReadStartElement("name");
            reader.MoveToContent();
            name = reader.ReadContentAsString();
            reader.ReadEndElement();
            return name;

        }

        #endregion

        #region SerializationMethod




        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read XML document and deserialize into object
        /// </summary>
        /// <param name="reader">XML Reader</param>
        /// <param name="verifyObjectName"></param>
        /// <returns></returns>
        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            MethodResponse methodResponse = null;
            MethodFaultResponse faultResponse = null;

            bool isFault = false;

            Param param = new Param();
            object value = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.ToLower() == "fault")
                    {
                        faultResponse = new MethodFaultResponse();
                        isFault = true;
                    }

                    if (isFault == true)
                    {
                        if (reader.Name.ToLower() == "value")
                        {
                            value = ParseValue(reader);
                            XmlRpcStruct xmlRpcStruct = (XmlRpcStruct)value;

                            foreach (StructMember member in xmlRpcStruct.Member)
                            {
                                if (member.Name == "faultCode") faultResponse.FaultCode = Convert.ToInt16(member.Value);
                                else if (member.Name == "faultString") faultResponse.Message = (string)member.Value;
                            }
                        }

                    }
                    else
                    {
                        if (reader.Name.ToLower() == "params")
                        {
                            methodResponse = new MethodResponse();
                            methodResponse.ReceiveParams = new List<Param>();
                        }

                        else if (reader.Name.ToLower() == "value")
                        {
                            value = ParseValue(reader);
                            if (value != null)
                            {
                                param.Value = value;
                                methodResponse.ReceiveParams.Add(param);
                            }
                        }


                    }
                }
            }


            if (isFault)
            {
                isFault = false;
                //return faultResponse;
                XmlRpcFaultException ex = new XmlRpcFaultException(faultResponse.FaultCode, faultResponse.Message);
                throw ex;
            }
            else return methodResponse;
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            MethodCall methodCall = graph as MethodCall;
            writer.WriteStartElement("methodName");
            writer.WriteString(methodCall.MethodName);
            writer.WriteEndElement();

            writer.WriteStartElement("params");

            // List Devices if SendParams is null
            if (methodCall.SendParams != null)
            {
                foreach (var param in methodCall.SendParams)
                {
                    writer.WriteStartElement("param");
                    writer.WriteStartElement("value");
                    writeValue(writer, param);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        private void writeValue(XmlDictionaryWriter writer, Param param)
        {
            string valueType;
            object value = param.Value;
            if (param.Value.GetType().IsArray) valueType = "array";
            else if (param.Value.GetType().Name.ToLower() == "xmlrpcstruct") valueType = "struct";
            else valueType = param.Value.GetType().Name.ToLower();

            switch (valueType)
            {
                case "string":
                    valueType = "string";
                    break;

                case "bool":
                case "boolean":
                    valueType = "boolean";
                    bool temp = (bool)value;
                    if (temp == true) { value = 1; }
                    else value = 0;
                    break;

                case "int16":
                case "int32":
                case "int64":
                case "int":
                case "short":
                case "long":
                    valueType = "i4";
                    break;

                case "float":
                case "double":
                    valueType = "double";
                    break;

                case "array":
                    valueType = "array";
                    writer.WriteStartElement(valueType);
                    writer.WriteStartElement("data");
                    Array arrayParam = (Array)value;
                    foreach (object element in arrayParam)
                    {
                        Param newParam = new Param() { Value = element };
                        writer.WriteStartElement("value");
                        writeValue(writer, newParam);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    return;

                case "struct":
                case "xmlrpcstruct":
                    valueType = "struct";
                    writer.WriteStartElement(valueType);
                    XmlRpcStruct structParam = (XmlRpcStruct)value;
                    foreach (StructMember member in structParam.Member)
                    {
                        writer.WriteStartElement("member");
                        Param memberName = new Param() { Value = member.Name };
                        Param memberValue = new Param() { Value = member.Value };
                        writer.WriteStartElement("name");
                        writeValue(writer, memberName);
                        writer.WriteEndElement();
                        writer.WriteStartElement("value");
                        writeValue(writer, memberValue);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    return;

                case "datetime":
                    valueType = "dateTime.iso8601";
                    DateTime dt = new DateTime();
                    dt = (DateTime)value;
                    value = dt.ToString("o");
                    break;

                default:
                    throw new NotSupportedException("Unsupported type!");
            }

            writer.WriteStartElement(valueType);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {

            if (graph.GetType() != typeof(MethodCall)) throw new NotSupportedException();
            writer.WriteStartElement("methodCall");
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            writer.WriteEndElement();
        }
        
        #endregion
    }
}
