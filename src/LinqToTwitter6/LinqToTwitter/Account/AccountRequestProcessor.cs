using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

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
        public virtual string? BaseUrl { get; set; }

        /// <summary>
        /// Type of account query (VerifyCredentials or Settings)
        /// </summary>
        public AccountType Type { get; set; }

        /// <summary>
        /// Don't include status in response
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Removes entities when set to false (true by default)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Includes the user's email address in response (requires whitelisting,
        /// see https://dev.twitter.com/rest/reference/get/account/verify_credentials)
        /// </summary>
        public bool IncludeEmail { get; set; }

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
                       "SkipStatus",
                       "IncludeEntities",
                       "IncludeEmail"
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
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<AccountType>(parameters[TypeParam]);

            return Type switch
            {
                AccountType.VerifyCredentials => BuildVerifyCredentialsUrl(parameters),
                AccountType.Settings => new Request(BaseUrl + "account/settings.json"),
                _ => throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified."),
            };
        }
  
        Request BuildVerifyCredentialsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "account/verify_credentials.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("SkipStatus") &&
                RequestProcessorHelper.FlagTrue(parameters, "SkipStatus"))
            {
                    SkipStatus = true;
                    urlParams.Add(new QueryParameter("skip_status", "true"));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("IncludeEmail"))
            {
                IncludeEmail = bool.Parse(parameters["IncludeEmail"]);
                urlParams.Add(new QueryParameter("include_email", parameters["IncludeEmail"].ToLower()));
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
            var list = new List<Account>();
            Account? acct = null;

            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                acct = Type switch
                {
                    AccountType.Settings => HandleSettingsResponse(responseJson),
                    AccountType.VerifyCredentials => HandleVerifyCredentialsResponse(responseJson),
                    _ => throw new InvalidOperationException("The default case of ProcessResults should never execute because a Type must be specified."),
                };

                acct.Type = Type;
                acct.SkipStatus = SkipStatus;
                acct.IncludeEntities = IncludeEntities;
                acct.IncludeEmail = IncludeEmail;
            }


            if (acct != null)
                list.Add(acct);

            return list.OfType<T>().ToList();
        }

        /// <summary>
        /// transforms json into an action response
        /// </summary>
        /// <param name="responseJson">json with Twitter response</param>
        /// <param name="theAction">Used to specify side-effect methods</param>
        /// <returns>Action response</returns>
        public virtual T? ProcessActionResult(string responseJson, Enum theAction)
        {
            Account? acct = null;

            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                acct = ((AccountAction)theAction) switch
                {
                    AccountAction.Settings => HandleSettingsResponse(responseJson),
                    _ => throw new InvalidOperationException("The default case of ProcessActionResult should never execute because a Type must be specified."),
                };
            }

            return acct.ItemCast(default(T));
        }

        public Account HandleSettingsResponse(string responseJson)
        {
            var settings = JsonDocument.Parse(responseJson).RootElement;
            var sleepTime = settings.GetProperty("sleep_time");
            var timeZone = settings.GetProperty("time_zone");
            var trendLocationData = settings.GetProperty("trend_location");
            var trendLocation = trendLocationData.EnumerateArray().FirstOrDefault();

            var acct = new Account
            {
                Type = Type,
                Settings = new Settings
                {
                    TrendLocation = new Location(trendLocation),
                    GeoEnabled = settings.GetBool("geo_enabled"),
                    SleepTime = new SleepTime(sleepTime),
                    Language = settings.GetString("language"),
                    AlwaysUseHttps = settings.GetBool("always_use_https"),
                    DiscoverableByEmail = settings.GetBool("discoverable_by_email"),
                    DiscoverableByMobilePhone = settings.GetBool("discoverable_by_mobile_phone"),
                    TimeZone = new TZInfo(timeZone),
                    ScreenName = settings.GetString("screen_name"),
                    UseCookiePersonalization = settings.GetBool("use_cookie_personalization"),
                    DisplaySensitiveMedia = settings.GetBool("display_sensitive_media"),
                    AllowContributorRequest = settings.GetString("allow_contributor_request"),
                    AllowDmsFrom = settings.GetString("allow_dms_from"),
                    AllowDmGroupsFrom = settings.GetString("allow_dm_groups_from")
                }
            };


            return acct;
        }

        Account HandleVerifyCredentialsResponse(string responseJson)
        {
            JsonElement user = JsonDocument.Parse(responseJson).RootElement;

            var acct = new Account
            {
                Type = Type,
                User = new User(user)
            };

            return acct;
        }
    }
}
