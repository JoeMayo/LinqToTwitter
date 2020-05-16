#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter.Common
{
    /// <summary>
    /// For working with lists of IDs
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class IDList
    {
        public IDList() { }
        public IDList(JsonElement idJson)
        {
            if (idJson.TryGetProperty("ids", out JsonElement ids))
            {
                CursorMovement = new Cursors(idJson);
                IDs =
                    (from id in ids.EnumerateArray()
                     select id.GetUInt64())
                    .ToList();
            }
        }

        /// <summary>
        /// Holds prev/next cursors
        /// </summary>
        public Cursors CursorMovement { get; set; }

        /// <summary>
        /// List of IDs returned
        /// </summary>
        public List<ulong> IDs { get; set; }
    }
}
