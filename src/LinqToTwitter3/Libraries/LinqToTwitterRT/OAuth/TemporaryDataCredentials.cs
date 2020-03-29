using System;
using System.Linq;
using Windows.Storage;

namespace LinqToTwitter
{
    public class TemporaryDataCredentials : WinRtCredentials, IOAuthCredentials
    {
        public TemporaryDataCredentials() : 
            base (ApplicationData.Current.TemporaryFolder, null) { }

        public TemporaryDataCredentials(string fileName) :
            base(ApplicationData.Current.TemporaryFolder, fileName) { }
    }
}
