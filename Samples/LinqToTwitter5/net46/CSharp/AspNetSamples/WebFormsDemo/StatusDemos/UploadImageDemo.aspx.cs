using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using LinqToTwitter;

namespace WebFormsDemo.StatusDemos
{
    public partial class UploadImageDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected async void UploadButton_Click(object sender, EventArgs e)
        {
            var auth = new AspNetAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(),
                GoToTwitterAuthorization = twitterUrl => { }
            };

            var twitterCtx = new TwitterContext(auth);

            string status = $"Testing multi-image tweet #Linq2Twitter £ {DateTime.Now}";
            string mediaCategory = "tweet_image";

            string path = Server.MapPath("..\\Content\\200xColor_2.png");
            var imageUploadTasks =
                new List<Task<Media>>
                {
                    twitterCtx.UploadMediaAsync(System.IO.File.ReadAllBytes(path), "image/jpg", mediaCategory),
                };

            await Task.WhenAll(imageUploadTasks);

            List<ulong> mediaIds =
                (from tsk in imageUploadTasks
                 select tsk.Result.MediaID)
                .ToList();

            Status tweet = await twitterCtx.TweetAsync(status, mediaIds);

            UserImage.ImageUrl = tweet.User.ProfileImageUrl;
            NameLabel.Text = tweet.User.ScreenNameResponse;
            TweetLabel.Text = tweet.Text;
        }
    }
}