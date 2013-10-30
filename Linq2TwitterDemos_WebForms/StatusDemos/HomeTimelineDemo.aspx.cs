using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;

namespace Linq2TwitterDemos_WebForms.StatusDemos
{
    public partial class HomeTimelineDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected async void RefreshButton_Click(object sender, EventArgs e)
        {
            var auth = new AspNetAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(),
                GoToTwitterAuthorization = twitterUrl => { }
            };

            var ctx = new TwitterContext(auth);

            var tweets =
                await
                (from tweet in ctx.Status
                 where tweet.Type == StatusType.Home
                 select tweet)
                .ToListAsync();

            TwitterListView.DataSource = tweets;
            TwitterListView.DataBind();
        }
    }
}