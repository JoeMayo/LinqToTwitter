using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Holds an atom (RFC 4287) document in object form
    /// </summary>
    /// <remarks>
    /// This is just enough information to extract essential
    /// bits from Twitter. If you need this object to hold 
    /// more information, let me know - Joe
    /// </remarks>
    public class AtomFeed
    {
        public string ID { get; set; }

        public string Alternate { get; set; }

        public string Self { get; set; }

        public string Title { get; set; }

        public string Search { get; set; }

        public string Refresh { get; set; }

        public DateTime Updated { get; set; }

        public int ItemsPerPage { get; set; }

        public string Language { get; set; }

        public string Next { get; set; }

        public string TwitterWarning { get; set; }

        public List<AtomEntry> Entries { get; set; }
    }
}
