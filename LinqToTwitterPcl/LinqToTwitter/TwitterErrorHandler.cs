using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    class TwitterErrorHandler
    {
        public static async Task ThrowIfErrorAsync(HttpResponseMessage msg)
        {
            string responseStr = await msg.Content.ReadAsStringAsync();

            if (!responseStr.StartsWith("{")) return;

            JsonData responseJson = JsonMapper.ToObject(responseStr);

            var errors = responseJson.GetValue<JsonData>("errors");
            if (errors != null && errors.Count > 0)
            {
                var error = errors[0];
                throw new TwitterQueryException(error.GetValue<string>("message"))
                {
                    ErrorCode = error.GetValue<int>("code"),
                    StatusCode = msg.StatusCode,
                    ReasonPhrase = msg.ReasonPhrase
                };
            }
        }
    }
}
