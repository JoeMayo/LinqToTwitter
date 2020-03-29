using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    public class StatusWithheld
    {
        public StatusWithheld() { }
        public StatusWithheld(JsonData status)
        {
            var withheld = status.GetValue<JsonData>("status_withheld");
            StatusID = withheld.GetValue<ulong>("id");
            UserID = withheld.GetValue<ulong>("user_id");
            var withheldCountries = withheld.GetValue<JsonData>("withheld_in_countries");
            WithheldInCountries =
                withheldCountries == null ? new List<string>() :
                (from JsonData country in withheld.GetValue<JsonData>("withheld_in_countries")
                 select country.ToString())
                .ToList();
        }

        public ulong StatusID { get; set; }

        public ulong UserID { get; set; }

        public List<string> WithheldInCountries { get; set; }
    }
}
