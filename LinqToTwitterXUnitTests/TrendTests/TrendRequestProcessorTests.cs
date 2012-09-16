using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.TrendTests
{
    public class TrendRequestProcessorTests
    {
        public TrendRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void ProcessResults_Handles_Available()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { Type = TrendType.Available };

            List<Trend> trends = trendReqProc.ProcessResults(TestAvailableQueryResponse);

            Assert.Equal(2, trends[0].Locations.Count);
            Location loc = trends[0].Location;
            Assert.Equal("Country", loc.PlaceTypeName);
            Assert.Equal(12, loc.PlaceTypeNameCode);
            Assert.Equal("Turkey", loc.Name);
            Assert.Equal("http://where.yahooapis.com/v1/place/23424969", loc.Url);
            Assert.Equal("23424969", loc.WoeID);
            Assert.Equal("TR", loc.CountryCode);
            Assert.Equal("1", loc.ParentID);
            Assert.Equal("Turkey", loc.Country);
        }

        [Fact]
        public void ProcessResults_Parses_Location()
        {
            var trendProc = new TrendRequestProcessor<Trend> { Type = TrendType.Place };

            List<Trend> trends = trendProc.ProcessResults(TestTrendQueryResponse);

            Assert.Equal(1, trends[0].Locations.Count);

            Location loc = trends[0].Locations.First();
            Assert.Equal("Worldwide", loc.Name);
            Assert.Equal("1", loc.WoeID);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var trendProc = new TrendRequestProcessor<Trend>();

            var trends = trendProc.ProcessResults(string.Empty);

            Assert.Equal(0, trends.Count);
        }

        [Fact]
        public void GetParameters_Collects_Parameters()
        {
            const bool ShouldExcludeHashtags = true;
            var trendReqProc = new TrendRequestProcessor<Trend>();
            Expression<Func<Trend, bool>> expression =
                trend =>
                    trend.Type == TrendType.Available &&
                    trend.ExcludeHashtags == ShouldExcludeHashtags;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = trendReqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture))));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ExcludeHashtags", "True")));
        }

        [Fact]
        public void BuildUrl_Handles_Available_Trends()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/trends/available.json";
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture) }
                };

            Request req = trendReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Constructs_AvailableTrends_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/trends/available.json?lat=37.78215&long=-122.40060";
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture) },
                    { "Latitude", "37.78215" },
                    { "Longitude", "-122.40060" }
                };

            Request req = trendReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_On_AvailableTrends_Without_Latitude()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture) },
                    { "Longitude", "-122.40060" }
                };

            var ex = Assert.Throws<ArgumentException>(() => trendReqProc.BuildUrl(parameters));

            Assert.Equal("Latitude/Longitude", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Constructs_Place_Trends_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/trends/place.json?id=1";
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Place).ToString(CultureInfo.InvariantCulture) },
                    { "WeoID", "1" }
                };

            Request req = trendReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_On_Place_Trends_WithoutWoeID()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Place).ToString(CultureInfo.InvariantCulture) },
                };

            var ex = Assert.Throws<ArgumentException>(() => trendReqProc.BuildUrl(parameters));

            Assert.Equal("WeoID", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_When_Type_Not_Provided()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>();

            var ex = Assert.Throws<ArgumentException>(() => trendReqProc.BuildUrl(parameters));

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_With_No_Parameters()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };

            var ex = Assert.Throws<ArgumentException>(() => trendReqProc.BuildUrl(null));

            Assert.Equal("Type", ex.ParamName);
        }

        const string TestTrendQueryResponse = @"[
   {
      ""as_of"":""2011-09-19T02:04:39Z"",
      ""locations"":[
         {
            ""name"":""Worldwide"",
            ""woeid"":1
         }
      ],
      ""trends"":[
         {
            ""query"":""%22Julianna%20Margulies%22"",
            ""name"":""Julianna Margulies"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Julianna%20Margulies%22"",
            ""events"":null
         },
         {
            ""query"":""%23ThingsThatGetMeUpset"",
            ""name"":""#ThingsThatGetMeUpset"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%23ThingsThatGetMeUpset"",
            ""events"":null
         },
         {
            ""query"":""%23ReasonsWeCantBeTogether"",
            ""name"":""#ReasonsWeCantBeTogether"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%23ReasonsWeCantBeTogether"",
            ""events"":null
         },
         {
            ""query"":""%22Peter%20Dinklage%22"",
            ""name"":""Peter Dinklage"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Peter%20Dinklage%22"",
            ""events"":null
         },
         {
            ""query"":""%22Connie%20Britton%22"",
            ""name"":""Connie Britton"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Connie%20Britton%22"",
            ""events"":null
         },
         {
            ""query"":""%23100BestGogoSongs"",
            ""name"":""#100BestGogoSongs"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%23100BestGogoSongs"",
            ""events"":null
         },
         {
            ""query"":""%22Kyle%20Chandler%22"",
            ""name"":""Kyle Chandler"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Kyle%20Chandler%22"",
            ""events"":null
         },
         {
            ""query"":""%22Tony%20Gonzalez%22"",
            ""name"":""Tony Gonzalez"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Tony%20Gonzalez%22"",
            ""events"":null
         },
         {
            ""query"":""%22Drew%20Barrymore%22"",
            ""name"":""Drew Barrymore"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Drew%20Barrymore%22"",
            ""events"":null
         },
         {
            ""query"":""%22Elisabeth%20Moss%22"",
            ""name"":""Elisabeth Moss"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Elisabeth%20Moss%22"",
            ""events"":null
         }
      ],
      ""created_at"":""2011-09-19T02:00:17Z""
   }
]";

        const string TestAvailableQueryResponse = @"[
   {
      ""placeType"":{
         ""name"":""Country"",
         ""code"":12
      },
      ""name"":""Turkey"",
      ""url"":""http:\/\/where.yahooapis.com\/v1\/place\/23424969"",
      ""woeid"":23424969,
      ""countryCode"":""TR"",
      ""parentid"":1,
      ""country"":""Turkey""
   },
   {
      ""name"":""Birmingham"",
      ""url"":""http:\/\/where.yahooapis.com\/v1\/place\/2364559"",
      ""parentid"":23424977,
      ""woeid"":2364559,
      ""countryCode"":""US"",
      ""placeType"":{
         ""name"":""Town"",
         ""code"":7
      },
      ""country"":""United States""
   }
]";

        const string TestDailyQueryResponse = @"{
           ""trends"":{
              ""2011-09-05 11:00"":[
                 {
                    ""name"":""#Youshouldbeashamedif"",
                    ""query"":""#Youshouldbeashamedif"",
                    ""events"":null,
                    ""promoted_content"":null
                 },
                 {
                    ""name"":""#iReallyHateWhen"",
                    ""query"":""#iReallyHateWhen"",
                    ""events"":null,
                    ""promoted_content"":null
                 }
              ],
              ""2011-09-05 10:00"":[
                 {
                    ""name"":""#iReallyHateWhen"",
                    ""query"":""#iReallyHateWhen"",
                    ""events"":null,
                    ""promoted_content"":null
                 },
                 {
                    ""name"":""#Youshouldbeashamedif"",
                    ""query"":""#Youshouldbeashamedif"",
                    ""events"":null,
                    ""promoted_content"":null
                 }
              ]
           },
           ""as_of"":1315440609
         }";
    }
}
