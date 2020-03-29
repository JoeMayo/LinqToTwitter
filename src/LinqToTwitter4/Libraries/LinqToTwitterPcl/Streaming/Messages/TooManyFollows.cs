using System;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class TooManyFollows
    {
        public TooManyFollows() { }
        public TooManyFollows(JsonData warning)
        {
            var warn = warning.GetValue<JsonData>("warning");
            Code = warn.GetValue<string>("code");
            Message = warn.GetValue<string>("message");
            UserID = warn.GetValue<ulong>("user_id");
        }

        public string Code { get; set; }

        public string Message { get; set; }

        public ulong UserID { get; set; }
    }
}
