using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter.Common;

namespace LinqToTwitter.OAuth
{
    public class OAuth : IOAuth
    {
        const string OAuthVersion = "1.0";
        const string OAuthSignatureMethod = "HMAC-SHA1";

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string OAuthToken { get; set; }

        public string OAuthTokenSecret { get; set; }

        public string GetAuthorizationString(string method, string url, IDictionary<string, string> parameters)
        {
            return null;
        }

        public string BuildEncodedSortedString(IDictionary<string, string> parameters)
        {
            return
                string.Join("&",
                    (from parm in parameters
                     orderby parm.Key
                     select parm.Key + "=" + BuildUrlHelper.UrlEncode(parameters[parm.Key]))
                    .ToArray());
        }
    }
}
