namespace LinqToTwitter.Common
{
    /// <summary>
    /// Fields that can be expanded on <see cref="TweetPlace"/>
    /// </summary>
    public class PlaceField
    {
        /// <summary>
        /// All expandable fields
        /// </summary>
        public const string AllFields = 
            "contained_within,country,country_code," +
            "full_name,geo,id,name,place_type";

        /// <summary>
        /// contained_within
        /// </summary>
        public const string ContainedWithin = "contained_within";

        /// <summary>
        /// country
        /// </summary>
        public const string Country = "country";

        /// <summary>
        /// country_code
        /// </summary>
        public const string CoutryCode = "country_code";

        /// <summary>
        /// full_name
        /// </summary>
        public const string FullName = "full_name";

        /// <summary>
        /// geo
        /// </summary>
        public const string Geo = "geo";

        /// <summary>
        /// id
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// name
        /// </summary>
        public const string Name = "name";

        /// <summary>
        /// place_type
        /// </summary>
        public const string PlaceType = "place_type";
    }
}
