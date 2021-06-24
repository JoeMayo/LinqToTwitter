using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter.Tests.BlocksTests
{
    [TestClass]
    public class LikeRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public LikeRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_ParsesParameters()
        {
            var likeReqProc = new LikeRequestProcessor<LikeQuery>();
            Expression<Func<LikeQuery, bool>> expression =
                like =>
                    like.Type == LikeType.Lookup &&
                    like.Expansions == "attachments.poll_ids,author_id" &&
                    like.ID == "123" &&
                    like.MaxResults == 99 &&
                    like.MediaFields == "height,width" &&
                    like.PaginationToken == "456" &&
                    like.PlaceFields == "country" &&
                    like.PollFields == "duration_minutes,end_datetime" &&
                    like.TweetFields == "author_id,created_at" &&
                    like.UserFields == "created_at,verified";
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = likeReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(LikeQuery.Type), ((int)LikeType.Lookup).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(LikeQuery.Expansions), "attachments.poll_ids,author_id")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(LikeQuery.ID), "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(LikeQuery.MaxResults), "99")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(LikeQuery.MediaFields), "height,width")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(LikeQuery.PaginationToken), "456")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(LikeQuery.PlaceFields), "country")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(LikeQuery.PollFields), "duration_minutes,end_datetime")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(LikeQuery.TweetFields), "author_id,created_at")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(LikeQuery.UserFields), "created_at,verified")));
        }

        [TestMethod]
        public void BuildUrl_WithUserID_CreatesLookupUrl()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/123/liked_tweets?" +
                "expansions=attachments.poll_ids%2Cauthor_id&" +
                "max_results=99&" +
                "media.fields=height%2Cwidth&" +
                "pagination_token=456&" +
                "place.fields=country&" +
                "poll.fields=duration_minutes%2Cend_datetime&" +
                "tweet.fields=author_id%2Ccreated_at&" +
                "user.fields=created_at%2Cverified";

            var likeReqProc = new LikeRequestProcessor<LikeQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(LikeQuery.Type), ((int)LikeType.Lookup).ToString(CultureInfo.InvariantCulture) },
                    { nameof(LikeQuery.ID), "123" },
                    { nameof(LikeQuery.Expansions), "attachments.poll_ids,author_id" },
                    { nameof(LikeQuery.MaxResults), "99" },
                    { nameof(LikeQuery.MediaFields), "height,width" },
                    { nameof(LikeQuery.PaginationToken), "456" },
                    { nameof(LikeQuery.PlaceFields), "country" },
                    { nameof(LikeQuery.PollFields), "duration_minutes,end_datetime" },
                    { nameof(LikeQuery.TweetFields), "author_id,created_at" },
                    { nameof(LikeQuery.UserFields), "created_at,verified" }
                };

            Request req = likeReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_MissingTypeParameter_Throw()
        {
            var likeReqProc = new LikeRequestProcessor<LikeQuery> { BaseUrl = BaseUrl2 };
            var parameters = new Dictionary<string, string>();

            var ex = L2TAssert.Throws<ArgumentException>(() => likeReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(Type), ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_WithNullParameter_Throws()
        {
            var likeReqProc = new LikeRequestProcessor<LikeQuery> { BaseUrl = BaseUrl2 };

            var ex = L2TAssert.Throws<ArgumentException>(() => likeReqProc.BuildUrl(null));

            Assert.AreEqual(nameof(Type), ex.ParamName);
        }

        [TestMethod]
        public void ProcessResults_WithLikes_HandlesResults()
        {
            var likeReqProc = new LikeRequestProcessor<LikeQuery>
            {
                Type = LikeType.Lookup
            };

            IList actual = likeReqProc.ProcessResults(LikesJson);

            var actualQuery = actual as IList<LikeQuery>;
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(4, actualQuery[0].Tweets.Count);
        }

        [TestMethod]
        public void ProcessResults_WithEmptyResults_ReturnsEmptyList()
        {
            var likeReqProc = new LikeRequestProcessor<LikeQuery> 
            { 
                BaseUrl = BaseUrl2,
                Type = LikeType.Lookup
            };

            var likes = likeReqProc.ProcessResults(NoLikes);

            Assert.AreEqual(1, likes.Count);
        }

        [TestMethod]
        public void ProcessResults_RetainsOriginalInputParameters()
        {
            var likeReqProc = new LikeRequestProcessor<LikeQuery>
            {
                Type = LikeType.Lookup,
                Expansions = "attachments.poll_ids,author_id",
                ID = "123",
                MaxResults = 99,
                MediaFields = "456",
                PaginationToken = "567",
                PlaceFields = "012",
                PollFields = "345",
                TweetFields = "author_id,created_at",
                UserFields = "created_at,verified"
            };

            var likes = likeReqProc.ProcessResults(LikesJson);

            Assert.IsNotNull(likes);
            Assert.IsNotNull(likes.SingleOrDefault());
            var like = likes.Single();
            Assert.AreEqual(LikeType.Lookup, like.Type);
            Assert.AreEqual("attachments.poll_ids,author_id", like.Expansions);
            Assert.AreEqual("123", like.ID);
            Assert.AreEqual(99, like.MaxResults);
            Assert.AreEqual("456", like.MediaFields);
            Assert.AreEqual("567", like.PaginationToken);
            Assert.AreEqual("012", like.PlaceFields);
            Assert.AreEqual("345", like.PollFields);
            Assert.AreEqual("author_id,created_at", like.TweetFields);
            Assert.AreEqual("created_at,verified", like.UserFields);
        }

        const string NoLikes = @"{""meta"":{""result_count"":0}}";

        const string LikesJson = @"{
	""data"": [
	{
	""id"": ""1399921020669022210"",
			""attachments"": {
		""media_keys"": [
					""3_1399919388547903488""
				]
			},
			""context_annotations"": [
				{
		""domain"": {
			""id"": ""67"",
						""name"": ""Interests and Hobbies"",
						""description"": ""Interests, opinions, and behaviors of individuals, groups, or cultures; like Speciality Cooking or Theme Parks""
					},
					""entity"": {
			""id"": ""1359865633672859649"",
						""name"": ""Upper body fitness""
					}
	},
				{
		""domain"": {
			""id"": ""65"",
						""name"": ""Interests and Hobbies Vertical"",
						""description"": ""Top level interests and hobbies groupings, like Food or Travel""
					},
					""entity"": {
			""id"": ""847868745150119936"",
						""name"": ""Home & family"",
						""description"": ""Hobbies and interests""
					}
	},
				{
		""domain"": {
			""id"": ""65"",
						""name"": ""Interests and Hobbies Vertical"",
						""description"": ""Top level interests and hobbies groupings, like Food or Travel""
					},
					""entity"": {
			""id"": ""872553326499422208"",
						""name"": ""Fitness"",
						""description"": ""Fitness""
					}
	},
				{
		""domain"": {
			""id"": ""66"",
						""name"": ""Interests and Hobbies Category"",
						""description"": ""A grouping of interests and hobbies entities, like Novelty Food or Destinations""
					},
					""entity"": {
			""id"": ""872554146750017536"",
						""name"": ""Exercises"",
						""description"": ""Exercises""
					}
	}
			],
			""entities"": {
		""urls"": [
					{
			""start"": 172,
						""end"": 195,
						""url"": ""https://t.co/lxI08XTVaW"",
						""expanded_url"": ""https://github.com/Azure/bicep#get-started-with-bicep"",
						""display_url"": ""github.com/Azure/bicep#ge…"",
						""images"": [
							{
				""url"": ""https://pbs.twimg.com/news_img/1405272347951603714/TqGIykgy?format=jpg&name=orig"",
								""width"": 1200,
								""height"": 600
							},
							{
				""url"": ""https://pbs.twimg.com/news_img/1405272347951603714/TqGIykgy?format=jpg&name=150x150"",
								""width"": 150,
								""height"": 150
							}
						],
						""status"": 200,
						""title"": ""Azure/bicep"",
						""description"": ""Bicep is a declarative language for describing and deploying Azure resources - Azure/bicep"",
						""unwound_url"": ""https://github.com/Azure/bicep#get-started-with-bicep""
					},
					{
			""start"": 196,
						""end"": 219,
						""url"": ""https://t.co/3zXjNOhv8w"",
						""expanded_url"": ""https://twitter.com/jongallant/status/1399921020669022210/photo/1"",
						""display_url"": ""pic.twitter.com/3zXjNOhv8w""
					},
					{
			""start"": 220,
						""end"": 243,
						""url"": ""https://t.co/AooS3lvcDf"",
						""expanded_url"": ""https://twitter.com/BicepLang/status/1399799768021278721"",
						""display_url"": ""twitter.com/BicepLang/stat…""
					}
				],
				""annotations"": [
					{
			""start"": 43,
						""end"": 51,
						""probability"": 0.6501,
						""type"": ""Organization"",
						""normalized_text"": ""Gym Azure""
					}
				],
				""mentions"": [
					{
			""start"": 83,
						""end"": 93,
						""username"": ""BicepLang""
					}
				]
			},
			""conversation_id"": ""1399921020669022210"",
			""possibly_sensitive"": false,
			""referenced_tweets"": [
				{
		""type"": ""quoted"",
					""id"": ""1399799768021278721""
				}
			],
			""author_id"": ""14631076"",
			""text"": ""Every day is bicep day when you workout at Gym Azure. 🦾\n\nToday I'm flexing the new @BicepLang linter to help guide me towards having better toned biceps.\n\nGet ripped today https://t.co/lxI08XTVaW https://t.co/3zXjNOhv8w https://t.co/AooS3lvcDf"",
			""reply_settings"": ""everyone"",
			""lang"": ""en"",
			""source"": ""Twitter Web App"",
			""public_metrics"": {
		""retweet_count"": 1,
				""reply_count"": 2,
				""like_count"": 25,
				""quote_count"": 0
			},
			""created_at"": ""2021-06-02T02:49:15.000Z""
		},
		{
	""id"": ""1398741415807664129"",
			""attachments"": {
		""media_keys"": [
					""3_1398724015984947200""
				]
			},
			""context_annotations"": [
				{
		""domain"": {
			""id"": ""123"",
						""name"": ""Ongoing News Story"",
						""description"": ""Ongoing News Stories like 'Brexit'""
					},
					""entity"": {
			""id"": ""1220701888179359745"",
						""name"": ""COVID-19""
					}
	},
				{
		""domain"": {
			""id"": ""66"",
						""name"": ""Interests and Hobbies Category"",
						""description"": ""A grouping of interests and hobbies entities, like Novelty Food or Destinations""
					},
					""entity"": {
			""id"": ""852219717016141824"",
						""name"": ""College life"",
						""description"": ""College Life""
					}
	}
			],
			""entities"": {
		""urls"": [
					{
			""start"": 278,
						""end"": 301,
						""url"": ""https://t.co/77Ezx4y5Ez"",
						""expanded_url"": ""https://twitter.com/AtlCodeCamp/status/1398741415807664129/photo/1"",
						""display_url"": ""pic.twitter.com/77Ezx4y5Ez""
					}
				],
				""hashtags"": [
					{
			""start"": 265,
						""end"": 277,
						""tag"": ""ATLCodeCamp""
					}
				],
				""annotations"": [
					{
			""start"": 0,
						""end"": 16,
						""probability"": 0.7556,
						""type"": ""Organization"",
						""normalized_text"": ""Atlanta Code Camp""
					},
					{
			""start"": 65,
						""end"": 98,
						""probability"": 0.9565,
						""type"": ""Organization"",
						""normalized_text"": ""Kennesaw State University Marietta""
					}
				]
			},
			""conversation_id"": ""1398741415807664129"",
			""possibly_sensitive"": false,
			""author_id"": ""1598668740"",
			""text"": ""Atlanta Code Camp 2021 will take place on Oct. 9th, 2021, at the Kennesaw State University Marietta campus. \n\nWe hope the world continues making progress in the recovery from the COVID-19 pandemic and that we can be back with our friends and colleagues this year.\n\n#ATLCodeCamp https://t.co/77Ezx4y5Ez"",
			""reply_settings"": ""everyone"",
			""lang"": ""en"",
			""source"": ""TweetDeck"",
			""public_metrics"": {
		""retweet_count"": 12,
				""reply_count"": 1,
				""like_count"": 17,
				""quote_count"": 1
			},
			""created_at"": ""2021-05-29T20:41:55.000Z""
		},
		{
	""id"": ""1397685862981308420"",
			""attachments"": {
		""media_keys"": [
					""3_1397685860414410755""
				]
			},
			""entities"": {
		""urls"": [
					{
			""start"": 17,
						""end"": 40,
						""url"": ""https://t.co/x9DBDjooFW"",
						""expanded_url"": ""https://twitter.com/Madelie90476411/status/1397685862981308420/photo/1"",
						""display_url"": ""pic.twitter.com/x9DBDjooFW""
					}
				]
			},
			""conversation_id"": ""1397685862981308420"",
			""possibly_sensitive"": false,
			""author_id"": ""1388658349810593793"",
			""text"": ""Kind reminder... https://t.co/x9DBDjooFW"",
			""reply_settings"": ""everyone"",
			""lang"": ""en"",
			""source"": ""Twitter for Android"",
			""public_metrics"": {
		""retweet_count"": 15,
				""reply_count"": 0,
				""like_count"": 56,
				""quote_count"": 3
			},
			""created_at"": ""2021-05-26T22:47:32.000Z""
		},
		{
	""id"": ""1380279280211537928"",
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
			""id"": ""898673391980261376"",
						""name"": ""Web development"",
						""description"": ""Web Development""
					}
	}
			],
			""entities"": {
		""urls"": [
					{
			""start"": 143,
						""end"": 166,
						""url"": ""https://t.co/kv0SIogtJb"",
						""expanded_url"": ""https://twitter.com/JoeMayo/status/1380211819676016643"",
						""display_url"": ""twitter.com/JoeMayo/status…""
					}
				],
				""hashtags"": [
					{
			""start"": 34,
						""end"": 45,
						""tag"": ""twitterapi""
					},
					{
			""start"": 121,
						""end"": 128,
						""tag"": ""dotnet""
					},
					{
			""start"": 129,
						""end"": 142,
						""tag"": ""linq2twitter""
					}
				],
				""annotations"": [
					{
			""start"": 0,
						""end"": 2,
						""probability"": 0.7058,
						""type"": ""Person"",
						""normalized_text"": ""Joe""
					}
				],
				""mentions"": [
					{
			""start"": 6,
						""end"": 17,
						""username"": ""Linq2Twitr""
					}
				]
			},
			""conversation_id"": ""1380279280211537928"",
			""possibly_sensitive"": false,
			""referenced_tweets"": [
				{
		""type"": ""quoted"",
					""id"": ""1380211819676016643""
				}
			],
			""author_id"": ""786491"",
			""text"": ""Joe's @Linq2Twitr library is 🔥 on #twitterapi v2! Thank you! dotNet devs get the latest API features, first, right here! #dotnet #linq2twitter https://t.co/kv0SIogtJb"",
			""reply_settings"": ""everyone"",
			""lang"": ""en"",
			""source"": ""Twitter Web App"",
			""public_metrics"": {
		""retweet_count"": 6,
				""reply_count"": 1,
				""like_count"": 6,
				""quote_count"": 0
			},
			""created_at"": ""2021-04-08T21:59:59.000Z""
		}
	],
	""meta"": {
	""result_count"": 20,
		""next_token"": ""7140dibdnow9c7btw45446skphfy2d9i9rkwfd2qe4q46""
	}
}";
    }
}
