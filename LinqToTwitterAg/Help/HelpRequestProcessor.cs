using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// used for processing help messages - we only use the request processing part
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

        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return new ParameterFinder<Help>(
               lambdaExpression.Body,
               new List<string> { 
                   "Type"
               })
               .Parameters;
        }

        public Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<HelpType>(parameters["Type"]);

            switch (Type)
            {
                case HelpType.Test:
                    return new Request(BaseUrl + "help/test.json");
                case HelpType.Configuration:
                    return new Request(BaseUrl + "help/configuration.json");
                case HelpType.Languages:
                    return new Request(BaseUrl + "help/languages.json");
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// return response from help request
        /// </summary>
        /// <param name="responseJson">response from twitter</param>
        /// <returns>true</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            Help help;

            switch (Type)
            {
                case HelpType.Test:
                    help = HandleHelpTest(responseJson);
                    break;
                case HelpType.Configuration:
                    help = HandleHelpConfiguration(responseJson);
                    break;
                case HelpType.Languages:
                    help = HandleHelpLanguages(responseJson);
                    break;
                default:
                    help = new Help();
                    break;
            }

            var helpList = new List<Help> { help };

            return helpList.OfType<T>().ToList();
        }
  
        Help HandleHelpTest(string responseJson)
        {
            const string TestResponse = "ok";

            return new Help
            {
                Type = HelpType.Test,
                OK = responseJson == TestResponse
            };
        }

        Help HandleHelpConfiguration(string responseJson)
        {
            JsonData helpJson = JsonMapper.ToObject(responseJson);

            var photoSizeDict = helpJson.GetValue<JsonData>("photo_sizes") as IDictionary<string, JsonData>;

            return new Help
            {
                Type = HelpType.Configuration,
                Configuration = new Configuration
                {
                    CharactersReservedPerMedia = helpJson.GetValue<int>("characters_reserved_per_media"),
                    PhotoSizes =
                        (from string key in photoSizeDict.Keys
                         let photoSize = photoSizeDict[key] as JsonData
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

        Help HandleHelpLanguages(string responseJson)
        {
            JsonData helpJson = JsonMapper.ToObject(responseJson);

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
    }
}
