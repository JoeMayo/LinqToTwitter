using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitter.Tests.TweetTests
{
    [TestClass]
    public class TweetRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public TweetRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParametersTest()
        {
            var target = new TweetRequestProcessor<TweetQuery>();

			var endTime = new DateTime(2020, 8, 30);
			var startTime = new DateTime(2020, 8, 1);
			Expression<Func<TweetQuery, bool>> expression =
				tweet =>
					tweet.Type == TweetType.Lookup &&
					tweet.EndTime == endTime &&
					tweet.Exclude == TweetExcludes.Replies &&
					tweet.Ids == "2,3" &&
					tweet.ID == "123" &&
					tweet.Expansions == "attachments.poll_ids,author_id" &&
					tweet.MaxResults == 99 &&
					tweet.MediaFields == "height,width" &&
					tweet.PaginationToken == "456" &&
					tweet.PlaceFields == "country" &&
					tweet.PollFields == "duration_minutes,end_datetime" &&
					tweet.SinceID == "789" &&
					tweet.SpaceID == "345" &&
					tweet.StartTime == startTime &&
					tweet.TweetFields == "author_id,created_at" &&
					tweet.UntilID == "012" &&
                    tweet.UserFields == "created_at,verified";

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.Type), ((int)TweetType.Lookup).ToString(CultureInfo.InvariantCulture))));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TweetQuery.EndTime), "08/30/2020 00:00:00")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TweetQuery.Exclude), TweetExcludes.Replies)));
			Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.Ids), "2,3")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TweetQuery.ID), "123")));
			Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.Expansions), "attachments.poll_ids,author_id")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TweetQuery.MaxResults), "99")));
			Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.MediaFields), "height,width")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TweetQuery.PaginationToken), "456")));
			Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(TweetQuery.PlaceFields), "country")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(TweetQuery.PollFields), "duration_minutes,end_datetime")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TweetQuery.SinceID), "789")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TweetQuery.SpaceID), "345")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TweetQuery.StartTime), "08/01/2020 00:00:00")));
			Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.TweetFields), "author_id,created_at")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TweetQuery.UntilID), "012")));
			Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.UserFields), "created_at,verified")));
        }

        [TestMethod]
        public void BuildUrl_ForLookup_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "tweets?" +
                "ids=2%2C3&" +
                "expansions=attachments.poll_ids%2Cauthor_id&" +
                "media.fields=height%2Cwidth&" +
                "place.fields=country&" +
                "poll.fields=duration_minutes%2Cend_datetime&" +
                "tweet.fields=author_id%2Ccreated_at&" +
                "user.fields=created_at%2Cverified";
            var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TweetQuery.Type), TweetType.Lookup.ToString() },
                    { nameof(TweetQuery.Ids), "2,3" },
                    { nameof(TweetQuery.Expansions), "attachments.poll_ids,author_id" },
                    { nameof(TweetQuery.MediaFields), "height,width" },
                    { nameof(TweetQuery.PlaceFields), "country" },
                    { nameof(TweetQuery.PollFields), "duration_minutes,end_datetime" },
                    { nameof(TweetQuery.TweetFields), "author_id,created_at" },
                    { nameof(TweetQuery.UserFields), "created_at,verified" },
               };

            Request req = tweetReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

		[TestMethod]
		public void BuildUrl_ForMentionsTimeline_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "users/123/mentions?" +
				"end_time=2021-01-01T12%3A59%3A59Z&" +
				"exclude=replies%2Cretweets&" +
				"max_results=50&" +
				"pagination_token=456&" +
				"since_id=789&" +
				"start_time=2020-12-31T00%3A00%3A01Z&" +
				"until_id=012&" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
				"media.fields=height%2Cwidth&" +
				"place.fields=country&" +
				"poll.fields=duration_minutes%2Cend_datetime&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TweetQuery.Type), TweetType.MentionsTimeline.ToString() },
					{ nameof(TweetQuery.EndTime), new DateTime(2021, 1, 1, 12, 59, 59).ToString() },
					{ nameof(TweetQuery.Exclude), TweetExcludes.All },
					{ nameof(TweetQuery.Expansions), "attachments.poll_ids,author_id" },
					{ nameof(TweetQuery.ID), "123" },
					{ nameof(TweetQuery.MaxResults), "50" },
					{ nameof(TweetQuery.MediaFields), "height,width" },
					{ nameof(TweetQuery.PaginationToken), "456" },
					{ nameof(TweetQuery.PlaceFields), "country" },
					{ nameof(TweetQuery.PollFields), "duration_minutes,end_datetime" },
					{ nameof(TweetQuery.SinceID), "789" },
					{ nameof(TweetQuery.StartTime), new DateTime(2020, 12, 31, 0, 0, 1).ToString() },
					{ nameof(TweetQuery.TweetFields), "author_id,created_at" },
					{ nameof(TweetQuery.UntilID), "012" },
					{ nameof(TweetQuery.UserFields), "created_at,verified" },
			   };

			Request req = tweetReqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
		public void BuildUrl_ForUserTimeline_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "users/123/tweets?" +
				"end_time=2021-01-01T12%3A59%3A59Z&" +
				"exclude=replies%2Cretweets&" +
				"max_results=50&" +
				"pagination_token=456&" +
				"since_id=789&" +
				"start_time=2020-12-31T00%3A00%3A01Z&" +
				"until_id=012&" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
				"media.fields=height%2Cwidth&" +
				"place.fields=country&" +
				"poll.fields=duration_minutes%2Cend_datetime&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TweetQuery.Type), TweetType.TweetsTimeline.ToString() },
					{ nameof(TweetQuery.EndTime), new DateTime(2021, 1, 1, 12, 59, 59).ToString() },
					{ nameof(TweetQuery.Exclude), TweetExcludes.All },
					{ nameof(TweetQuery.Expansions), "attachments.poll_ids,author_id" },
					{ nameof(TweetQuery.ID), "123" },
					{ nameof(TweetQuery.MaxResults), "50" },
					{ nameof(TweetQuery.MediaFields), "height,width" },
					{ nameof(TweetQuery.PaginationToken), "456" },
					{ nameof(TweetQuery.PlaceFields), "country" },
					{ nameof(TweetQuery.PollFields), "duration_minutes,end_datetime" },
					{ nameof(TweetQuery.SinceID), "789" },
					{ nameof(TweetQuery.StartTime), new DateTime(2020, 12, 31, 0, 0, 1).ToString() },
					{ nameof(TweetQuery.TweetFields), "author_id,created_at" },
					{ nameof(TweetQuery.UntilID), "012" },
					{ nameof(TweetQuery.UserFields), "created_at,verified" },
			   };

			Request req = tweetReqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
        public void BuildUrl_WithNullParameters_Throws()
        {
            var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };

            L2TAssert.Throws<NullReferenceException>(() =>
            {
                tweetReqProc.BuildUrl(null);
            });
        }

		[TestMethod]
        public void BuildUrl_WithSpacesInFields_FixesSpaces()
        {
            const string ExpectedUrl =
                BaseUrl2 + "tweets?" +
                "ids=2%2C3&" +
                "expansions=attachments.poll_ids%2Cauthor_id&" +
                "media.fields=height%2Cwidth&" +
                "place.fields=country&" +
                "poll.fields=duration_minutes%2Cend_datetime&" +
                "tweet.fields=author_id%2Ccreated_at&" +
                "user.fields=created_at%2Cverified";
            var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TweetQuery.Type), TweetType.Lookup.ToString() },
                    { nameof(TweetQuery.Ids), "2, 3" },
                    { nameof(TweetQuery.Expansions), "attachments.poll_ids, author_id" },
                    { nameof(TweetQuery.MediaFields), "height, width" },
                    { nameof(TweetQuery.PlaceFields), "country" },
                    { nameof(TweetQuery.PollFields), "duration_minutes, end_datetime" },
                    { nameof(TweetQuery.TweetFields), "author_id, created_at" },
                    { nameof(TweetQuery.UserFields), "created_at, verified" },
               };

            Request req = tweetReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

		[TestMethod]
		public void BuildUrl_ForList_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "lists/12345/tweets?" +
				"expansions=author_id&" +
				"max_results=50&" +
				"pagination_token=def&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TweetQuery.Type), TweetType.List.ToString() },
					{ nameof(TweetQuery.ListID), "12345" },
					{ nameof(TweetQuery.Expansions), "author_id" },
					{ nameof(TweetQuery.MaxResults), "50" },
					{ nameof(TweetQuery.PaginationToken), "def" },
					{ nameof(TweetQuery.TweetFields), "author_id,created_at" },
					{ nameof(TweetQuery.UserFields), "created_at,verified" },
			   };

			Request req = tweetReqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
		public void BuildUrl_ForPinnedWithSpacesInFields_RemovesSpaces()
		{
			const string ExpectedUrl =
				BaseUrl2 + "lists/12345/tweets?" +
				"expansions=author_id&" +
				"max_results=50&" +
				"pagination_token=def&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TweetQuery.Type), TweetType.List.ToString() },
					{ nameof(TweetQuery.ListID), "12345" },
					{ nameof(TweetQuery.Expansions), "author_id" },
					{ nameof(TweetQuery.MaxResults), "50" },
					{ nameof(TweetQuery.PaginationToken), "def" },
					{ nameof(TweetQuery.TweetFields), "author_id, created_at" },
					{ nameof(TweetQuery.UserFields), "created_at, verified" },
			   };

			Request req = tweetReqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
		public void BuildUrl_ForPinned_RequiresListID()
		{
			var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TweetQuery.Type), TweetType.List.ToString() },
					//{ nameof(TweetQuery.ListID), "1234" },
			   };

			ArgumentException ex =
				L2TAssert.Throws<ArgumentException>(() =>
					tweetReqProc.BuildUrl(parameters));

			Assert.AreEqual(nameof(TweetQuery.ListID), ex.ParamName);
		}

		[TestMethod]
        public void BuildUrl_ForLookup_RequiresIds()
        {
            var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TweetQuery.Type), TweetType.Lookup.ToString() },
                    //{ nameof(Tweet.Ids), null }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
                    tweetReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(TweetQuery.Ids), ex.ParamName);
        }

		[TestMethod]
		public void BuildUrl_ForUserTimeline_RequiresID()
		{
			var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TweetQuery.Type), TweetType.TweetsTimeline.ToString() },
                    //{ nameof(Tweet.ID), null }
                };

			ArgumentException ex =
				L2TAssert.Throws<ArgumentException>(() =>
					tweetReqProc.BuildUrl(parameters));

			Assert.AreEqual(nameof(TweetQuery.ID), ex.ParamName);
		}

		[TestMethod]
		public void BuildUrl_ForMentionsTimeline_RequiresID()
		{
			var tweetReqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TweetQuery.Type), TweetType.MentionsTimeline.ToString() },
                    //{ nameof(Tweet.ID), null }
                };

			ArgumentException ex =
				L2TAssert.Throws<ArgumentException>(() =>
					tweetReqProc.BuildUrl(parameters));

			Assert.AreEqual(nameof(TweetQuery.ID), ex.ParamName);
		}

		[TestMethod]
		public void BuildUrl_ForSpaceTweets_ConstructsUrl()
		{
			const string ExpectedUrl =
				BaseUrl2 + "spaces/345/tweets?" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
				"media.fields=height%2Cwidth&" +
				"place.fields=country&" +
				"poll.fields=duration_minutes%2Cend_datetime&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var reqProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TweetQuery.Type), TweetType.SpaceTweets.ToString() },
					{ nameof(TweetQuery.SpaceID), "345" },
					{ nameof(TweetQuery.Expansions), "attachments.poll_ids,author_id" },
					{ nameof(TweetQuery.MediaFields), "height,width" },
					{ nameof(TweetQuery.PlaceFields), "country" },
					{ nameof(TweetQuery.PollFields), "duration_minutes,end_datetime" },
					{ nameof(TweetQuery.TweetFields), "author_id,created_at" },
					{ nameof(TweetQuery.UserFields), "created_at,verified" }
			   };

			Request req = reqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
        public void ProcessResults_Populates_Tweets()
        {
            var tweetProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };

            List<TweetQuery> results = tweetProc.ProcessResults(SingleTweet);

            Assert.IsNotNull(results);
			TweetQuery tweetQuery = results.SingleOrDefault();
            Assert.IsNotNull(tweetQuery);
            List<Tweet> tweets = tweetQuery.Tweets;
            Assert.IsNotNull(tweets);
            Assert.AreEqual(1, tweets.Count);
            Tweet tweet = tweets.FirstOrDefault();
            Assert.IsNotNull(tweet);
			Assert.AreEqual("en", tweet.Language);
            Assert.AreEqual("Thanks @github for approving sponsorship for LINQ to Twitter: https://t.co/jWeDEN07HN", tweet.Text);
            Assert.AreEqual("1305895383260782593", tweet.ID);
			Assert.AreEqual("15411837", tweet.AuthorID);
			Assert.AreEqual(DateTime.Parse("2020-09-15T15:44:56.000Z").Date, tweet.CreatedAt?.Date);
			Assert.AreEqual("1305895383260782593", tweet.ConversationID);
			Assert.IsTrue(tweet.PossiblySensitive ?? false);
			Assert.AreEqual(tweet.ReplySettings, TweetReplySettings.Everyone);
			Assert.AreEqual("Twitter Web App", tweet.Source);
        }

		[TestMethod]
		public void ProcessResults_Populates_Entities()
		{
			var tweetProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };

			List<TweetQuery> results = tweetProc.ProcessResults(SingleTweet);

			Assert.IsNotNull(results);
			TweetQuery tweetQuery = results.SingleOrDefault();
			Assert.IsNotNull(tweetQuery);

			List<Tweet> tweets = tweetQuery.Tweets;
			Assert.IsNotNull(tweets);
			Assert.AreEqual(1, tweets.Count);
			Tweet tweet = tweets.FirstOrDefault();
			Assert.IsNotNull(tweet);
			TweetEntities entities = tweet.Entities;
			Assert.IsNotNull(entities);

			TweetEntityAnnotation annotation = entities?.Annotations?.FirstOrDefault();
			Assert.IsNotNull(annotation);
			Assert.AreEqual(53, annotation.Start);
			Assert.AreEqual(59, annotation.End);
			Assert.AreEqual(0.5865f, annotation.Probability);
			Assert.AreEqual("Organization", annotation.Type);
			Assert.AreEqual("Twitter", annotation.NormalizedText);

			TweetEntityMention mention = entities?.Mentions?.FirstOrDefault();
			Assert.IsNotNull(mention);
			Assert.AreEqual(14, mention.End);
			Assert.AreEqual("13334762", mention.ID);
			Assert.AreEqual(7, mention.Start);
			Assert.AreEqual("github", mention.Username);

			TweetEntityUrl url = entities?.Urls?.FirstOrDefault();
			Assert.IsNotNull(url);
			Assert.AreEqual(62, url.Start);
			Assert.AreEqual(85, url.End);
            Assert.AreEqual("https://t.co/jWeDEN07HN", url.Url);
			Assert.AreEqual("http://bit.ly/1b2wrHb", url.ExpandedUrl);
			Assert.AreEqual("bit.ly/1b2wrHb", url.DisplayUrl);
			Assert.AreEqual(200, url.Status);
			Assert.AreEqual("GitHub - JoeMayo/LinqToTwitter: LINQ Provider for the Twitter API (C# Twitter Library)", url.Title);
			Assert.AreEqual("LINQ Provider for the Twitter API (C# Twitter Library) - GitHub - JoeMayo/LinqToTwitter: LINQ Provider for the Twitter API (C# Twitter Library)", url.Description);
			Assert.AreEqual("https://github.com/JoeMayo/LinqToTwitter", url.UnwoundUrl);
			List<TweetEntityImage> images = url.Images;
			Assert.IsNotNull(images);
			Assert.AreEqual(2, images.Count);
			TweetEntityImage image = images.First();
			Assert.AreEqual("https://pbs.twimg.com/news_img/1527013178009329664/f45coPtn?format=png&name=orig", image.Url);
			Assert.AreEqual(200, image.Width);
			Assert.AreEqual(200, image.Height);
		}

		[TestMethod]
		public void ProcessResults_Populates_ContextAnnotations()
		{
			var tweetProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };

			List<TweetQuery> results = tweetProc.ProcessResults(SingleTweet);

			Assert.IsNotNull(results);
			TweetQuery tweetQuery = results.SingleOrDefault();
			Assert.IsNotNull(tweetQuery);
			List<Tweet> tweets = tweetQuery.Tweets;
			Assert.IsNotNull(tweets);
			Assert.AreEqual(1, tweets.Count);
			Tweet tweet = tweets.FirstOrDefault();
			Assert.IsNotNull(tweet);

			List<TweetContextAnnotation> annotations = tweet.ContextAnnotations;
			Assert.IsNotNull(annotations);
			Assert.AreEqual(7, annotations.Count);
			TweetContextAnnotation annotation = annotations.First();
			Assert.IsNotNull(annotation);
			TweetContextAnnotationDetails domain = annotation.Domain;
			Assert.IsNotNull(domain);
			Assert.AreEqual("46", domain.ID);
			Assert.AreEqual("Brand Category", domain.Name);
			Assert.AreEqual("Categories within Brand Verticals that narrow down the scope of Brands", domain.Description);
			TweetContextAnnotationDetails entity = annotation.Entity;
			Assert.IsNotNull(entity);
			Assert.AreEqual("781974596752842752", entity.ID);
			Assert.AreEqual("Services", entity.Name);
		}

		[TestMethod]
		public void ProcessResults_Populates_PublicMetrics()
		{
			var tweetProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };

			List<TweetQuery> results = tweetProc.ProcessResults(SingleTweet);

			Assert.IsNotNull(results);
			TweetQuery tweetQuery = results.SingleOrDefault();
			Assert.IsNotNull(tweetQuery);
			List<Tweet> tweets = tweetQuery.Tweets;
			Assert.IsNotNull(tweets);
			Assert.AreEqual(1, tweets.Count);
			Tweet tweet = tweets.FirstOrDefault();
			Assert.IsNotNull(tweet);

			TweetPublicMetrics metrics = tweet.PublicMetrics;
			Assert.IsNotNull(metrics);
			Assert.AreEqual(1, metrics.RetweetCount);
			Assert.AreEqual(1, metrics.ReplyCount);
			Assert.AreEqual(1, metrics.LikeCount);
			Assert.AreEqual(0, metrics.QuoteCount);
		}

		[TestMethod]
		public void ProcessResults_Populates_Includes()
		{
			var tweetProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };

			List<TweetQuery> results = tweetProc.ProcessResults(SingleTweet);

			Assert.IsNotNull(results);
			TweetQuery tweetQuery = results.SingleOrDefault();
			Assert.IsNotNull(tweetQuery);
			TwitterInclude includes = tweetQuery.Includes;
			Assert.IsNotNull(includes);
			List<TwitterUser> users = includes.Users;
			Assert.IsNotNull(users);
			Assert.AreEqual(2, users.Count);
		}

		[TestMethod]
        public void ProcessResults_Handles_Response_With_No_Results()
        {
            var tweetProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = BaseUrl2 };

            List<TweetQuery> results = tweetProc.ProcessResults(ErrorTweet);

            Assert.IsNotNull(results);
			TweetQuery tweetQuery = results.SingleOrDefault();
            Assert.IsNotNull(tweetQuery);
            List<Tweet> tweets = tweetQuery.Tweets;
            Assert.IsNull(tweets);
        }

		[TestMethod]
		public void ProcessResults_PopulatesInputParameters()
		{
			var tweetProc = new TweetRequestProcessor<TweetQuery>()
			{
				BaseUrl = BaseUrl2,
				Type = TweetType.MentionsTimeline,
				ID = "567",
				Ids = "123,456",
				EndTime = new DateTime(2020, 12, 31),
				Exclude = TweetExcludes.Retweets,
				MaxResults = 73,
				Expansions = "123",
				MediaFields = "456",
				PaginationToken = "567",
				PlaceFields = "012",
				PollFields = "345",
				SinceID = "890",
				SpaceID = "345",
				StartTime = new DateTime(2020, 1, 1),
				TweetFields = "678",
				UntilID = "123",
				UserFields = "234"
			};

			var results = tweetProc.ProcessResults(SingleTweet);

			Assert.IsNotNull(results);
			Assert.AreEqual(1, results.Count);
			var tweetQuery = results.Single();
			Assert.IsNotNull(tweetQuery);
			Assert.AreEqual(TweetType.MentionsTimeline, tweetQuery.Type);
			Assert.AreEqual(new DateTime(2020, 12, 31), tweetQuery.EndTime);
			Assert.AreEqual(TweetExcludes.Retweets, tweetQuery.Exclude);
			Assert.AreEqual("123", tweetQuery.Expansions);
			Assert.AreEqual("567", tweetQuery.ID);
			Assert.AreEqual("123,456", tweetQuery.Ids);
			Assert.AreEqual(73, tweetQuery.MaxResults);
			Assert.AreEqual("456", tweetQuery.MediaFields);
			Assert.AreEqual("567", tweetQuery.PaginationToken);
			Assert.AreEqual("012", tweetQuery.PlaceFields);
			Assert.AreEqual("345", tweetQuery.PollFields);
			Assert.AreEqual("890", tweetQuery.SinceID);
			Assert.AreEqual("345", tweetQuery.SpaceID);
			Assert.AreEqual(new DateTime(2020, 1, 1), tweetQuery.StartTime);
			Assert.AreEqual("678", tweetQuery.TweetFields);
			Assert.AreEqual("123", tweetQuery.UntilID);
			Assert.AreEqual("234", tweetQuery.UserFields);
		}

		[TestMethod]
        public void ProcessResults_WithErrors_PopulatesErrorList()
        {
            var tweetProc = new TweetRequestProcessor<TweetQuery> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<TweetQuery> results = tweetProc.ProcessResults(ErrorTweet);

            Assert.IsNotNull(results);
			TweetQuery tweetQuery = results.SingleOrDefault();
            Assert.IsNotNull(tweetQuery);
            List<TwitterError> errors = tweetQuery.Errors;
            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
            TwitterError error = errors.FirstOrDefault();
            Assert.IsNotNull(error);
            Assert.AreEqual("Could not find tweet with ids: [1].", error.Detail);
            Assert.AreEqual("Not Found Error", error.Title);
            Assert.AreEqual("tweet", error.ResourceType);
            Assert.AreEqual("ids", error.Parameter);
            Assert.AreEqual("1", error.Value);
            Assert.AreEqual("https://api.twitter.com/2/problems/resource-not-found", error.Type);
        }

		const string SingleTweet = @"{
	""data"": [
		{
			""context_annotations"": [
				{
					""domain"": {
						""id"": ""46"",
						""name"": ""Brand Category"",
						""description"": ""Categories within Brand Verticals that narrow down the scope of Brands""
					},
					""entity"": {
						""id"": ""781974596752842752"",
						""name"": ""Services""
					}
				},
				{
					""domain"": {
						""id"": ""47"",
						""name"": ""Brand"",
						""description"": ""Brands and Companies""
					},
					""entity"": {
						""id"": ""10045225402"",
						""name"": ""Twitter""
					}
				},
				{
					""domain"": {
						""id"": ""30"",
						""name"": ""Entities [Entity Service]"",
						""description"": ""Entity Service top level domain, every item that is in Entity Service should be in this domain""
					},
					""entity"": {
						""id"": ""848920371311001600"",
						""name"": ""Technology"",
						""description"": ""Technology and computing""
					}
				},
				{
					""domain"": {
						""id"": ""66"",
						""name"": ""Interests and Hobbies Category"",
						""description"": ""A grouping of interests and hobbies entities, like Novelty Food or Destinations""
					},
					""entity"": {
						""id"": ""848921413196984320"",
						""name"": ""Computer programming"",
						""description"": ""Computer programming""
					}
				},
				{
					""domain"": {
						""id"": ""45"",
						""name"": ""Brand Vertical"",
						""description"": ""Top level entities that describe a Brands industry""
					},
					""entity"": {
						""id"": ""781974597226799105"",
						""name"": ""B2B""
					}
				},
				{
					""domain"": {
						""id"": ""46"",
						""name"": ""Brand Category"",
						""description"": ""Categories within Brand Verticals that narrow down the scope of Brands""
					},
					""entity"": {
						""id"": ""781974597172203520"",
						""name"": ""Services""
					}
				},
				{
					""domain"": {
						""id"": ""47"",
						""name"": ""Brand"",
						""description"": ""Brands and Companies""
					},
					""entity"": {
						""id"": ""10040692468"",
						""name"": ""GitHub""
					}
				}
			],
			""reply_settings"": ""everyone"",
			""id"": ""1305895383260782593"",
			""source"": ""Twitter Web App"",
			""public_metrics"": {
				""retweet_count"": 1,
				""reply_count"": 1,
				""like_count"": 1,
				""quote_count"": 0
			},
			""lang"": ""en"",
			""entities"": {
				""annotations"": [
					{
						""start"": 53,
						""end"": 59,
						""probability"": 0.5865,
						""type"": ""Organization"",
						""normalized_text"": ""Twitter""
					}
				],
				""urls"": [
					{
						""start"": 62,
						""end"": 85,
						""url"": ""https://t.co/jWeDEN07HN"",
						""expanded_url"": ""http://bit.ly/1b2wrHb"",
						""display_url"": ""bit.ly/1b2wrHb"",
						""images"": [
							{
								""url"": ""https://pbs.twimg.com/news_img/1527013178009329664/f45coPtn?format=png&name=orig"",
								""width"": 200,
								""height"": 200
							},
							{
								""url"": ""https://pbs.twimg.com/news_img/1527013178009329664/f45coPtn?format=png&name=150x150"",
								""width"": 150,
								""height"": 150
							}
						],
						""status"": 200,
						""title"": ""GitHub - JoeMayo/LinqToTwitter: LINQ Provider for the Twitter API (C# Twitter Library)"",
						""description"": ""LINQ Provider for the Twitter API (C# Twitter Library) - GitHub - JoeMayo/LinqToTwitter: LINQ Provider for the Twitter API (C# Twitter Library)"",
						""unwound_url"": ""https://github.com/JoeMayo/LinqToTwitter""
					}
				],
				""mentions"": [
					{
						""start"": 7,
						""end"": 14,
						""username"": ""github"",
						""id"": ""13334762""
					}
				]
			},
			""text"": ""Thanks @github for approving sponsorship for LINQ to Twitter: https://t.co/jWeDEN07HN"",
			""conversation_id"": ""1305895383260782593"",
			""author_id"": ""15411837"",
			""possibly_sensitive"": true,
			""created_at"": ""2020-09-15T15:44:56.000Z""
		}
	],
	""includes"": {
		""users"": [
			{
				""verified"": false,
				""username"": ""JoeMayo"",
				""created_at"": ""2008-07-13T04:35:50.000Z"",
				""description"": ""Author, Instructor, and Independent Consultant \n\nNewest Release: C# Cookbook (https://t.co/acNTiAe6HQ)\n\n#AI #Chatbots #CSharp #Linq2Twitter #NLP"",
				""public_metrics"": {
					""followers_count"": 10778,
					""following_count"": 4193,
					""tweet_count"": 4226,
					""listed_count"": 271
				},
				""url"": ""https://t.co/Pd3XsnJwJU"",
				""pinned_tweet_id"": ""1516074915803254788"",
				""protected"": false,
				""name"": ""Joe Mayo"",
				""profile_image_url"": ""https://pbs.twimg.com/profile_images/1185764990403268613/8GoXoOtz_normal.jpg"",
				""location"": ""Las Vegas, NV"",
				""entities"": {
					""url"": {
						""urls"": [
							{
								""start"": 0,
								""end"": 23,
								""url"": ""https://t.co/Pd3XsnJwJU"",
								""expanded_url"": ""https://joemayo.medium.com"",
								""display_url"": ""joemayo.medium.com""
							}
						]
					},
					""description"": {
						""urls"": [
							{
								""start"": 78,
								""end"": 101,
								""url"": ""https://t.co/acNTiAe6HQ"",
								""expanded_url"": ""http://bit.ly/CSharpCookbook"",
								""display_url"": ""bit.ly/CSharpCookbook""
							}
						],
						""hashtags"": [
							{
								""start"": 104,
								""end"": 107,
								""tag"": ""AI""
							},
							{
								""start"": 108,
								""end"": 117,
								""tag"": ""Chatbots""
							},
							{
								""start"": 118,
								""end"": 125,
								""tag"": ""CSharp""
							},
							{
								""start"": 126,
								""end"": 139,
								""tag"": ""Linq2Twitter""
							},
							{
								""start"": 140,
								""end"": 144,
								""tag"": ""NLP""
							}
						]
					}
				},
				""id"": ""15411837""
			},
			{
				""verified"": true,
				""username"": ""github"",
				""created_at"": ""2008-02-11T04:41:50.000Z"",
				""description"": ""How people build software. \n\nNeed help? Send us a message at https://t.co/aspNQGzzZH for support."",
				""public_metrics"": {
					""followers_count"": 2277687,
					""following_count"": 334,
					""tweet_count"": 7061,
					""listed_count"": 17306
				},
				""url"": """",
				""protected"": false,
				""name"": ""GitHub"",
				""profile_image_url"": ""https://pbs.twimg.com/profile_images/1414990564408262661/r6YemvF9_normal.jpg"",
				""location"": ""San Francisco, CA"",
				""entities"": {
					""description"": {
						""urls"": [
							{
								""start"": 61,
								""end"": 84,
								""url"": ""https://t.co/aspNQGzzZH"",
								""expanded_url"": ""https://support.github.com"",
								""display_url"": ""support.github.com""
							}
						]
					}
				},
				""id"": ""13334762""
			}
		]
	}
}";

		const string ErrorTweet = @"{
	""errors"": [
		{
			""detail"": ""Could not find tweet with ids: [1]."",
			""title"": ""Not Found Error"",
			""resource_type"": ""tweet"",
			""parameter"": ""ids"",
			""value"": ""1"",
			""type"": ""https://api.twitter.com/2/problems/resource-not-found""
		}
	]
}";
    }
}
