namespace LinqToTwitter
{
    public enum SpacesType
    { 
        /// <summary
        /// Search for spaces
        /// </summary>
        Search,

        /// <summary>
        /// Return spaces matching one or more space ids (comma-separated)
        /// </summary>
        BySpaceID,

        /// <summary>
        /// Return spaces matching one or more creators (comma-separated)
        /// </summary>
        ByCreatorID,
    }
}
