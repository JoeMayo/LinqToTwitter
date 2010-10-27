using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using LinqToTwitter;

namespace LinqToTwitterTests
{
    [TestClass]
    public class TwitterExtensionsTest
    {
        [TestMethod]
        public void StreamingCallback_Sets_TwitterExecutor_StreamCallback()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctxMock = new Mock<TwitterContext>(authMock.Object, execMock.Object, string.Empty, string.Empty);
            var twQueryable = new TwitterQueryable<Streaming>(ctxMock.Object);
            Action<StreamContent> callback = content => Console.WriteLine(content.Content);

            twQueryable.StreamingCallback(callback);

            execMock.VerifySet(exec => exec.StreamingCallback);
        }
    }
}
