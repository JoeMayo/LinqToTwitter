using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.FriendshipTests
{
    public class FriendshipRequestProcessorTests
    {
        public FriendshipRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void FriendshipRequestProcessor_Works_With_Json_Format_Data()
        {
            var freindReqProc = new FriendshipRequestProcessor<Friendship>();

            Assert.IsAssignableFrom<IRequestProcessorWantsJson>(freindReqProc);
        }

        [Fact]
        public void ProcessResults_Parses_Exists()
        {
            const bool ExpectedResponse = true;
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = FriendshipType.Exists };

            var friends = friendReqProc.ProcessResults(FrienshipExistsResponse);

            Assert.NotNull(friends);
            Assert.Single(friends);
            var friend = friends.Single();
            Assert.NotNull(friend);
            Assert.Equal(ExpectedResponse, friend.IsFriend);
        }

        [Fact]
        public void ProcessResults_Translates_Relationships_From_LookupQuery()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = FriendshipType.Lookup };

            List<Friendship> friends = friendReqProc.ProcessResults(LookupResponse);

            Assert.NotNull(friends);
            Assert.Single(friends);
            var relations = friends.Single().Relationships;
            Assert.NotNull(relations);
            Assert.Single(relations);
            var connections = relations.Single().Connections;
            Assert.NotNull(connections);
            Assert.Single(connections);
            var connection = connections.First();
            Assert.NotNull(connection);
            Assert.Equal("following", connection);
        }

        [Fact]
        public void ProcessResults_Parses_Show_Response()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = FriendshipType.Show };

            List<Friendship> friends = friendReqProc.ProcessResults(RelationshipResponse);

            Assert.NotNull(friends);
            Assert.Single(friends);
            var friend = friends.Single();
            Assert.NotNull(friend);
            var tgtRel = friend.TargetRelationship;
            Assert.NotNull(tgtRel);
            Assert.Equal("JoeMayo", tgtRel.ScreenName);
            Assert.True(tgtRel.FollowedBy);
            Assert.Equal("15411837", tgtRel.ID);
            Assert.False(tgtRel.Following);
            var srcRel = friend.SourceRelationship;
            Assert.NotNull(srcRel);
            Assert.Equal("Linq2Tweeter", srcRel.ScreenName);
            Assert.True(srcRel.RetweetsWanted);
            Assert.False(srcRel.AllReplies);
            Assert.False(srcRel.MarkedSpam);
            Assert.False(srcRel.FollowedBy);
            Assert.Equal("16761255", srcRel.ID);
            Assert.False((bool)srcRel.Blocking);
            Assert.True((bool)srcRel.NotificationsEnabled);
            Assert.True(srcRel.Following);
            Assert.False(srcRel.CanDm);
        }

        void TestParsingIds(FriendshipType friendType)
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = friendType };

            List<Friendship> friends = friendReqProc.ProcessResults(IdsResponse);

            Assert.NotNull(friends);
            Assert.Single(friends);
            var friend = friends.Single();
            Assert.NotNull(friend);
            var idList = friend.IDInfo;
            Assert.NotNull(idList);
            var cursor = idList.CursorMovement;
            Assert.NotNull(cursor);
            Assert.Equal("2", cursor.Previous);
            Assert.Equal("1", cursor.Next);
            var ids = idList.IDs;
            Assert.NotNull(ids);
            Assert.NotEmpty(ids);
            var id = ids.First();
            Assert.Equal(5676142ul, id);
        }

        [Fact]
        public void ProcessResults_Parses_Incoming_Response()
        {
            TestParsingIds(FriendshipType.Incoming);
        }

        [Fact]
        public void ProcessResults_Parses_Outgoing_Response()
        {
            TestParsingIds(FriendshipType.Outgoing);
        }

        [Fact]
        public void ProcessResults_Parses_NoRetweetIDs_Response()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = FriendshipType.NoRetweetIDs };

            List<Friendship> friends = friendReqProc.ProcessResults(NoRetweetIDsResponse);

            Assert.NotNull(friends);
            Assert.Single(friends);
            var friend = friends.Single();
            Assert.NotNull(friend);
            var idList = friend.IDInfo;
            Assert.NotNull(idList);
            var ids = idList.IDs;
            Assert.NotNull(ids);
            Assert.NotEmpty(ids);
            var id = ids.First();
            Assert.Equal(15411837ul, id);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://api.twitter.com/1/" };

            List<Friendship> friendships = reqProc.ProcessResults(string.Empty);

            Assert.Empty(friendships);
        }

        [Fact]
        public void ProcessResults_Retains_Original_Input_Params()
        {
            const string SubjUser = "Joe";
            const string FollUser = "May";
            const string SrcUsrID = "1";
            const string SrcScrNm = "JoeMayo";
            const string TgtUsrID = "2";
            const string TgtScrNm = "MayMayo";
            const string Cursor = "123";
            const string ScrNm = "JoeMayo,MayMayo";
            const string UsrID = "1,2";
            var friendReqProc = new FriendshipRequestProcessor<Friendship> 
            { 
                Type = FriendshipType.Lookup,
                SubjectUser = SubjUser,
                FollowingUser = FollUser,
                SourceUserID = SrcUsrID,
                SourceScreenName = SrcScrNm,
                TargetUserID = TgtUsrID,
                TargetScreenName = TgtScrNm,
                Cursor = Cursor,
                ScreenName = ScrNm,
                UserID = UsrID
            };

            List<Friendship> friendships = friendReqProc.ProcessResults(LookupResponse);

            Assert.NotNull(friendships);
            Assert.NotEmpty(friendships);
            var friendship = friendships.First();
            Assert.NotNull(friendship);
            Assert.Equal(FriendshipType.Lookup, friendship.Type);
            Assert.Equal(SubjUser, friendship.SubjectUser);
            Assert.Equal(FollUser, friendship.FollowingUser);
            Assert.Equal(SrcUsrID, friendship.SourceUserID);
            Assert.Equal(SrcScrNm, friendship.SourceScreenName);
            Assert.Equal(TgtUsrID, friendship.TargetUserID);
            Assert.Equal(TgtScrNm, friendship.TargetScreenName);
            Assert.Equal(Cursor, friendship.Cursor);
            Assert.Equal(ScrNm, friendship.ScreenName);
            Assert.Equal(UsrID, friendship.UserID);
        }

        [Fact]
        public void ProcessActionResult_Translates_Relationships_From_UpdateSettings()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>();

            Friendship friend = friendReqProc.ProcessActionResult(RelationshipResponse, FriendshipAction.Update);

            var srcRel = friend.SourceRelationship;
            Assert.Equal(true, srcRel.RetweetsWanted);
            Assert.Equal(true, srcRel.NotificationsEnabled);
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
                         friend.ScreenName == "twitter,joemayo" &&
                         friend.UserID == "123,456";
            var lambdaExpression = expression as LambdaExpression;

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
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("UserID", "123,456")));
        }

        [Fact]
        public void BuildUrl_Constructs_Exists_Url()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Exists.ToString() },
                    { "SubjectUser", "123" },
                    { "FollowingUser", "456" }
                };
            string expected = "https://api.twitter.com/1/friendships/exists.json?user_a=123&user_b=456";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_Show_Url()
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
            string expected = "https://api.twitter.com/1/friendships/show.json?source_id=123&source_screen_name=JoeMayo&target_id=456&target_screen_name=LinqToTweeter";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_Without_Source_Url()
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
        public void BuildUrl_Throws_Without_FriendsReqProcUrl()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
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
            string expected = "http://api.twitter.com/friendships/no_retweet_ids.json";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_When_Missing_Type()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = new Dictionary<string, string> { };

            var ex = Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_On_Null_Parameters()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = null;

            var ex = Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Constructs_Incoming_Url()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Incoming.ToString() }
                };
            string expected = "https://api.twitter.com/1/friendships/incoming.json";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_With_Cursor_Param_Constructs_Incoming_Url()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Incoming.ToString() },
                    { "Cursor", "-1" }
                };
            string expected = "https://api.twitter.com/1/friendships/incoming.json?cursor=-1";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_Outgoing_Url()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Outgoing.ToString() }
                };
            string expected = "https://api.twitter.com/1/friendships/outgoing.json";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_With_Cursor_Param_Constructs_Outgoing_Url()
        {
            FriendshipRequestProcessor<Friendship> friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Outgoing.ToString() },
                    { "Cursor", "-1" }
                };
            string expected = "https://api.twitter.com/1/friendships/outgoing.json?cursor=-1";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildLookupUrl_Returns_Url_For_ScreenName()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Lookup.ToString() },
                    { "ScreenName", "twitter,joemayo" }
                };
            string expected = "https://api.twitter.com/1/friendships/lookup.json?screen_name=twitter%2Cjoemayo";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildLookupUrl_Returns_Url_For_UserID()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Lookup.ToString() },
                    { "UserID", "123,456" }
                };
            string expected = "https://api.twitter.com/1/friendships/lookup.json?user_id=123%2C456";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildLookupUrl_Requires_ScreenName_Or_UserID()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Lookup.ToString() },
                    //{ "ScreenName", "twitter,joemayo" }
                };

            var ex = Assert.Throws<ArgumentNullException>(() => friendReqProc.BuildUrl(parameters));
                 
            Assert.Equal("ScreenNameOrUserID", ex.ParamName);
        }

        const string FrienshipExistsResponse = "true";

        const string LookupResponse = @"[
   {
      ""screen_name"":""JoeMayo"",
      ""name"":""Joe Mayo"",
      ""id_str"":""15411837"",
      ""connections"":[
         ""following""
      ],
      ""id"":15411837
   }
]";

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

        const string IdsResponse = @"{
   ""previous_cursor"":0,
   ""next_cursor_str"":""1"",
   ""ids"":[
      5676142
   ],
   ""previous_cursor_str"":""2"",
   ""next_cursor"":0
}";

        const string NoRetweetIDsResponse = @"[
15411837,
241594327
]";
    }
}
