using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class ForUser
    {
        public ForUser() { }
        public ForUser(JsonData user)
        {
            UserID = user.GetValue<ulong>("for_user");
            var message = user.GetValue<JsonData>("message");
            var friends = message.GetValue<JsonData>("friends");
            Friends =
                friends == null ? new List<ulong>() :
                (from JsonData friend in friends
                 select (ulong)friend)
                .ToList();
        }

        public ulong UserID { get; set; }

        public List<ulong> Friends { get; set; }
    }
}
