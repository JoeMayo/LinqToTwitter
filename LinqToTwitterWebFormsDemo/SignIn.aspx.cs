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
    private const string OAuthCredentialsKey = "OAuthCredentialsKey";
    private SignInAuthorizer auth;

    protected void Page_Load(object sender, EventArgs e)
    {
        IOAuthCredentials credentials = new InMemoryCredentials();
        string authString = Session[OAuthCredentialsKey] as string;

        if (authString == null)
        {
            credentials.ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
            credentials.ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];

            Session[OAuthCredentialsKey] = credentials.ToString();
        }
        else
        {
            credentials.Load(authString);
        }

        auth = new SignInAuthorizer
        {
            Credentials = new InMemoryCredentials
            {
                ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
            },
            PerformRedirect = authUrl => Response.Redirect(authUrl)
        };

        if (!Page.IsPostBack)
        {
            if (!string.IsNullOrWhiteSpace(credentials.ConsumerKey) &&
                !string.IsNullOrWhiteSpace(credentials.ConsumerSecret))
            {
                AuthMultiView.ActiveViewIndex = 1;

                if (auth.CompleteAuthorization(Request.Url))
                {
                    AuthMultiView.SetActiveView(SignedInView);
                    screenNameLabel.Text = auth.ScreenName;
                }
            }
        }
    }

    protected void signInButton_Click(object sender, ImageClickEventArgs e)
    {
        auth.BeginAuthorization(Request.Url);
    }
}
