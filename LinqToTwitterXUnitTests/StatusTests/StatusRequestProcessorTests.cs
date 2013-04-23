using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
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
                status.IncludeUserEntities == true &&
                status.TrimUser == true &&
                status.IncludeContributorDetails == true &&
                status.IncludeMyRetweet == true &&
                status.OEmbedUrl == "http://myurl.com" &&
                status.OEmbedAlign == EmbeddedStatusAlignment.Center &&
                status.OEmbedHideMedia == true &&
                status.OEmbedHideThread == true &&
                status.OEmbedLanguage == "en" &&
                status.OEmbedMaxWidth == 300 &&
                status.OEmbedOmitScript == true &&
                status.OEmbedRelated == "JoeMayo";

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
                  new KeyValuePair<string, string>("IncludeUserEntities", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("OEmbedRelated", "JoeMayo")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("TrimUser", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("IncludeContributorDetails", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("IncludeMyRetweet", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("OEmbedUrl", "http://myurl.com")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("OEmbedAlign", ((int)EmbeddedStatusAlignment.Center).ToString())));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("OEmbedHideMedia", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("OEmbedHideThread", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("OEmbedLanguage", "en")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("OEmbedMaxWidth", "300")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("OEmbedOmitScript", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("OEmbedRelated", "JoeMayo")));
        }

        [Fact]
        public void BuildUrl_Constructs_Mentions_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/mentions_timeline.json?since_id=123&max_id=145&count=50&page=1";
            var statProc = new StatusRequestProcessor<Status> { BaseUrl = "https://api.twitter.com/1.1/" };
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
        public void BuildUrl_Constructs_Show_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/show.json?id=945932078&include_my_retweet=true&include_entities=true&trim_user=true";
            var reqProc = new StatusRequestProcessor<Status> 
            { 
                Type = StatusType.Show,
                BaseUrl = "https://api.twitter.com/1.1/" 
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.Show).ToString() },
                { "ID", "945932078" },
                { "TrimUser", true.ToString() },
                { "IncludeMyRetweet", true.ToString() },
                { "IncludeEntities", true.ToString() }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_User_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json?id=15411837&user_id=15411837&screen_name=JoeMayo";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1.1/"
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
        public void BuildUrl_Constructs_Home_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/home_timeline.json?count=5";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.Home).ToString() },
                { "Count", "5" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Returns_Url_For_Retweets()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/retweets/15411837.json";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.Retweets).ToString() },
                { "ID", "15411837" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Returns_Url_For_IncludeRetweets_On_User_Timeline()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json?id=15411837&include_rts=true";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1.1/"
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

        [Fact]
        public void BuildUrl_Includes_False_Include_Rts_Param()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json?id=15411837&include_rts=false";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.User).ToString() },
                { "ID", "15411837" },
                { "IncludeRetweets", false.ToString() }
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
                { "ScreenName", "JoeMayo" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Returns_Url_For_OEmbed()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/oembed.json?id=1&url=abc&maxwidth=300&hide_media=true&hide_thread=true&omit_script=true&align=left&related=JoeMayo%2CTwitterAPI&lang=en";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.Oembed,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.Oembed).ToString() },
                { "ID", "1" },
                { "OEmbedUrl", "abc" },
                { "OEmbedMaxWidth", "300" },
                { "OEmbedHideMedia", true.ToString() },
                { "OEmbedHideThread", true.ToString() },
                { "OEmbedOmitScript", true.ToString() },
                { "OEmbedAlign", ((int)EmbeddedStatusAlignment.Left).ToString() },
                { "OEmbedRelated", "JoeMayo, TwitterAPI" },
                { "OEmbedLanguage", "en" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Returns_Url_For_RetweetsOfMe()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/retweets_of_me.json?since_id=2&max_id=3&count=1&include_entities=true&include_user_entities=true&trim_user=true";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.Oembed,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.RetweetsOfMe).ToString() },
                { "Count", "1" },
                { "SinceID", "2" },
                { "MaxID", "3" },
                { "TrimUser", true.ToString() },
                { "IncludeEntities", true.ToString() },
                { "IncludeUserEntities", true.ToString() },
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Includes_False_IncludeUserEntities_Param()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/retweets_of_me.json?include_user_entities=false";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.RetweetsOfMe).ToString() },
                { "IncludeUserEntities", false.ToString() },
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_RetweetedBy_Throws_On_Missing_ID()
        {
            const string ExpectedParam = "ID";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.RetweetedBy,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.RetweetedBy).ToString() },
                //{ "ID", "123" },
                { "Count", "25" },
                { "Page", "2" }
            };

            var ex = Assert.Throws<ArgumentException>(() => reqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParam, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_RetweetedBy_Throws_On_Count_Over_100()
        {
            const string ExpectedParam = "Count";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.RetweetedBy,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.RetweetedBy).ToString() },
                { "ID", "123" },
                { "Count", "101" },
                { "Page", "2" }
            };

            var ex = Assert.Throws<ArgumentException>(() => reqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParam, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_On_Missing_Type()
        {
            var statusReqProc = new StatusRequestProcessor<Status> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string> { };

            var ex = Assert.Throws<ArgumentException>(() => statusReqProc.BuildUrl(parameters));

            Assert.Equal<string>("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_On_Null_Parameter()
        {
            var target = new StatusRequestProcessor<Status> { BaseUrl = "https://api.twitter.com/1.1/" };

            var ex = Assert.Throws<ArgumentException>(() => target.BuildUrl(null));

            Assert.Equal<string>("Type", ex.ParamName);
        }

        [Fact]
        public void StatusRequestProcessor_Works_With_Json_Format_Data()
        {
            var statProc = new StatusRequestProcessor<Status> { BaseUrl = "https://api.twitter.com/1.1/" };

            Assert.IsAssignableFrom<IRequestProcessorWantsJson>(statProc);
        }

        [Fact]
        public void ProcessResults_Handles_Multiple_Statuses()
        {
            var statProc = new StatusRequestProcessor<Status> { Type = StatusType.Home, BaseUrl = "https://api.twitter.com/1.1/" };

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
            var contributors = status.Contributors;
            Assert.NotNull(contributors);
            Assert.Empty(contributors);
            var coords = status.Coordinates;
            Assert.NotNull(coords);
            Assert.Equal(-122.40060, coords.Longitude);
            Assert.Equal(37.78215, coords.Latitude);
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
            Assert.NotNull(status.Text);
            Assert.True(status.Text.StartsWith("RT @scottgu: I just blogged about"));
            Assert.NotNull(status.Annotation);
            Assert.Empty(status.Annotation.Attributes);
            Assert.NotNull(status.Entities);
            Assert.Null(status.Entities.HashTagEntities);
        }

        [Fact]
        public void ProcessResults_Handles_A_Single_Status()
        {
            var statProc = new StatusRequestProcessor<Status> { Type = StatusType.Show, BaseUrl = "https://api.twitter.com/1.1/" };

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
            var contributors = status.Contributors;
            Assert.NotNull(contributors);
            Assert.Empty(contributors);
            var coords = status.Coordinates;
            Assert.NotNull(coords);
            Assert.Equal(-122.40060, coords.Longitude);
            Assert.Equal(37.78215, coords.Latitude);
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
            Assert.NotNull(status.Text);
            Assert.True(status.Text.StartsWith("RT @scottgu: I just blogged about"));
            Assert.NotNull(status.Annotation);
            Assert.Empty(status.Annotation.Attributes);
            Assert.NotNull(status.Entities);
            Assert.Null(status.Entities.HashTagEntities);
        }

        [Fact]
        public void ProcessResults_Handles_Multiple_Users()
        {
            var statProc = new StatusRequestProcessor<Status> { Type = StatusType.RetweetedBy, BaseUrl = "https://api.twitter.com/1.1/" };

            var statuses = statProc.ProcessResults(MultipleUsersResponse);

            Assert.NotNull(statuses);
            Assert.Single(statuses);
            var status = statuses.Single();
            Assert.NotNull(status);
            var users = status.Users;
            Assert.NotNull(users);
            Assert.Equal(5, users.Count);
            var user = users.First();
            Assert.NotNull(user.Identifier);
            Assert.Equal("gcaughey", user.Identifier.ScreenName);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var statProc = new StatusRequestProcessor<Status>() { BaseUrl = "https://api.twitter.com/1.1/" };

            var stats = statProc.ProcessResults(string.Empty);

            Assert.Equal(0, stats.Count);
        }

        [Fact]
        public void ProcessResults_Handles_An_Embedded_Status()
        {
            var statProc = new StatusRequestProcessor<Status> { Type = StatusType.Oembed, BaseUrl = "https://api.twitter.com/1.1/" };
            const string ExpectedType = "rich";
            const string ExpectedCacheAge = "31536000000";
            const string ExpectedVersion = "1.0";
            const string ExpectedProviderName = "Twitter";
            const string ExpectedUrl = "https://twitter.com/JoeMayo/status/305050067973312514";
            const int ExpectedWidth = 550;
            const int ExpectedHeight = 0;
            const string ExpectedHtml = "some html";
            const string ExpectedProviderUrl = "http://twitter.com";
            const string ExpectedAuthorUrl = "https://twitter.com/JoeMayo";
            const string ExpectedAuthorName = "Joe Mayo";

            var statuses = statProc.ProcessResults(OEmbedResponse);

            Assert.NotNull(statuses);
            Assert.Single(statuses);
            var status = statuses.Single();
            Assert.NotNull(status);
            var embeddedStatus = status.EmbeddedStatus;
            Assert.NotNull(embeddedStatus);
            Assert.Equal(ExpectedType, embeddedStatus.Type);
            Assert.Equal(ExpectedCacheAge, embeddedStatus.CacheAge);
            Assert.Equal(ExpectedVersion, embeddedStatus.Version);
            Assert.Equal(ExpectedProviderName, embeddedStatus.ProviderName);
            Assert.Equal(ExpectedUrl, embeddedStatus.Url);
            Assert.Equal(ExpectedWidth, embeddedStatus.Width);
            Assert.Equal(ExpectedHeight, embeddedStatus.Height);
            Assert.Equal(ExpectedHtml, embeddedStatus.Html);
            Assert.Equal(ExpectedProviderUrl, embeddedStatus.ProviderUrl);
            Assert.Equal(ExpectedAuthorUrl, embeddedStatus.AuthorUrl);
            Assert.Equal(ExpectedAuthorName, embeddedStatus.AuthorName);
        }

        [Fact]
        public void ProcessResults_Populates_Input_Parameters()
        {
            var statProc = new StatusRequestProcessor<Status>() 
            { 
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = StatusType.Show,
                ID = "123",
                UserID = "123",
                ScreenName = "abc",
                SinceID = 1,
                MaxID = 2,
                Count = 3,
                Page = 4,
                IncludeRetweets = true,
                ExcludeReplies = true,
                IncludeEntities = true,
                IncludeUserEntities = true,
                TrimUser = true,
                IncludeContributorDetails = true,
                IncludeMyRetweet = true,
            };

            var statuses = statProc.ProcessResults(SingleStatusResponse);

            Assert.NotNull(statuses);
            Assert.Single(statuses);
            var status = statuses.Single();
            Assert.NotNull(status);
            Assert.Equal(StatusType.Show, status.Type);
            Assert.Equal("123", status.ID);
            Assert.Equal("123", status.UserID);
            Assert.Equal("abc", status.ScreenName);
            Assert.Equal(1ul, status.SinceID);
            Assert.Equal(2ul, status.MaxID);
            Assert.Equal(3, status.Count);
            Assert.Equal(4, status.Page);
            Assert.True(status.IncludeRetweets);
            Assert.True(status.ExcludeReplies);
            Assert.True(status.IncludeEntities);
            Assert.True(status.IncludeUserEntities);
            Assert.True(status.TrimUser);
            Assert.True(status.IncludeContributorDetails);
            Assert.True(status.IncludeMyRetweet);
        }

        [Fact]
        public void ProcessResults_Populates_EmbeddedStatus_Parameters()
        {
            var statProc = new StatusRequestProcessor<Status>()
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = StatusType.Oembed,
                ID = "123",
                OEmbedUrl = "http://myurl.com",
                OEmbedMaxWidth = 300,
                OEmbedHideMedia = true,
                OEmbedHideThread = true,
                OEmbedOmitScript = true,
                OEmbedAlign = EmbeddedStatusAlignment.Left,
                OEmbedRelated = "JoeMayo,TwitterAPI",
                OEmbedLanguage = "en"
            };

            var statuses = statProc.ProcessResults(OEmbedResponse);

            Assert.NotNull(statuses);
            Assert.Single(statuses);
            var status = statuses.Single();
            Assert.NotNull(status);
            Assert.Equal(StatusType.Oembed, status.Type);
            Assert.Equal("123", status.ID);
            Assert.Equal("http://myurl.com", status.OEmbedUrl);
            Assert.Equal(300, status.OEmbedMaxWidth);
            Assert.True(status.OEmbedHideMedia);
            Assert.True(status.OEmbedHideThread);
            Assert.True(status.OEmbedOmitScript);
            Assert.Equal(EmbeddedStatusAlignment.Left, status.OEmbedAlign);
            Assert.Equal("JoeMayo,TwitterAPI", status.OEmbedRelated);
            Assert.Equal("en", status.OEmbedLanguage);
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
      ""coordinates"":{
          ""type"":""Point"",
          ""coordinates"":[
              -122.40060,
              37.78215
          ]
      },
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
      ""coordinates"":{
          ""type"":""Point"",
          ""coordinates"":[
              -122.40060,
              37.78215
          ]
      },
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

        const string MultipleUsersResponse = @"[
   {
      ""show_all_inline_media"":false,
      ""id"":106069564,
      ""default_profile"":true,
      ""profile_background_color"":""C0DEED"",
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1440573976\/Ginny2011_normal.jpg"",
      ""following"":false,
      ""statuses_count"":19611,
      ""followers_count"":2793,
      ""utc_offset"":-18000,
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
      ""screen_name"":""gcaughey"",
      ""name"":""Ginny Caughey"",
      ""profile_link_color"":""0084B4"",
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
      ""listed_count"":234,
      ""url"":null,
      ""protected"":false,
      ""follow_request_sent"":false,
      ""created_at"":""Mon Jan 18 11:55:17 +0000 2010"",
      ""profile_use_background_image"":true,
      ""verified"":false,
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1440573976\/Ginny2011_normal.jpg"",
      ""is_translator"":false,
      ""profile_text_color"":""333333"",
      ""description"":""Ginny is a Windows Phone Development MVP."",
      ""notifications"":false,
      ""time_zone"":""Eastern Time (US & Canada)"",
      ""id_str"":""106069564"",
      ""default_profile_image"":false,
      ""location"":""North Carolina"",
      ""profile_sidebar_border_color"":""C0DEED"",
      ""favourites_count"":340,
      ""contributors_enabled"":false,
      ""lang"":""en"",
      ""geo_enabled"":false,
      ""friends_count"":570,
      ""profile_background_tile"":false,
      ""status"":{
         ""in_reply_to_status_id_str"":""211432937089011712"",
         ""truncated"":false,
         ""in_reply_to_user_id_str"":""16864582"",
         ""coordinates"":null,
         ""geo"":null,
         ""in_reply_to_user_id"":16864582,
         ""retweeted"":false,
         ""in_reply_to_screen_name"":""DDReaper"",
         ""contributors"":null,
         ""created_at"":""Sat Jun 09 13:22:33 +0000 2012"",
         ""retweet_count"":0,
         ""source"":""\u003Ca href=\""http:\/\/www.metrotwit.com\/\"" rel=\""nofollow\""\u003EMetroTwit\u003C\/a\u003E"",
         ""id_str"":""211448196407377920"",
         ""place"":null,
         ""in_reply_to_status_id"":211432937089011712,
         ""id"":211448196407377920,
         ""favorited"":false,
         ""text"":""@DDReaper Oh yes definitely. But for me that ends up taking a couple of days usually.""
      },
      ""profile_sidebar_fill_color"":""DDEEF6""
   },
   {
      ""show_all_inline_media"":false,
      ""id"":34649740,
      ""default_profile"":false,
      ""profile_background_color"":""131516"",
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2102800167\/223368_23679679112_773624112_392961_1835_n_normal.jpg"",
      ""following"":false,
      ""statuses_count"":257,
      ""followers_count"":38,
      ""utc_offset"":null,
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme14\/bg.gif"",
      ""screen_name"":""AdamBenoit"",
      ""name"":""Adam Benoit"",
      ""profile_link_color"":""009999"",
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme14\/bg.gif"",
      ""listed_count"":0,
      ""url"":""http:\/\/AdamBenoit.com"",
      ""protected"":false,
      ""follow_request_sent"":false,
      ""created_at"":""Thu Apr 23 16:00:56 +0000 2009"",
      ""profile_use_background_image"":true,
      ""verified"":false,
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2102800167\/223368_23679679112_773624112_392961_1835_n_normal.jpg"",
      ""is_translator"":false,
      ""profile_text_color"":""333333"",
      ""description"":""Father, Bass Player, Paintball enthusiast, Windows Phone developer and Quality Assurance Analyst at Web.com eCommerce."",
      ""notifications"":false,
      ""time_zone"":null,
      ""id_str"":""34649740"",
      ""default_profile_image"":false,
      ""location"":""Barrie, Ontario, Canada"",
      ""profile_sidebar_border_color"":""eeeeee"",
      ""favourites_count"":6,
      ""contributors_enabled"":false,
      ""lang"":""en"",
      ""geo_enabled"":false,
      ""friends_count"":102,
      ""profile_background_tile"":true,
      ""profile_sidebar_fill_color"":""efefef""
   },
   {
      ""show_all_inline_media"":true,
      ""id"":6411122,
      ""default_profile"":false,
      ""profile_background_color"":""9ae4e8"",
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1142064000\/dc2334a6-814c-4113-856f-ce2b37c2cc7a_normal.png"",
      ""following"":false,
      ""statuses_count"":5181,
      ""followers_count"":450,
      ""utc_offset"":36000,
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
      ""screen_name"":""indyfromoz"",
      ""name"":""Indrajit Chakrabarty"",
      ""profile_link_color"":""0000ff"",
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
      ""listed_count"":31,
      ""url"":""http:\/\/indyfromoz.wordpress.com\/"",
      ""protected"":false,
      ""follow_request_sent"":false,
      ""created_at"":""Tue May 29 10:39:17 +0000 2007"",
      ""profile_use_background_image"":true,
      ""verified"":false,
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1142064000\/dc2334a6-814c-4113-856f-ce2b37c2cc7a_normal.png"",
      ""is_translator"":false,
      ""profile_text_color"":""000000"",
      ""description"":""C# Web\/WPF\/Silverlight & WinPhone\/Android\/iOS App developer, HAM - VK2IJC, entrepreneur, dad. Runs @lianasolutions, a micro-ISV in Sydney, Australia "",
      ""notifications"":false,
      ""time_zone"":""Sydney"",
      ""id_str"":""6411122"",
      ""default_profile_image"":false,
      ""location"":""Sydney, Australia"",
      ""profile_sidebar_border_color"":""87bc44"",
      ""favourites_count"":6259,
      ""contributors_enabled"":false,
      ""lang"":""en"",
      ""geo_enabled"":true,
      ""friends_count"":272,
      ""profile_background_tile"":false,
      ""profile_sidebar_fill_color"":""e0ff92""
   },
   {
      ""show_all_inline_media"":false,
      ""id"":152318142,
      ""default_profile"":false,
      ""profile_background_color"":""8B542B"",
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1346401100\/solaIyanu_normal.jpg"",
      ""following"":false,
      ""statuses_count"":5522,
      ""followers_count"":434,
      ""utc_offset"":0,
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme8\/bg.gif"",
      ""screen_name"":""solaadio"",
      ""name"":""Olusola Adio"",
      ""profile_link_color"":""7d7069"",
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme8\/bg.gif"",
      ""listed_count"":8,
      ""url"":""http:\/\/www.logion.co.uk"",
      ""protected"":false,
      ""follow_request_sent"":false,
      ""created_at"":""Sat Jun 05 16:31:20 +0000 2010"",
      ""profile_use_background_image"":true,
      ""verified"":false,
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1346401100\/solaIyanu_normal.jpg"",
      ""is_translator"":false,
      ""profile_text_color"":""333333"",
      ""description"":""Jesus lover, Husband, Father, Developer with  (MCTS in #WPF4 & #Silverlight4). Director in @iLogion. Jesus is the ONLY way to heaven. #Gooner"",
      ""notifications"":false,
      ""time_zone"":""London"",
      ""id_str"":""152318142"",
      ""default_profile_image"":false,
      ""location"":""UK"",
      ""profile_sidebar_border_color"":""D9B17E"",
      ""favourites_count"":2024,
      ""contributors_enabled"":false,
      ""lang"":""en"",
      ""geo_enabled"":true,
      ""friends_count"":774,
      ""profile_background_tile"":false,
      ""status"":{
         ""in_reply_to_status_id_str"":null,
         ""truncated"":false,
         ""in_reply_to_user_id_str"":null,
         ""coordinates"":null,
         ""geo"":null,
         ""in_reply_to_user_id"":null,
         ""retweeted"":false,
         ""in_reply_to_screen_name"":null,
         ""contributors"":null,
         ""retweeted_status"":{
            ""in_reply_to_status_id_str"":null,
            ""truncated"":false,
            ""in_reply_to_user_id_str"":null,
            ""coordinates"":null,
            ""geo"":null,
            ""in_reply_to_user_id"":null,
            ""retweeted"":false,
            ""in_reply_to_screen_name"":null,
            ""contributors"":null,
            ""created_at"":""Sat Jun 09 06:47:02 +0000 2012"",
            ""retweet_count"":785,
            ""source"":""\u003Ca href=\""http:\/\/bufferapp.com\"" rel=\""nofollow\""\u003EBuffer\u003C\/a\u003E"",
            ""id_str"":""211348662993960960"",
            ""place"":null,
            ""in_reply_to_status_id"":null,
            ""id"":211348662993960960,
            ""favorited"":false,
            ""text"":""In 2010, Verizon, GE and Boeing all earned over $1 billion, but paid absolutely no federal taxes.""
         },
         ""created_at"":""Sat Jun 09 07:04:49 +0000 2012"",
         ""retweet_count"":785,
         ""source"":""\u003Ca href=\""http:\/\/twitter.com\/#!\/download\/iphone\"" rel=\""nofollow\""\u003ETwitter for iPhone\u003C\/a\u003E"",
         ""id_str"":""211353138324701184"",
         ""place"":null,
         ""in_reply_to_status_id"":null,
         ""id"":211353138324701184,
         ""favorited"":false,
         ""text"":""RT @UberFacts: In 2010, Verizon, GE and Boeing all earned over $1 billion, but paid absolutely no federal taxes.""
      },
      ""profile_sidebar_fill_color"":""EADEAA""
   },
   {
      ""show_all_inline_media"":false,
      ""id"":72197816,
      ""default_profile"":false,
      ""profile_background_color"":""C0DEED"",
      ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/456137779\/6140_98319542965_628617965_2172691_4400036_n_normal.jpg"",
      ""following"":false,
      ""statuses_count"":3234,
      ""followers_count"":272,
      ""utc_offset"":-18000,
      ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/43195547\/white.jpg"",
      ""screen_name"":""gecheverry"",
      ""name"":""Gustavo Echeverry"",
      ""profile_link_color"":""0084B4"",
      ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/43195547\/white.jpg"",
      ""listed_count"":4,
      ""url"":null,
      ""protected"":false,
      ""follow_request_sent"":false,
      ""created_at"":""Mon Sep 07 03:47:54 +0000 2009"",
      ""profile_use_background_image"":true,
      ""verified"":false,
      ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/456137779\/6140_98319542965_628617965_2172691_4400036_n_normal.jpg"",
      ""is_translator"":false,
      ""profile_text_color"":""333333"",
      ""description"":""technology enthusiast, developer -c#, java, iPhone-, poker player, fun-addict"",
      ""notifications"":false,
      ""time_zone"":""Eastern Time (US & Canada)"",
      ""id_str"":""72197816"",
      ""default_profile_image"":false,
      ""location"":""Colombia"",
      ""profile_sidebar_border_color"":""C0DEED"",
      ""favourites_count"":1,
      ""contributors_enabled"":false,
      ""lang"":""en"",
      ""geo_enabled"":false,
      ""friends_count"":536,
      ""profile_background_tile"":false,
      ""profile_sidebar_fill_color"":""DDEEF6""
   }
]";

        const string OEmbedResponse = @"{
   ""type"":""rich"",
   ""cache_age"":""31536000000"",
   ""version"":""1.0"",
   ""provider_name"":""Twitter"",
   ""url"":""https://twitter.com/JoeMayo/status/305050067973312514"",
   ""width"":550,
   ""height"":null,
   ""html"":""some html"",
   ""provider_url"":""http://twitter.com"",
   ""author_url"":""https://twitter.com/JoeMayo"",
   ""author_name"":""Joe Mayo""
}";
    }
}
