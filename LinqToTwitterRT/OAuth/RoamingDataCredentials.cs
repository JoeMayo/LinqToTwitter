using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LinqToTwitter
{
    public class RoamingDataCredentials : WinRtCredentials, IOAuthCredentials
    {
        public RoamingDataCredentials() : 
            base (ApplicationData.Current.RoamingFolder) { }
    }
}
