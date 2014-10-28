using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class UserWithheld
    {
        public UserWithheld() { }
        public UserWithheld(JsonData user)
        {
            var withheld = user.GetValue<JsonData>("user_withheld");
            UserID = withheld.GetValue<ulong>("user_id");
            var withheldCountries = withheld.GetValue<JsonData>("withheld_in_countries");
            WithheldInCountries =
                withheldCountries == null ? new List<string>() :
                (from JsonData country in withheld.GetValue<JsonData>("withheld_in_countries")
                 select country.ToString())
                .ToList();
        }

        public ulong UserID { get; set; }

        public List<string> WithheldInCountries { get; set; }
    }
}
