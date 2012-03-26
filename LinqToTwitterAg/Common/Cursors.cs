using System.Xml.Linq;
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
            Next = cursors.GetValue<string>("next_cursor");
            Previous = cursors.GetValue<string>("previous_cursor");
        }

        /// <summary>
        /// Transforms XML document into a Cursors
        /// </summary>
        /// <param name="cursors">XElement with info</param>
        /// <returns>New Cursors instance</returns>
        public static Cursors CreateCursors(XElement cursors)
        {
            return
                new Cursors
                {
                    Next =
                        cursors == null || cursors.Element("next_cursor") == null ?
                            string.Empty :
                            cursors.Element("next_cursor").Value,
                    Previous =
                        cursors == null || cursors.Element("previous_cursor") == null ?
                            string.Empty :
                            cursors.Element("previous_cursor").Value
                };
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
