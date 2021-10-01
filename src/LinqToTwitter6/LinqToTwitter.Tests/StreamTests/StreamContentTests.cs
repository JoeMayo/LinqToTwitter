using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter.Tests.StreamContentTests
{
    [TestClass]
    public class StreamContentTests
    {
		const string BaseUrl2 = "https://api.twitter.com/2/";

		Mock<ITwitterExecute> execMock = new();

        public StreamContentTests()
        {
            TestCulture.SetCulture();
        }

		[TestMethod]
		public void GetParametersTest()
		{
			var target = new StreamingRequestProcessor<Streaming>();

			var endTime = new DateTime(2020, 8, 30);
			var startTime = new DateTime(2020, 8, 1);
			Expression<Func<Streaming, bool>> expression =
				tweet =>
					tweet.Type == StreamingType.Filter &&
					tweet.BackfillMinutes == 5 &&
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
					new KeyValuePair<string, string>(nameof(Streaming.Type), ((int)StreamingType.Filter).ToString(CultureInfo.InvariantCulture))));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(Streaming.BackfillMinutes), "5")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(Streaming.Ids), "2,3")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(Streaming.Expansions), "attachments.poll_ids,author_id")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(Streaming.MediaFields), "height,width")));
			Assert.IsTrue(
			   queryParams.Contains(
				   new KeyValuePair<string, string>(nameof(Streaming.PlaceFields), "country")));
			Assert.IsTrue(
			   queryParams.Contains(
				   new KeyValuePair<string, string>(nameof(Streaming.PollFields), "duration_minutes,end_datetime")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(Streaming.TweetFields), "author_id,created_at")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(Streaming.UserFields), "created_at,verified")));
		}

		[TestMethod]
		public void BuildUrl_ForFilterStream_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "tweets/search/stream?" +
				"backfill_minutes=5&" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
				"ids=2%2C3&" +
				"media.fields=height%2Cwidth&" +
				"place.fields=country&" +
				"poll.fields=duration_minutes%2Cend_datetime&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var reqProc = new StreamingRequestProcessor<Streaming> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(Streaming.Type), StreamingType.Filter.ToString() },
					{ nameof(Streaming.BackfillMinutes), "5" },
					{ nameof(Streaming.Expansions), "attachments.poll_ids,author_id" },
					{ nameof(Streaming.MediaFields), "height,width" },
					{ nameof(Streaming.PlaceFields), "country" },
					{ nameof(Streaming.PollFields), "duration_minutes,end_datetime" },
					{ nameof(Streaming.TweetFields), "author_id,created_at" },
					{ nameof(Streaming.UserFields), "created_at,verified" },
					{ nameof(Streaming.Ids), "2,3" },
			   };

			Request req = reqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
        public void ParseJson_WithTweetEntity_Succeeds()
        {
            var content = new StreamContent(execMock.Object, TweetContent);

            (StreamTweet entity, StreamEntityType entityType) = content.ParseJson(TweetContent);

            Assert.IsNotNull(entity);
            Assert.AreEqual("1439984798332866573", entity.Tweet.ID);
            Assert.IsTrue(entity.Tweet.Text.StartsWith("Very"));
            Assert.AreEqual("1324201416731160579", entity.MatchingRules[0].ID);
            Assert.AreEqual("funny things", entity.MatchingRules[0].Tag);
            Assert.AreEqual(StreamEntityType.Tweet, entityType);
        }

        const string TweetContent = @"{
	""data"": {
		""id"": ""1439984798332866573"",
		""text"": ""Very very true, Dev! ..😜😂💕\n#Dev https://t.co/cHFjan""
	},
	""matching_rules"": [
		{
			""id"": ""1324201416731160579"",
			""tag"": ""funny things""

        }
	]
}";
    }
}
