using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Direct Messages
    /// </summary>
    public class DirectMessageRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// Type of Direct Message
        /// </summary>
        private DirectMessageType Type { get; set; }

        /// <summary>
        /// since this message ID
        /// </summary>
        private ulong SinceID { get; set; }

        /// <summary>
        /// max ID to return
        /// </summary>
        private ulong MaxID { get; set; }

        /// <summary>
        /// page number to return
        /// </summary>
        private int Page { get; set; }

        /// <summary>
        /// number of items to return (works for SentBy and SentTo
        /// </summary>
        private int Count { get; set; }

        /// <summary>
        /// ID of DM
        /// </summary>
        private ulong ID { get; set; }

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
                       "ID"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildURL(Dictionary<string, string> parameters)
        {
            const string typeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", typeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<DirectMessageType>(parameters["Type"]);

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

        private Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            const string idParam = "ID";
            if (parameters == null || !parameters.ContainsKey("ID"))
                throw new ArgumentNullException(idParam, "ID is required.");

            ID = ulong.Parse(parameters["ID"]);

            var url = BuildUrlHelper.TransformIDUrl(parameters, "direct_messages/show.json");
            return new Request(BaseUrl + url);
        }

        /// <summary>
        /// builds an url for getting a list of direct message sent to a user
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private Request BuildSentToUrl(Dictionary<string, string> parameters)
        {
            return BuildSentUrlParameters(parameters, "direct_messages.json");
        }

        /// <summary>
        /// builds an url for getting a list of direct message sent by a user
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        private Request BuildSentByUrl(Dictionary<string, string> parameters)
        {
            return BuildSentUrlParameters(parameters, "direct_messages/sent.json");
        }

        /// <summary>
        /// common code for building parameter list for both sent by and sent to urls
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <param name="url">url to start with</param>
        /// <returns>new url with parameters</returns>
        private Request BuildSentUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

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

            return req;
        }

        /// <summary>
        /// Transforms twitter response into List of Blocks objects
        /// </summary>
        /// <param name="responseJson">JSON with Twitter response</param>
        /// <returns>List of DirectMessage</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrEmpty(responseJson)) return new List<T>();

            var dmJson = JsonMapper.ToObject(responseJson);

            var dmList =
                from JsonData dm in dmJson
                select new DirectMessage(dm)
                {
                    Type = Type,
                    SinceID = SinceID,
                    MaxID = MaxID,
                    Page = Page,
                    Count = Count
                };

            return dmList.OfType<T>().ToList();
        }
    }
}
