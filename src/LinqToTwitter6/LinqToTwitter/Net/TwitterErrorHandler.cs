using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LinqToTwitter.Common;

namespace LinqToTwitter.Net
{
    public class TwitterErrorHandler
    {
        const int TwitterApiV1 = 1;
        const int TwitterApiV2 = 2;

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
                StatusCode = HttpStatusCode.TooManyRequests,
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

            string title = error?.Title ?? string.Empty;

            throw new TwitterQueryException(title)
            {
                StatusCode = msg.StatusCode,
                ReasonPhrase = msg.ReasonPhrase,
                Title = title,
                Details = error?.Detail ?? string.Empty,
                Type = error?.Type ?? string.Empty,
                Errors = error?.Errors ?? new List<Error>()
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

        public static TwitterErrorDetails ParseTwitterErrorMessage(string responseStr)
        {
            try
            {
                var responseJson = JsonDocument.Parse(responseStr);
                var root = responseJson.RootElement;

                int apiVersion = GetTwitterApiVersion(root);

                if (apiVersion == TwitterApiV2) // version 2
                {
                    List<Error>? errors = null;
                    if (root.TryGetProperty("errors", out JsonElement errorElement))
                        errors =
                            (from error in errorElement.EnumerateArray()
                             select new Error
                             {
                                 Message = error.GetString("message"),
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
                            .ToList();

                    return new TwitterErrorDetails
                    {
                        Title = root.GetString("title"),
                        Detail = root.GetString("detail"),
                        Type = root.GetString("type"),
                        Errors = errors
                    };
                }
                else // version 1
                {
                    if (root.TryGetProperty("request", out JsonElement discard))
                    {
                        return new TwitterErrorDetails
                        {
                            Errors = new List<Error>
                            {
                                new Error
                                {
                                    Request = root.GetString("request"),
                                    Message = root.GetString("error")
                                }
                            }
                        };
                    }
                    else
                    {
                        return new TwitterErrorDetails
                        {
                            Errors =
                                (from error in root.GetProperty("errors").EnumerateArray()
                                 select new Error
                                 {
                                     Message = error.GetString("message"),
                                     Code = error.GetInt("code")
                                 })
                                .ToList()
                        };
                    }

                }
            }
            catch (Exception)
            {
                return new TwitterErrorDetails 
                { 
                    Title = 
                        "Unhandled Error - LINQ to Twitter wasn't able to parse Twitter error message. " +
                        "Please copy this message, with the Detail property contents and the query you " +
                        "were using (how to reproduce) to Joe Mayo at https://github.com/JoeMayo/LinqToTwitter/issues.",
                    Detail = responseStr 
                };
            }

        }

        static int GetTwitterApiVersion(JsonElement root)
        {
            bool hasTitle = root.TryGetProperty("title", out _);
            bool hasType = root.TryGetProperty("type", out _);

            return hasTitle && hasType ? TwitterApiV2 : TwitterApiV1;
        }
    }
}
