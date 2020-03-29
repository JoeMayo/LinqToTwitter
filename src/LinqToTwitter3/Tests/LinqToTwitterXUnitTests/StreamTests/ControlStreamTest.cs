using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class ControlStreamTest
    {
        public ControlStreamTest()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void GetParameters_Returns_Parameters()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream>();
            Expression<Func<ControlStream, bool>> expression =
                strm =>
                    strm.Type == ControlStreamType.Followers &&
                    strm.UserID == 10 &&
                    strm.StreamID == "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f";
            var lambdaExpression = expression as LambdaExpression;

            var parms = reqProc.GetParameters(lambdaExpression);

            Assert.True(parms.Contains(
                    new KeyValuePair<string, string>("Type", ((int)ControlStreamType.Followers).ToString())));
            Assert.True(parms.Contains(
                   new KeyValuePair<string, string>("UserID", "10")));
            Assert.True(parms.Contains(
                   new KeyValuePair<string, string>("StreamID", "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f")));
        }

        [Fact]
        public void ProcessResults_Replaces_Input_Params()
        {
            const ulong UserID = 1;
            const string StreamID = "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f";

            var csReqProc = new ControlStreamRequestProcessor<ControlStream>
            {
                BaseUrl = "https://api.twitter.com/1/",
                Type = ControlStreamType.Followers,
                UserID = UserID,
                StreamID = StreamID
            };

            var csResult = csReqProc.ProcessResults(FollowersMessage);

            var cs = csResult.First();

            Assert.Equal(ControlStreamType.Followers, cs.Type);
            Assert.Equal(UserID, cs.UserID);
            Assert.Equal(StreamID, cs.StreamID);
        }

        [Fact]
        public void BuildUrl_Constructs_Followers_Url()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream> { SiteStreamUrl = "https://sitestream.twitter.com/1.1/" };
            const string Expected = "https://sitestream.twitter.com/1.1/site/c/1_1_54e345d655ee3e8df359ac033648530bfbe26c5f/friends/ids.json?user_id=478805447";
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ControlStreamType.Followers).ToString(CultureInfo.InvariantCulture) },
                    { "UserID", "478805447" },
                    { "StreamID", "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f" }
                };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(Expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Followers_Requires_UserID()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream> { SiteStreamUrl = "https://sitestream.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ControlStreamType.Followers).ToString(CultureInfo.InvariantCulture) },
                    //{ "UserID", "478805447" },
                    { "StreamID", "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f" }
                };

            Assert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));
        }

        [Fact]
        public void BuildUrl_Followers_Requires_StreamID()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream> { SiteStreamUrl = "https://sitestream.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ControlStreamType.Followers).ToString(CultureInfo.InvariantCulture) },
                    { "UserID", "478805447" },
                    //{ "StreamID", "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f" }
                };

            Assert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));
        }

        [Fact]
        public void BuildUrl_Followers_Sets_UserID()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream> { SiteStreamUrl = "https://sitestream.twitter.com/1.1/" };
            const ulong ExpectedUserID = 478805447;
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ControlStreamType.Followers).ToString(CultureInfo.InvariantCulture) },
                    { "UserID", "478805447" },
                    { "StreamID", "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f" }
                };

            reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUserID, reqProc.UserID);
        }

        [Fact]
        public void BuildUrl_Followers_Sets_StreamID()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream> { SiteStreamUrl = "https://sitestream.twitter.com/1.1/" };
            const string ExpectedStreamID = "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f";
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ControlStreamType.Followers).ToString(CultureInfo.InvariantCulture) },
                    { "UserID", "478805447" },
                    { "StreamID", "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f" }
                };

            reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedStreamID, reqProc.StreamID);
        }

        [Fact]
        public void BuildUrl_Constructs_Info_Url()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream> { SiteStreamUrl = "https://sitestream.twitter.com/1.1/" };
            const string Expected = "https://sitestream.twitter.com/1.1/site/c/1_1_54e345d655ee3e8df359ac033648530bfbe26c5f/info.json";
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ControlStreamType.Info).ToString(CultureInfo.InvariantCulture) },
                    { "StreamID", "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f" }
                };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(Expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Info_Requires_StreamID()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream> { SiteStreamUrl = "https://sitestream.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ControlStreamType.Info).ToString(CultureInfo.InvariantCulture) },
                    //{ "StreamID", "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f" }
                };

            Assert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parameters));
        }

        [Fact]
        public void BuildUrl_Info_Sets_Stream_ID()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream> { SiteStreamUrl = "https://sitestream.twitter.com/1.1/" };
            const string ExpectedStreamID = "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f";
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ControlStreamType.Info).ToString(CultureInfo.InvariantCulture) },
                    { "StreamID", "1_1_54e345d655ee3e8df359ac033648530bfbe26c5f" }
                };

            reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedStreamID, reqProc.StreamID);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new ControlStreamRequestProcessor<ControlStream> { SiteStreamUrl = "https://sitestream.twitter.com/1.1/" };

            var csList = reqProc.ProcessResults(string.Empty);

            Assert.False(csList.Any());
        }

        [Fact]
        public void ProcessResults_Handles_Follower_Response()
        {
            const ulong ExpectedUserID = 15411837;
            const string ExpectedUserName = "JoeMayo";
            const bool ExpectedDM = true;
            const string ExpectedPrevCursor = "1";
            const string ExpectedNextCursor = "3";
            const int ExpectedNumberOfFriends = 5;
            const ulong ExpectedFirstFriendID = 13348;
            var reqProc = new ControlStreamRequestProcessor<ControlStream> 
            { 
                SiteStreamUrl = "https://sitestream.twitter.com/1.1/",
                Type = ControlStreamType.Followers
            };

            var csList = reqProc.ProcessResults(FollowersMessage);

            Assert.Single(csList);
            var cs = csList.Single();
            var follow = cs.Follow;
            Assert.NotNull(follow);
            var user = follow.User;
            Assert.NotNull(user);
            Assert.Equal(ExpectedUserID, user.UserID);
            Assert.Equal(ExpectedUserName, user.Name);
            Assert.Equal(ExpectedDM, user.DM);
            var cursors = follow.Cursors;
            Assert.NotNull(cursors);
            Assert.Equal(ExpectedPrevCursor, cursors.Previous);
            Assert.Equal(ExpectedNextCursor, cursors.Next);
            var friends = follow.Friends;
            Assert.NotNull(friends);
            Assert.NotEmpty(friends);
            Assert.Equal(ExpectedNumberOfFriends, friends.Count);
            Assert.Equal(ExpectedFirstFriendID, friends.First());
        }

        [Fact]
        public void ProcessResults_Handles_Info_Response()
        {
            const ulong ExpectedUserID = 15411837;
            const string ExpectedUserName = "JoeMayo";
            const bool ExpectedDM = true;
            const string ExpectedDelimited = "none";
            const bool ExpectedIncludeFollowingsActivity = true;
            const bool ExpectedIncludeUserChanges = true;
            const string ExpectedReplies = "none";
            const string ExpectedWith = "user";
            var reqProc = new ControlStreamRequestProcessor<ControlStream>
            {
                SiteStreamUrl = "https://sitestream.twitter.com/1.1/",
                Type = ControlStreamType.Info
            };

            var csList = reqProc.ProcessResults(InfoMessage);

            Assert.Single(csList);
            var cs = csList.Single();
            var info = cs.Info;
            Assert.NotNull(info);
            var users = info.Users;
            Assert.NotNull(users);
            Assert.NotEmpty(users);
            var user = users.First();
            Assert.NotNull(user);
            Assert.Equal(ExpectedUserID, user.UserID);
            Assert.Equal(ExpectedUserName, user.Name);
            Assert.Equal(ExpectedDM, user.DM);
            Assert.Equal(ExpectedDelimited, info.Delimited);
            Assert.Equal(ExpectedIncludeFollowingsActivity, info.IncludeFollowingsActivity);
            Assert.Equal(ExpectedIncludeUserChanges, info.IncludeUserChanges);
            Assert.Equal(ExpectedReplies, info.Replies);
            Assert.Equal(ExpectedWith, info.With);
        }


        const string FollowersMessage = @"{
   ""follow"":{
      ""user"":{
         ""id"":15411837,
         ""name"":""JoeMayo"",
         ""dm"":true
      },
      ""friends"":[
         13348,
         783214,
         819797,
         823083,
         1535551
      ],
      ""previous_cursor"":1,
      ""next_cursor"":3
   }
}";

        const string InfoMessage = @"{
   ""info"":{
      ""users"":[
         {
            ""id"":15411837,
            ""name"":""JoeMayo"",
            ""dm"":true
         }
      ],
      ""delimited"":""none"",
      ""include_followings_activity"":true,
      ""include_user_changes"":true,
      ""replies"":""none"",
      ""with"":""user""
   }
}";
    }
}
