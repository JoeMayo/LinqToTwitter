#if !NETCORE
#endif
using System.Globalization;

namespace LinqToTwitter.Tests.Common
{
    public class TestCulture
    {
        public static void SetCulture()
        {
            string culture = string.Empty;
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.CurrentCulture = cultureInfo;
        }
    }
}
