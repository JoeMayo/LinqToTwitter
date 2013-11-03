using System;
using System.Linq;
using System.Net;
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
            // TODO: research proper handling of 304

            if ((int)msg.StatusCode < 400) return;

            switch (msg.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    await HandleUnauthorizedAsync(msg);
                    break;
                default:
                    await HandleGenericErrorAsync(msg);
                    break;
            } 
        }
  
        static async Task HandleGenericErrorAsync(HttpResponseMessage msg)
        {
            string responseStr = await msg.Content.ReadAsStringAsync();

            try
            {
                HandleTwitterError(responseStr, msg);
            }
            catch (Exception)
            {
                throw new TwitterQueryException(
                    "Unknown error - please report issue if reproducible.") 
                { 
                    StatusCode = msg.StatusCode
                };
            }

            throw new TwitterQueryException(responseStr)
            {
                StatusCode = msg.StatusCode
            };
        }
  
        static void HandleTwitterError(string responseStr, HttpResponseMessage msg)
        {
            if (!responseStr.StartsWith("{"))
            {
                JsonData responseJson = JsonMapper.ToObject(responseStr);

                var errors = responseJson.GetValue<JsonData>("errors");
                if (errors != null && errors.Count > 0)
                {
                    var error = errors[0];
                    error.GetValue<string>("message");

                    throw new TwitterQueryException(error.GetValue<string>("message"))
                    {
                        ErrorCode = error.GetValue<int>("code"),
                        StatusCode = msg.StatusCode,
                        ReasonPhrase = msg.ReasonPhrase
                    };
                }
            }
        }
  
        async static Task HandleUnauthorizedAsync(HttpResponseMessage msg)
        {
            string responseStr = await msg.Content.ReadAsStringAsync();

            string message = responseStr + " - Please visit the LINQ to Twitter FAQ (at the HelpLink) for help on resolving this error.";

            throw new TwitterQueryException(message)
                {
                    HelpLink = "https://linqtotwitter.codeplex.com/wikipage?title=LINQ%20to%20Twitter%20FAQ",
                    StatusCode = HttpStatusCode.Unauthorized
                };
        }
    }
}
