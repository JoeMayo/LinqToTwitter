using LinqToTwitter.Common;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.ListTests
{
    [TestClass]
    public class ListRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public ListRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_Parses_All_Available_Parameters()
        {
            var listReqProc = new ListRequestProcessor<ListQuery>();
            Expression<Func<ListQuery, bool>> expression =
                list =>
                    list.Type == ListType.Lookup &&
                    list.Expansions == ExpansionField.OwnerID &&
                    list.ListID == "abc" &&
                    list.ListFields == ListField.CreatedAt &&
                    list.MaxResults == 50 &&
                    list.PaginationToken == "def" &&
                    list.UserFields == UserField.ProfileImageUrl &&
                    list.UserID == "123";

            var queryParams = listReqProc.GetParameters(expression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ListQuery.Type), ((int)ListType.Lookup).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ListQuery.Expansions), ExpansionField.OwnerID)));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ListQuery.ListID), "abc")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ListQuery.ListFields), ListField.CreatedAt)));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ListQuery.MaxResults), "50")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ListQuery.PaginationToken), "def")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ListQuery.UserFields), UserField.ProfileImageUrl)));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ListQuery.UserID), "123")));
        }

        [TestMethod]
        public void BuildUrl_WithNullParameters_Throws()
        {
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };

            L2TAssert.Throws<ArgumentNullException>(() =>
            {
                reqProc.BuildUrl(null);
            });
        }

        [TestMethod]
        public void BuildUrl_ForLookup_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "lists/12345?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Lookup.ToString() },
                    { nameof(ListQuery.ListID), "12345" },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description},{ListField.MemberCount}" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt},{UserField.Verified}" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForLookupWithSpacesInFields_RemovesSpaces()
        {
            const string ExpectedUrl =
                BaseUrl2 + "lists/12345?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Lookup.ToString() },
                    { nameof(ListQuery.ListID), "12345" },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description}, {ListField.MemberCount}" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt}, {UserField.Verified}" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForLookup_RequiresListID()
        {
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Lookup.ToString() },
                    //{ nameof(ListType.ListID), null }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
                    reqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(ListQuery.ListID), ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_ForOwnership_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/12345/owned_lists?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "max_results=50&" +
                "pagination_token=def&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Owned.ToString() },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description},{ListField.MemberCount}" },
                    { nameof(ListQuery.MaxResults), "50" },
                    { nameof(ListQuery.PaginationToken), "def" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt},{UserField.Verified}" },
                    { nameof(ListQuery.UserID), "12345" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForOwnershipWithSpacesInFields_RemovesSpaces()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/12345/owned_lists?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Owned.ToString() },
                    { nameof(ListQuery.UserID), "12345" },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description}, {ListField.MemberCount}" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt}, {UserField.Verified}" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForOwnership_RequiresListID()
        {
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Owned.ToString() },
                    //{ nameof(ListType.UserID), null }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
                    reqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(ListQuery.UserID), ex.ParamName);
        }

        [TestMethod]
        public void ProcessReults_WithSingleList_ReturnsListData()
        {
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };

            List<ListQuery> results = reqProc.ProcessResults(SingleListResponse);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            ListQuery query = results.SingleOrDefault();
            Assert.IsNotNull(query);
            List<List> lists = query.Lists;
            Assert.IsNotNull(lists);
            Assert.AreEqual(1, lists.Count);
            List list = query.Lists.SingleOrDefault();
            Assert.AreEqual("898994036043689985", list.ID);
            Assert.AreEqual("AI", list.Name);
        }

        [TestMethod]
        public void ProcessReults_WithMultipleLists_ReturnsListData()
        {
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };

            List<ListQuery> results = reqProc.ProcessResults(MultipleListsResponse);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            ListQuery query = results.SingleOrDefault();
            Assert.IsNotNull(query);
            List<List> lists = query.Lists;
            Assert.IsNotNull(lists);
            Assert.AreEqual(2, lists.Count);
            List list = query.Lists.FirstOrDefault();
            Assert.AreEqual("1465133473744654337", list.ID);
            Assert.AreEqual("Crypto", list.Name);
            Assert.AreEqual(12, list.MemberCount);
            Assert.AreEqual("Accounts Related to Crypto Currencies", list.Description);
            Assert.IsTrue(list.Private);
            Assert.AreEqual(1024, list.FollowerCount);
            Assert.AreEqual(DateTime.Parse("2021-11-29T01:40:15.000Z").ToUniversalTime(), list.CreatedAt);
            Assert.AreEqual("15411837", list.OwnerID);
            ListMeta meta = query.Meta;
            Assert.IsNotNull(meta);
            Assert.AreEqual(2, meta.ResultCount);
            Assert.AreEqual("1695585497111705486", meta.NextToken);
            Assert.AreEqual("1695585497111705485", meta.PreviousToken);
        }

        [TestMethod]
        public void BuildUrl_ForMembership_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/12345/list_memberships?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "max_results=50&" +
                "pagination_token=def&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Member.ToString() },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description},{ListField.MemberCount}" },
                    { nameof(ListQuery.MaxResults), "50" },
                    { nameof(ListQuery.PaginationToken), "def" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt},{UserField.Verified}" },
                    { nameof(ListQuery.UserID), "12345" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForMembershipWithSpacesInFields_RemovesSpaces()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/12345/list_memberships?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Member.ToString() },
                    { nameof(ListQuery.UserID), "12345" },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description}, {ListField.MemberCount}" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt}, {UserField.Verified}" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForMembership_RequiresListID()
        {
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Member.ToString() },
                    //{ nameof(ListType.UserID), null }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
                    reqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(ListQuery.UserID), ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_ForFollowed_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/12345/followed_lists?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "max_results=50&" +
                "pagination_token=def&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Follow.ToString() },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description},{ListField.MemberCount}" },
                    { nameof(ListQuery.MaxResults), "50" },
                    { nameof(ListQuery.PaginationToken), "def" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt},{UserField.Verified}" },
                    { nameof(ListQuery.UserID), "12345" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForFollowedWithSpacesInFields_RemovesSpaces()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/12345/followed_lists?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Follow.ToString() },
                    { nameof(ListQuery.UserID), "12345" },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description}, {ListField.MemberCount}" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt}, {UserField.Verified}" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForFollowed_RequiresListID()
        {
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Follow.ToString() },
                    //{ nameof(ListType.UserID), null }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
                    reqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(ListQuery.UserID), ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_ForPinned_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/12345/pinned_lists?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "max_results=50&" +
                "pagination_token=def&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Pin.ToString() },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description},{ListField.MemberCount}" },
                    { nameof(ListQuery.MaxResults), "50" },
                    { nameof(ListQuery.PaginationToken), "def" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt},{UserField.Verified}" },
                    { nameof(ListQuery.UserID), "12345" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForPinnedWithSpacesInFields_RemovesSpaces()
        {
            const string ExpectedUrl =
                BaseUrl2 + "users/12345/pinned_lists?" +
                "expansions=owner_id&" +
                "list.fields=description%2Cmember_count&" +
                "user.fields=created_at%2Cverified";
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Pin.ToString() },
                    { nameof(ListQuery.UserID), "12345" },
                    { nameof(ListQuery.Expansions), ExpansionField.OwnerID },
                    { nameof(ListQuery.ListFields), $"{ListField.Description}, {ListField.MemberCount}" },
                    { nameof(ListQuery.UserFields), $"{UserField.CreatedAt}, {UserField.Verified}" },
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForPinned_RequiresListID()
        {
            var reqProc = new ListRequestProcessor<ListQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ListQuery.Type), ListType.Pin.ToString() },
                    //{ nameof(ListType.UserID), null }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
                    reqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(ListQuery.UserID), ex.ParamName);
        }

        const string SingleListResponse = @"{
	""data"": {
		""id"": ""898994036043689985"",
		""name"": ""AI""
	}
}";

        const string MultipleListsResponse = @"{
	""data"": [
		{
			""member_count"": 12,
			""description"": ""Accounts Related to Crypto Currencies"",
			""private"": true,
			""id"": ""1465133473744654337"",
			""follower_count"": 1024,
			""created_at"": ""2021-11-29T01:40:15.000Z"",
			""owner_id"": ""15411837"",
			""name"": ""Crypto""
		},
		{
			""member_count"": 1764,
			""description"": ""Startup and VC Ecosystem"",
			""private"": false,
			""id"": ""1288570987919757312"",
			""follower_count"": 6,
			""created_at"": ""2020-07-29T20:23:58.000Z"",
			""owner_id"": ""15411837"",
			""name"": ""VC and Startup""
		}
	],
	""includes"": {
		""users"": [
			{
				""created_at"": ""2008-07-13T04:35:50.000Z"",
				""id"": ""15411837"",
				""username"": ""JoeMayo"",
				""public_metrics"": {
					""followers_count"": 10000,
					""following_count"": 1639,
					""tweet_count"": 4031,
					""listed_count"": 265

                },
				""protected"": false,
				""verified"": false,
				""pinned_tweet_id"": ""1461369532514127877"",
				""name"": ""Joe Mayo"",
				""profile_image_url"": ""https://pbs.twimg.com/profile_images/1185764990403268613/8GoXoOtz_normal.jpg"",
				""url"": ""https://t.co/1V3mZMjFvp"",
				""description"": ""Author, Instructor, and Independent Consultant \n\nNewest Release: C# Cookbook (https://t.co/acNTiAe6HQ)\n\n#AI #Azure #Chatbots #CSharp #Linq2Twitter #NLP"",
				""entities"": {
					""url"": {
						""urls"": [
							{
								""start"": 0,
								""end"": 23,
								""url"": ""https://t.co/1V3mZMjFvp"",
								""expanded_url"": ""https://www.linkedin.com/in/joemayo/"",
								""display_url"": ""linkedin.com/in/joemayo/""

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
								""end"": 114,
								""tag"": ""Azure""

                            },
							{
        ""start"": 115,
								""end"": 124,
								""tag"": ""Chatbots""

                            },
							{
        ""start"": 125,
								""end"": 132,
								""tag"": ""CSharp""

                            },
							{
        ""start"": 133,
								""end"": 146,
								""tag"": ""Linq2Twitter""

                            },
							{
        ""start"": 147,
								""end"": 151,
								""tag"": ""NLP""

                            }
						]
					}
				},
				""location"": ""Las Vegas, NV""
			}
		]
	},
	""meta"": {
        ""result_count"": 2,
		""next_token"": ""1695585497111705486"",
		""previous_token"": ""1695585497111705485""

    }
}";
    }
}
