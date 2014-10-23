using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class FriendsList
    {
        public FriendsList() { }
        public FriendsList(JsonData friends)
        {
            Friends =
                friends == null ? new List<ulong>() :
                (from JsonData friend in friends.GetValue<JsonData>("friends")
                 select (ulong)friend)
                .ToList();
        }

        public List<ulong> Friends { get; set; }
    }
}
