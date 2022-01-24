using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public class MvcOAuth2Authorizer : OAuth2Authorizer
    {
        string authUrl;

        public new async Task<ActionResult> BeginAuthorizeAsync(string state = null)
        {
            if (GoToTwitterAuthorization == null)
                GoToTwitterAuthorization = authUrl => { this.authUrl = authUrl; };

            await base.BeginAuthorizeAsync(state).ConfigureAwait(false);

            return new RedirectResult(authUrl);
        }
    }
}
