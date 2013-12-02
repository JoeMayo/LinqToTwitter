using System;
using System.Linq;
using LinqToTwitter;

namespace Linq2TwitterDemos_WPF
{
    class SharedState
    {
        public static IAuthorizer Authorizer { get; set; }
    }
}
