using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.RawTests
{
    public class RawExtensionsTests
    {
        public RawExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void RawRequestProcessor_Works_With_Actions()
        {
            var rawReqProc = new RawRequestProcessor<Raw>();

            Assert.IsAssignableFrom<IRequestProcessorWithAction<Raw>>(rawReqProc);
        }

        [Fact]
        public void ExecuteRawRequest_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            const string QueryString = "statuses/update.json";
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            ctx.ExecuteRaw(QueryString, parameters);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    "https://api.twitter.com/1.1/statuses/update.json",
                    parameters,
                    It.IsAny<Func<string, Raw>>()),
                Times.Once());
        }

        [Fact]
        public void ExecuteRawRequest_Returns_Raw_Result()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            const string QueryString = "statuses/update.json";
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };
            const string ExpectedResult = "<status>xxx</status>";
            const string FullUrl = "https://api.twitter.com/1.1/statuses/update.json";
            execMock.Setup(exec => exec.PostToTwitter(FullUrl, parameters, It.IsAny<Func<string, Raw>>())).Returns(ExpectedResult);

            string actualResult = ctx.ExecuteRaw(QueryString, parameters);

            Assert.Equal(ExpectedResult, actualResult);
        }

        [Fact]
        public void ExecuteRawRequest_Resolves_Too_Many_Url_Slashes()
        {
            const string BaseUrlWithTrailingSlash = "https://api.twitter.com/1.1/";
            const string QueryStringWithBeginningSlash = "/statuses/update.json";
            const string FullUrl = "https://api.twitter.com/1.1/statuses/update.json";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, BaseUrlWithTrailingSlash, "");
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            ctx.ExecuteRaw(QueryStringWithBeginningSlash, parameters);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    FullUrl,
                    parameters,
                    It.IsAny<Func<string, Raw>>()), Times.Once());
        }

        [Fact]
        public void ExecuteRawRequest_Resolves_Too_Few_Url_Slashes()
        {
            const string BaseUrlWithoutTrailingSlash = "https://api.twitter.com/1.1/";
            const string QueryStringWithoutBeginningSlash = "statuses/update.json";
            const string FullUrl = "https://api.twitter.com/1.1/statuses/update.json";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, BaseUrlWithoutTrailingSlash, "");
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            ctx.ExecuteRaw(QueryStringWithoutBeginningSlash, parameters);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    FullUrl,
                    parameters, It.IsAny<Func<string, Raw>>()), Times.Once());
        }
    }
}
