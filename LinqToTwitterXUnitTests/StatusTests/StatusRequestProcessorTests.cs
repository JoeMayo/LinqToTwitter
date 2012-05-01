using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.StatusTests
{
    public class StatusRequestProcessorTests
    {
        public StatusRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void StatusRequestProcessor_Works_With_Json_Format_Data()
        {
            var statProc = new StatusRequestProcessor<Status> { BaseUrl = "https://api.twitter.com/1/" };

            Assert.IsAssignableFrom<IRequestProcessorWantsJson>(statProc);
        }

        [Fact]
        public void BuildUrl_Constructs_Mentions_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/statuses/mentions.json?since_id=123&max_id=145&count=50&page=1";
            var statProc = new StatusRequestProcessor<Status> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.Mentions).ToString() },
                { "SinceID", "123" },
                { "MaxID", "145" },
                { "Count", "50" },
                { "Page", "1" }
            };

            Request req = statProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void ProcessResultsMultipleResultsTest()
        {
            var statProc = new StatusRequestProcessor<Status> { Type = StatusType.Home, BaseUrl = "https://api.twitter.com/1/" };

            var statuses = statProc.ProcessResults(MultipleStatusResponse);

            Assert.NotNull(statuses);
            Assert.Equal(3, statuses.Count);
            var status = statuses.First();
            Assert.NotNull(status);
            Assert.False(status.Retweeted);
            Assert.Null(status.InReplyToScreenName);
            Assert.False(status.PossiblySensitive);
            var retweetedStatus = status.RetweetedStatus;
            Assert.NotNull(retweetedStatus);
            Assert.NotNull(retweetedStatus.Text);
            Assert.True(retweetedStatus.Text.StartsWith("I just blogged about"));
            Assert.NotNull(status.Contributors);
            Assert.Empty(status.Contributors);
            Assert.NotNull(status.Coordinates);
            Assert.Equal(0, status.Coordinates.Latitude);
            Assert.NotNull(status.Place);
            Assert.Null(status.Place.Name);
            Assert.NotNull(status.User);
            Assert.Equal("Joe Mayo", status.User.Name);
            Assert.Equal(393, status.RetweetCount);
            Assert.Equal("184835136037191681", status.StatusID);
            Assert.Null(status.InReplyToUserID);
            Assert.False(status.Favorited);
            Assert.Null(status.InReplyToStatusID);
            Assert.Equal("web", status.Source);
            Assert.Equal(new DateTime(2012, 3, 28, 2, 51, 45), status.CreatedAt);
            Assert.Null(status.InReplyToUserID);
            Assert.False(status.Truncated);
            Assert.NotNull(status.Geo);
            Assert.Null(status.Geo.Query);
            Assert.NotNull(status.Text);
            Assert.True(status.Text.StartsWith("RT @scottgu: I just blogged about"));
            Assert.NotNull(status.Annotation);
            Assert.Empty(status.Annotation.Attributes);
            Assert.NotNull(status.Entities);
            Assert.Null(status.Entities.HashTagMentions);
        }

        [Fact]
        public void ProcessResultsSingleResultTest()
        {
            var statProc = new StatusRequestProcessor<Status> { Type = StatusType.Show, BaseUrl = "https://api.twitter.com/1/" };

            var statuses = statProc.ProcessResults(SingleStatusResponse);

            Assert.NotNull(statuses);
            Assert.Single(statuses);
            var status = statuses.Single();
            Assert.NotNull(status);
            Assert.False(status.Retweeted);
            Assert.Null(status.InReplyToScreenName);
            Assert.False(status.PossiblySensitive);
            var retweetedStatus = status.RetweetedStatus;
            Assert.NotNull(retweetedStatus);
            Assert.NotNull(retweetedStatus.Text);
            Assert.True(retweetedStatus.Text.StartsWith("I just blogged about"));
            Assert.NotNull(status.Contributors);
            Assert.Empty(status.Contributors);
            Assert.NotNull(status.Coordinates);
            Assert.Equal(0, status.Coordinates.Latitude);
            Assert.NotNull(status.Place);
            Assert.Null(status.Place.Name);
            Assert.NotNull(status.User);
            Assert.Equal("Joe Mayo", status.User.Name);
            Assert.Equal(393, status.RetweetCount);
            Assert.Equal("184835136037191681", status.StatusID);
            Assert.Null(status.InReplyToUserID);
            Assert.False(status.Favorited);
            Assert.Null(status.InReplyToStatusID);
            Assert.Equal("web", status.Source);
            Assert.Equal(new DateTime(2012, 3, 28, 2, 51, 45), status.CreatedAt);
            Assert.Null(status.InReplyToUserID);
            Assert.False(status.Truncated);
            Assert.NotNull(status.Geo);
            Assert.Null(status.Geo.Query);
            Assert.NotNull(status.Text);
            Assert.True(status.Text.StartsWith("RT @scottgu: I just blogged about"));
            Assert.NotNull(status.Annotation);
            Assert.Empty(status.Annotation.Attributes);
            Assert.NotNull(status.Entities);
            Assert.Null(status.Entities.HashTagMentions);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var statProc = new StatusRequestProcessor<Status>() { BaseUrl = "https://api.twitter.com/1/" };

            var stats = statProc.ProcessResults(string.Empty);

            Assert.Equal(0, stats.Count);
        }

        [Fact]
        public void GetParameters_Handles_Input_Params()
        {
            var reqProc = new StatusRequestProcessor<Status>();

            Expression<Func<Status, bool>> expression =
            status =>
                status.Type == StatusType.Home &&
                status.ID == "10" &&
                status.UserID == "10" &&
                status.ScreenName == "JoeMayo" &&
                status.SinceID == 123 &&
                status.MaxID == 456 &&
                status.Count == 50 &&
                status.Page == 2 &&
                status.IncludeRetweets == true &&
                status.ExcludeReplies == true &&
                status.IncludeEntities == true &&
                status.TrimUser == true &&
                status.IncludeContributorDetails == true;

            var lambdaExpression = expression as LambdaExpression;

            var queryParams = reqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)StatusType.Home).ToString())));
            Assert.True(
               queryParams.Contains(
                   new KeyValuePair<string, string>("ID", "10")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("UserID", "10")));
            Assert.True(
               queryParams.Contains(
                   new KeyValuePair<string, string>("ScreenName", "JoeMayo")));
            Assert.True(
               queryParams.Contains(
                   new KeyValuePair<string, string>("SinceID", "123")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("MaxID", "456")));
            Assert.True(
               queryParams.Contains(
                   new KeyValuePair<string, string>("Count", "50")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Page", "2")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Page", "2")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("IncludeRetweets", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("ExcludeReplies", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("IncludeEntities", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("TrimUser", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("IncludeContributorDetails", "True")));
        }

        [Fact]
        public void BuildUrl_Constructs_Show_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/statuses/show/945932078.json";
            var reqProc = new StatusRequestProcessor<Status> 
            { 
                Type = StatusType.Show,
                BaseUrl = "https://api.twitter.com/1/" 
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.Show).ToString() },
                { "ID", "945932078" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_User_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/statuses/user_timeline/15411837.json?user_id=15411837&screen_name=JoeMayo";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.User).ToString() },
                { "ID", "15411837" },
                { "UserID", "15411837" },
                { "ScreenName", "JoeMayo" },
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Returns_Url_For_Retweets()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/statuses/user_timeline/15411837.json?include_rts=true";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.User).ToString() },
                { "ID", "15411837" },
                { "IncludeRetweets", "True" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact()]
        public void BuildUrl_Returns_Url_Without_Include_Rts_Param_For_False_Retweets()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/statuses/user_timeline/15411837.json";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.User).ToString() },
                { "ID", "15411837" },
                { "IncludeRetweets", "False" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_Url_For_RetweetedByUser()
        {
            const string ExpectedUrl = "http://api.twitter.com/1/statuses/retweeted_by_user.json?screen_name=JoeMayo";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.RetweetedByUser,
                BaseUrl = "http://api.twitter.com/1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.RetweetedByUser).ToString() },
                { "ID", "15411837" },
                { "ScreenName", "JoeMayo" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact()]
        public void BuildUrl_Constructs_Url_For_RetweetedToUser()
        {
            const string ExpectedUrl = "http://api.twitter.com/1/statuses/retweeted_to_user.json?screen_name=JoeMayo";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.RetweetedToUser,
                BaseUrl = "http://api.twitter.com/1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.RetweetedToUser).ToString() },
                { "ID", "15411837" },
                { "ScreenName", "JoeMayo" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_On_Missing_Type()
        {
            var statusReqProc = new StatusRequestProcessor<Status> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string> { };

            var ex = Assert.Throws<ArgumentException>(() => statusReqProc.BuildUrl(parameters));

            Assert.Equal<string>("Type", ex.ParamName);
        }

        [Fact]
        public void NullParametersTest()
        {
            var target = new StatusRequestProcessor<Status> { BaseUrl = "https://api.twitter.com/1/" };

            var ex = Assert.Throws<ArgumentException>(() => target.BuildUrl(null));

            Assert.Equal<string>("Type", ex.ParamName);
        }

        const string SingleStatusResponse = @"{
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""retweeted_status"":{
         ""retweeted"":false,
         ""in_reply_to_screen_name"":null,
         ""possibly_sensitive"":false,
         ""contributors"":null,
         ""coordinates"":null,
         ""place"":null,
         ""user"":{
            ""id"":41754227,
            ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""url"":""http:\/\/weblogs.asp.net\/scottgu"",
            ""created_at"":""Fri May 22 04:39:35 +0000 2009"",
            ""followers_count"":57222,
            ""default_profile"":true,
            ""profile_background_color"":""C0DEED"",
            ""lang"":""en"",
            ""utc_offset"":-28800,
            ""name"":""Scott Guthrie"",
            ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""location"":""Redmond, WA"",
            ""profile_link_color"":""0084B4"",
            ""listed_count"":4390,
            ""verified"":false,
            ""protected"":false,
            ""profile_use_background_image"":true,
            ""is_translator"":false,
            ""following"":false,
            ""description"":""I live in Seattle and build a few products for Microsoft"",
            ""profile_text_color"":""333333"",
            ""statuses_count"":3054,
            ""screen_name"":""scottgu"",
            ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""time_zone"":""Pacific Time (US & Canada)"",
            ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""friends_count"":86,
            ""default_profile_image"":false,
            ""contributors_enabled"":false,
            ""profile_sidebar_border_color"":""C0DEED"",
            ""id_str"":""41754227"",
            ""geo_enabled"":false,
            ""favourites_count"":44,
            ""profile_background_tile"":false,
            ""notifications"":false,
            ""show_all_inline_media"":false,
            ""profile_sidebar_fill_color"":""DDEEF6"",
            ""follow_request_sent"":false
         },
         ""retweet_count"":393,
         ""id_str"":""184793217231880192"",
         ""in_reply_to_user_id"":null,
         ""favorited"":false,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_status_id"":null,
         ""source"":""web"",
         ""created_at"":""Wed Mar 28 00:05:10 +0000 2012"",
         ""in_reply_to_user_id_str"":null,
         ""truncated"":false,
         ""id"":184793217231880192,
         ""geo"":null,
         ""text"":""I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
      },
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":15411837,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""url"":""http:\/\/www.mayosoftware.com"",
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""followers_count"":1102,
         ""default_profile"":false,
         ""profile_background_color"":""0099B9"",
         ""lang"":""en"",
         ""utc_offset"":-25200,
         ""name"":""Joe Mayo"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0099B9"",
         ""listed_count"":112,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":true,
         ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
         ""profile_text_color"":""3C3940"",
         ""statuses_count"":1906,
         ""screen_name"":""JoeMayo"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""friends_count"":211,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""5ED4DC"",
         ""id_str"":""15411837"",
         ""geo_enabled"":true,
         ""favourites_count"":44,
         ""profile_background_tile"":false,
         ""notifications"":true,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""95E8EC"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":393,
      ""id_str"":""184835136037191681"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""source"":""web"",
      ""created_at"":""Wed Mar 28 02:51:45 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":184835136037191681,
      ""geo"":null,
      ""text"":""RT @scottgu: I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
   }";

        const string MultipleStatusResponse = @"[
   {
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""retweeted_status"":{
         ""retweeted"":false,
         ""in_reply_to_screen_name"":null,
         ""possibly_sensitive"":false,
         ""contributors"":null,
         ""coordinates"":null,
         ""place"":null,
         ""user"":{
            ""id"":41754227,
            ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""url"":""http:\/\/weblogs.asp.net\/scottgu"",
            ""created_at"":""Fri May 22 04:39:35 +0000 2009"",
            ""followers_count"":57222,
            ""default_profile"":true,
            ""profile_background_color"":""C0DEED"",
            ""lang"":""en"",
            ""utc_offset"":-28800,
            ""name"":""Scott Guthrie"",
            ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""location"":""Redmond, WA"",
            ""profile_link_color"":""0084B4"",
            ""listed_count"":4390,
            ""verified"":false,
            ""protected"":false,
            ""profile_use_background_image"":true,
            ""is_translator"":false,
            ""following"":false,
            ""description"":""I live in Seattle and build a few products for Microsoft"",
            ""profile_text_color"":""333333"",
            ""statuses_count"":3054,
            ""screen_name"":""scottgu"",
            ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""time_zone"":""Pacific Time (US & Canada)"",
            ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""friends_count"":86,
            ""default_profile_image"":false,
            ""contributors_enabled"":false,
            ""profile_sidebar_border_color"":""C0DEED"",
            ""id_str"":""41754227"",
            ""geo_enabled"":false,
            ""favourites_count"":44,
            ""profile_background_tile"":false,
            ""notifications"":false,
            ""show_all_inline_media"":false,
            ""profile_sidebar_fill_color"":""DDEEF6"",
            ""follow_request_sent"":false
         },
         ""retweet_count"":393,
         ""id_str"":""184793217231880192"",
         ""in_reply_to_user_id"":null,
         ""favorited"":false,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_status_id"":null,
         ""source"":""web"",
         ""created_at"":""Wed Mar 28 00:05:10 +0000 2012"",
         ""in_reply_to_user_id_str"":null,
         ""truncated"":false,
         ""id"":184793217231880192,
         ""geo"":null,
         ""text"":""I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
      },
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":15411837,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""url"":""http:\/\/www.mayosoftware.com"",
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""followers_count"":1102,
         ""default_profile"":false,
         ""profile_background_color"":""0099B9"",
         ""lang"":""en"",
         ""utc_offset"":-25200,
         ""name"":""Joe Mayo"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0099B9"",
         ""listed_count"":112,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":true,
         ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
         ""profile_text_color"":""3C3940"",
         ""statuses_count"":1906,
         ""screen_name"":""JoeMayo"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""friends_count"":211,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""5ED4DC"",
         ""id_str"":""15411837"",
         ""geo_enabled"":true,
         ""favourites_count"":44,
         ""profile_background_tile"":false,
         ""notifications"":true,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""95E8EC"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":393,
      ""id_str"":""184835136037191681"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""source"":""web"",
      ""created_at"":""Wed Mar 28 02:51:45 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":184835136037191681,
      ""geo"":null,
      ""text"":""RT @scottgu: I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
   },
   {
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":15411837,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""url"":""http:\/\/www.mayosoftware.com"",
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""followers_count"":1102,
         ""default_profile"":false,
         ""profile_background_color"":""0099B9"",
         ""lang"":""en"",
         ""utc_offset"":-25200,
         ""name"":""Joe Mayo"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0099B9"",
         ""listed_count"":112,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":true,
         ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
         ""profile_text_color"":""3C3940"",
         ""statuses_count"":1906,
         ""screen_name"":""JoeMayo"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""friends_count"":211,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""5ED4DC"",
         ""id_str"":""15411837"",
         ""geo_enabled"":true,
         ""favourites_count"":44,
         ""profile_background_tile"":false,
         ""notifications"":true,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""95E8EC"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":0,
      ""id_str"":""184374428111601664"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""source"":""web"",
      ""created_at"":""Mon Mar 26 20:21:03 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":184374428111601664,
      ""geo"":null,
      ""text"":""Speaking at Twin Cities Code Camp: http:\/\/t.co\/3tJz5vPW #tccc12""
   },
   {
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":15411837,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""url"":""http:\/\/www.mayosoftware.com"",
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""followers_count"":1102,
         ""default_profile"":false,
         ""profile_background_color"":""0099B9"",
         ""lang"":""en"",
         ""utc_offset"":-25200,
         ""name"":""Joe Mayo"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0099B9"",
         ""listed_count"":112,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":true,
         ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
         ""profile_text_color"":""3C3940"",
         ""statuses_count"":1906,
         ""screen_name"":""JoeMayo"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""friends_count"":211,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""5ED4DC"",
         ""id_str"":""15411837"",
         ""geo_enabled"":true,
         ""favourites_count"":44,
         ""profile_background_tile"":false,
         ""notifications"":true,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""95E8EC"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":1,
      ""id_str"":""183620070084325376"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""source"":""web"",
      ""created_at"":""Sat Mar 24 18:23:30 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":183620070084325376,
      ""geo"":null,
      ""text"":""Free ebook: Introducing Microsoft SQL Server 2012: http:\/\/t.co\/VZ52WIZf""
   }
]";
    }
}
