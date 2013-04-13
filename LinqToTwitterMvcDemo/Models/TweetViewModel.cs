using System;
using System.Linq;

namespace LinqToTwitterMvcDemo.Models
{
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