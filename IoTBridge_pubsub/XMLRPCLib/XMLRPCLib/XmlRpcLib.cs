using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace XmlRpcLib
{
    //List All the Device
    public interface IXmlRpcAll : IXmlRpcProxy
    {
        [XmlRpcMethod("listDevices")]
        XmlRpcLib.DeviceDesription.DeviceDescription[] ListDevices();
    }

    //Key Matic Related Methods
    public interface IXmlRpcKeyMatic : IXmlRpcProxy
    {
        [XmlRpcMethod("getValue")]
        object GetValue(string address, string valueKey);

        [XmlRpcMethod("setValue")]
        void SetValue(string address, string valueKey, object value);
    }

    //Temperature Sensor Methods
    public interface IXmlRpcTemperatureSensor : IXmlRpcProxy
    {
        [XmlRpcMethod("getValue")]
        double GetValue(string address, string valueKey);
    }

    //To Get Humidity Sensor Values and To get the Modes from Heater Methods
    public interface IXmlRpcHeaterHumiditySensor : IXmlRpcProxy
    {
        [XmlRpcMethod("getValue")]
        int GetValue(string address, string valueKey);
    }

    //Heater Sensor Methods
    public interface IXmlRpcHeaterSensor : IXmlRpcProxy
    {
        [XmlRpcMethod("getValue")]
        double GetValue(string address, string valueKey);

        [XmlRpcMethod("setValue")]
        void SetValue(string address, string valueKey, double value);

        [XmlRpcMethod("setValue")]
        void SetValue(string address, string valueKey, int value);
    }

    public interface IXmlRpcDimmaktorSensor : IXmlRpcProxy
    {
        //Remote Procedure Call to get the values of sensors with return type object
        [XmlRpcMethod("getValue")]
        object GetValue(string address, string value_key);


        //Remote Procedure Call to Set the values of sensors.
        //Value to be set is Double
        [XmlRpcMethod("setValue")]
        void SetValue(string address, string value_key, double level);
    }

}
