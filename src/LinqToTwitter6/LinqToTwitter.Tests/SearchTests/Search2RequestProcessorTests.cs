using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitter.Tests.SearchTests
{
    [TestClass]
    public class Search2RequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public Search2RequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParametersTest()
        {
            var target = new Search2RequestProcessor<Search>();

            Expression<Func<Search2, bool>> expression =
                search =>
                    search.Type == SearchType.RecentSearch &&
                    search.EndTime == "2020-08-30T12:59:59Z" &&
                    search.Expansions == "attachments.poll_ids,author_id" &&
                    search.MaxResults == 10 &&
                    search.MediaFields == "height,width" &&
                    search.NextToken == "abc" &&
                    search.PlaceFields == "country" &&
                    search.PollFields == "duration_minutes,end_datetime" &&
                    search.Query == "LINQ to Twitter" &&
                    search.SinceID == "123" &&
                    search.StartTime == "2020-08-30T12:59:59Z" &&
                    search.TweetFields == "author_id,created_at" &&
                    search.UntilID == "525" &&
                    search.UserFields == "created_at,verified";

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.Type), ((int)SearchType.RecentSearch).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.EndTime), "2020-08-30T12:59:59Z")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.Expansions), "attachments.poll_ids,author_id")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.MaxResults), "10")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.MediaFields), "height,width")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.NextToken), "abc")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(Search2.PlaceFields), "country")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(Search2.PollFields), "duration_minutes,end_datetime")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>(nameof(Search2.Query), "LINQ to Twitter")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.SinceID), "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.StartTime), "2020-08-30T12:59:59Z")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.TweetFields), "author_id,created_at")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.UntilID), "525")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Search2.UserFields), "created_at,verified")));
        }

        [TestMethod]
        public void BuildUrl_Includes_Parameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "tweets/search/recent?" +
                "query=LINQ%20to%20Twitter&" +
                "end_time=2021-01-01T12%3A59%3A59Z&" +
                "expansions=attachments.poll_ids%2Cauthor_id&" +
                "max_results=10&" +
                "media.fields=height%2Cwidth&" +
                "next_token=abc&" +
                "place.fields=country&" +
                "poll.fields=duration_minutes%2Cend_datetime&" +
                "since_id=123&" +
                "start_time=2020-12-31T00%3A00%3A01Z&" +
                "tweet.fields=author_id%2Ccreated_at&" +
                "until_id=525&" +
                "user.fields=created_at%2Cverified";
            var searchReqProc = new Search2RequestProcessor<Search> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(Search2.Query), "LINQ to Twitter" },
                    { nameof(Search2.Type), SearchType.RecentSearch.ToString() },
                    { nameof(Search2.EndTime), new DateTime(2021, 1, 1, 12, 59, 59).ToString() },
                    { nameof(Search2.Expansions), "attachments.poll_ids,author_id" },
                    { nameof(Search2.MaxResults), 10.ToString() },
                    { nameof(Search2.MediaFields), "height,width" },
                    { nameof(Search2.NextToken), "abc" },
                    { nameof(Search2.PlaceFields), "country" },
                    { nameof(Search2.PollFields), "duration_minutes,end_datetime" },
                    { nameof(Search2.SinceID), "123" },
                    { nameof(Search2.StartTime), new DateTime(2020, 12, 31, 0, 0, 1).ToString() },
                    { nameof(Search2.TweetFields), "author_id,created_at" },
                    { nameof(Search2.UntilID), "525" },
                    { nameof(Search2.UserFields), "created_at,verified" },
               };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Throws_When_Parameters_Null()
        {
            var searchReqProc = new Search2RequestProcessor<Search> { BaseUrl = BaseUrl2 };

            L2TAssert.Throws<NullReferenceException>(() =>
            {
                searchReqProc.BuildUrl(null);
            });
        }

        [TestMethod]
        public void BuildUrl_Encodes_Query()
        {
            var searchReqProc = new Search2RequestProcessor<Search> { BaseUrl = BaseUrl2 };
            string expected = searchReqProc.BaseUrl + "tweets/search/recent?query=Contains%20Space";
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(Search2.Type), SearchType.RecentSearch.ToString() },
                    { nameof(Search2.Query), "Contains Space" }
                };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Requires_Query()
        {
            var searchReqProc = new Search2RequestProcessor<Search> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(Search2.Type), SearchType.RecentSearch.ToString() },
                    { nameof(Search2.Query), null }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentNullException>(() =>
                    searchReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(Search2.Query), ex.ParamName);
        }

        [TestMethod]
        public void ProcessResults_Populates_Meta()
        {
            var searchProc = new Search2RequestProcessor<Search2> { BaseUrl = BaseUrl2 };

            List<Search2> results = searchProc.ProcessResults(SearchDefaultJson);

            Assert.IsNotNull(results);
            Search2 search = results.SingleOrDefault();
            Assert.IsNotNull(search);
            Search2Meta meta = search.Meta;
            Assert.IsNotNull(meta);
            Assert.AreEqual("1317802724407316480", meta.NewestID);
            Assert.AreEqual("1316030424850800640", meta.OldestID);
            Assert.AreEqual(4, meta.Count);
        }

        [TestMethod]
        public void ProcessResults_Populates_DefaultTweets()
        {
            var searchProc = new Search2RequestProcessor<Search2> { BaseUrl = BaseUrl2 };

            List<Search2> results = searchProc.ProcessResults(SearchDefaultJson);

            Assert.IsNotNull(results);
            Search2 search = results.SingleOrDefault();
            Assert.IsNotNull(search);
            List<Tweet> tweets = search.Tweets;
            Assert.IsNotNull(tweets);
            Assert.AreEqual(4, tweets.Count);
            Tweet firstTweet = tweets.FirstOrDefault();
            Assert.IsNotNull(firstTweet);
            Assert.AreEqual("1317802724407316480", firstTweet.ID);
            Assert.AreEqual("Test Data 1", firstTweet.Text);
        }

        [TestMethod]
        public void ProcessResults_Handles_Response_With_No_Results()
        {
            var searchProc = new Search2RequestProcessor<Search2> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search2> searches = searchProc.ProcessResults(EmptyResponse);

            Assert.IsNotNull(searches);
            Search2 search = searches.SingleOrDefault();
            Assert.IsNotNull(search);
            List<Tweet> results = search.Tweets;
            Assert.IsNull(results);

            Search2Meta meta = search.Meta;
            Assert.IsNotNull(meta);
            Assert.IsNull(meta.NewestID);
            Assert.IsNull(meta.OldestID);
            Assert.AreEqual(0, meta.Count);
        }


        [TestMethod]
        public void ProcessResults_Populates_Input_Parameters()
        {
            var searchProc = new Search2RequestProcessor<Search2>()
            {
                BaseUrl = BaseUrl2,
                Type = SearchType.RecentSearch,
                EndTime = new DateTime(2020, 12, 31),
                Expansions = "123",
                MaxResults = 100,
                MediaFields = "456",
                NextToken = "789",
                PlaceFields = "012",
                PollFields = "345",
                Query = "JoeMayo",
                SinceID = "1",
                StartTime = new DateTime(2020, 1, 1),
                TweetFields = "678",
                UntilID = "901",
                UserFields = "234"
            };

            var searchResult = searchProc.ProcessResults(SearchDefaultJson);

            Assert.IsNotNull(searchResult);
            Assert.AreEqual(1, searchResult.Count);
            var search = searchResult.Single();
            Assert.IsNotNull(search);
            Assert.AreEqual(SearchType.RecentSearch, search.Type);
            //Assert.AreEqual(new DateTime(2020, 12, 31), search.EndTime); // TODO: Finish Test
            Assert.AreEqual("123", search.Expansions);
            Assert.AreEqual(100, search.MaxResults);
            Assert.AreEqual("456", search.MediaFields);
            Assert.AreEqual("789", search.NextToken);
            Assert.AreEqual("012", search.PlaceFields);
            Assert.AreEqual("345", search.PollFields);
            Assert.AreEqual("JoeMayo", search.Query);
            Assert.AreEqual("1", search.SinceID);
            //Assert.AreEqual(new DateTime(2020, 1, 1), search.StartTime); // TODO: Finish Test
            Assert.AreEqual("678", search.TweetFields);
            Assert.AreEqual("901", search.UntilID);
            Assert.AreEqual("234", search.UserFields);
        }

        #region EmptyResponse

        const string EmptyResponse = @"{""meta"":{""result_count"":0}}";

        #endregion

        #region SearchJson

        const string SearchDefaultJson = @"{
	""data"": [
		{
			""id"": ""1317802724407316480"",
			""text"": ""Test Data 1""

        },
		{
			""id"": ""1317307540561121280"",
			""text"": ""Test Data 2""
		},
		{
    ""id"": ""1317077936948785152"",
			""text"": ""Test Data 3""

        },
		{
    ""id"": ""1316030424850800640"",
			""text"": ""Test Data 4""

        }
	],
	""meta"": {
        ""newest_id"": ""1317802724407316480"",
		""oldest_id"": ""1316030424850800640"",
		""result_count"": 4

    }
}";

        #endregion
    }
}
