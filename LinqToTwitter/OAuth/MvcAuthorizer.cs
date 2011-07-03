using System.Web.Mvc;

namespace LinqToTwitter
{
    public class MvcAuthorizer : WebAuthorizer
    {
        public ActionResult BeginAuthorization()
        {
            return new MvcOAuthActionResult(this);
        }
    }
}
