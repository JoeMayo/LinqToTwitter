using System.Collections.Generic;

namespace LinqToTwitter.Security
{
    public interface IOAuth
    {
        string GetAuthorizationString(string method, string url, IDictionary<string, string> parameters, string consumerSecret, string oAuthTokenSecret);
    }
}