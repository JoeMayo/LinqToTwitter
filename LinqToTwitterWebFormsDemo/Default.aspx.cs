using System;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using LinqToTwitter;

public partial class _Default : System.Web.UI.Page
{
    private const string OAuthCredentialsKey = "OAuthCredentialsKey";
    private WebAuthorizer auth;
    private TwitterContext twitterCtx;

    protected void Page_Load(object sender, EventArgs e)
    {
        IOAuthCredentials credentials = new SessionStateCredentials();

        if (credentials.ConsumerKey == null || credentials.ConsumerSecret == null)
        {
            credentials.ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
            credentials.ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];
        }

        auth = new WebAuthorizer
        {
            Credentials = credentials,
            PerformRedirect = authUrl => Response.Redirect(authUrl)
        };

        if (!Page.IsPostBack)
        {
            auth.CompleteAuthorization(Request.Url);
        }

        if (credentials.ConsumerKey.IsNullOrWhiteSpace() ||
            credentials.ConsumerSecret.IsNullOrWhiteSpace())
        {
            // The user needs to set up the web.config file to include Twitter consumer key and secret.
            PrivateDataMultiView.SetActiveView(SetupTwitterConsumer);
        }
        else if (auth.IsAuthorized)
        {
            screenNameLabel.Text = auth.ScreenName;
            PrivateDataMultiView.SetActiveView(ViewPrivateUpdates);
            updateBox.Focus();
        }
        else
        {
            PrivateDataMultiView.SetActiveView(AuthorizeTwitter);
        }

        twitterCtx = auth.IsAuthorized ? new TwitterContext(auth) : new TwitterContext();

        var tweets =
            from tweet in twitterCtx.Status
            where tweet.Type == (auth.IsAuthorized ? StatusType.Friends : StatusType.Public)
            select tweet;

        TwitterListView.DataSource = tweets;
        TwitterListView.DataBind();
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (twitterCtx != null)
        {
            twitterCtx.Dispose();
            twitterCtx = null;
        }
    }

    protected void authorizeTwitterButton_Click(object sender, EventArgs e)
    {
        auth.BeginAuthorization(Request.Url);
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
