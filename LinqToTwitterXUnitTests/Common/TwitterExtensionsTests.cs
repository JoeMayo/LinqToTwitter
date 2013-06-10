using System;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class TwitterExtensionsTests
    {
        public TwitterExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void StreamingCallback_Sets_TwitterExecutor_StreamCallback()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctxMock = new Mock<TwitterContext>(execMock.Object);
            var twQueryable = new TwitterQueryable<Streaming>(ctxMock.Object);
            Action<StreamContent> callback = content => Console.WriteLine(content.Content);

            twQueryable.StreamingCallback(callback);

            execMock.VerifySet(exec => exec.StreamingCallback = It.IsAny<Action<StreamContent>>());
        }
    }
}
