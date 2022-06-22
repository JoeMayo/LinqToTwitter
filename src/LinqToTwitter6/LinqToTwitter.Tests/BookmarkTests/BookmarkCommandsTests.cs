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
    public class BookmarkCommandsTests
    {
        public BookmarkCommandsTests()
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
                    It.IsAny<BookmarkedTweetID>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<BookmarkedTweetID>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task BookmarkAsync_WithValidID_Succeeds()
        {
            const string UserId = "123";
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(BookmarkResponse);

            BookmarkResponse actual = await ctx.BookmarkAsync(UserId, TweetID);

            Assert.IsTrue(actual.Data.Bookmarked);
        }

        [TestMethod]
        public async Task BookmarkAsync_WithRawResult_Succeeds()
        {
            const string UserId = "123";
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(BookmarkResponse);

            await ctx.BookmarkAsync(UserId, TweetID);

            Assert.AreEqual(BookmarkResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task BookmarkAsync_WithNullUserID_Throws()
        {
            const string UserId = null;
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(BookmarkResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.BookmarkAsync(UserId, TweetID));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task BookmarkAsync_WithNullTweetID_Throws()
        {
            const string UserId = "123";
            const string TweetID = null;
            var ctx = InitializeTwitterContext(BookmarkResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.BookmarkAsync(UserId, TweetID));

            Assert.AreEqual("tweetID", ex.ParamName);
        }

        [TestMethod]
        public async Task RemoveBookmarkAsync_WithValidID_Succeeds()
        {
            const string UserId = "123";
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(RemoveBookmarkResponse);

            BookmarkResponse actual = await ctx.RemoveBookmarkAsync(UserId, TweetID);

            Assert.IsFalse(actual.Data.Bookmarked);
        }

        [TestMethod]
        public async Task RemoveBookmarkAsync_WithRawResult_Succeeds()
        {
            const string UserId = "123";
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(RemoveBookmarkResponse);

            await ctx.RemoveBookmarkAsync(UserId, TweetID);

            Assert.AreEqual(RemoveBookmarkResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task RemoveBookmarkAsync_WithNullUserID_Throws()
        {
            const string UserId = null;
            const string TweetID = "456";
            var ctx = InitializeTwitterContext(RemoveBookmarkResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.RemoveBookmarkAsync(UserId, TweetID));

            Assert.AreEqual("userID", ex.ParamName);
        }

        [TestMethod]
        public async Task RemoveBookmarkAsync_WithNullTweetID_Throws()
        {
            const string UserId = "123";
            const string TweetID = null;
            var ctx = InitializeTwitterContext(RemoveBookmarkResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.RemoveBookmarkAsync(UserId, TweetID));

            Assert.AreEqual("tweetID", ex.ParamName);
        }

        const string BookmarkResponse = @"{
	""data"": {
		""bookmarked"": true
	}
}";

        const string RemoveBookmarkResponse = @"{
	""data"": {
		""bookmarked"": false
	}
}";
    }
}
