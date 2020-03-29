using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Processes Twitter Direct Messages.
    /// </summary>
    public class DirectMessageRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// Type of Direct Message
        /// </summary>
        internal DirectMessageType Type { get; set; }

        /// <summary>
        /// since this message ID
        /// </summary>
        internal ulong SinceID { get; set; }

        /// <summary>
        /// max ID to return
        /// </summary>
        internal ulong MaxID { get; set; }

        /// <summary>
        /// page number to return
        /// </summary>
        internal int Page { get; set; }

        /// <summary>
        /// number of items to return (works for SentBy and SentTo
        /// </summary>
        internal int Count { get; set; }

        /// <summary>
        /// ID of DM
        /// </summary>
        internal ulong ID { get; set; }

        /// <summary>
        /// Include entities in tweets
        /// </summary>
        internal bool IncludeEntities { get; set; }

        /// <summary>
        /// Don't include statuses in response
        /// </summary>
        internal bool SkipStatus { get; set; }

        /// <summary>
        /// Returns text beyond 140 characters, if present
        /// </summary>
        public bool FullText { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<DirectMessage>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "SinceID",
                       "MaxID",
                       "Page",
                       "Count",
                       "ID",
                       "IncludeEntities",
                       "SkipStatus",
                       "FullText"
                   });

            Dictionary<string, string> parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<DirectMessageType>(parameters["Type"]);

            switch (Type)
            {
                case DirectMessageType.SentBy:
                    return BuildSentByUrl(parameters);
                case DirectMessageType.SentTo:
                    return BuildSentToUrl(parameters);
                case DirectMessageType.Show:
                    return BuildShowUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            const string IdParam = "ID";
            if (parameters == null || !parameters.ContainsKey(IdParam))
                throw new ArgumentNullException(IdParam, "ID is required.");

            var req = new Request(BaseUrl + "direct_messages/show.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

            ID = ulong.Parse(parameters[IdParam]);
            urlParams.Add(new QueryParameter("id", ID.ToString()));

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("FullText"))
            {
                FullText = bool.Parse(parameters["FullText"]);
                urlParams.Add(new QueryParameter("full_text", parameters["FullText"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// builds an url for getting a list of direct message sent to a user
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        Request BuildSentToUrl(Dictionary<string, string> parameters)
        {
            return BuildSentUrlParameters(parameters, "direct_messages.json");
        }

        /// <summary>
        /// builds an url for getting a list of direct message sent by a user
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        Request BuildSentByUrl(Dictionary<string, string> parameters)
        {
            return BuildSentUrlParameters(parameters, "direct_messages/sent.json");
        }

        /// <summary>
        /// Common code for building parameter list for both sent by and sent to urls.
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <param name="url">url to start with</param>
        /// <returns>new url with parameters</returns>
        Request BuildSentUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            IList<QueryParameter> urlParams = req.RequestParameters;

            if (parameters == null)
                return req;

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add(new QueryParameter("since_id", SinceID.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add(new QueryParameter("max_id", MaxID.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add(new QueryParameter("page", Page.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", Count.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", SkipStatus.ToString().ToLower()));
            }

            if (parameters.ContainsKey("FullText"))
            {
                FullText = bool.Parse(parameters["FullText"]);
                urlParams.Add(new QueryParameter("full_text", parameters["FullText"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// Transforms twitter response into List of Blocks objects
        /// </summary>
        /// <param name="responseJson">JSON with Twitter response</param>
        /// <returns>List of DirectMessage</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            JsonData dmJson = JsonMapper.ToObject(responseJson);

            IEnumerable<DirectMessage> dmList;

            switch (Type)
            {
                case DirectMessageType.SentBy:
                case DirectMessageType.SentTo:
                    dmList = HandleMultipleDirectMessages(dmJson);
                    break;
                case DirectMessageType.Show:
                    dmList = HandleSingleDirectMessage(dmJson);
                    break;
                default:
                    dmList = new List<DirectMessage>();
                    break;
            }

            return dmList.OfType<T>().ToList();
        }
  
        IEnumerable<DirectMessage> HandleMultipleDirectMessages(JsonData dmJson)
        {
            IEnumerable<DirectMessage> dmList =
                from JsonData dm in dmJson
                select new DirectMessage(dm)
                {
                    Type = Type,
                    SinceID = SinceID,
                    MaxID = MaxID,
                    Page = Page,
                    Count = Count,
                    ID = ID,
                    IncludeEntities = IncludeEntities,
                    SkipStatus = SkipStatus,
                    FullText = FullText
                };

            return dmList;
        }

        IEnumerable<DirectMessage> HandleSingleDirectMessage(JsonData dmJson)
        {
            return new List<DirectMessage> 
            { 
                new DirectMessage(dmJson) 
                {
                    Type = Type,
                    SinceID = SinceID,
                    MaxID = MaxID,
                    Page = Page,
                    Count = Count,
                    ID = ID,
                    IncludeEntities = IncludeEntities,
                    SkipStatus = SkipStatus,
                    FullText = FullText
                }
            };
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonData dmJson = JsonMapper.ToObject(responseJson);

            var dm = new DirectMessage(dmJson);

            return dm.ItemCast(default(T));
        }
    }
}
