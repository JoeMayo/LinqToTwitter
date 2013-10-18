using System;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterPcl.Tests.Common
{
    public class TwitterExtensionsTests
    {
        public TwitterExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void StreamingCallback_Sets_TwitterExecutor_StreamCallback()
        {
            var authMock = new Mock<IAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            var ctxMock = new Mock<TwitterContext>(execMock.Object);
            var twQueryable = new TwitterQueryable<Streaming>(ctxMock.Object);
            Action<StreamContent> callback = content => Console.WriteLine(content.Content);

            twQueryable.StreamingCallback(callback);

            execMock.VerifySet(exec => exec.StreamingCallback = It.IsAny<Action<StreamContent>>());
        }
    }
}
