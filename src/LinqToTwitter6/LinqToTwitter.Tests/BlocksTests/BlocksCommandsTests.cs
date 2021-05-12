using LinqToTwitter.OAuth;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.BlocksTests
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

        TwitterContext InitializeTwitterContext(string response)
        {
            authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(response);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<TwitterUserTargetID>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task BlockUserAsync_Handles_Response()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";
            var ctx = InitializeTwitterContext(BlockUserResponse);

            BlockingResponse actual = await ctx.BlockUserAsync(SourceUserID, TargetUserID);

            Assert.IsTrue(actual.Data.Blocking);
        }

        [TestMethod]
        public async Task BlockUserAsync_WithRawResult_Succeeds()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";
            var ctx = InitializeTwitterContext(BlockUserResponse);

            await ctx.BlockUserAsync(SourceUserID, TargetUserID);

            Assert.AreEqual(BlockUserResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task BlockUserAsync_Builds_Url()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";
            var ctx = InitializeTwitterContext(BlockUserResponse);

            await ctx.BlockUserAsync(SourceUserID, TargetUserID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/2/users/1/blocking",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<TwitterUserTargetID>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task BlockUserAsync_Throws_On_Null_SourceUserID()
        {
            const string SourceUserID = null;
            const string TargetUserID = "2";
            var ctx = InitializeTwitterContext(BlockUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.BlockUserAsync(SourceUserID, TargetUserID));

            Assert.AreEqual("sourceUserID", ex.ParamName);
        }

        [TestMethod]
        public async Task BlockUserAsync_Throws_On_Null_TargetUserID()
        {
            const string SourceUserID = "1";
            const string TargetUserID = null;
            var ctx = InitializeTwitterContext(BlockUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.BlockUserAsync(SourceUserID, TargetUserID));

            Assert.AreEqual("targetUserID", ex.ParamName);
        }

        [TestMethod]
        public async Task UnblockUserAsync_Handles_Response()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";
            var ctx = InitializeTwitterContext(UnblockUserResponse);

            BlockingResponse actual = await ctx.UnblockUserAsync(SourceUserID, TargetUserID);

            Assert.IsFalse(actual.Data.Blocking);
        }

        [TestMethod]
        public async Task UnblockUserAsync_WithRawResult_Succeeds()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";
            var ctx = InitializeTwitterContext(UnblockUserResponse);

            await ctx.UnblockUserAsync(SourceUserID, TargetUserID);

            Assert.AreEqual(UnblockUserResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UnblockUserAsync_Builds_Url()
        {
            const string SourceUserID = "1";
            const string TargetUserID = "2";
            var ctx = InitializeTwitterContext(UnblockUserResponse);

            await ctx.UnblockUserAsync(SourceUserID, TargetUserID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    "https://api.twitter.com/2/users/1/blocking/2",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<TwitterUserTargetID>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task UnblockUserAsync_Throws_On_No_SourceUserID()
        {
            const string SourceUserID = null;
            const string TargetUserID = "2";
            var ctx = InitializeTwitterContext(UnblockUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UnblockUserAsync(SourceUserID, TargetUserID));

            Assert.AreEqual("sourceUserID", ex.ParamName);
        }

        [TestMethod]
        public async Task UnblockUserAsync_Throws_On_No_TargetUserID()
        {
            const string SourceUserID = "1";
            const string TargetUserID = null;
            var ctx = InitializeTwitterContext(UnblockUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UnblockUserAsync(SourceUserID, TargetUserID));

            Assert.AreEqual("targetUserID", ex.ParamName);
        }

        const string BlockUserResponse = @"{
    ""data"": {
        ""blocking"":true}
    }";

        const string UnblockUserResponse = @"{
    ""data"": {
        ""blocking"":false}
    }";

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
