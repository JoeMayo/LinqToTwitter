using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace LinqToTwitterPcl.Tests.DirectMessageTests
{
    [TestClass]
    public class WelcomeMessageCommandsTests
    {
        Mock<IAuthorizer> authMock;
        Mock<ITwitterExecute> execMock;

        public WelcomeMessageCommandsTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void DirectMessageRequestProcessor_Works_With_Actions()
        {
            var wmReqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>();

            Assert.IsInstanceOfType(wmReqProc, typeof(IRequestProcessorWithAction<WelcomeMessage>));
        }

        TwitterContext InitializeTwitterContext(string responseJson)
        {
            authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(responseJson);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec => exec.SendJsonToTwitterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<WelcomeMessageValue>(),
                It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            execMock.Setup(exec => exec.PostFormUrlEncodedToTwitterAsync<WelcomeMessage>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithNullText_Throws()
        {
            const string Name = "Test Welcome Message";
            const string Text = null;
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.NewWelcomeMessageAsync(Name, Text));

            Assert.AreEqual("text", ex.ParamName);
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithEmptyText_Throws()
        {
            const string Name = "Test Welcome Message";
            const string Text = "";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.NewWelcomeMessageAsync(Name, Text));

            Assert.AreEqual("text", ex.ParamName);
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithValidParameters_ConstructsUrl()
        {
            const string Name = "Test Welcome Message";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageAsync(Name, Text);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    "https://api.twitter.com/1.1/direct_messages/welcome_messages/new.json",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<WelcomeMessageValue>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithParameters_PopulatesInputParametersInResponse()
        {
            const string Name = "Test Welcome Message";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageAsync(Name, Text);

            Assert.IsNotNull(welcomeMsg);
            Assert.AreEqual(Name, welcomeMsg.Name);
            Assert.AreEqual(Text, welcomeMsg.Text);
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithName_ReturnsCreatedWelcomeMessage()
        {
            const string Name = "Test Welcome Message";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageAsync(Name, Text);

            Assert.IsNotNull(welcomeMsg);
            WelcomeMessageValue value = welcomeMsg.Value;
            Assert.IsNotNull(value);
            WelcomeMsg msg = value.WelcomeMessage;
            Assert.IsNotNull(msg);
            Assert.AreEqual("945484689657102340", msg.Id);
            Assert.AreEqual("1514256090927", msg.CreatedTimestamp);
            Assert.AreEqual(new DateTime(2017, 12, 26, 02, 41, 30, 927), msg.CreatedAt);
            WelcomeMessageData msgData = msg.MessageData;
            Assert.IsNotNull(msgData);
            Assert.AreEqual("Welcome!", msgData.Text);
            Entities entities = msgData.Entities;
            Assert.IsNotNull(entities);
            Assert.AreEqual("472356", msg.SourceAppId);
            Assert.AreEqual("New Welcome Message", msg.Name);
            JObject apps = value.Apps;
            Assert.IsNotNull(apps);
            JToken app = apps[msg.SourceAppId];
            Assert.IsNotNull(app);
            Assert.AreEqual("472356", app["id"]);
            Assert.AreEqual("LINQ to Tweeter", app["name"]);
            Assert.AreEqual("https://github.com/JoeMayo/LinqToTwitter", app["url"]);
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithValidParameters_PopulatesRawResult()
        {
            const string Name = "Test Welcome Message";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageAsync(Name, Text);

            Assert.AreEqual(WelcomeMessageResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UpdateWelcomeMessageRuleAsync_WithMissingMessageID_Throws()
        {
            const ulong WelcomeMessageID = 0;
            const string Name = "New Name";
            const string Text = null;
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text));

            Assert.AreEqual("welcomeMessageID", ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateWelcomeMessageAsync_WithNullText_Throws()
        {
            const ulong WelcomeMessageID = 1;
            const string Name = "New Name";
            const string Text = null;
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text));

            Assert.AreEqual("text", ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateWelcomeMessageAsync_WithEmptyText_Throws()
        {
            const ulong WelcomeMessageID = 1;
            const string Name = "New Name";
            const string Text = "";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text));

            Assert.AreEqual("text", ex.ParamName);
        }

        [TestMethod]
        public async Task UpdateWelcomeMessageAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong WelcomeMessageID = 1;
            const string Name = "New Name";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    "https://api.twitter.com/1.1/direct_messages/welcome_messages/update.json?id=1",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<WelcomeMessageValue>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task UpdateWelcomeMessageAsync_WithParameters_PopulatesInputParametersInResponse()
        {
            const ulong WelcomeMessageID = 1;
            const string Name = "New Name";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text);

            Assert.IsNotNull(welcomeMsg);
            Assert.AreEqual(WelcomeMessageID, welcomeMsg.WelcomeMessageID);
            Assert.AreEqual(Text, welcomeMsg.Text);
        }

        [TestMethod]
        public async Task UpdateWelcomeMessageAsync_WithName_ReturnsCreatedWelcomeMessage()
        {
            const ulong WelcomeMessageID = 1;
            const string Name = "New Name";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text);

            Assert.IsNotNull(welcomeMsg);
            WelcomeMessageValue value = welcomeMsg.Value;
            Assert.IsNotNull(value);
            WelcomeMsg msg = value.WelcomeMessage;
            Assert.IsNotNull(msg);
            Assert.AreEqual("945484689657102340", msg.Id);
            Assert.AreEqual("1514256090927", msg.CreatedTimestamp);
            Assert.AreEqual(new DateTime(2017, 12, 26, 02, 41, 30, 927), msg.CreatedAt);
            WelcomeMessageData msgData = msg.MessageData;
            Assert.IsNotNull(msgData);
            Assert.AreEqual("Welcome!", msgData.Text);
            Entities entities = msgData.Entities;
            Assert.IsNotNull(entities);
            Assert.AreEqual("472356", msg.SourceAppId);
            Assert.AreEqual("New Welcome Message", msg.Name);
            JObject apps = value.Apps;
            Assert.IsNotNull(apps);
            JToken app = apps[msg.SourceAppId];
            Assert.IsNotNull(app);
            Assert.AreEqual("472356", app["id"]);
            Assert.AreEqual("LINQ to Tweeter", app["name"]);
            Assert.AreEqual("https://github.com/JoeMayo/LinqToTwitter", app["url"]);
        }

        [TestMethod]
        public async Task UpdateWelcomeMessageAsync_WithValidParameters_PopulatesRawResult()
        {
            const ulong WelcomeMessageID = 1;
            const string Name = "New Name";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text);

            Assert.AreEqual(WelcomeMessageResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task NewWelcomeMessageRuleAsync_WithMissingMessageID_Throws()
        {
            const ulong welcomeMessageID = 0;
            var ctx = InitializeTwitterContext(WelcomeMessageRuleResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.NewWelcomeMessageRuleAsync(welcomeMessageID));

            Assert.AreEqual("welcomeMessageID", ex.ParamName);
        }

        [TestMethod]
        public async Task NewWelcomeMessageRuleAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong welcomeMessageID = 1;
            var ctx = InitializeTwitterContext(WelcomeMessageRuleResponse);

            await ctx.NewWelcomeMessageRuleAsync(welcomeMessageID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    "https://api.twitter.com/1.1/direct_messages/welcome_messages/rules/new.json",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<WelcomeMessageValue>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task NewWelcomeMessageRuleAsync_WithParameters_PopulatesInputParametersInResponse()
        {
            const ulong welcomeMessageID = 1;
            var ctx = InitializeTwitterContext(WelcomeMessageRuleResponse);

            WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageRuleAsync(welcomeMessageID);

            Assert.IsNotNull(welcomeMsg);
            Assert.AreEqual(welcomeMessageID, welcomeMsg.WelcomeMessageID);
        }

        [TestMethod]
        public async Task NewWelcomeMessageRuleAsync_WithValidParameters_PopulatesRawResult()
        {
            const ulong welcomeMessageID = 1;
            var ctx = InitializeTwitterContext(WelcomeMessageRuleResponse);

            WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageRuleAsync(welcomeMessageID);

            Assert.AreEqual(WelcomeMessageRuleResponse, ctx.RawResult);
        }

        #region Response Strings

        const string WelcomeMessageResponse = @"{
	""welcome_message"": {
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
	""apps"": {
		""472356"": {
			""id"": ""472356"",
			""name"": ""LINQ to Tweeter"",
			""url"": ""https:\/\/github.com\/JoeMayo\/LinqToTwitter""
		}
	}
}";

        const string WelcomeMessageRuleResponse = @"{
	""welcome_message_rule"": {
		""id"": ""949450720239001600"",
		""created_timestamp"": ""1515201666303"",
		""welcome_message_id"": ""949450173926617092""
	}
}";

        #endregion
    }
}
