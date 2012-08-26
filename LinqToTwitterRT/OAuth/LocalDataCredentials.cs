using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace LinqToTwitter
{
    public class LocalDataCredentials : WinRtCredentials, IOAuthCredentials
    {
        public LocalDataCredentials() : 
            base (ApplicationData.Current.LocalFolder) { }
    }
}