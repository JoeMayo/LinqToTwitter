using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Net.Http;

namespace LinqToTwitterPcl.Tests.DirectMessageTests
{
    [TestClass]
    public class AccountActivityCommandsTests
    {
        Mock<IAuthorizer> authMock;
        Mock<ITwitterExecute> execMock;

        public AccountActivityCommandsTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void AccountActivityRequestProcessor_Works_With_Actions()
        {
            var accActReqProc = new AccountActivityRequestProcessor<AccountActivity>();

            Assert.IsInstanceOfType(accActReqProc, typeof(IRequestProcessorWithAction<AccountActivity>));
        }

        TwitterContext InitializeTwitterContext()
        {
            authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(AccountActivityResponse);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec => exec.SendJsonToTwitterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<AccountActivity>(),
                It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            execMock.Setup(exec => exec.PostFormUrlEncodedToTwitterAsync<AccountActivity>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task AddAccountActivitySubscriptionAsync_WithWebhookParameter_PopulatesInputParametersInResponse()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            AccountActivity accAct = await ctx.AddAccountActivitySubscriptionAsync(WebhookID);

            Assert.IsNotNull(accAct);
            Assert.AreEqual(WebhookID, accAct.WebhookID);
        }

        [TestMethod]
        public async Task AddAccountActivitySubscriptionAsync_WithWebhookID_ReturnsCreatedAccountActivity()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            AccountActivity accAct = await ctx.AddAccountActivitySubscriptionAsync(WebhookID);

            Assert.IsNotNull(accAct);
            SubscriptionValue subsVal = accAct.SubscriptionValue;
            Assert.IsNotNull(subsVal);
            Assert.IsTrue(subsVal.IsSubscribed);
        }

        [TestMethod]
        public async Task AddAccountActivitySubscriptionAsync_WithValidParameters_LeavesNullRawResult()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.AddAccountActivitySubscriptionAsync(WebhookID);

            Assert.IsNull(ctx.RawResult);
        }

        [TestMethod]
        public async Task AddAccountActivitySubscriptionAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.AddAccountActivitySubscriptionAsync(WebhookID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/account_activity/webhooks/1/subscriptions.json",
                    It.IsAny<AccountActivityValue>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task AddAccountActivitySubscriptionAsync_WithZeroWebhookID_Throws()
        {
            const int WebhookID = 0;
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddAccountActivitySubscriptionAsync(WebhookID));

            Assert.AreEqual("webhookID", ex.ParamName);
        }

        [TestMethod]
        public async Task AddAccountActivityWebhookAsync_WithUrlParameter_PopulatesInputParametersInResponse()
        {
            const string Url = "https://www.example.com/api/accountactivity";
            var ctx = InitializeTwitterContext();

            AccountActivity accAct = await ctx.AddAccountActivityWebhookAsync(Url);

            Assert.IsNotNull(accAct);
            Assert.AreEqual(Url, accAct.Url);
        }

        [TestMethod]
        public async Task AddAccountActivityWebhookAsync_WithUrlParameter_ReturnsCreatedAccountActivity()
        {
            const string Url = "https://www.example.com/api/accountactivity";
            var ctx = InitializeTwitterContext();

            AccountActivity accAct = await ctx.AddAccountActivityWebhookAsync(Url);

            Assert.IsNotNull(accAct);
            WebhooksValue accActVal = accAct.WebhooksValue;
            Assert.IsNotNull(accActVal);
            Webhook[] webhooks = accActVal.Webhooks;
            Assert.IsNotNull(webhooks);
            Assert.AreEqual(1, webhooks.Length);
            Webhook webhook = webhooks[0];
            Assert.IsNotNull(webhook);
            Assert.AreEqual("921246280768151552", webhook.ID);
            Assert.AreEqual("https://accountactivitydemo.azurewebsites.net/api/accountactivity", webhook.Url);
            Assert.IsTrue(webhook.Valid);
            Assert.AreEqual("2017-10-20 05:26:44 +0000", webhook.CreatedTimestamp);
        }

        [TestMethod]
        public async Task AddAccountActivityWebhookAsync_WithValidParameters_PopulatesRawResult()
        {
            const string Url = "https://www.example.com/api/accountactivity";
            var ctx = InitializeTwitterContext();

            await ctx.AddAccountActivityWebhookAsync(Url);

            Assert.AreEqual(AccountActivityResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task AddAccountActivityWebhookAsync_WithValidParameters_ConstructsUrl()
        {
            const string Url = "https://www.example.com/api/accountactivity";
            var ctx = InitializeTwitterContext();

            await ctx.AddAccountActivityWebhookAsync(Url);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<AccountActivity>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/account_activity/webhooks.json?url=https://www.example.com/api/accountactivity",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task AddAccountActivityWebhookAsync_WithNullUrl_Throws()
        {
            const string Url = null;
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddAccountActivityWebhookAsync(Url));

            Assert.AreEqual("url", ex.ParamName);
        }

        [TestMethod]
        public async Task AddAccountActivityWebhookAsync_WithEmptyUrl_Throws()
        {
            const string Url = "";
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.AddAccountActivityWebhookAsync(Url));

            Assert.AreEqual("url", ex.ParamName);
        }

        [TestMethod]
        public async Task SendAccountActivityCrcAsync_WithWebhookParameter_PopulatesInputParametersInResponse()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            AccountActivity accAct = await ctx.SendAccountActivityCrcAsync(WebhookID);

            Assert.IsNotNull(accAct);
            Assert.AreEqual(WebhookID, accAct.WebhookID);
        }

        [TestMethod]
        public async Task SendAccountActivityCrcAsync_WithWebhookID_ReturnsCreatedAccountActivity()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            AccountActivity accAct = await ctx.SendAccountActivityCrcAsync(WebhookID);

            Assert.IsNotNull(accAct);
        }

        [TestMethod]
        public async Task SendAccountActivityCrcAsync_WithValidParameters_SetsNullRawResult()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.SendAccountActivityCrcAsync(WebhookID);

            Assert.IsNull(ctx.RawResult);
        }

        [TestMethod]
        public async Task SendAccountActivityCrcAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.SendAccountActivityCrcAsync(WebhookID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Put.ToString(),
                    "https://api.twitter.com/1.1/account_activity/webhooks/1.json",
                    It.IsAny<AccountActivityValue>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task SendAccountActivityCrcAsync_WithZeroWebhookID_Throws()
        {
            const int WebhookID = 0;
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.SendAccountActivityCrcAsync(WebhookID));

            Assert.AreEqual("webhookID", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteAccountActivitySubscriptionAsync_WithWebhookParameter_PopulatesInputParametersInResponse()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            AccountActivity accAct = await ctx.DeleteAccountActivitySubscriptionAsync(WebhookID);

            Assert.IsNotNull(accAct);
            Assert.AreEqual(WebhookID, accAct.WebhookID);
        }

        [TestMethod]
        public async Task DeleteAccountActivitySubscriptionAsync_WithWebhookID_ReturnsCreatedAccountActivity()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            AccountActivity accAct = await ctx.DeleteAccountActivitySubscriptionAsync(WebhookID);

            Assert.IsNotNull(accAct);
        }

        [TestMethod]
        public async Task DeleteAccountActivitySubscriptionAsync_WithValidParameters_SetsNullRawResult()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.DeleteAccountActivitySubscriptionAsync(WebhookID);

            Assert.IsNull(ctx.RawResult);
        }

        [TestMethod]
        public async Task DeleteAccountActivitySubscriptionAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.DeleteAccountActivitySubscriptionAsync(WebhookID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    "https://api.twitter.com/1.1/account_activity/webhooks/1/subscriptions.json",
                    It.IsAny<AccountActivityValue>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteAccountActivitySubscriptionAsync_WithZeroWebhookID_Throws()
        {
            const int WebhookID = 0;
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteAccountActivitySubscriptionAsync(WebhookID));

            Assert.AreEqual("webhookID", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteAccountActivityWebhookAsync_WithValidParameters_PopulatesRawResult()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.DeleteAccountActivityWebhookAsync(WebhookID);

            Assert.AreEqual(AccountActivityResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task DeleteAccountActivityWebhookAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong WebhookID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.DeleteAccountActivityWebhookAsync(WebhookID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    "https://api.twitter.com/1.1/account_activity/webhooks/1.json",
                    It.IsAny<AccountActivity>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        const string AccountActivityResponse = @"{
	""id"": ""921246280768151552"",
	""url"": ""https://accountactivitydemo.azurewebsites.net/api/accountactivity"",
	""valid"": true,
	""created_timestamp"": ""2017-10-20 05:26:44 +0000""
}";
    }
}
