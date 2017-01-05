using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Uwp
{
    public class Settings
    {
        public string CcuUrl;
        public TimeSpan Timeout;

        public Settings()
        {
            CcuUrl = "http://192.168.0.222:2001";
            Timeout = new TimeSpan(5000);
        }

        public Settings(string ccuUrl, TimeSpan timeout)
        {
            CcuUrl = ccuUrl;
            Timeout = timeout;
        }
    }
}
