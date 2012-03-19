namespace LinqToTwitter
{
    /// <summary>
    /// Restricts access type
    /// </summary>
    public enum AuthAccessType
    {
        /// <summary>
        /// Default - use account settings access
        /// </summary>
        NoChange,

        /// <summary>
        /// Restrict to read-only
        /// </summary>
        Read,

        /// <summary>
        /// Read/write access
        /// </summary>
        Write
    }
}
