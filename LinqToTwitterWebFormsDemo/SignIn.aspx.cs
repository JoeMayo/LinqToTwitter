using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;
using System.Configuration;

public partial class SignIn : System.Web.UI.Page
{
    private SignInAuthorizer auth;

    protected void Page_Load(object sender, EventArgs e)
    {
        auth = Session["SignInAuthorizer"] as SignInAuthorizer;

        if (auth == null)
        {
            auth = new SignInAuthorizer
            {
                ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"],
            };

            Session["SignInAuthorizer"] = auth;
        }

        if (!Page.IsPostBack)
        {
            if (!string.IsNullOrWhiteSpace(auth.ConsumerKey) &&
                !string.IsNullOrWhiteSpace(auth.ConsumerSecret))
            {
                AuthMultiView.ActiveViewIndex = 1;

                if (auth.CompleteAuthorization())
                {
                    AuthMultiView.SetActiveView(SignedInView);
                    screenNameLabel.Text = auth.ScreenName;
                }
            }
        }
    }

    protected void signInButton_Click(object sender, ImageClickEventArgs e)
    {
        auth.BeginAuthorization();
    }
}
