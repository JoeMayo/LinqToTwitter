using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter.OAuth;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitter.Tests.StatusTests
{
    [TestClass]
    public class TweetCommandsTests
    {
        public TweetCommandsTests()
        {
            TestCulture.SetCulture();
        }

        async Task<TwitterContext> InitializeTwitterContextAsync(string result)
        {
            await Task.Delay(1);
            var authMock = new Mock<IAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();

            var tcsAuth = new TaskCompletionSource<IAuthorizer>();
            tcsAuth.SetResult(authMock.Object);

            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(result);

            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Put.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<TweetHidden>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<TweetRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<TweetDeleteRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task TweetAsync_WithText_PopulatesResponse()
        {
            const string ExpectedText = "Hello";
            const string ExpectedID = "1460045236168654853";
            var ctx = await InitializeTwitterContextAsync(TweetResponse);

            Tweet actual = await ctx.TweetAsync(ExpectedText);

            Assert.AreEqual(ExpectedID, actual.ID);
            Assert.AreEqual(ExpectedText, actual.Text);
        }

        [TestMethod]
        public async Task TweetAsync_WithNullText_Throws()
        {
            var ctx = await InitializeTwitterContextAsync(TweetResponse);

            ArgumentNullException ex =
                await L2TAssert.Throws<ArgumentNullException>(async () =>
                    await ctx.TweetAsync(null));

            Assert.AreEqual("text", ex.ParamName);
        }

        [TestMethod]
        public async Task TweetMediaAsync_WithMediaIds_Succeeds()
        {
            const string ExpectedText = "Hello";
            const string ExpectedID = "1460045236168654853";

            List<string> mediaIds = new() { "521449660083609601" };
            List<string> taggedUserIds = new() { "521449660083609601" };

            var ctx = await InitializeTwitterContextAsync(TweetResponse);

            Tweet actual = await ctx.TweetMediaAsync("a", mediaIds, taggedUserIds);

            Assert.IsNotNull(actual);
            Assert.AreEqual(ExpectedID, actual.ID);
            Assert.AreEqual(ExpectedText, actual.Text);
        }

        [TestMethod]
        public async Task TweetMediaAsync_WithoutMediaIds_Throws()
        {
            List<string> mediaIds = null;
            List<string> taggedUserIds = new() { "521449660083609601" };

            var ctx = await InitializeTwitterContextAsync(TweetResponse);

            ArgumentNullException ex =
                await L2TAssert.Throws<ArgumentNullException>(async () =>
                    await ctx.TweetMediaAsync("a", mediaIds, taggedUserIds));

            Assert.AreEqual("mediaIds", ex.ParamName);
        }

        [TestMethod]
        public async Task TweetPollAsync_WithPollData_Succeeds()
        {
            const string ExpectedText = "Hello";
            const string ExpectedID = "1460045236168654853";

            int duration = 360;
            List<string> options = new() { "ein", "zwei", "drei" };

            var ctx = await InitializeTwitterContextAsync(TweetResponse);

            Tweet actual = await ctx.TweetPollAsync("a", duration, options);

            Assert.IsNotNull(actual);
            Assert.AreEqual(ExpectedID, actual.ID);
            Assert.AreEqual(ExpectedText, actual.Text);
        }

        [TestMethod]
        public async Task TweetPollAsync_WithoutDuration_Throws()
        {
            int duration = 0;
            List<string> options = new() { "ein", "zwei", "drei" };

            var ctx = await InitializeTwitterContextAsync(TweetResponse);

            ArgumentNullException ex =
                await L2TAssert.Throws<ArgumentNullException>(async () =>
                    await ctx.TweetPollAsync("a", duration, options));

            Assert.AreEqual("duration", ex.ParamName);
        }

        [TestMethod]
        public async Task TweetPollAsync_WithoutOptions_Throws()
        {
            int duration = 360;
            List<string> options = null;

            var ctx = await InitializeTwitterContextAsync(TweetResponse);

            ArgumentNullException ex =
                await L2TAssert.Throws<ArgumentNullException>(async () =>
                    await ctx.TweetPollAsync("a", duration, options));

            Assert.AreEqual("options", ex.ParamName);
        }

        [TestMethod]
        public async Task ReplyAsync_WithText_PopulatesResponse()
        {
            const string ExpectedText = "Hello";
            const string ExpectedID = "1460045236168654853";
            var ctx = await InitializeTwitterContextAsync(TweetResponse);

            Tweet actual = await ctx.ReplyAsync(ExpectedText, "42");

            Assert.AreEqual(ExpectedID, actual.ID);
            Assert.AreEqual(ExpectedText, actual.Text);
        }

        [TestMethod]
        public async Task ReplyAsync_WithNullText_Throws()
        {
            var ctx = await InitializeTwitterContextAsync(TweetResponse);

            ArgumentNullException ex =
                await L2TAssert.Throws<ArgumentNullException>(async () =>
                    await ctx.ReplyAsync("Hello", null));

            Assert.AreEqual("replyTweetID", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteTweetAsync_WithTweetID_Succeeds()
        {
            var ctx = await InitializeTwitterContextAsync(TweetDeletedResponse);

            TweetDeletedResponse actual = await ctx.DeleteTweetAsync("42");

            Assert.IsTrue(actual.Data.Deleted);
        }

        [TestMethod]
        public async Task DeleteTweetAsync_WithMissingID_Throws()
        {
            var ctx = await InitializeTwitterContextAsync(TweetDeletedResponse);

            ArgumentNullException ex =
                await L2TAssert.Throws<ArgumentNullException>(async () =>
                    await ctx.DeleteTweetAsync(null));

            Assert.AreEqual("tweetID", ex.ParamName);
        }

        [TestMethod]
        public async Task HideReplyAsync_WithReply_ReturnsTrue()
        {
            const string ReplyID = "184835136037191681";
            var ctx = await InitializeTwitterContextAsync(HideReplyResponse);

            TweetHideResponse actual = await ctx.HideReplyAsync(ReplyID);

            Assert.IsTrue(actual.Data.Hidden);
        }

        [TestMethod]
        public async Task UnHideReplyAsync_WithReply_ReturnsFalse()
        {
            const string ReplyID = "184835136037191681";
            var ctx = await InitializeTwitterContextAsync(UnHideReplyResponse);

            TweetHideResponse actual = await ctx.HideReplyAsync(ReplyID);

            Assert.IsFalse(actual.Data.Hidden);
        }

        [TestMethod]
        public async Task HideReplyAsync_WithNullTweetID_ReturnsTrue()
        {
            const string ReplyID = null;
            var ctx = await InitializeTwitterContextAsync(HideReplyResponse);

            await L2TAssert.Throws<ArgumentNullException>(() => ctx.HideReplyAsync(ReplyID));
        }

        const string TweetResponse = @"{
	""data"": {
		""id"": ""1460045236168654853"",
		""text"": ""Hello""
	}
}";

        const string HideReplyResponse = @"{
	""data"": {
		""hidden"": true
	}
}";

        const string UnHideReplyResponse = @"{
	""data"": {
		""hidden"": false
	}
}";

        const string TweetDeletedResponse = @"{
  ""data"": {
    ""deleted"": true
  }
}";
    }
}
