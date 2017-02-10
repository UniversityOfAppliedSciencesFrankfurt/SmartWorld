using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZigBeeConnector
{
    public class ZigbeeConnector: ISendModule
    {
        protected bool m_responseRequired = true;
        protected UInt16 m_clusterId = 0;
        protected UInt16 m_responseClusterId = 0;
        protected byte[] m_payload = null;
        protected bool m_isZdoCommand = true;
        protected bool m_isNotification = false;
        private bool m_Mock;
        public ISendModule NextSendModule
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Open(Dictionary<string, object> args)
        {
            System.Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;
            Windows.ApplicationModel.PackageId packageId = package.Id;
            Windows.ApplicationModel.PackageVersion versionFromPkg = packageId.Version;

            this.Vendor = AdapterHelper.ADAPTER_VENDOR;
            this.AdapterName = AdapterHelper.ADAPTER_NAME;

            // the adapter prefix must be something like "com.mycompany" (only alpha num and dots)
            // it is used by the Device System Bridge as root string for all services and interfaces it exposes
            this.ExposedAdapterPrefix = AdapterHelper.ADAPTER_DOMAIN + "." + this.Vendor.ToLower();
            this.ExposedApplicationGuid = Guid.Parse(AdapterHelper.ADAPTER_APPLICATION_GUID);

            if (null != package && null != packageId)
            {
                this.ExposedApplicationName = packageId.Name;
                this.Version = versionFromPkg.Major.ToString() + "." +
                               versionFromPkg.Minor.ToString() + "." +
                               versionFromPkg.Revision.ToString() + "." +
                               versionFromPkg.Build.ToString();
            }
            else
            {
                this.ExposedApplicationName = AdapterHelper.ADAPTER_DEFAULT_APPLICATION_NAME;
                this.Version = AdapterHelper.ADAPTER_DEFAULT_VERSION;
            }

            try
            {
                this.Signals = new List<IAdapterSignal>();
                this.m_signalListeners = new Dictionary<int, IList<SIGNAL_LISTENER_ENTRY>>();
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void sendMessage()
        {
        }
    }
}
