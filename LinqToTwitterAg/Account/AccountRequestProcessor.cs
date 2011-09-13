using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// handles query processing for accounts
    /// </summary>
    public class AccountRequestProcessor<T>
        : IRequestProcessor<T>
        , IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// Type of account query (VerifyCredentials or RateLimitStatus)
        /// </summary>
        private AccountType Type { get; set; }

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
        public virtual Request BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<AccountType>(parameters["Type"]);

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
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of User</returns>
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

        private Account HandleSettingsResponse(string responseJson)
        {
            var serializer = Json.AccountConverter.GetSerializer();
            var settings = serializer.Deserialize<Json.Settings>(responseJson);

            var acct = new Account
            {
                Type = Type,
                Settings = new Settings
                {
                    TrendLocation = settings.trend_location.ToLocation(),
                    GeoEnabled = settings.geo_enabled,
                    SleepTime = settings.sleep_time.ToSleepTime(),
                    Language = settings.language,
                    AlwaysUseHttps = settings.always_use_https,
                    DiscoverableByEmail = settings.discoverable_by_email,
                    TimeZone = settings.time_zone.ToTZInfo()
                }
            };

            return acct;
        }

        private Account HandleRateLimitResponse(string responseJson)
        {
            var serializer = Json.AccountConverter.GetSerializer();
            var status = serializer.Deserialize<Json.RateLimitStatus>(responseJson);

            var acct = new Account
            {
                Type = Type,
                RateLimitStatus = new RateLimitStatus
                {
                    HourlyLimit = status.hourly_limit,
                    RemainingHits = status.remaining_hits,
                    ResetTime = status.reset_time,
                    ResetTimeInSeconds = status.reset_time_in_seconds
                }
            };

            return acct;
        }

        private Account HandleVerifyCredentialsResponse(string responseJson)
        {
            var serializer = Json.AccountConverter.GetSerializer();
            var user = serializer.Deserialize<Json.User>(responseJson);

            var acct = new Account
            {
                Type = Type,
                User = user.ToUser()
            };

            return acct;
        }

        private Account HandleTotalsResponse(string responseJson)
        {
            var serializer = Json.AccountConverter.GetSerializer();
            var totals = serializer.Deserialize<Json.Totals>(responseJson);

            var acct = new Account
            {
                Type = Type,
                Totals = new Totals
                {
                    Favorites = totals.favorites,
                    Followers = totals.followers,
                    Friends = totals.friends,
                    Updates = totals.updates
                }
            };

            return acct;
        }
#if THESEDONTSEEMTOBEINJSON

            else if (twitterResponse.Name == "hash")
            {
                if (twitterResponse.Element("hourly-limit") != null)
                {
                    var rateLimits = new RateLimitStatus
                    {
                        HourlyLimit = int.Parse(twitterResponse.Element("hourly-limit").Value),
                        RemainingHits = int.Parse(twitterResponse.Element("remaining-hits").Value),
                        ResetTime = DateTime.Parse(twitterResponse.Element("reset-time").Value,
                                                    CultureInfo.InvariantCulture,
                                                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                        ResetTimeInSeconds = int.Parse(twitterResponse.Element("reset-time-in-seconds").Value)
                    };

                    acct.RateLimitStatus = rateLimits;
                }
                else if (twitterResponse.Element("request") != null)
                {
                    var endSession = new TwitterHashResponse
                    {
                        Request = twitterResponse.Element("request").Value,
                        Error = twitterResponse.Element("error").Value
                    };

                    acct.EndSessionStatus = endSession;
                }
            }
#endif
    }
}
