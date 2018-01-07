using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;

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
        /// Type of Welcome Message
        /// </summary>
        internal WelcomeMessageType Type { get; set; }

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
                       nameof(Type),
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

            Type = RequestProcessorHelper.ParseQueryEnumType<WelcomeMessageType>(parameters[nameof(Type)]);

            switch (Type)
            {
                //case WelcomeMessageType.List:
                //    return BuildListUrl(parameters);
                //case WelcomeMessageType.Show:
                //    return BuildShowUrl(parameters);
                default:
                    throw new InvalidOperationException(
                        $"Didn't recognize '{Type}' for {nameof(Type)} parameter in WelcomeMessageRequestProcessor.BuildUrl.");
            }
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
                case WelcomeMessageType.List:
                case WelcomeMessageType.Show:
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
            WelcomeMessageValue welcomeMsg = JsonConvert.DeserializeObject<WelcomeMessageValue>(msgJson);

            return new List<WelcomeMessage>
            {
                new WelcomeMessage
                {
                    Type = Type,
                    Value = welcomeMsg
                }
            };
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            var msg = new WelcomeMessage
            {
                Value = JsonConvert.DeserializeObject<WelcomeMessageValue>(responseJson ?? "")
            };

            return msg.ItemCast(default(T));
        }
    }
}
