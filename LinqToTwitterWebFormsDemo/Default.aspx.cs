using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;

public partial class _Default : System.Web.UI.Page
{
    private WebOAuthAuthorization auth;
    private TwitterContext twitterCtx;

    protected void Page_Load(object sender, EventArgs e)
    {
        auth = new WebOAuthAuthorization(InMemoryTokenManager.Instance, InMemoryTokenManager.AccessToken);

        if (!Page.IsPostBack)
        {
             // try to complete a pending authorization in case there is one.
            string accessToken = auth.CompleteAuthorize();
            if (accessToken != null)
            {
                // Store the access token in session state so we can get at it across page refreshes.
                // In a real app, you'd want to associate this access token with the user that is
                // logged into your app at this point.
                InMemoryTokenManager.AccessToken = accessToken;

                // Clear away the OAuth message so that users can refresh the page without problems.
                Response.Redirect(Page.Request.Path);
            }
        }

        if (string.IsNullOrEmpty(InMemoryTokenManager.Instance.ConsumerKey) || string.IsNullOrEmpty(InMemoryTokenManager.Instance.ConsumerSecret))
        {
            // The user needs to set up the web.config file to include Twitter consumer key and secret.
            PrivateDataMultiView.SetActiveView(SetupTwitterConsumer);
        }
        else if (auth.CachedCredentialsAvailable)
        {
            auth.SignOn(); // acquire the screen name, and ensure the access token is still valid.
            screenNameLabel.Text = auth.ScreenName;
            PrivateDataMultiView.SetActiveView(ViewPrivateUpdates);
            updateBox.Focus();
        }
        else
        {
            PrivateDataMultiView.SetActiveView(AuthorizeTwitter);
        }

        twitterCtx = auth.CachedCredentialsAvailable ? new TwitterContext(auth) : new TwitterContext();

        var tweets =
            from tweet in twitterCtx.Status
            where tweet.Type == (auth.CachedCredentialsAvailable ? StatusType.Friends : StatusType.Public)
            select tweet;

        TwitterListView.DataSource = tweets;
        TwitterListView.DataBind();
    }

    protected void authorizeTwitterButton_Click(object sender, EventArgs e)
    {
        auth.BeginAuthorize();
    }

    protected void postUpdateButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            return;
        }

        twitterCtx.UpdateStatus(updateBox.Text);
        updateBox.Text = string.Empty;
    }
}
