using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Linq2TwitterDemos_WindowsForms
{
    public class Tweet
    {
        public string ImageUrl { get; set; }

        public Bitmap UserImage { get; set; }

        public string ScreenName { get; set; }

        public string TweetText { get; set; }

        public async Task LoadImage()
        {
            HttpResponseMessage response = await new HttpClient().GetAsync(ImageUrl);
            Stream responseStream = await response.Content.ReadAsStreamAsync();
            UserImage = new Bitmap(responseStream);
        }

        public object[] ToArray()
        {
            return new object[] { UserImage, ScreenName, TweetText };
        }
    }
}
