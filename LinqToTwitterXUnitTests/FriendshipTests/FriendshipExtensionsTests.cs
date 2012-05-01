using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.FriendshipTests
{
    class FriendshipExtensionsTests
    {
        public FriendshipExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void FriendshipRequestProcessor_Works_With_Actions()
        {
            var freindReqProc = new FriendshipRequestProcessor<Friendship>();

            Assert.IsAssignableFrom<IRequestProcessorWithAction<Friendship>>(freindReqProc);
        }

        [Fact]
        public void CreateFriendshipTest()
        {
            const string id = "1";
            const string userID = "2";
            const string screenName = "JoeMayo";
            const bool follow = false;
            string expectedName = "Joe Mayo";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(SingleUserResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.CreateFriendship(id, userID, screenName, follow);

            Assert.Equal(expectedName, actual.Name);
        }

        [Fact]
        public void CreateFriendshipNoInputTest()
        {
            string id = string.Empty;
            string userID = string.Empty;
            const bool follow = false;
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(SingleUserResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            var ex = Assert.Throws<ArgumentException>(() => ctx.CreateFriendship(id, userID, null, follow));

            Assert.Equal("IdUserIDOrScreenName", ex.ParamName);
        }

        [Fact]
        public void DestroyFriendshipTest()
        {
            const string id = "1";
            const string userID = "2";
            const string screenName = "JoeMayo";
            string expectedName = "Joe Mayo";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(SingleUserResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DestroyFriendship(id, userID, screenName);

            Assert.Equal(expectedName, actual.Name);
        }

        [Fact]
        public void DestroyFriendshipNoInputTest()
        {
            string userID = string.Empty;
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(SingleUserResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            var ex = Assert.Throws<ArgumentException>(() => ctx.DestroyFriendship(null, userID, null));

            Assert.Equal("IdUserIDOrScreenName", ex.ParamName);
        }

        [Fact]
        public void UpdateFriendshipSettings_Calls_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.ExecuteTwitter(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<IRequestProcessor<Friendship>>())).Returns(RelationshipResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "https://api.twitter.com/1/", "");

            ctx.UpdateFriendshipSettings("joemayo", true, true);

            execMock.Verify(exec => exec.ExecuteTwitter(
                "https://api.twitter.com/1/friendships/update.json",
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<IRequestProcessor<Friendship>>()),
                Times.Once());
        }

        [Fact]
        public void UpdateFriendshipSettings_Requires_ScreenName()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.ExecuteTwitter(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<IRequestProcessor<Friendship>>())).Returns(RelationshipResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "https://api.twitter.com/1/", "");

            var ex = Assert.Throws<ArgumentNullException>(() => ctx.UpdateFriendshipSettings(/*"joemayo"*/ null, true, true));

            Assert.Equal("screenName", ex.ParamName);
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
