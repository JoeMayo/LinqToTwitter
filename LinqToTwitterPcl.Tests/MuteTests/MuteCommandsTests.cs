using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterPcl.Tests.MuteTests
{
    [TestClass]
    public class MuteCommandsTests
    {
        Mock<ITwitterExecute> execMock;

        public MuteCommandsTests()
        {
            TestCulture.SetCulture();
        }
  
        TwitterContext InitTwitterContextWithPostToTwitter<TEntity>(string response)
        {
            var authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(response);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(
                exec => exec.PostToTwitterAsync<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>()))
                    .Returns(tcsResponse.Task);
            execMock.Setup(
                exec => exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                    .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task MuteAsync_ScreenName_Invokes_Executor_Execute()
        {
            const string ScreenName = "twitterapi";

            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = await ctx.MuteAsync(ScreenName);

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<User>(
                    "https://api.twitter.com/1.1/mutes/users/create.json",
                    It.IsAny<Dictionary<string, string>>()),
                Times.Once());
            Assert.AreEqual(ScreenName, actual.ScreenNameResponse);
        }

        [TestMethod]
        public async Task MuteAsync_UserID_Invokes_Executor_Execute()
        {
            const string ScreenName = "twitterapi";
            const ulong UserID = 1;

            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = await ctx.MuteAsync(UserID);

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<User>(
                    "https://api.twitter.com/1.1/mutes/users/create.json",
                    It.IsAny<Dictionary<string, string>>()),
                Times.Once());
            Assert.AreEqual(ScreenName, actual.ScreenNameResponse);
        }

        [TestMethod]
        public async Task MuteAsync_WithRawResult_Succeeds()
        {
            const string ScreenName = "twitterapi";

            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            await ctx.MuteAsync(ScreenName);

            Assert.AreEqual(SingleUserResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task MuteAsync_ScreenName_Throws_On_Null_Input()
        {
            const string ExpectedParamName = "screenName";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.MuteAsync(null));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task MuteAsync_UserID_Throws_On_Zero_Input()
        {
            const string ExpectedParamName = "userID";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.MuteAsync(0));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UnMuteAsync_ScreenName_Invokes_Executor_Execute()
        {
            const string ScreenName = "twitterapi";

            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = await ctx.UnMuteAsync(ScreenName);

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<User>(
                    "https://api.twitter.com/1.1/mutes/users/destroy.json",
                    It.IsAny<Dictionary<string, string>>()),
                Times.Once());
            Assert.AreEqual(ScreenName, actual.ScreenNameResponse);
        }

        [TestMethod]
        public async Task UnMuteAsync_UserID_Invokes_Executor_Execute()
        {
            const string ScreenName = "twitterapi";
            const ulong UserID = 1;

            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = await ctx.UnMuteAsync(UserID);

            execMock.Verify(exec =>
                exec.PostToTwitterAsync<User>(
                    "https://api.twitter.com/1.1/mutes/users/destroy.json",
                    It.IsAny<Dictionary<string, string>>()),
                Times.Once());
            Assert.AreEqual(ScreenName, actual.ScreenNameResponse);
        }

        [TestMethod]
        public async Task UnMuteAsync_WithRawResult_Succeeds()
        {
            const string ScreenName = "twitterapi";

            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            await ctx.UnMuteAsync(ScreenName);

            Assert.AreEqual(SingleUserResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UnMuteAsync_ScreenName_Throws_On_Null_Input()
        {
            const string ExpectedParamName = "screenName";

            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentNullException>(
                async () => await ctx.UnMuteAsync(null));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        [TestMethod]
        public async Task UnMuteAsync_UserID_Throws_On_Zero_Input()
        {
            const string ExpectedParamName = "userID";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UnMuteAsync(0));

            Assert.AreEqual(ExpectedParamName, ex.ParamName);
        }

        const string SingleUserResponse = @"{
   ""id"":6253282,
   ""id_str"":""6253282"",
   ""name"":""Twitter API"",
   ""screen_name"":""twitterapi"",
   ""location"":""San Francisco, CA"",
   ""description"":""The Real Twitter API. I tweet about API changes, service issues and happily answer questions about Twitter and our API. Don't get an answer? It's on my website."",
   ""url"":""http:\/\/dev.twitter.com"",
   ""protected"":false,
   ""followers_count"":1009508,
   ""friends_count"":31,
   ""listed_count"":10361,
   ""created_at"":""Wed May 23 06:01:13 +0000 2007"",
   ""favourites_count"":24,
   ""utc_offset"":-28800,
   ""time_zone"":""Pacific Time (US & Canada)"",
   ""geo_enabled"":true,
   ""verified"":true,
   ""statuses_count"":3278,
   ""lang"":""en"",
   ""status"":{
      ""created_at"":""Mon Apr 30 17:16:17 +0000 2012"",
      ""id"":197011505181507585,
      ""id_str"":""197011505181507585"",
      ""text"":""Developer Teatime is coming to Paris - please sign up to join us on June 16th! https:\/\/t.co\/pQOUNKGD  @rno @jasoncosta"",
      ""source"":""web"",
      ""truncated"":false,
      ""in_reply_to_status_id"":null,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_user_id"":null,
      ""in_reply_to_user_id_str"":null,
      ""in_reply_to_screen_name"":null,
      ""geo"":null,
      ""coordinates"":null,
      ""place"":null,
      ""contributors"":[
         14927800
      ],
      ""retweet_count"":25,
      ""favorited"":false,
      ""retweeted"":false,
      ""possibly_sensitive"":false
   },
   ""contributors_enabled"":true,
   ""is_translator"":false,
   ""profile_background_color"":""E8F2F7"",
   ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/229557229\/twitterapi-bg.png"",
   ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/229557229\/twitterapi-bg.png"",
   ""profile_background_tile"":false,
   ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1438634086\/avatar_normal.png"",
   ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1438634086\/avatar_normal.png"",
   ""profile_banner_url"":""https:\/\/si0.twimg.com\/profile_images\/1438634086\/avatar_normal.png"",   
   ""profile_link_color"":""0094C2"",
   ""profile_sidebar_border_color"":""0094C2"",
   ""profile_sidebar_fill_color"":""A9D9F1"",
   ""profile_text_color"":""437792"",
   ""profile_use_background_image"":true,
   ""show_all_inline_media"":false,
   ""default_profile"":false,
   ""default_profile_image"":false,
   ""following"":false,
   ""follow_request_sent"":false,
   ""notifications"":false
}";
    }
}
