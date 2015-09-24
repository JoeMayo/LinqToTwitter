using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.BlocksTests
{
    [TestClass]
    public class BlocksRequestProcessorTests
    {
        public BlocksRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void ProcessResults_Handles_IDs()
        {
            var blockReqProc = new BlocksRequestProcessor<Blocks>
            {
                Type = BlockingType.Ids
            };

            IList<Blocks> blocksResponse = blockReqProc.ProcessResults(BlockedIDsJson);

            Assert.IsNotNull(blocksResponse);
            Blocks blocks = blocksResponse.First();
            List<string> ids = blocks.IDs;
            Assert.AreEqual(5, ids.Count());
            Assert.AreEqual("35080286", ids.First());
        }

        [TestMethod]
        public void ProcessResults_Handles_BlockedUsers()
        {
            var blockedReqProc = new BlocksRequestProcessor<Blocks>
            {
                Type = BlockingType.List
            };

            IList actual = blockedReqProc.ProcessResults(BlockedUsersJson);

            var actualQuery = actual as IList<Blocks>;
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery[0].Users.Count, 3);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var blocksReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1.1/" };

            var blocks = blocksReqProc.ProcessResults(string.Empty);

            Assert.AreEqual(1, blocks.Count);
        }

        [TestMethod]
        public void ProcessResults_Retains_Original_Input_Parameters()
        {
            var blockedReqProc = new BlocksRequestProcessor<Blocks>
            {
                Type = BlockingType.List,
                ScreenName = "somename",
                Page = 1,
                PerPage = 10,
                IncludeEntities = true,
                SkipStatus = true,
                Cursor = "789"
            };

            var blocks = blockedReqProc.ProcessResults(BlockedUsersJson);

            Assert.IsNotNull(blocks);
            Assert.IsNotNull(blocks.SingleOrDefault());
            var block = blocks.Single();
            Assert.AreEqual(BlockingType.List, block.Type);
            Assert.AreEqual("somename", block.ScreenName);
            Assert.AreEqual(1, block.Page);
            Assert.AreEqual(10, block.PerPage);
            Assert.IsTrue(block.IncludeEntities);
            Assert.IsTrue(block.SkipStatus);
            Assert.AreEqual("789", block.Cursor);
        }

        [TestMethod]
        public void GetParameters_Parses_Parameters()
        {
            var blocksReqProc = new BlocksRequestProcessor<Blocks>();
            Expression<Func<Blocks, bool>> expression =
                block =>
                    block.Type == BlockingType.List &&
                    block.UserID == 123ul &&
                    block.ScreenName == "JoeMayo" &&
                    block.Page == 1 &&
                    block.PerPage == 10 &&
                    block.IncludeEntities == true &&
                    block.SkipStatus == true &&
                    block.Cursor == "789";
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = blocksReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)BlockingType.List).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("UserID", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "JoeMayo")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("PerPage", "10")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SkipStatus", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeEntities", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "789")));
        }

        [TestMethod]
        public void BuildUrl_Creates_List_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/blocks/list.json?page=2&per_page=10&include_entities=true&skip_status=true&cursor=789";
            var blocksReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)BlockingType.List).ToString(CultureInfo.InvariantCulture) },
                    { "Page", "2" },
                    { "PerPage", "10" },
                    { "SkipStatus", true.ToString() },
                    { "Cursor", "789" },
                    { "IncludeEntities", true.ToString() }
                };

            Request req = blocksReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Creates_BlockingIDs_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/blocks/ids.json?cursor=789";
            var blocksReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)BlockingType.Ids).ToString(CultureInfo.InvariantCulture) },
                    { "Cursor", "789" }
                };

            Request req = blocksReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Throws_On_Missing_Type_Parameter()
        {
            var blockReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>();

            var ex = L2TAssert.Throws<ArgumentException>(() => blockReqProc.BuildUrl(parameters));

            Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Throws_On_Null_Parameters()
        {
            var blockReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1.1/" };

            var ex = L2TAssert.Throws<ArgumentException>(() => blockReqProc.BuildUrl(null));

            Assert.AreEqual("Type", ex.ParamName);
        }


        const string BlockedUsersJson = @"{
   ""users"":[
      {
         ""id"":35080286,
         ""location"":null,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/sticky\/default_profile_images\/default_profile_3_normal.png"",
         ""following"":false,
         ""default_profile_image"":true,
         ""is_translator"":false,
         ""favourites_count"":0,
         ""profile_background_color"":""C0DEED"",
         ""utc_offset"":null,
         ""name"":""Nunya Business"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""statuses_count"":1,
         ""profile_link_color"":""0084B4"",
         ""protected"":false,
         ""verified"":false,
         ""entities"":{
            ""description"":{
               ""urls"":[

               ]
            }
         },
         ""contributors_enabled"":false,
         ""default_profile"":true,
         ""profile_use_background_image"":true,
         ""created_at"":""Fri Apr 24 23:44:07 +0000 2009"",
         ""time_zone"":null,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_text_color"":""333333"",
         ""id_str"":""35080286"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/sticky\/default_profile_images\/default_profile_3_normal.png"",
         ""followers_count"":1,
         ""geo_enabled"":false,
         ""url"":null,
         ""lang"":""en"",
         ""notifications"":false,
         ""profile_sidebar_border_color"":""C0DEED"",
         ""listed_count"":0,
         ""screen_name"":""somename"",
         ""status"":{
            ""contributors"":null,
            ""coordinates"":null,
            ""geo"":null,
            ""retweeted"":false,
            ""retweet_count"":0,
            ""in_reply_to_user_id"":null,
            ""in_reply_to_status_id"":null,
            ""created_at"":""Fri Apr 24 23:44:34 +0000 2009"",
            ""id_str"":""1608673165"",
            ""in_reply_to_screen_name"":null,
            ""entities"":{
               ""user_mentions"":[

               ],
               ""urls"":[

               ],
               ""hashtags"":[

               ]
            },
            ""favorited"":false,
            ""truncated"":false,
            ""text"":""whatever"",
            ""in_reply_to_status_id_str"":null,
            ""place"":null,
            ""source"":""web"",
            ""id"":1608673165,
            ""in_reply_to_user_id_str"":null
         },
         ""profile_background_tile"":false,
         ""follow_request_sent"":false,
         ""description"":null,
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""friends_count"":0
      },
      {
         ""id"":132447355,
         ""location"":null,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/sticky\/default_profile_images\/default_profile_5_normal.png"",
         ""following"":false,
         ""default_profile_image"":true,
         ""is_translator"":false,
         ""favourites_count"":1,
         ""profile_background_color"":""C0DEED"",
         ""utc_offset"":14400,
         ""name"":""abcdefg"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""statuses_count"":1,
         ""profile_link_color"":""0084B4"",
         ""protected"":false,
         ""verified"":false,
         ""entities"":{
            ""description"":{
               ""urls"":[

               ]
            }
         },
         ""contributors_enabled"":false,
         ""default_profile"":true,
         ""profile_use_background_image"":true,
         ""created_at"":""Tue Apr 13 07:49:37 +0000 2010"",
         ""time_zone"":""Baku"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_text_color"":""333333"",
         ""id_str"":""132447355"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/sticky\/default_profile_images\/default_profile_5_normal.png"",
         ""followers_count"":33,
         ""geo_enabled"":false,
         ""url"":null,
         ""lang"":""en"",
         ""notifications"":false,
         ""profile_sidebar_border_color"":""C0DEED"",
         ""listed_count"":0,
         ""screen_name"":""abcdefg"",
         ""status"":{
            ""contributors"":null,
            ""coordinates"":null,
            ""geo"":null,
            ""retweeted"":false,
            ""retweet_count"":17,
            ""in_reply_to_user_id"":null,
            ""in_reply_to_status_id"":null,
            ""created_at"":""Tue Apr 13 08:09:41 +0000 2010"",
            ""id_str"":""12093232220"",
            ""in_reply_to_screen_name"":null,
            ""entities"":{
               ""user_mentions"":[

               ],
               ""urls"":[

               ],
               ""hashtags"":[

               ]
            },
            ""favorited"":false,
            ""truncated"":false,
            ""text"":""hi"",
            ""in_reply_to_status_id_str"":null,
            ""place"":null,
            ""source"":""web"",
            ""id"":12093232220,
            ""in_reply_to_user_id_str"":null
         },
         ""profile_background_tile"":false,
         ""follow_request_sent"":false,
         ""description"":null,
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""friends_count"":6
      },
      {
         ""id"":22785157,
         ""location"":""Orange County, CA"",
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1267036839\/pkresume_normal.jpg"",
         ""following"":false,
         ""default_profile_image"":false,
         ""is_translator"":false,
         ""favourites_count"":2,
         ""profile_background_color"":""B2DFDA"",
         ""utc_offset"":-28800,
         ""name"":""Patrick Kerr"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme13\/bg.gif"",
         ""statuses_count"":9823,
         ""profile_link_color"":""93A644"",
         ""protected"":false,
         ""verified"":false,
         ""entities"":{
            ""url"":{
               ""urls"":[
                  {
                     ""display_url"":null,
                     ""expanded_url"":null,
                     ""indices"":[
                        0,
                        31
                     ],
                     ""url"":""http:\/\/www.kerrorganization.com""
                  }
               ]
            },
            ""description"":{
               ""urls"":[

               ]
            }
         },
         ""contributors_enabled"":false,
         ""default_profile"":false,
         ""profile_use_background_image"":true,
         ""created_at"":""Wed Mar 04 15:15:54 +0000 2009"",
         ""time_zone"":""Pacific Time (US & Canada)"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme13\/bg.gif"",
         ""profile_text_color"":""333333"",
         ""id_str"":""22785157"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1267036839\/pkresume_normal.jpg"",
         ""followers_count"":1149,
         ""geo_enabled"":false,
         ""url"":""http:\/\/www.kerrorganization.com"",
         ""lang"":""en"",
         ""notifications"":false,
         ""profile_sidebar_border_color"":""eeeeee"",
         ""listed_count"":11,
         ""screen_name"":""patrickkerr"",
         ""status"":{
            ""contributors"":null,
            ""coordinates"":null,
            ""geo"":null,
            ""retweeted"":false,
            ""retweet_count"":0,
            ""in_reply_to_user_id"":null,
            ""in_reply_to_status_id"":null,
            ""created_at"":""Fri Dec 28 15:00:13 +0000 2012"",
            ""id_str"":""284675125813575680"",
            ""in_reply_to_screen_name"":null,
            ""entities"":{
               ""urls"":[
                  {
                     ""display_url"":""lnkd.in\/_n9Mq5"",
                     ""expanded_url"":""http:\/\/lnkd.in\/_n9Mq5"",
                     ""indices"":[
                        66,
                        86
                     ],
                     ""url"":""http:\/\/t.co\/Fil30ira""
                  }
               ],
               ""user_mentions"":[

               ],
               ""hashtags"":[

               ]
            },
            ""favorited"":false,
            ""truncated"":false,
            ""text"":""Lincoln Property Makes Second SoCal Office Acquisition in a Month http:\/\/t.co\/Fil30ira"",
            ""in_reply_to_status_id_str"":null,
            ""place"":null,
            ""source"":""\u003Ca href=\""http:\/\/www.linkedin.com\/\"" rel=\""nofollow\""\u003ELinkedIn\u003C\/a\u003E"",
            ""id"":284675125813575680,
            ""possibly_sensitive"":false,
            ""in_reply_to_user_id_str"":null
         },
         ""profile_background_tile"":false,
         ""follow_request_sent"":false,
         ""description"":""Patrick Kerr at Kerr Organization, develops & invests in commercial real estate #realestate #properties #buildings #multifamily #investments #homes"",
         ""profile_sidebar_fill_color"":""ffffff"",
         ""friends_count"":155
      }
   ],
   ""next_cursor"":0,
   ""previous_cursor"":0,
   ""next_cursor_str"":""0"",
   ""previous_cursor_str"":""0""
}";

        const string BlockedIDsJson = @"{
   ""previous_cursor"":0,
   ""ids"":[
      35080286,
      132447355,
      22785157,
      238429025,
      256075978
   ],
   ""next_cursor_str"":""0"",
   ""previous_cursor_str"":""0"",
   ""next_cursor"":0
}";
    }
}
