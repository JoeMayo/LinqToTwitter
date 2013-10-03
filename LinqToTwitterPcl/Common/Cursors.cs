using System.Xml.Serialization;

using LinqToTwitter.Common;

using LitJson;

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
        public Cursors() {}
        internal Cursors(JsonData cursors)
        {
            Next = cursors.GetValue<ulong>("next_cursor").ToString();
            Previous = cursors.GetValue<ulong>("previous_cursor").ToString();
        }

        /// <summary>
        /// Use this value to retrieve the next page
        /// </summary>
        [XmlIgnore]
        public string Next { get; internal set; }

        /// <summary>
        /// Use this value to go back to the previous page
        /// </summary>
        [XmlIgnore]
        public string Previous { get; internal set; }
    }
}
