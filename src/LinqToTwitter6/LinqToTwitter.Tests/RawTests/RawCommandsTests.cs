using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Net.Http;
using LinqToTwitter.Provider;
using LinqToTwitter.OAuth;

namespace LinqToTwitter.Tests.RawTests
{
    [TestClass]
    public class RawCommandsTests
    {
        TwitterContext ctx;
        Mock<ITwitterExecute> execMock;

        public RawCommandsTests()
        {
            TestCulture.SetCulture();
        }

        void InitializeTwitterContext()
        {
            var authMock = new Mock<IAuthorizer>();
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(EmptyRawResponse);
            execMock.Setup(
                exec => exec.PostFormUrlEncodedToTwitterAsync<Raw>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            ctx = new TwitterContext(execMock.Object);
        }

        [TestMethod]
        public void RawRequestProcessor_WorksWithActions()
        {
            var rawReqProc = new RawRequestProcessor<Raw>();

            Assert.IsInstanceOfType(rawReqProc, typeof(IRequestProcessorWithAction<Raw>));
        }

        [TestMethod]
        public async Task ExecuteRawAsync_InvokesPostFormUrlEncodedToTwitterAsync()
        {
            InitializeTwitterContext();
            const string QueryString = "statuses/update.json";
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            await ctx.ExecuteRawAsync(QueryString, parameters);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<Raw>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/statuses/update.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task ExecuteRawAsync_WithRawResultProperty_Succeeds()
        {
            InitializeTwitterContext();
            const string QueryString = "statuses/update.json";
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            await ctx.ExecuteRawAsync(QueryString, parameters, HttpMethod.Post);

            Assert.AreEqual(EmptyRawResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task ExecuteRawRequest_ReturnsRawResult()
        {
            InitializeTwitterContext();
            const string QueryString = "statuses/update.json";
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };
            const string ExpectedResult = "<status>xxx</status>";
            const string FullUrl = "https://api.twitter.com/1.1/statuses/update.json";
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(ExpectedResult);
            execMock.Setup(exec => exec.PostFormUrlEncodedToTwitterAsync<Raw>(HttpMethod.Post.ToString(), FullUrl, parameters, It.IsAny<CancellationToken>())).Returns(tcsResponse.Task);

            string actualResult = await ctx.ExecuteRawAsync(QueryString, parameters);

            Assert.AreEqual(ExpectedResult, actualResult);
        }

        [TestMethod]
        public async Task ExecuteRawRequest_WithTooManyUrlSlashes_StillWorks()
        {
            const string QueryStringWithBeginningSlash = "/statuses/update.json";
            const string FullUrl = "https://api.twitter.com/1.1/statuses/update.json";
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            await ctx.ExecuteRawAsync(QueryStringWithBeginningSlash, parameters);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<Raw>(
                    HttpMethod.Post.ToString(),
                    FullUrl,
                    parameters,
                    It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task ExecuteRawRequest_WithTooFewUrlSlashes_StillWorks()
        {
            const string QueryStringWithoutBeginningSlash = "statuses/update.json";
            const string FullUrl = "https://api.twitter.com/1.1/statuses/update.json";
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            await ctx.ExecuteRawAsync(QueryStringWithoutBeginningSlash, parameters);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<Raw>(
                    HttpMethod.Post.ToString(),
                    FullUrl,
                    parameters,
                    It.IsAny<CancellationToken>()), Times.Once());
        }

        const string EmptyRawResponse = "{}";
    }
}
