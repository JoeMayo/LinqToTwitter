using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;

namespace Linq2TwitterDemos_WebForms
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!new SessionStateCredentialStore().HasAllCredentials())
                Response.Redirect("~/Oauth.aspx", false);
        }
    }
}