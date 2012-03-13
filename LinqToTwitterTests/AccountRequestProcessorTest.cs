using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterTests
{
    
    
    /// <summary>
    ///This is a test class for AccountRequestProcessorTest and is intended
    ///to contain all AccountRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass]
    public class AccountRequestProcessorTest
    {
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

        const string TestRateLimitStatusQueryResponse = @"{
   ""remaining_hits"":343,
   ""hourly_limit"":350,
   ""reset_time_in_seconds"":1316397996,
   ""reset_time"":""Mon Sep 19 02:06:36 +0000 2011""
}";

        const string TestEndSessionResponse = @"{
  ""request"": ""/1/account/end_session.json"",
  ""error"": ""Logged out.""
}";

        const string TestTotalsResponse = @"{
   ""followers"":875,
   ""favorites"":65,
   ""friends"":161,
   ""updates"":1624
}";

        const string TestSettingsResponse = @"{
   ""protected"":false,
   ""trend_location"":[
      {
         ""name"":""United States"",
         ""countryCode"":""US"",
         ""placeType"":{
            ""name"":""Country"",
            ""code"":12
         },
         ""parentid"":1,
         ""country"":""United States"",
         ""url"":""http:\/\/where.yahooapis.com\/v1\/place\/23424977"",
         ""woeid"":23424977
      }
   ],
   ""language"":""en"",
   ""sleep_time"":{
      ""start_time"":null,
      ""end_time"":null,
      ""enabled"":false
   },
   ""show_all_inline_media"":false,
   ""discoverable_by_email"":true,
   ""time_zone"":{
      ""tzinfo_name"":""America\/Denver"",
      ""name"":""Mountain Time (US & Canada)"",
      ""utc_offset"":-25200
   },
   ""geo_enabled"":true,
   ""screen_name"":""JoeMayo"",
   ""always_use_https"":true
}";

        [TestMethod]
        public void ProcessResults_Handles_VerifyCredentials()
        {
            var target = new AccountRequestProcessor<Account>();

            IList actual = target.ProcessResults(TestVerifyCredentialsQueryResponse);

            var acct = actual.Cast<Account>().ToList().FirstOrDefault();
            Assert.IsNotNull(acct);
            Assert.AreEqual("Joe Mayo", acct.User.Name);
        }

        [TestMethod]
        public void ProcessResults_Handles_RateLimitStatus()
        {
            var acctReqProc = new AccountRequestProcessor<Account> { Type = AccountType.RateLimitStatus };

            IList actual = acctReqProc.ProcessResults(TestRateLimitStatusQueryResponse);

            var acct = actual.Cast<Account>().ToList().FirstOrDefault();
            Assert.IsNotNull(acct);
            Assert.AreEqual(350, acct.RateLimitStatus.HourlyLimit);
        }

        [TestMethod]
        public void ProcessResults_Converts_Totals_To_Account()
        {
            var acctReqProc = new AccountRequestProcessor<Account> { Type = AccountType.Totals };
            
            List<Account> actual = acctReqProc.ProcessResults(TestTotalsResponse);

            var acct = actual.FirstOrDefault();

            Assert.IsNotNull(acct);
            Assert.AreEqual(1624, acct.Totals.Updates);
            Assert.AreEqual(161, acct.Totals.Friends);
            Assert.AreEqual(65, acct.Totals.Favorites);
            Assert.AreEqual(875, acct.Totals.Followers);
        }

        [Ignore]
        [TestMethod]
        public void ProcessResults_Converts_Settings_To_Account()
        {
            var acctReqProc = new AccountRequestProcessor<Account>();

            List<Account> actual = acctReqProc.ProcessResults(TestSettingsResponse);

            var acct = actual.FirstOrDefault();

            Assert.IsNotNull(acct);
            Assert.AreEqual("23424977", acct.Settings.TrendLocation.WoeID);
            Assert.AreEqual("United States", acct.Settings.TrendLocation.Name);
            Assert.AreEqual(12, acct.Settings.TrendLocation.PlaceTypeNameCode);
            Assert.AreEqual("Country", acct.Settings.TrendLocation.PlaceTypeName);
            Assert.AreEqual("US", acct.Settings.TrendLocation.CountryCode);
            Assert.AreEqual("United States", acct.Settings.TrendLocation.Country);
            Assert.AreEqual("http://where.yahooapis.com/v1/place/23424977", acct.Settings.TrendLocation.Url);
            Assert.AreEqual(true, acct.Settings.GeoEnabled);
            Assert.AreEqual(false, acct.Settings.SleepTime.Enabled);
            Assert.AreEqual(null, acct.Settings.SleepTime.StartHour);
            Assert.AreEqual(null, acct.Settings.SleepTime.EndHour);
        }

        [TestMethod]
        public void ProcessActionResult_Handles_EndSession()
        {
            var acctReqProc = new AccountRequestProcessor<Account>();
            
            var acct = acctReqProc.ProcessActionResult(TestEndSessionResponse, AccountAction.EndSession);

            Assert.AreEqual("Logged out.", acct.EndSessionStatus.Error);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new AccountRequestProcessor<Account>() { BaseUrl = "http://api.twitter.com/1/" };

            var accts = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, accts.Count);
        }

        [TestMethod]
        public void GetParametersTest()
        {
            var target = new AccountRequestProcessor<Account>();
            Expression<Func<Account, bool>> expression = acct => acct.Type == AccountType.RateLimitStatus;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)AccountType.RateLimitStatus).ToString(CultureInfo.InvariantCulture))));
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildVerifyCredentialsStatusURLTest()
        {
            var target = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.VerifyCredentials).ToString() }
                };
            string expected = "https://api.twitter.com/1/account/verify_credentials.json";
            
            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildRateLimitStatusURLTest()
        {
            AccountRequestProcessor<Account> target = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.RateLimitStatus).ToString() }
                };
            string expected = "https://api.twitter.com/1/account/rate_limit_status.json";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod()]
        public void BuildUrl_Returns_Totals_Url()
        {
            var acctReqProc = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.Totals).ToString() }
                };
            string expected = "https://api.twitter.com/1/account/totals.json";

            Request req = acctReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod()]
        public void BuildUrl_Returns_Settings_Url()
        {
            var acctReqProc = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.Settings).ToString() }
                };
            string expected = "https://api.twitter.com/1/account/settings.json";

            Request req = acctReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            AccountRequestProcessor<Account> target = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = new Dictionary<string, string> { };

            try
            {
                target.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual<string>("Type", ae.ParamName);
            }
        }

        /// <summary>
        ///A test for null parameters
        ///</summary>
        [TestMethod()]
        public void NullParametersTest()
        {
            AccountRequestProcessor<Account> target = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = null;

            try
            {
                target.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual<string>("Type", ae.ParamName);
            }
        }
    }
}
