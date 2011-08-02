using System;
using System.Web;
using System.Web.Mvc;

namespace LinqToTwitter
{
    public class MvcOAuthActionResult : ActionResult
    {
        private WebAuthorizer m_webAuth;

        public MvcOAuthActionResult(WebAuthorizer webAuth)
        {
            m_webAuth = webAuth;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            m_webAuth.PerformRedirect = authUrl =>
            {
                HttpContext.Current.Response.Redirect(authUrl);
            };

            Uri callback = 
                m_webAuth.Callback == null ? 
                    HttpContext.Current.Request.Url : 
                    m_webAuth.Callback;

            m_webAuth.BeginAuthorization(callback);
        }
    }
}
