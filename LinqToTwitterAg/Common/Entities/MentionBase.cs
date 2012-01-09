using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Base for all entity mentions
    /// </summary>
    public abstract class MentionBase
    {
        internal void SetStartEndValues(XElement element)
        {
            Start = Int32.Parse(element.Attribute("start").Value);
            End = Int32.Parse(element.Attribute("end").Value);
        }

        /// <summary>
        /// Start of the mention in the tweet
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// End of the mention in the tweet
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Processes an XElement collection of entity mentions
        /// </summary>
        /// <typeparam name="TMention">The type of mention (must derive from MentionBase)</typeparam>
        /// <param name="entities">The entities node</param>
        /// <param name="collectionName">The name of the node-collection (e.g. hashtags)</param>
        /// <param name="elementName">The name of the element node entry (e.g. hashtag)</param>
        /// <param name="requiresStart">True if the collection should be filtered requiring a start attribute</param>
        /// <param name="createMention">A delegate that can create a <typeparamref name="TMention"/> from the XElement</param>
        /// <returns>A IEnumerable if <typeparamref name="TMention"/> items</returns>
        public static IEnumerable<TMention> ProcessMentions<TMention>(XElement entities,
                string collectionName,
                string elementName,
                bool requiresStart,
                Func<XElement, TMention> createMention)
            where TMention : MentionBase
        {
            var mentions = entities.Element(collectionName);

            if (mentions != null && mentions.HasElements)
            {
                var entries = mentions.Descendants(elementName);

                if (requiresStart)
                    entries = entries.Where(x => x.Attribute("start") != null);

                return entries.Select(mention => createMention(mention));
            }

            return Enumerable.Empty<TMention>();
        }
    }
}
