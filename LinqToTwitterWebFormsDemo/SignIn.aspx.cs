using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;

public partial class SignIn : System.Web.UI.Page
{
    //private WebOAuthAuthorization auth;

    protected void Page_Load(object sender, EventArgs e)
    {
        //auth = new WebOAuthAuthorization(InMemoryTokenManager.Instance, InMemoryTokenManager.AccessToken);
        //if (!string.IsNullOrEmpty(InMemoryTokenManager.Instance.ConsumerKey) &&
        //    !string.IsNullOrEmpty(InMemoryTokenManager.Instance.ConsumerSecret))
        //{
        //    AuthMultiView.ActiveViewIndex = 1;

        //    if (!IsPostBack)
        //    {
        //        if (auth.CompleteAuthenticate())
        //        {
        //            AuthMultiView.SetActiveView(SignedInView);
        //            screenNameLabel.Text = auth.ScreenName;
        //        }
        //    }
        //}
    }

    protected void signInButton_Click(object sender, ImageClickEventArgs e)
    {
        //auth.BeginAuthenticate(false);
    }
}
