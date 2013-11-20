using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterPcl.Tests.FriendshipTests
{
    [TestClass]
    class FriendshipExtensionsTests
    {
        TwitterContext ctx;
        Mock<ITwitterExecute> execMock;

        public FriendshipExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        void InitializeTwitterContext<TEntity>(string response)
        {
            var authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(response);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostToTwitterAsync<TEntity>(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>()))
                .Returns(tcsResponse.Task);
            ctx = new TwitterContext(execMock.Object);
        }

        [TestMethod]
        public async Task FriendshipRequestProcessor_Works_With_Actions()
        {
            var freindReqProc = new FriendshipRequestProcessor<Friendship>();

            Assert.IsInstanceOfType(freindReqProc, typeof(IRequestProcessorWithAction<Friendship>));
        }

        [TestMethod]
        public async Task CreateFriendshipTest()
        {
            const string ScreenName = "JoeMayo";
            const bool Follow = false;
            string expectedName = "Joe Mayo";
            InitializeTwitterContext<User>(SingleUserResponse);

            User actual = await ctx.CreateFriendshipAsync(ScreenName, Follow);

            Assert.AreEqual(expectedName, actual.Name);
        }

        [TestMethod]
        [Ignore]
        public async Task CreateFriendshipNoInputTest()
        {
            string userID = string.Empty;
            const bool Follow = false;
            InitializeTwitterContext<User>(SingleUserResponse);

            //var ex = Assert.Throws<ArgumentException>(() => ctx.CreateFriendshipAsync(userID, null, Follow));

            //Assert.AreEqual("UserIDOrScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task DestroyFriendshipTest()
        {
            const string UserID = "2";
            const string ScreenName = "JoeMayo";
            string expectedName = "Joe Mayo";
            InitializeTwitterContext<User>(SingleUserResponse);

            User actual = await ctx.DestroyFriendshipAsync(ScreenName);

            Assert.AreEqual(expectedName, actual.Name);
        }

        [TestMethod]
        [Ignore]
        public async Task DestroyFriendshipNoInputTest()
        {
            string userID = string.Empty;
            InitializeTwitterContext<User>(SingleUserResponse);

            //var ex = Assert.Throws<ArgumentException>(() => ctx.DestroyFriendshipAsync(null, userID, null));

            //Assert.AreEqual("UserIDOrScreenName", ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateFriendshipSettings_Calls_Execute()
        {
            InitializeTwitterContext<Friendship>(RelationshipResponse);

            await ctx.UpdateFriendshipSettingsAsync("Linq2Tweeter", true, true);

            execMock.Verify(exec => exec.PostToTwitterAsync<Friendship>(
                "https://api.twitter.com/1.1/friendships/update.json",
                It.IsAny<Dictionary<string, string>>()),
                Times.Once());
        }

        [TestMethod]
        [Ignore]
        public async Task UpdateFriendshipSettings_Throws_Without_ScreenName_Or_UserID()
        {
            InitializeTwitterContext<Friendship>(RelationshipResponse);

            //var ex = Assert.Throws<ArgumentNullException>(() => ctx.UpdateFriendshipSettingsAsync(null, true, true));

            //Assert.AreEqual("screenNameOrUserID", ex.ParamName);
        }

        const string SingleUserResponse = @"{
   ""id"":15411837,
   ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
   ""url"":""http:\/\/www.mayosoftware.com"",
   ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
   ""followers_count"":1101,
   ""default_profile"":false,
   ""profile_background_color"":""0099B9"",
   ""lang"":""en"",
   ""utc_offset"":-25200,
   ""name"":""Joe Mayo"",
   ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
   ""location"":""Denver, CO"",
   ""profile_link_color"":""0099B9"",
   ""listed_count"":113,
   ""verified"":false,
   ""protected"":false,
   ""profile_use_background_image"":true,
   ""is_translator"":false,
   ""following"":true,
   ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
   ""profile_text_color"":""3C3940"",
   ""statuses_count"":1907,
   ""screen_name"":""JoeMayo"",
   ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
   ""time_zone"":""Mountain Time (US & Canada)"",
   ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
   ""friends_count"":210,
   ""default_profile_image"":false,
   ""contributors_enabled"":false,
   ""profile_sidebar_border_color"":""5ED4DC"",
   ""id_str"":""15411837"",
   ""geo_enabled"":true,
   ""favourites_count"":44,
   ""status"":{
      ""retweeted"":true,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""contributors"":null,
      ""coordinates"":null,
      ""possibly_sensitive_editable"":true,
      ""place"":null,
      ""retweet_count"":3,
      ""id_str"":""196991337554378752"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""source"":""web"",
      ""created_at"":""Mon Apr 30 15:56:09 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":196991337554378752,
      ""geo"":null,
      ""text"":""Funny - http:\/\/t.co\/yZW2Sbmi :)""
   },
   ""profile_background_tile"":false,
   ""notifications"":false,
   ""show_all_inline_media"":false,
   ""profile_sidebar_fill_color"":""95E8EC"",
   ""follow_request_sent"":false
}";

        const string RelationshipResponse = @"{
   ""relationship"":{
      ""target"":{
         ""screen_name"":""JoeMayo"",
         ""followed_by"":true,
         ""id_str"":""15411837"",
         ""following"":false,
         ""id"":15411837
      },
      ""source"":{
         ""screen_name"":""Linq2Tweeter"",
         ""want_retweets"":true,
         ""all_replies"":false,
         ""marked_spam"":false,
         ""followed_by"":false,
         ""id_str"":""16761255"",
         ""blocking"":false,
         ""notifications_enabled"":true,
         ""following"":true,
         ""id"":16761255,
         ""can_dm"":false
      }
   }
}";

    }
}
