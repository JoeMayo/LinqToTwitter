using LinqToTwitter.Common;
using LinqToTwitter.Common.Entities;
using LinqToTwitter.OAuth;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.WelcomeMessageTests
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

        TwitterContext InitializeTwitterContext<TMessageType>(string responseJson)
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
                It.IsAny<TMessageType>(),
                It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            execMock.Setup(exec => exec.PostFormUrlEncodedToTwitterAsync<WelcomeMessage>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            return new TwitterContext(execMock.Object);
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithNullText_Throws()
        {
            const string Name = "Test Welcome Message";
            const string Text = null;
            var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.NewWelcomeMessageAsync(Name, Text));

            Assert.AreEqual("text", ex.ParamName);
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithEmptyText_Throws()
        {
            const string Name = "Test Welcome Message";
            const string Text = "";
            var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.NewWelcomeMessageAsync(Name, Text));

            Assert.AreEqual("text", ex.ParamName);
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithValidParameters_ConstructsUrl()
        {
            const string Name = "Test Welcome Message";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageResponse);

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

        //[TestMethod]
        //public async Task NewWelcomeMessageAsync_WithParameters_PopulatesInputParametersInResponse()
        //{
        //    const string Name = "Test Welcome Message";
        //    const string Text = "Welcome!";
        //    var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageResponse);

        //    WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageAsync(Name, Text);

        //    Assert.IsNotNull(welcomeMsg);
        //    Assert.AreEqual(Name, welcomeMsg.Name);
        //    Assert.AreEqual(Text, welcomeMsg.Text);
        //}

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithName_ReturnsCreatedWelcomeMessage()
        {
            const string Name = "Test Welcome Message";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageResponse);

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
            JsonElement apps = value.Apps;
            Assert.IsNotNull(apps);
            JsonElement app = apps.GetProperty(msg.SourceAppId);
            Assert.IsNotNull(app);
            Assert.AreEqual("472356", app.GetString("id"));
            Assert.AreEqual("LINQ to Tweeter", app.GetString("name"));
            Assert.AreEqual("https://github.com/JoeMayo/LinqToTwitter", app.GetString("url"));
        }

        [TestMethod]
        public async Task NewWelcomeMessageAsync_WithValidParameters_PopulatesRawResult()
        {
            const string Name = "Test Welcome Message";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageAsync(Name, Text);

            Assert.AreEqual(WelcomeMessageResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task UpdateWelcomeMessageRuleAsync_WithMissingMessageID_Throws()
        {
            const ulong WelcomeMessageID = 0;
            const string Name = "New Name";
            const string Text = null;
            var ctx = InitializeTwitterContext<WelcomeMsg>(WelcomeMessageResponse);

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
            var ctx = InitializeTwitterContext<WelcomeMsg>(WelcomeMessageResponse);

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
            var ctx = InitializeTwitterContext<WelcomeMsg>(WelcomeMessageResponse);

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
            var ctx = InitializeTwitterContext<WelcomeMsg>(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    "https://api.twitter.com/1.1/direct_messages/welcome_messages/update.json?id=1",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<WelcomeMsg>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        //[TestMethod]
        //public async Task UpdateWelcomeMessageAsync_WithParameters_PopulatesInputParametersInResponse()
        //{
        //    const ulong WelcomeMessageID = 1;
        //    const string Name = "New Name";
        //    const string Text = "Welcome!";
        //    var ctx = InitializeTwitterContext<WelcomeMsg>(WelcomeMessageResponse);

        //    WelcomeMessage welcomeMsg = await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text);

        //    Assert.IsNotNull(welcomeMsg);
        //    Assert.AreEqual(WelcomeMessageID, welcomeMsg.ID);
        //    Assert.AreEqual(Text, welcomeMsg.Text);
        //}

        [TestMethod]
        public async Task UpdateWelcomeMessageAsync_WithName_ReturnsCreatedWelcomeMessage()
        {
            const ulong WelcomeMessageID = 1;
            const string Name = "New Name";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext<WelcomeMsg>(WelcomeMessageResponse);

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
            JsonElement apps = value.Apps;
            Assert.IsNotNull(apps);
            JsonElement app = apps.GetProperty(msg.SourceAppId);
            Assert.IsNotNull(app);
            Assert.AreEqual("472356", app.GetString("id"));
            Assert.AreEqual("LINQ to Tweeter", app.GetString("name"));
            Assert.AreEqual("https://github.com/JoeMayo/LinqToTwitter", app.GetString("url"));
        }

        [TestMethod]
        public async Task UpdateWelcomeMessageAsync_WithValidParameters_PopulatesRawResult()
        {
            const ulong WelcomeMessageID = 1;
            const string Name = "New Name";
            const string Text = "Welcome!";
            var ctx = InitializeTwitterContext<WelcomeMsg>(WelcomeMessageResponse);

            WelcomeMessage welcomeMsg = await ctx.UpdateWelcomeMessageAsync(WelcomeMessageID, Name, Text);

            Assert.AreEqual(WelcomeMessageResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task DeleteWelcomeMessageAsync_WithMissingMessageID_Throws()
        {
            const ulong WelcomeMessageID = 0;
            var ctx = InitializeTwitterContext<WelcomeMessage>(string.Empty);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteWelcomeMessageAsync(WelcomeMessageID));

            Assert.AreEqual("welcomeMessageID", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteWelcomeMessageAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong WelcomeMessageID = 1;
            var ctx = InitializeTwitterContext<WelcomeMessage>(string.Empty);

            await ctx.DeleteWelcomeMessageAsync(WelcomeMessageID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    "https://api.twitter.com/1.1/direct_messages/welcome_messages/destroy.json?id=1",
                    It.IsAny<IDictionary<string, string>>(),
                    string.Empty,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteWelcomeMessageAsync_WithValidParameters_HasEmptyRawResult()
        {
            const ulong WelcomeMessageID = 1;
            var ctx = InitializeTwitterContext<WelcomeMessage>(string.Empty);

            await ctx.DeleteWelcomeMessageAsync(WelcomeMessageID);

            Assert.AreEqual(string.Empty, ctx.RawResult);
        }

        [TestMethod]
        public async Task NewWelcomeMessageRuleAsync_WithMissingMessageID_Throws()
        {
            const ulong welcomeMessageID = 0;
            var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageRuleResponse);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.NewWelcomeMessageRuleAsync(welcomeMessageID));

            Assert.AreEqual("welcomeMessageID", ex.ParamName);
        }

        [TestMethod]
        public async Task NewWelcomeMessageRuleAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong welcomeMessageID = 1;
            var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageRuleResponse);

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
            var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageRuleResponse);

            WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageRuleAsync(welcomeMessageID);

            Assert.IsNotNull(welcomeMsg);
            Assert.AreEqual(welcomeMessageID, welcomeMsg.ID);
        }

        [TestMethod]
        public async Task NewWelcomeMessageRuleAsync_WithValidParameters_PopulatesRawResult()
        {
            const ulong welcomeMessageID = 1;
            var ctx = InitializeTwitterContext<WelcomeMessageValue>(WelcomeMessageRuleResponse);

            WelcomeMessage welcomeMsg = await ctx.NewWelcomeMessageRuleAsync(welcomeMessageID);

            Assert.AreEqual(WelcomeMessageRuleResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task DeleteWelcomeMessageRuleAsync_WithMissingMessageID_Throws()
        {
            const ulong WelcomeMessageRuleID = 0;
            var ctx = InitializeTwitterContext<WelcomeMessage>(string.Empty);

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteWelcomeMessageRuleAsync(WelcomeMessageRuleID));

            Assert.AreEqual("welcomeMessageRuleID", ex.ParamName);
        }

        [TestMethod]
        public async Task DeleteWelcomeMessageRuleAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong WelcomeMessageRuleID = 1;
            var ctx = InitializeTwitterContext<WelcomeMessage>(string.Empty);

            await ctx.DeleteWelcomeMessageRuleAsync(WelcomeMessageRuleID);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    "https://api.twitter.com/1.1/direct_messages/welcome_messages/rules/destroy.json?id=1",
                    It.IsAny<IDictionary<string, string>>(),
                    string.Empty,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteWelcomeMessageRuleAsync_WithValidParameters_HasEmptyRawResult()
        {
            const ulong WelcomeMessageRuleID = 1;
            var ctx = InitializeTwitterContext<WelcomeMessage>(string.Empty);

            await ctx.DeleteWelcomeMessageRuleAsync(WelcomeMessageRuleID);

            Assert.AreEqual(string.Empty, ctx.RawResult);
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
