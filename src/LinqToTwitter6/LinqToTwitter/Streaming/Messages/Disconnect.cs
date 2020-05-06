using System.Text.Json;

namespace LinqToTwitter
{
    public class Disconnect
    {
        public Disconnect() { }
        public Disconnect(JsonElement json)
        {
            var disconnect = json.GetProperty("disconnect");
            Code = disconnect.GetProperty("code").GetInt32();
            StreamName = disconnect.GetProperty("stream_name").GetString();
            Reason = disconnect.GetProperty("reason").GetString();
        }

        public int Code { get; set; }

        public string StreamName { get; set; }

        public string Reason { get; set; }
    }
}
