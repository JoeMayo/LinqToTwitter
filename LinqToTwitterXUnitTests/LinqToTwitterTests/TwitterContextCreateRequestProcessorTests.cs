using System;
using System.Linq;
using LinqToTwitter;
using Xunit;

namespace LinqToTwitterXUnitTests.LinqToTwitterTests
{
    public class TwitterContextCreateRequestProcessorTests
    {
        [Fact]
        public void CreateRequestProcessor_Returns_StatusRequestProcessor()
        {
            var reqProc = new TwitterContext().CreateRequestProcessor<Status>();

            Assert.IsType<StatusRequestProcessor<Status>>(reqProc);
        }
    }
}
