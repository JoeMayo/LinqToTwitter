
namespace LinqToTwitter
{
    /// <summary>
    /// Info for queries and responses from control streams
    /// </summary>
    public class ControlStream
    {
        /// <summary>
        /// Type of control stream query (Followers or Info)
        /// </summary>
        public ControlStreamType Type { get; set; }

        /// <summary>
        /// ID of user to get followers for
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// ID of stream to query
        /// </summary>
        public string StreamID { get; set; }

        /// <summary>
        /// Response from an Info query
        /// </summary>
        public ControlStreamInfo Info { get; set; }

        /// <summary>
        /// Response from a Follow query
        /// </summary>
        public ControlStreamFollow Follow { get; set; }

        /// <summary>
        /// Response from a command, such as AddSiteStreamUser or RemoveSiteStreamUser
        /// </summary>
        public string CommandResponse { get; set; }
    }
}
