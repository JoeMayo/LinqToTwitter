#nullable disable
using System.Text.Json;

namespace LinqToTwitter
{
    public class TooManyFollows
    {
        public TooManyFollows() { }
        public TooManyFollows(JsonElement warning)
        {
            var warn = warning.GetProperty("warning");
            Code = warn.GetProperty("code").GetString();
            Message = warn.GetProperty("message").GetString();
            UserID = warn.GetProperty("user_id").GetUInt64();
        }

        public string Code { get; set; }

        public string Message { get; set; }

        public ulong UserID { get; set; }
    }
}
