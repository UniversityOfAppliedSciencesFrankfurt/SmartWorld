using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlRpcCore
{
    /// <summary>
    /// Defines MethodCall object which contains name and parameters of method should be provided to the request
    /// </summary>
    public class MethodCall
    {
        /// <summary>
        /// Name of Method Call
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// List of needed parameters as Param object for Method Call
        /// </summary>
        public List<Param> SendParams { get; set; }

        /// <summary>
        /// Creates MethodCall object
        /// </summary>
        /// <param name="name">Name of MethodCall</param>
        /// <param name="listParam">List of Param objects</param>
        public MethodCall(string name, List<Param> listParam)
        {
            MethodName = name;
            SendParams = listParam;
        }

        /// <summary>
        /// Creates Method Call object
        /// </summary>
        /// <param name="name">Name of Method</param>
        /// <param name="listParams">List of parameters</param>
        public MethodCall(string name, List<object> listParams)
        {
            MethodName = name;
            SendParams = CreateListParams(listParams);
        }

        public MethodCall()
        {
        }

        /// <summary>
        /// Converts list of objects into list of Param-type objects
        /// </summary>
        /// <param name="paramList">list of objects</param>
        /// <returns>List of parameters</returns>
        public List<Param> CreateListParams(List<object> paramList)
        {
            List<Param> methodList = new List<Param>();
            foreach (object obj in paramList)
            {
                Param param = new Param();
                param.Value = obj;
                methodList.Add(param);
            }
            return methodList;
        }

        /// <summary>
        /// Add new item into list of params
        /// </summary>
        /// <param name="paramList">avalaible list of params</param>
        /// <param name="param">new param to be added</param>
        public void AddObjectToList(List<Param> paramList, Param param)
        {
            paramList.Add(param);
        }
    }
}
