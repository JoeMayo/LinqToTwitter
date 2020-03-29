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
                status.Cursor == "789" &&
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
                   new KeyValuePair<string, string>("Cursor", "789")));
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
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/mentions_timeline.json?since_id=123&max_id=145&count=50";
            var statProc = new StatusRequestProcessor<Status> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.Mentions).ToString() },
                { "SinceID", "123" },
                { "MaxID", "145" },
                { "Count", "50" }
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
        public void BuildUrl_Constructs_Retweeters_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/statuses/retweeters/ids.json?id=5&cursor=7";
            var reqProc = new StatusRequestProcessor<Status>
            {
                Type = StatusType.User,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.Retweeters).ToString() },
                { "ID", "5" },
                { "Cursor", "7" }
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
                Type = StatusType.Retweeters,
                BaseUrl = "https://api.twitter.com/1.1/"
            };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)StatusType.Retweeters).ToString() },
                //{ "ID", "123" },
                { "Cursor", "25" }
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
            var statProc = new StatusRequestProcessor<Status> { Type = StatusType.Retweeters, BaseUrl = "https://api.twitter.com/1.1/" };

            var statuses = statProc.ProcessResults(MultipleUsersResponse);

            Assert.NotNull(statuses);
            Assert.Single(statuses);
            var status = statuses.Single();
            Assert.NotNull(status);
            var users = status.Users;
            Assert.NotNull(users);
            Assert.Equal(5, users.Count);
            ulong user = users.First();
            Assert.Equal(34649740ul, user);
            var cursor = status.CursorMovement;
            Assert.NotNull(cursor);
            Assert.Equal("123", cursor.Next);
            Assert.Equal("456", cursor.Previous);
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
            const ulong ExpectedCacheAge = 3153600000;
            const string ExpectedVersion = "1.0";
            const string ExpectedProviderName = "Twitter";
            const string ExpectedUrl = "https://twitter.com/JoeMayo/statuses/305050067973312514";
            const int ExpectedWidth = 550;
            const int ExpectedHeight = 0;
            const string ExpectedHtml = "some html";
            const string ExpectedProviderUrl = "https://twitter.com";
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
                Cursor="123",
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
            Assert.Equal("123", status.Cursor);
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

        const string MultipleUsersResponse = @"{
   ""ids"":[
      34649740,
      6411122,
      106069564,
      152318142,
      72197816
   ],
   ""next_cursor"":123,
   ""next_cursor_str"":""123"",
   ""previous_cursor"":456,
   ""previous_cursor_str"":""456""
}";

        const string OEmbedResponse = @"{
   ""cache_age"":""3153600000"",
   ""url"":""https://twitter.com/JoeMayo/statuses/305050067973312514"",
   ""height"":null,
   ""provider_url"":""https://twitter.com"",
   ""provider_name"":""Twitter"",
   ""author_name"":""Joe Mayo"",
   ""version"":""1.0"",
   ""author_url"":""https://twitter.com/JoeMayo"",
   ""type"":""rich"",
   ""html"":""some html"",
   ""width"":550
}";
    }
}
