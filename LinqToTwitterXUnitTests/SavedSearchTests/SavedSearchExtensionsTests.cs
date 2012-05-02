using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.SavedSearchTests
{
    public class SavedSearchExtensionsTests
    {
        Mock<ITwitterAuthorizer> authMock;
        Mock<ITwitterExecute> execMock;

        public SavedSearchExtensionsTests()
        {
            TestCulture.SetCulture();
        }
  
        TwitterContext InitializeTwitterContextMock()
        {
            authMock = new Mock<ITwitterAuthorizer>();
            execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            return ctx;
        }

        [Fact]
        public void SavedSearchRequestProcessor_Handles_Actions()
        {
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch>();

            Assert.IsAssignableFrom<IRequestProcessorWithAction<SavedSearch>>(searchReqProc);
        }

        [Fact]
        public void CreateSavedSearch_Throws_On_Missing_Query()
        {
            TwitterContext ctx = InitializeTwitterContextMock();

            var ex = Assert.Throws<ArgumentException>(() => ctx.CreateSavedSearch(null));

            Assert.Equal("query", ex.ParamName);
        }

        [Fact]
        public void CreateSavedSearch_Invokes_Executor_Execute()
        {
            const string Query = "#LinqToTwitter";
            TwitterContext ctx = InitializeTwitterContextMock();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<SavedSearch>>()))
                .Returns(SavedSearchResponse);
            var parameters = new Dictionary<string, string>
            {
                { "query", Query }
            };

            SavedSearch search = ctx.CreateSavedSearch("#LinqToTwitter");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/saved_searches/create.json",
                    parameters,
                    It.IsAny<IRequestProcessor<SavedSearch>>()),
                Times.Once());

            Assert.NotNull(search);
            Assert.Equal(Query, search.Name);
            Assert.Equal(Query, search.Query);
        }

        [Fact]
        public void DestroySavedSearch_Throws_On_Invalid_ID()
        {
            TwitterContext ctx = InitializeTwitterContextMock();

            var ex = Assert.Throws<ArgumentException>(() => ctx.DestroySavedSearch(0));

            Assert.Equal("id", ex.ParamName);
        }

        [Fact]
        public void DestroySavedSearch_Invokes_Executor_Execute()
        {
            TwitterContext ctx = InitializeTwitterContextMock();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<SavedSearch>>()))
                .Returns(SavedSearchResponse);
            var parameters = new Dictionary<string, string>();

            SavedSearch search = ctx.DestroySavedSearch(123);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/saved_searches/destroy/123.json",
                    parameters,
                    It.IsAny<IRequestProcessor<SavedSearch>>()),
                Times.Once());

            Assert.NotNull(search);
            Assert.Equal("#LinqToTwitter", search.Name);
            Assert.Equal("123", search.ID);
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
