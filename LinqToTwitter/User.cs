/***********************************************************
 * Credits:
 * 
 * Created By: Joe Mayo, 8/26/08
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public class User
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ProfileImageUrl { get; set; }
        public string URL { get; set; }
        public bool Protected { get; set; }
        public int FollowersCount { get; set; }
    }
}
