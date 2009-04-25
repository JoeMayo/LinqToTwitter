using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Holds an atom (RFC 4287) author in object form
    /// </summary>
    public class AtomAuthor
    {
        public string Name { get; set; }

        public string URI { get; set; }
    }
}
