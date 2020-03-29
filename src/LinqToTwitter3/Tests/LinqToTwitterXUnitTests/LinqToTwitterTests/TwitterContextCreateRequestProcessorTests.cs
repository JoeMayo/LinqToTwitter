using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.LinqToTwitterTests
{
    public class TwitterContextCreateRequestProcessorTests
    {
        public TwitterContextCreateRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void CreateRequestProcessor_Returns_StatusRequestProcessor()
        {
            ITwitterAuthorizer auth = new Mock<ITwitterAuthorizer>().Object;

            var reqProc = new TwitterContext(auth).CreateRequestProcessor<Status>();

            Assert.IsType<StatusRequestProcessor<Status>>(reqProc);
        }
    }
}
