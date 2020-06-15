#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LinqToTwitter.Common;

namespace LinqToTwitter.Net
{
    class TwitterErrorHandler
    {
        public static async Task ThrowIfErrorAsync(HttpResponseMessage msg)
        {
            const int TooManyRequests = 429;

            if ((int)msg.StatusCode < 400) return;

            switch (msg.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    await HandleUnauthorizedAsync(msg).ConfigureAwait(false);
                    break;
                default:
                    switch ((int)msg.StatusCode)
                    {
                        case TooManyRequests:
                            await HandleTooManyRequestsAsync(msg).ConfigureAwait(false);
                            break;
                        default:
                            await HandleGenericErrorAsync(msg).ConfigureAwait(false);
                            break;
                    }
                    break;
            }
        }

        internal static async Task HandleGenericErrorAsync(HttpResponseMessage msg)
        {
            string responseStr = await msg.Content.ReadAsStringAsync().ConfigureAwait(false);

            BuildAndThrowTwitterQueryException(responseStr, msg);
        }

        internal static async Task HandleTooManyRequestsAsync(HttpResponseMessage msg)
        {
            string responseStr = await msg.Content.ReadAsStringAsync().ConfigureAwait(false);

            TwitterErrorDetails error = ParseTwitterErrorMessage(responseStr);

            string message = error.Detail + " - Please visit the LINQ to Twitter FAQ (at the HelpLink) for help on resolving this error.";

            throw new TwitterQueryException(message)
            {
                HelpLink = L2TKeys.FaqHelpUrl,
                Type = error.Type,
                StatusCode = HttpStatusCode.SeeOther,
                ReasonPhrase = msg.ReasonPhrase + " (HTTP 429 - Too Many Requests)"
            };
        }

        internal static void BuildAndThrowTwitterQueryException(string responseStr, HttpResponseMessage msg)
        {
            TwitterErrorDetails error = ParseTwitterErrorMessage(responseStr);

            throw new TwitterQueryException(error.Title)
            {
                Type = error.Type,
                StatusCode = msg.StatusCode,
                ReasonPhrase = msg.ReasonPhrase
            };
        }

        internal async static Task HandleUnauthorizedAsync(HttpResponseMessage msg)
        {
            string responseStr = await msg.Content.ReadAsStringAsync().ConfigureAwait(false);

            TwitterErrorDetails error = ParseTwitterErrorMessage(responseStr);

            string message = error.Detail + " - Please visit the LINQ to Twitter FAQ (at the HelpLink) for help on resolving this error.";

            throw new TwitterQueryException(message)
            {
                HelpLink = L2TKeys.FaqHelpUrl,
                Type = error.Type,
                StatusCode = HttpStatusCode.Unauthorized,
                ReasonPhrase = msg.ReasonPhrase
            };
        }

        internal static TwitterErrorDetails ParseTwitterErrorMessage(string responseStr)
        {
            if (responseStr.StartsWith("{"))
            {
                var responseJson = JsonDocument.Parse(responseStr);
                var root = responseJson.RootElement;

                if (root.TryGetProperty("errors", out JsonElement errors))
                {
                    JsonElement errorElement = errors.EnumerateArray().FirstOrDefault();

                    return new TwitterErrorDetails
                    {
                        Title = errorElement.GetProperty("title").GetString(),
                        Detail = errorElement.GetProperty("detail").GetString(),
                        Type = errorElement.GetProperty("type").GetString(),
                        Errors =
                            (from error in errorElement.GetProperty("errors").EnumerateArray()
                             select new Error
                             {
                                 Message = error.GetProperty("message").ToString(),
                                 Parameters =
                                    (from pram in error.GetProperty("parameters").EnumerateObject()
                                     from key in pram.EnumerateObject()
                                     select pram)
                                    .ToDictionary(
                                        key => key)
                             })
                            .ToArray()
                    };
                }
            }

            return new TwitterErrorDetails { Detail = responseStr };
        }

// TwitterErrorDetails
//{
//	"errors": [
//		{
//			"parameters": {
//				"query": []
//          },
//			"message": "Request parameter `query` can not be empty"
//		},
//		{
//			"parameters": {
//				"q": [
//					"LINQ%20to%20Twitter"
//				]
//			},
//			"message": "[q] is not one of [query,start_time,end_time,since_id,until_id,max_results,next_token,expansions,tweet.fields,media.fields,poll.fields,place.fields,user.fields]"
//		}
//	],
//	"title": "Invalid Request",
//	"detail": "One or more parameters to your request was invalid.",
//	"type": "https://api.twitter.com/labs/2/problems/invalid-request"
//}

        public class TwitterErrorDetails
        {
            public Error[] Errors { get; set; }
            public string Title { get; set; }
            public string Detail { get; set; }
            public string Type { get; set; }
        }

        public class Error
        {
            public Dictionary<string, string[]> Parameters { get; set; }
            public string Message { get; set; }
        }
    }
}
