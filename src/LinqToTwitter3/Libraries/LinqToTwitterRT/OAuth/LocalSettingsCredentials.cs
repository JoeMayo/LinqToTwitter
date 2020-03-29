//Contributed by: Ayo Adesugba

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace LinqToTwitter
{
    public class LocalSettingsCredentials : WinRtSettingsCredentials, IOAuthCredentials
    {
        public LocalSettingsCredentials() : 
            base (ApplicationData.Current.LocalSettings) { }
    }
}