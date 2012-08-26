using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LinqToTwitter
{
    public class TemporaryDataCredentials : WinRtCredentials, IOAuthCredentials
    {
        public TemporaryDataCredentials() : 
            base (ApplicationData.Current.TemporaryFolder) { }
    }
}
