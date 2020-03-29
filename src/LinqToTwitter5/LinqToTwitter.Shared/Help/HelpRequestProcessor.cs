using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Common;
using LitJson;

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
        public string BaseUrl { get; set; }

        /// <summary>
        /// Type of Help request (Test, Configuration, or Languages)
        /// </summary>
        public HelpType Type { get; set; }

        /// <summary>
        /// Comma-separated list of resources for rate limit status request (setting to null returns all)
        /// </summary>
        internal string Resources { get; set; }

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
                case HelpType.Configuration:
                    return new Request(BaseUrl + "help/configuration.json");
                case HelpType.Languages:
                    return new Request(BaseUrl + "help/languages.json");
                case HelpType.Privacy:
                    return new Request(BaseUrl + "help/privacy.json");
                case HelpType.RateLimits:
                    return BuildRateLimitsUrl(parameters);
                case HelpType.Tos:
                    return new Request(BaseUrl + "help/tos.json");
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        private Request BuildRateLimitsUrl(Dictionary<string, string> parameters)
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
            JsonData helpJson = JsonMapper.ToObject(responseJson);

            switch (Type)
            {
                case HelpType.Configuration:
                    help = HandleHelpConfiguration(helpJson);
                    break;
                case HelpType.Languages:
                    help = HandleHelpLanguages(helpJson);
                    break;
                case HelpType.RateLimits:
                    help = HandleHelpRateLimits(helpJson);
                    break;
                case HelpType.Privacy:
                    help = new Help 
                    { 
                        Type = HelpType.Privacy,
                        Policies = helpJson.GetValue<string>("privacy") 
                    };
                    break;
                case HelpType.Tos:
                    help = new Help 
                    { 
                        Type = HelpType.Tos,
                        Policies = helpJson.GetValue<string>("tos") 
                    };
                    break;
                default:
                    help = new Help();
                    break;
            }

            var helpList = new List<Help> { help };

            return helpList.OfType<T>().ToList();
        }

        Help HandleHelpConfiguration(JsonData helpJson)
        {
            var photoSizeDict = helpJson.GetValue<JsonData>("photo_sizes") as IDictionary<string, JsonData>;

            return new Help
            {
                Type = HelpType.Configuration,
                Configuration = new Configuration
                {
                    CharactersReservedPerMedia = helpJson.GetValue<int>("characters_reserved_per_media"),
                    PhotoSizes =
                        (from string key in photoSizeDict.Keys
                         let photoSize = photoSizeDict[key]
                         select new PhotoSize
                         {
                             Type = key,
                             Height = photoSize.GetValue<int>("h"),
                             Width = photoSize.GetValue<int>("w"),
                             Resize = photoSize.GetValue<string>("resize")
                         })
                        .ToList(),
                    ShortUrlLength = helpJson.GetValue<int>("short_url_length"),
                    PhotoSizeLimit = helpJson.GetValue<int>("photo_size_limit"),
                    NonUserNamePaths =
                        (from JsonData path in helpJson.GetValue<JsonData>("non_username_paths")
                         select path.ToString())
                        .ToList(),
                    MaxMediaPerUpload = helpJson.GetValue<int>("max_media_per_upload"),
                    ShortUrlLengthHttps = helpJson.GetValue<int>("short_url_length_https")
                }
            };
        }

        Help HandleHelpLanguages(JsonData helpJson)
        {
            return new Help
            {
                Type = HelpType.Languages,
                Languages =
                    (from JsonData lang in helpJson
                     select new Language
                     {
                         Code = lang.GetValue<string>("code"),
                         Name = lang.GetValue<string>("name"),
                         Status = lang.GetValue<string>("status")
                     })
                    .ToList()
            };
        }

        Help HandleHelpRateLimits(JsonData helpJson)
        {
            var context = helpJson.GetValue<JsonData>("rate_limit_context");
            var resources = helpJson.GetValue<JsonData>("resources") as IDictionary<string, JsonData>;

            return new Help
            {
                Type = HelpType.RateLimits,
                Resources = Resources,
                RateLimitAccountContext = context.GetValue<string>("access_token"),
                RateLimits = 
                    (from key in resources.Keys
                     let category = resources[key] as IDictionary<string, JsonData>
                     select new
                     {
                         Key = key,
                         Value =
                            (from cat in category.Keys
                             let limit = category[cat]
                             select new RateLimits
                             {
                                 Resource = cat,
                                 Limit = limit.GetValue<int>("limit"),
                                 Remaining = limit.GetValue<int>("remaining"),
                                 Reset = limit.GetValue<ulong>("reset")
                             })
                            .ToList()
                     })
                    .ToDictionary(
                        key => key.Key,
                        val => val.Value)
            };
        }
    }
}
