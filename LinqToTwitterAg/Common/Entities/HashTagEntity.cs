namespace LinqToTwitter
{
    /// <summary>
    /// Hash tag entity
    /// </summary>
    /// <example>#linqtotwitter</example>
    public class HashTagEntity : EntityBase
    {
        /// <summary>
        /// Tag name without the # sign
        /// </summary>
        public string Tag { get; set; }
    }
}
