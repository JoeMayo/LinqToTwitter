using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.FriendshipTests
{
    [TestClass]
    public class FriendshipRequestProcessorTests
    {
        public FriendshipRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void FriendshipRequestProcessor_Works_With_Json_Format_Data()
        {
            var freindReqProc = new FriendshipRequestProcessor<Friendship>();

            Assert.IsInstanceOfType(freindReqProc, typeof(IRequestProcessorWantsJson));
        }

        [TestMethod]
        public void ProcessResults_Translates_Relationships_From_LookupQuery()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = FriendshipType.Lookup };

            List<Friendship> friends = friendReqProc.ProcessResults(LookupResponse);

            Assert.IsNotNull(friends);
            Assert.IsNotNull(friends.SingleOrDefault());
            var relations = friends.Single().Relationships;
            Assert.IsNotNull(relations);
            Assert.IsNotNull(relations.SingleOrDefault());
            var connections = relations.Single().Connections;
            Assert.IsNotNull(connections);
            Assert.IsNotNull(connections.SingleOrDefault());
            var connection = connections.First();
            Assert.IsNotNull(connection);
            Assert.AreEqual("following", connection);
        }

        [TestMethod]
        public void ProcessResults_Parses_Show_Response()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = FriendshipType.Show };

            List<Friendship> friends = friendReqProc.ProcessResults(RelationshipResponse);

            Assert.IsNotNull(friends);
            Assert.IsNotNull(friends.SingleOrDefault());
            var friend = friends.Single();
            Assert.IsNotNull(friend);
            var tgtRel = friend.TargetRelationship;
            Assert.IsNotNull(tgtRel);
            Assert.AreEqual("JoeMayo", tgtRel.ScreenName);
            Assert.IsTrue(tgtRel.FollowedBy);
            Assert.AreEqual(15411837ul, tgtRel.ID);
            Assert.IsFalse(tgtRel.Following);
            var srcRel = friend.SourceRelationship;
            Assert.IsNotNull(srcRel);
            Assert.AreEqual("Linq2Tweeter", srcRel.ScreenName);
            Assert.IsTrue(srcRel.RetweetsWanted);
            Assert.IsFalse(srcRel.AllReplies);
            Assert.IsFalse(srcRel.MarkedSpam);
            Assert.IsFalse(srcRel.FollowedBy);
            Assert.AreEqual(16761255ul, srcRel.ID);
            Assert.IsFalse((bool)srcRel.Blocking);
            Assert.IsTrue((bool)srcRel.NotificationsEnabled);
            Assert.IsTrue(srcRel.Following);
            Assert.IsFalse(srcRel.CanDm);
        }

        void TestParsingIds(FriendshipType friendType)
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = friendType };

            List<Friendship> friends = friendReqProc.ProcessResults(IdsResponse);

            Assert.IsNotNull(friends);
            Assert.IsNotNull(friends.SingleOrDefault());
            var friend = friends.Single();
            Assert.IsNotNull(friend);
            var idList = friend.IDInfo;
            Assert.IsNotNull(idList);
            var cursor = idList.CursorMovement;
            Assert.IsNotNull(cursor);
            Assert.AreEqual(2, cursor.Previous);
            Assert.AreEqual(1, cursor.Next);
            var ids = idList.IDs;
            Assert.IsNotNull(ids);
            Assert.IsTrue(ids.Any());
            var id = ids.First();
            Assert.AreEqual(5676142ul, id);
        }

        [TestMethod]
        public void ProcessResults_Parses_Incoming_Response()
        {
            TestParsingIds(FriendshipType.Incoming);
        }

        [TestMethod]
        public void ProcessResults_Parses_Outgoing_Response()
        {
            TestParsingIds(FriendshipType.Outgoing);
        }

        [TestMethod]
        public void ProcessResults_Parses_NoRetweetIDs_Response()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = FriendshipType.NoRetweetIDs };

            List<Friendship> friends = friendReqProc.ProcessResults(NoRetweetIDsResponse);

            Assert.IsNotNull(friends);
            Assert.IsNotNull(friends.SingleOrDefault());
            var friend = friends.Single();
            Assert.IsNotNull(friend);
            var idList = friend.IDInfo;
            Assert.IsNotNull(idList);
            var ids = idList.IDs;
            Assert.IsNotNull(ids);
            Assert.IsTrue(ids.Any());
            var id = ids.First();
            Assert.AreEqual(15411837ul, id);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://api.twitter.com/1.1/" };

            List<Friendship> friendships = reqProc.ProcessResults(string.Empty);

            Assert.IsFalse(friendships.Any());
        }

        [TestMethod]
        public void ProcessResults_Parses_FriendsListOrFollowersList_Response()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = FriendshipType.FriendsList };

            List<Friendship> friendshipList = friendReqProc.ProcessResults(FriendsFollowersResponse);

            Assert.IsNotNull(friendshipList);
            Assert.IsNotNull(friendshipList.SingleOrDefault());
            var friendship = friendshipList.Single();
            Assert.IsNotNull(friendship);
            var users = friendship.Users;
            Assert.IsNotNull(users);
            Assert.AreEqual(3, users.Count);
            var user = users.First();
            Assert.IsNotNull(user);
            Assert.AreEqual("93604443", user.UserIDResponse);
         }

        [TestMethod]
        public void ProcessResults_Retains_Original_Input_Params()
        {
            const ulong SrcUsrID = 1;
            const string SrcScrNm = "JoeMayo";
            const ulong TgtUsrID = 2;
            const string TgtScrNm = "MayMayo";
            const long Cursor = 123;
            const string ScrNm = "JoeMayo,MayMayo";
            const string UsrID = "1,2";
            const bool SkipStatus = true;
            const bool IncludeUserEntities = true;
            var friendReqProc = new FriendshipRequestProcessor<Friendship> 
            { 
                Type = FriendshipType.Lookup,
                SourceUserID = SrcUsrID,
                SourceScreenName = SrcScrNm,
                TargetUserID = TgtUsrID,
                TargetScreenName = TgtScrNm,
                Cursor = Cursor,
                ScreenName = ScrNm,
                UserID = UsrID,
                SkipStatus = SkipStatus,
                IncludeUserEntities = IncludeUserEntities
            };

            List<Friendship> friendships = friendReqProc.ProcessResults(LookupResponse);

            Assert.IsNotNull(friendships);
            Assert.IsTrue(friendships.Any());
            var friendship = friendships.First();
            Assert.IsNotNull(friendship);
            Assert.AreEqual(FriendshipType.Lookup, friendship.Type);
            Assert.AreEqual(SrcUsrID, friendship.SourceUserID);
            Assert.AreEqual(SrcScrNm, friendship.SourceScreenName);
            Assert.AreEqual(TgtUsrID, friendship.TargetUserID);
            Assert.AreEqual(TgtScrNm, friendship.TargetScreenName);
            Assert.AreEqual(Cursor, friendship.Cursor);
            Assert.AreEqual(ScrNm, friendship.ScreenName);
            Assert.AreEqual(UsrID, friendship.UserID);
            Assert.AreEqual(SkipStatus, friendship.SkipStatus);
            Assert.AreEqual(IncludeUserEntities, friendship.IncludeUserEntities);
        }

        [TestMethod]
        public void ProcessActionResult_Translates_Relationships_From_UpdateSettings()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>();

            Friendship friend = friendReqProc.ProcessActionResult(RelationshipResponse, FriendshipAction.Update);

            var srcRel = friend.SourceRelationship;
            Assert.AreEqual(true, srcRel.RetweetsWanted);
            Assert.AreEqual(true, srcRel.NotificationsEnabled);
        }

        [TestMethod]
        public void GetParameters_Returns_Parameters()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            Expression<Func<Friendship, bool>> expression =
                friend =>
                         friend.Type == FriendshipType.Show &&
                         friend.SourceUserID == 1 &&
                         friend.SourceScreenName == "Name" &&
                         friend.TargetUserID == 2 &&
                         friend.TargetScreenName == "Name" &&
                         friend.Cursor == -1 &&
                         friend.ScreenName == "twitter,joemayo" &&
                         friend.UserID == "123,456" &&
                         friend.SkipStatus == true &&
                         friend.IncludeUserEntities == true &&
                         friend.Count == 7;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = friendReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)FriendshipType.Show).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SourceUserID", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SourceScreenName", "Name")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("TargetUserID", "2")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("TargetScreenName", "Name")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "-1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "twitter,joemayo")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("UserID", "123,456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SkipStatus", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeUserEntities", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Count", "7")));
        }

        [TestMethod]
        public void BuildUrl_Constructs_Show_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "SourceUserID", "123" },
                    { "SourceScreenName", "JoeMayo" },
                    { "TargetUserID", "456" },
                    { "TargetScreenName", "LinqToTweeter" }
                };
            string expected = "https://api.twitter.com/1.1/friendships/show.json?source_id=123&source_screen_name=JoeMayo&target_id=456&target_screen_name=LinqToTweeter";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        [Ignore]
        public void BuildUrl_Throws_Without_Source_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "friendReqProcUserID", "456" },
                    { "friendReqProcScreenName", "LinqToTweeter" }
                };

            //Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));
        }

        [TestMethod]
        [Ignore]
        public void BuildUrl_Throws_Without_FriendsReqProcUrl()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.Show.ToString() },
                { "SourceUserID", "123" },
                { "SourceScreenName", "JoeMayo" },
            };

            //Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));
        }

        [TestMethod]
        public void BuildUrl_Creates_No_Retweet_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.NoRetweetIDs.ToString() }
            };
            string expected = "http://api.twitter.com/1.1/friendships/no_retweets/ids.json";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        [Ignore]
        public void BuildUrl_Throws_When_Missing_Type()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string> { };

            //var ex = Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));

            //Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        [Ignore]
        public void BuildUrl_Throws_On_Null_Parameters()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            Dictionary<string, string> parameters = null;

            //var ex = Assert.Throws<ArgumentException>(() => friendReqProc.BuildUrl(parameters));

            //Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Incoming_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.Incoming.ToString() }
            };
            string expected = "https://api.twitter.com/1.1/friendships/incoming.json";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_With_Cursor_Param_Constructs_Incoming_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.Incoming.ToString() },
                { "Cursor", "-1" }
            };
            string expected = "https://api.twitter.com/1.1/friendships/incoming.json?cursor=-1";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Outgoing_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.Outgoing.ToString() }
            };
            string expected = "https://api.twitter.com/1.1/friendships/outgoing.json";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_With_Cursor_Param_Constructs_Outgoing_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.Outgoing.ToString() },
                { "Cursor", "-1" }
            };
            string expected = "https://api.twitter.com/1.1/friendships/outgoing.json?cursor=-1";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildLookupUrl_Returns_Url_For_ScreenName()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.Lookup.ToString() },
                { "ScreenName", "twitter,joemayo" }
            };
            string expected = "https://api.twitter.com/1.1/friendships/lookup.json?screen_name=twitter%2Cjoemayo";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildLookupUrl_Returns_Url_For_UserID()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.Lookup.ToString() },
                { "UserID", "123,456" }
            };
            string expected = "https://api.twitter.com/1.1/friendships/lookup.json?user_id=123%2C456";

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        [Ignore]
        public void BuildLookupUrl_Requires_ScreenName_Or_UserID()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.Lookup.ToString() },
                //{ "ScreenName", "twitter,joemayo" }
            };

            //var ex = Assert.Throws<ArgumentNullException>(() => friendReqProc.BuildUrl(parameters));
                 
            //Assert.AreEqual("ScreenNameOrUserID", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_FriendsList_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/friends/list.json?user_id=1&screen_name=abc&cursor=234&skip_status=true&include_user_entities=false";
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.FriendsList.ToString() },
                { "UserID", "1" },
                { "ScreenName", "abc" },
                { "Cursor", "234" },
                { "SkipStatus", true.ToString() },
                { "IncludeUserEntities", false.ToString() }
            };

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        [Ignore]
        public void BuildFriendListUrl_Requires_ScreenName_Or_UserID()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.FriendsList.ToString() },
                //{ "UserID", "1" },
                //{ "ScreenName", "abc" }
            };

            //var ex = Assert.Throws<ArgumentNullException>(() => friendReqProc.BuildUrl(parameters));

            //Assert.AreEqual("ScreenNameOrUserID", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_FollowersList_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/followers/list.json?user_id=1&screen_name=abc&cursor=234&skip_status=true&include_user_entities=false";
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.FollowersList.ToString() },
                { "UserID", "1" },
                { "ScreenName", "abc" },
                { "Cursor", "234" },
                { "SkipStatus", true.ToString() },
                { "IncludeUserEntities", false.ToString() }
            };

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        [Ignore]
        public void BuildFollowerListUrl_Requires_ScreenName_Or_UserID()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.FriendsList.ToString() },
                //{ "UserID", "1" },
                //{ "ScreenName", "abc" }
            };

            //var ex = Assert.Throws<ArgumentNullException>(() => friendReqProc.BuildUrl(parameters));

            //Assert.AreEqual("ScreenNameOrUserID", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_FollowerIDs_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/followers/ids.json?user_id=123&screen_name=456&cursor=1&count=1";
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
                    {
                        { "Type", ((int)FriendshipType.FollowerIDs).ToString() },
                        { "UserID", "123" },
                        { "ScreenName", "456" },
                        { "Cursor", "1" },
                        { "Count", "1" }
                    };

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_FriendIDs_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/friends/ids.json?user_id=123&screen_name=456&cursor=1&count=1";
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
                    {
                        { "Type", ((int)FriendshipType.FriendIDs).ToString() },
                        { "UserID", "123" },
                        { "ScreenName", "456" },
                        { "Cursor", "1" },
                        { "Count", "1" }
                    };

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Defaults_Cursor_When_Not_Specified()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/friends/ids.json?screen_name=JoeMayo&cursor=-1";
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)FriendshipType.FriendIDs).ToString() },
                        { "ScreenName", "JoeMayo" },
                    };

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

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
   ""previous_cursor"":2,
   ""next_cursor_str"":""1"",
   ""ids"":[
      5676142
   ],
   ""previous_cursor_str"":""2"",
   ""next_cursor"":1
}";

        const string NoRetweetIDsResponse = @"[
15411837,
241594327
]";

        const string FriendsFollowersResponse = @"{
   ""users"":[
      {
         ""id"":93604443,
         ""id_str"":""93604443"",
         ""name"":""Geert van Horrik"",
         ""screen_name"":""GeertvanHorrik"",
         ""location"":""Heeze, The Netherlands"",
         ""description"":""Software Developer and creator of the Catel MVVM framework (see http:\/\/catel.codeplex.com)"",
         ""url"":""http:\/\/blog.catenalogic.com"",
         ""entities"":{
            ""url"":{
               ""urls"":[
                  {
                     ""url"":""http:\/\/blog.catenalogic.com"",
                     ""expanded_url"":null,
                     ""indices"":[
                        0,
                        27
                     ]
                  }
               ]
            },
            ""description"":{
               ""urls"":[

               ]
            }
         },
         ""protected"":false,
         ""followers_count"":212,
         ""friends_count"":97,
         ""listed_count"":12,
         ""created_at"":""Mon Nov 30 10:32:19 +0000 2009"",
         ""favourites_count"":5,
         ""utc_offset"":3600,
         ""time_zone"":""Amsterdam"",
         ""geo_enabled"":true,
         ""verified"":false,
         ""statuses_count"":4431,
         ""lang"":""en"",
         ""status"":{
            ""created_at"":""Tue Dec 18 11:10:16 +0000 2012"",
            ""id"":280993376646598658,
            ""id_str"":""280993376646598658"",
            ""text"":""@FransBouma Dank, ik zocht op changes maar moest queue hebben, kan ik weer verder :-)"",
            ""source"":""\u003ca href=\""http:\/\/www.metrotwit.com\/\"" rel=\""nofollow\""\u003eMetroTwit\u003c\/a\u003e"",
            ""truncated"":false,
            ""in_reply_to_status_id"":280993183754768384,
            ""in_reply_to_status_id_str"":""280993183754768384"",
            ""in_reply_to_user_id"":67035608,
            ""in_reply_to_user_id_str"":""67035608"",
            ""in_reply_to_screen_name"":""FransBouma"",
            ""geo"":null,
            ""coordinates"":null,
            ""place"":null,
            ""contributors"":null,
            ""retweet_count"":0,
            ""entities"":{
               ""hashtags"":[

               ],
               ""urls"":[

               ],
               ""user_mentions"":[
                  {
                     ""screen_name"":""FransBouma"",
                     ""name"":""Frans Bouma"",
                     ""id"":67035608,
                     ""id_str"":""67035608"",
                     ""indices"":[
                        0,
                        11
                     ]
                  }
               ]
            },
            ""favorited"":false,
            ""retweeted"":false
         },
         ""contributors_enabled"":false,
         ""is_translator"":false,
         ""profile_background_color"":""C0DEED"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_background_tile"":false,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1420487736\/Avatar_3_small_normal.png"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1420487736\/Avatar_3_small_normal.png"",
         ""profile_link_color"":""0084B4"",
         ""profile_sidebar_border_color"":""C0DEED"",
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""profile_text_color"":""333333"",
         ""profile_use_background_image"":true,
         ""default_profile"":true,
         ""default_profile_image"":false,
         ""following"":false,
         ""follow_request_sent"":false,
         ""notifications"":false
      },
      {
         ""id"":389487039,
         ""id_str"":""389487039"",
         ""name"":""Elizabeth Hulett"",
         ""screen_name"":""EHulett"",
         ""location"":""Defuniak Springs"",
         ""description"":"""",
         ""url"":null,
         ""entities"":{
            ""description"":{
               ""urls"":[

               ]
            }
         },
         ""protected"":false,
         ""followers_count"":4,
         ""friends_count"":10,
         ""listed_count"":0,
         ""created_at"":""Wed Oct 12 14:16:54 +0000 2011"",
         ""favourites_count"":1,
         ""utc_offset"":null,
         ""time_zone"":null,
         ""geo_enabled"":false,
         ""verified"":false,
         ""statuses_count"":3,
         ""lang"":""en"",
         ""status"":{
            ""created_at"":""Mon Dec 17 19:47:23 +0000 2012"",
            ""id"":280761124901765120,
            ""id_str"":""280761124901765120"",
            ""text"":""Can't believe Amanda is 28weeks today.... Lily Rose will be here soon :)"",
            ""source"":""\u003ca href=\""http:\/\/twitter.com\/#!\/download\/ipad\"" rel=\""nofollow\""\u003eTwitter for iPad\u003c\/a\u003e"",
            ""truncated"":false,
            ""in_reply_to_status_id"":null,
            ""in_reply_to_status_id_str"":null,
            ""in_reply_to_user_id"":null,
            ""in_reply_to_user_id_str"":null,
            ""in_reply_to_screen_name"":null,
            ""geo"":null,
            ""coordinates"":null,
            ""place"":null,
            ""contributors"":null,
            ""retweet_count"":0,
            ""entities"":{
               ""hashtags"":[

               ],
               ""urls"":[

               ],
               ""user_mentions"":[

               ]
            },
            ""favorited"":false,
            ""retweeted"":false
         },
         ""contributors_enabled"":false,
         ""is_translator"":false,
         ""profile_background_color"":""C0DEED"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
         ""profile_background_tile"":false,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1584881066\/Callie_025_normal.JPG"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1584881066\/Callie_025_normal.JPG"",
         ""profile_link_color"":""0084B4"",
         ""profile_sidebar_border_color"":""C0DEED"",
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""profile_text_color"":""333333"",
         ""profile_use_background_image"":true,
         ""default_profile"":true,
         ""default_profile_image"":false,
         ""following"":false,
         ""follow_request_sent"":false,
         ""notifications"":false
      },
      {
         ""id"":608455311,
         ""id_str"":""608455311"",
         ""name"":""\/\/ mellodev"",
         ""screen_name"":""mellodev"",
         ""location"":""Las Vegas, NV"",
         ""description"":"".Net aficionado and Software craftsman. C#, Asp.Net, TSQL, Quartz.Net, WebForms, MVC + WP7. I build Social Office Suite (@SOS) and tweet about #DevLife"",
         ""url"":""http:\/\/stackoverflow.com\/users\/531901\/mello702"",
         ""entities"":{
            ""url"":{
               ""urls"":[
                  {
                     ""url"":""http:\/\/stackoverflow.com\/users\/531901\/mello702"",
                     ""expanded_url"":null,
                     ""indices"":[
                        0,
                        46
                     ]
                  }
               ]
            },
            ""description"":{
               ""urls"":[

               ]
            }
         },
         ""protected"":false,
         ""followers_count"":355,
         ""friends_count"":1373,
         ""listed_count"":20,
         ""created_at"":""Thu Jun 14 18:32:56 +0000 2012"",
         ""favourites_count"":109,
         ""utc_offset"":-28800,
         ""time_zone"":""Pacific Time (US & Canada)"",
         ""geo_enabled"":true,
         ""verified"":false,
         ""statuses_count"":1272,
         ""lang"":""en"",
         ""status"":{
            ""created_at"":""Tue Dec 18 21:05:14 +0000 2012"",
            ""id"":281143104046768129,
            ""id_str"":""281143104046768129"",
            ""text"":""@adamralph Nice background btw! Can't wait to go riding this year!"",
            ""source"":""web"",
            ""truncated"":false,
            ""in_reply_to_status_id"":281141527877652480,
            ""in_reply_to_status_id_str"":""281141527877652480"",
            ""in_reply_to_user_id"":5966642,
            ""in_reply_to_user_id_str"":""5966642"",
            ""in_reply_to_screen_name"":""adamralph"",
            ""geo"":null,
            ""coordinates"":null,
            ""place"":null,
            ""contributors"":null,
            ""retweet_count"":0,
            ""entities"":{
               ""hashtags"":[

               ],
               ""urls"":[

               ],
               ""user_mentions"":[
                  {
                     ""screen_name"":""adamralph"",
                     ""name"":""Adam Ralph"",
                     ""id"":5966642,
                     ""id_str"":""5966642"",
                     ""indices"":[
                        0,
                        10
                     ]
                  }
               ]
            },
            ""favorited"":false,
            ""retweeted"":false
         },
         ""contributors_enabled"":false,
         ""is_translator"":false,
         ""profile_background_color"":""191134"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/591395288\/3jgrymfj3hmiokbf3n2i.jpeg"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/591395288\/3jgrymfj3hmiokbf3n2i.jpeg"",
         ""profile_background_tile"":false,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2357024693\/ad1730c5-ad81-4485-af3b-1cb03a7ffacc_normal.png"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2357024693\/ad1730c5-ad81-4485-af3b-1cb03a7ffacc_normal.png"",
         ""profile_banner_url"":""https:\/\/si0.twimg.com\/profile_banners\/608455311\/1348679711"",
         ""profile_link_color"":""0084B4"",
         ""profile_sidebar_border_color"":""FFFFFF"",
         ""profile_sidebar_fill_color"":""DDEEF6"",
         ""profile_text_color"":""333333"",
         ""profile_use_background_image"":true,
         ""default_profile"":false,
         ""default_profile_image"":false,
         ""following"":false,
         ""follow_request_sent"":false,
         ""notifications"":false
      }
   ],
   ""next_cursor"":2,
   ""next_cursor_str"":""1"",
   ""previous_cursor"":0,
   ""previous_cursor_str"":""0""
}";
    }
}
