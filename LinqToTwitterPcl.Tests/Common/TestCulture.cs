using System.Configuration;
using System.Globalization;
using System.Threading;

namespace LinqToTwitterPcl.Tests.Common
{
    public class TestCulture
    {
        public static void SetCulture()
        {
            string culture = ConfigurationManager.AppSettings["culture"];
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}
