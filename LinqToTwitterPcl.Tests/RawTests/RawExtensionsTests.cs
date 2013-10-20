using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterPcl.Tests.RawTests
{
    [TestClass]
    public class RawExtensionsTests
    {
        TwitterContext ctx;
        Mock<ITwitterExecute> execMock;

        public RawExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        void InitializeTwitterContext()
        {
            var authMock = new Mock<IAuthorizer>();
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            ctx = new TwitterContext(execMock.Object);
        }

        [TestMethod]
        public async Task RawRequestProcessor_Works_With_Actions()
        {
            var rawReqProc = new RawRequestProcessor<Raw>();

            Assert.IsInstanceOfType(rawReqProc, typeof(IRequestProcessorWithAction<Raw>));
        }

        [TestMethod]
        public async Task ExecuteRawRequest_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            const string QueryString = "statuses/update.json";
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            ctx.ExecuteRawAsync(QueryString, parameters);

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<Raw>(
                    "https://api.twitter.com/1.1/statuses/update.json",
                    parameters),
                Times.Once());
        }

        [TestMethod]
        public async Task ExecuteRawRequest_Returns_Raw_Result()
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
            execMock.Setup(exec => exec.PostToTwitterAsync<Raw>(FullUrl, parameters)).Returns(tcsResponse.Task);

            string actualResult = await ctx.ExecuteRawAsync(QueryString, parameters);

            Assert.AreEqual(ExpectedResult, actualResult);
        }

        [TestMethod]
        public async Task ExecuteRawRequest_Resolves_Too_Many_Url_Slashes()
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
                exec.PostToTwitterAsync<Raw>(
                    FullUrl,
                    parameters), Times.Once());
        }

        [TestMethod]
        public async Task ExecuteRawRequest_Resolves_Too_Few_Url_Slashes()
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
                exec.PostToTwitterAsync<Raw>(
                    FullUrl,
                    parameters), Times.Once());
        }
    }
}
