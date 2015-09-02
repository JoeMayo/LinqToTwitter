using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LinqToTwitter
{
    public class MvcAuthorizer : AspNetAuthorizer
    {
        private string _authUrl;

        public async  Task<ActionResult> BeginAuthorizationAsync()
        {
            return await BeginAuthorizationAsync(Callback);
        }

        public async Task<ActionResult> BeginAuthorizationAsync(Uri callback)
        {
            if (GoToTwitterAuthorization == null)
                GoToTwitterAuthorization = authUrl => { _authUrl = authUrl; };

            Callback = callback;

            await base.BeginAuthorizeAsync(callback);

            return new RedirectResult(_authUrl);
        }
    }
}
