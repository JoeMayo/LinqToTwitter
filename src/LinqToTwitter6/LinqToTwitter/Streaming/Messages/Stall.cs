#nullable disable
using System.Text.Json;

namespace LinqToTwitter
{
    public class Stall
    {
        public Stall() { }
        public Stall(JsonElement stall)
        {
            var warning = stall.GetProperty("user_withheld");
            Code = warning.GetProperty("code").GetString();
            Message = warning.GetProperty("message").GetString();
            PercentFull = warning.GetProperty("percent_full").GetInt32();
        }

        public string Code { get; set; }

        public string Message { get; set; }

        public int PercentFull { get; set; }
    }
}
