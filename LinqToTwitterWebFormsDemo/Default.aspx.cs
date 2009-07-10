using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;

public partial class _Default : System.Web.UI.Page
{
    private static InMemoryTokenManager TokenManager
    {
        get
        {
            InMemoryTokenManager tokenManager = (InMemoryTokenManager)HttpContext.Current.Application["TokenManager"];
            if (tokenManager == null)
            {
                HttpContext.Current.Application["TokenManager"] = tokenManager = new InMemoryTokenManager();
            }

            return tokenManager;
        }
    }

    private string AccessToken
    {
        get { return (string)Session["AccessToken"]; }
        set { Session["AccessToken"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        WebOAuthAuthorization auth = new WebOAuthAuthorization(TokenManager, this.AccessToken);
        if (!Page.IsPostBack)
        {
             // try to complete a pending authorization in case there is one.
            string accessToken = auth.CompleteAuthorize();
            if (accessToken != null)
            {
                // Store the access token in session state so we can get at it across page refreshes.
                // In a real app, you'd want to associate this access token with the user that is
                // logged into your app at this point.
                this.AccessToken = accessToken;

                // Clear away the OAuth message so that users can refresh the page without problems.
                Response.Redirect(Page.Request.Path);
            }
        }

        if (string.IsNullOrEmpty(TokenManager.ConsumerKey) || string.IsNullOrEmpty(TokenManager.ConsumerSecret))
        {
            // The user needs to set up the web.config file to include Twitter consumer key and secret.
            PrivateDataMultiView.SetActiveView(SetupTwitterConsumer);
        }
        else if (auth.CachedCredentialsAvailable)
        {
            auth.SignOn(); // acquire the screen name, and ensure the access token is still valid.
            screenNameLabel.Text = auth.ScreenName;
            PrivateDataMultiView.SetActiveView(ViewPrivateUpdates);
        }
        else
        {
            PrivateDataMultiView.SetActiveView(AuthorizeTwitter);
        }

        var twitterCtx = auth.CachedCredentialsAvailable ? new TwitterContext(auth) : new TwitterContext();

        var tweets =
            from tweet in twitterCtx.Status
            where tweet.Type == (auth.CachedCredentialsAvailable ? StatusType.Friends : StatusType.Public)
            select tweet;

        TwitterListView.DataSource = tweets;
        TwitterListView.DataBind();
    }

    protected void authorizeTwitterButton_Click(object sender, EventArgs e)
    {
        var oauth = new WebOAuthAuthorization(TokenManager, this.AccessToken);
        oauth.BeginAuthorize();
    }
}
