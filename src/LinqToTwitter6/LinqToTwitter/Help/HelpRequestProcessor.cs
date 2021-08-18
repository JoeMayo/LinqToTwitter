using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// Used for Help queries
    /// </summary>
    public class HelpRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// Type of Help request (Test, Configuration, or Languages)
        /// </summary>
        public HelpType Type { get; set; }

        /// <summary>
        /// Comma-separated list of resources for rate limit status request (setting to null returns all)
        /// </summary>
        internal string? Resources { get; set; }

        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return new ParameterFinder<Help>(
               lambdaExpression.Body,
               new List<string> { 
                   "Type",
                   "Resources"
               })
               .Parameters;
        }

        public Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<HelpType>(parameters["Type"]);

            switch (Type)
            {
                case HelpType.Languages:
                    return new Request(BaseUrl + "help/languages.json");
                case HelpType.RateLimits:
                    return BuildRateLimitsUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        Request BuildRateLimitsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "application/rate_limit_status.json");

            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Resources"))
            {
                Resources = parameters["Resources"];
                urlParams.Add(new QueryParameter("resources", Resources.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// return response from help request
        /// </summary>
        /// <param name="responseJson">response from twitter</param>
        /// <returns>true</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            Help help;
            JsonElement helpJson = JsonDocument.Parse(responseJson).RootElement;

            switch (Type)
            {
                case HelpType.Languages:
                    help = HandleHelpLanguages(helpJson);
                    break;
                case HelpType.RateLimits:
                    help = HandleHelpRateLimits(helpJson);
                    break;
                default:
                    help = new Help();
                    break;
            }

            var helpList = new List<Help> { help };

            return helpList.OfType<T>().ToList();
        }

        Help HandleHelpLanguages(JsonElement helpJson)
        {
            return new Help
            {
                Type = HelpType.Languages,
                Languages =
                    (from lang in helpJson.EnumerateArray()
                     select new Language
                     {
                         Code = lang.GetString("code"),
                         Name = lang.GetString("name"),
                         Status = lang.GetString("status")
                     })
                    .ToList()
            };
        }

        Help HandleHelpRateLimits(JsonElement helpJson)
        {
            if (helpJson.TryGetProperty("rate_limit_context", out JsonElement context) &&
                helpJson.TryGetProperty("resources", out JsonElement resources))
                return new Help
                {
                    Type = HelpType.RateLimits,
                    Resources = Resources,
                    RateLimitAccountContext = context.GetString("access_token"),
                    RateLimits =
                        (from key in resources.EnumerateObject()
                         let category = resources.GetProperty(key.Name)
                         select new
                         {
                             Key = key,
                             Value =
                                (from cat in category.EnumerateObject()
                                 let limit = category.GetProperty(cat.Name)
                                 select new RateLimits
                                 {
                                     Resource = cat.Name,
                                     Limit = limit.GetInt("limit"),
                                     Remaining = limit.GetInt("remaining"),
                                     Reset = limit.GetUlong("reset")
                                 })
                                .ToList()
                         })
                        .ToDictionary(
                            key => key.Key.Name,
                            val => val.Value)
                };
            else
                return new Help();
        }
    }
}
