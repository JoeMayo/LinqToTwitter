using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.SavedSearchTests
{
    public class SavedSearchRequestProcessorTests
    {
        public SavedSearchRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void SavedSearchRequestProcessor_Works_On_Json_Format_Data()
        {
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch>();

            Assert.IsAssignableFrom<IRequestProcessorWantsJson>(searchReqProc);
        }

        [Fact]
        public void ProcessResults_Parses_Searches_Response()
        {
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch> { Type = SavedSearchType.Searches };

            var searches = searchReqProc.ProcessResults(SearchesResponse);

            Assert.NotNull(searches);
            Assert.Equal(2, searches.Count);
            var search = searches.First();
            Assert.NotNull(search);
            Assert.Equal("#LinqToTwitter", search.Query);
            Assert.Equal("#LinqToTwitter", search.Name);
            Assert.Equal(0, search.Postition);
            Assert.Equal("3275867", search.IDString);
            Assert.Equal(new DateTime(2009, 12, 18, 4, 17, 24), search.CreatedAt);
        }

        [Fact]
        public void ProcessResults_Parses_Show_Response()
        {
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch> { Type = SavedSearchType.Show };

            var searches = searchReqProc.ProcessResults(ShowResponse);

            Assert.NotNull(searches);
            Assert.Single(searches);
            var search = searches.Single();
            Assert.NotNull(search);
            Assert.Equal("#LinqToTwitter", search.Query);
            Assert.Equal("#LinqToTwitter", search.Name);
            Assert.Equal(0, search.Postition);
            Assert.Equal("3275867", search.IDString);
            Assert.Equal(new DateTime(2009, 12, 18, 4, 17, 24), search.CreatedAt);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            var searches = reqProc.ProcessResults(string.Empty);

            Assert.Equal(0, searches.Count);
        }

        [Fact]
        public void GetParameters_Handles_Input_Parameters()
        {
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch> { BaseUrl = "https://api.twitter.com/1.1/" };
            Expression<Func<SavedSearch, bool>> expression =
                search =>
                    search.Type == SavedSearchType.Show &&
                    search.ID == "123";
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = searchReqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)SavedSearchType.Show).ToString())));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "123")));
        }

        [Fact]
        public void BuildUrl_Show_Throws_On_Missing_ID()
        {
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", SavedSearchType.Show.ToString() }
            };

            var ex = Assert.Throws<ArgumentException>(() => searchReqProc.BuildUrl(parameters));

            Assert.Equal("ID", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Constructs_Show_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/saved_searches/show/123.json";
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", SavedSearchType.Show.ToString() },
                { "ID", "123" }
            };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_SavedSearches_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/saved_searches/list.json";
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", SavedSearchType.Searches.ToString() }
            };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_On_Missing_Type()
        {
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch>() { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string> { };

            var ex = Assert.Throws<ArgumentException>(() => searchReqProc.BuildUrl(parameters));

            Assert.Equal<string>("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_On_Null_Parameters()
        {
            var searchReqProc = new SavedSearchRequestProcessor<SavedSearch>() { BaseUrl = "https://api.twitter.com/1.1/" };

            var ex = Assert.Throws<ArgumentException>(() => searchReqProc.BuildUrl(null));

            Assert.Equal<string>("Type", ex.ParamName);
        }


        const string SearchesResponse = @"[
   {
      ""query"":""#LinqToTwitter"",
      ""name"":""#LinqToTwitter"",
      ""position"":null,
      ""id_str"":""3275867"",
      ""created_at"":""Fri Dec 18 04:17:24 +0000 2009"",
      ""id"":3275867
   },
   {
      ""query"":""\""Windows 8\"""",
      ""name"":""\""Windows 8\"""",
      ""position"":null,
      ""id_str"":""101035995"",
      ""created_at"":""Sun Apr 29 04:09:00 +0000 2012"",
      ""id"":101035995
   }
]";

        const string ShowResponse = @"{
   ""query"":""#LinqToTwitter"",
   ""name"":""#LinqToTwitter"",
   ""position"":null,
   ""id_str"":""3275867"",
   ""created_at"":""Fri Dec 18 04:17:24 +0000 2009"",
   ""id"":3275867
}";
    }
}
