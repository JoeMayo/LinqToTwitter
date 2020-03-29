using System;
using System.Linq;
using System.Web.UI;
using LinqToTwitter;

namespace WebFormsDemo
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