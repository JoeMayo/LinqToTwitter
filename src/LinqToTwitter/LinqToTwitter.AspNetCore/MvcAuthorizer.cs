using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public class MvcAuthorizer : AspNetAuthorizer
    {
        private string _authUrl;

        public async  Task<ActionResult> BeginAuthorizationAsync()
        {
            return await BeginAuthorizationAsync(Callback).ConfigureAwait(false);
        }

        public async Task<ActionResult> BeginAuthorizationAsync(Uri callback)
        {
            if (GoToTwitterAuthorization == null)
                GoToTwitterAuthorization = authUrl => { _authUrl = authUrl; };

            Callback = callback;

            await base.BeginAuthorizeAsync(callback).ConfigureAwait(false);

            return new RedirectResult(_authUrl);
        }
    }
}
