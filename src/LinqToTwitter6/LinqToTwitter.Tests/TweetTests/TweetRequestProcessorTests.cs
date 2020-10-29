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
            var target = new TweetRequestProcessor<Tweet>();

            Expression<Func<Tweet, bool>> expression =
                tweet =>
                    tweet.Type == TweetType.Tweets &&
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
                    new KeyValuePair<string, string>(nameof(Tweet.Type), ((int)TweetType.Tweets).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Tweet.Ids), "2,3")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Tweet.Expansions), "attachments.poll_ids,author_id")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Tweet.MediaFields), "height,width")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(Tweet.PlaceFields), "country")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>(nameof(Tweet.PollFields), "duration_minutes,end_datetime")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Tweet.TweetFields), "author_id,created_at")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(Tweet.UserFields), "created_at,verified")));
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
            var tweetReqProc = new TweetRequestProcessor<Tweet> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(Tweet.Type), TweetType.Tweets.ToString() },
                    { nameof(Tweet.Ids), "2,3" },
                    { nameof(Tweet.Expansions), "attachments.poll_ids,author_id" },
                    { nameof(Tweet.MediaFields), "height,width" },
                    { nameof(Tweet.PlaceFields), "country" },
                    { nameof(Tweet.PollFields), "duration_minutes,end_datetime" },
                    { nameof(Tweet.TweetFields), "author_id,created_at" },
                    { nameof(Tweet.UserFields), "created_at,verified" },
               };

            Request req = tweetReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Throws_When_Parameters_Null()
        {
            var tweetReqProc = new TweetRequestProcessor<Tweet> { BaseUrl = BaseUrl2 };

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
            var tweetReqProc = new TweetRequestProcessor<Tweet> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(Tweet.Type), TweetType.Tweets.ToString() },
                    { nameof(Tweet.Ids), "2, 3" },
                    { nameof(Tweet.Expansions), "attachments.poll_ids, author_id" },
                    { nameof(Tweet.MediaFields), "height, width" },
                    { nameof(Tweet.PlaceFields), "country" },
                    { nameof(Tweet.PollFields), "duration_minutes, end_datetime" },
                    { nameof(Tweet.TweetFields), "author_id, created_at" },
                    { nameof(Tweet.UserFields), "created_at, verified" },
               };

            Request req = tweetReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Requires_Ids()
        {
            var tweetReqProc = new TweetRequestProcessor<Tweet> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(Tweet.Type), TweetType.Tweets.ToString() },
                    //{ nameof(Tweet.Ids), null }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
                    tweetReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(Tweet.Ids), ex.ParamName);
        }

    }
}
