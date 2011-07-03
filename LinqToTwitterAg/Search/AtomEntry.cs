using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Holds an atom (RFC 4287) entry in object form, member of AtomFeed
    /// </summary>
    public class AtomEntry
    {
        public string ID { get; set; }

        public DateTime Published { get; set; }

        public string Alternate { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Updated { get; set; }

        public string Image { get; set; }

        public string Source { get; set; }

        public string Language { get; set; }

        public AtomAuthor Author { get; set; }

        /// <summary>
        /// Geo location of tweet
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Result type returned from twitter for this entry
        /// </summary>
        public string ResultType { get; set; }
    }
}
