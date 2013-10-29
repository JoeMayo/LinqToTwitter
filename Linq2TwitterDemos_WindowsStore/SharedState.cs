using System;
using System.Linq;
using LinqToTwitter;

namespace Linq2TwitterDemos_WindowsStore
{
    public class SharedState
    {
        public static IAuthorizer Authorizer { get; set; }
    }
}
