using LinqToTwitter.OAuth;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.FavoritesTests
{
    [TestClass]
    public class LikeCommandsTests
    {
        public LikeCommandsTests()
        {
            TestCulture.SetCulture();
        }

        static TwitterContext InitializeTwitterContext(string response)
        {
            var authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(response);
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<LikedTweetID>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<LikedTweetID>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task LikeAsync_WithValidID_Succeeds()
        {
            const string UserId = "123";
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(LikeResponse);

            LikedResponse actual = await ctx.LikeAsync(UserId, TweetID);

            Assert.IsTrue(actual.Data.Liked);
        }

        [TestMethod]
        public async Task LikeAsync_WithRawResult_Succeeds()
        {
            const string UserId = "123";
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(LikeResponse);

            await ctx.LikeAsync(UserId, TweetID);

            Assert.AreEqual(LikeResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task LikeAsync_WithNullUserID_Throws()
        {
            const string UserId = null;
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(LikeResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.LikeAsync(UserId, TweetID));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task LikeAsync_WithNullTweetID_Throws()
        {
            const string UserId = "123";
            const string TweetID = null;
            var ctx = InitializeTwitterContext(LikeResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.LikeAsync(UserId, TweetID));

            Assert.AreEqual("tweetID", ex.ParamName);
        }

        [TestMethod]
        public async Task UnlikeAsync_WithValidID_Succeeds()
        {
            const string UserId = "123";
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(UnlikeResponse);

            LikedResponse actual = await ctx.UnlikeAsync(UserId, TweetID);

            Assert.IsFalse(actual.Data.Liked);
        }

        [TestMethod]
        public async Task UnlikeAsync_WithRawResult_Succeeds()
        {
            const string UserId = "123";
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(UnlikeResponse);

            await ctx.UnlikeAsync(UserId, TweetID);

            Assert.AreEqual(UnlikeResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UnlikeAsync_WithNullUserID_Throws()
        {
            const string UserId = null;
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(UnlikeResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.UnlikeAsync(UserId, TweetID));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task UnlikeAsync_WithNullTweetID_Throws()
        {
            const string UserId = "123";
            const string TweetID = null;
            var ctx = InitializeTwitterContext(UnlikeResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.UnlikeAsync(UserId, TweetID));

            Assert.AreEqual("tweetID", ex.ParamName);
        }

        const string LikeResponse = @"{
	""data"": {
		""liked"": true
	}
}";

        const string UnlikeResponse = @"{
	""data"": {
		""liked"": false
	}
}";
    }
}
