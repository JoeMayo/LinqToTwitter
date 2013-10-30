using System;
using System.Configuration;
using System.Linq;
using LinqToTwitter;

namespace Linq2TwitterDemos_WebForms
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
                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
                },
                GoToTwitterAuthorization = twitterUrl => Response.Redirect(twitterUrl, false)
            };

            if (!Page.IsPostBack && Request.QueryString["oauth_token"] != null)
            {
                await auth.CompleteAuthorizeAsync(Request.Url);
                Response.Redirect("~/Default.aspx", false);
            }
        }

        protected async void AuthorizeButton_Click(object sender, EventArgs e)
        {
            await auth.BeginAuthorizeAsync(Request.Url);
        }
    }
}