using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Lets caller know the percentage of completion of operation
    /// </summary>
    public class TwitterProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Percentage of completion
        /// </summary>
        public int PercentComplete { get; set; }
    }
}
