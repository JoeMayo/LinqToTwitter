using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterTests.Common;
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
            var reqProc = new TwitterContext().CreateRequestProcessor<Status>();

            Assert.IsType<StatusRequestProcessor<Status>>(reqProc);
        }
    }
}
