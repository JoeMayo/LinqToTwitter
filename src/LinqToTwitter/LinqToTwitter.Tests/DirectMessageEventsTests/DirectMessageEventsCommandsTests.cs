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
    public class DirectMessageEventsCommandsTests
    {
        Mock<IAuthorizer> authMock;
        Mock<ITwitterExecute> execMock;

        public DirectMessageEventsCommandsTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void DirectMessageRequestProcessor_Works_With_Actions()
        {
            var dmReqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents>();

            Assert.IsInstanceOfType(dmReqProc, typeof(IRequestProcessorWithAction<DirectMessageEvents>));
        }

        TwitterContext InitializeTwitterContext()
        {
            authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(DirectMessageEventsResponse);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec => exec.SendJsonToTwitterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<DirectMessageEventsValue>(),
                It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            execMock.Setup(exec => exec.PostFormUrlEncodedToTwitterAsync<DirectMessageEvents>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task NewDirectMessageEventsAsync_WithRecipientID_ReturnsCreatedDM()
        {
            const ulong RecipientID = 1;
            const string Text = "Hi";
            var ctx = InitializeTwitterContext();

            DirectMessageEvents dmEvt = await ctx.NewDirectMessageEventAsync(RecipientID, Text);

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
        }

        [TestMethod]
        public async Task NewDirectMessageEventsAsync_WithValidParameters_PopulatesRawResult()
        {
            const ulong RecipientID = 1;
            const string Text = "Hi";
            var ctx = InitializeTwitterContext();

            await ctx.NewDirectMessageEventAsync(RecipientID, Text);

            Assert.AreEqual(DirectMessageEventsResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task NewDirectMessageEventsAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong RecipientID = 1;
            const string Text = "Hi";
            var ctx = InitializeTwitterContext();

            await ctx.NewDirectMessageEventAsync(RecipientID, Text);

            execMock.Verify(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    "https://api.twitter.com/1.1/direct_messages/events/new.json",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<DirectMessageEventsValue>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task NewDirectMessageEventsAsync_WithZeroRecipientID_Throws()
        {
            const int RecipientID = 0;
            const string Text = "Test Text";
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.NewDirectMessageEventAsync(RecipientID, Text));

            Assert.AreEqual("recipientID", ex.ParamName);
        }

        [TestMethod]
        public async Task NewDirectMessageEventsAsync_WithNullText_Throws()
        {
            const int RecipientID = 1;
            const string Text = null;
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.NewDirectMessageEventAsync(RecipientID, Text));

            Assert.AreEqual("text", ex.ParamName);
        }

        [TestMethod]
        public async Task NewDirectMessageEventsAsync_WithEmptyText_Throws()
        {
            const int RecipientID = 1;
            const string Text = "";
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.NewDirectMessageEventAsync(RecipientID, Text));

            Assert.AreEqual("text", ex.ParamName);
        }


        [TestMethod]
        public async Task DeleteDirectMessageEventsAsync_WithValidParameters_HasAnEmptyRawResult()
        {
            const ulong DirectMessageID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.DeleteDirectMessageEventAsync(DirectMessageID);

            Assert.AreEqual(DirectMessageEventsResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task DeleteDirectMessageEventsAsync_WithValidParameters_ConstructsUrl()
        {
            const ulong DirectMessageID = 1;
            var ctx = InitializeTwitterContext();

            await ctx.DeleteDirectMessageEventAsync(DirectMessageID);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<DirectMessageEvents>(
                    HttpMethod.Delete.ToString(),
                    "https://api.twitter.com/1.1/direct_messages/events/destroy.json?id=1",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public async Task DeleteDirectMessageEventsAsync_WithZeroDirectMessageID_Throws()
        {
            const int DirectMessageID = 0;
            var ctx = InitializeTwitterContext();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DeleteDirectMessageEventAsync(DirectMessageID));

            Assert.AreEqual("directMessageID", ex.ParamName);
        }

        const string DirectMessageEventsResponse = @"{
	""event"": {
		""type"": ""message_create"",
		""id"": ""917929712638246916"",
		""created_timestamp"": ""1507686472459"",
		""message_create"": {
            ""target"": {
                ""recipient_id"": ""16761255""

            },
			""sender_id"": ""15411837"",
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
	}
}";
    }
}
