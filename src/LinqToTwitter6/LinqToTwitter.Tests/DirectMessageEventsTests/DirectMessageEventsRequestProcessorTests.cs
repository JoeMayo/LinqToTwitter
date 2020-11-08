using LinqToTwitter.Common;
using LinqToTwitter.Common.Entities;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.DirectMessageEventsTests
{
    [TestClass]
    public class DirectMessageEventsRequestProcessorTests
    {
        public DirectMessageEventsRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void BuildUrl_WithShowType_ConstructsUrl()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents> { BaseUrl = "https://api.twitter.com/1.1/" };
            const string Expected = "https://api.twitter.com/1.1/direct_messages/events/show.json?id=1";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageEventsType.Show).ToString(CultureInfo.InvariantCulture) },
                        { "ID", "1" }
                };

            Request req = dmReqProc.BuildUrl(parameters);

            Assert.AreEqual(Expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_WithListType_ConstructsUrl()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents> { BaseUrl = "https://api.twitter.com/1.1/" };
            const string Expected = "https://api.twitter.com/1.1/direct_messages/events/list.json?count=2&cursor=ABC123";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageEventsType.List).ToString(CultureInfo.InvariantCulture) },
                        { "Count", "2" },
                        { "Cursor", "ABC123" }
                };

            Request req = dmReqProc.BuildUrl(parameters);

            Assert.AreEqual(Expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForShowWithoutID_Throws()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageEventsType.Show).ToString(CultureInfo.InvariantCulture) },
                        //{ "ID", "478805447" },
                };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => dmReqProc.BuildUrl(parameters));

            Assert.AreEqual("ID", ex.ParamName);
        }

        [TestMethod]
        public void ProcessResults_WithEmptyResults_ReturnsEmptyCollection()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents> { BaseUrl = "https://api.twitter.com/1.1/" };

            List<DirectMessageEvents> dms = dmReqProc.ProcessResults(string.Empty);

            Assert.IsFalse(dms.Any());
        }

        [TestMethod]
        public void ProcessResults_ForShow_HandlesSingleResult()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = DirectMessageEventsType.Show
            };

            List<DirectMessageEvents> dms = dmReqProc.ProcessResults(TestQuerySingleResponse);

            Assert.IsNotNull(dms?.SingleOrDefault());

            DirectMessageEvents dmEvt = dms.First();

            Assert.IsNotNull(dmEvt);
            DirectMessageEventsValue dmVal = dmEvt.Value;
            Assert.IsNotNull(dmVal);
            DMEvent evt = dmVal.DMEvent;
            Assert.IsNotNull(evt);
            Assert.AreEqual("message_create", evt.Type);
            Assert.AreEqual("917929712638246916", evt.ID);
            Assert.AreEqual("1507686472459", evt.CreatedTimestamp);
            Assert.AreEqual(new DateTime(2017, 10, 11, 01, 47, 52, 459), evt.CreatedAt);
            DirectMessageCreate mc = evt.MessageCreate;
            Assert.IsNotNull(mc);
            Assert.AreEqual("15411837", mc.SenderID);
            Assert.AreEqual("472356", mc.SourceAppID);
            DirectMessageTarget tgt = mc.Target;
            Assert.IsNotNull(tgt);
            Assert.AreEqual("16761255", tgt.RecipientID);
            DirectMessageData md = mc.MessageData;
            Assert.IsNotNull(md);
            Assert.AreEqual(
                "DM from @JoeMayo to @Linq2Twitr of $MSFT &amp; $TSLA with #TwitterAPI #chatbot at https://t.co/ikGq2AnAir and https://t.co/nLvwwjz40q on 10/10/2017 6:47:46 PM!'",
                md.Text);
            Entities ent = md.Entities;
            Assert.IsNotNull(ent);
            List<HashTagEntity> hashtags = ent.HashTagEntities;
            Assert.IsNotNull(hashtags);
            Assert.AreEqual(2, hashtags.Count);
            HashTagEntity ht1 = hashtags[0];
            Assert.IsNotNull(ht1);
            Assert.AreEqual("TwitterAPI", ht1.Text);
            int[] ht1i = ht1.Indices;
            Assert.IsNotNull(ht1i);
            Assert.AreEqual(58, ht1i[0]);
            Assert.AreEqual(69, ht1i[1]);
            HashTagEntity ht2 = hashtags[1];
            Assert.IsNotNull(ht2);
            Assert.AreEqual("chatbot", ht2.Text);
            int[] ht2i = ht2.Indices;
            Assert.IsNotNull(ht2i);
            Assert.AreEqual(70, ht2i[0]);
            Assert.AreEqual(78, ht2i[1]);
            List<SymbolEntity> syms = ent.SymbolEntities;
            Assert.IsNotNull(syms);
            Assert.AreEqual(2, syms.Count);
            SymbolEntity sym1 = syms[0];
            Assert.IsNotNull(sym1);
            Assert.AreEqual("MSFT", sym1.Text);
            int[] sym1i = sym1.Indices;
            Assert.IsNotNull(sym1i);
            Assert.AreEqual(35, sym1i[0]);
            Assert.AreEqual(40, sym1i[1]);
            SymbolEntity sym2 = syms[1];
            Assert.IsNotNull(sym2);
            Assert.AreEqual("TSLA", sym2.Text);
            int[] sym2i = sym2.Indices;
            Assert.IsNotNull(sym2i);
            Assert.AreEqual(47, sym2i[0]);
            Assert.AreEqual(52, sym2i[1]);
            List<UserMentionEntity> usrs = ent.UserMentionEntities;
            Assert.IsNotNull(usrs);
            Assert.AreEqual(2, usrs.Count);
            UserMentionEntity um1 = usrs[0];
            Assert.IsNotNull(um1);
            Assert.AreEqual("JoeMayo", um1.ScreenName);
            Assert.AreEqual("Joe Mayo", um1.Name);
            Assert.AreEqual(15411837ul, um1.Id);
            Assert.AreEqual("15411837", um1.IdStr);
            int[] um1i = um1.Indices;
            Assert.IsNotNull(um1i);
            Assert.AreEqual(2, um1i.Length);
            Assert.AreEqual(8, um1i[0]);
            Assert.AreEqual(16, um1i[1]);
            UserMentionEntity um2 = usrs[1];
            Assert.IsNotNull(um2);
            Assert.AreEqual("Linq2Twitr", um2.ScreenName);
            Assert.AreEqual("LINQ to Twitr", um2.Name);
            Assert.AreEqual(16761255ul, um2.Id);
            Assert.AreEqual("16761255", um2.IdStr);
            int[] um2i = um2.Indices;
            Assert.IsNotNull(um2i);
            Assert.AreEqual(2, um2i.Length);
            Assert.AreEqual(20, um2i[0]);
            Assert.AreEqual(31, um2i[1]);
            List<UrlEntity> urls = ent.UrlEntities;
            Assert.IsNotNull(urls);
            Assert.AreEqual(2, urls.Count);
            UrlEntity url1 = urls[0];
            Assert.IsNotNull(url1);
            Assert.AreEqual("https://t.co/ikGq2AnAir", url1.Url);
            Assert.AreEqual("http://bit.ly/2xSJWJk", url1.ExpandedUrl);
            Assert.AreEqual("bit.ly/2xSJWJk", url1.DisplayUrl);
            int[] url1i = url1.Indices;
            Assert.IsNotNull(url1i);
            Assert.AreEqual(2, url1i.Length);
            Assert.AreEqual(82, url1i[0]);
            Assert.AreEqual(105, url1i[1]);
            UrlEntity url2 = urls[1];
            Assert.IsNotNull(url2);
            Assert.AreEqual("https://t.co/nLvwwjz40q", url2.Url);
            Assert.AreEqual("http://amzn.to/2gD09X6", url2.ExpandedUrl);
            Assert.AreEqual("amzn.to/2gD09X6", url2.DisplayUrl);
            int[] url2i = url2.Indices;
            Assert.IsNotNull(url2i);
            Assert.AreEqual(2, url2i.Length);
            Assert.AreEqual(110, url2i[0]);
            Assert.AreEqual(133, url2i[1]);
            JsonElement app = dmVal.Apps;
            JsonElement appDetail = app.GetProperty("472356");
            Assert.AreEqual("472356", appDetail.GetString("id"));
            Assert.AreEqual("LINQ to Tweeter", appDetail.GetString("name"));
            Assert.AreEqual("https://github.com/JoeMayo/LinqToTwitter", appDetail.GetString("url"));
        }

        [TestMethod]
        public void ProcessResults_ForList_HandlesMultipleResults()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = DirectMessageEventsType.List
            };

            List<DirectMessageEvents> dms = dmReqProc.ProcessResults(TestQueryMultipleResponses);

            Assert.IsNotNull(dms?.SingleOrDefault());

            DirectMessageEvents dmEvt = dms.First();

            Assert.IsNotNull(dmEvt);
            DirectMessageEventsValue dmVal = dmEvt.Value;
            Assert.IsNotNull(dmVal);
            List<DMEvent> evts = dmVal.DMEvents;
            Assert.IsNotNull(evts);
            Assert.AreEqual(2, evts.Count);
            Assert.AreEqual("OTE3ODE0NTUzMzExOTMyNDIy", dmVal.NextCursor);
        }

        [TestMethod]
        public void ProcessResults_WithShowQuery_ReplacesInputParams()
        {
            const ulong ID = 5;

            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = DirectMessageEventsType.Show,
                ID = ID
            };

            List<DirectMessageEvents> dms = dmReqProc.ProcessResults(TestQuerySingleResponse);

            DirectMessageEvents dm = dms.First();

            Assert.AreEqual(ID, dm.ID);
        }

        [TestMethod]
        public void ProcessResults_WithListQuery_ReplacesInputParams()
        {
            const int Count = 4;
            const string Cursor = "ABC123";

            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = DirectMessageEventsType.List,
                Count = Count,
                Cursor = Cursor
            };

            List<DirectMessageEvents> dms = dmReqProc.ProcessResults(TestQueryMultipleResponses);

            DirectMessageEvents dm = dms.First();

            Assert.AreEqual(Count, dm.Count);
            Assert.AreEqual(Cursor, dm.Cursor);
        }

        [TestMethod]
        public void GetParameters_Returns_Parameters()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents>();
            Expression<Func<DirectMessageEvents, bool>> expression =
                dm =>
                    dm.Type == DirectMessageEventsType.Show &&
                    dm.Count == 1 &&
                    dm.Cursor == "ABC123" &&
                    dm.ID == 456;
            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = dmReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)DirectMessageEventsType.Show).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Count", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "ABC123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "456")));
        }

        [TestMethod]
        public void BuildUrl_WithMissingType_Throws()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>();

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => dmReqProc.BuildUrl(parameters));

            Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_WithNullParamDictionary_Throws()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents> { BaseUrl = "https://api.twitter.com/1.1/" };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => dmReqProc.BuildUrl(null));

            Assert.AreEqual("Type", ex.ParamName);
        }

        const string TestQuerySingleResponse = @"{
	""event"": {
		""type"": ""message_create"",
		""id"": ""917929712638246916"",
		""created_timestamp"": ""1507686472459"",
		""message_create"": {
            ""target"": {
                ""recipient_id"": ""16761255""

            },
			""sender_id"": ""15411837"",
			""source_app_id"": ""472356"",
			""message_data"": {
                ""text"": ""DM from @JoeMayo to @Linq2Twitr of $MSFT &amp; $TSLA with #TwitterAPI #chatbot at https:\/\/t.co\/ikGq2AnAir and https:\/\/t.co\/nLvwwjz40q on 10\/10\/2017 6:47:46 PM!'"",
				""entities"": {
                    ""hashtags"": [{
						""text"": ""TwitterAPI"",
						""indices"": [58,
						69]

                    },
					{
						""text"": ""chatbot"",
						""indices"": [70,
						78]
    }],
					""symbols"": [{
						""text"": ""MSFT"",
						""indices"": [35,
						40]
},
					{
						""text"": ""TSLA"",
						""indices"": [47,
						52]
					}],
					""user_mentions"": [{
						""screen_name"": ""JoeMayo"",
						""name"": ""Joe Mayo"",
						""id"": 15411837,
						""id_str"": ""15411837"",
						""indices"": [8,
						16]
					},
					{
						""screen_name"": ""Linq2Twitr"",
						""name"": ""LINQ to Twitr"",
						""id"": 16761255,
						""id_str"": ""16761255"",
						""indices"": [20,
						31]
					}],
					""urls"": [{
						""url"": ""https:\/\/t.co\/ikGq2AnAir"",
						""expanded_url"": ""http:\/\/bit.ly\/2xSJWJk"",
						""display_url"": ""bit.ly\/2xSJWJk"",
						""indices"": [82,
						105]
					},
					{
						""url"": ""https:\/\/t.co\/nLvwwjz40q"",
						""expanded_url"": ""http:\/\/amzn.to\/2gD09X6"",
						""display_url"": ""amzn.to\/2gD09X6"",
						""indices"": [110,
						133]
					}]
				}
			}
		}
	},
	""apps"": {
		""472356"": {
			""id"": ""472356"",
			""name"": ""LINQ to Tweeter"",
			""url"": ""https:\/\/github.com\/JoeMayo\/LinqToTwitter""
		}
	}
}";

        public const string TestQueryMultipleResponses = @"{
	""events"": [{
		""type"": ""message_create"",
		""id"": ""918326071484805124"",
		""created_timestamp"": ""1507780971772"",
		""message_create"": {
			""target"": {
				""recipient_id"": ""16761255""
			},
			""sender_id"": ""15411837"",
			""source_app_id"": ""472356"",
			""message_data"": {
				""text"": ""DM from @JoeMayo to @Linq2Twitr of $MSFT &amp; $TSLA with #TwitterAPI #chatbot at https:\/\/t.co\/ikGq2AnAir and https:\/\/t.co\/nLvwwjz40q on 10\/11\/2017 9:02:49 PM!'"",
				""entities"": {
					""hashtags"": [{
						""text"": ""TwitterAPI"",
						""indices"": [58,
						69]
    },
					{
						""text"": ""chatbot"",
						""indices"": [70,
						78]
}],
					""symbols"": [{
						""text"": ""MSFT"",
						""indices"": [35,
						40]
					},
					{
						""text"": ""TSLA"",
						""indices"": [47,
						52]
					}],
					""user_mentions"": [{
						""screen_name"": ""JoeMayo"",
						""name"": ""Joe Mayo"",
						""id"": 15411837,
						""id_str"": ""15411837"",
						""indices"": [8,
						16]
					},
					{
						""screen_name"": ""Linq2Twitr"",
						""name"": ""LINQ to Twitr"",
						""id"": 16761255,
						""id_str"": ""16761255"",
						""indices"": [20,
						31]
					}],
					""urls"": [{
						""url"": ""https:\/\/t.co\/ikGq2AnAir"",
						""expanded_url"": ""http:\/\/bit.ly\/2xSJWJk"",
						""display_url"": ""bit.ly\/2xSJWJk"",
						""indices"": [82,
						105]
					},
					{
						""url"": ""https:\/\/t.co\/nLvwwjz40q"",
						""expanded_url"": ""http:\/\/amzn.to\/2gD09X6"",
						""display_url"": ""amzn.to\/2gD09X6"",
						""indices"": [110,
						133]
					}]
				}
			}
		}
	},
	{
		""type"": ""message_create"",
		""id"": ""918309059970842629"",
		""created_timestamp"": ""1507776915911"",
		""message_create"": {
			""target"": {
				""recipient_id"": ""16761255""
			},
			""sender_id"": ""15411837"",
			""source_app_id"": ""472356"",
			""message_data"": {
				""text"": ""DM from @JoeMayo to @Linq2Twitr of $MSFT &amp; $TSLA with #TwitterAPI #chatbot at https:\/\/t.co\/ikGq2AnAir and https:\/\/t.co\/nLvwwjz40q on 10\/11\/2017 7:55:13 PM!'"",
				""entities"": {
					""hashtags"": [{
						""text"": ""TwitterAPI"",
						""indices"": [58,
						69]
					},
					{
						""text"": ""chatbot"",
						""indices"": [70,
						78]
					}],
					""symbols"": [{
						""text"": ""MSFT"",
						""indices"": [35,
						40]
					},
					{
						""text"": ""TSLA"",
						""indices"": [47,
						52]
					}],
					""user_mentions"": [{
						""screen_name"": ""JoeMayo"",
						""name"": ""Joe Mayo"",
						""id"": 15411837,
						""id_str"": ""15411837"",
						""indices"": [8,
						16]
					},
					{
						""screen_name"": ""Linq2Twitr"",
						""name"": ""LINQ to Twitr"",
						""id"": 16761255,
						""id_str"": ""16761255"",
						""indices"": [20,
						31]
					}],
					""urls"": [{
						""url"": ""https:\/\/t.co\/ikGq2AnAir"",
						""expanded_url"": ""http:\/\/bit.ly\/2xSJWJk"",
						""display_url"": ""bit.ly\/2xSJWJk"",
						""indices"": [82,
						105]
					},
					{
						""url"": ""https:\/\/t.co\/nLvwwjz40q"",
						""expanded_url"": ""http:\/\/amzn.to\/2gD09X6"",
						""display_url"": ""amzn.to\/2gD09X6"",
						""indices"": [110,
						133]
					}]
				}
			}
		}
	}],
	""apps"": {
		""268278"": {
			""id"": ""268278"",
			""name"": ""Twitter Web Client"",
			""url"": ""http:\/\/twitter.com""
		},
		""472356"": {
			""id"": ""472356"",
			""name"": ""LINQ to Tweeter"",
			""url"": ""https:\/\/github.com\/JoeMayo\/LinqToTwitter""
		}
	},
	""next_cursor"": ""OTE3ODE0NTUzMzExOTMyNDIy""
}";

    }
}
