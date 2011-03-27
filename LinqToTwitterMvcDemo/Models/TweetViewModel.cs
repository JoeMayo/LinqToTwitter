

namespace LinqToTwitterMvcDemo.Models
{
    /// <summary>
    /// Info on friend tweets
    /// </summary>
    public class TweetViewModel
    {
        /// <summary>
        /// User's avatar
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// User's Twitter name
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Text containing user's tweet
        /// </summary>
        public string Tweet { get; set; }
    }
}