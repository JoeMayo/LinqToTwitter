using System.Collections.Generic;

namespace LinqToTwitter.Security
{
    public interface IOAuth
    {
        string ConsumerKey { get; set; }

        string ConsumerSecret { get; set; }

        string OAuthToken { get; set; }

        string OAuthTokenSecret { get; set; }

        string GetAuthorizationString(string method, string url, IDictionary<string, string> parameters);
    }
}