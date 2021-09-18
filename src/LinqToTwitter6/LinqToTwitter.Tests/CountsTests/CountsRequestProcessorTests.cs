using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitter.Tests.CountsTests
{
    [TestClass]
    public class CountsRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public CountsRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParametersTest()
        {
            var target = new CountsRequestProcessor<Counts>();

            var endTime = new DateTime(2020, 8, 30);
            var startTime = new DateTime(2020, 8, 1);
            Expression<Func<Counts, bool>> expression =
                count =>
                    count.Type == CountType.All &&
                    count.EndTime == endTime &&
                    count.Granularity == Granularity.Day &&
                    count.NextToken == "abc" &&
                    count.Query == "LINQ to Twitter" &&
                    count.SinceID == "123" &&
                    count.StartTime == startTime &&
                    count.UntilID == "525";

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Counts.Type), ((int)CountType.All).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Counts.EndTime), "08/30/2020 00:00:00")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Counts.Granularity), ((int)Granularity.Day).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Counts.NextToken), "abc")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>(nameof(Counts.Query), "LINQ to Twitter")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Counts.SinceID), "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Counts.StartTime), "08/01/2020 00:00:00")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Counts.UntilID), "525")));
        }

        [TestMethod]
        public void BuildUrl_ForRecent_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "tweets/counts/recent?" +
                "query=LINQ%20to%20Twitter&" +
                "end_time=2021-01-01T12%3A59%3A59Z&" +
                "granularity=day&" +
                "next_token=abc&" +
                "since_id=123&" +
                "start_time=2020-12-31T00%3A00%3A01Z&" +
                "until_id=525";
            var countReqProc = new CountsRequestProcessor<Counts> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(Counts.Query), "LINQ to Twitter" },
                    { nameof(Counts.Type), CountType.Recent.ToString() },
                    { nameof(Counts.EndTime), new DateTime(2021, 1, 1, 12, 59, 59).ToString() },
                    { nameof(Counts.Granularity), Granularity.Day.ToString().ToLower() },
                    { nameof(Counts.NextToken), "abc" },
                    { nameof(Counts.SinceID), "123" },
                    { nameof(Counts.StartTime), new DateTime(2020, 12, 31, 0, 0, 1).ToString() },
                    { nameof(Counts.UntilID), "525" },
               };

            Request req = countReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForAll_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "tweets/counts/all?" +
                "query=LINQ%20to%20Twitter&" +
                "end_time=2021-01-01T12%3A59%3A59Z&" +
                "granularity=day&" +
                "next_token=abc&" +
                "since_id=123&" +
                "start_time=2020-12-31T00%3A00%3A01Z&" +
                "until_id=525";
            var countReqProc = new CountsRequestProcessor<Counts> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(Counts.Query), "LINQ to Twitter" },
                    { nameof(Counts.Type), CountType.All.ToString() },
                    { nameof(Counts.EndTime), new DateTime(2021, 1, 1, 12, 59, 59).ToString() },
                    { nameof(Counts.Granularity), Granularity.Day.ToString().ToLower() },
                    { nameof(Counts.NextToken), "abc" },
                    { nameof(Counts.SinceID), "123" },
                    { nameof(Counts.StartTime), new DateTime(2020, 12, 31, 0, 0, 1).ToString() },
                    { nameof(Counts.UntilID), "525" },
               };

            Request req = countReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Throws_When_Parameters_Null()
        {
            var countReqProc = new CountsRequestProcessor<Counts> { BaseUrl = BaseUrl2 };

            L2TAssert.Throws<NullReferenceException>(() =>
            {
                countReqProc.BuildUrl(null);
            });
        }

        [TestMethod]
        public void BuildUrl_Requires_Query()
        {
            var countReqProc = new CountsRequestProcessor<Counts> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(Counts.Type), CountType.Recent.ToString() },
                    { nameof(Counts.Query), null },
               };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentNullException>(() =>
                    countReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(Counts.Query), ex.ParamName);
        }

        [TestMethod]
        public void ProcessResults_Populates_CountRanges()
        {
            var countReqProc = new CountsRequestProcessor<Counts> { BaseUrl = BaseUrl2 };

            List<Counts> results = countReqProc.ProcessResults(CountResponse);

            Assert.IsNotNull(results);
            Counts counts = results.SingleOrDefault();
            Assert.IsNotNull(counts);
            List<CountRange> ranges = counts.CountRanges;
            Assert.IsNotNull(ranges);
            Assert.AreEqual(8, ranges.Count);
            CountRange range = ranges[6];
            Assert.AreEqual(DateTime.Parse("2021-09-18").Date, range.End.Date);
            Assert.AreEqual(DateTime.Parse("2021-09-17").Date, range.Start.Date);
            Assert.AreEqual(2, range.TweetCount);
        }

        [TestMethod]
        public void ProcessResults_Populates_Meta()
        {
            var countReqProc = new CountsRequestProcessor<Counts> { BaseUrl = BaseUrl2 };

            List<Counts> results = countReqProc.ProcessResults(CountResponse);

            Assert.IsNotNull(results);
            Counts counts = results.SingleOrDefault();
            Assert.IsNotNull(counts);
            CountsMeta meta = counts.Meta;
            Assert.IsNotNull(meta);
            Assert.AreEqual(5, meta.TotalTweetCount);
        }

        [TestMethod]
        public void ProcessResults_Populates_Input_Parameters()
        {
            var countReqProc = new CountsRequestProcessor<Counts>
            {
                BaseUrl = BaseUrl2,
                Type = CountType.Recent,
                EndTime = new DateTime(2020, 12, 31),
                Granularity = Granularity.Minute,
                NextToken = "789",
                Query = "JoeMayo",
                SinceID = "1",
                StartTime = new DateTime(2020, 1, 1),
                UntilID = "901"
            };

            var countResult = countReqProc.ProcessResults(CountResponse);

            Assert.IsNotNull(countResult);
            Assert.AreEqual(1, countResult.Count);
            var count = countResult.Single();
            Assert.IsNotNull(count);
            Assert.AreEqual(CountType.Recent, count.Type);
            Assert.AreEqual(new DateTime(2020, 12, 31), count.EndTime);
            Assert.AreEqual(Granularity.Minute, count.Granularity);
            Assert.AreEqual("789", count.NextToken);
            Assert.AreEqual("JoeMayo", count.Query);
            Assert.AreEqual("1", count.SinceID);
            Assert.AreEqual(new DateTime(2020, 1, 1), count.StartTime);
            Assert.AreEqual("901", count.UntilID);
        }

        #region CountResponse

        const string CountResponse = @"{
	""data"": [
		{
			""end"": ""2021-09-12T00:00:00.000Z"",
			""start"": ""2021-09-11T02:15:42.000Z"",
			""tweet_count"": 0
		},
		{
			""end"": ""2021-09-13T00:00:00.000Z"",
			""start"": ""2021-09-12T00:00:00.000Z"",
			""tweet_count"": 3
		},
		{
			""end"": ""2021-09-14T00:00:00.000Z"",
			""start"": ""2021-09-13T00:00:00.000Z"",
			""tweet_count"": 0
		},
		{
			""end"": ""2021-09-15T00:00:00.000Z"",
			""start"": ""2021-09-14T00:00:00.000Z"",
			""tweet_count"": 0
		},
		{
			""end"": ""2021-09-16T00:00:00.000Z"",
			""start"": ""2021-09-15T00:00:00.000Z"",
			""tweet_count"": 0
		},
		{
			""end"": ""2021-09-17T00:00:00.000Z"",
			""start"": ""2021-09-16T00:00:00.000Z"",
			""tweet_count"": 0
		},
		{
			""end"": ""2021-09-18T00:00:00.000Z"",
			""start"": ""2021-09-17T00:00:00.000Z"",
			""tweet_count"": 2
		},
		{
			""end"": ""2021-09-18T02:15:42.000Z"",
			""start"": ""2021-09-18T00:00:00.000Z"",
			""tweet_count"": 0
		}
	],
	""meta"": {
		""total_tweet_count"": 5
	}
}";

        #endregion

    }
}
