namespace LinqToTwitter
{
    /// <summary>
    /// type of saved search queries
    /// </summary>
    public enum SavedSearchType
    {
        /// <summary>
        /// query all searches by logged in user
        /// </summary>
        Searches,

        /// <summary>
        /// query for a single search by the search id
        /// </summary>
        Show
    }

    enum SavedSearchAction
    {
        Create,

        Destroy
    }
}
