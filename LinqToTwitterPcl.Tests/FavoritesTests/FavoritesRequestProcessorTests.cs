using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.FavoritesTests
{
    public class FavoritesRequestProcessorTests
    {
        public FavoritesRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void ProcessResults_Handles_Response()
        {
            var favReqProc = new FavoritesRequestProcessor<Favorites>();

            List<Favorites> favorites = favReqProc.ProcessResults(TwitterResponse);

            Assert.AreEqual(2, favorites.Count());
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new FavoritesRequestProcessor<Favorites> { BaseUrl = "https://api.twitter.com/1.1/" };

            var favorites = reqProc.ProcessResults(string.Empty);

            Assert.IsFalse(favorites.Any());
        }

        [TestMethod]
        public void GetParameters_Handles_Input_Parameters()
        {
            var favReqProc = new FavoritesRequestProcessor<Favorites> { BaseUrl = "https://api.twitter.com/1.1/" };
            Expression<Func<Favorites, bool>> expression =
                fav =>
                    fav.Type == FavoritesType.Favorites &&
                    fav.UserID == "123" &&
                    fav.ScreenName == "JoeMayo" &&
                    fav.Count == 100 &&
                    fav.SinceID == 456 &&
                    fav.MaxID == 789 &&
                    fav.IncludeEntities == true;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = favReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)FavoritesType.Favorites).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("UserID", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "JoeMayo")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Count", "100")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SinceID", "456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxID", "789")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeEntities", "True")));
        }

        [TestMethod]
        public void BuildUrl_Constructs_Favorites_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/favorites/list.json?user_id=123&screen_name=JoeMayo&count=100&since_id=456&max_id=789&include_entities=true";
            var favReqProc = new FavoritesRequestProcessor<Favorites> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", FavoritesType.Favorites.ToString() },
                    { "UserID", "123" },
                    { "ScreenName", "JoeMayo" },
                    { "Count", "100" },
                    { "SinceID", "456" },
                    { "MaxID", "789" },
                    { "IncludeEntities", true.ToString() }

                };

            Request req = favReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        [Ignore]
        public void BuildUrl_Throws_On_Missing_Type_Param()
        {
            var favReqProc = new FavoritesRequestProcessor<Favorites> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>();

            //var ex = Assert.Throws<ArgumentException>(() => favReqProc.BuildUrl(parameters));

            //Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        [Ignore]
        public void BuildUrl_Throws_On_Null_Params_Dictionary()
        {
            var favReqProc = new FavoritesRequestProcessor<Favorites> { BaseUrl = "https://api.twitter.com/1.1/" };

            //var ex = Assert.Throws<ArgumentException>(() => favReqProc.BuildUrl(null));

            //Assert.AreEqual("parameters", ex.ParamName);
        }

        const string TwitterResponse = @"[
   {
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""created_at"":""Thu Apr 05 16:44:08 +0000 2012"",
      ""id_str"":""187943716558864384"",
      ""in_reply_to_user_id_str"":null,
      ""coordinates"":null,
      ""truncated"":false,
      ""place"":null,
      ""retweeted"":false,
      ""user"":{
         ""is_translator"":false,
         ""show_all_inline_media"":true,
         ""id"":24358412,
         ""statuses_count"":1921,
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""follow_request_sent"":false,
         ""default_profile_image"":false,
         ""favourites_count"":10,
         ""utc_offset"":null,
         ""profile_background_color"":""C0DEED"",
         ""verified"":false,
         ""name"":""Mark Russinovich"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/198832493\/ZeroDayCover.jpg"",
         ""protected"":false,
         ""profile_link_color"":""0084B4"",
         ""lang"":""en"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1497556516\/markrussinovich_normal.jpg"",
         ""url"":""http:\/\/www.zerodaythebook.com"",
         ""created_at"":""Sat Mar 14 11:43:34 +0000 2009"",
         ""default_profile"":false,
         ""profile_use_background_image"":true,
         ""description"":""Technical Fellow at Microsoft in Windows Azure, author of Zero Day: A Novel, Windows Internals, Sysinternals Admin Reference and the Sysinternals utilities. "",
         ""id_str"":""24358412"",
         ""profile_text_color"":""333333"",
         ""time_zone"":null,
         ""following"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""C0DEED"",
         ""location"":"""",
         ""notifications"":false,
         ""listed_count"":596,
         ""geo_enabled"":false,
         ""friends_count"":70,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1497556516\/markrussinovich_normal.jpg"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/198832493\/ZeroDayCover.jpg"",
         ""followers_count"":11740,
         ""screen_name"":""markrussinovich"",
         ""profile_background_tile"":true
      },
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""annotations"":null,
      ""contributors"":null,
      ""source"":""web"",
      ""geo"":null,
      ""retweet_count"":41,
      ""favorited"":true,
      ""id"":187943716558864384,
      ""in_reply_to_user_id"":null,
      ""text"":""Windows Internals 6th Edition Part 1 (on Win7\/Server 2008R2) is now available for purchase! http:\/\/t.co\/F8vE1LnC""
   },
   {
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""created_at"":""Thu Mar 22 15:34:00 +0000 2012"",
      ""id_str"":""182852636905701377"",
      ""in_reply_to_user_id_str"":null,
      ""coordinates"":null,
      ""truncated"":false,
      ""place"":null,
      ""retweeted"":false,
      ""user"":{
         ""is_translator"":false,
         ""show_all_inline_media"":false,
         ""id"":14457688,
         ""statuses_count"":8803,
         ""profile_sidebar_fill_color"":""e9e9dc"",
         ""follow_request_sent"":false,
         ""default_profile_image"":false,
         ""favourites_count"":5,
         ""utc_offset"":-18000,
         ""profile_background_color"":""1574a4"",
         ""verified"":false,
         ""name"":""Andrew Brust"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""protected"":false,
         ""profile_link_color"":""1574a4"",
         ""lang"":""en"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1826210109\/AndrewBrust-_normal.jpg"",
         ""url"":""http:\/\/bit.ly\/bigondata"",
         ""created_at"":""Mon Apr 21 02:57:59 +0000 2008"",
         ""default_profile"":false,
         ""profile_use_background_image"":true,
         ""description"":""Founder\/CEO, Blue Badge Insights; ZDNet Big Data blogger; RedDevNews.com columnist; co-author Programming Microsoft SQL Server 2012; Microsoft tech influencer."",
         ""id_str"":""14457688"",
         ""profile_text_color"":""5e6466"",
         ""time_zone"":""Eastern Time (US & Canada)"",
         ""following"":true,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""1c4273"",
         ""location"":""New York, NY"",
         ""notifications"":false,
         ""listed_count"":111,
         ""geo_enabled"":false,
         ""friends_count"":720,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1826210109\/AndrewBrust-_normal.jpg"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""followers_count"":1743,
         ""screen_name"":""andrewbrust"",
         ""profile_background_tile"":false
      },
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""annotations"":null,
      ""contributors"":null,
      ""source"":""\u003Ca href=\""http:\/\/www.tweetdeck.com\"" rel=\""nofollow\""\u003ETweetDeck\u003C\/a\u003E"",
      ""geo"":null,
      ""retweet_count"":0,
      ""favorited"":true,
      ""id"":182852636905701377,
      ""in_reply_to_user_id"":null,
      ""text"":""Check out my #ZDNet blog post - Big Data\u2019s Ground Floor Consulting Opportunity - http:\/\/t.co\/PxDMPRJT""
   }
]";
    }
}
