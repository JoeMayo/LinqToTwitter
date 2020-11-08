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
    public class WelcomeMessageRequestProcessor<T> :
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
        /// Number of items to return
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Helps page through results greater than Count items
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// ID of item to show
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// Type of Welcome Message
        /// </summary>
        public WelcomeMessageType Type { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<WelcomeMessage>(
                   lambdaExpression.Body,
                   new List<string> {
                       nameof(Count),
                       nameof(Cursor),
                       nameof(ID),
                       nameof(Type)
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

            Type = RequestProcessorHelper.ParseEnum<WelcomeMessageType>(parameters[nameof(Type)]);

            switch (Type)
            {
                case WelcomeMessageType.ListMessages:
                    return BuildListMessagesUrl(parameters);
                case WelcomeMessageType.ListRules:
                    return BuildListRulesUrl(parameters);
                case WelcomeMessageType.ShowMessage:
                    return BuildShowMessagesUrl(parameters);
                case WelcomeMessageType.ShowRule:
                    return BuildShowRuleUrl(parameters);
                default:
                    throw new InvalidOperationException(
                        $"Didn't recognize '{Type}' for {nameof(Type)} parameter in WelcomeMessageRequestProcessor.BuildUrl.");
            }
        }

        /// <summary>
        /// builds an url for getting a list of direct messages
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        Request BuildListMessagesUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "direct_messages/welcome_messages/list.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

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
        /// builds an url for getting a list of direct messages
        /// </summary>
        /// <param name="parameters">parameters to add</param>
        /// <returns>new url with parameters</returns>
        Request BuildListRulesUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "direct_messages/welcome_messages/rules/list.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

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

        Request BuildShowMessagesUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(nameof(ID)))
                throw new ArgumentNullException(nameof(ID), $"{nameof(ID)} is required.");

            var req = new Request(BaseUrl + "direct_messages/welcome_messages/show.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

            ID = ulong.Parse(parameters[nameof(ID)]);
            urlParams.Add(new QueryParameter("id", ID.ToString()));

            return req;
        }

        Request BuildShowRuleUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(nameof(ID)))
                throw new ArgumentNullException(nameof(ID), $"{nameof(ID)} is required.");

            var req = new Request(BaseUrl + "direct_messages/welcome_messages/rules/show.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

            ID = ulong.Parse(parameters[nameof(ID)]);
            urlParams.Add(new QueryParameter("id", ID.ToString()));

            return req;
        }

        /// <summary>
        /// Transforms twitter response into List of Blocks objects
        /// </summary>
        /// <param name="responseJson">JSON with Twitter response</param>
        /// <returns>List of WelcomeMessage</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson))
                return new List<T>();

            IEnumerable<WelcomeMessage> msgList;

            switch (Type)
            {
                case WelcomeMessageType.ListMessages:
                case WelcomeMessageType.ListRules:
                case WelcomeMessageType.ShowMessage:
                case WelcomeMessageType.ShowRule:
                    msgList = HandleWelcomeMessage(responseJson);
                    break;
                default:
                    msgList = new List<WelcomeMessage>();
                    break;
            }

            return msgList.OfType<T>().ToList();
        }

        IEnumerable<WelcomeMessage> HandleWelcomeMessage(string msgJson)
        {
            WelcomeMessageValue welcomeMsg = JsonSerializer.Deserialize<WelcomeMessageValue>(msgJson);

            return new List<WelcomeMessage>
            {
                new WelcomeMessage
                {
                    Type = Type,
                    Count = Count,
                    Cursor = Cursor,
                    ID = ID,
                    Value = welcomeMsg
                }
            };
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            WelcomeMessage msg = null;

            if (!string.IsNullOrWhiteSpace(responseJson))
                msg = new WelcomeMessage
                {
                    Value = JsonSerializer.Deserialize<WelcomeMessageValue>(responseJson ?? "")
                };
            else
                msg = new WelcomeMessage();

            return msg.ItemCast(default(T));
        }
    }
}
