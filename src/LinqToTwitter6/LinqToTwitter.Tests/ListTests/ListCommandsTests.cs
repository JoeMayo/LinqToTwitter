using LinqToTwitter.OAuth;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.ListTests
{
    [TestClass]
    public class ListCommandsTests
    {
        TwitterContext ctx;
        Mock<ITwitterExecute> execMock;

        public ListCommandsTests()
        {
            TestCulture.SetCulture();
        }

        void InitializeTwitterContext(string response)
        {
            var authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(response);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<List>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<CreateOrUpdateListRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<DeleteListRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            ctx = new TwitterContext(execMock.Object);
        }

        [TestMethod]
        public void ListRequestProcessor_Handles_Actions()
        {
            var listReqProc = new ListRequestProcessor<List>();

            Assert.IsInstanceOfType(listReqProc, typeof(IRequestProcessorWithAction<List>));
        }

        [TestMethod]
        public async Task CreateListAsync_MissingNameParam_Throws()
        {
            InitializeTwitterContext(CreateListResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.CreateListAsync(null, "desc", isPrivate: true));

            Assert.AreEqual("name", ex.ParamName);
        }

        [TestMethod]
        public async Task CreateListAsync_WithParameters_BuildsUrl()
        {
            InitializeTwitterContext(CreateListResponse);
            var parameters = new Dictionary<string, string>
            {
                { "name", "test" },
                { "description", "desc" },
                { "private", true.ToString() }
            };

            await ctx.CreateListAsync("test", "desc", isPrivate: true);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/2/lists",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<CreateOrUpdateListRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task CreateListAsync_WithGoodParams_PopulatesResponse()
        {
            InitializeTwitterContext(CreateListResponse);

            ListResponse response = await ctx.CreateListAsync("test", "desc", isPrivate: true);

            Assert.IsNotNull(response);
            ListResponseData data = response.Data;
            Assert.AreEqual("1441162269824405510", data.ID);
            Assert.AreEqual("test v2 create list", data.Name);
        }

        [TestMethod]
        public async Task UpdateListAsync_MissingIDParam_Throws()
        {
            InitializeTwitterContext(UpdateListResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateListAsync(null, "test", "desc", isPrivate: true));

            Assert.AreEqual("id", ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateListAsync_WithParams_BuildsUrl()
        {
            InitializeTwitterContext(UpdateListResponse);

            await ctx.UpdateListAsync("123", "test", "desc", isPrivate: true);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Put.ToString(),
                    "https://api.twitter.com/2/lists/123",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<CreateOrUpdateListRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task UpdateListAsync_WithGoodParams_PopulatesResponse()
        {
            InitializeTwitterContext(UpdateListResponse);

            ListResponse response = await ctx.UpdateListAsync("123", "test", "desc", isPrivate: true);

            Assert.IsNotNull(response);
            ListResponseData data = response.Data;
            Assert.IsTrue(data.Updated);
        }

        [TestMethod]
        public async Task DeleteListAsync_WithMissingID_Throws()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteListAsync(null));

            Assert.AreEqual("id", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteListAsync_WithGoodID_BuildsUrl()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var parameters = new Dictionary<string, string>
            {
                { "id", "123" }
            };

            await ctx.DeleteListAsync("123");

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    "https://api.twitter.com/2/lists/123",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<DeleteListRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteListAsync_WithRawResult_Succeeds()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            await ctx.DeleteListAsync("123");

            Assert.AreEqual(TestStatusQueryResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task AddMemberToListAsync_Requires_UserID_Or_ScreenName()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberToListAsync(null, 0, null, 0, null));

            Assert.AreEqual("UserIdOrScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberToListAsync_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberToListAsync("JoeMayo", 0, null, 0, null));

            Assert.AreEqual("ListIdOrSlug", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberToListAsync_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberToListAsync("JoeMayo", 0, "linq", 0, null));

            Assert.AreEqual("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberToListAsync_Invokes_Executor_Execute()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "screen_name", "JoeMayo" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            await ctx.AddMemberToListAsync("JoeMayo", 123, "test", 456, "JoeMayo");

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/lists/members/create.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task AddMemberToListAsync_WithRawResult_Succeeds()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            await ctx.AddMemberToListAsync("JoeMayo", 123, "test", 456, "JoeMayo");

            Assert.AreEqual(TestStatusQueryResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_ScreenNames_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberRangeToListAsync(0, null, 0, null, new List<string> { "SomeName" }));

            Assert.AreEqual("ListIdOrSlug", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_ScreenNames_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberRangeToListAsync(0, "test", 0, null, new List<string> { "SomeOne" }));

            Assert.AreEqual("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_ScreenNames_Requires_ScreenNames()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberRangeToListAsync(0, "test", 123, null, (List<string>)null));

            Assert.AreEqual("screenNames", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_ScreenNames_Requires_ScreenNames_With_Values()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberRangeToListAsync(0, "test", 123, null, new List<string>()));

            Assert.AreEqual("screenNames", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_ScreenNames_Requires_ScreenNames_Count_LessThanOrEqualTo_100()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var screenNames = Enumerable.Range(1, 101).Select(item => item.ToString(CultureInfo.InvariantCulture)).ToList();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberRangeToListAsync(0, "test", 123, null, screenNames));

            Assert.AreEqual("screenNames", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_ScreenNames_Invokes_Executor_Execute()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "screen_name", "JoeMayo,Linq2Tweeter,SomeOneElse" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" },
            };
            var screenNames = new List<string> { "JoeMayo", "Linq2Tweeter", "SomeOneElse" };

            await ctx.AddMemberRangeToListAsync(123, "test", 456, "JoeMayo", screenNames);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/lists/members/create_all.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_WithRawResult_Succeeds()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var screenNames = new List<string> { "JoeMayo", "Linq2Tweeter", "SomeOneElse" };

            await ctx.AddMemberRangeToListAsync(123, "test", 456, "JoeMayo", screenNames);

            Assert.AreEqual(TestStatusQueryResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_UserIDs_Requires_UserIDs()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberRangeToListAsync(0, "test", 123, null, (List<ulong>)null));

            Assert.AreEqual("userIDs", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_UserIDs_Requires_UserIDs_With_Values()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberRangeToListAsync(0, "test", 123, null, new List<ulong>()));

            Assert.AreEqual("userIDs", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_UserIDs_Requires_UserIDs_Count_LessThanOrEqualTo_100()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var userIDs = Enumerable.Range(1, 101).Select(item => (ulong)item).ToList();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberRangeToListAsync(0, "test", 123, null, userIDs));

            Assert.AreEqual("userIDs", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberRangeToListAsync_For_UserIDs_Invokes_Executor_Execute()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "user_id", "123,234,345" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" },
            };
            var userIDs = new List<ulong> { 123ul, 234ul, 345ul };

            await ctx.AddMemberRangeToListAsync(123, "test", 456, "JoeMayo", userIDs);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/lists/members/create_all.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteMemberFromListAsync_Requires_UserID_Or_ScreenName()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteMemberFromListAsync(0, null, 0, null, 0, null));

            Assert.AreEqual("UserIdOrScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteMemberFromListAsync_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteMemberFromListAsync(0, "JoeMayo", 0, null, 0, null));

            Assert.AreEqual("ListIdOrSlug", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteMemberFromListAsync_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteMemberFromListAsync(0, "JoeMayo", 0, "linq", 0, null));

            Assert.AreEqual("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteMemberFromListAsync_Invokes_Executor_Execute()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "user_id", "789" },
                { "screen_name", "JoeMayo" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            await ctx.DeleteMemberFromListAsync(789, "JoeMayo", 123, "test", 456, "JoeMayo");

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/lists/members/destroy.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteMemberFromListAsync_WithRawResult_Succeeds()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            await ctx.DeleteMemberFromListAsync(789, "JoeMayo", 123, "test", 456, "JoeMayo");

            Assert.AreEqual(TestStatusQueryResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task SubscribeToListAsync_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.SubscribeToListAsync(0, null, 0, null));

            Assert.AreEqual("ListIdOrSlug", ex.ParamName);
        }

        [TestMethod]
        public async Task SubscribeToListAsync_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.SubscribeToListAsync(0, "linq", 0, null));

            Assert.AreEqual("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task SubscribeToListAsync_Invokes_Executor_Execute()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            await ctx.SubscribeToListAsync(123, "test", 456, "JoeMayo");

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/lists/subscribers/create.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task SubscribeToListAsync_WithRawResult_Succeeds()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            await ctx.SubscribeToListAsync(123, "test", 456, "JoeMayo");

            Assert.AreEqual(TestStatusQueryResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UnsubscribeFromListAsync_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UnsubscribeFromListAsync(0, null, 0, null));

            Assert.AreEqual("ListIdOrSlug", ex.ParamName);
        }

        [TestMethod]
        public async Task UnsubscribeFromListAsync_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UnsubscribeFromListAsync(0, "linq", 0, null));

            Assert.AreEqual("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task UnsubscribeFromListAsync_Invokes_Executor_Execute()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            await ctx.UnsubscribeFromListAsync(123, "test", 456, "JoeMayo");

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/lists/subscribers/destroy.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task UnsubscribeFromListAsync_WithRawResult_Succeeds()
        {
            InitializeTwitterContext(TestStatusQueryResponse);

            await ctx.UnsubscribeFromListAsync(123, "test", 456, "JoeMayo");

            Assert.AreEqual(TestStatusQueryResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task DestroyAllFromListAsync_Invokes_Executor_Execute()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var userIDs = new List<ulong> { 456 };
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "user_id", "456" },
                { "owner_id", "789" },
                { "owner_screen_name", "JoeMayo" }
            };

            await ctx.DeleteMemberRangeFromListAsync(123, "test", userIDs, 789, "JoeMayo");

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/lists/members/destroy_all.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DestroyAllFromListAsync_WithRawResult_Succeeds()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var userIDs = new List<ulong> { 456 };

            await ctx.DeleteMemberRangeFromListAsync(123, "test", userIDs, 789, "JoeMayo");

            Assert.AreEqual(TestStatusQueryResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task DestroyAllFromListAsync_Requires_Either_ListID_Or_Slug()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var userIDs = new List<ulong> { 1, 2, 3 };

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteMemberRangeFromListAsync(0, null, userIDs, 0, null));

            Assert.AreEqual("ListIdOrSlug", ex.ParamName);
        }

        [TestMethod]
        public async Task DestroyAllFromListAsync_Requires_OwnerID_Or_OwnerScreenName_If_Using_Slug()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var userIDs = new List<ulong> { 1, 2, 3 };

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteMemberRangeFromListAsync(0, "slug", userIDs, 0, null));

            Assert.AreEqual("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteMemberRangeFromListAsync_Accepts_Missing_OwnerID_And_OwnerScreenName_If_Using_ListID()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var userIDs = new List<ulong> { 1, 2, 3 };

            await ctx.DeleteMemberRangeFromListAsync(1, "slug", userIDs, 0, null);
        }

        [TestMethod]
        public async Task DeleteMemberRangeFromListAsync_WithRawResult_Succeeds()
        {
            InitializeTwitterContext(TestStatusQueryResponse);
            var userIDs = new List<ulong> { 1, 2, 3 };

            await ctx.DeleteMemberRangeFromListAsync(1, "slug", userIDs, 0, null);

            Assert.AreEqual(TestStatusQueryResponse, ctx.RawResult);
        }

        const string CreateListResponse = @"{
  ""data"": {
    ""id"": ""1441162269824405510"",
    ""name"": ""test v2 create list""
  }
}";

        const string UpdateListResponse = @"{
  ""data"": {
    ""updated"": true
  }
}";

        const string DeleteListResponse = @"{
  ""data"": {
    ""deleted"": true
  }
}";

        const string TestStatusQueryResponse = @"{
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":6194482,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1618873297\/iPhone_Pictures_524_normal.jpg"",
         ""url"":""http:\/\/techpreacher.corti.com"",
         ""created_at"":""Mon May 21 08:57:50 +0000 2007"",
         ""followers_count"":815,
         ""default_profile"":false,
         ""profile_background_color"":""a6cce6"",
         ""lang"":""en"",
         ""utc_offset"":3600,
         ""name"":""Sascha Corti"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/242394801\/TwitterBackground2.png"",
         ""location"":""47.580262,-122.135105"",
         ""profile_link_color"":""0084B4"",
         ""listed_count"":47,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":false,
         ""description"":""Developer evangelist for Microsoft in Switzerland. Focus on web 2.0 technologies, Windows Phone 7 development. Passionate gamer with a life."",
         ""profile_text_color"":""333333"",
         ""statuses_count"":4293,
         ""screen_name"":""TechPreacher"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1618873297\/iPhone_Pictures_524_normal.jpg"",
         ""time_zone"":""Bern"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/242394801\/TwitterBackground2.png"",
         ""friends_count"":517,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""C0DEED"",
         ""id_str"":""6194482"",
         ""geo_enabled"":true,
         ""favourites_count"":37,
         ""profile_background_tile"":false,
         ""notifications"":false,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":0,
      ""id_str"":""196286470443642880"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""annotations"":null,
      ""source"":""\u003Ca href=\""http:\/\/raptr.com\/\"" rel=\""nofollow\""\u003ERaptr\u003C\/a\u003E"",
      ""created_at"":""Sat Apr 28 17:15:16 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":196286470443642880,
      ""geo"":null,
      ""text"":""I unlocked the Get a cube achievement on Fez! http:\/\/t.co\/Hqhl5oix""
   }";
    }
}
