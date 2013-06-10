using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class TwitterContextTests
    {
        ITwitterAuthorizer auth;

        public TwitterContextTests()
        {
            TestCulture.SetCulture();
            auth = new Mock<ITwitterAuthorizer>().Object;
        }

        void InitializeTwitterContextForExecuteTest(out TwitterContext ctx, out Expression expression)
        {
            var exec = new Mock<ITwitterExecute>();
            exec.Setup(exc => exc.QueryTwitter(It.IsAny<Request>(), It.IsAny<IRequestProcessor<Status>>()))
                .Returns(SingleStatusResponse);

            ctx = new TwitterContext(exec.Object);
            var publicQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Show
                select tweet;

            expression = publicQuery.Expression;
        }

        [Fact]
        public void TwitterContext_Single_Param_Constructor_Sets_Defaults()
        {
            const string BaseUrl = "https://api.twitter.com/1.1/";
            const string SearchUrl = "https://api.twitter.com/1.1/search/";
            ITwitterAuthorizer authorizedClient = new PinAuthorizer();
            var ctx = new TwitterContext(authorizedClient);

            Assert.Same(authorizedClient, ctx.AuthorizedClient);
            Assert.Equal(BaseUrl, ctx.BaseUrl);
            Assert.Equal(SearchUrl, ctx.SearchUrl);
        }

        [Fact]
        public void TwitterContext_1_Param_Requres_NonNull_Authorization()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new TwitterContext((PinAuthorizer)null));

            Assert.Equal("authorizedClient", ex.ParamName);
        }

        [Fact]
        public void TwitterContext_Requres_NonNull_Executor()
        {
            var authMock = new Mock<ITwitterAuthorizer>();

            var ex = Assert.Throws<ArgumentNullException>(() => new TwitterContext((ITwitterExecute)null));

            Assert.Equal("execute", ex.ParamName);
        }

        [Fact]
        public void CreateRequestProcessor_Returns_ProperRequestProcessor()
        {
            var ctx = new TwitterContext(auth);

            var showQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Show
                select tweet;

            var statusProc = ctx.CreateRequestProcessor<Status>(showQuery.Expression);
            Assert.IsType(typeof(StatusRequestProcessor<Status>), statusProc);
        }

        [Fact]
        public void CreateStatusRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.Status select tweet;

            IRequestProcessor<Status> actual = ctx.CreateRequestProcessor<Status>(queryResult.Expression);
            Assert.IsType(typeof(StatusRequestProcessor<Status>), actual);
        }

        [Fact]
        public void CreateAccountRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.Account select tweet;

            IRequestProcessor<Account> actual = ctx.CreateRequestProcessor<Account>(queryResult.Expression);
            Assert.IsType(typeof(AccountRequestProcessor<Account>), actual);
        }

        [Fact]
        public void CreateBlocksRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.Blocks select tweet;

            IRequestProcessor<Blocks> actual = ctx.CreateRequestProcessor<Blocks>(queryResult.Expression);
            Assert.IsType(typeof(BlocksRequestProcessor<Blocks>), actual);
        }

        [Fact]
        public void CreateDirectMessageRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.DirectMessage select tweet;

            IRequestProcessor<DirectMessage> actual = ctx.CreateRequestProcessor<DirectMessage>(queryResult.Expression);
            Assert.IsType(typeof(DirectMessageRequestProcessor<DirectMessage>), actual);
        }

        [Fact]
        public void CreateFavoritesRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.Favorites select tweet;

            IRequestProcessor<Favorites> actual = ctx.CreateRequestProcessor<Favorites>(queryResult.Expression);
            Assert.IsType(typeof(FavoritesRequestProcessor<Favorites>), actual);
        }

        [Fact]
        public void CreateFriendshipRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.Friendship select tweet;

            IRequestProcessor<Friendship> actual = ctx.CreateRequestProcessor<Friendship>(queryResult.Expression);
            Assert.IsType(typeof(FriendshipRequestProcessor<Friendship>), actual);
        }

        [Fact]
        public void CreateSearchRequestProcessor_Returns_RawRequestProcessor()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from raw in ctx.RawQuery select raw;

            IRequestProcessor<Raw> actual = ctx.CreateRequestProcessor<Raw>(queryResult.Expression);
            Assert.IsType(typeof(RawRequestProcessor<Raw>), actual);
        }

        [Fact]
        public void CreateSearchRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.Search select tweet;

            IRequestProcessor<Search> actual = ctx.CreateRequestProcessor<Search>(queryResult.Expression);
            Assert.IsType(typeof(SearchRequestProcessor<Search>), actual);
        }

        [Fact]
        public void CreateSocialGraphRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.SocialGraph select tweet;

            IRequestProcessor<SocialGraph> actual = ctx.CreateRequestProcessor<SocialGraph>(queryResult.Expression);
            Assert.IsType(typeof(SocialGraphRequestProcessor<SocialGraph>), actual);
        }

        [Fact]
        public void CreateTrendRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.Trends select tweet;

            IRequestProcessor<Trend> actual = ctx.CreateRequestProcessor<Trend>(queryResult.Expression);
            Assert.IsType(typeof(TrendRequestProcessor<Trend>), actual);
        }

        [Fact]
        public void CreateUserRequestProcessorTest()
        {
            var ctx = new TwitterContext(auth);

            var queryResult = from tweet in ctx.User select tweet;

            IRequestProcessor<User> actual = ctx.CreateRequestProcessor<User>(queryResult.Expression);
            Assert.IsType(typeof(UserRequestProcessor<User>), actual);
        }

        [Fact]
        public void CreateRequestProcessorNullExpressionTest1()
        {
            var ctx = new TwitterContext(auth);

            var ex = Assert.Throws<ArgumentNullException>(() => ctx.CreateRequestProcessor<Status>((Expression)null));

            Assert.Equal("Expression", ex.ParamName);
        }

        [Fact]
        public void CreateRequestProcessor_Returns_RelatedResultsRequestProcessor()
        {
            var ctx = new TwitterContext(auth) {BaseUrl = "https://api.twitter.com/1.1/"};
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var resultsQuery =
                from tweet in ctx.RelatedResults
                where tweet.Type == RelatedResultsType.Show
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<RelatedResults>(resultsQuery.Expression);

            Assert.IsType(typeof(RelatedResultsRequestProcessor<RelatedResults>), reqProc);
            Assert.Equal("https://api.twitter.com/1.1/", reqProc.BaseUrl);
        }

        [Fact]
        public void Execute_Returns_List_Of_Status()
        {
            TwitterContext ctx;
            Expression expression;
            InitializeTwitterContextForExecuteTest(out ctx, out expression);
            ctx.Log = new DebuggerWriter();

            var actual = ctx.Execute<Status>(expression, true);

            var tweets = actual as IEnumerable<Status>;
            Assert.NotNull(tweets);
            Assert.True(tweets.Any());
        }

        [Fact]
        public void Execute_Logs_Results()
        {
            TwitterContext ctx;
            Expression expression;
            InitializeTwitterContextForExecuteTest(out ctx, out expression);

            var actual = ctx.Execute<Status>(expression, true);

            var tweets = actual as IEnumerable<Status>;
            Assert.NotNull(tweets);
            Assert.True(tweets.Any());
        }
  
        [Fact]
        public void Execute_Sets_RawResults_Property()
        {
            TwitterContext ctx;
            Expression expression;
            InitializeTwitterContextForExecuteTest(out ctx, out expression);

            ctx.Execute<Status>(expression, true);

            Assert.Equal(SingleStatusResponse, ctx.RawResult);
        }

        [Fact]
        public void CreateRequestProcessor_Returns_StreamingRequestProcessor()
        {
            var ctx = new TwitterContext(auth) {StreamingUrl = "https://stream.twitter.com/1/"};
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var streamingQuery =
                from tweet in ctx.Streaming
                where tweet.Type == StreamingType.Sample
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<Streaming>(streamingQuery.Expression);

            Assert.IsType(typeof(StreamingRequestProcessor<Streaming>), reqProc);
            Assert.Equal("https://stream.twitter.com/1/", reqProc.BaseUrl);
            var streamingRequestProcessor = reqProc as StreamingRequestProcessor<Streaming>;
            if (streamingRequestProcessor != null)
                Assert.Equal(execMock.Object, streamingRequestProcessor.TwitterExecutor);
        }

        [Fact]
        public void Execute_Calls_QueryTwitterStream_For_Streaming_Queries()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(execMock.Object);
            var streamingQuery =
                from tweet in ctx.Streaming
                where tweet.Type == StreamingType.Sample
                select tweet;

            ctx.Execute<Streaming>(streamingQuery.Expression, isEnumerable: true);

            execMock.Verify(exec => exec.QueryTwitterStream(It.IsAny<Request>()), Times.Once());
        }

        [Fact]
        public void Execute_Calls_QueryTwitter_InsteadOf_QueryTwitterStream_For_NonStreaming_Queries()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.QueryTwitter(It.IsAny<Request>(), It.IsAny<StatusRequestProcessor<Status>>())).Returns(SingleStatusResponse);
            var ctx = new TwitterContext(execMock.Object);
            var statusQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Show
                select tweet;

            ctx.Execute<Status>(statusQuery.Expression, isEnumerable: true);

            execMock.Verify(exec => exec.QueryTwitterStream(It.IsAny<Request>()), Times.Never());
            execMock.Verify(exec => exec.QueryTwitter(It.IsAny<Request>(), It.IsAny<StatusRequestProcessor<Status>>()), Times.Once());
        }

        [Fact]
        public void CreateRequestProcessor_Returns_UserStreamRequestProcessor()
        {
            var ctx = new TwitterContext(auth) {StreamingUrl = "https://userstream.twitter.com/1.1/"};
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var streamingQuery =
                from tweet in ctx.UserStream
                where tweet.Type == UserStreamType.User
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<UserStream>(streamingQuery.Expression);

            Assert.IsType(typeof(UserStreamRequestProcessor<UserStream>), reqProc);
            var userStreamRequestProcessor = reqProc as UserStreamRequestProcessor<UserStream>;
            if (userStreamRequestProcessor != null)
                Assert.Equal("https://userstream.twitter.com/1.1/", userStreamRequestProcessor.UserStreamUrl);
            var streamRequestProcessor = reqProc as UserStreamRequestProcessor<UserStream>;
            if (streamRequestProcessor != null)
                Assert.Equal(execMock.Object, streamRequestProcessor.TwitterExecutor);
        }

        [Fact]
        public void Execute_Calls_QueryTwitterStream_For_UserStream_Queries()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(execMock.Object);
            var streamingQuery =
                from tweet in ctx.UserStream
                where tweet.Type == UserStreamType.User
                select tweet;

            ctx.Execute<UserStream>(streamingQuery.Expression, isEnumerable: true);

            execMock.Verify(exec => exec.QueryTwitterStream(It.IsAny<Request>()), Times.Once());
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
    }
}
