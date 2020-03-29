#if !NETCORE
using System.Configuration;
#endif
using System.Globalization;
using System.Threading;

namespace LinqToTwitterPcl.Tests.Common
{
    public class TestCulture
    {
        public static void SetCulture()
        {
#if NETCORE
            string culture = string.Empty;
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.CurrentCulture = cultureInfo;
#else
            string culture = ConfigurationManager.AppSettings["culture"];
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
#endif
        }
    }
}
