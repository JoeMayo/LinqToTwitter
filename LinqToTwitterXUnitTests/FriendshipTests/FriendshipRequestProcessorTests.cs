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
        public void ProcessResults_Parses_FriendsListOrFollowersList_Response()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship> { Type = FriendshipType.FriendsList };

            List<Friendship> friendshipList = friendReqProc.ProcessResults(FreindsFollowersResponse);

            Assert.NotNull(friendshipList);
            Assert.Single(friendshipList);
            var friendship = friendshipList.Single();
            Assert.NotNull(friendship);
            var users = friendship.Users;
            Assert.NotNull(users);
            Assert.Equal(3, users.Count);
            var user = users.First();
            Assert.NotNull(user);
            Assert.Equal("93604443", user.Identifier.UserID);
         }

        [Fact]
        public void ProcessResults_Retains_Original_Input_Params()
        {
            const string SrcUsrID = "1";
            const string SrcScrNm = "JoeMayo";
            const string TgtUsrID = "2";
            const string TgtScrNm = "MayMayo";
            const string Cursor = "123";
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

            Assert.NotNull(friendships);
            Assert.NotEmpty(friendships);
            var friendship = friendships.First();
            Assert.NotNull(friendship);
            Assert.Equal(FriendshipType.Lookup, friendship.Type);
            Assert.Equal(SrcUsrID, friendship.SourceUserID);
            Assert.Equal(SrcScrNm, friendship.SourceScreenName);
            Assert.Equal(TgtUsrID, friendship.TargetUserID);
            Assert.Equal(TgtScrNm, friendship.TargetScreenName);
            Assert.Equal(Cursor, friendship.Cursor);
            Assert.Equal(ScrNm, friendship.ScreenName);
            Assert.Equal(UsrID, friendship.UserID);
            Assert.Equal(SkipStatus, friendship.SkipStatus);
            Assert.Equal(IncludeUserEntities, friendship.IncludeUserEntities);
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
                         friend.Type == FriendshipType.Show &&
                         friend.SourceUserID == "1" &&
                         friend.SourceScreenName == "Name" &&
                         friend.TargetUserID == "2" &&
                         friend.TargetScreenName == "Name" &&
                         friend.Cursor == "-1" &&
                         friend.ScreenName == "twitter,joemayo" &&
                         friend.UserID == "123,456" &&
                         friend.SkipStatus == true &&
                         friend.IncludeUserEntities == true;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = friendReqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)FriendshipType.Show).ToString())));
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
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SkipStatus", "True")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeUserEntities", "True")));
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

        [Fact]
        public void BuildUrl_Constructs_FriendsList_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/friends/list.json?user_id=1&screen_name=abc&cursor=def&skip_status=true&include_user_entities=false";
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.FriendsList.ToString() },
                { "UserID", "1" },
                { "ScreenName", "abc" },
                { "Cursor", "def" },
                { "SkipStatus", true.ToString() },
                { "IncludeUserEntities", false.ToString() }
            };

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildFriendListUrl_Requires_ScreenName_Or_UserID()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.FriendsList.ToString() },
                    //{ "UserID", "1" },
                    //{ "ScreenName", "abc" }
                };

            var ex = Assert.Throws<ArgumentNullException>(() => friendReqProc.BuildUrl(parameters));

            Assert.Equal("ScreenNameOrUserID", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Constructs_FollowersList_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/followers/list.json?user_id=1&screen_name=abc&cursor=def&skip_status=true&include_user_entities=false";
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", FriendshipType.FollowersList.ToString() },
                { "UserID", "1" },
                { "ScreenName", "abc" },
                { "Cursor", "def" },
                { "SkipStatus", true.ToString() },
                { "IncludeUserEntities", false.ToString() }
            };

            Request req = friendReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildFollowerListUrl_Requires_ScreenName_Or_UserID()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1.1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.FriendsList.ToString() },
                    //{ "UserID", "1" },
                    //{ "ScreenName", "abc" }
                };

            var ex = Assert.Throws<ArgumentNullException>(() => friendReqProc.BuildUrl(parameters));

            Assert.Equal("ScreenNameOrUserID", ex.ParamName);
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

        const string FreindsFollowersResponse = @"{
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
   ""next_cursor"":1412318031598699496,
   ""next_cursor_str"":""1412318031598699496"",
   ""previous_cursor"":0,
   ""previous_cursor_str"":""0""
}";
    }
}
