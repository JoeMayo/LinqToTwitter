using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.SocialGraphTests
{
    public class SocialGraphRequestProcessorTests
    {
        public SocialGraphRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void SavedSearchRequestProcessor_Works_On_Json_Format_Data()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph>();

            Assert.IsAssignableFrom<IRequestProcessorWantsJson>(graphReqProc);
        }

        void TestQueryResponseParsesCorrectly(SocialGraphType type)
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph> { Type = type };

            List<SocialGraph> graphResponse = graphReqProc.ProcessResults(TestQueryResponse);

            Assert.NotNull(graphResponse);
            Assert.Single(graphResponse);
            var graph = graphResponse.First();
            var ids = graph.IDs;
            Assert.NotNull(ids);
            Assert.Equal(ids[0], "547559234");
            Assert.Equal(ids[1], "189123075");
            var cursor = graph.CursorMovement;
            Assert.NotNull(cursor);
            Assert.Equal("2", cursor.Previous);
            Assert.Equal("3", cursor.Next);
        }

        [Fact]
        public void ProcessResults_Parses_Followers_Response()
        {
            TestQueryResponseParsesCorrectly(SocialGraphType.Followers);
        }

        [Fact]
        public void ProcessResults_Parses_Friends_Response()
        {
            TestQueryResponseParsesCorrectly(SocialGraphType.Friends);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph>();

            var graph = graphReqProc.ProcessResults(string.Empty);

            Assert.Empty(graph);
        }

        [Fact]
        public void GetParameters_Handles_Input_Params()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph>();
            Expression<Func<SocialGraph, bool>> expression =
                graph =>
                    graph.Type == SocialGraphType.Followers &&
                    graph.UserID == 123ul &&
                    graph.ScreenName == "456" &&
                    graph.Cursor == "-1";
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = graphReqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)SocialGraphType.Followers).ToString())));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("UserID", "123")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "456")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "-1")));
        }

        [Fact]
        public void BuildUrl_Throws_On_Missing_Type()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>();

            var ex = Assert.Throws<ArgumentException>(() => graphReqProc.BuildUrl(parameters));

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Constructs_Friends_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/friends/ids.json?user_id=123&screen_name=456&cursor=1";
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", "0" },
                { "UserID", "123" },
                { "ScreenName", "456" },
                { "Cursor", "1" }
            };

            Request req = graphReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Defaults_Cursor_When_Not_Specified()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/friends/ids.json?screen_name=JoeMayo&cursor=-1";
            var socialGraph = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SocialGraphType.Friends.ToString() },
                    { "ScreenName", "JoeMayo" },
                };

            Request req = socialGraph.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_Followers_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/followers/ids.json?user_id=123&screen_name=456&cursor=1";
            var reqProc = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)SocialGraphType.Followers).ToString() },
                { "UserID", "123" },
                { "ScreenName", "456" },
                { "Cursor", "1" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_On_Null_Params()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph> { BaseUrl = "https://api.twitter.com/1/" };

            var ex = Assert.Throws<ArgumentException>(() => graphReqProc.BuildUrl(null));

            Assert.Equal<string>("Type", ex.ParamName);
        }

        const string TestQueryResponse = @"{
   ""previous_cursor"":2,
   ""next_cursor_str"":""3"",
   ""ids"":[
      547559234,
      189123075
   ],
   ""previous_cursor_str"":""2"",
   ""next_cursor"":3
}";
    }
}
