using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;

public partial class PostOnly : System.Web.UI.Page
{
    private WebOAuthAuthorization auth;
    private TwitterContext twitterCtx;

    protected void Page_Load(object sender, EventArgs e)
    {
        auth = new WebOAuthAuthorization(InMemoryTokenManager.Instance, InMemoryTokenManager.AccessToken);

        if (string.IsNullOrEmpty(InMemoryTokenManager.Instance.ConsumerKey) ||
            string.IsNullOrEmpty(InMemoryTokenManager.Instance.ConsumerSecret) ||
            !auth.CachedCredentialsAvailable)
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
