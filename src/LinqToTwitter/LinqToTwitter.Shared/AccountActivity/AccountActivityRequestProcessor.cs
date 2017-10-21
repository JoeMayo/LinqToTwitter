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
    public class AccountActivityRequestProcessor<T> :
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
        /// Type of Account Activity
        /// </summary>
        public AccountActivityType Type { get; set; }

        /// <summary>
        /// ID for a webhook
        /// </summary>
        internal ulong WebhookID { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<AccountActivity>(
                   lambdaExpression.Body,
                   new List<string> { 
                       nameof(Type),
                       nameof(WebhookID)
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

            Type = RequestProcessorHelper.ParseQueryEnumType<AccountActivityType>(parameters[nameof(Type)]);

            switch (Type)
            {
                case AccountActivityType.Subscriptions:
                    return BuildSubscriptionsUrl(parameters);
                case AccountActivityType.Webhooks:
                    return BuildWebhooksUrl(parameters);
                default:
                    throw new InvalidOperationException(
                        $"Didn't recognize '{Type}' for {nameof(Type)} parameter in AccountActivityRequestProcessor.BuildUrl.");
            }
        }

        Request BuildWebhooksUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "account_activity/webhooks.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

            return req;
        }

        Request BuildSubscriptionsUrl(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(nameof(WebhookID)))
                throw new ArgumentNullException(nameof(WebhookID), $"{nameof(WebhookID)} is required.");

            WebhookID = ulong.Parse(parameters[nameof(WebhookID)]);

            var req = new Request(BaseUrl + $"account_activity/webhooks/{WebhookID}/subscriptions.json");

            return req;
        }

        /// <summary>
        /// Transforms twitter response into List of Blocks objects
        /// </summary>
        /// <param name="responseJson">JSON with Twitter response</param>
        /// <returns>List of DirectMessage</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            IEnumerable<AccountActivity> accActList;

            switch (Type)
            {
                case AccountActivityType.Subscriptions:
                    accActList = HandleSubscriptions(responseJson);
                    break;
                case AccountActivityType.Webhooks:
                    accActList = HandleWebhooks(responseJson);
                    break;
                default:
                    accActList = new List<AccountActivity>();
                    break;
            }

            return accActList.OfType<T>().ToList();
        }

        IEnumerable<AccountActivity> HandleSubscriptions(string accActJson)
        {
            return new List<AccountActivity>
            {
                new AccountActivity
                {
                    Type = Type,
                    WebhookID = WebhookID,
                    SubscriptionValue = new SubscriptionValue { IsSubscribed = true }
                }
            };
        }
        IEnumerable<AccountActivity> HandleWebhooks(string accActJson)
        {
            Webhook[] webhooks = JsonConvert.DeserializeObject<Webhook[]>(accActJson);

            return new List<AccountActivity> 
            { 
                new AccountActivity
                {
                    Type = Type,
                    WebhookID = WebhookID,
                    WebhooksValue = new WebhooksValue
                    {
                        Webhooks = webhooks
                    }
                }
            };
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            var accAct = new AccountActivity();

            if ((AccountActivityType)theAction == AccountActivityType.Webhooks)
                accAct.WebhooksValue =
                    new WebhooksValue
                    {
                        Webhooks =
                            new Webhook[]
                            {
                                responseJson == null ?
                                    new Webhook() :
                                    JsonConvert.DeserializeObject<Webhook>(responseJson)
                            }
                    };
            else if ((AccountActivityType)theAction == AccountActivityType.Subscriptions)
                accAct.SubscriptionValue = new SubscriptionValue { IsSubscribed = true };

            return accAct.ItemCast(default(T));
        }
    }
}
