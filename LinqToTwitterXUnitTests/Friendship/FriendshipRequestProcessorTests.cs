using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using LinqToTwitterTests.Common;
using LinqToTwitter;
using System.Collections;
using System.Linq.Expressions;
using Moq;

namespace LinqToTwitterXUnitTests
{
    public class FriendshipRequestProcessorTests
    {
        public FriendshipRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void ProcessResultsTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>();
            bool expected = true;

            IList actual = friendReqProc.ProcessResults(m_testQueryResponse);

            var isFriend = actual.Cast<Friendship>().First().IsFriend;
            Assert.Equal(expected, actual.Cast<Friendship>().First().IsFriend);
        }

        [Fact]
        public void ProcessResults_Translates_Relationships_From_LookupQuery()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>();

            List<Friendship> friends = friendReqProc.ProcessResults(m_testLookupResponse);

            var relations = friends.First().Relationships;
            Assert.Equal(2, relations.Count);
            Assert.Equal("none", relations[0].Connections[0]);
            Assert.Equal("following", relations[1].Connections[0]);
        }

        [Fact]
        public void ProcessResults_Translates_Relationships_From_UpdateSettings()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>();

            List<Friendship> friends = friendReqProc.ProcessResults(m_testUpdateResponse);

            var srcRel = friends.First().SourceRelationship;
            Assert.Equal(true, srcRel.RetweetsWanted);
            Assert.Equal(true, srcRel.NotificationsEnabled);
        }

        [Fact]
        public void ProcessResults_Translates_IDs_From_Response()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>();

            List<Friendship> friends = friendReqProc.ProcessResults(m_testNoRetweetIDsResponse);

            var ids = friends.First().IDInfo.IDs;
            Assert.Equal(2, ids.Count);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://api.twitter.com/1/" };

            var friendships = reqProc.ProcessResults(string.Empty);

            Assert.Equal(1, friendships.Count);
        }

        [Fact]
        public void GetParameters_Returns_Parameters()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Expression<Func<Friendship, bool>> expression =
                friend =>
                    friend.Type == FriendshipType.Exists &&
                    friend.SubjectUser == "123" &&
                    friend.FollowingUser == "456" &&
                    friend.SourceUserID == "1" &&
                    friend.SourceScreenName == "Name" &&
                    friend.TargetUserID == "2" &&
                    friend.TargetScreenName == "Name" &&
                    friend.Cursor == "-1" &&
                    friend.ScreenName == "twitter,joemayo";
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = friendReqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)FriendshipType.Exists).ToString())));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SubjectUser", "123")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("FollowingUser", "456")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SourceUserID", "1")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SourceScreenName", "Name")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("TargetUserID", "2")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("TargetScreenName", "Name")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "-1")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "twitter,joemayo")));
        }

        [Fact]
        public void BuildExistsURLTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Exists.ToString() },
                    { "SubjectUser", "123" },
                    { "FollowingUser", "456" }
                };
            string expected = "https://api.twitter.com/1/friendships/exists.xml?user_a=123&user_b=456";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildShowURLTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "SourceUserID", "123" },
                    { "SourceScreenName", "JoeMayo" },
                    { "TargetUserID", "456" },
                    { "TargetScreenName", "LinqToTweeter" }
                };
            string expected = "https://api.twitter.com/1/friendships/show.xml?source_id=123&source_screen_name=JoeMayo&target_id=456&target_screen_name=LinqToTweeter";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildShowWithoutSourceURLTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "friendReqProcUserID", "456" },
                    { "friendReqProcScreenName", "LinqToTweeter" }
                };

            Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));
        }

        [Fact]
        public void BuildShowWithoutfriendReqProcURLTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "SourceUserID", "123" },
                    { "SourceScreenName", "JoeMayo" },
                };

            Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));
        }

        [Fact]
        public void BuildUrl_Creates_No_Retweet_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://api.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.NoRetweetIDs.ToString() }
                };
            string expected = "http://api.twitter.com/friendships/no_retweet_ids.xml";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void MissingTypeTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = new Dictionary<string, string> { };

            var ex = Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void NullParametersTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = null;

            var ex = Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildIncomingURLTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Incoming.ToString() }
                };
            string expected = "https://api.twitter.com/1/friendships/incoming.xml";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildIncomingWithCursorURLTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Incoming.ToString() },
                    { "Cursor", "-1" }
                };
            string expected = "https://api.twitter.com/1/friendships/incoming.xml?cursor=-1";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildOutgoingURLTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Outgoing.ToString() }
                };
            string expected = "https://api.twitter.com/1/friendships/outgoing.xml";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildOutgoingWithCursorURLTest()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Outgoing.ToString() },
                    { "Cursor", "-1" }
                };
            string expected = "https://api.twitter.com/1/friendships/outgoing.xml?cursor=-1";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildLookupUrl_Returns_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Lookup.ToString() },
                    { "ScreenName", "twitter,joemayo" }
                };
            string expected = "https://api.twitter.com/1/friendships/lookup.xml?screen_name=twitter%2Cjoemayo";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildLookupUrl_Requires_ScreenName()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Lookup.ToString() },
                    //{ "ScreenName", "twitter,joemayo" }
                };

            var ex = Assert.Throws<ArgumentNullException>(() => friendReqProc.BuildUrl(parameters));
                
            Assert.Equal("ScreenName", ex.ParamName);
        }

        [Fact]
        public void UpdateFriendshipSettings_Calls_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.ExecuteTwitter(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<IRequestProcessor<Friendship>>())).Returns("<friends>true</friends>");
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "https://api.twitter.com/1/", "");
            var reqProc = new Mock<IRequestProcessor<User>>();

            ctx.UpdateFriendshipSettings("joemayo", true, true);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/friendships/update.xml",
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
            execMock.Setup(exec => exec.ExecuteTwitter(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<IRequestProcessor<Friendship>>())).Returns("<friends>true</friends>");
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "https://api.twitter.com/1/", "");
            var reqProc = new Mock<IRequestProcessor<User>>();

            var ex = Assert.Throws<ArgumentNullException>(() => ctx.UpdateFriendshipSettings(/*"joemayo"*/ null, true, true));

            Assert.Equal("screenName", ex.ParamName);
        }

        private string m_testQueryResponse = "<friends>true</friends>";

        private string m_testLookupResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<relationships>
    <relationship id=""783214"">
        <id>783214</id>
        <name>Twitter</name>
        <screen_name>twitter</screen_name>
        <connections>
            <connection>none</connection>
        </connections>
    </relationship>
        <relationship id=""15411837"">
        <id>15411837</id>
        <name>Joe Mayo</name>
        <screen_name>JoeMayo</screen_name>
        <connections>
            <connection>following</connection>
        </connections>
    </relationship>
</relationships>";

        private string m_testUpdateResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<relationship>
  <friendReqProc>
    <followed_by type=""boolean"">true</followed_by>
    <screen_name>JoeMayo</screen_name>
    <following type=""boolean"">false</following>
    <id type=""integer"">15411837</id>
  </friendReqProc>
  <source>
    <marked_spam type=""boolean"">false</marked_spam>
    <followed_by type=""boolean"">false</followed_by>
    <notifications_enabled type=""boolean"">true</notifications_enabled>
    <screen_name>LinqToTweeter</screen_name>
    <following type=""boolean"">true</following>
    <all_replies type=""boolean"">false</all_replies>
    <want_retweets type=""boolean"">true</want_retweets>
    <blocking type=""boolean"">false</blocking>
    <id type=""integer"">16761255</id>
  </source>
</relationship>";

        private string m_testNoRetweetIDsResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ids>
<id>15411837</id>
<id>241594327</id>
</ids>";
    }
}
