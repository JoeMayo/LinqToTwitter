using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Search for tweets with different types of attitudes
    /// </summary>
    [Flags]
    public enum Attitude
    {
        /// <summary>
        /// Happy
        /// </summary>
        Positive = 0x01,

        /// <summary>
        /// Sad
        /// </summary>
        Negative = 0x02,

        /// <summary>
        /// Curious
        /// </summary>
        Question = 0x04
    }
}
