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
                    It.IsAny<ListCreateOrUpdateRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ListDeleteRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ListMemberRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ListFollowOrPinRequest>(),
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
                    It.IsAny<ListCreateOrUpdateRequest>(),
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
                    It.IsAny<ListCreateOrUpdateRequest>(),
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
            InitializeTwitterContext(DeleteListResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteListAsync(null));

            Assert.AreEqual("id", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteListAsync_WithGoodID_BuildsUrl()
        {
            InitializeTwitterContext(DeleteListResponse);
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
                    It.IsAny<ListDeleteRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteListAsync_WithGoodParams_PopulatesResponse()
        {
            InitializeTwitterContext(DeleteListResponse);

            ListResponse response = await ctx.DeleteListAsync("123");

            Assert.IsNotNull(response);
            ListResponseData data = response.Data;
            Assert.IsTrue(data.Deleted);
        }

        [TestMethod]
        public async Task AddMemberToListAsync_WithoutListID_Throws()
        {
            InitializeTwitterContext(AddListMemberResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberToListAsync(null, "def"));

            Assert.AreEqual("listID", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberToListAsync_WithoutUserID_Throws()
        {
            InitializeTwitterContext(AddListMemberResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddMemberToListAsync("abc", null));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task AddMemberToListAsync_WithGoodIDs_BuildsUrl()
        {
            InitializeTwitterContext(AddListMemberResponse);

            await ctx.AddMemberToListAsync("abc", "def");

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/2/lists/abc/members",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ListMemberRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task AddMemberToListAsync_WithGoodParams_PopulatesResponse()
        {
            InitializeTwitterContext(AddListMemberResponse);

            ListResponse response = await ctx.AddMemberToListAsync("abc", "def");

            Assert.IsNotNull(response);
            ListResponseData data = response.Data;
            Assert.IsTrue(data.IsMember);
        }


        [TestMethod]
        public async Task DeleteMemberFromListAsync_WithoutListID_Throws()
        {
            InitializeTwitterContext(DeleteListMemberResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteMemberFromListAsync(null, "def"));

            Assert.AreEqual("listID", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteMemberFromListAsync_WithoutUserID_Throws()
        {
            InitializeTwitterContext(DeleteListMemberResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteMemberFromListAsync("abc", null));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteMemberFromListAsync_WithGoodIDs_BuildsUrl()
        {
            InitializeTwitterContext(DeleteListMemberResponse);

            await ctx.DeleteMemberFromListAsync("abc", "def");

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    "https://api.twitter.com/2/lists/abc/members/def",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ListMemberRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteMemberFromListAsync_WithGoodParams_PopulatesResponse()
        {
            InitializeTwitterContext(DeleteListMemberResponse);

            ListResponse response = await ctx.DeleteMemberFromListAsync("abc", "def");

            Assert.IsNotNull(response);
            ListResponseData data = response.Data;
            Assert.IsFalse(data.IsMember);
        }

        [TestMethod]
        public async Task AddFollowerToListAsync_WithoutListID_Throws()
        {
            InitializeTwitterContext(AddListFollowerResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddFollowerToListAsync(null, "def"));

            Assert.AreEqual("listID", ex.ParamName);
        }

        [TestMethod]
        public async Task AddFollowerToListAsync_WithoutUserID_Throws()
        {
            InitializeTwitterContext(AddListFollowerResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddFollowerToListAsync("abc", null));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task AddFollowerToListAsync_WithGoodIDs_BuildsUrl()
        {
            InitializeTwitterContext(AddListFollowerResponse);

            await ctx.AddFollowerToListAsync("abc", "def");

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/2/users/def/followed_lists",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ListFollowOrPinRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task AddFollowerToListAsync_WithGoodParams_PopulatesResponse()
        {
            InitializeTwitterContext(AddListFollowerResponse);

            ListResponse response = await ctx.AddFollowerToListAsync("abc", "def");

            Assert.IsNotNull(response);
            ListResponseData data = response.Data;
            Assert.IsTrue(data.Following);
        }


        [TestMethod]
        public async Task DeleteFollowerFromListAsync_WithoutListID_Throws()
        {
            InitializeTwitterContext(DeleteListFollowResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteFollowerFromListAsync(null, "def"));

            Assert.AreEqual("listID", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteFollowerFromListAsync_WithoutUserID_Throws()
        {
            InitializeTwitterContext(DeleteListFollowResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteFollowerFromListAsync("abc", null));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteFollowerFromListAsync_WithGoodIDs_BuildsUrl()
        {
            InitializeTwitterContext(DeleteListFollowResponse);

            await ctx.DeleteFollowerFromListAsync("abc", "def");

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    "https://api.twitter.com/2/users/def/followed_lists/abc",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ListFollowOrPinRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteFollowerFromListAsync_WithGoodParams_PopulatesResponse()
        {
            InitializeTwitterContext(DeleteListFollowResponse);

            ListResponse response = await ctx.DeleteFollowerFromListAsync("abc", "def");

            Assert.IsNotNull(response);
            ListResponseData data = response.Data;
            Assert.IsFalse(data.Following);
        }

        [TestMethod]
        public async Task PinListAsync_WithoutListID_Throws()
        {
            InitializeTwitterContext(PinResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.PinListAsync(null, "def"));

            Assert.AreEqual("listID", ex.ParamName);
        }

        [TestMethod]
        public async Task PinListAsync_WithoutUserID_Throws()
        {
            InitializeTwitterContext(PinResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddFollowerToListAsync("abc", null));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task PinListAsync_WithGoodIDs_BuildsUrl()
        {
            InitializeTwitterContext(PinResponse);

            await ctx.PinListAsync("abc", "def");

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/2/users/def/pinned_lists",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ListFollowOrPinRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task PinListAsync_WithGoodParams_PopulatesResponse()
        {
            InitializeTwitterContext(PinResponse);

            ListResponse response = await ctx.PinListAsync("abc", "def");

            Assert.IsNotNull(response);
            ListResponseData data = response.Data;
            Assert.IsTrue(data.Pinned);
        }


        [TestMethod]
        public async Task UnpinListAsync_WithoutListID_Throws()
        {
            InitializeTwitterContext(UnpinResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UnpinListAsync(null, "def"));

            Assert.AreEqual("listID", ex.ParamName);
        }

        [TestMethod]
        public async Task UnpinListAsync_WithoutUserID_Throws()
        {
            InitializeTwitterContext(UnpinResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UnpinListAsync("abc", null));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task UnpinListAsync_WithGoodIDs_BuildsUrl()
        {
            InitializeTwitterContext(UnpinResponse);

            await ctx.UnpinListAsync("abc", "def");

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    "https://api.twitter.com/2/users/def/pinned_lists/abc",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ListFollowOrPinRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task UnpinListAsync_WithGoodParams_PopulatesResponse()
        {
            InitializeTwitterContext(DeleteListFollowResponse);

            ListResponse response = await ctx.UnpinListAsync("abc", "def");

            Assert.IsNotNull(response);
            ListResponseData data = response.Data;
            Assert.IsFalse(data.Pinned);
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

        const string AddListMemberResponse = @"{
  ""data"": {
    ""is_member"": true
  }
}";

        const string DeleteListMemberResponse = @"{
  ""data"": {
    ""is_member"": false
  }
}";

        const string AddListFollowerResponse = @"{
  ""data"": {
    ""following"": true
  }
}";

        const string DeleteListFollowResponse = @"{
  ""data"": {
    ""following"": false
  }
}";

        const string PinResponse = @"{
  ""data"": {
    ""pinned"": true
  }
}";

        const string UnpinResponse = @"{
  ""data"": {
    ""pinned"": false
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
