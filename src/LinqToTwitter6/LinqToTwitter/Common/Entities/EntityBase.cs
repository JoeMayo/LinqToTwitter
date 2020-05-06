namespace LinqToTwitter.Common.Entities
{
    /// <summary>
    /// Base for all entities
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// Start of the entity in the tweet
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// End of the entity in the tweet
        /// </summary>
        public int End { get; set; }
    }
}
