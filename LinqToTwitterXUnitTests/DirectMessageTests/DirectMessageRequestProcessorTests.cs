using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using LinqToTwitter;

using LinqToTwitterXUnitTests.Common;

using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class DirectMessageRequestProcessorTests
    {
        public DirectMessageRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void BuildUrl_Constructs_SentTo_Url()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage> { BaseUrl = "https://api.twitter.com/1/" };
            const string Expected = "https://api.twitter.com/1/direct_messages.json?since_id=1234567&max_id=357&page=1&count=2&skip_status=true";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageType.SentTo).ToString(CultureInfo.InvariantCulture) },
                        { "SinceID", "1234567" },
                        { "MaxID", "357" },
                        { "Page", "1" },
                        { "Count", "2" },
                        { "SkipStatus", true.ToString() }
                };

            Request req = dmReqProc.BuildUrl(parameters);

            Assert.Equal(Expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_SentBy_Url()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage> { BaseUrl = "https://api.twitter.com/1/" };
            const string Expected = "https://api.twitter.com/1/direct_messages/sent.json?since_id=1234567&max_id=357&page=1&count=2";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageType.SentBy).ToString(CultureInfo.InvariantCulture) },
                        { "SinceID", "1234567" },
                        { "MaxID", "357" },
                        { "Page", "1" },
                        { "Count", "2" }
                };

            Request req = dmReqProc.BuildUrl(parameters);

            Assert.Equal(Expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_Show_Url()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage> { BaseUrl = "https://api.twitter.com/1/" };
            const string Expected = "https://api.twitter.com/1/direct_messages/show/478805447.json";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageType.Show).ToString(CultureInfo.InvariantCulture) },
                        { "ID", "478805447" },
                };

            Request req = dmReqProc.BuildUrl(parameters);

            Assert.Equal(Expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Show_Requires_ID()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageType.Show).ToString(CultureInfo.InvariantCulture) },
                        //{ "ID", "478805447" },
                };

            var ex = Assert.Throws<ArgumentNullException>(() => dmReqProc.BuildUrl(parameters));
            
            Assert.Equal("ID", ex.ParamName);
        }

        [Fact]
        public void ProcessResults_Handles_Response()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage> { BaseUrl = "https://api.twitter.com/1/" };

            var actual = dmReqProc.ProcessResults(TestQueryResponse);

            var actualQuery = actual as IList<DirectMessage>;
            Assert.NotNull(actualQuery);
            Assert.Equal(1, actualQuery.Count());
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage> { BaseUrl = "https://api.twitter.com/1/" };

            var dms = dmReqProc.ProcessResults(string.Empty);

            Assert.False(dms.Any());
        }

        [Fact]
        public void ProcessResults_Handles_Single_Result_For_Show()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage> 
            { 
                BaseUrl = "https://api.twitter.com/1/",
                Type = DirectMessageType.Show
            };

            var dms = dmReqProc.ProcessResults(TestQuerySingleResponse);

            Assert.Single(dms);
        }

        [Fact]
        public void ProcessResults_Replaces_Input_Params_For_Show()
        {
            const ulong SinceID = 1;
            const ulong MaxID = 2;
            const int Page = 3;
            const int Count = 4;
            const ulong ID = 5;
            const bool SkipStatus = true;

            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage>
            {
                BaseUrl = "https://api.twitter.com/1/",
                Type = DirectMessageType.Show,
                SinceID = SinceID,
                MaxID = MaxID,
                Page = Page,
                Count = Count,
                ID = ID,
                SkipStatus = SkipStatus
            };

            var dms = dmReqProc.ProcessResults(TestQuerySingleResponse);

            var dm = dms.First();

            Assert.Equal(SinceID, dm.SinceID);
            Assert.Equal(MaxID, dm.MaxID);
            Assert.Equal(Page, dm.Page);
            Assert.Equal(Count, dm.Count);
            Assert.Equal(ID, dm.ID);
            Assert.Equal(SkipStatus, dm.SkipStatus);
        }

        [Fact]
        public void ProcessResults_Replaces_Input_Params_For_SentBy_And_SentTo()
        {
            const ulong SinceID = 1;
            const ulong MaxID = 2;
            const int Page = 3;
            const int Count = 4;
            const ulong ID = 5;
            const bool SkipStatus = true;

            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage>
            {
                BaseUrl = "https://api.twitter.com/1/",
                Type = DirectMessageType.SentBy,
                SinceID = SinceID,
                MaxID = MaxID,
                Page = Page,
                Count = Count,
                ID = ID,
                SkipStatus = SkipStatus
            };

            var dms = dmReqProc.ProcessResults(TestQueryResponse);

            var dm = dms.First();

            Assert.Equal(SinceID, dm.SinceID);
            Assert.Equal(MaxID, dm.MaxID);
            Assert.Equal(Page, dm.Page);
            Assert.Equal(Count, dm.Count);
            Assert.Equal(ID, dm.ID);
            Assert.Equal(SkipStatus, SkipStatus);
        }

        [Fact]
        public void GetParameters_Returns_Parameters()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage>();
            Expression<Func<DirectMessage, bool>> expression =
                dm =>
                    dm.Type == DirectMessageType.Show &&
                    dm.Count == 1 &&
                    dm.MaxID == 789 &&
                    dm.Page == 1 &&
                    dm.SinceID == 123 &&
                    dm.ID == 456 &&
                    dm.SkipStatus == true;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = dmReqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)DirectMessageType.Show).ToString(CultureInfo.InvariantCulture))));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Count", "1")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxID", "789")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SinceID", "123")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "456")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SkipStatus", "True")));
        }

        [Fact]
        public void BuildUrl_Throws_On_Missing_Type_Param()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>();

            var ex = Assert.Throws<ArgumentException>(() => dmReqProc.BuildUrl(parameters));

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_On_Null_Param_Dictionary()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage> { BaseUrl = "https://api.twitter.com/1/" };

            var ex = Assert.Throws<ArgumentException>(() => dmReqProc.BuildUrl(null));

            Assert.Equal("Type", ex.ParamName);
        }

        const string TestQueryResponse = @"[
   {
      ""recipient"":{
         ""id"":16761255,
         ""listed_count"":2,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""87bc44"",
         ""geo_enabled"":false,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
         ""friends_count"":1,
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
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""screen_name"":""Linq2Tweeter""
      },
      ""created_at"":""Sun Apr 08 20:26:00 +0000 2012"",
      ""entities"":{
         ""user_mentions"":[

         ],
         ""urls"":[

         ],
         ""hashtags"":[

         ]
      },
      ""sender_id"":15411837,
      ""sender_screen_name"":""JoeMayo"",
      ""recipient_screen_name"":""Linq2Tweeter"",
      ""recipient_id"":16761255,
      ""id"":189086715040903168,
      ""sender"":{
         ""id"":15411837,
         ""listed_count"":108,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""5ED4DC"",
         ""geo_enabled"":true,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""friends_count"":204,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""profile_background_tile"":false,
         ""followers_count"":1083,
         ""is_translator"":false,
         ""show_all_inline_media"":false,
         ""follow_request_sent"":false,
         ""statuses_count"":1867,
         ""utc_offset"":-25200,
         ""profile_sidebar_fill_color"":""95E8EC"",
         ""name"":""Joe Mayo"",
         ""default_profile_image"":false,
         ""protected"":false,
         ""profile_background_color"":""0099B9"",
         ""favourites_count"":39,
         ""lang"":""en"",
         ""url"":""http:\/\/linqtotwitter.codeplex.com\/"",
         ""verified"":false,
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP; interests: Windows 8."",
         ""profile_link_color"":""0099B9"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""default_profile"":false,
         ""following"":true,
         ""profile_use_background_image"":true,
         ""location"":""Denver, CO"",
         ""notifications"":false,
         ""id_str"":""15411837"",
         ""profile_text_color"":""3C3940"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""screen_name"":""JoeMayo""
      },
      ""text"":""This is a test."",
      ""id_str"":""189086715040903168""
   }
]";

        const string TestQuerySingleResponse = @"{
   ""recipient"":{
      ""id"":16761255,
      ""profile_background_tile"":false,
      ""listed_count"":3,
      ""profile_sidebar_fill_color"":""e0ff92"",
      ""location"":""Anywhere In The World"",
      ""utc_offset"":-25200,
      ""name"":""LINQ to Tweeter Test"",
      ""is_translator"":false,
      ""time_zone"":""Mountain Time (US & Canada)"",
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
      ""contributors_enabled"":false,
      ""profile_background_color"":""9ae4e8"",
      ""protected"":false,
      ""geo_enabled"":false,
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
      ""friends_count"":1,
      ""lang"":""en"",
      ""default_profile_image"":false,
      ""favourites_count"":2,
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
      ""statuses_count"":100,
      ""show_all_inline_media"":true,
      ""profile_link_color"":""0000ff"",
      ""description"":""Testing the LINQ to Twitter Account Profile Update."",
      ""follow_request_sent"":false,
      ""verified"":false,
      ""screen_name"":""Linq2Tweeter"",
      ""profile_use_background_image"":true,
      ""id_str"":""16761255"",
      ""notifications"":false,
      ""following"":false,
      ""profile_text_color"":""000000"",
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
      ""default_profile"":false,
      ""url"":""http:\/\/linqtotwitter.codeplex.com"",
      ""followers_count"":22,
      ""profile_sidebar_border_color"":""87bc44"",
      ""created_at"":""Wed Oct 15 05:15:40 +0000 2008""
   },
   ""sender"":{
      ""id"":15411837,
      ""profile_background_tile"":false,
      ""listed_count"":111,
      ""profile_sidebar_fill_color"":""95E8EC"",
      ""location"":""Denver, CO"",
      ""utc_offset"":-25200,
      ""name"":""Joe Mayo"",
      ""is_translator"":false,
      ""time_zone"":""Mountain Time (US & Canada)"",
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
      ""contributors_enabled"":false,
      ""profile_background_color"":""0099B9"",
      ""protected"":false,
      ""geo_enabled"":true,
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
      ""friends_count"":214,
      ""lang"":""en"",
      ""default_profile_image"":false,
      ""favourites_count"":39,
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
      ""statuses_count"":1891,
      ""show_all_inline_media"":false,
      ""profile_link_color"":""0099B9"",
      ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
      ""follow_request_sent"":false,
      ""verified"":false,
      ""screen_name"":""JoeMayo"",
      ""profile_use_background_image"":true,
      ""id_str"":""15411837"",
      ""notifications"":false,
      ""following"":false,
      ""profile_text_color"":""3C3940"",
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
      ""default_profile"":false,
      ""url"":""http:\/\/www.mayosoftware.com"",
      ""followers_count"":1092,
      ""profile_sidebar_border_color"":""5ED4DC"",
      ""created_at"":""Sun Jul 13 04:35:50 +0000 2008""
   },
   ""created_at"":""Sat Apr 21 03:23:55 +0000 2012"",
   ""recipient_screen_name"":""Linq2Tweeter"",
   ""recipient_id"":16761255,
   ""id_str"":""193540539663126529"",
   ""id"":193540539663126529,
   ""sender_screen_name"":""JoeMayo"",
   ""sender_id"":15411837,
   ""text"":""Direct Message Test - 4\/20\/2012 9:25:06 PM""
}";

    }
}
