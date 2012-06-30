namespace LinqToTwitter
{
    /// <summary>
    /// Hash tag mention
    /// </summary>
    /// <example>#linqtotwitter</example>
    public class HashTagMention : MentionBase
    {
        /// <summary>
        /// Tag name without the # sign
        /// </summary>
        public string Tag { get; set; }
    }
}
