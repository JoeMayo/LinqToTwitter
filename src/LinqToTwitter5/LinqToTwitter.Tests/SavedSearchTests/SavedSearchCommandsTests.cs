using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Net.Http;

namespace LinqToTwitterPcl.Tests.SavedSearchTests
{
    [TestClass]
    public class SavedSearchCommandsTests
    {
        Mock<IAuthorizer> authMock;
        Mock<ITwitterExecute> execMock;

        public SavedSearchCommandsTests()
        {
            TestCulture.SetCulture();
        }
  
        TwitterContext InitializeTwitterContextMock()
        {
            authMock = new Mock<IAuthorizer>();
            execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public void SavedSearchRequestProcessor_Handles_Actions()
        {
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch>();

            Assert.IsInstanceOfType(searchReqProc, typeof(IRequestProcessorWithAction<SavedSearch>));
        }

        [TestMethod]
        public async Task CreateSavedSearchAsync_Throws_On_Missing_Query()
        {
            TwitterContext ctx = InitializeTwitterContextMock();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.CreateSavedSearchAsync(null));

            Assert.AreEqual("query", ex.ParamName);
        }

        [TestMethod]
        public async Task CreateSavedSearchAsync_Invokes_Executor_Execute()
        {
            const string Query = "#LinqToTwitter";
            TwitterContext ctx = InitializeTwitterContextMock();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(SavedSearchResponse);
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<SavedSearch>(
                    HttpMethod.Post.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var parameters = new Dictionary<string, string>
            {
                { "query", Query }
            };

            SavedSearch search = await ctx.CreateSavedSearchAsync("#LinqToTwitter");

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<SavedSearch>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/saved_searches/create.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());

            Assert.IsNotNull(search);
            Assert.AreEqual(Query, search.Name);
            Assert.AreEqual(Query, search.Query);
        }

        [TestMethod]
        public async Task CreateSavedSearchAsync_WithRawResult_Succeeds()
        {
            TwitterContext ctx = InitializeTwitterContextMock();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(SavedSearchResponse);
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<SavedSearch>(
                    HttpMethod.Post.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);

            await ctx.CreateSavedSearchAsync("#LinqToTwitter");

            Assert.AreEqual(SavedSearchResponse, ctx.RawResult);
        }

        [TestMethod]
        public async Task DestroySavedSearch_Throws_On_Invalid_ID()
        {
            TwitterContext ctx = InitializeTwitterContextMock();

            var ex = await L2TAssert.Throws<ArgumentException>(
                async () => await ctx.DestroySavedSearchAsync(0));

            Assert.AreEqual("id", ex.ParamName);
        }

        [TestMethod]
        public async Task DestroySavedSearchAsync_Invokes_Executor_Execute()
        {
            TwitterContext ctx = InitializeTwitterContextMock();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(SavedSearchResponse);
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<SavedSearch>(
                    HttpMethod.Post.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var parameters = new Dictionary<string, string>();

            SavedSearch search = await ctx.DestroySavedSearchAsync(123);

            execMock.Verify(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<SavedSearch>(
                    HttpMethod.Post.ToString(),
                    "https://api.twitter.com/1.1/saved_searches/destroy/123.json",
                    parameters,
                    It.IsAny<CancellationToken>()),
                Times.Once());

            Assert.IsNotNull(search);
            Assert.AreEqual("#LinqToTwitter", search.Name);
            Assert.AreEqual(123ul, search.ID);
        }

        [TestMethod]
        public async Task DestroySavedSearchAsync_WithRawResult_Succeeds()
        {
            TwitterContext ctx = InitializeTwitterContextMock();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(SavedSearchResponse);
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostFormUrlEncodedToTwitterAsync<SavedSearch>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);

            await ctx.DestroySavedSearchAsync(123);

            Assert.AreEqual(SavedSearchResponse, ctx.RawResult);
        }

        const string SavedSearchResponse = @"{
   ""query"":""#LinqToTwitter"",
   ""name"":""#LinqToTwitter"",
   ""position"":null,
   ""id_str"":""3275867"",
   ""created_at"":""Fri Dec 18 04:17:24 +0000 2009"",
   ""id"":3275867
}";
    }
}
