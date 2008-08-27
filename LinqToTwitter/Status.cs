/***********************************************************
 * Credits:
 * 
 * Written by: Joe Mayo, 8/26/08
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public class Status
    {
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ID { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public bool Truncated { get; set; }
        public string InReplyToStatusID { get; set; }
        public string InReplyToUserID { get; set; }
        public bool Favorited { get; set; }
        public User User { get; set; }
    }
}
