using System;
using System.Web.UI;
using LinqToTwitter;

public partial class PostOnly : System.Web.UI.Page
{
    private WebAuthorizer auth;
    private TwitterContext twitterCtx;

    protected void Page_Load(object sender, EventArgs e)
    {
        auth = new WebAuthorizer();

        if (string.IsNullOrEmpty(auth.ConsumerKey) ||
            string.IsNullOrEmpty(auth.ConsumerSecret) ||
            !auth.IsAuthorized)
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
