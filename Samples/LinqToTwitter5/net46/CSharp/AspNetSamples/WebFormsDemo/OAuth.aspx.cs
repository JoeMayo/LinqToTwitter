using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;
using System.Configuration;

namespace WebFormsDemos
{
    public partial class OAuth : System.Web.UI.Page
    {
        AspNetAuthorizer auth;

        protected async void Page_Load(object sender, EventArgs e)
        {
            auth = new AspNetAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"]
                },
                GoToTwitterAuthorization =
                    twitterUrl => Response.Redirect(twitterUrl, false)
            };

            if (!Page.IsPostBack && Request.QueryString["oauth_token"] != null)
            {
                await auth.CompleteAuthorizeAsync(Request.Url);

                // This is how you access credentials after authorization.
                // The oauthToken and oauthTokenSecret do not expire.
                // You can use the userID to associate the credentials with the user.
                // You can save credentials any way you want - database, isolated 
                //   storage, etc. - it's up to you.
                // You can retrieve and load all 4 credentials on subsequent queries 
                //   to avoid the need to re-authorize.
                // When you've loaded all 4 credentials, LINQ to Twitter will let you 
                //   make queries without re-authorizing.
                //
                //var credentials = auth.CredentialStore;
                //string oauthToken = credentials.OAuthToken;
                //string oauthTokenSecret = credentials.OAuthTokenSecret;
                //string screenName = credentials.ScreenName;
                //ulong userID = credentials.UserID;
                //

                Response.Redirect("~/Default.aspx", false);
            }
        }

        protected async void AuthorizeButton_Click(object sender, EventArgs e)
        {
            await auth.BeginAuthorizeAsync(Request.Url);
        }
    }
}