using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Response from Twitter for previous and next pages
    /// </summary>
    /// <remarks>
    /// To use a cursor, start by setting the cursor to -1
    /// and then use one of these response cursors to move
    /// backwards or forwards in paged results.
    /// </remarks>
    public class Cursors
    {
        /// <summary>
        /// Use this value to retrieve the next page
        /// </summary>
        public string Next { get; internal set; }

        /// <summary>
        /// Use this value to go back to the previous page
        /// </summary>
        public string Previous { get; internal set; }
    }
}
