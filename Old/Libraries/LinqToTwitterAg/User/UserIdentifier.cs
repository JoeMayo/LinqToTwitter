namespace LinqToTwitter
{
    /// <summary>
    /// Identifier info returned from Twitter
    /// </summary>
    /// <remarks>
    /// This type was created to differentiate between the
    /// identifiers used in a query and the identifiers
    /// returned by Twitter. i.e. you might filter on UserID
    /// but not fill in the ID or ScreenName. However, Twitter
    /// will return all three identifiers for a user, which
    /// you might want to extract from query results. Therefore,
    /// you would use this instance, rather than the same-named
    /// properties of the parent object.
    /// </remarks>
    public class UserIdentifier
    {
        /// <summary>
        /// user's Twitter ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// User ID for disambiguating when ID is screen name
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// user's screen name
        /// On Input - disambiguates when ID is User ID
        /// </summary>
        public string ScreenName { get; set; }
    }
}
