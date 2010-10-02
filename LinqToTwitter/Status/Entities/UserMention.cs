using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Twitter user mention in the tweet
    /// </summary>
    /// <example>@linkedin</example>
    [Serializable]
    public class UserMention : MentionBase
    {
        /// <summary>
        /// Tweitter user Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Screen name of the Twitter User
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Name of the Twitter User
        /// </summary>
        public string Name { get; set; }
    }
}
