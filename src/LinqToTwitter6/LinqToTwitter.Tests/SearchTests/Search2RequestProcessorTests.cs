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
                "https://api.twitter.com/2/tweets/search/recent?" +
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
            var searchReqProc = new Search2RequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };
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
    }
}
