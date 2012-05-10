using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public class Help
    {
        /// <summary>
        /// Help Type (Test, Configuration, or Languages)
        /// </summary>
        public HelpType Type { get; set; }

        /// <summary>
        /// Will be true if help Test succeeds
        /// </summary>
        public bool OK { get; set; }

        /// <summary>
        /// Populated for Help Configuration query
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// List of languages, codes, and statuses
        /// </summary>
        public List<Language> Languages { get; set; }
    }
}
