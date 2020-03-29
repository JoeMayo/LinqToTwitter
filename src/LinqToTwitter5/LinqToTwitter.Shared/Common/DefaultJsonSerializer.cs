using Newtonsoft.Json;

namespace LinqToTwitter.Shared.Common
{
    public class DefaultJsonSerializer : JsonSerializerSettings
    {
        public DefaultJsonSerializer()
        {
            NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
