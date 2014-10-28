using System;
using System.Linq;
using Windows.Storage;

namespace LinqToTwitter
{
    public class RoamingDataCredentials : WinRtCredentials, IOAuthCredentials
    {
        public RoamingDataCredentials() : 
            base (ApplicationData.Current.RoamingFolder, null) { }

        public RoamingDataCredentials(string fileName) :
            base(ApplicationData.Current.RoamingFolder, fileName) { }
    }
}
