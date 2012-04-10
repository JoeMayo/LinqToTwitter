using System;
using System.Collections.Generic;

using LinqToTwitter;

using LinqToTwitterTests.Common;

using Moq;

using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class DirectMessageExtensionsTests
    {
        public DirectMessageExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void NewDirectMessage_Returns_Created_DM()
        {
            const string userID = "1";
            const string text = "Hi";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(TestQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            DirectMessage actual = ctx.NewDirectMessage(userID, text);

            Assert.Equal(189086715040903168ul, actual.ID);
        }

        [Fact]
        public void NewDirectMessage_Constructs_URL()
        {
            const string userID = "1";
            const string text = "Hi";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(TestQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            ctx.NewDirectMessage(userID, text);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/direct_messages/new.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()),
                Times.Once());
        }

        [Fact]
        public void NewDirectMessage_Throws_On_Null_Text()
        {
            const string userID = "1";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(TestQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            var ex = Assert.Throws<ArgumentException>(() => ctx.NewDirectMessage(userID, null));

            Assert.Equal("text", ex.ParamName);
        }

        [Fact]
        public void NewDirectMessage_Throws_On_Empty_UserID()
        {
            string userID = string.Empty;
            const string text = "Test Text";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(TestQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            var ex = Assert.Throws<ArgumentException>(() => ctx.NewDirectMessage(userID, text));

            Assert.Equal("user", ex.ParamName);
        }

        [Fact]
        public void NewDirectMessage_Sets_WrapLinks()
        {
            const bool wrapLinks = true;
            const string userID = "1";
            const string text = "Hi";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            bool wrapLinksPassedToExecute = false;
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Callback<string, IDictionary<string, string>, IRequestProcessor<DirectMessage>>(
                    (url, postData, reqProc) => wrapLinksPassedToExecute =
                        postData.ContainsKey("wrap_links") && bool.Parse(postData["wrap_links"]))
                .Returns(TestQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            ctx.NewDirectMessage(userID, text, wrapLinks);

            Assert.True(wrapLinksPassedToExecute);
        }

        [Fact]
        public void NewDirectMessage_Sets_WrapLinks_To_Null_When_False()
        {
            const bool wrapLinks = false;
            const string userID = "1";
            const string text = "Hi";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            bool wrapLinksIsSetToNull = false;
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Callback<string, IDictionary<string, string>, IRequestProcessor<DirectMessage>>(
                    (url, postData, reqProc) =>
                        wrapLinksIsSetToNull = postData["wrap_links"] == null)
                .Returns(TestQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            ctx.NewDirectMessage(userID, text, wrapLinks);

            Assert.True(wrapLinksIsSetToNull);
        }

        [Fact]
        public void DestroyDirectMessage_Returns_Deleted_DM()
        {
            const string id = "1";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(TestQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            DirectMessage actual = ctx.DestroyDirectMessage(id);

            Assert.Equal(189086715040903168ul, actual.ID);
        }

        [Fact]
        public void DestroyDirectMessage_Constructs_URL()
        {
            const string id = "1";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(TestQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            ctx.DestroyDirectMessage(id);

            execMock.Verify(
                exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/direct_messages/destroy/1.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()),
                Times.Once());
        }

        [Fact]
        public void DestroyDirectMessage_Throws_On_Null_ID()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(TestQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            var ex = Assert.Throws<ArgumentException>(() => ctx.DestroyDirectMessage(null));

            Assert.Equal("id", ex.ParamName);
        }

        const string TestQueryResponse = @"
   {
      ""recipient"":{
         ""id"":16761255,
         ""listed_count"":2,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""87bc44"",
         ""geo_enabled"":false,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/308329951\/linq2twitter_v3_300x90.png"",
         ""friends_count"":1,
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
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""screen_name"":""Linq2Tweeter""
      },
      ""created_at"":""Sun Apr 08 20:26:00 +0000 2012"",
      ""entities"":{
         ""user_mentions"":[

         ],
         ""urls"":[

         ],
         ""hashtags"":[

         ]
      },
      ""sender_id"":15411837,
      ""sender_screen_name"":""JoeMayo"",
      ""recipient_screen_name"":""Linq2Tweeter"",
      ""recipient_id"":16761255,
      ""id"":189086715040903168,
      ""sender"":{
         ""id"":15411837,
         ""listed_count"":108,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""5ED4DC"",
         ""geo_enabled"":true,
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""friends_count"":204,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""profile_background_tile"":false,
         ""followers_count"":1083,
         ""is_translator"":false,
         ""show_all_inline_media"":false,
         ""follow_request_sent"":false,
         ""statuses_count"":1867,
         ""utc_offset"":-25200,
         ""profile_sidebar_fill_color"":""95E8EC"",
         ""name"":""Joe Mayo"",
         ""default_profile_image"":false,
         ""protected"":false,
         ""profile_background_color"":""0099B9"",
         ""favourites_count"":39,
         ""lang"":""en"",
         ""url"":""http:\/\/linqtotwitter.codeplex.com\/"",
         ""verified"":false,
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP; interests: Windows 8."",
         ""profile_link_color"":""0099B9"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""default_profile"":false,
         ""following"":true,
         ""profile_use_background_image"":true,
         ""location"":""Denver, CO"",
         ""notifications"":false,
         ""id_str"":""15411837"",
         ""profile_text_color"":""3C3940"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""screen_name"":""JoeMayo""
      },
      ""text"":""This is a test."",
      ""id_str"":""189086715040903168""
   }";
    }
}
