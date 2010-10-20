using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// handles query processing for accounts
    /// </summary>
    public class AccountRequestProcessor<T> : IRequestProcessor<T>
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
        public virtual string BuildURL(Dictionary<string, string> parameters)
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
                    url = BaseUrl + "account/verify_credentials.xml";
                    break;
                case AccountType.RateLimitStatus:
                    url = BaseUrl + "account/rate_limit_status.xml";
                    break;
                case AccountType.Totals:
                    url = BaseUrl + "account/totals.xml";
                    break;
                case AccountType.Settings:
                    url = BaseUrl + "account/settings.xml";
                    break;
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

            return url;
        }

        /// <summary>
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of User</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);
            var acct = new Account { Type = Type };

            if (twitterResponse.Name == "user")
            {
                var user = User.CreateUser(twitterResponse);

                acct.User = user;
            }
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
                else
                {
                    acct.Totals = new Totals
                    {
                        Updates = int.Parse(twitterResponse.Element("updates").Value),
                        Friends = int.Parse(twitterResponse.Element("friends").Value),
                        Favorites = int.Parse(twitterResponse.Element("favorites").Value),
                        Followers = int.Parse(twitterResponse.Element("followers").Value)
                    };
                }
            }
            else if (twitterResponse.Name == "settings")
            {
                acct.Settings = new Settings
                {
                    TrendLocation = Location.CreateLocation(twitterResponse.Element("trend_location")),
                    GeoEnabled = bool.Parse(twitterResponse.Element("geo_enabled").Value),
                    SleepTime = SleepTime.CreateSleepTime(twitterResponse.Element("sleep_time"))
                };
            }
            else
            {
                throw new ArgumentException("Account Results Processing expected a Twitter response for either a user or hash, but received an unknown element type instead.");
            }

            return new List<Account> { acct }.OfType<T>().ToList();
        }
    }
}
