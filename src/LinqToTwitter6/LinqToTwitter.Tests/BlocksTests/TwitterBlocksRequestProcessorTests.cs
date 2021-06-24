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
    public class TwitterBlocksRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public TwitterBlocksRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_ParsesParameters()
        {
            var blocksReqProc = new TwitterBlocksRequestProcessor<TwitterBlocksQuery>();
            Expression<Func<TwitterBlocksQuery, bool>> expression =
                block =>
                    block.Type == BlockingType.Lookup &&
                    block.Expansions == "attachments.poll_ids,author_id" &&
                    block.ID == "123" &&
                    block.MaxResults == 99 &&
                    block.PaginationToken == "456" &&
                    block.TweetFields == "author_id,created_at" &&
                    block.UserFields == "created_at,verified";
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = blocksReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterBlocksQuery.Type), ((int)BlockingType.Lookup).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterBlocksQuery.Expansions), "attachments.poll_ids,author_id")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterBlocksQuery.ID), "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterBlocksQuery.MaxResults), "99")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterBlocksQuery.PaginationToken), "456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterBlocksQuery.TweetFields), "author_id,created_at")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterBlocksQuery.UserFields), "created_at,verified")));
        }

        [TestMethod]
        public void BuildUrl_WithUserID_CreatesLookupUrl()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/123/blocking?" +
                "expansions=attachments.poll_ids%2Cauthor_id&" +
                "max_results=99&" +
                "pagination_token=456&" +
                "tweet.fields=author_id%2Ccreated_at&" +
                "user.fields=created_at%2Cverified";

            var blocksReqProc = new TwitterBlocksRequestProcessor<TwitterBlocksQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TwitterBlocksQuery.Type), ((int)BlockingType.Lookup).ToString(CultureInfo.InvariantCulture) },
                    { nameof(TwitterBlocksQuery.ID), "123" },
                    { nameof(TwitterBlocksQuery.Expansions), "attachments.poll_ids,author_id" },
                    { nameof(TwitterBlocksQuery.MaxResults), "99" },
                    { nameof(TwitterBlocksQuery.PaginationToken), "456" },
                    { nameof(TwitterBlocksQuery.TweetFields), "author_id,created_at" },
                    { nameof(TwitterBlocksQuery.UserFields), "created_at,verified" }
                };

            Request req = blocksReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_MissingTypeParameter_Throw()
        {
            var blockReqProc = new TwitterBlocksRequestProcessor<TwitterBlocksQuery> { BaseUrl = BaseUrl2 };
            var parameters = new Dictionary<string, string>();

            var ex = L2TAssert.Throws<ArgumentException>(() => blockReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(Type), ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_WithNullParameter_Throws()
        {
            var blockReqProc = new TwitterBlocksRequestProcessor<Blocks> { BaseUrl = BaseUrl2 };

            var ex = L2TAssert.Throws<ArgumentException>(() => blockReqProc.BuildUrl(null));

            Assert.AreEqual(nameof(Type), ex.ParamName);
        }

        [TestMethod]
        public void ProcessResults_WithBlockedUsers_HandlesResults()
        {
            var blockedReqProc = new TwitterBlocksRequestProcessor<TwitterBlocksQuery>
            {
                Type = BlockingType.Lookup
            };

            IList actual = blockedReqProc.ProcessResults(BlockedUsersJson);

            var actualQuery = actual as IList<TwitterBlocksQuery>;
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery[0].Users.Count, 1);
        }

        [TestMethod]
        public void ProcessResults_WithEmptyResults_ReturnsEmptyList()
        {
            var blocksReqProc = new TwitterBlocksRequestProcessor<TwitterBlocksQuery> 
            { 
                BaseUrl = BaseUrl2,
                Type = BlockingType.Lookup
            };

            var blocks = blocksReqProc.ProcessResults(NoBlockedUsers);

            Assert.AreEqual(1, blocks.Count);
        }

        [TestMethod]
        public void ProcessResults_RetainsOriginalInputParameters()
        {
            var blockedReqProc = new TwitterBlocksRequestProcessor<TwitterBlocksQuery>
            {
                Type = BlockingType.Lookup,
                Expansions = "attachments.poll_ids,author_id",
                ID = "123",
                MaxResults = 99,
                PaginationToken = "456",
                TweetFields = "author_id,created_at",
                UserFields = "created_at,verified"
            };

            var blocks = blockedReqProc.ProcessResults(BlockedUsersJson);

            Assert.IsNotNull(blocks);
            Assert.IsNotNull(blocks.SingleOrDefault());
            var block = blocks.Single();
            Assert.AreEqual(BlockingType.Lookup, block.Type);
            Assert.AreEqual("attachments.poll_ids,author_id", block.Expansions);
            Assert.AreEqual("123", block.ID);
            Assert.AreEqual(99, block.MaxResults);
            Assert.AreEqual("456", block.PaginationToken);
            Assert.AreEqual("author_id,created_at", block.TweetFields);
            Assert.AreEqual("created_at,verified", block.UserFields);
        }

        const string NoBlockedUsers = @"{""meta"":{""result_count"":0}}";

        const string BlockedUsersJson = @"{
	""data"": [
		{
			""profile_image_url"": ""https://pbs.twimg.com/profile_images/1195087456548401166/kmi8U-M9_normal.jpg"",
			""verified"": true,
			""location"": ""Kansas City, MO"",
			""created_at"": ""2007-11-27T23:50:12.000Z"",
			""entities"": {
				""url"": {
					""urls"": [
						{
							""start"": 0,
							""end"": 23,
							""url"": ""https://t.co/4jenbcenp0"",
							""expanded_url"": ""http://hrblock.io/WaysToFile"",
							""display_url"": ""hrblock.io/WaysToFile""

                        }
					]
				},
				""description"": {
    ""mentions"": [

                        {
        ""start"": 117,
							""end"": 132,
							""username"": ""HRBlockAnswers""

                        }
					]
				}
			},
			""username"": ""HRBlock"",
			""description"": ""At H&R Block, we have many filing options to make it easy for you. Expert tax prep in person or virtually. \n\nContact @HRBlockAnswers for support."",
			""id"": ""10673252"",
			""public_metrics"": {
    ""followers_count"": 39286,
				""following_count"": 6622,
				""tweet_count"": 33591,
				""listed_count"": 677

            },
			""pinned_tweet_id"": ""1397658537053102082"",
			""name"": ""H&R Block"",
			""url"": ""https://t.co/4jenbcenp0"",
			""protected"": false
		}
	],
	""meta"": {
    ""result_count"": 1

    }
}";
    }
}
