using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// For working with lists of IDs
    /// </summary>
    [Serializable]
    public class IDList
    {
        /// <summary>
        /// Translate XML to IDList
        /// </summary>
        /// <param name="idList">XML with IDs and cursor info</param>
        /// <returns>New IDList instance</returns>
        public static IDList CreateIDList(XElement idList)
        {
            return new IDList
            {
                CursorMovement = Cursors.CreateCursors(idList),
                IDs =
                    (from id in idList.Element("ids").Elements("id")
                     select id.Value)
                     .ToList()
            };
        }

        /// <summary>
        /// Holds prev/next cursors
        /// </summary>
        public Cursors CursorMovement { get; set; }

        /// <summary>
        /// List of IDs returned
        /// </summary>
        public List<string> IDs { get; set; }
    }
}
