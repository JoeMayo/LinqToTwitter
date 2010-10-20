using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Twitter's legal information
    /// </summary>
    [Serializable]
    public class Legal
    {
        /// <summary>
        /// Type of legal info (Privacy or TOS)
        /// </summary>
        public LegalType Type { get; set; }

        /// <summary>
        /// Text from Twitter's Policy
        /// </summary>
        public string Text { get; set; }
    }
}
