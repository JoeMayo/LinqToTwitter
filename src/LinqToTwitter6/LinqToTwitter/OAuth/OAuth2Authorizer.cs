using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    public class OAuth2Authorizer : AuthorizerBase, IAuthorizer
    {
        List<string> permissions;

        public OAuth2Authorizer()
        {
            permissions = new List<string>();
        }

        public Task AuthorizeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
