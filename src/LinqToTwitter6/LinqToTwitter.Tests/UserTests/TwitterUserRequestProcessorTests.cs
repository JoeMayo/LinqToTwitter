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
					tweet.MediaFields == "height,width" &&
					tweet.PaginationToken == "123" &&
					tweet.PlaceFields == "country" &&
					tweet.PollFields == "duration_minutes,end_datetime" &&
					tweet.Expansions == "attachments.poll_ids,author_id" &&
					tweet.SpaceID == "345" &&
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
					new KeyValuePair<string, string>(nameof(TwitterUserQuery.MediaFields), "height,width")));
			Assert.IsTrue(
				queryParams.Contains(
					new KeyValuePair<string, string>(nameof(TwitterUserQuery.PaginationToken), "123")));
			Assert.IsTrue(
			   queryParams.Contains(
				   new KeyValuePair<string, string>(nameof(TwitterUserQuery.PlaceFields), "country")));
			Assert.IsTrue(
			   queryParams.Contains(
				   new KeyValuePair<string, string>(nameof(TwitterUserQuery.PollFields), "duration_minutes,end_datetime")));
			Assert.IsTrue(
			   queryParams.Contains(
				   new KeyValuePair<string, string>(nameof(TwitterUserQuery.SpaceID), "345")));
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
		public void BuildUrl_ForRetweetedBy_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "tweets/123/retweeted_by?" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
				"media.fields=height%2Cwidth&" +
				"place.fields=country&" +
				"poll.fields=duration_minutes%2Cend_datetime&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.RetweetedBy.ToString() },
					{ nameof(TwitterUserQuery.ID), "123" },
					{ nameof(TwitterUserQuery.Expansions), "attachments.poll_ids,author_id" },
					{ nameof(TwitterUserQuery.MediaFields), "height,width" },
					{ nameof(TwitterUserQuery.PlaceFields), "country" },
					{ nameof(TwitterUserQuery.PollFields), "duration_minutes,end_datetime" },
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
		public void BuildUrl_ForRetweetedBy_RequiresID()
		{
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
                new Dictionary<string, string>
                {
                    { nameof(TweetQuery.Type), UserType.RetweetedBy.ToString() },
			        //{ nameof(Tweet.ID), null }
			    };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
					twitterUserReqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(TwitterUserQuery.ID), ex.ParamName);
        }

		[TestMethod]
		public void BuildUrl_ForListFollowers_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "lists/12345/followers?" +
				"expansions=owner_id&" +
				"max_results=50&" +
				"pagination_token=def&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var twitterUserProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.ListFollowers.ToString() },
					{ nameof(TwitterUserQuery.Expansions), ExpansionField.OwnerID },
					{ nameof(TwitterUserQuery.ListID), "12345" },
					{ nameof(TwitterUserQuery.MaxResults), "50" },
					{ nameof(TwitterUserQuery.PaginationToken), "def" },
					{ nameof(TwitterUserQuery.TweetFields), $"{TweetField.AuthorID},{TweetField.CreatedAt}" },
					{ nameof(TwitterUserQuery.UserFields), $"{UserField.CreatedAt},{UserField.Verified}" },
			   };

			Request req = twitterUserProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
		public void BuildUrl_ForListFollowersWithSpacesInFields_RemovesSpaces()
		{
			const string ExpectedUrl =
				BaseUrl2 + "lists/12345/followers?" +
				"expansions=owner_id&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var twitterUserProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.ListFollowers.ToString() },
					{ nameof(TwitterUserQuery.ListID), "12345" },
					{ nameof(TwitterUserQuery.Expansions), ExpansionField.OwnerID },
					{ nameof(TwitterUserQuery.TweetFields), $"{TweetField.AuthorID}, {TweetField.CreatedAt}" },
					{ nameof(TwitterUserQuery.UserFields), $"{UserField.CreatedAt}, {UserField.Verified}" },
			   };

			Request req = twitterUserProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
		public void BuildUrl_ForListFollowers_RequiresListID()
		{
			var twitterUserProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.ListFollowers.ToString() },
                    //{ nameof(TwitterUserQuery.ListID), null }
                };

			ArgumentException ex =
				L2TAssert.Throws<ArgumentException>(() =>
					twitterUserProc.BuildUrl(parameters));

			Assert.AreEqual(nameof(TwitterUserQuery.ListID), ex.ParamName);
		}

		[TestMethod]
		public void BuildUrl_ForListMembers_IncludesParameters()
		{
			const string ExpectedUrl =
				BaseUrl2 + "lists/12345/members?" +
				"expansions=owner_id&" +
				"max_results=50&" +
				"pagination_token=def&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var twitterUserProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.ListMembers.ToString() },
					{ nameof(TwitterUserQuery.Expansions), ExpansionField.OwnerID },
					{ nameof(TwitterUserQuery.ListID), "12345" },
					{ nameof(TwitterUserQuery.MaxResults), "50" },
					{ nameof(TwitterUserQuery.PaginationToken), "def" },
					{ nameof(TwitterUserQuery.TweetFields), $"{TweetField.AuthorID},{TweetField.CreatedAt}" },
					{ nameof(TwitterUserQuery.UserFields), $"{UserField.CreatedAt},{UserField.Verified}" },
			   };

			Request req = twitterUserProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
		public void BuildUrl_ForListMembersWithSpacesInFields_RemovesSpaces()
		{
			const string ExpectedUrl =
				BaseUrl2 + "lists/12345/members?" +
				"expansions=owner_id&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var twitterUserProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.ListMembers.ToString() },
					{ nameof(TwitterUserQuery.ListID), "12345" },
					{ nameof(TwitterUserQuery.Expansions), ExpansionField.OwnerID },
					{ nameof(TwitterUserQuery.TweetFields), $"{TweetField.AuthorID}, {TweetField.CreatedAt}" },
					{ nameof(TwitterUserQuery.UserFields), $"{UserField.CreatedAt}, {UserField.Verified}" },
			   };

			Request req = twitterUserProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
		}

		[TestMethod]
		public void BuildUrl_ForListMembers_RequiresListID()
		{
			var twitterUserProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.ListMembers.ToString() },
                    //{ nameof(TwitterUserQuery.ListID), null }
                };

			ArgumentException ex =
				L2TAssert.Throws<ArgumentException>(() =>
					twitterUserProc.BuildUrl(parameters));

			Assert.AreEqual(nameof(TwitterUserQuery.ListID), ex.ParamName);
		}

		[TestMethod]
		public void BuildUrl_ForSpaceBuyers_ConstructsUrl()
		{
			const string ExpectedUrl =
				BaseUrl2 + "spaces/345/buyers?" +
				"expansions=attachments.poll_ids%2Cauthor_id&" +
				"media.fields=height%2Cwidth&" +
				"place.fields=country&" +
				"poll.fields=duration_minutes%2Cend_datetime&" +
				"tweet.fields=author_id%2Ccreated_at&" +
				"user.fields=created_at%2Cverified";
			var reqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };
			var parameters =
				new Dictionary<string, string>
				{
					{ nameof(TwitterUserQuery.Type), UserType.SpaceBuyers.ToString() },
					{ nameof(TwitterUserQuery.SpaceID), "345" },
					{ nameof(TwitterUserQuery.Expansions), "attachments.poll_ids,author_id" },
					{ nameof(TwitterUserQuery.MediaFields), "height,width" },
					{ nameof(TwitterUserQuery.PlaceFields), "country" },
					{ nameof(TwitterUserQuery.PollFields), "duration_minutes,end_datetime" },
					{ nameof(TwitterUserQuery.TweetFields), "author_id,created_at" },
					{ nameof(TwitterUserQuery.UserFields), "created_at,verified" }
			   };

			Request req = reqProc.BuildUrl(parameters);

			Assert.AreEqual(ExpectedUrl, req.FullUrl);
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
		public void ProcessResults_WithFullUserExpansionsAndTweets_Deserializes()
		{
			var twitterUserReqProc = new TwitterUserRequestProcessor<TwitterUserQuery> { BaseUrl = BaseUrl2 };

			List<TwitterUserQuery> results = twitterUserReqProc.ProcessResults(FullUserWithTweet);

			Assert.IsNotNull(results);
			TwitterUserQuery twitterUserQuery = results.SingleOrDefault();
			Assert.IsNotNull(twitterUserQuery);
			TwitterInclude includes = twitterUserQuery.Includes;
			Assert.IsNotNull(includes);
			List<Tweet> tweets = includes.Tweets;
			Assert.IsNotNull(tweets);
			Assert.IsTrue(tweets.Any());
			Tweet tweet = tweets.First();
			Assert.IsNotNull(tweet);
			Assert.AreEqual(TweetReplySettings.Everyone, tweet.ReplySettings);
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
				ListID = "9025",
				MaxResults = 50,
				PaginationToken = "567",
				SpaceID = "345",
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
            Assert.AreEqual("123", twitterUserQuery.Expansions);
			Assert.AreEqual("9025", twitterUserQuery.ListID);
			Assert.AreEqual(50, twitterUserQuery.MaxResults);
			Assert.AreEqual("567", twitterUserQuery.PaginationToken);
			Assert.AreEqual("345", twitterUserQuery.SpaceID);
            Assert.AreEqual("678", twitterUserQuery.TweetFields);
            Assert.AreEqual("234", twitterUserQuery.UserFields);
			Assert.AreEqual("9,0", twitterUserQuery.Usernames);
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

		public const string FullUserWithTweet = @"{
	""data"": [
		{
			""protected"": false,
			""name"": ""Joe Mayo"",
			""verified"": false,
			""created_at"": ""2008-07-13T04:35:50.000Z"",
			""url"": ""https://t.co/Y6dXyWxanS"",
			""description"": ""Author, Instructor, & Independent Consultant. Author of C# Cookbook:\n\n  - https://t.co/b436r6hCUK - @OReillyMedia\n\n#ai #chatbots #csharp #linq2twitter #twitterapi"",
			""username"": ""JoeMayo"",
			""profile_image_url"": ""https://pbs.twimg.com/profile_images/1185764990403268613/8GoXoOtz_normal.jpg"",
			""pinned_tweet_id"": ""1326282271372963840"",
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
	""urls"": [
						{
		""start"": 74,
							""end"": 97,
							""url"": ""https://t.co/b436r6hCUK"",
							""expanded_url"": ""http://bit.ly/CSharpCookbook"",
							""display_url"": ""bit.ly/CSharpCookbook""
						}
					],
					""hashtags"": [
						{
		""start"": 115,
							""end"": 118,
							""tag"": ""ai""
						},
						{
		""start"": 119,
							""end"": 128,
							""tag"": ""chatbots""
						},
						{
		""start"": 129,
							""end"": 136,
							""tag"": ""csharp""
						},
						{
		""start"": 137,
							""end"": 150,
							""tag"": ""linq2twitter""
						},
						{
		""start"": 151,
							""end"": 162,
							""tag"": ""twitterapi""
						}
					],
					""mentions"": [
						{
		""start"": 100,
							""end"": 113,
							""username"": ""OReillyMedia""
						}
					]
				}
			},
			""public_metrics"": {
	""followers_count"": 10094,
				""following_count"": 2530,
				""tweet_count"": 3850,
				""listed_count"": 269
			},
			""id"": ""15411837"",
			""location"": ""Las Vegas, NV""
		},
		{
	""protected"": false,
			""name"": ""LINQ to Twitr"",
			""verified"": false,
			""created_at"": ""2008-10-15T05:15:40.000Z"",
			""url"": ""https://t.co/7AhNKZC73J"",
			""description"": ""LINQ to Twitter is a 3rd party library that helps .NET developers write code for the Twitter API - Created by @JoeMayo"",
			""username"": ""Linq2Twitr"",
			""profile_image_url"": ""https://pbs.twimg.com/profile_images/378800000625948439/57f4351535721aeedc632745ceaacfea_normal.png"",
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
	},
			""public_metrics"": {
		""followers_count"": 354,
				""following_count"": 40,
				""tweet_count"": 677,
				""listed_count"": 14
			},
			""id"": ""16761255"",
			""location"": ""Las Vegas, NV""
		}
	],
	""includes"": {
	""tweets"": [
			{
		""text"": ""Announcing (Early Release) C# Cookbook: Modern Recipes for Professional Developers: https://t.co/dvlZNGp0px\n\nThis is the first two chapters and there are more to come. This is the Raw and Unedited version - available at @OReillyMedia \n\n#dotnet #csharp"",
				""id"": ""1326282271372963840"",
				""public_metrics"": {
			""retweet_count"": 3,
					""reply_count"": 0,
					""like_count"": 14,
					""quote_count"": 0
				},
				""entities"": {
			""urls"": [
						{
				""start"": 84,
							""end"": 107,
							""url"": ""https://t.co/dvlZNGp0px"",
							""expanded_url"": ""https://bit.ly/CSharpCookbook"",
							""display_url"": ""bit.ly/CSharpCookbook"",
							""images"": [
								{
					""url"": ""https://pbs.twimg.com/news_img/1334056172349923328/PVWC5sd_?format=jpg&name=orig"",
									""width"": 140,
									""height"": 184
								},
								{
					""url"": ""https://pbs.twimg.com/news_img/1334056172349923328/PVWC5sd_?format=jpg&name=150x150"",
									""width"": 140,
									""height"": 140
								}
							],
							""status"": 200,
							""title"": ""C# Cookbook"",
							""description"": ""Even if you're familiar with C# syntax, knowing how to combine various language features is a critical skill when building applications. This handy cookbook is packed full of recipes to … - Selection from C# Cookbook [Book]"",
							""unwound_url"": ""https://www.oreilly.com/library/view/c-cookbook/9781492093688/""
						}
					],
					""mentions"": [
						{
				""start"": 220,
							""end"": 233,
							""username"": ""OReillyMedia""
						}
					],
					""hashtags"": [
						{
				""start"": 236,
							""end"": 243,
							""tag"": ""dotnet""
						},
						{
				""start"": 244,
							""end"": 251,
							""tag"": ""csharp""
						}
					]
				},
				""author_id"": ""15411837"",
				""context_annotations"": [
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
				""id"": ""66"",
							""name"": ""Interests and Hobbies Category"",
							""description"": ""A grouping of interests and hobbies entities, like Novelty Food or Destinations""
						},
						""entity"": {
				""id"": ""898673391980261376"",
							""name"": ""Web development"",
							""description"": ""Web Development""
						}
		},
					{
			""domain"": {
				""id"": ""85"",
							""name"": ""Book Genre"",
							""description"": ""A genre for books, like Fiction""
						},
						""entity"": {
				""id"": ""859532072813158400"",
							""name"": ""Food inspiration"",
							""description"": ""Food""
						}
		},
					{
			""domain"": {
				""id"": ""65"",
							""name"": ""Interests and Hobbies Vertical"",
							""description"": ""Top level interests and hobbies groupings, like Food or Travel""
						},
						""entity"": {
				""id"": ""825047692124442624"",
							""name"": ""Food"",
							""description"": ""Food""
						}
		},
					{
			""domain"": {
				""id"": ""66"",
							""name"": ""Interests and Hobbies Category"",
							""description"": ""A grouping of interests and hobbies entities, like Novelty Food or Destinations""
						},
						""entity"": {
				""id"": ""831530561680191490"",
							""name"": ""Cooking"",
							""description"": ""Cooking/Baking""
						}
		},
					{
			""domain"": {
				""id"": ""67"",
							""name"": ""Interests and Hobbies"",
							""description"": ""Interests, opinions, and behaviors of individuals, groups, or cultures; like Speciality Cooking or Theme Parks""
						},
						""entity"": {
				""id"": ""846736745517350912"",
							""name"": ""Recipes"",
							""description"": ""Recipies/Books""
						}
		}
				],
				""conversation_id"": ""1326282271372963840"",
				""reply_settings"": ""everyone"",
				""source"": ""Twitter Web App"",
				""lang"": ""en"",
				""created_at"": ""2020-11-10T21:55:09.000Z"",
				""possibly_sensitive"": false
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
