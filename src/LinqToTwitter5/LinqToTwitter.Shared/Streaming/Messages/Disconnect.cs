using System;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class Disconnect
    {
        public Disconnect() { }
        public Disconnect(JsonData json)
        {
            var disconnect = json.GetValue<JsonData>("disconnect");
            Code = disconnect.GetValue<int>("code");
            StreamName = disconnect.GetValue<string>("stream_name");
            Reason = disconnect.GetValue<string>("reason");
        }

        public int Code { get; set; }

        public string StreamName { get; set; }

        public string Reason { get; set; }
    }
}
