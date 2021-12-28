using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public class MvcAuthorizer : AspNetAuthorizer
    {
        string _authUrl;

        public async Task<ActionResult> BeginAuthorizationAsync()
        {
            return await BeginAuthorizationAsync(Callback).ConfigureAwait(false);
        }

        public async Task<ActionResult> BeginAuthorizationAsync(Uri callback, Dictionary<string, string> parameters = null)
        {
            if (GoToTwitterAuthorization == null)
                GoToTwitterAuthorization = authUrl => { _authUrl = authUrl; };

            Callback = callback;

            await base.BeginAuthorizeAsync(callback, parameters).ConfigureAwait(false);

            return new RedirectResult(_authUrl);
        }
    }
}
