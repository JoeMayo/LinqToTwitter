using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.NotificationsTests
{
    public class NotificationsExtensionsTests
    {
        public NotificationsExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void EnableNotifications_Invokes_Executor_Execute()
        {
            const string ExpectedName = "Twitter API";
            const string Id = "1";
            const string UserID = "2";
            const string ScreenName = "JoeMayo";
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

            User actual = ctx.EnableNotifications(Id, UserID, ScreenName);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/notifications/follow/1.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()),
                Times.Once());
            Assert.Equal(ExpectedName, actual.Name);
        }

        [Fact]
        public void EnableNotifications_Throws_On_NoInput()
        {
            const string ExpectedParamName = "UserIdOrScreenName";
            string screenName = string.Empty;
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

            var ex = Assert.Throws<ArgumentException>(() => ctx.EnableNotifications(null, null, screenName));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void DisableNotifications_Invokes_Executor_Execute()
        {
            const string ExpectedName = "Twitter API";
            const string Id = "1";
            const string UserID = "2";
            const string ScreenName = "JoeMayo";
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

            User actual = ctx.DisableNotifications(Id, UserID, ScreenName);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/notifications/leave/1.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()),
                Times.Once());
            Assert.Equal(ExpectedName, actual.Name);
        }

        [Fact]
        public void DisableNotifications_Throws_On_NoInput()
        {
            const string ExpectedParamName = "UserIdOrScreenName";
            string id = string.Empty;
            string screenName = string.Empty;
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

            var ex = Assert.Throws<ArgumentException>(() => ctx.DisableNotifications(id, null, screenName));

            Assert.Equal(ExpectedParamName, ex.ParamName);
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
