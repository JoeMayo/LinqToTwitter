using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// Processes Twitter Direct Messages.
    /// </summary>
    public class DirectMessageEventsRequestProcessor<T> :
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
        /// Type of Direct Message Events
        /// </summary>
        public DirectMessageEventsType Type { get; set; }

        /// <summary>
        /// Number of items to return for a single page.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Supports paging through results on List queries
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// ID of DM
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<DirectMessageEvents>(
                   lambdaExpression.Body,
                   new List<string> { 
                       nameof(Type),
                       nameof(Count),
                       nameof(ID),
                       nameof(Cursor)
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
            if (parameters == null || !parameters.ContainsKey(nameof(Type)))
                throw new ArgumentException($"You must set {nameof(Type)}.", nameof(Type));

            Type = RequestProcessorHelper.ParseEnum<DirectMessageEventsType>(parameters[nameof(Type)]);

            switch (Type)
            {
                case DirectMessageEventsType.List:
                    return BuildListUrl(parameters);
                case DirectMessageEventsType.Show:
                    return BuildShowUrl(parameters);
                default:
                    throw new InvalidOperationException(
                        $"Didn't recognize '{Type}' for {nameof(Type)} parameter in DirectMessageEventsRequestProcessor.BuildUrl.");
            }
        }

        Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(nameof(ID)))
                throw new ArgumentNullException(nameof(ID), $"{nameof(ID)} is required.");

            var req = new Request(BaseUrl + "direct_messages/events/show.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

            ID = ulong.Parse(parameters[nameof(ID)]);
            urlParams.Add(new QueryParameter("id", ID.ToString()));

            return req;
        }

        /// <summary>
        /// builds an url for getting a list of direct messages
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        Request BuildListUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "direct_messages/events/list.json");
            IList <QueryParameter> urlParams = req.RequestParameters;

            if (parameters == null)
                return req;

            if (parameters.ContainsKey(nameof(Count)))
            {
                Count = int.Parse(parameters[nameof(Count)]);
                urlParams.Add(new QueryParameter("count", Count.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey(nameof(Cursor)))
            {
                Cursor = parameters[nameof(Cursor)];
                urlParams.Add(new QueryParameter("cursor", parameters[nameof(Cursor)]));
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
            if (string.IsNullOrWhiteSpace(responseJson))
                return new List<T>();

            IEnumerable<DirectMessageEvents> dmList;

            switch (Type)
            {
                case DirectMessageEventsType.List:
                case DirectMessageEventsType.Show:
                    dmList = HandleDirectMessage(responseJson);
                    break;
                default:
                    dmList = new List<DirectMessageEvents>();
                    break;
            }

            return dmList.OfType<T>().ToList();
        }

        IEnumerable<DirectMessageEvents> HandleDirectMessage(string dmJson)
        {
            DirectMessageEventsValue dmEvent = JsonSerializer.Deserialize<DirectMessageEventsValue>(dmJson);

            return new List<DirectMessageEvents> 
            { 
                new DirectMessageEvents
                {
                    Type = Type,
                    Count = Count,
                    Cursor = Cursor,
                    ID = ID,
                    Value = dmEvent
                }
            };
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            var dm = new DirectMessageEvents
            {
                Value = JsonSerializer.Deserialize<DirectMessageEventsValue>(responseJson ?? "")
            };

            return dm.ItemCast(default(T));
        }
    }
}
