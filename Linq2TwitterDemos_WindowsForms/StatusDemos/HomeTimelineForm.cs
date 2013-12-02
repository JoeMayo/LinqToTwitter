using System;
using System.Linq;
using System.Windows.Forms;
using LinqToTwitter;

namespace Linq2TwitterDemos_WindowsForms.StatusDemos
{
    public partial class HomeTimelineForm : Form
    {
        public HomeTimelineForm()
        {
            InitializeComponent();
        }

        async void HomeTimelineForm_Load(object sender, EventArgs e)
        {
            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home
                 select new Tweet
                 {
                     ImageUrl = tweet.User.ProfileImageUrl,
                     ScreenName = tweet.User.ScreenNameResponse,
                     TweetText = tweet.Text
                 })
                .ToListAsync();

            TweetDataGridView.Rows.Clear();

            tweets.ForEach(async tweet =>
                {
                    await tweet.LoadImage();
                    TweetDataGridView.Rows.Add(tweet.ToArray());
                });
        }


    }
}
