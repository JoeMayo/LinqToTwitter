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
                exec.SendJsonToTwitterAsync<TwitterUserTargetID>(
                    HttpMethod.Put.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<TwitterUserTargetID>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task HideReplyAsync_WithReply_ReturnsTrue()
        {
            const string ReplyID = "184835136037191681";
            var ctx = await InitializeTwitterContextAsync(HideReplyResponse);

            bool isHidden = await ctx.HideReplyAsync(ReplyID);

            Assert.IsTrue(isHidden);
        }

        [TestMethod]
        public async Task UnHideReplyAsync_WithReply_ReturnsFalse()
        {
            const string ReplyID = "184835136037191681";
            var ctx = await InitializeTwitterContextAsync(UnHideReplyResponse);

            bool isHidden = await ctx.HideReplyAsync(ReplyID);

            Assert.IsFalse(isHidden);
        }

        [TestMethod]
        public async Task HideReplyAsync_WithNullTweetID_ReturnsTrue()
        {
            const string ReplyID = null;
            var ctx = await InitializeTwitterContextAsync(HideReplyResponse);

            await L2TAssert.Throws<ArgumentNullException>(() => ctx.HideReplyAsync(ReplyID));
        }

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
    }
}
