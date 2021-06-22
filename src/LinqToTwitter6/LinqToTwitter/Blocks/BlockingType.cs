namespace LinqToTwitter
{
    public enum BlockingType
    {
        /// <summary>
        /// Retrieve a list of IDs of users being blocked
        /// </summary>
        Ids,

        /// <summary>
        /// Retrieve list of users (full User objects) being blocked
        /// </summary>
        List,

        /// <summary>
        /// Retrieve list of users being blocked
        /// </summary>
        Lookup,
    }
}
