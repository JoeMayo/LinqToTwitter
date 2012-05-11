using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.ListTests
{
    public class ListExtensionsTests
    {
        TwitterContext ctx;
        Mock<ITwitterExecute> execMock;

        public ListExtensionsTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void ListRequestProcessor_Handles_Actions()
        {
            var listReqProc = new ListRequestProcessor<List>();

            Assert.IsAssignableFrom<IRequestProcessorWithAction<List>>(listReqProc);
        }

        void InitializeTwitterContext()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => 
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(TestStatusQueryResponse);
            ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
        }

        [Fact]
        public void CreateList_Requires_ListName()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.CreateList(null, "public", "desc"));

            Assert.Equal("listName", ex.ParamName);
        }

        [Fact]
        public void CreateList_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "name", "test" },
                { "mode", "public" },
                { "description", "desc" }
            };

            ctx.CreateList("test", "public", "desc");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/create.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void UpdateList_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateList(null, null, null, null, null, null));

            Assert.Equal("ListIdOrSlug", ex.ParamName);
        }

        [Fact]
        public void UpdateList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateList("123", "test", null, null, null, null));

            Assert.Equal("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [Fact]
        public void UpdateList_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" },
                { "mode", "public" },
                { "description", "desc" }
            };

            ctx.UpdateList("123", "test", "456", "JoeMayo", "public", "desc");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/update.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void DeleteList_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.DeleteList(null, null, null, null));

            Assert.Equal("ListIdOrSlug", ex.ParamName);
        }

        [Fact]
        public void DeleteList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.DeleteList("123", "test", null, null));

            Assert.Equal("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [Fact]
        public void DeleteList_Works_With_Slug_And_OwnerID()
        {
            InitializeTwitterContext();

            ctx.DeleteList(null, "test", "456", null);
        }

        [Fact]
        public void DeleteList_Works_With_ListID_Only()
        {
            InitializeTwitterContext();

            ctx.DeleteList("123", null, null, null);
        }

        [Fact]
        public void DeleteList_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.DeleteList("123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/destroy.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void AddMemberToList_Requires_UserID_Or_ScreenName()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberToList(null, null, null, null, null, null));

            Assert.Equal("UserIdOrScreenName", ex.ParamName);
        }

        [Fact]
        public void AddMemberToList_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberToList(null, "JoeMayo", null, null, null, null));

            Assert.Equal("ListIdOrSlug", ex.ParamName);
        }

        [Fact]
        public void AddMemberToList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberToList(null, "JoeMayo", null, "linq", null, null));

            Assert.Equal("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [Fact]
        public void AddMemberToList_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "user_id", "789" },
                { "screen_name", "JoeMayo" },
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.AddMemberToList("789", "JoeMayo", "123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/create.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void AddMemberRangeToList_For_ScreenNames_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberRangeToList(null, null, null, null, new List<string> { "SomeName" }));

            Assert.Equal("ListIdOrSlug", ex.ParamName);
        }

        [Fact]
        public void AddMemberRangeToList_For_ScreenNames_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberRangeToList(null, "test", null, null, new List<string> { "SomeOne" }));

            Assert.Equal("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [Fact]
        public void AddMemberRangeToList_For_ScreenNames_Requires_ScreenNames()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberRangeToList(null, "test", "123", null, (List<string>)null));

            Assert.Equal("screenNames", ex.ParamName);
        }

        [Fact]
        public void AddMemberRangeToList_For_ScreenNames_Requires_ScreenNames_With_Values()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberRangeToList(null, "test", "123", null, new List<string>()));

            Assert.Equal("screenNames", ex.ParamName);
        }

        [Fact]
        public void AddMemberRangeToList_For_ScreenNames_Requires_ScreenNames_Count_LessThanOrEqualTo_100()
        {
            InitializeTwitterContext();
            var screenNames = Enumerable.Range(1, 101).Select(item => item.ToString(CultureInfo.InvariantCulture)).ToList();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberRangeToList(null, "test", "123", null, screenNames));

            Assert.Equal("screenNames", ex.ParamName);
        }

        [Fact]
        public void AddMemberRangeToList_For_ScreenNames_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" },
                { "user_id", null },
                { "screen_name", "JoeMayo,Linq2Tweeter,SomeOneElse" },
            };
            var screenNames = new List<string> { "JoeMayo", "Linq2Tweeter", "SomeOneElse" };

            ctx.AddMemberRangeToList("123", "test", "456", "JoeMayo", screenNames);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/create_all.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void AddMemberRangeToList_For_UserIDs_Requires_UserIDs()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberRangeToList(null, "test", "123", null, (List<ulong>)null));

            Assert.Equal("userIDs", ex.ParamName);
        }

        [Fact]
        public void AddMemberRangeToList_For_UserIDs_Requires_UserIDs_With_Values()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberRangeToList(null, "test", "123", null, new List<ulong>()));

            Assert.Equal("userIDs", ex.ParamName);
        }

        [Fact]
        public void AddMemberRangeToList_For_UserIDs_Requires_UserIDs_Count_LessThanOrEqualTo_100()
        {
            InitializeTwitterContext();
            var userIDs = Enumerable.Range(1, 101).Select(item => (ulong)item).ToList();

            var ex = Assert.Throws<ArgumentException>(() => ctx.AddMemberRangeToList(null, "test", "123", null, userIDs));

            Assert.Equal("userIDs", ex.ParamName);
        }

        [Fact]
        public void AddMemberRangeToList_For_UserIDs_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" },
                { "user_id", "123,234,345" },
                { "screen_name", null },
            };
            var userIDs = new List<ulong> { 123ul, 234ul, 345ul };

            ctx.AddMemberRangeToList("123", "test", "456", "JoeMayo", userIDs);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/create_all.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void DeleteMemberFromList_Requires_UserID_Or_ScreenName()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.DeleteMemberFromList(null, null, null, null, null, null));

            Assert.Equal("UserIdOrScreenName", ex.ParamName);
        }

        [Fact]
        public void DeleteMemberFromList_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.DeleteMemberFromList(null, "JoeMayo", null, null, null, null));

            Assert.Equal("ListIdOrSlug", ex.ParamName);
        }

        [Fact]
        public void DeleteMemberFromList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.DeleteMemberFromList(null, "JoeMayo", null, "linq", null, null));

            Assert.Equal("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [Fact]
        public void DeleteMemberFromList_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "user_id", "789" },
                { "screen_name", "JoeMayo" },
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.DeleteMemberFromList("789", "JoeMayo", "123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/destroy.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void SubscribeToList_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.SubscribeToList(null, null, null, null));

            Assert.Equal("ListIdOrSlug", ex.ParamName);
        }

        [Fact]
        public void SubscribeToList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.SubscribeToList(null, "linq", null, null));

            Assert.Equal("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [Fact]
        public void SubscribeToList_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.SubscribeToList("123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/subscribers/create.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void UnsubscribeFromList_Requires_ListID_Or_Slug()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UnsubscribeFromList(null, null, null, null));

            Assert.Equal("ListIdOrSlug", ex.ParamName);
        }

        [Fact]
        public void UnsubscribeFromList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UnsubscribeFromList(null, "linq", null, null));

            Assert.Equal("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [Fact]
        public void UnsubscribeFromList_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.UnsubscribeFromList("123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/subscribers/destroy.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void DestroyAllFromList_Invokes_Executor_Execute()
        {
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "user_id", "456" },
                { "screen_name", "JoeMayo" },
                { "owner_id", "789" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.DestroyAllFromList("123", "test", "456", "JoeMayo", "789", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/destroy_all.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void DestroyAllFromList_Requires_Either_ListID_Or_Slug()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(
                () => ctx.DestroyAllFromList(null, null, "1,2,3", null, null, null));

            Assert.Equal("ListIdOrSlug", ex.ParamName);
        }

        [Fact]
        public void DestroyAllFromList_Requires_OwnerID_Or_OwnerScreenName_If_Using_Slug()
        {
            InitializeTwitterContext();

            var ex = Assert.Throws<ArgumentException>(
                () => ctx.DestroyAllFromList(null, "slug", "1,2,3", null, null, null));

            Assert.Equal("OwnerIdOrOwnerScreenName", ex.ParamName);
        }

        [Fact]
        public void DestroyAllFromList_Accepts_Missing_OwnerID_And_OwnerScreenName_If_Using_ListID()
        {
            InitializeTwitterContext();

            ctx.DestroyAllFromList("1", "slug", "1,2,3", null, null, null);
        }

        [Fact]
        public void DestroyAllFromList_Handles_Null_UserIDs()
        {
            InitializeTwitterContext();

            ctx.DestroyAllFromList("1", "slug", null, "", null, null);
        }

        [Fact]
        public void DestroyAllFromList_Handles_Null_ScreenNames()
        {
            InitializeTwitterContext();

            ctx.DestroyAllFromList("1", "slug", "", null, null, null);
        }

        [Fact]
        public void DestroyAllFromList_Removes_Spaces_From_ID_List()
        {
            const string ExpectedUserIDs = "456,789";
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "user_id", ExpectedUserIDs },
                { "screen_name", "JoeMayo" },
                { "owner_id", "789" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.DestroyAllFromList("123", "test", "456, 789", "JoeMayo", "789", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/destroy_all.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [Fact]
        public void DestroyAllFromList_Removes_Spaces_From_ScreenNames()
        {
            const string ExpectedScreenNames = "JoeMayo,Linq2Tweeter";
            InitializeTwitterContext();
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "user_id", "456" },
                { "screen_name", ExpectedScreenNames },
                { "owner_id", "789" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.DestroyAllFromList("123", "test", "456", "JoeMayo, Linq2Tweeter", "789", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/destroy_all.json",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        const string TestStatusQueryResponse = @"{
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":6194482,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1618873297\/iPhone_Pictures_524_normal.jpg"",
         ""url"":""http:\/\/techpreacher.corti.com"",
         ""created_at"":""Mon May 21 08:57:50 +0000 2007"",
         ""followers_count"":815,
         ""default_profile"":false,
         ""profile_background_color"":""a6cce6"",
         ""lang"":""en"",
         ""utc_offset"":3600,
         ""name"":""Sascha Corti"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/242394801\/TwitterBackground2.png"",
         ""location"":""47.580262,-122.135105"",
         ""profile_link_color"":""0084B4"",
         ""listed_count"":47,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":false,
         ""description"":""Developer evangelist for Microsoft in Switzerland. Focus on web 2.0 technologies, Windows Phone 7 development. Passionate gamer with a life."",
         ""profile_text_color"":""333333"",
         ""statuses_count"":4293,
         ""screen_name"":""TechPreacher"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1618873297\/iPhone_Pictures_524_normal.jpg"",
         ""time_zone"":""Bern"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/242394801\/TwitterBackground2.png"",
         ""friends_count"":517,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""C0DEED"",
         ""id_str"":""6194482"",
         ""geo_enabled"":true,
         ""favourites_count"":37,
         ""profile_background_tile"":false,
         ""notifications"":false,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":0,
      ""id_str"":""196286470443642880"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""annotations"":null,
      ""source"":""\u003Ca href=\""http:\/\/raptr.com\/\"" rel=\""nofollow\""\u003ERaptr\u003C\/a\u003E"",
      ""created_at"":""Sat Apr 28 17:15:16 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":196286470443642880,
      ""geo"":null,
      ""text"":""I unlocked the Get a cube achievement on Fez! http:\/\/t.co\/Hqhl5oix""
   }";
    }
}
