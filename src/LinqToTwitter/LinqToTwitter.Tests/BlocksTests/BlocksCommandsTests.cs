using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace LinqToTwitterPcl.Tests.BlocksTests
{
    [TestClass]
    public class BlocksCommandsTests
    {
        Mock<IAuthorizer> authMock;
        Mock<ITwitterExecute> execMock;

        public BlocksCommandsTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void BlocksRequestProcessor_Works_With_Actions()
        {
            var blocksReqProc = new BlocksRequestProcessor<User>();

            Assert.IsInstanceOfType(blocksReqProc, typeof(IRequestProcessorWithAction<User>));
        }

        TwitterContext InitializeTwitterContext()
        {
            authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(BlocksUserJson);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec => exec.PostFormUrlEncodedToTwitterAsync<User>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task CreateBlockAsync_Handles_Response()
        {
            const ulong Id = 1;
            const bool SkipStatus = true;
            var ctx = InitializeTwitterContext();

            User actual = await ctx.CreateBlockAsync(Id, null, SkipStatus);

            Assert.AreEqual("LINQ to Tweeter Test", actual.Name);
        }

        [TestMethod]
        public async Task CreateBlockAsync_WithRawResult_Succeeds()
        {
            const ulong Id = 1;
            const bool SkipStatus = true;
            var ctx = InitializeTwitterContext();

            await ctx.CreateBlockAsync(Id, null, SkipStatus);

            Assert.AreEqual(BlocksUserJson, ctx.RawResult);
        }

        [TestMethod]
        public async Task CreateBlockAsync_Builds_Url()
        {
            const ulong Id = 1;
            const bool SkipStatus = true;
            var ctx = InitializeTwitterContext();

            await ctx.CreateBlockAsync(Id, null, SkipStatus);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<User>(
                    "https://api.twitter.com/1.1/blocks/create.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task CreateBlockAsync_Throws_On_Null_UserID_And_ScreenName()
        {
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.CreateBlockAsync(0, null, true));

            Assert.AreEqual("UserIDOrScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task DestroyBlockAsync_Handles_Response()
        {
            const ulong Id = 1;
            const bool SkipStatus = true;
            var ctx = InitializeTwitterContext();

            User actual = await ctx.DestroyBlockAsync(Id, null, SkipStatus);

            Assert.AreEqual("LINQ to Tweeter Test", actual.Name);
        }

        [TestMethod]
        public async Task DestroyBlockAsync_WithRawResult_Succeeds()
        {
            const ulong Id = 1;
            const bool SkipStatus = true;
            var ctx = InitializeTwitterContext();

            await ctx.DestroyBlockAsync(Id, null, SkipStatus);

            Assert.AreEqual(BlocksUserJson, ctx.RawResult);
        }

        [TestMethod]
        public async Task DestroyBlockAsync_Builds_Url()
        {
            const ulong Id = 1;
            const bool SkipStatus = true;
            var ctx = InitializeTwitterContext();

            await ctx.DestroyBlockAsync(Id, null, SkipStatus);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<User>(
                    "https://api.twitter.com/1.1/blocks/destroy.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DestroyBlockAsync_Throws_On_No_ID_Or_ScreenName()
        {
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DestroyBlockAsync(0, null, true));

            Assert.AreEqual("UserIDOrScreenName", ex.ParamName);
        }

        const string BlocksUserJson = @"{
   ""id"":16761255,
   ""listed_count"":2,
   ""contributors_enabled"":false,
   ""profile_sidebar_border_color"":""87bc44"",
   ""geo_enabled"":false,
   ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
   ""friends_count"":0,
   ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
   ""profile_background_tile"":false,
   ""followers_count"":22,
   ""is_translator"":false,
   ""show_all_inline_media"":true,
   ""follow_request_sent"":false,
   ""statuses_count"":100,
   ""utc_offset"":-25200,
   ""profile_sidebar_fill_color"":""e0ff92"",
   ""name"":""LINQ to Tweeter Test"",
   ""default_profile_image"":false,
   ""protected"":false,
   ""profile_background_color"":""9ae4e8"",
   ""favourites_count"":2,
   ""lang"":""en"",
   ""url"":""http:\/\/linqtotwitter.codeplex.com"",
   ""verified"":false,
   ""created_at"":""Wed Oct 15 05:15:40 +0000 2008"",
   ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
   ""description"":""Testing the LINQ to Twitter Account Profile Update."",
   ""profile_link_color"":""0000ff"",
   ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1446295540\/200xColor_2_normal.png"",
   ""default_profile"":false,
   ""following"":false,
   ""profile_use_background_image"":true,
   ""location"":""Anywhere In The World"",
   ""notifications"":false,
   ""id_str"":""16761255"",
   ""profile_text_color"":""000000"",
   ""status"":{
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""created_at"":""Sun Mar 04 23:16:17 +0000 2012"",
      ""coordinates"":null,
      ""retweeted"":false,
      ""place"":null,
      ""in_reply_to_screen_name"":null,
      ""contributors"":null,
      ""retweet_count"":0,
      ""favorited"":false,
      ""in_reply_to_user_id"":null,
      ""source"":""\u003Ca href=\""http:\/\/www.csharp-station.com\/\"" rel=\""nofollow\""\u003EC# Station\u003C\/a\u003E"",
      ""in_reply_to_status_id_str"":null,
      ""geo"":null,
      ""in_reply_to_status_id"":null,
      ""id"":176445993091481604,
      ""id_str"":""176445993091481604"",
      ""text"":""Windows Phone Test, 03\/04\/2012 16:15:12 #linq2twitter""
   },
   ""time_zone"":""Mountain Time (US & Canada)"",
   ""screen_name"":""Linq2Tweeter""
}";
    }
}
