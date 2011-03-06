using System;
namespace LinqToTwitter
{
    /// <summary>
    /// For working with Twitter Geo places
    /// </summary>
    public enum GeoType
    {
        /// <summary>
        /// Get a list of valid places (from Twitter) that can be attached to an Update
        /// </summary>
        Reverse,

        /// <summary>
        /// Get more details on a place (found via GeoType.Reverse)
        /// </summary>
        ID,

        /// <summary>
        /// Get a list of nearby places
        /// </summary>
        [Obsolete("Nearby has been deprecated, please use Search instead.", true)]
        Nearby,

        /// <summary>
        /// Performs a search, based on various criteria
        /// </summary>
        Search
    }
}
