using System;
using System.Web.Mvc;

namespace LinqToTwitter
{
    public class MvcAuthorizer : WebAuthorizer
    {
        public ActionResult BeginAuthorization()
        {
            return new MvcOAuthActionResult(this);
        }

        public ActionResult BeginAuthorization(Uri callback)
        {
            this.Callback = callback;
            return new MvcOAuthActionResult(this);
        }
    }
}
