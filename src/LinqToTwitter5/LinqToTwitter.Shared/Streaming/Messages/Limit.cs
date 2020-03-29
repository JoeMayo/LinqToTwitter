using System;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class Limit
    {
        public Limit() { }
        public Limit(JsonData json)
        {
            var scrub = json.GetValue<JsonData>("limit");
            Track = scrub.GetValue<ulong>("track");
        }

        public ulong Track { get; set; }
    }
}
