using System;
using System.Configuration;
using LinqToTwitter;

public partial class PostOnly : System.Web.UI.Page
{
    private const string OAuthCredentialsKey = "OAuthCredentialsKey";
    private WebAuthorizer auth;
    private TwitterContext twitterCtx;

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

        auth = new WebAuthorizer
        {
            Credentials = new InMemoryCredentials
            {
                ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
            },
            PerformRedirect = authUrl => Response.Redirect(authUrl)
        };

        if (string.IsNullOrEmpty(credentials.ConsumerKey) ||
            string.IsNullOrEmpty(credentials.ConsumerSecret) ||
            !auth.IsAuthorized)
        {
            // Authorization occurs only on the home page.
            Response.Redirect("~/");
        }

        updateBox.Focus();
    }


    protected void postUpdateButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            return;
        }

        twitterCtx = new TwitterContext(auth);
        twitterCtx.UpdateStatus(updateBox.Text);
        updateBox.Text = string.Empty;
        successLabel.Visible = true;
    }
}
