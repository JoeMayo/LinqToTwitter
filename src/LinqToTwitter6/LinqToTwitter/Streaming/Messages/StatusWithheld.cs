using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LinqToTwitter
{
    public class StatusWithheld
    {
        public StatusWithheld() { }
        public StatusWithheld(JsonElement status)
        {
            var withheld = status.GetProperty("status_withheld");
            StatusID = withheld.GetProperty("id").GetUInt64();
            UserID = withheld.GetProperty("user_id").GetUInt64();

            if (withheld.TryGetProperty("withheld_in_countries", out JsonElement withheldInCountries))
                WithheldInCountries =
                    (from country in withheldInCountries.EnumerateArray()
                     select country.GetString())
                    .ToList();
            else
                WithheldInCountries = new List<string>();
        }

        public ulong StatusID { get; set; }

        public ulong UserID { get; set; }

        public List<string> WithheldInCountries { get; set; }
    }
}
