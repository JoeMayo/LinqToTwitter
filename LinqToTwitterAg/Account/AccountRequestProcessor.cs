using System;
using System.Collections.Generic;
using System.Linq;

using LinqToTwitter.Common;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// handles query processing for accounts
    /// </summary>
    public class AccountRequestProcessor<T>
        : IRequestProcessor<T>
        , IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// Type of account query (VerifyCredentials or RateLimitStatus)
        /// </summary>
        internal AccountType Type { get; set; }

        /// <summary>
        /// Don't include status in response
        /// </summary>
        internal bool SkipStatus { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(System.Linq.Expressions.LambdaExpression lambdaExpression)
        {
            return
               new ParameterFinder<Account>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "SkipStatus"
                   })
                   .Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            string url;

            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<AccountType>(parameters[TypeParam]);

            switch (Type)
            {
                case AccountType.VerifyCredentials:
                    url = BuildVerifyCredentialsUrl(parameters).FullUrl;
                    break;
                case AccountType.Settings:
                    url = BaseUrl + "account/settings.json";
                    break;
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

            return new Request(url);
        }
  
        private Request BuildVerifyCredentialsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "account/verify_credentials.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("SkipStatus"))
            {
                if (RequestProcessorHelper.FlagTrue(parameters, "SkipStatus"))
                {
                    SkipStatus = true;
                    urlParams.Add(new QueryParameter("skip_status", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// transforms json into IQueryable of Account
        /// </summary>
        /// <param name="responseJson">json with Twitter response</param>
        /// <returns>List of Account</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            Account acct = null;

            if (!string.IsNullOrEmpty(responseJson))
            {
                switch (Type)
                {
                    case AccountType.Settings:
                        acct = HandleSettingsResponse(responseJson);
                        break;

                    case AccountType.VerifyCredentials:
                        acct = HandleVerifyCredentialsResponse(responseJson);
                        break;

                    default:
                        throw new InvalidOperationException("The default case of ProcessResults should never execute because a Type must be specified.");
                }

                acct.Type = Type;
                acct.SkipStatus = SkipStatus;
            }

            return new List<Account> { acct }.OfType<T>().ToList();
        }

        /// <summary>
        /// transforms json into an action response
        /// </summary>
        /// <param name="responseJson">json with Twitter response</param>
        /// <param name="theAction">Used to specify side-effect methods</param>
        /// <returns>Action response</returns>
        public virtual T ProcessActionResult(string responseJson, Enum theAction)
        {
            Account acct = null;

            if (!string.IsNullOrEmpty(responseJson))
            {
                switch ((AccountAction)theAction)
                {
                    case AccountAction.Settings:
                        acct = HandleSettingsResponse(responseJson);
                        break;
                    default:
                        throw new InvalidOperationException("The default case of ProcessActionResult should never execute because a Type must be specified.");
                }
            }

            return acct.ItemCast(default(T));
        }

        internal Account HandleSettingsResponse(string responseJson)
        {
            var settings = JsonMapper.ToObject(responseJson);
            var sleepTime = settings.GetValue<JsonData>("sleep_time");
            var timeZone = settings.GetValue<JsonData>("time_zone");
            var trendLocationData = settings.GetValue<JsonData>("trend_location");
            var trendLocation = trendLocationData == null ? null : trendLocationData[0];

            var acct = new Account
            {
                Type = Type,
                Settings = new Settings
                {
                    TrendLocation = new Location(trendLocation),
                    GeoEnabled = settings.GetValue<bool>("geo_enabled"),
                    SleepTime = new SleepTime(sleepTime),
                    Language = settings.GetValue<string>("language"),
                    AlwaysUseHttps = settings.GetValue<bool>("always_use_https"),
                    DiscoverableByEmail = settings.GetValue<bool>("discoverable_by_email"),
                    TimeZone = new TZInfo(timeZone)
                }
            };


            return acct;
        }

        private Account HandleVerifyCredentialsResponse(string responseJson)
        {
            var user = JsonMapper.ToObject(responseJson);

            var acct = new Account
            {
                Type = Type,
                User = new User(user)
            };

            return acct;
        }
    }
}
