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

namespace LinqToTwitter.Tests.SearchTests
{
    [TestClass]
    public class TwitterSearchRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public TwitterSearchRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParametersTest()
        {
            var target = new TwitterSearchRequestProcessor<Search>();

            var endTime = new DateTime(2020, 8, 30);
            var startTime = new DateTime(2020, 8, 1);
            Expression<Func<TwitterSearch, bool>> expression =
                search =>
                    search.Type == SearchType.RecentSearch &&
                    search.EndTime == endTime &&
                    search.Expansions == "attachments.poll_ids,author_id" &&
                    search.MaxResults == 10 &&
                    search.MediaFields == "height,width" &&
                    search.NextToken == "abc" &&
                    search.PlaceFields == "country" &&
                    search.PollFields == "duration_minutes,end_datetime" &&
                    search.Query == "LINQ to Twitter" &&
                    search.SinceID == "123" &&
                    search.SortOrder == SearchSortOrder.Relevancy &&
                    search.StartTime == startTime &&
                    search.TweetFields == "author_id,created_at" &&
                    search.UntilID == "525" &&
                    search.UserFields == "created_at,verified";

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.Type), ((int)SearchType.RecentSearch).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.EndTime), "08/30/2020 00:00:00")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.Expansions), "attachments.poll_ids,author_id")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.MaxResults), "10")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.MediaFields), "height,width")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.NextToken), "abc")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(TwitterSearch.PlaceFields), "country")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(TwitterSearch.PollFields), "duration_minutes,end_datetime")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>(nameof(TwitterSearch.Query), "LINQ to Twitter")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.SinceID), "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.SortOrder), "relevancy")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.StartTime), "08/01/2020 00:00:00")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.TweetFields), "author_id,created_at")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.UntilID), "525")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterSearch.UserFields), "created_at,verified")));
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
                "sort_order=relevancy&" +
                "start_time=2020-12-31T00%3A00%3A01Z&" +
                "tweet.fields=author_id%2Ccreated_at&" +
                "until_id=525&" +
                "user.fields=created_at%2Cverified";
            var searchReqProc = new TwitterSearchRequestProcessor<Search> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TwitterSearch.Query), "LINQ to Twitter" },
                    { nameof(TwitterSearch.Type), SearchType.RecentSearch.ToString() },
                    { nameof(TwitterSearch.EndTime), new DateTime(2021, 1, 1, 12, 59, 59).ToString() },
                    { nameof(TwitterSearch.Expansions), "attachments.poll_ids,author_id" },
                    { nameof(TwitterSearch.MaxResults), 10.ToString() },
                    { nameof(TwitterSearch.MediaFields), "height,width" },
                    { nameof(TwitterSearch.NextToken), "abc" },
                    { nameof(TwitterSearch.PlaceFields), "country" },
                    { nameof(TwitterSearch.PollFields), "duration_minutes,end_datetime" },
                    { nameof(TwitterSearch.SinceID), "123" },
                    { nameof(TwitterSearch.SortOrder), SearchSortOrder.Relevancy },
                    { nameof(TwitterSearch.StartTime), new DateTime(2020, 12, 31, 0, 0, 1).ToString() },
                    { nameof(TwitterSearch.TweetFields), "author_id,created_at" },
                    { nameof(TwitterSearch.UntilID), "525" },
                    { nameof(TwitterSearch.UserFields), "created_at,verified" },
               };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Throws_When_Parameters_Null()
        {
            var searchReqProc = new TwitterSearchRequestProcessor<Search> { BaseUrl = BaseUrl2 };

            L2TAssert.Throws<NullReferenceException>(() =>
            {
                searchReqProc.BuildUrl(null);
            });
        }

        [TestMethod]
        public void BuildUrl_WithAllParameters_SendsParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "tweets/search/recent?" +
                "query=LINQ%20to%20Twitter&" +
                "end_time=2021-01-01T12%3A59%3A59Z&" +
                "expansions=author_id&" +
                "max_results=10&" +
                "media.fields=url&" +
                "next_token=abc&" +
                "place.fields=full_name&" +
                "poll.fields=voting_status&" +
                "since_id=123&" +
                "sort_order=recency&" +
                "start_time=2020-12-31T00%3A00%3A01Z&" +
                "tweet.fields=text&" +
                "until_id=525&" +
                "user.fields=username";
            var searchReqProc = new TwitterSearchRequestProcessor<TwitterSearch> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TwitterSearch.Type), SearchType.RecentSearch.ToString() },
                    { nameof(TwitterSearch.Query), "LINQ to Twitter" },
                    { nameof(TwitterSearch.EndTime), new DateTime(2021, 01, 01, 12, 59, 59).ToString() },
                    { nameof(TwitterSearch.Expansions), ExpansionField.AuthorID },
                    { nameof(TwitterSearch.MaxResults), 10.ToString() },
                    { nameof(TwitterSearch.MediaFields), MediaField.Url },
                    { nameof(TwitterSearch.NextToken), "abc" },
                    { nameof(TwitterSearch.PlaceFields), PlaceField.FullName },
                    { nameof(TwitterSearch.PollFields), PollField.VotingStatus },
                    { nameof(TwitterSearch.SinceID), 123.ToString() },
                    { nameof(TwitterSearch.SortOrder), SearchSortOrder.Recency },
                    { nameof(TwitterSearch.StartTime), new DateTime(2020, 12, 31, 00, 00, 01).ToString() },
                    { nameof(TwitterSearch.TweetFields), TweetField.Text },
                    { nameof(TwitterSearch.UntilID), 525.ToString() },
                    { nameof(TwitterSearch.UserFields), UserField.UserName },
               };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_WithSpacesInFields_FixesSpaces()
        {
            const string ExpectedUrl =
                BaseUrl2 + "tweets/search/recent?" +
                "query=LINQ%20to%20Twitter&" +
                "expansions=author_id%2Cgeo.place_id&" +
                "media.fields=url%2Cduration_ms&" +
                "place.fields=full_name%2Ccountry&" +
                "poll.fields=voting_status%2Coptions&" +
                "tweet.fields=text%2Ccreated_at&" +
                "user.fields=username%2Clocation";
            var searchReqProc = new TwitterSearchRequestProcessor<TwitterSearch> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TwitterSearch.Type), SearchType.RecentSearch.ToString() },
                    { nameof(TwitterSearch.Query), "LINQ to Twitter" },
                    { nameof(TwitterSearch.Expansions), ExpansionField.AuthorID + ", " + ExpansionField.PlaceID },
                    { nameof(TwitterSearch.MediaFields), MediaField.Url + ", " + MediaField.Duration },
                    { nameof(TwitterSearch.PlaceFields), PlaceField.FullName + ", " + PlaceField.Country },
                    { nameof(TwitterSearch.PollFields), PollField.VotingStatus + ", " + PollField.Options },
                    { nameof(TwitterSearch.TweetFields), TweetField.Text + ", " + TweetField.CreatedAt },
                    { nameof(TwitterSearch.UserFields), UserField.UserName + ", " + UserField.Location },
               };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Encodes_Query()
        {
            var searchReqProc = new TwitterSearchRequestProcessor<Search> { BaseUrl = BaseUrl2 };
            string expected = searchReqProc.BaseUrl + "tweets/search/recent?query=Contains%20Space";
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TwitterSearch.Type), SearchType.RecentSearch.ToString() },
                    { nameof(TwitterSearch.Query), "Contains Space" }
                };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Requires_Query()
        {
            var searchReqProc = new TwitterSearchRequestProcessor<Search> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TwitterSearch.Type), SearchType.RecentSearch.ToString() },
                    { nameof(TwitterSearch.Query), null }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentNullException>(() =>
                    searchReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(TwitterSearch.Query), ex.ParamName);
        }

        [TestMethod]
        public void ProcessResults_Populates_Meta()
        {
            var searchProc = new TwitterSearchRequestProcessor<TwitterSearch> { BaseUrl = BaseUrl2 };

            List<TwitterSearch> results = searchProc.ProcessResults(SearchDefaultJson);

            Assert.IsNotNull(results);
            TwitterSearch search = results.SingleOrDefault();
            Assert.IsNotNull(search);
            TwitterSearchMeta meta = search.Meta;
            Assert.IsNotNull(meta);
            Assert.AreEqual("1317802724407316480", meta.NewestID);
            Assert.AreEqual("1316030424850800640", meta.OldestID);
            Assert.AreEqual(4, meta.Count);
        }

        [TestMethod]
        public void ProcessResults_Populates_DefaultTweets()
        {
            var searchProc = new TwitterSearchRequestProcessor<TwitterSearch> { BaseUrl = BaseUrl2 };

            List<TwitterSearch> results = searchProc.ProcessResults(SearchDefaultJson);

            Assert.IsNotNull(results);
            TwitterSearch search = results.SingleOrDefault();
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
            var searchProc = new TwitterSearchRequestProcessor<TwitterSearch> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<TwitterSearch> searches = searchProc.ProcessResults(EmptyResponse);

            Assert.IsNotNull(searches);
            TwitterSearch search = searches.SingleOrDefault();
            Assert.IsNotNull(search);
            List<Tweet> results = search.Tweets;
            Assert.IsNull(results);

            TwitterSearchMeta meta = search.Meta;
            Assert.IsNotNull(meta);
            Assert.IsNull(meta.NewestID);
            Assert.IsNull(meta.OldestID);
            Assert.AreEqual(0, meta.Count);
        }

        [TestMethod]
        public void ProcessResults_Populates_Input_Parameters()
        {
            var searchProc = new TwitterSearchRequestProcessor<TwitterSearch>()
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
                SortOrder = SearchSortOrder.Relevancy,
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
            Assert.AreEqual(new DateTime(2020, 12, 31), search.EndTime);
            Assert.AreEqual("123", search.Expansions);
            Assert.AreEqual(100, search.MaxResults);
            Assert.AreEqual("456", search.MediaFields);
            Assert.AreEqual("789", search.NextToken);
            Assert.AreEqual("012", search.PlaceFields);
            Assert.AreEqual("345", search.PollFields);
            Assert.AreEqual("JoeMayo", search.Query);
            Assert.AreEqual("1", search.SinceID);
            Assert.AreEqual(SearchSortOrder.Relevancy, search.SortOrder);
            Assert.AreEqual(new DateTime(2020, 1, 1), search.StartTime);
            Assert.AreEqual("678", search.TweetFields);
            Assert.AreEqual("901", search.UntilID);
            Assert.AreEqual("234", search.UserFields);
        }


        [TestMethod]
        public void ProcessResults_WithErrors_PopulatesErrorList()
        {
            var searchProc = new TwitterSearchRequestProcessor<TwitterSearch> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<TwitterSearch> searches = searchProc.ProcessResults(SearchErrorJson);

            Assert.IsNotNull(searches);
            TwitterSearch search = searches.SingleOrDefault();
            Assert.IsNotNull(search);
            List<TwitterError> errors = search.Errors;
            Assert.IsNotNull(errors);
            Assert.AreEqual(2, errors.Count);
            TwitterError error = errors.FirstOrDefault();
            Assert.IsNotNull(error);
            Assert.AreEqual("tweet", error.ResourceType);
            Assert.AreEqual("non_public_metrics.impression_count", error.Field);
            Assert.AreEqual("Field Authorization Error", error.Title);
            Assert.AreEqual("data", error.Section);
            Assert.AreEqual("Sorry, you are not authorized to access non_public_metrics.impression_count on a Tweet.", error.Detail);
            Assert.AreEqual("https://api.twitter.com/2/problems/not-authorized-for-field", error.Type);
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

        #region SearchError

        const string SearchErrorJson = @"{
	""errors"": [
		{
			""resource_type"": ""tweet"",
			""field"": ""non_public_metrics.impression_count"",
			""title"": ""Field Authorization Error"",
			""section"": ""data"",
			""detail"": ""Sorry, you are not authorized to access non_public_metrics.impression_count on a Tweet."",
			""type"": ""https://api.twitter.com/2/problems/not-authorized-for-field""
		},
		{
			""resource_type"": ""tweet"",
			""field"": ""non_public_metrics.url_link_clicks"",
			""title"": ""Field Authorization Error"",
			""section"": ""data"",
			""detail"": ""Sorry, you are not authorized to access non_public_metrics.url_link_clicks on a Tweet."",
			""type"": ""https://api.twitter.com/2/problems/not-authorized-for-field""
		}
	],
	""meta"": {
		""result_count"": 0
	}
}";

        #endregion
    }
}
