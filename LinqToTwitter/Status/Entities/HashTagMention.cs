using System;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Hash tag mention
    /// </summary>
    /// <example>#linqtotwitter</example>
    [Serializable]
    public class HashTagMention : MentionBase
    {
        /// <summary>
        /// Create HashTagMention out of the XElement
        /// </summary>
        /// <param name="element">the entry node</param>
        /// <returns>HashTagMention</returns>
        public static HashTagMention FromXElement(XElement element)
        {
            var mention = new HashTagMention { Tag = element.Value };

            mention.SetStartEndValues(element);
            return mention;
        }

        /// <summary>
        /// Tag name without the # sign
        /// </summary>
        public string Tag { get; set; }
    }
}
