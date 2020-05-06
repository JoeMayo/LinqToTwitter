using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LinqToTwitter
{
    public class UserWithheld
    {
        public UserWithheld() { }
        public UserWithheld(JsonElement user)
        {
            var withheld = user.GetProperty("user_withheld");
            UserID = withheld.GetProperty("user_id").GetUInt64();

            if (withheld.TryGetProperty("withheld_in_countries", out JsonElement withheldCountries))
                WithheldInCountries =
                    (from country in withheldCountries.EnumerateArray()
                     select country.GetString())
                    .ToList();
            else
                WithheldInCountries = new List<string>();
        }

        public ulong UserID { get; set; }

        public List<string> WithheldInCountries { get; set; }
    }
}
