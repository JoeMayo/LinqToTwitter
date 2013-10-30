using System;
using System.Linq;
using LinqToTwitter;

namespace Linq2TwitterDemos_WebForms.StatusDemos
{
    public partial class TweetDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateTextBox.Text = "Testing LINQ to Twitter WebForms Demo - " + DateTime.Now;
        }

        protected async void PostUpdateButton_Click(object sender, EventArgs e)
        {
            var auth = new AspNetAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(),
                GoToTwitterAuthorization = twitterUrl => { }
            };

            var ctx = new TwitterContext(auth);

            await ctx.TweetAsync(UpdateTextBox.Text);

            SuccessLabel.Visible = true;
        }
    }
}