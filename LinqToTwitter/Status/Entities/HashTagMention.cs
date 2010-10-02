using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Hash tag mention
    /// </summary>
    /// <example>#linqtotwitter</example>
    [Serializable]
    public class HashTagMention : MentionBase
    {
        /// <summary>
        /// Tag name without the # sign
        /// </summary>
        public string Tag { get; set; }
    }
}
