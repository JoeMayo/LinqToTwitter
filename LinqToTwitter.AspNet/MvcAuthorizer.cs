using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LinqToTwitter
{
    public class MvcAuthorizer : AspNetAuthorizer
    {
        public async  Task<ActionResult> BeginAuthorizationAsync()
        {
            return await BeginAuthorizationAsync(Callback);
        }

        public new async Task<ActionResult> BeginAuthorizationAsync(Uri callback)
        {
            if (GoToTwitterAuthorization == null)
                GoToTwitterAuthorization = authUrl =>
                        HttpContext.Current.Response.Redirect(authUrl, false); 

            Callback = callback;

            await base.BeginAuthorizeAsync(callback);

            return new EmptyResult();
        }
    }
}
