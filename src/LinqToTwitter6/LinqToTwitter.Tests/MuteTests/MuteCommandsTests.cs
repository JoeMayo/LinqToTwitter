using LinqToTwitter.OAuth;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.MuteTests
{
    [TestClass]
    public class MuteCommandsTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        Mock<ITwitterExecute> execMock;

        public MuteCommandsTests()
        {
            TestCulture.SetCulture();
        }

        TwitterContext InitTwitterContextWithPostToTwitter<TEntity>(string response)
        {
            var authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(response);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<TwitterUserTargetID>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task MuteAsync_WithUserParams_SendsJson()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";

            var ctx = InitTwitterContextWithPostToTwitter<MuteResponse>(MuteUserResponse);

            MuteResponse actual = await ctx.MuteAsync(SourceUserID, TargetUserID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    $"{BaseUrl2}users/{SourceUserID}/muting",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<TwitterUserTargetID>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }


        [TestMethod]
        public async Task MuteAsync_WithUserParams_ReturnsMutingTrue()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";

            var ctx = InitTwitterContextWithPostToTwitter<MuteResponse>(MuteUserResponse);

            MuteResponse actual = await ctx.MuteAsync(SourceUserID, TargetUserID);

            Assert.IsTrue(actual?.Data?.Muting ?? false);
        }

        [TestMethod]
        public async Task MuteAsync_MissingSourceID_Throws()
        {
            const string ExpectedParamName = "sourceUserID";
            var ctx = InitTwitterContextWithPostToTwitter<User>(MuteUserResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.MuteAsync(null, "2"));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task MuteAsync_MissingTargetID_Throws()
        {
            const string ExpectedParamName = "targetUserID";
            var ctx = InitTwitterContextWithPostToTwitter<User>(MuteUserResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.MuteAsync("1", null));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UnMuteAsync_WithUserParams_SendsJson()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";

            var ctx = InitTwitterContextWithPostToTwitter<MuteResponse>(MuteUserResponse);

            MuteResponse actual = await ctx.UnMuteAsync(SourceUserID, TargetUserID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    $"{BaseUrl2}users/{SourceUserID}/muting/{TargetUserID}",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<TwitterUserTargetID>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }


        [TestMethod]
        public async Task UnMuteAsync_WithUserParams_ReturnsMutingTrue()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";

            var ctx = InitTwitterContextWithPostToTwitter<MuteResponse>(MuteUserResponse);

            MuteResponse actual = await ctx.UnMuteAsync(SourceUserID, TargetUserID);

            Assert.IsTrue(actual?.Data?.Muting ?? false);
        }

        [TestMethod]
        public async Task UnMuteAsync_MissingSourceID_Throws()
        {
            const string ExpectedParamName = "sourceUserID";
            var ctx = InitTwitterContextWithPostToTwitter<User>(MuteUserResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.UnMuteAsync(null, "2"));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UnMuteAsync_MissingTargetID_Throws()
        {
            const string ExpectedParamName = "targetUserID";
            var ctx = InitTwitterContextWithPostToTwitter<User>(MuteUserResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.UnMuteAsync("1", null));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        const string MuteUserResponse = @"{
  ""data"": {
    ""muting"": true
  }
}";
    }
}
