//Contributed by: Ayo Adesugba

using System;
using System.Linq;
using Windows.Storage;

namespace LinqToTwitter
{
    public class RoamingSettingsCredentials : WinRtSettingsCredentials, IOAuthCredentials
    {
        public RoamingSettingsCredentials() : 
            base (ApplicationData.Current.RoamingSettings) { }
    }
}