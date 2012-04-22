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
                       "Type"
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

            const string typeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", typeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<AccountType>(parameters[typeParam]);

            switch (Type)
            {
                case AccountType.VerifyCredentials:
                    url = BaseUrl + "account/verify_credentials.json";
                    break;
                case AccountType.RateLimitStatus:
                    url = BaseUrl + "account/rate_limit_status.json";
                    break;
                case AccountType.Totals:
                    url = BaseUrl + "account/totals.json";
                    break;
                case AccountType.Settings:
                    url = BaseUrl + "account/settings.json";
                    break;
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

            return new Request(url);
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

                    case AccountType.RateLimitStatus:
                        acct = HandleRateLimitResponse(responseJson);
                        break;

                    case AccountType.Totals:
                        acct = HandleTotalsResponse(responseJson);
                        break;

                    default:
                        throw new InvalidOperationException("The default case of ProcessResults should never execute because a Type must be specified.");
                }
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
                    case AccountAction.EndSession:
                        acct = HandleEndSessionResponse(responseJson);
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

        internal Account HandleRateLimitResponse(string responseJson)
        {
            var status = JsonMapper.ToObject(responseJson);

            var acct = new Account
            {
                Type = Type,
                RateLimitStatus = new RateLimitStatus
                {
                    HourlyLimit = status.GetValue<int>("hourly_limit"),
                    RemainingHits = status.GetValue<int>("remaining_hits"),
                    ResetTime = status.GetValue<string>("reset_time").GetDate(DateTime.MaxValue),
                    ResetTimeInSeconds = status.GetValue<int>("reset_time_in_seconds")
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

        internal Account HandleTotalsResponse(string responseJson)
        {
            var totals = JsonMapper.ToObject(responseJson);

            var acct = new Account
            {
                Type = Type,
                Totals = new Totals
                {
                    Favorites = totals.GetValue<int>("favorites"),
                    Followers = totals.GetValue<int>("followers"),
                    Friends = totals.GetValue<int>("friends"),
                    Updates = totals.GetValue<int>("updates")
                }
            };

            return acct;
        }

        internal Account HandleEndSessionResponse(string responseJson)
        {
            var endSession = JsonMapper.ToObject(responseJson);

            var acct = new Account
            {
                EndSessionStatus = new TwitterHashResponse
                {
                    Request = endSession.GetValue<string>("request"),
                    Error = endSession.GetValue<string>("error")
                }
            };

            return acct;
        }
    }
}
