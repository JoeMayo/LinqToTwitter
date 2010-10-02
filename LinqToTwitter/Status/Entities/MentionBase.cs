using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Base for all entity mentions
    /// </summary>
    [Serializable]
    public abstract class MentionBase
    {
        /// <summary>
        /// Start of the mention in the tweet
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// End of the mention in the tweet
        /// </summary>
        public int End { get; set; }
    }
}
