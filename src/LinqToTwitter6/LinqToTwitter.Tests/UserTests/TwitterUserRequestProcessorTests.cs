﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitter.Tests.UserTests
{
    [TestClass]
    public class TwitterUserRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public TwitterUserRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParametersTest()
        {
            var target = new TwitterUserRequestProcessor<TwitterUserQuery>();

            Expression<Func<TwitterUserQuery, bool>> expression =
                tweet =>
                    tweet.Type == UserType.IdLookup &&
					tweet.ID == "456" &&
                    tweet.Ids == "2,3" &&
					tweet.Usernames == "joemayo,linq2twitr" &&
					tweet.MaxResults == 50 &&
					tweet.PaginationToken == "123" &&
					tweet.Expansions == "attachments.poll_ids,author_id" &&
                    tweet.TweetFields == "author_id,created_at" &&
                    tweet.UserFields == "created_at,verified";

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterUserQuery.Type), ((int)UserType.IdLookup).ToString(CultureInfo.InvariantCulture))));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TwitterUserQuery.ID), "456")));
			Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterUserQuery.Ids), "2,3")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TwitterUserQuery.Usernames), "joemayo,linq2twitr")));
			Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterUserQuery.Expansions), "attachments.poll_ids,author_id")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TwitterUserQuery.MaxResults), "50")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TwitterUserQuery.PaginationToken), "123")));
						Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterUserQuery.TweetFields), "author_id,created_at")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(TwitterUserQuery.UserFields), "created_at,verified")));
        }

        [TestMethod]
        public void BuildUrl_ForIdLookup_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users?" +
                "ids=2%2C3&" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
                "tweet.fields=author_id%2Ccreated_at&" +
                "user.fields=created_at%2Cverified";
            var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TwitterUserQuery.Type), UserType.IdLookup.ToString() },
                    { nameof(TwitterUserQuery.Ids), "2,3" },
                    { nameof(TwitterUserQuery.Expansions), "attachments.poll_ids,author_id" },
                    { nameof(TwitterUserQuery.TweetFields), "author_id,created_at" },
                    { nameof(TwitterUserQuery.UserFields), "created_at,verified" },
               };

            Request req = twitterUserReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

		[TestMethod]
		public void BuildUrl_ForFollowing_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "users/123/following?" +
				"max_results=50&" +
				"pagination_token=456&" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
                    { nameof(TwitterUserQuery.Type), UserType.Following.ToString() },
                    { nameof(TwitterUserQuery.ID), "123" },
					{ nameof(TwitterUserQuery.Expansions), "attachments.poll_ids,author_id" },
					{ nameof(TwitterUserQuery.MaxResults), "50" },
					{ nameof(TwitterUserQuery.PaginationToken), "456" },
					{ nameof(TwitterUserQuery.TweetFields), "author_id,created_at" },
					{ nameof(TwitterUserQuery.UserFields), "created_at,verified" },
			   };

			Request req = twitterUserReqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
		public void BuildUrl_ForFollowers_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "users/123/followers?" +
				"max_results=50&" +
				"pagination_token=456&" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.Followers.ToString() },
					{ nameof(TwitterUserQuery.ID), "123" },
					{ nameof(TwitterUserQuery.Expansions), "attachments.poll_ids,author_id" },
					{ nameof(TwitterUserQuery.MaxResults), "50" },
					{ nameof(TwitterUserQuery.PaginationToken), "456" },
					{ nameof(TwitterUserQuery.TweetFields), "author_id,created_at" },
					{ nameof(TwitterUserQuery.UserFields), "created_at,verified" },
			   };

			Request req = twitterUserReqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
		public void BuildUrl_WithUsernames_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "users/by?" +
				"usernames=joemayo%2Clinq2twitr&" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.UsernameLookup.ToString() },
					{ nameof(TwitterUserQuery.Usernames), "joemayo,linq2twitr" },
					{ nameof(TwitterUserQuery.Expansions), "attachments.poll_ids,author_id" },
					{ nameof(TwitterUserQuery.TweetFields), "author_id,created_at" },
					{ nameof(TwitterUserQuery.UserFields), "created_at,verified" },
			   };

			Request req = twitterUserReqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
        public void BuildUrl_WithNoParameters_Fails()
        {
            var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };

            L2TAssert.Throws<NullReferenceException>(() =>
            {
                twitterUserReqProc.BuildUrl(null);
            });
        }

        [TestMethod]
        public void BuildUrl_WithSpacesInFields_FixesSpaces()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users?" +
                "ids=2%2C3&" +
                "expansions=attachments.poll_ids%2Cauthor_id&" +
                "tweet.fields=author_id%2Ccreated_at&" +
                "user.fields=created_at%2Cverified";
            var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TwitterUserQuery.Type), UserType.IdLookup.ToString() },
                    { nameof(TwitterUserQuery.Ids), "2, 3" },
                    { nameof(TwitterUserQuery.Expansions), "attachments.poll_ids, author_id" },
                    { nameof(TwitterUserQuery.TweetFields), "author_id, created_at" },
                    { nameof(TwitterUserQuery.UserFields), "created_at, verified" },
               };

            Request req = twitterUserReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_WithoutIdsOnIdLookup_Throws()
        {
            var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TwitterUserQuery.Type), UserType.IdLookup.ToString() },
                    //{ nameof(TwitterUserQuery.Ids), null }
                };

            ArgumentNullException ex =
                L2TAssert.Throws<ArgumentNullException>(() =>
                    twitterUserReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(TwitterUserQuery.Ids), ex.ParamName);
        }

		[TestMethod]
		public void BuildUrl_WithoutIDOnFollowers_Throws()
		{
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.Followers.ToString() },
                    //{ nameof(TwitterUserQuery.ID), null }
                };

			ArgumentException ex =
				L2TAssert.Throws<ArgumentException>(() =>
					twitterUserReqProc.BuildUrl(parameters));

			Assert.AreEqual(nameof(TwitterUserQuery.ID), ex.ParamName);
		}

		[TestMethod]
		public void BuildUrl_WithoutIDOnFollowing_Throws()
		{
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.Following.ToString() },
                    //{ nameof(TwitterUserQuery.ID), null }
                };

			ArgumentException ex =
				L2TAssert.Throws<ArgumentException>(() =>
					twitterUserReqProc.BuildUrl(parameters));

			Assert.AreEqual(nameof(TwitterUserQuery.ID), ex.ParamName);
		}

		[TestMethod]
		public void BuildUrl_WithoutUsernamesOnUsernameLookup_Throws()
		{
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.UsernameLookup.ToString() },
                    //{ nameof(TwitterUserQuery.Usernames), null }
                };

			ArgumentNullException ex =
				L2TAssert.Throws<ArgumentNullException>(() =>
					twitterUserReqProc.BuildUrl(parameters));

			Assert.AreEqual(nameof(TwitterUserQuery.Usernames), ex.ParamName);
		}

		[TestMethod]
        public void ProcessResults_Populates_Users()
        {
            var twitterUserProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };

            List<TwitterUserQuery> results = twitterUserProc.ProcessResults(UsersJson);

            Assert.IsNotNull(results);
			TwitterUserQuery twitterUserQuery = results.SingleOrDefault();
            Assert.IsNotNull(twitterUserQuery);
            List<TwitterUser> users = twitterUserQuery.Users;
            Assert.IsNotNull(users);
            Assert.AreEqual(2, users.Count);
            TwitterUser user = users.FirstOrDefault();
            Assert.IsNotNull(user);
			Assert.AreEqual(DateTime.Parse("2008-07-13").Date, user.CreatedAt.Date);
            Assert.AreEqual("Author, Instructor, and Independent Consultant. Author of Programming the Microsoft Bot Framework/MS Press.\n#ai #chatbots #csharp #linq2twitter #twitterapi", user.Description);
            Assert.AreEqual("15411837", user.ID);
			Assert.AreEqual("Las Vegas, NV", user.Location);
			Assert.AreEqual("Joe Mayo", user.Name);
			Assert.AreEqual("1258043891434962945", user.PinnedTweetId);
			Assert.AreEqual("https://pbs.twimg.com/profile_images/1185764990403268613/8GoXoOtz_normal.jpg", user.ProfileImageUrl);
			Assert.IsTrue(user.Protected);
			Assert.AreEqual("https://t.co/Y6dXyWxanS", user.Url);
			Assert.AreEqual("JoeMayo", user.Username);
			Assert.IsTrue(user.Verified);
        }

		[TestMethod]
		public void ProcessResults_Populates_Entities()
		{
			var twitterUserProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };

			List<TwitterUserQuery> results = twitterUserProc.ProcessResults(UsersJson);

			Assert.IsNotNull(results);
			TwitterUserQuery twitterUserQuery = results.SingleOrDefault();
			Assert.IsNotNull(twitterUserQuery);

			List<TwitterUser> users = twitterUserQuery.Users;
			Assert.IsNotNull(users);
			Assert.AreEqual(2, users.Count);
			TwitterUser user = users.FirstOrDefault();
			Assert.IsNotNull(user);
			TwitterUserEntity entities = user.Entities;
			Assert.IsNotNull(entities);

            TweetEntityUrl url = entities?.Url?.Urls?.FirstOrDefault();
            Assert.IsNotNull(url);
            Assert.AreEqual(0, url.Start);
            Assert.AreEqual(23, url.End);
            Assert.AreEqual("https://t.co/Y6dXyWxanS", url.Url);
            Assert.AreEqual("https://github.com/JoeMayo", url.ExpandedUrl);
            Assert.AreEqual("github.com/JoeMayo", url.DisplayUrl);

			TweetEntityHashtag hashtag = entities?.Description?.Hashtags?.FirstOrDefault();
			Assert.IsNotNull(hashtag);
			Assert.AreEqual(108, hashtag.Start);
			Assert.AreEqual(111, hashtag.End);
			Assert.AreEqual("ai", hashtag.Tag);
		}

		[TestMethod]
		public void ProcessResults_Populates_PublicMetrics()
		{
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };

			List<TwitterUserQuery> results = twitterUserReqProc.ProcessResults(UsersJson);

			Assert.IsNotNull(results);
			TwitterUserQuery twitterUserQuery = results.SingleOrDefault();
			Assert.IsNotNull(twitterUserQuery);
			List<TwitterUser> users = twitterUserQuery.Users;
			Assert.IsNotNull(users);
			Assert.AreEqual(2, users.Count);
			TwitterUser user = users.FirstOrDefault();
			Assert.IsNotNull(user);

			TwitterUserPublicMetrics metrics = user.PublicMetrics;
			Assert.IsNotNull(metrics);
			Assert.AreEqual(10024, metrics.FollowersCount);
			Assert.AreEqual(3539, metrics.FollowingCount);
			Assert.AreEqual(270, metrics.ListedCount);
			Assert.AreEqual(3800, metrics.TweetCount);
		}

		[TestMethod]
        public void ProcessResults_Handles_Response_With_No_Results()
        {
            var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };

            List<TwitterUserQuery> results = twitterUserReqProc.ProcessResults(ErrorTweet);

            Assert.IsNotNull(results);
			TwitterUserQuery twitterUserQuery = results.SingleOrDefault();
            Assert.IsNotNull(twitterUserQuery);
            List<TwitterUser> users = twitterUserQuery.Users;
            Assert.IsNull(users);
        }

        [TestMethod]
        public void ProcessResults_Populates_Input_Parameters()
        {
            var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery>()
            {
                BaseUrl = BaseUrl2,
                Type = UserType.IdLookup,
				ID = "890",
				Ids = "3,7",
				Usernames = "9,0",
                Expansions = "123",
				MaxResults = 50,
				PaginationToken = "567",
                TweetFields = "678",
                UserFields = "234"
            };

            var results = twitterUserReqProc.ProcessResults(UsersJson);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            var twitterUserQuery = results.Single();
            Assert.IsNotNull(twitterUserQuery);
            Assert.AreEqual(UserType.IdLookup, twitterUserQuery.Type);
			Assert.AreEqual("890", twitterUserQuery.ID);
			Assert.AreEqual("3,7", twitterUserQuery.Ids);
			Assert.AreEqual("9,0", twitterUserQuery.Usernames);
            Assert.AreEqual("123", twitterUserQuery.Expansions);
			Assert.AreEqual(50, twitterUserQuery.MaxResults);
			Assert.AreEqual("567", twitterUserQuery.PaginationToken);
            Assert.AreEqual("678", twitterUserQuery.TweetFields);
            Assert.AreEqual("234", twitterUserQuery.UserFields);
        }

		[TestMethod]
        public void ProcessResults_WithErrors_PopulatesErrorList()
        {
            var twitterUserProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<TwitterUserQuery> results = twitterUserProc.ProcessResults(ErrorTweet);

            Assert.IsNotNull(results);
			TwitterUserQuery twitterUserQuery = results.SingleOrDefault();
            Assert.IsNotNull(twitterUserQuery);
            List<TwitterError> errors = twitterUserQuery.Errors;
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

        const string UsersJson = @"{
	""data"": [
		{
			""username"": ""JoeMayo"",
			""pinned_tweet_id"": ""1258043891434962945"",
			""protected"": true,
			""name"": ""Joe Mayo"",
			""profile_image_url"": ""https://pbs.twimg.com/profile_images/1185764990403268613/8GoXoOtz_normal.jpg"",
			""id"": ""15411837"",
			""public_metrics"": {
				""followers_count"": 10024,
				""following_count"": 3539,
				""tweet_count"": 3800,
				""listed_count"": 270
			},
			""verified"": true,
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
	""username"": ""Linq2Twitr"",
			""protected"": false,
			""name"": ""LINQ to Twitr"",
			""profile_image_url"": ""https://pbs.twimg.com/profile_images/378800000625948439/57f4351535721aeedc632745ceaacfea_normal.png"",
			""id"": ""16761255"",
			""public_metrics"": {
		""followers_count"": 353,
				""following_count"": 35,
				""tweet_count"": 646,
				""listed_count"": 14
			},
			""verified"": false,
			""description"": ""LINQ to Twitter is a 3rd party library that helps .NET developers write code for the Twitter API - Created by @JoeMayo"",
			""created_at"": ""2008-10-15T05:15:40.000Z"",
			""location"": ""Las Vegas, NV"",
			""url"": ""https://t.co/7AhNKZC73J"",
			""entities"": {
		""url"": {
			""urls"": [
						{
				""start"": 0,
							""end"": 23,
							""url"": ""https://t.co/7AhNKZC73J"",
							""expanded_url"": ""https://github.com/JoeMayo/LinqToTwitter"",
							""display_url"": ""github.com/JoeMayo/LinqTo…""
						}
					]
				},
				""description"": {
			""mentions"": [
						{
				""start"": 110,
							""end"": 118,
							""username"": ""JoeMayo""
						}
					]
				}
	}
}
	]
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
