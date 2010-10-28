using System;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using LinqToTwitter;

public partial class _Default : System.Web.UI.Page
{
    private WebAuthorizer auth;
    private TwitterContext twitterCtx;

    protected void Page_Load(object sender, EventArgs e)
    {
        auth = Session["WebAuthorizer"] as WebAuthorizer;

        if (auth == null)
        {
            string consumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
            string consumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];

            auth = new WebAuthorizer
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
            };

            Session["WebAuthorizer"] = auth; 
        }

        if (!Page.IsPostBack)
        {
            if (!string.IsNullOrWhiteSpace(auth.ConsumerKey) && 
                !string.IsNullOrWhiteSpace(auth.ConsumerSecret))
            {
                auth.CompleteAuthorization();
            }
        }

        if (string.IsNullOrWhiteSpace(auth.ConsumerKey) || 
            string.IsNullOrWhiteSpace(auth.ConsumerSecret))
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
        auth.BeginAuthorization();
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
