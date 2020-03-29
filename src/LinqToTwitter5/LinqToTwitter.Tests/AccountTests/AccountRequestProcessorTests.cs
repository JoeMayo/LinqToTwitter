using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.AccountTests
{
    [TestClass]
    public class AccountRequestProcessorTests
    {
        public AccountRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void ProcessResults_Handles_VerifyCredentials()
        {
            var target = new AccountRequestProcessor<Account> { Type = AccountType.VerifyCredentials };

            List<Account> actual = target.ProcessResults(TestVerifyCredentialsQueryResponse);

            var acct = actual.Cast<Account>().ToList().FirstOrDefault();
            Assert.IsNotNull(acct);
            Assert.AreEqual("Joe Mayo", acct.User.Name);
        }

        [TestMethod]
        public void HandleSettingsResponse_Converts_Settings_To_Account()
        {
            var acctReqProc = new AccountRequestProcessor<Account>();

            Account acct = acctReqProc.HandleSettingsResponse(TestSettingsResponse);

            Assert.IsNotNull(acct);
            Assert.IsNotNull(acct.Settings);
            var settings = acct.Settings;
            Assert.IsNotNull(settings.TimeZone);
            var timeZone = settings.TimeZone;
            Assert.AreEqual("Mountain Time (US & Canada)", timeZone.Name);
            Assert.AreEqual(-21600, timeZone.UtcOffset);
            Assert.AreEqual("America/Denver", timeZone.TzInfoName);
            Assert.AreEqual("Mayoster", settings.ScreenName);
            Assert.AreEqual(true, settings.AlwaysUseHttps);
            Assert.AreEqual(true, settings.UseCookiePersonalization);
            Assert.IsNotNull(settings.SleepTime);
            var sleepTime = settings.SleepTime;
            Assert.AreEqual(true, sleepTime.Enabled);
            Assert.AreEqual(20, sleepTime.StartHour);
            Assert.AreEqual(0, sleepTime.EndHour);
            Assert.AreEqual(true, settings.GeoEnabled);
            Assert.AreEqual("en", settings.Language);
            Assert.AreEqual(true, settings.DiscoverableByEmail);
            Assert.AreEqual(true, settings.DiscoverableByMobilePhone);
            Assert.AreEqual(true, settings.DisplaySensitiveMedia);
            Assert.AreEqual("all", settings.AllowContributorRequest);
            Assert.AreEqual("following", settings.AllowDmsFrom);
            Assert.AreEqual("following", settings.AllowDmGroupsFrom);
            Assert.IsNotNull(settings.TrendLocation);
            var trendLocation = settings.TrendLocation;
            Assert.AreEqual("Las Vegas", trendLocation.Name);
            Assert.AreEqual("US", trendLocation.CountryCode);
            Assert.AreEqual("http://where.yahooapis.com/v1/place/2436704", trendLocation.Url);
            Assert.AreEqual(2436704, trendLocation.WoeID);
            Assert.AreEqual("Town", trendLocation.PlaceTypeName);
            Assert.AreEqual(7, trendLocation.PlaceTypeNameCode);
            Assert.AreEqual("23424977", trendLocation.ParentID);
            Assert.AreEqual("United States", trendLocation.Country);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new AccountRequestProcessor<Account> { BaseUrl = "https://api.twitter.com/1.1/" };

            var accts = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, accts.Count);
        }

        [TestMethod]
        public void GetParameters_Handles_Input_Params()
        {
            var target = new AccountRequestProcessor<Account>();
            Expression<Func<Account, bool>> expression = 
                acct => 
                    acct.Type == AccountType.Settings &&
                    acct.SkipStatus == true &&
                    acct.IncludeEntities == true &&
                    acct.IncludeEmail == true;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)AccountType.Settings).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SkipStatus", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeEntities", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeEmail", "True")));
        }

        [TestMethod]
        public void BuildUrl_Constructs_VerifyCredentials_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/account/verify_credentials.json?skip_status=true&include_entities=false&include_email=true";
            var acctReqProc = new AccountRequestProcessor<Account> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.VerifyCredentials).ToString(CultureInfo.InvariantCulture) },
                        { "SkipStatus", true.ToString() },
                        { "IncludeEntities", false.ToString() },
                        { "IncludeEmail", true.ToString() }
                };

            Request req = acctReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Returns_Settings_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/account/settings.json";
            var acctReqProc = new AccountRequestProcessor<Account> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.Settings).ToString(CultureInfo.InvariantCulture) }
                };

            Request req = acctReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Throws_When_Type_Not_Provided()
        {
            var acctReqProc = new AccountRequestProcessor<Account> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>();

            var ex = L2TAssert.Throws<ArgumentException>(() => acctReqProc.BuildUrl(parameters));

            Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Throws_With_Null_Parameters()
        {
            var acctReqProc = new AccountRequestProcessor<Account> { BaseUrl = "https://api.twitter.com/1.1/" };

            var ex = L2TAssert.Throws<ArgumentException>(() => acctReqProc.BuildUrl(null));

            Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void ProcessResults_Retains_Original_Input_Parameters()
        {
            var acctReqProc = new AccountRequestProcessor<Account>
            {
                Type = AccountType.VerifyCredentials,
                SkipStatus = true,
                IncludeEntities = true
            };

            var accounts = acctReqProc.ProcessResults(TestVerifyCredentialsQueryResponse);

            Assert.IsNotNull(accounts);
            Assert.IsNotNull(accounts.SingleOrDefault());
            var account = accounts.Single();
            Assert.AreEqual(AccountType.VerifyCredentials, account.Type);
            Assert.IsTrue(account.SkipStatus);
            Assert.IsTrue(account.IncludeEntities);
        }

        const string TestVerifyCredentialsQueryResponse = @"{
   ""statuses_count"":1624,
   ""favourites_count"":65,
   ""protected"":false,
   ""profile_text_color"":""3C3940"",
   ""profile_image_url"":""http:\/\/a2.twimg.com\/profile_images\/520626655\/JoeTwitterBW_-_150_x_150_normal.jpg"",
   ""name"":""Joe Mayo"",
   ""profile_sidebar_fill_color"":""95E8EC"",
   ""default_profile"":false,
   ""listed_count"":101,
   ""following"":false,
   ""profile_background_tile"":false,
   ""utc_offset"":-25200,
   ""description"":""Independent .NET Consultant; author of 6 books; MVP; interested in C#, MVC, and Language Integrated Query (LINQ)"",
   ""location"":""Denver, CO"",
   ""contributors_enabled"":false,
   ""verified"":false,
   ""profile_link_color"":""0099B9"",
   ""followers_count"":875,
   ""url"":""http:\/\/linqtotwitter.codeplex.com\/"",
   ""status"":{
      ""in_reply_to_status_id_str"":null,
      ""truncated"":false,
      ""in_reply_to_user_id_str"":null,
      ""geo"":null,
      ""retweet_count"":2,
      ""contributors"":null,
      ""coordinates"":null,
      ""favorited"":false,
      ""in_reply_to_screen_name"":null,
      ""source"":""web"",
      ""id_str"":""115131137906589696"",
      ""in_reply_to_status_id"":null,
      ""id"":115131137906589696,
      ""created_at"":""Sat Sep 17 18:32:56 +0000 2011"",
      ""possibly_sensitive"":false,
      ""place"":null,
      ""retweeted"":false,
      ""in_reply_to_user_id"":null,
      ""text"":""Visual Studio 11 Developer Preview: http:\/\/t.co\/fISuavMD""
   },
   ""profile_sidebar_border_color"":""5ED4DC"",
   ""screen_name"":""JoeMayo"",
   ""default_profile_image"":false,
   ""notifications"":false,
   ""show_all_inline_media"":false,
   ""geo_enabled"":true,
   ""profile_use_background_image"":true,
   ""friends_count"":161,
   ""id_str"":""15411837"",
   ""is_translator"":false,
   ""lang"":""en"",
   ""time_zone"":""Mountain Time (US & Canada)"",
   ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
   ""profile_background_color"":""0099B9"",
   ""id"":15411837,
   ""follow_request_sent"":false,
   ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
   ""profile_background_image_url"":""http:\/\/a1.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
   ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/520626655\/JoeTwitterBW_-_150_x_150_normal.jpg""
}";

        const string TestSettingsResponse = @"{
	""time_zone"": {
		""name"": ""Mountain Time (US & Canada)"",
		""utc_offset"": -21600,
		""tzinfo_name"": ""America\/Denver""
	},
	""protected"": true,
	""screen_name"": ""Mayoster"",
	""always_use_https"": true,
	""use_cookie_personalization"": true,
	""sleep_time"": {
		""enabled"": true,
		""end_time"": 0,
		""start_time"": 20
	},
	""geo_enabled"": true,
	""language"": ""en"",
	""discoverable_by_email"": true,
	""discoverable_by_mobile_phone"": true,
	""display_sensitive_media"": true,
	""allow_contributor_request"": ""all"",
	""allow_dms_from"": ""following"",
	""allow_dm_groups_from"": ""following"",
	""trend_location"": [{
		""name"": ""Las Vegas"",
		""countryCode"": ""US"",
		""url"": ""http:\/\/where.yahooapis.com\/v1\/place\/2436704"",
		""woeid"": 2436704,
		""placeType"": {
			""name"": ""Town"",
			""code"": 7
		},
		""parentid"": 23424977,
		""country"": ""United States""
	}]
}";
    }
}
