#if !NETFX_CORE
using System;
using System.Web;
using System.Web.Mvc;

namespace LinqToTwitter
{
    public class MvcOAuthActionResult : ActionResult
    {
        private readonly WebAuthorizer webAuth;

        public MvcOAuthActionResult(WebAuthorizer webAuth)
        {
            this.webAuth = webAuth;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            webAuth.PerformRedirect = authUrl =>
            {
                HttpContext.Current.Response.Redirect(authUrl);
            };

            Uri callback =
                webAuth.Callback == null ?
                    HttpContext.Current.Request.Url :
                    webAuth.Callback;

            webAuth.BeginAuthorization(callback);
        }
    }
}

#endif