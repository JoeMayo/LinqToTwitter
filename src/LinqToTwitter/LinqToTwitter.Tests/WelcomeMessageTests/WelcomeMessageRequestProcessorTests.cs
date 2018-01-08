using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace LinqToTwitterPcl.Tests.DirectMessageTests
{
    [TestClass]
    public class WelcomeMessageRequestProcessorTests
    {
        public WelcomeMessageRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_Returns_Parameters()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>();
            Expression<Func<WelcomeMessage, bool>> expression =
                wm =>
                    wm.Type == WelcomeMessageType.ListMessages &&
                    wm.Count == 1 &&
                    wm.Cursor == "ABC123" &&
                    wm.ID == 456;
            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = wmReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)WelcomeMessageType.ListMessages).ToString(CultureInfo.InvariantCulture))));
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
        public void BuildUrl_WithListMessages_ConstructsUrl()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> { BaseUrl = "https://api.twitter.com/1.1/" };
            const string Expected = "https://api.twitter.com/1.1/direct_messages/welcome_messages/list.json?count=2&cursor=ABC123";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)WelcomeMessageType.ListMessages).ToString(CultureInfo.InvariantCulture) },
                        { "Count", "2" },
                        { "Cursor", "ABC123" }
                };

            Request req = wmReqProc.BuildUrl(parameters);

            Assert.AreEqual(Expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_WithListRules_ConstructsUrl()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> { BaseUrl = "https://api.twitter.com/1.1/" };
            const string Expected = "https://api.twitter.com/1.1/direct_messages/welcome_messages/rules/list.json?count=2&cursor=ABC123";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)WelcomeMessageType.ListRules).ToString(CultureInfo.InvariantCulture) },
                        { "Count", "2" },
                        { "Cursor", "ABC123" }
                };

            Request req = wmReqProc.BuildUrl(parameters);

            Assert.AreEqual(Expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForShowMessageWithoutID_Throws()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)WelcomeMessageType.ShowMessage).ToString(CultureInfo.InvariantCulture) },
                        //{ "ID", "478805447" },
                };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => wmReqProc.BuildUrl(parameters));

            Assert.AreEqual("ID", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_WithShowMessage_ConstructsUrl()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> { BaseUrl = "https://api.twitter.com/1.1/" };
            const string Expected = "https://api.twitter.com/1.1/direct_messages/welcome_messages/show.json?id=1";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)WelcomeMessageType.ShowMessage).ToString(CultureInfo.InvariantCulture) },
                        { "ID", "1" }
                };

            Request req = wmReqProc.BuildUrl(parameters);

            Assert.AreEqual(Expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForShowRuleWithoutID_Throws()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)WelcomeMessageType.ShowRule).ToString(CultureInfo.InvariantCulture) },
                        //{ "ID", "478805447" },
                };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => wmReqProc.BuildUrl(parameters));

            Assert.AreEqual("ID", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_WithShowRule_ConstructsUrl()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> { BaseUrl = "https://api.twitter.com/1.1/" };
            const string Expected = "https://api.twitter.com/1.1/direct_messages/welcome_messages/rules/show.json?id=1";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)WelcomeMessageType.ShowRule).ToString(CultureInfo.InvariantCulture) },
                        { "ID", "1" }
                };

            Request req = wmReqProc.BuildUrl(parameters);

            Assert.AreEqual(Expected, req.FullUrl);
        }

        [TestMethod]
        public void ProcessResults_WithEmptyResults_ReturnsEmptyCollection()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> { BaseUrl = "https://api.twitter.com/1.1/" };

            List<WelcomeMessage> msgs = wmReqProc.ProcessResults(string.Empty);

            Assert.IsFalse(msgs.Any());
        }

        [TestMethod]
        public void ProcessResults_ForListWelcomeMessages_HandlesMultipleResults()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = WelcomeMessageType.ListMessages
            };

            List<WelcomeMessage> msgs = wmReqProc.ProcessResults(TestQueryMultipleWelcomeMessageResponses);

            WelcomeMessage welcomeMsg = msgs?.First();
            Assert.IsNotNull(welcomeMsg);
            WelcomeMessageValue val = welcomeMsg.Value;
            Assert.IsNotNull(val);
            List<WelcomeMsg> msgList = val.WelcomeMessages;
            WelcomeMsg msg = msgList.FirstOrDefault();
            Assert.IsNotNull(msg);
            Assert.AreEqual("New Welcome Message", msg.Name);
            WelcomeMessageData data = msg.MessageData;
            Assert.IsNotNull(data);
            Assert.AreEqual("Welcome!", data.Text);
        }

        [TestMethod]
        public void ProcessResults_ForListRules_HandlesMultipleResults()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = WelcomeMessageType.ListMessages
            };

            List<WelcomeMessage> msgs = wmReqProc.ProcessResults(TestQueryMultipleRuleResponses);

            WelcomeMessage welcomeMsg = msgs.FirstOrDefault();
            Assert.IsNotNull(welcomeMsg);
            WelcomeMessageValue val = welcomeMsg.Value;
            Assert.IsNotNull(val);
            List<WelcomeMessageRule> rules = val.WelcomeMessageRules;
            Assert.IsNotNull(rules);
            WelcomeMessageRule rule = rules.FirstOrDefault();
            Assert.IsNotNull(rule);
            Assert.AreEqual("949450720239001600", rule.ID);
            Assert.AreEqual("1515201666303", rule.CreatedTimestamp);
            Assert.AreEqual("949450173926617092", rule.WelcomeMessageID);
        }

        [TestMethod]
        public void ProcessResults_ForShowWelcomeMessage_HandlesSingleResult()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> 
            { 
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = WelcomeMessageType.ShowMessage
            };

            List<WelcomeMessage> msgs = wmReqProc.ProcessResults(TestQuerySingleWelcomeMessageResponse);

            WelcomeMessage welcomeMsg = msgs?.First();
            Assert.IsNotNull(welcomeMsg);
            WelcomeMessageValue val = welcomeMsg.Value;
            Assert.IsNotNull(val);
            WelcomeMsg msg = val.WelcomeMessage;
            Assert.IsNotNull(msg);
            Assert.AreEqual("950134376347574276", msg.Id);
            Assert.AreEqual("1515364662621", msg.CreatedTimestamp);
            Assert.AreEqual("472356", msg.SourceAppId);
            Assert.AreEqual("New Welcome Message", msg.Name);
            WelcomeMessageData data = msg.MessageData;
            Assert.IsNotNull(data);
            Assert.AreEqual("Welcome!", data.Text);
            JObject app = val.Apps;
            JObject appDetail = app.Value<JObject>("472356");
            Assert.AreEqual("472356", appDetail.Value<string>("id"));
            Assert.AreEqual("LINQ to Tweeter", appDetail.Value<string>("name"));
            Assert.AreEqual("https://github.com/JoeMayo/LinqToTwitter", appDetail.Value<string>("url"));
        }

        [TestMethod]
        public void ProcessResults_ForShowWelcomeMessageRule_HandlesSingleResult()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = WelcomeMessageType.ShowMessage
            };

            List<WelcomeMessage> msgs = wmReqProc.ProcessResults(TestQuerySingleWelcomeMessageRuleResponse);

            WelcomeMessage welcomeMsg = msgs.FirstOrDefault();
            Assert.IsNotNull(welcomeMsg);
            WelcomeMessageValue val = welcomeMsg.Value;
            Assert.IsNotNull(val);
            WelcomeMessageRule rule = val.WelcomeMessageRule;
            Assert.IsNotNull(rule);
            Assert.AreEqual("950225380517937152", rule.ID);
            Assert.AreEqual("1515386359706", rule.CreatedTimestamp);
            Assert.AreEqual("950134376347574276", rule.WelcomeMessageID);
        }

        [TestMethod]
        public void ProcessResults_WithShowQuery_ReplacesInputParams()
        {
            const ulong ID = 5;

            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = WelcomeMessageType.ShowMessage,
                ID = ID
            };

            List<WelcomeMessage> msgs = wmReqProc.ProcessResults(TestQuerySingleWelcomeMessageResponse);

            WelcomeMessage wm = msgs.First();

            Assert.AreEqual(ID, wm.ID);
        }

        [TestMethod]
        public void ProcessResults_WithListQuery_ReplacesInputParams()
        {
            const int Count = 4;
            const string Cursor = "ABC123";

            var dmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = WelcomeMessageType.ListMessages,
                Count = Count,
                Cursor = Cursor
            };

            List<WelcomeMessage> msgs = dmReqProc.ProcessResults(TestQueryMultipleWelcomeMessageResponses);

            WelcomeMessage wm = msgs.First();

            Assert.AreEqual(Count, wm.Count);
            Assert.AreEqual(Cursor, wm.Cursor);
        }

        [TestMethod]
        public void BuildUrl_WithMissingType_Throws()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>();

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => wmReqProc.BuildUrl(parameters));

            Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_WithNullParamDictionary_Throws()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage> { BaseUrl = "https://api.twitter.com/1.1/" };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => wmReqProc.BuildUrl(null));

            Assert.AreEqual("Type", ex.ParamName);
        }

        #region Response Strings

        public const string TestQuerySingleWelcomeMessageResponse = @"{
	""welcome_message"": {
		""id"": ""950134376347574276"",
		""created_timestamp"": ""1515364662621"",
		""message_data"": {
			""text"": ""Welcome!"",
			""entities"": {
				""hashtags"": [],
				""symbols"": [],
				""user_mentions"": [],
				""urls"": []
            }
        },
		""source_app_id"": ""472356"",
		""name"": ""New Welcome Message""
	},
	""apps"": {
		""472356"": {
			""id"": ""472356"",
			""name"": ""LINQ to Tweeter"",
			""url"": ""https:\/\/github.com\/JoeMayo\/LinqToTwitter""
		}
	}
}";

        const string TestQuerySingleWelcomeMessageRuleResponse = @"{
	""welcome_message_rule"": {
		""id"": ""950225380517937152"",
		""created_timestamp"": ""1515386359706"",
		""welcome_message_id"": ""950134376347574276""
	}
}";

        public const string TestQueryMultipleWelcomeMessageResponses = @"{
	""next_cursor"": ""OTQ5NzczMTIzOTc2NDQ1OTU3"",
	""welcome_messages"": [{
		""id"": ""945484689657102340"",
		""created_timestamp"": ""1514256090927"",
		""message_data"": {
			""text"": ""Welcome!"",
			""entities"": {
				""hashtags"": [],
				""symbols"": [],
				""user_mentions"": [],
				""urls"": []
    }
},
		""source_app_id"": ""472356"",
		""name"": ""New Welcome Message""
	},
	{
		""id"": ""949450173926617092"",
		""created_timestamp"": ""1515201536052"",
		""message_data"": {
			""text"": ""Welcome!"",
			""entities"": {
				""hashtags"": [],
				""symbols"": [],
				""user_mentions"": [],
				""urls"": []
			}
		},
		""source_app_id"": ""472356"",
		""name"": ""New Welcome Message""
	},
	{
		""id"": ""949765733034033156"",
		""created_timestamp"": ""1515276771206"",
		""message_data"": {
			""text"": ""Welcome!"",
			""entities"": {
				""hashtags"": [],
				""symbols"": [],
				""user_mentions"": [],
				""urls"": []
			}
		},
		""source_app_id"": ""472356"",
		""name"": ""New Welcome Message""
	}],
	""apps"": {
		""472356"": {
			""id"": ""472356"",
			""name"": ""LINQ to Tweeter"",
			""url"": ""https:\/\/github.com\/JoeMayo\/LinqToTwitter""
		}
	}
}";

        public const string TestQueryMultipleRuleResponses = @"{
	""welcome_message_rules"": [{
		""id"": ""949450720239001600"",
		""created_timestamp"": ""1515201666303"",
		""welcome_message_id"": ""949450173926617092""
	}]
}";

        #endregion
    }
}
