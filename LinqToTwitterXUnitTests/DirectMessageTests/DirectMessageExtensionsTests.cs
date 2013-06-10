using System;
using System.Collections.Generic;

using LinqToTwitter;

using LinqToTwitterXUnitTests.Common;

using Moq;

using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class DirectMessageExtensionsTests
    {
        Mock<ITwitterAuthorizer> authMock;
        Mock<ITwitterExecute> execMock;

        public DirectMessageExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void DirectMessageRequestProcessor_Works_With_Actions()
        {
            var dmReqProc = new DirectMessageRequestProcessor<DirectMessage>();

            Assert.IsAssignableFrom<IRequestProcessorWithAction<DirectMessage>>(dmReqProc);
        }

        TwitterContext InitializeTwitterContext()
        {
            authMock = new Mock<ITwitterAuthorizer>();
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.PostToTwitter(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<Func<string, DirectMessage>>()))
                    .Returns(TestQueryResponse);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [Fact]
        public void NewDirectMessage_Returns_Created_DM()
        {
            const string UserID = "1";
            const string Text = "Hi";
            var ctx = InitializeTwitterContext();

            DirectMessage actual = ctx.NewDirectMessage(UserID, Text);

            Assert.Equal(189086715040903168ul, actual.IDResponse);
        }

        [Fact]
        public void NewDirectMessage_Constructs_Url()
        {
            const string UserID = "1";
            const string Text = "Hi";
            var ctx = InitializeTwitterContext();

            ctx.NewDirectMessage(UserID, Text);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    "https://api.twitter.com/1.1/direct_messages/new.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Func<string, DirectMessage>>()),
                Times.Once());
        }

        [Fact]
        public void NewDirectMessage_Throws_On_Null_Text()
        {
            const string UserID = "1";
            var ctx = InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.NewDirectMessage(UserID, null));

            Assert.Equal("text", ex.ParamName);
        }

        [Fact]
        public void NewDirectMessage_Throws_On_Empty_UserID()
        {
            string userID = string.Empty;
            const string Text = "Test Text";
            var ctx = InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.NewDirectMessage(userID, Text));

            Assert.Equal("user", ex.ParamName);
        }

        [Fact]
        public void DestroyDirectMessage_Returns_Deleted_DM()
        {
            const string Id = "1";
            var ctx = InitializeTwitterContext();

            DirectMessage actual = ctx.DestroyDirectMessage(Id, true);

            Assert.Equal(189086715040903168ul, actual.IDResponse);
        }

        [Fact]
        public void DestroyDirectMessage_Constructs_Url()
        {
            const string Id = "1";
            var ctx = InitializeTwitterContext();

            ctx.DestroyDirectMessage(Id, true);

            execMock.Verify(
                exec =>
                exec.PostToTwitter(
                    "https://api.twitter.com/1.1/direct_messages/destroy.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Func<string, DirectMessage>>()),
                Times.Once());
        }

        [Fact]
        public void DestroyDirectMessage_Throws_On_Null_ID()
        {
            var ctx = InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.DestroyDirectMessage(null, true));

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
