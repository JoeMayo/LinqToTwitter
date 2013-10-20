using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.SocialGraphTests
{
    public class SocialGraphRequestProcessorTests
    {
        public SocialGraphRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void SavedSearchRequestProcessor_Works_On_Json_Format_Data()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph>();

            Assert.IsNotInstanceOfType(graphReqProc, typeof(IRequestProcessorWantsJson));
        }

        void TestQueryResponseParsesCorrectly(SocialGraphType type)
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph> { Type = type };

            List<SocialGraph> graphResponse = graphReqProc.ProcessResults(TestQueryResponse);

            Assert.IsNotNull(graphResponse);
            Assert.IsNotNull(graphResponse.SingleOrDefault());
            var graph = graphResponse.First();
            var ids = graph.IDs;
            Assert.IsNotNull(ids);
            Assert.AreEqual(ids[0], "547559234");
            Assert.AreEqual(ids[1], "189123075");
            var cursor = graph.CursorMovement;
            Assert.IsNotNull(cursor);
            Assert.AreEqual("2", cursor.Previous);
            Assert.AreEqual("3", cursor.Next);
        }

        [TestMethod]
        public void ProcessResults_Parses_Followers_Response()
        {
            TestQueryResponseParsesCorrectly(SocialGraphType.Followers);
        }

        [TestMethod]
        public void ProcessResults_Parses_Friends_Response()
        {
            TestQueryResponseParsesCorrectly(SocialGraphType.Friends);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph>();

            var graph = graphReqProc.ProcessResults(string.Empty);

            Assert.IsFalse(graph.Any());
        }

        [TestMethod]
        public void GetParameters_Handles_Input_Params()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph>();
            Expression<Func<SocialGraph, bool>> expression =
                graph =>
                    graph.Type == SocialGraphType.Followers &&
                    graph.UserID == 123ul &&
                    graph.ScreenName == "456" &&
                    graph.Cursor == "-1" &&
                    graph.Count == 1;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = graphReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)SocialGraphType.Followers).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("UserID", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "-1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Count", "1")));
        }

        [TestMethod]
        public void ProcessResults_Populates_Input_Parameters()
        {
            var statProc = new SocialGraphRequestProcessor<SocialGraph>()
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = SocialGraphType.Friends,
                UserID = 123,
                ScreenName = "abc",
                Cursor = "-1",
                Count = 3,
            };

            var graphResult = statProc.ProcessResults(TestQueryResponse);

            Assert.IsNotNull(graphResult);
            Assert.IsNotNull(graphResult.SingleOrDefault());
            var graph = graphResult.Single();
            Assert.IsNotNull(graph);
            Assert.AreEqual(SocialGraphType.Friends, graph.Type);
            Assert.AreEqual(123ul, graph.UserID);
            Assert.AreEqual("abc", graph.ScreenName);
            Assert.AreEqual("-1", graph.Cursor);
            Assert.AreEqual(3, graph.Count);
        }

        [TestMethod]
        [Ignore]
        public void BuildUrl_Throws_On_Missing_Type()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>();

            //var ex = Assert.Throws<ArgumentException>(() => graphReqProc.BuildUrl(parameters));

            //Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Friends_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/friends/ids.json?user_id=123&screen_name=456&cursor=1&count=1";
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", "0" },
                { "UserID", "123" },
                { "ScreenName", "456" },
                { "Cursor", "1" },
                { "Count", "1" }
            };

            Request req = graphReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Defaults_Cursor_When_Not_Specified()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/friends/ids.json?screen_name=JoeMayo&cursor=-1";
            var socialGraph = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "https://api.twitter.com/1.1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SocialGraphType.Friends.ToString() },
                    { "ScreenName", "JoeMayo" },
                };

            Request req = socialGraph.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Followers_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/followers/ids.json?user_id=123&screen_name=456&cursor=1&count=1";
            var reqProc = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int)SocialGraphType.Followers).ToString() },
                { "UserID", "123" },
                { "ScreenName", "456" },
                { "Cursor", "1" },
                { "Count", "1" }
            };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        [Ignore]
        public void BuildUrl_Throws_On_Null_Params()
        {
            var graphReqProc = new SocialGraphRequestProcessor<SocialGraph> { BaseUrl = "https://api.twitter.com/1.1/" };

            //var ex = Assert.Throws<ArgumentException>(() => graphReqProc.BuildUrl(null));

            //Assert.AreEqual<string>("Type", ex.ParamName);
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
