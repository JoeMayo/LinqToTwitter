using LinqToTwitter.Common;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter.Tests.SpaceTests
{
    [TestClass]
    public class SpacesRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public SpacesRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_WithInputParams_Succeeds()
        {
            var target = new SpacesRequestProcessor<SpacesQuery>();
            Expression<Func<SpacesQuery, bool>> expression =
                space =>
                    space.Type == SpacesType.Search &&
                    space.Query == "My Space" &&
                    space.Expansions == "author_id,attachments.media_keys" &&
                    space.MaxResults == 100 &&
                    space.SpaceFields == "id,title" &&
                    space.State == "live" &&
                    space.UserFields == "id,name";
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(SpacesQuery.Type), ((int)SpacesType.Search).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(SpacesQuery.Query), "My Space")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(SpacesQuery.Expansions), "author_id,attachments.media_keys")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(SpacesQuery.MaxResults), "100")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(SpacesQuery.SpaceFields), "id,title")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(SpacesQuery.State), "live")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(SpacesQuery.UserFields), "id,name")));
        }

        [TestMethod]
        public void BuildUrl_WithParams_ConstructsUrl()
        {
            const string ExpectedUrl = 
                BaseUrl2 + "spaces/search?" +
                "query=twitter&" +
                "expansions=attachments.poll_ids%2Cauthor_id&" +
                "max_results=99&" +
                "space.fields=id%2Ctitle&" +
                "state=live&" +
                "user.fields=created_at%2Cverified";

            var reqProc = new SpacesRequestProcessor<SpacesQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(SpacesQuery.Type), ((int)SpacesType.Search).ToString(CultureInfo.InvariantCulture) },
                    { nameof(SpacesQuery.Query), "twitter" },
                    { nameof(SpacesQuery.Expansions), "attachments.poll_ids,author_id" },
                    { nameof(SpacesQuery.MaxResults), "99" },
                    { nameof(SpacesQuery.SpaceFields), "id,title" },
                    { nameof(SpacesQuery.State), SpaceState.Live },
                    { nameof(SpacesQuery.UserFields), "created_at,verified" }
                };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

		[TestMethod]
		public void ProcessResults_WithInputFilters_RepopulatesInputFilterProperties()
        {
			var reqProc = new SpacesRequestProcessor<SpacesQuery> 
			{ 
				Type = SpacesType.Search,
				Query = "twitter",
				Expansions = ExpansionField.HostIds,
				MaxResults = 99,
				SpaceFields = SpaceField.HostIds,
				State = SpaceState.Live,
				UserFields = UserField.Name
			};

			List<SpacesQuery> searchResponse = reqProc.ProcessResults(SearchResponse);

			var spaceQuery = searchResponse.FirstOrDefault();
			Assert.IsNotNull(spaceQuery);
			Assert.AreEqual(SpacesType.Search, spaceQuery.Type);
			Assert.AreEqual("twitter", spaceQuery.Query);
			Assert.AreEqual(ExpansionField.HostIds, spaceQuery.Expansions);
			Assert.AreEqual(99, spaceQuery.MaxResults);
			Assert.AreEqual(SpaceField.HostIds, spaceQuery.SpaceFields);
			Assert.AreEqual(SpaceState.Live, spaceQuery.State);
			Assert.AreEqual(UserField.Name, spaceQuery.UserFields);
		}

		[TestMethod]
        public void ProcessResults_WithSearchResponse_Succeeds()
        {
            var reqProc = new SpacesRequestProcessor<SpacesQuery> { Type = SpacesType.Search };

            List<SpacesQuery> searchResponse = reqProc.ProcessResults(SearchResponse);

            Assert.IsNotNull(searchResponse);
            Assert.IsNotNull(searchResponse.SingleOrDefault());
            var spaceQuery = searchResponse.Single();
            Assert.IsNotNull(spaceQuery);
            var spaces = spaceQuery.Spaces;
            Assert.IsNotNull(spaces);
            Assert.IsTrue(spaces.Any());
            Space space = spaces.First();
            Assert.IsNotNull(space);
            Assert.AreEqual("1109717717895049216", space.CreatorID);
			Assert.IsNotNull(space.InvitedUserIds);
			Assert.AreEqual(7, space.InvitedUserIds.Count);
			Assert.AreEqual(1, space.ParticipantCount);
			Assert.AreEqual(true, space.IsTicketed);
			Assert.AreEqual("fr", space.Lang);
			Assert.AreEqual("10/20/2021 00:54:23", space.CreatedAt.Value.ToString());
			Assert.AreEqual("10/20/2021 04:04:53", space.UpdatedAt.Value.ToString());
			Assert.AreEqual("10/20/2021 00:54:25", space.StartedAt.Value.ToString());
			Assert.AreEqual("1rmxPgZLjEZJN", space.ID);
			Assert.IsNotNull(space.SpeakerIds);
			Assert.AreEqual(1, space.SpeakerIds.Count);
			Assert.IsNotNull(space.HostIds);
			Assert.AreEqual(2, space.HostIds.Count);

            SpaceMeta meta = spaceQuery.Meta;
            Assert.IsNotNull(meta);
            Assert.AreEqual(3, meta.ResultCount);
        }

		const string SearchResponse = @"{
	""data"": [
		{
			""creator_id"": ""1109717717895049216"",
			""invited_user_ids"": [
				""1242778807930683399"",
				""30089230"",
				""617105864"",
				""1439813626060230657"",
				""1268062881884618752"",
				""1420497557281771520"",
				""20550618""
			],
			""participant_count"": 1,
			""is_ticketed"": true,
			""lang"": ""fr"",
			""created_at"": ""2021-10-20T00:54:23.000Z"",
			""updated_at"": ""2021-10-20T04:04:53.000Z"",
			""started_at"": ""2021-10-20T00:54:25.000Z"",
			""state"": ""live"",
			""id"": ""1rmxPgZLjEZJN"",
			""speaker_ids"": [
				""1439813626060230657""
			],
			""host_ids"": [
				""1109717717895049216"",
				""1439813626060230657""
			]
	},
		{
			""creator_id"": ""1131099534786945024"",
			""invited_user_ids"": [
				""1290675089222991874"",
				""1246445180908998656"",
				""1275005734606876673"",
				""1414825887854252034"",
				""1434147837831364610"",
				""1140992831391240193"",
				""985972363""
			],
			""participant_count"": 8,
			""is_ticketed"": false,
			""lang"": ""hi"",
			""created_at"": ""2021-10-20T03:24:00.000Z"",
			""updated_at"": ""2021-10-20T04:04:54.000Z"",
			""title"": ""my last space twitter 😊"",
			""started_at"": ""2021-10-20T03:24:02.000Z"",
			""state"": ""live"",
			""id"": ""1vAxRkZazZqKl"",
			""speaker_ids"": [
				""1246445180908998656"",
				""1275005734606876673"",
				""1165133352728289280"",
				""1434147837831364610"",
				""1140992831391240193""
			],
			""host_ids"": [
				""1131099534786945024"",
				""1434147837831364610""
			]
},
		{
	""creator_id"": ""1137751093"",
			""participant_count"": 0,
			""is_ticketed"": false,
			""lang"": ""da"",
			""created_at"": ""2021-10-13T18:57:44.000Z"",
			""updated_at"": ""2021-10-19T19:45:48.000Z"",
			""title"": ""Online Safety for Parents & Kids #InfosecAwareness"",
			""state"": ""scheduled"",
			""id"": ""1ypKdERNAnqGW"",
			""scheduled_start"": ""2021-10-21T18:00:00.000Z"",
			""host_ids"": [
				""1137751093""
			]
		}
	],
	""includes"": {
	""users"": [
			{
		""id"": ""1109717717895044328"",
				""created_at"": ""2019-03-24T07:24:56.000Z"",
				""profile_image_url"": ""https://pbs.twimg.com/profile_images/1445704677840994314/UpZdlVbD_normal.jpg"",
				""verified"": false,
				""name"": ""shegraegaga"",
				""entities"": {
			""url"": {
				""urls"": [
							{
					""start"": 0,
								""end"": 23,
								""url"": ""https://t.co/G6pm5Gmar0"",
								""expanded_url"": ""http://gradioofficiel.com/"",
								""display_url"": ""gradioofficiel.com""
							}
						]
					},
					""description"": {
				""urls"": [
							{
					""start"": 19,
								""end"": 42,
								""url"": ""https://t.co/EWcI1NBzT7"",
								""expanded_url"": ""http://facebook.com/GRadioOfficiel"",
								""display_url"": ""facebook.com/GRadioOfficiel""
							},
							{
					""start"": 43,
								""end"": 66,
								""url"": ""https://t.co/zduUUeNpOd"",
								""expanded_url"": ""http://instagram.com/gradioofficiel/"",
								""display_url"": ""instagram.com/gradioofficiel/""
							},
							{
					""start"": 67,
								""end"": 90,
								""url"": ""https://t.co/b1ydyQfqWV"",
								""expanded_url"": ""https://www.twitch.tv/gradioofficiel"",
								""display_url"": ""twitch.tv/gradioofficiel""
							}
						]
					}
		},
				""public_metrics"": {
			""followers_count"": 440,
					""following_count"": 4116,
					""tweet_count"": 28328,
					""listed_count"": 3
				},
				""protected"": false,
				""location"": ""France"",
				""description"": ""Suivez le Tempo ! \nhttps://t.co/EWcI1NBzT7\nhttps://t.co/zduUUeNpOd\nhttps://t.co/b1ydyQfqWV"",
				""pinned_tweet_id"": ""1243446272205426695"",
				""username"": ""jjsjhshrthr"",
				""url"": ""https://t.co/G6pm5Gmar0""
			},
			{
		""id"": ""1439813626060235432"",
				""created_at"": ""2021-09-20T04:48:59.000Z"",
				""profile_image_url"": ""https://pbs.twimg.com/profile_images/1443036575932239876/Z4buzkDB_normal.jpg"",
				""verified"": false,
				""name"": ""ASFASFf"",
				""public_metrics"": {
			""followers_count"": 5,
					""following_count"": 71,
					""tweet_count"": 18,
					""listed_count"": 0
				},
				""protected"": false,
				""description"": ""Pas trop républicain"",
				""username"": ""fafWFWEF"",
				""url"": """"
			},
			{
		""id"": ""1242778807930686789"",
				""created_at"": ""2020-03-25T11:42:43.000Z"",
				""profile_image_url"": ""https://pbs.twimg.com/profile_images/1393808717729210371/Cf8G8rhy_normal.jpg"",
				""verified"": false,
				""name"": ""⚜ afsfdaf ⚜"",
				""public_metrics"": {
			""followers_count"": 45,
					""following_count"": 121,
					""tweet_count"": 223,
					""listed_count"": 0
				},
				""protected"": false,
				""description"": """",
				""username"": ""sggWFwf"",
				""url"": """"
			}
		]
	},
	""meta"": {
	""result_count"": 3
	}
}";
    }
}
