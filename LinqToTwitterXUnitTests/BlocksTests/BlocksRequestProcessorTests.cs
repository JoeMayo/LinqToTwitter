using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using LinqToTwitter;

using LinqToTwitterTests.Common;

using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class BlocksRequestProcessorTests
    {
        public BlocksRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void ProcessResults_Handles_Exists()
        {
            var blocksReqProc = new BlocksRequestProcessor<Blocks>
            {
                Type = BlockingType.Exists
            };

            IList<Blocks> blocksResponse = blocksReqProc.ProcessResults(BlockExistsJson);

            Assert.NotNull(blocksResponse);
            Assert.Equal(blocksResponse.Count(), 1);
        }

        [Fact]
        public void ProcessResults_Handles_IDs()
        {
            var blockReqProc = new BlocksRequestProcessor<Blocks>
            {
                Type = BlockingType.Ids
            };

            IList<Blocks> blocksResponse = blockReqProc.ProcessResults(BlockedIDsJson);

            Assert.NotNull(blocksResponse);
            Blocks blocks = blocksResponse.First();
            List<string> ids = blocks.IDs;
            Assert.Equal(5, ids.Count());
            Assert.Equal("16761255", ids.First());
        }

        [Fact]
        public void ProcessResults_Handles_BlockedUsers()
        {
            var blockedReqProc = new BlocksRequestProcessor<Blocks>
            {
                Type = BlockingType.Blocking
            };

            IList actual = blockedReqProc.ProcessResults(BlockedUsersJson);

            var actualQuery = actual as IList<Blocks>;
            Assert.NotNull(actualQuery);
            Assert.Equal(actualQuery[0].Users.Count, 3);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var blocksReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1/" };

            var blocks = blocksReqProc.ProcessResults(string.Empty);

            Assert.Equal(1, blocks.Count);
        }

        [Fact]
        public void ProcessResults_Retains_Original_Input_Parameters()
        {
            var blockedReqProc = new BlocksRequestProcessor<Blocks>
            {
                Type = BlockingType.Blocking,
                ID = "123",
                ScreenName = "JoeMayo",
                Page = 1,
                PerPage = 10,
                SkipStatus = true,
                Cursor = "789"
            };

            var blocks = blockedReqProc.ProcessResults(BlockedUsersJson);

            Assert.NotNull(blocks);
            Assert.Single(blocks);
            var block = blocks.Single();
            Assert.Equal(BlockingType.Blocking, block.Type);
            Assert.Equal("123", block.ID);
            Assert.Equal("JoeMayo", block.ScreenName);
            Assert.Equal(1, block.Page);
            Assert.Equal(10, block.PerPage);
            Assert.True(block.SkipStatus);
            Assert.Equal("789", block.Cursor);
        }

        [Fact]
        public void GetParameters_Parses_Parameters()
        {
            var blocksReqProc = new BlocksRequestProcessor<Blocks>();
            Expression<Func<Blocks, bool>> expression =
                block =>
                    block.Type == BlockingType.Blocking &&
                    block.ID == "123" &&
                    block.ScreenName == "JoeMayo" &&
                    block.Page == 1 &&
                    block.PerPage == 10 &&
                    block.SkipStatus == true &&
                    block.Cursor == "789";
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = blocksReqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)BlockingType.Blocking).ToString(CultureInfo.InvariantCulture))));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "123")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "JoeMayo")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("PerPage", "10")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SkipStatus", "True")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "789")));
        }

        [Fact]
        public void BuildUrl_Creates_Exists_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/blocks/exists/123.json?user_id=456&screen_name=789&skip_status=true";
            var buildReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)BlockingType.Exists).ToString(CultureInfo.InvariantCulture) },
                    { "ID", "123" },
                    { "UserID", "456" },
                    { "ScreenName", "789" },
                    { "SkipStatus", true.ToString() }
                };

            Request req = buildReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Creates_Blocking_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/blocks/blocking.json?page=2&per_page=10&skip_status=true&cursor=789";
            var blocksReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)BlockingType.Blocking).ToString(CultureInfo.InvariantCulture) },
                    { "Page", "2" },
                    { "PerPage", "10" },
                    { "SkipStatus", true.ToString() },
                    { "Cursor", "789" }
                };

            Request req = blocksReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Creates_BlockingIDs_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/blocks/blocking/ids.json?cursor=789";
            var blocksReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)BlockingType.Ids).ToString(CultureInfo.InvariantCulture) },
                    { "Cursor", "789" }
                };

            Request req = blocksReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_On_Missing_Type_Parameter()
        {
            var blockReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>();

            var ex = Assert.Throws<ArgumentException>(() => blockReqProc.BuildUrl(parameters));
                
            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_On_Null_Parameters()
        {
            var blockReqProc = new BlocksRequestProcessor<Blocks> { BaseUrl = "https://api.twitter.com/1/" };

            var ex = Assert.Throws<ArgumentException>(() => blockReqProc.BuildUrl(null));

            Assert.Equal("Type", ex.ParamName);
        }


        const string BlockedUsersJson = @"[
   {
      ""id"":16761255,
      ""listed_count"":2,
      ""contributors_enabled"":false,
      ""profile_sidebar_border_color"":""87bc44"",
      ""geo_enabled"":false,
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
      ""friends_count"":0,
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
      ""profile_background_tile"":false,
      ""followers_count"":22,
      ""is_translator"":false,
      ""show_all_inline_media"":true,
      ""follow_request_sent"":false,
      ""statuses_count"":100,
      ""utc_offset"":-25200,
      ""profile_sidebar_fill_color"":""e0ff92"",
      ""name"":""LINQ to Tweeter Test"",
      ""default_profile_image"":false,
      ""protected"":false,
      ""profile_background_color"":""9ae4e8"",
      ""favourites_count"":2,
      ""lang"":""en"",
      ""url"":""http:\/\/linqtotwitter.codeplex.com"",
      ""verified"":false,
      ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
      ""description"":""Testing the LINQ to Twitter Account Profile Update."",
      ""profile_link_color"":""0000ff"",
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
      ""default_profile"":false,
      ""following"":false,
      ""profile_use_background_image"":true,
      ""location"":""Anywhere In The World"",
      ""notifications"":false,
      ""id_str"":""16761255"",
      ""profile_text_color"":""000000"",
      ""status"":{
         ""in_reply_to_user_id_str"":null,
         ""annotations"":null,
         ""truncated"":false,
         ""created_at"":""Sun Mar 04 23:16:17 +0000 2012"",
         ""coordinates"":null,
         ""retweeted"":false,
         ""place"":null,
         ""in_reply_to_screen_name"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""favorited"":false,
         ""in_reply_to_user_id"":null,
         ""source"":""\u003Ca href=\""http:\/\/www.csharp-station.com\/\"" rel=\""nofollow\""\u003EC# Station\u003C\/a\u003E"",
         ""in_reply_to_status_id_str"":null,
         ""geo"":null,
         ""in_reply_to_status_id"":null,
         ""id"":176445993091481604,
         ""id_str"":""176445993091481604"",
         ""text"":""Windows Phone Test, 03\/04\/2012 16:15:12 #linq2twitter""
      },
      ""time_zone"":""Mountain Time (US & Canada)"",
      ""screen_name"":""Linq2Tweeter""
   },
   {
      ""id"":427652062,
      ""listed_count"":0,
      ""contributors_enabled"":false,
      ""profile_sidebar_border_color"":""DBE9ED"",
      ""geo_enabled"":false,
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme17\/bg.gif"",
      ""friends_count"":4,
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1671951300\/f_29_w_0018_normal.jpg"",
      ""profile_background_tile"":false,
      ""followers_count"":2,
      ""is_translator"":false,
      ""show_all_inline_media"":false,
      ""follow_request_sent"":false,
      ""statuses_count"":176,
      ""utc_offset"":-32400,
      ""profile_sidebar_fill_color"":""E6F6F9"",
      ""name"":""Randrup Aitkin"",
      ""default_profile_image"":false,
      ""protected"":false,
      ""profile_background_color"":""DBE9ED"",
      ""favourites_count"":0,
      ""lang"":""en"",
      ""url"":null,
      ""verified"":false,
      ""created_at"":""Sat Dec 03 20:08:22 +0000 2011"",
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme17\/bg.gif"",
      ""description"":"""",
      ""profile_link_color"":""CC3366"",
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1671951300\/f_29_w_0018_normal.jpg"",
      ""default_profile"":false,
      ""following"":false,
      ""profile_use_background_image"":true,
      ""location"":"""",
      ""notifications"":false,
      ""id_str"":""427652062"",
      ""profile_text_color"":""333333"",
      ""status"":{
         ""in_reply_to_user_id_str"":""458671214"",
         ""annotations"":null,
         ""truncated"":false,
         ""created_at"":""Fri Mar 30 13:56:57 +0000 2012"",
         ""coordinates"":null,
         ""retweeted"":false,
         ""place"":null,
         ""in_reply_to_screen_name"":""UmYUMadDoe"",
         ""possibly_sensitive"":false,
         ""contributors"":null,
         ""possibly_sensitive_editable"":true,
         ""retweet_count"":0,
         ""favorited"":false,
         ""in_reply_to_user_id"":458671214,
         ""source"":""web"",
         ""in_reply_to_status_id_str"":""185523952989372416"",
         ""geo"":null,
         ""in_reply_to_status_id"":185523952989372416,
         ""id"":185727316322824192,
         ""id_str"":""185727316322824192"",
         ""text"":""@UmYUMadDoe http:\/\/t.co\/HoYui9tl""
      },
      ""time_zone"":""Alaska"",
      ""screen_name"":""randrupxnfyl6""
   },
   {
      ""id"":398317420,
      ""listed_count"":6,
      ""contributors_enabled"":false,
      ""profile_sidebar_border_color"":""CC3366"",
      ""geo_enabled"":false,
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme11\/bg.gif"",
      ""friends_count"":1619,
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1629628675\/1452967202Jill_normal.jpg"",
      ""profile_background_tile"":true,
      ""followers_count"":1513,
      ""is_translator"":false,
      ""show_all_inline_media"":false,
      ""follow_request_sent"":false,
      ""statuses_count"":883,
      ""utc_offset"":null,
      ""profile_sidebar_fill_color"":""E5507E"",
      ""name"":""Livia Nardo"",
      ""default_profile_image"":false,
      ""protected"":false,
      ""profile_background_color"":""FF6699"",
      ""favourites_count"":0,
      ""lang"":""en"",
      ""url"":""http:\/\/cum-facial-pics.net"",
      ""verified"":false,
      ""created_at"":""Tue Oct 25 21:57:31 +0000 2011"",
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme11\/bg.gif"",
      ""description"":"""",
      ""profile_link_color"":""B40B43"",
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1629628675\/1452967202Jill_normal.jpg"",
      ""default_profile"":false,
      ""following"":false,
      ""profile_use_background_image"":true,
      ""location"":""Florida"",
      ""notifications"":false,
      ""id_str"":""398317420"",
      ""profile_text_color"":""362720"",
      ""status"":{
         ""in_reply_to_user_id_str"":null,
         ""truncated"":false,
         ""created_at"":""Sun Apr 08 01:04:57 +0000 2012"",
         ""coordinates"":null,
         ""retweeted"":false,
         ""place"":null,
         ""in_reply_to_screen_name"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""favorited"":false,
         ""in_reply_to_user_id"":null,
         ""source"":""web"",
         ""in_reply_to_status_id_str"":null,
         ""geo"":null,
         ""in_reply_to_status_id"":null,
         ""id"":188794526750617600,
         ""id_str"":""188794526750617600"",
         ""text"":""People grow through experience if they meet life honestly and courageously""
      },
      ""time_zone"":null,
      ""screen_name"":""Flore580Home""
   }
]";

        const string BlockedIDsJson = @"[
   16761255,
   427652062,
   398317420,
   389287543,
   35142832
]";

        const string BlockExistsJson = @"{
   ""id"":16761255,
   ""listed_count"":2,
   ""contributors_enabled"":false,
   ""profile_sidebar_border_color"":""87bc44"",
   ""geo_enabled"":false,
   ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
   ""friends_count"":0,
   ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
   ""profile_background_tile"":false,
   ""followers_count"":22,
   ""is_translator"":false,
   ""show_all_inline_media"":true,
   ""follow_request_sent"":false,
   ""statuses_count"":100,
   ""utc_offset"":-25200,
   ""profile_sidebar_fill_color"":""e0ff92"",
   ""name"":""LINQ to Tweeter Test"",
   ""default_profile_image"":false,
   ""protected"":false,
   ""profile_background_color"":""9ae4e8"",
   ""favourites_count"":2,
   ""lang"":""en"",
   ""url"":""http:\/\/linqtotwitter.codeplex.com"",
   ""verified"":false,
   ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
   ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
   ""description"":""Testing the LINQ to Twitter Account Profile Update."",
   ""profile_link_color"":""0000ff"",
   ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
   ""default_profile"":false,
   ""following"":false,
   ""profile_use_background_image"":true,
   ""location"":""Anywhere In The World"",
   ""notifications"":false,
   ""id_str"":""16761255"",
   ""profile_text_color"":""000000"",
   ""status"":{
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""created_at"":""Sun Mar 04 23:16:17 +0000 2012"",
      ""coordinates"":null,
      ""retweeted"":false,
      ""place"":null,
      ""in_reply_to_screen_name"":null,
      ""contributors"":null,
      ""retweet_count"":0,
      ""favorited"":false,
      ""in_reply_to_user_id"":null,
      ""source"":""\u003Ca href=\""http:\/\/www.csharp-station.com\/\"" rel=\""nofollow\""\u003EC# Station\u003C\/a\u003E"",
      ""in_reply_to_status_id_str"":null,
      ""geo"":null,
      ""in_reply_to_status_id"":null,
      ""id"":176445993091481604,
      ""id_str"":""176445993091481604"",
      ""text"":""Windows Phone Test, 03\/04\/2012 16:15:12 #linq2twitter""
   },
   ""time_zone"":""Mountain Time (US & Canada)"",
   ""screen_name"":""Linq2Tweeter""
}";
    }
}
