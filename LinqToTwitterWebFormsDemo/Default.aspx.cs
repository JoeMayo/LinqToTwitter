using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;
using System.Configuration;

public partial class _Default : System.Web.UI.Page
{
    //private WebOAuthAuthorization auth;
    WebAuthorizer auth;
    private TwitterContext twitterCtx;

    protected void Page_Load(object sender, EventArgs e)
    {
        //auth = new WebOAuthAuthorization(InMemoryTokenManager.Instance, InMemoryTokenManager.AccessToken);

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
            if (!string.IsNullOrWhiteSpace(auth.ConsumerKey) && !string.IsNullOrWhiteSpace(auth.ConsumerSecret))
            {
                auth.CompleteAuthorization();
            }
            //// try to complete a pending authorization in case there is one.
            //string accessToken = auth.CompleteAuthorize();
            //if (accessToken != null)
            //{
            //    // Store the access token in session state so we can get at it across page refreshes.
            //    // In a real app, you'd want to associate this access token with the user that is
            //    // logged into your app at this point.
            //    InMemoryTokenManager.AccessToken = accessToken;

            //    // Clear away the OAuth message so that users can refresh the page without problems.
            //    Response.Redirect(Page.Request.Path);
            //}
        }

        if (string.IsNullOrWhiteSpace(auth.ConsumerKey) || string.IsNullOrWhiteSpace(auth.ConsumerSecret))
        {
            // The user needs to set up the web.config file to include Twitter consumer key and secret.
            PrivateDataMultiView.SetActiveView(SetupTwitterConsumer);
        }
        else if (auth.IsAuthorized)
        {
            //auth.SignOn(); // acquire the screen name, and ensure the access token is still valid.
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

        //// demonstrate serialization

        //var serializableUser =
        //    (from user in twitterCtx.User
        //     where user.Type == UserType.Show &&
        //           user.ScreenName == "JoeMayo"
        //     select user)
        //     .FirstOrDefault();

        //// if you have ASP.NET state server turned on
        //// this will still work because User is serializable
        //Session["SerializableUser"] = serializableUser;
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
