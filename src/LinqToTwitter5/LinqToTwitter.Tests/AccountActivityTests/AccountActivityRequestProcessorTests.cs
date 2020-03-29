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
    public class AccountActivityRequestProcessorTests
    {
        public AccountActivityRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void BuildUrl_WithSubscriptionsType_ConstructsUrl()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity> { BaseUrl = "https://api.twitter.com/1.1/" };
            const string Expected = "https://api.twitter.com/1.1/account_activity/webhooks/1/subscriptions.json";
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)AccountActivityType.Subscriptions).ToString(CultureInfo.InvariantCulture) },
                        { "WebhookID", "1" }
                };

            Request req = accActReqProc.BuildUrl(parameters);

            Assert.AreEqual(Expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_WithWebhooksType_ConstructsUrl()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity> { BaseUrl = "https://api.twitter.com/1.1/" };
            const string Expected = "https://api.twitter.com/1.1/account_activity/webhooks.json";
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)AccountActivityType.Webhooks).ToString(CultureInfo.InvariantCulture) }
                };

            Request req = accActReqProc.BuildUrl(parameters);

            Assert.AreEqual(Expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForSubscriptionsWithoutWebhookID_Throws()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)AccountActivityType.Subscriptions).ToString(CultureInfo.InvariantCulture) },
                        //{ "WebhookID", "478805447" },
                };

            ArgumentNullException ex = L2TAssert.Throws<ArgumentNullException>(() => accActReqProc.BuildUrl(parameters));

            Assert.AreEqual("WebhookID", ex.ParamName);
        }

        [TestMethod]
        public void ProcessResults_ForWebhooks_HandlesSingleResult()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity>
            { 
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = AccountActivityType.Webhooks
            };

            List<AccountActivity> accActs = accActReqProc.ProcessResults(WebhooksResponse);

            Assert.IsNotNull(accActs?.SingleOrDefault());

            AccountActivity accAct = accActs.First();

            Assert.IsNotNull(accAct);
            WebhooksValue webhooksVal = accAct.WebhooksValue;
            Assert.IsNotNull(webhooksVal);
            Webhook[] webhooks = webhooksVal.Webhooks;
            Assert.IsNotNull(webhooks);
            Assert.AreEqual(1, webhooks.Length);
            Webhook webhook = webhooks.First();
            Assert.IsNotNull(webhook);
            Assert.AreEqual("920835776169910272", webhook.ID);
            Assert.AreEqual("https://accountactivitydemo.azurewebsites.net/api/accountactivity", webhook.Url);
            Assert.IsTrue(webhook.Valid);
            Assert.AreEqual("2017-10-19 02:15:32 +0000", webhook.CreatedTimestamp);
        }
        
        [TestMethod]
        public void ProcessResults_ForSubscriptions_HandlesResult()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = AccountActivityType.Subscriptions,
                WebhookID = 1
            };

            List<AccountActivity> accActs = accActReqProc.ProcessResults("");

            AccountActivity accAct = accActs?.SingleOrDefault();
            Assert.IsNotNull(accAct);
            SubscriptionValue subsVal = accAct.SubscriptionValue;
            Assert.IsNotNull(subsVal);
            Assert.IsTrue(subsVal.IsSubscribed);
        }

        [TestMethod]
        public void ProcessResults_WithWebhooksQuery_ReplacesInputParams()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = AccountActivityType.Webhooks
            };

            List<AccountActivity> accActList = accActReqProc.ProcessResults(WebhooksResponse);

            AccountActivity accAct = accActList.First();

            Assert.AreEqual(AccountActivityType.Webhooks, accAct.Type);
        }

        [TestMethod]
        public void ProcessResults_WithSubscriptionsQuery_ReplacesInputParams()
        {
            const ulong WebhookID = 4;

            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity>
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = AccountActivityType.Subscriptions,
                WebhookID = WebhookID
            };

            List<AccountActivity> accActs = accActReqProc.ProcessResults("");

            AccountActivity accAct = accActs.First();
            Assert.AreEqual(WebhookID, accAct.WebhookID);
        }

        [TestMethod]
        public void GetParameters_Returns_Parameters()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity>();
            Expression<Func<AccountActivity, bool>> expression =
                dm =>
                    dm.Type == AccountActivityType.Subscriptions &&
                    dm.WebhookID == 1;
            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = accActReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)AccountActivityType.Subscriptions).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("WebhookID", "1")));
        }

        [TestMethod]
        public void BuildUrl_WithMissingType_Throws()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>();

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => accActReqProc.BuildUrl(parameters));

            Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_WithNullParamDictionary_Throws()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity> { BaseUrl = "https://api.twitter.com/1.1/" };

            ArgumentException ex = L2TAssert.Throws<ArgumentException>(() => accActReqProc.BuildUrl(null));

            Assert.AreEqual("Type", ex.ParamName);
        }

        const string WebhooksResponse = @"[{
	""id"": ""920835776169910272"",
	""url"": ""https://accountactivitydemo.azurewebsites.net/api/accountactivity"",
	""valid"": true,
	""created_timestamp"": ""2017-10-19 02:15:32 +0000""
}]";
    }
}
