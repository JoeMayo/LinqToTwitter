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
    partial class TwitterErrorHandler
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
                StatusCode = HttpStatusCode.SeeOther,
                ReasonPhrase = msg.ReasonPhrase + " (HTTP 429 - Too Many Requests)",
                Title = error.Title,
                Details = error.Detail,
                Type = error.Type,
                Errors = error.Errors
            };
        }

        internal static void BuildAndThrowTwitterQueryException(string responseStr, HttpResponseMessage msg)
        {
            TwitterErrorDetails error = ParseTwitterErrorMessage(responseStr);

            throw new TwitterQueryException(error.Title)
            {
                StatusCode = msg.StatusCode,
                ReasonPhrase = msg.ReasonPhrase,
                Title = error.Title,
                Details = error.Detail,
                Type = error.Type,
                Errors = error.Errors
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
                StatusCode = HttpStatusCode.Unauthorized,
                ReasonPhrase = msg.ReasonPhrase,
                Title = error.Title,
                Details = error.Detail,
                Type = error.Type,
                Errors = error.Errors
            };
        }

        internal static TwitterErrorDetails ParseTwitterErrorMessage(string responseStr)
        {
            if (responseStr.StartsWith("{"))
            {
                var responseJson = JsonDocument.Parse(responseStr);
                var root = responseJson.RootElement;

                if (IsErrorFormatRecognized(root))
                {
                    return new TwitterErrorDetails
                    {
                        Title = root.GetProperty("title").GetString(),
                        Detail = root.GetProperty("detail").GetString(),
                        Type = root.GetProperty("type").GetString(),
                        Errors =
                            (from error in root.GetProperty("errors").EnumerateArray()
                             select new Error
                             {
                                 Message = error.GetProperty("message").ToString(),
                                 Parameters =
                                    (from parm in error.GetProperty("parameters").EnumerateObject()
                                     let vals =
                                     (from val in parm.Value.EnumerateArray()
                                      select val.GetString())
                                     .ToArray()
                                     select new { parm.Name, vals })
                                    .ToDictionary(
                                        key => key.Name,
                                        val => val.vals)
                             })
                            .ToArray()
                    };
                }
            }

            return new TwitterErrorDetails { Detail = responseStr };
        }

        static bool IsErrorFormatRecognized(JsonElement root)
        {
            return root.TryGetProperty("errors", out JsonElement errors);
        }
    }
}
