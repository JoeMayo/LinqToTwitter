using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class ControlStreamExtensionsTest
    {
        Mock<ITwitterAuthorizer> authMock;
        Mock<ITwitterExecute> execMock;

        public ControlStreamExtensionsTest()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void ControlStreamRequestProcessor_Works_With_Actions()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream>();

            Assert.IsAssignableFrom<IRequestProcessorWithAction<ControlStream>>(reqProc);
        }

        TwitterContext InitializeTwitterContext()
        {
            authMock = new Mock<ITwitterAuthorizer>();
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.PostToTwitter(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<Func<string, ControlStream>>()))
                    .Returns(CommandResponse);
            var ctx = new TwitterContext(execMock.Object);
            ctx.SiteStreamUrl = "https://sitestream.twitter.com/1.1/";
            return ctx;
        }

        [Fact]
        public void AddSiteStreamUser_Constructs_Url()
        {
            const ulong UserID = 1;
            const string StreamID = "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f";

            var ctx = InitializeTwitterContext();

            ctx.AddSiteStreamUser(new List<ulong> { UserID }, StreamID);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    "https://sitestream.twitter.com/1.1/site/c/1_1_54e345d655ee3e8df359ac033648530bfbe26c5f/add_user.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Func<string, ControlStream>>()),
                Times.Once());
        }

        [Fact]
        public void AddSiteStreamUser_Throws_On_Null_StreamID()
        {
            const ulong UserID = 1;
            var ctx = InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentNullException>(() => ctx.AddSiteStreamUser(new List<ulong> { UserID }, null));

            Assert.Equal("streamID", ex.ParamName);
        }

        [Fact]
        public void AddSiteStreamUser_Returns_ControlStream()
        {
            const ulong UserID = 1;
            const string StreamID = "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f";

            var ctx = InitializeTwitterContext();

            ControlStream cs = ctx.AddSiteStreamUser(new List<ulong> { UserID }, StreamID);

            Assert.NotNull(cs);
            Assert.Equal(CommandResponse, cs.CommandResponse);
        }

        [Fact]
        public void RemoveSiteStreamUser_Constructs_Url()
        {
            const ulong UserID = 1;
            const string StreamID = "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f";

            var ctx = InitializeTwitterContext();

            ctx.RemoveSiteStreamUser(new List<ulong> { UserID }, StreamID);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    "https://sitestream.twitter.com/1.1/site/c/1_1_54e345d655ee3e8df359ac033648530bfbe26c5f/remove_user.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Func<string, ControlStream>>()),
                Times.Once());
        }

        [Fact]
        public void RemoveSiteStreamUser_Throws_On_Null_StreamID()
        {
            const ulong UserID = 1;
            var ctx = InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentNullException>(() => ctx.RemoveSiteStreamUser(new List<ulong> { UserID }, null));

            Assert.Equal("streamID", ex.ParamName);
        }

        [Fact]
        public void RemoveSiteStreamUser_Returns_ControlStream()
        {
            const ulong UserID = 1;
            const string StreamID = "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f";

            var ctx = InitializeTwitterContext();

            ControlStream cs = ctx.RemoveSiteStreamUser(new List<ulong> { UserID }, StreamID);

            Assert.NotNull(cs);
            Assert.Equal(CommandResponse, cs.CommandResponse);
        }


        const string CommandResponse = @"stream updated

";
    }
}
