using System;
using Windows.Storage;

namespace LinqToTwitter
{
    public class LocalDataCredentials : WinRtCredentials, IOAuthCredentials
    {
        public LocalDataCredentials() : 
            base (ApplicationData.Current.LocalFolder, null) { }

        public LocalDataCredentials(string fileName) :
            base(ApplicationData.Current.LocalFolder, fileName) { }
    }
}