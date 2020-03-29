using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// For working with lists of IDs
    /// </summary>
    public class IDList
    {
        public IDList() { }
        public IDList(JsonData idJson)
        {
            if (idJson == null) return;

            CursorMovement = new Cursors(idJson);
            var ids = idJson.GetValue<JsonData>("ids");
            IDs =
                (from JsonData id in ids
                 select (ulong)id)
                .ToList();
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
