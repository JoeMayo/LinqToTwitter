using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace LinqToTwitter
{
    public class MvcAuthorizer : WebAuthorizer
    {
        public new ActionResult BeginAuthorization()
        {
            return new MvcOAuthActionResult(this);
        }
    }
}
