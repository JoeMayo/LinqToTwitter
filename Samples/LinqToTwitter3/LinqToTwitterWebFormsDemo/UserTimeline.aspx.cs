using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserTimeline : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        IOAuthCredentials creds = new SessionStateCredentials();
        creds.ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
        creds.ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];
        creds.AccessToken = ConfigurationManager.AppSettings["twitterAccessToken"];
        creds.OAuthToken = ConfigurationManager.AppSettings["twitterOauthToken"];
        //Auth Object With Credentials
        var auth = new WebAuthorizer
        {
            Credentials = creds
        };     
        var twitterCtx = new TwitterContext(auth);

        var users =
            (from user in twitterCtx.Status
             where user.Type == StatusType.User &&
                   user.ScreenName == "JoeMayo" &&
                   user.Count == 200
             select user)
            .ToList();

        UserListView.DataSource = users;
        UserListView.DataBind();
    }
}