
using Daenet.Iot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DeleteMe
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            run();
        }

        private void run()
        {
            Task.Run(() =>
            {
                IotHubConnector conn = new IotHubConnector();
                conn.Open(new Dictionary<string, object>() {
                { "ConnStr", "HostName=DRoth-IotHub-01.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=G0ddVsZb5UYFIt2iVTnN+psldF0qRHHxKMUcAo1tdWE=" },
                { "DeviceId", "PI2-01" },
            }).Wait();


                int cnt = 0;
                var tokenSource = new CancellationTokenSource();

                conn.OnMessage((msg) =>
                {

                    cnt++;

                    if (cnt == 10)
                        tokenSource.Cancel();

                    return true;

                }, tokenSource.Token);

            });
        }
    }
}
