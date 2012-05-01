using System;
using System.Collections.Generic;

using LinqToTwitter;

using LinqToTwitterTests.Common;

using Moq;

using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class BlocksExtensionsTests
    {
        public BlocksExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void CreateBlock_Handles_Response()
        {
            const string id = "1";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Blocks>>()))
                .Returns(BlocksUserJson);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.CreateBlock(id);

            Assert.Equal("LINQ to Tweeter Test", actual.Name);
        }

        [Fact]
        public void CreateBlock_Builds_URL()
        {
            const string id = "1";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Blocks>>()))
                .Returns(BlocksUserJson);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "https://api.twitter.com/1/", "");

            ctx.CreateBlock(id);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/blocks/create/1.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Blocks>>()),
                Times.Once());
        }

        [Fact]
        public void CreateBlock_Throws_On_Null_ID()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Blocks>>()))
                .Returns(BlocksUserJson);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            var ex = Assert.Throws<ArgumentException>(() => ctx.CreateBlock(null));

            Assert.Equal("id", ex.ParamName);
        }

        [Fact]
        public void DestroyBlock_Handles_Response()
        {
            const string id = "1";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Blocks>>()))
                .Returns(BlocksUserJson);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DestroyBlock(id);

            Assert.Equal("LINQ to Tweeter Test", actual.Name);
        }

        [Fact]
        public void DestroyBlock_Builds_URL()
        {
            const string id = "1";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Blocks>>()))
                .Returns(BlocksUserJson);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "https://api.twitter.com/1/", "");

            ctx.DestroyBlock(id);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/blocks/destroy/1.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Blocks>>()),
                Times.Once());
        }

        [Fact]
        public void DestroyBlockNullIDTest()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Blocks>>()))
                .Returns(BlocksUserJson);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            var ex = Assert.Throws<ArgumentException>(() => ctx.DestroyBlock(null));

            Assert.Equal("id", ex.ParamName);
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
