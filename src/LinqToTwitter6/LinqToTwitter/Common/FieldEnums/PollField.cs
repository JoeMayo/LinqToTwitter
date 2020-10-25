namespace LinqToTwitter.Common
{
    /// <summary>
    /// Fields that can be expanded on <see cref="TweetPoll"/>
    /// </summary>
    public class PollField
    {
        /// <summary>
        /// All expandable fields
        /// </summary>
        public const string AllFields = 
            "duration_minutes, end_datetime, id, options, voting_status";

        /// <summary>
        /// duration_minutes
        /// </summary>
        public const string Duration = "duration_minutes";

        /// <summary>
        /// end_datetime
        /// </summary>
        public const string EndDateTime = "end_datetime";

        /// <summary>
        /// id
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// options
        /// </summary>
        public const string Options = "options";

        /// <summary>
        /// voting_status
        /// </summary>
        public const string VotingStatus = "voting_status";
    }
}
