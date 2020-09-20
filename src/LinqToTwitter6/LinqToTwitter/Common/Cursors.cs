using System.Text.Json;
using System.Xml.Serialization;

namespace LinqToTwitter.Common
{
    /// <summary>
    /// Response from Twitter for previous and next pages
    /// </summary>
    /// <remarks>
    /// To use a cursor, start by setting the cursor to -1
    /// and then use one of these response cursors to move
    /// backwards or forwards in paged results.
    /// </remarks>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Cursors
    {
        public Cursors() {}
        internal Cursors(JsonElement cursors)
        {
            Next = cursors.GetLong("next_cursor");
            Previous = cursors.GetLong("previous_cursor");
        }

        /// <summary>
        /// Use this value to retrieve the next page
        /// </summary>
        [XmlIgnore]
        public long Next { get; internal set; }

        /// <summary>
        /// Use this value to go back to the previous page
        /// </summary>
        [XmlIgnore]
        public long Previous { get; internal set; }
    }
}
