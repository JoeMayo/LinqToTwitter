using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.SearchTests
{
    public class SearchRequestProcessorTests
    {
        public SearchRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void GetParametersTest()
        {
            var target = new SearchRequestProcessor<Search>();
            Expression<Func<Search, bool>> expression =
                search =>
                    search.Type == SearchType.Search &&
                    search.GeoCode == "40.757929,-73.985506,25km" &&
                    search.SearchLanguage == "en" &&
                    search.Page == 1 &&
                    search.PageSize == 10 &&
                    search.Query == "LINQ to Twitter" &&
                    search.ShowUser == true &&
                    search.SinceID == 123 &&
                    search.MaxID == 200 &&
                    search.ResultType == ResultType.Popular &&
                    search.WordPhrase == "LINQ to Twitter" &&
                    search.WordAnd == "LINQ Twitter" &&
                    search.WordOr == "LINQ Twitter" &&
                    search.WordNot == "LINQ Twitter" &&
                    search.Hashtag == "linqtotwitter" &&
                    search.PersonFrom == "JoeMayo" &&
                    search.PersonTo == "JoeMayo" &&
                    search.PersonReference == "JoeMayo" &&
                    search.Attitude == Attitude.Positive &&
                    search.WithLinks == true &&
                    search.WithRetweets == true &&
                    search.IncludeEntities == true;
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)SearchType.Search).ToString(CultureInfo.InvariantCulture))));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("GeoCode", "40.757929,-73.985506,25km")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SearchLanguage", "en")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("PageSize", "10")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Query", "LINQ to Twitter")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ShowUser", "True")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SinceID", "123")));
            Assert.True(
               queryParams.Contains(
                   new KeyValuePair<string, string>("MaxID", "200")));
            Assert.True(
               queryParams.Contains(
                   new KeyValuePair<string, string>("ResultType", ((int)ResultType.Popular).ToString(CultureInfo.InvariantCulture))));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WordPhrase", "LINQ to Twitter")));
            Assert.True(
             queryParams.Contains(
                 new KeyValuePair<string, string>("WordAnd", "LINQ Twitter")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WordOr", "LINQ Twitter")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WordNot", "LINQ Twitter")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Hashtag", "linqtotwitter")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("PersonFrom", "JoeMayo")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("PersonTo", "JoeMayo")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("PersonReference", "JoeMayo")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Attitude", ((int)Attitude.Positive).ToString(CultureInfo.InvariantCulture))));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WithLinks", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WithRetweets", "True")));
            Assert.True(
              queryParams.Contains(
                  new KeyValuePair<string, string>("IncludeEntities", "True")));
        }

        [Fact]
        public void BuildURL_Includes_Parameters()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "GeoCode", "40.757929,-73.985506,25km" },
                    { "SearchLanguage", "en" },
                    { "Page", "1" },
                    { "PageSize", "10" },
                    { "Query", "LINQ to Twitter" },
                    { "ShowUser", "true" },
                    { "SinceID", "1" },
                    { "Since", new DateTime(2010, 7, 4).ToString(CultureInfo.InvariantCulture) },
                    { "Until", new DateTime(2011, 7, 4).ToString(CultureInfo.InvariantCulture) },
                    { "ResultType", ResultType.Popular.ToString() },
               };
            const string expected = "http://search.twitter.com/search.json?geocode=40.757929%2C-73.985506%2C25km&lang=en&page=1&rpp=10&q=LINQ%20to%20Twitter&show_user=true&since=2010-07-04&until=2011-07-04&since_id=1&result_type=popular";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildURL_Uses_Only_Date_Part_Of_Since()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Since", new DateTime(2010, 7, 4, 7, 30, 10).ToString(CultureInfo.InvariantCulture) },
               };
            const string expected = "http://search.twitter.com/search.json?since=2010-07-04";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Does_Not_Include_false_ShowUser()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "ShowUser", false.ToString(CultureInfo.InvariantCulture) },
                };
            const string expected = "http://search.twitter.com/search.json";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Handles_Word_Paramters()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "WordPhrase", "LINQ to Twitter" },
                    { "WordAnd", "LINQ Twitter" },
                    { "WordOr", "LINQ Twitter" },
                    { "WordNot", "LINQ Twitter" },
                    { "Hashtag", "linqtotwitter" },
               };
            const string expected = "http://search.twitter.com/search.json?exact=LINQ%20to%20Twitter&ands=LINQ%20Twitter&ors=LINQ%20Twitter&nots=LINQ%20Twitter&tag=linqtotwitter";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Includes_Person_Parameters()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "PersonFrom", "JoeMayo" },
                    { "PersonTo", "JoeMayo" },
                    { "PersonReference", "JoeMayo" },
              };
            const string expected = "http://search.twitter.com/search.json?from=JoeMayo&to=JoeMayo&ref=JoeMayo";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Includes_Attitude_Parameters()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Attitude", (Attitude.Positive | Attitude.Negative | Attitude.Question).ToString() },
                };
            const string expected = "http://search.twitter.com/search.json?tude%5B%5D=%3A)&tude%5B%5D=%3A(&tude%5B%5D=%3F";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Includes_All_Attitude_Except_Positive()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Attitude", (Attitude.Negative | Attitude.Question).ToString() },
                };
            const string expected = "http://search.twitter.com/search.json?tude%5B%5D=%3A(&tude%5B%5D=%3F";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Includes_WithX_Parameters()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "WithLinks", true.ToString(CultureInfo.InvariantCulture) },
                    { "WithRetweets", true.ToString(CultureInfo.InvariantCulture) }
                };
            const string expected = "http://search.twitter.com/search.json?filter%5B%5D=links&include%5B%5D=retweets";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Does_Not_Include_false_WithX_Parameters()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "WithLinks", false.ToString(CultureInfo.InvariantCulture) },
                    { "WithRetweets", false.ToString(CultureInfo.InvariantCulture) }
                };
            const string expected = "http://search.twitter.com/search.json";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_With_Missing_Type_Parameter()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters = new Dictionary<string, string> { };

            ArgumentException ex =
                Assert.Throws<ArgumentException>(() =>
                {
                    searchReqProc.BuildURL(parameters);
                });

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_When_Parameters_Null()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            ArgumentException ex =
                Assert.Throws<ArgumentException>(() =>
                {
                    searchReqProc.BuildURL(null);
                });

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Encodes_Query()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            string expected = searchReqProc.BaseUrl + "search.json?q=Contains%20Space";
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Query", "Contains Space" }
                };

            Request req = searchReqProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Adds_true_IncludeEntities()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "IncludeEntities", true.ToString(CultureInfo.InvariantCulture) }
                };
            const string expected = "http://search.twitter.com/search.json?include_entities=true";

            Request req = searchProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Does_Not_Add_false_IncludeEntities()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "IncludeEntities", false.ToString(CultureInfo.InvariantCulture) }
                };
            const string expected = "http://search.twitter.com/search.json";

            Request req = searchProc.BuildURL(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void ProcessResults_Populates_CompletedIn()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(0.057m, results.First().CompletedIn);
        }

        [Fact]
        public void ProcessResults_Populates_MaxID()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(155786587962224641ul, results.First().MaxID);
        }

        [Fact]
        public void ProcessResults_Populates_NextPage()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("?page=2&max_id=155786587962224641&q=blue%20angels&include_entities=1", results.First().NextPage);
        }

        [Fact]
        public void ProcessResults_Populates_PageResult()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(1, results.First().PageResult);
        }

        [Fact]
        public void ProcessResults_Populates_QueryResult()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("blue+angels", results.First().QueryResult);
        }

        [Fact]
        public void ProcessResults_Populates_ResultsPerPageResult()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(15, results.First().ResultsPerPageResult);
        }

        [Fact]
        public void ProcessResults_Populates_SinceIDResult()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(3ul, results.First().SinceIDResult);
        }

        [Fact]
        public void ProcessResults_Populates_RefreshUrl()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("?since_id=155786587962224641&q=blue%20angels&include_entities=1", results.First().RefreshUrl);
        }

        [Fact]
        public void ProcessResults_Creates_List_of_SearchResult()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.NotNull(results.First().Results);
            Assert.True(results.First().Results.Any());
        }

        [Fact]
        public void ProcessResults_Populates_CreatedAt()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(
                new DateTimeOffset(2012, 1, 7, 23, 03, 11, new TimeSpan(0, 0, 0)), 
                results.First().Results.First().CreatedAt);
        }

        [Fact]
        public void ProcessResults_Instantiates_Entities()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.NotNull(results.First().Results.First().Entities);
        }

        [Fact]
        public void ProcessResults_Populates_Entity_Urls()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            List<UrlMention> urls = results.First().Results.First().Entities.UrlMentions;
            Assert.NotNull(urls);
            Assert.Equal(1, urls.Count);
            UrlMention firstUrl = urls.First();
            Assert.Equal("http://t.co/xSmFKo5h", firstUrl.Url);
            Assert.Equal("http://bit.ly/yXkWPy", firstUrl.ExpandedUrl);
            Assert.Equal("bit.ly/yXkWPy", firstUrl.DisplayUrl);
            Assert.Equal(116, firstUrl.Start);
            Assert.Equal(136, firstUrl.End);
        }

        [Fact]
        public void ProcessResults_Populates_Entity_Hashtags()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            List<HashTagMention> hashes = results.First().Results[1].Entities.HashTagMentions;
            Assert.NotNull(hashes);
            Assert.Equal(2, hashes.Count);
            HashTagMention firstHash = hashes.First();
            Assert.Equal("Presidential", firstHash.Tag);
            Assert.Equal(0, firstHash.Start);
            Assert.Equal(13, firstHash.End);
        }

        [Fact]
        public void ProcessResults_Populates_Entity_Users()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            List<UserMention> users = results.First().Results[2].Entities.UserMentions;
            Assert.NotNull(users);
            Assert.Equal(1, users.Count);
            UserMention firstUser = users.First();
            Assert.Equal("DesharThomas30", firstUser.ScreenName);
            Assert.Equal("DeShar Thomas", firstUser.Name);
            Assert.Equal(323629022UL, firstUser.Id);
            Assert.Equal(0, firstUser.Start);
            Assert.Equal(15, firstUser.End);
        }

        [Fact]
        public void ProcessResults_Populates_Entity_Media()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            List<MediaMention> media = results.First().Results[3].Entities.MediaMentions;
            Assert.NotNull(media);
            Assert.Equal(1, media.Count);
            MediaMention firstMedia = media.First();
            Assert.Equal(155683816676134913ul, firstMedia.ID);
            Assert.Equal("http://p.twimg.com/AikZmz5CEAESBHD.jpg", firstMedia.MediaUrl);
            Assert.Equal("https://p.twimg.com/AikZmz5CEAESBHD.jpg", firstMedia.MediaUrlHttps);
            Assert.Equal("http://t.co/36MZIOyW", firstMedia.Url);
            Assert.Equal("pic.twitter.com/36MZIOyW", firstMedia.DisplayUrl);
            Assert.Equal("http://twitter.com/rschu/status/155683816671940609/photo/1", firstMedia.ExpandedUrl);
            Assert.Equal("photo", firstMedia.Type);
            Assert.NotNull(firstMedia.Sizes);
            Assert.Equal(5, firstMedia.Sizes.Count);
            PhotoSize firstSize = firstMedia.Sizes.First();
            Assert.Equal("orig", firstSize.Type);
            Assert.Equal(1161, firstSize.Width);
            Assert.Equal(925, firstSize.Height);
            Assert.Equal("fit", firstSize.Resize);
            Assert.Equal(59, firstMedia.Start);
            Assert.Equal(79, firstMedia.End);
        }

        [Fact]
        public void ProcessResults_Populates_FromUser()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("LakeMtkaLiberty", results.First().Results.First().FromUser);
        }

        [Fact]
        public void ProcessResults_Populates_FromUserID()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(15117715ul, results.First().Results.First().FromUserID);
        }

        [Fact]
        public void ProcessResults_Populates_FromUserName()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("The Admiral", results.First().Results.First().FromUserName);
        }

        [Fact]
        public void ProcessResults_Populates_Geo()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };
            var expectedLatitude = double.Parse("-22.7747", CultureInfo.InvariantCulture);
            var expectedLongitude = double.Parse("-41.9052", CultureInfo.InvariantCulture);

            List<Search> results = searchProc.ProcessResults(SearchJson);

            var geo = results.First().Results.First().Geo;
            Assert.NotNull(geo);
            Assert.Equal("Point", geo.Type);
            var coordinate = geo.Coordinates.First();
            Assert.NotNull(coordinate);
            Assert.Equal(expectedLatitude, coordinate.Latitude);
            Assert.Equal(expectedLongitude, coordinate.Longitude);
        }

        [Fact]
        public void ProcessResults_Populates_ID()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(155786587962224641ul, results.First().Results.First().ID);
        }

        [Fact]
        public void ProcessResults_Populates_IsoLanguageCode()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("en", results.First().Results.First().IsoLanguageCode);
        }

        [Fact]
        public void ProcessResults_Populates_MetaData()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            SearchEntry firstEntry = results.First().Results.First();
            Assert.NotNull(firstEntry);
            SearchMetaData metaData = firstEntry.MetaData;
            Assert.Equal(3, metaData.RecentRetweets);
            Assert.Equal(ResultType.Recent, metaData.ResultType);
        }

        [Fact]
        public void ProcessResults_Populates_ProfileImageUrl()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("http://a1.twimg.com/profile_images/69013587/small_The_Admiral_normal.jpg", results.First().Results.First().ProfileImageUrl);
        }

        [Fact]
        public void ProcessResults_Populates_ProfileImageUrlHttps()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("https://si0.twimg.com/profile_images/69013587/small_The_Admiral_normal.jpg", results.First().Results.First().ProfileImageUrlHttps);
        }

        [Fact]
        public void ProcessResults_Populates_Source()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("&lt;a href=&quot;http://twitterfeed.com&quot; rel=&quot;nofollow&quot;&gt;twitterfeed&lt;/a&gt;", results.First().Results.First().Source);
        }

        [Fact]
        public void ProcessResults_Populates_Text()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("Photo of the week: US Navy rescues 18 Iranians from Somali Pirates: Related Posts:Daily Navy Photo: Blue Angels ... http://t.co/xSmFKo5h", results.First().Results.First().Text);
        }

        [Fact]
        public void ProcessResults_Populates_ToUser()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("JoeMayo", results.First().Results.First().ToUser);
        }

        [Fact]
        public void ProcessResults_Populates_ToUserID()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(123456789ul, results.First().Results.First().ToUserID);
        }

        [Fact]
        public void ProcessResults_Populates_ToUserName()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "http://search.twitter.com/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("Joe Mayo", results.First().Results.First().ToUserName);
        }

        const string SearchJson = @"{
   ""completed_in"":0.057,
   ""max_id"":155786587962224641,
   ""max_id_str"":""155786587962224641"",
   ""next_page"":""?page=2&max_id=155786587962224641&q=blue%20angels&include_entities=1"",
   ""page"":1,
   ""query"":""blue+angels"",
   ""refresh_url"":""?since_id=155786587962224641&q=blue%20angels&include_entities=1"",
   ""results"":[
      {
         ""created_at"":""Sat, 07 Jan 2012 23:03:11 +0000"",
         ""entities"":{
            ""urls"":[
               {
                  ""url"":""http://t.co/xSmFKo5h"",
                  ""expanded_url"":""http://bit.ly/yXkWPy"",
                  ""display_url"":""bit.ly/yXkWPy"",
                  ""indices"":[
                     116,
                     136
                  ]
               }
            ]
         },
         ""from_user"":""LakeMtkaLiberty"",
         ""from_user_id"":15117715,
         ""from_user_id_str"":""15117715"",
         ""from_user_name"":""The Admiral"",
         ""geo"":{
            ""coordinates"":[
               -22.7747,
               -41.9052
            ],
            ""type"":""Point""
         },
         ""id"":155786587962224641,
         ""id_str"":""155786587962224641"",
         ""iso_language_code"":""en"",
         ""metadata"":{
            ""recent_retweets"":3,         
            ""result_type"":""recent""
         },
         ""profile_image_url"":""http://a1.twimg.com/profile_images/69013587/small_The_Admiral_normal.jpg"",
         ""profile_image_url_https"":""https://si0.twimg.com/profile_images/69013587/small_The_Admiral_normal.jpg"",
         ""source"":""&lt;a href=&quot;http://twitterfeed.com&quot; rel=&quot;nofollow&quot;&gt;twitterfeed&lt;/a&gt;"",
         ""text"":""Photo of the week: US Navy rescues 18 Iranians from Somali Pirates: Related Posts:Daily Navy Photo: Blue Angels ... http://t.co/xSmFKo5h"",
         ""to_user"":""JoeMayo"",
         ""to_user_id"":123456789,
         ""to_user_id_str"":""123456789"",
         ""to_user_name"":""Joe Mayo""
      },
      {
         ""created_at"":""Sat, 07 Jan 2012 22:27:21 +0000"",
         ""entities"":{
            ""hashtags"":[
               {
                  ""text"":""Presidential"",
                  ""indices"":[
                     0,
                     13
                  ]
               },
               {
                  ""text"":""Newt"",
                  ""indices"":[
                     62,
                     67
                  ]
               }
            ]
         },
         ""from_user"":""cu_mr2ducks"",
         ""from_user_id"":27061351,
         ""from_user_id_str"":""27061351"",
         ""from_user_name"":""cu_mr2ducks"",
         ""geo"":null,
         ""id"":155777569373945856,
         ""id_str"":""155777569373945856"",
         ""iso_language_code"":""en"",
         ""metadata"":{
            ""result_type"":""recent""
         },
         ""profile_image_url"":""http://a3.twimg.com/profile_images/1508391830/IMG_0004_normal.JPG"",
         ""profile_image_url_https"":""https://si0.twimg.com/profile_images/1508391830/IMG_0004_normal.JPG"",
         ""source"":""&lt;a href=&quot;http://twitter.com/#!/download/iphone&quot; rel=&quot;nofollow&quot;&gt;Twitter for iPhone&lt;/a&gt;"",
         ""text"":""#Presidential Race -Intelligence without character is hallow. #Newt multiple affairs. Trust? Even Blue Angels are to be faithful."",
         ""to_user"":null,
         ""to_user_id"":null,
         ""to_user_id_str"":null,
         ""to_user_name"":null
      },
      {
         ""created_at"":""Sat, 07 Jan 2012 21:24:50 +0000"",
         ""entities"":{
            ""user_mentions"":[
               {
                  ""screen_name"":""DesharThomas30"",
                  ""name"":""DeShar Thomas"",
                  ""id"":323629022,
                  ""id_str"":""323629022"",
                  ""indices"":[
                     0,
                     15
                  ]
               }
            ]
         },
         ""from_user"":""OurtneyLamie"",
         ""from_user_id"":280234351,
         ""from_user_id_str"":""280234351"",
         ""from_user_name"":""Court Lamie"",
         ""geo"":null,
         ""id"":155761836736786432,
         ""id_str"":""155761836736786432"",
         ""iso_language_code"":""en"",
         ""metadata"":{
            ""result_type"":""recent""
         },
         ""profile_image_url"":""http://a2.twimg.com/profile_images/1730998374/image_normal.jpg"",
         ""profile_image_url_https"":""https://si0.twimg.com/profile_images/1730998374/image_normal.jpg"",
         ""source"":""&lt;a href=&quot;http://twitter.com/#!/download/iphone&quot; rel=&quot;nofollow&quot;&gt;Twitter for iPhone&lt;/a&gt;"",
         ""text"":""@DesharThomas30 Ohhh haha Blue devils are mean because they are devils. And umm UOFM are nice like angels (:"",
         ""to_user"":""DesharThomas30"",
         ""to_user_id"":323629022,
         ""to_user_id_str"":""323629022"",
         ""to_user_name"":""DeShar Thomas"",
         ""in_reply_to_status_id"":155761481038835713,
         ""in_reply_to_status_id_str"":""155761481038835713""
      },
      {
         ""created_at"":""Sat, 07 Jan 2012 18:42:36 +0000"",
         ""entities"":{
            ""user_mentions"":[
               {
                  ""screen_name"":""rschu"",
                  ""name"":""Ren\u00e9 Schulte"",
                  ""id"":18668342,
                  ""id_str"":""18668342"",
                  ""indices"":[
                     3,
                     9
                  ]
               },
               {
                  ""screen_name"":""PicturesLab"",
                  ""name"":""Pictures Lab"",
                  ""id"":195138719,
                  ""id_str"":""195138719"",
                  ""indices"":[
                     37,
                     49
                  ]
               }
            ],
            ""media"":[
               {
                  ""id"":155683816676134913,
                  ""id_str"":""155683816676134913"",
                  ""indices"":[
                     59,
                     79
                  ],
                  ""media_url"":""http://p.twimg.com/AikZmz5CEAESBHD.jpg"",
                  ""media_url_https"":""https://p.twimg.com/AikZmz5CEAESBHD.jpg"",
                  ""url"":""http://t.co/36MZIOyW"",
                  ""display_url"":""pic.twitter.com/36MZIOyW"",
                  ""expanded_url"":""http://twitter.com/rschu/status/155683816671940609/photo/1"",
                  ""type"":""photo"",
                  ""sizes"":{
                     ""orig"":{
                        ""w"":1161,
                        ""h"":925,
                        ""resize"":""fit""
                     },
                     ""thumb"":{
                        ""w"":150,
                        ""h"":150,
                        ""resize"":""crop""
                     },
                     ""large"":{
                        ""w"":1024,
                        ""h"":816,
                        ""resize"":""fit""
                     },
                     ""small"":{
                        ""w"":340,
                        ""h"":271,
                        ""resize"":""fit""
                     },
                     ""medium"":{
                        ""w"":600,
                        ""h"":478,
                        ""resize"":""fit""
                     }
                  }
               }
            ]
         },
         ""from_user"":""PicturesLab"",
         ""from_user_id"":195138719,
         ""from_user_id_str"":""195138719"",
         ""from_user_name"":""Pictures Lab"",
         ""geo"":null,
         ""id"":155721009704599552,
         ""id_str"":""155721009704599552"",
         ""iso_language_code"":""en"",
         ""metadata"":{
            ""result_type"":""recent""
         },
         ""profile_image_url"":""http://a3.twimg.com/profile_images/1138811413/VideoLogo_ohne_Text_400x400_normal.png"",
         ""profile_image_url_https"":""https://si0.twimg.com/profile_images/1138811413/VideoLogo_ohne_Text_400x400_normal.png"",
         ""source"":""&lt;a href=&quot;http://pictureslab.rene-schulte.info&quot; rel=&quot;nofollow&quot;&gt;Pictures Lab&lt;/a&gt;"",
         ""text"":""RT @rschu: Goodbye Hygiene Museum. | @PicturesLab Sepia FX http://t.co/36MZIOyW"",
         ""to_user"":null,
         ""to_user_id"":null,
         ""to_user_id_str"":null,
         ""to_user_name"":null
      }
   ],
   ""results_per_page"":15,
   ""since_id"":3,
   ""since_id_str"":""3""
}";
    }
}
