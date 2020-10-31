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

            Expression<Func<TweetQuery, bool>> expression =
                tweet =>
                    tweet.Type == TweetType.Lookup &&
                    tweet.Ids == "2,3" &&
                    tweet.Expansions == "attachments.poll_ids,author_id" &&
                    tweet.MediaFields == "height,width" &&
                    tweet.PlaceFields == "country" &&
                    tweet.PollFields == "duration_minutes,end_datetime" &&
                    tweet.TweetFields == "author_id,created_at" &&
                    tweet.UserFields == "created_at,verified";

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.Type), ((int)TweetType.Lookup).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.Ids), "2,3")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.Expansions), "attachments.poll_ids,author_id")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.MediaFields), "height,width")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(TweetQuery.PlaceFields), "country")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(TweetQuery.PollFields), "duration_minutes,end_datetime")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.TweetFields), "author_id,created_at")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TweetQuery.UserFields), "created_at,verified")));
        }

        [TestMethod]
        public void BuildUrl_Includes_Parameters()
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
        public void BuildUrl_Throws_When_Parameters_Null()
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
        public void BuildUrl_Requires_Ids()
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
			Assert.AreEqual(7, mention.Start);
			Assert.AreEqual(14, mention.End);
			Assert.AreEqual("github", mention.Username);

			TweetEntityUrl url = entities?.Urls?.FirstOrDefault();
			Assert.IsNotNull(url);
			Assert.AreEqual(62, url.Start);
			Assert.AreEqual(85, url.End);
            Assert.AreEqual("https://t.co/jWeDEN07HN", url.Url);
			Assert.AreEqual("http://bit.ly/1b2wrHb", url.ExpandedUrl);
			Assert.AreEqual("bit.ly/1b2wrHb", url.DisplayUrl);
			Assert.AreEqual(200, url.Status);
			Assert.AreEqual("JoeMayo/LinqToTwitter", url.Title);
			Assert.AreEqual("LINQ Provider for the Twitter API (Twitter Library) - JoeMayo/LinqToTwitter", url.Description);
			Assert.AreEqual("https://github.com/JoeMayo/LinqToTwitter", url.UnwoundUrl);
			List<TweetEntityImage> images = url.Images;
			Assert.IsNotNull(images);
			Assert.AreEqual(2, images.Count);
			TweetEntityImage image = images.First();
			Assert.AreEqual("https://pbs.twimg.com/news_img/1321470110356287488/pWdfwBk5?format=png&name=orig", image.Url);
			Assert.AreEqual(100, image.Width);
			Assert.AreEqual(100, image.Height);
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
        public void ProcessResults_Populates_Input_Parameters()
        {
            var searchProc = new TweetRequestProcessor<TweetQuery>()
            {
                BaseUrl = BaseUrl2,
                Type = TweetType.Lookup,
				Ids = "3,7",
                Expansions = "123",
                MediaFields = "456",
                PlaceFields = "012",
                PollFields = "345",
                TweetFields = "678",
                UserFields = "234"
            };

            var results = searchProc.ProcessResults(SingleTweet);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            var tweetQuery = results.Single();
            Assert.IsNotNull(tweetQuery);
            Assert.AreEqual(TweetType.Lookup, tweetQuery.Type);
			Assert.AreEqual("3,7", tweetQuery.Ids);
            Assert.AreEqual("123", tweetQuery.Expansions);
            Assert.AreEqual("456", tweetQuery.MediaFields);
            Assert.AreEqual("012", tweetQuery.PlaceFields);
            Assert.AreEqual("345", tweetQuery.PollFields);
            Assert.AreEqual("678", tweetQuery.TweetFields);
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

        #region Single Tweet

        const string SingleTweet = @"{
	""data"": [
		{
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
				""mentions"": [
					{
						""start"": 7,
						""end"": 14,
						""username"": ""github""
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
								""url"": ""https://pbs.twimg.com/news_img/1321470110356287488/pWdfwBk5?format=png&name=orig"",
								""width"": 100,
								""height"": 100
							},
							{
	""url"": ""https://pbs.twimg.com/news_img/1321470110356287488/pWdfwBk5?format=png&name=150x150"",
								""width"": 100,
								""height"": 100
							}
						],
						""status"": 200,
						""title"": ""JoeMayo/LinqToTwitter"",
						""description"": ""LINQ Provider for the Twitter API (Twitter Library) - JoeMayo/LinqToTwitter"",
						""unwound_url"": ""https://github.com/JoeMayo/LinqToTwitter""
					}
				]
			},
			""text"": ""Thanks @github for approving sponsorship for LINQ to Twitter: https://t.co/jWeDEN07HN"",
			""id"": ""1305895383260782593"",
			""author_id"": ""15411837"",
			""created_at"": ""2020-09-15T15:44:56.000Z"",
			""conversation_id"": ""1305895383260782593"",
			""possibly_sensitive"": true,
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
		""id"": ""65"",
						""name"": ""Interests and Hobbies Vertical"",
						""description"": ""Top level interests and hobbies groupings, like Food or Travel""
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
			""source"": ""Twitter Web App"",
			""public_metrics"": {
			""retweet_count"": 1,
				""reply_count"": 1,
				""like_count"": 1,
				""quote_count"": 0
			}
		}
	],
	""includes"": {
	""users"": [
			{
		""username"": ""JoeMayo"",
				""protected"": false,
				""name"": ""Joe Mayo"",
				""profile_image_url"": ""https://pbs.twimg.com/profile_images/1185764990403268613/8GoXoOtz_normal.jpg"",
				""id"": ""15411837"",
				""public_metrics"": {
			""followers_count"": 10023,
					""following_count"": 3783,
					""tweet_count"": 3797,
					""listed_count"": 269
				},
				""verified"": false,
				""description"": ""Author, Instructor, and Independent Consultant. Author of Programming the Microsoft Bot Framework/MS Press.\n#ai #chatbots #csharp #linq2twitter #twitterapi"",
				""created_at"": ""2008-07-13T04:35:50.000Z"",
				""location"": ""Las Vegas, NV"",
				""url"": ""https://t.co/Y6dXyWxanS"",
				""entities"": {
			""url"": {
				""urls"": [
							{
					""start"": 0,
								""end"": 23,
								""url"": ""https://t.co/Y6dXyWxanS"",
								""expanded_url"": ""https://github.com/JoeMayo"",
								""display_url"": ""github.com/JoeMayo""
							}
						]
					},
					""description"": {
				""hashtags"": [
							{
					""start"": 108,
								""end"": 111,
								""tag"": ""ai""
							},
							{
					""start"": 112,
								""end"": 121,
								""tag"": ""chatbots""
							},
							{
					""start"": 122,
								""end"": 129,
								""tag"": ""csharp""
							},
							{
					""start"": 130,
								""end"": 143,
								""tag"": ""linq2twitter""
							},
							{
					""start"": 144,
								""end"": 155,
								""tag"": ""twitterapi""
							}
						]
					}
		}
	},
			{
		""username"": ""github"",
				""protected"": false,
				""name"": ""GitHub"",
				""profile_image_url"": ""https://pbs.twimg.com/profile_images/1157035760085684224/iuxTnT5g_normal.jpg"",
				""id"": ""13334762"",
				""public_metrics"": {
			""followers_count"": 1997039,
					""following_count"": 311,
					""tweet_count"": 5517,
					""listed_count"": 15474
				},
				""verified"": true,
				""description"": ""How people build software. \n\nNeed help? Send us a message at https://t.co/YU5nzbpDIg for support."",
				""created_at"": ""2008-02-11T04:41:50.000Z"",
				""location"": ""San Francisco, CA"",
				""url"": """",
				""entities"": {
			""description"": {
				""urls"": [
							{
					""start"": 61,
								""end"": 84,
								""url"": ""https://t.co/YU5nzbpDIg"",
								""expanded_url"": ""http://git.io/c"",
								""display_url"": ""git.io/c""
							}
						]
					}
		}
	}
		]
	}
}";

		#endregion

		#region Error Tweet

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

        #endregion
    }
}
