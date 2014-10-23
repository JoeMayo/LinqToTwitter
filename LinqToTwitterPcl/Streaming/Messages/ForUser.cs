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
            var withheldCountries = user.GetValue<JsonData>("message");
            Friends =
                withheldCountries == null ? new List<ulong>() :
                (from JsonData friend in user.GetValue<JsonData>("friends")
                 select (ulong)friend)
                .ToList();
        }

        public ulong UserID { get; set; }

        public List<ulong> Friends { get; set; }
    }
}
