using System;
using LinqToTwitter;

namespace WindowsPhoneDemo
{
    public static class SharedState
    {
        public static ITwitterAuthorizer Authorizer { get; set; }
    }
}
