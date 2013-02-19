using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
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
                    search.Count == 10 &&
                    search.Query == "LINQ to Twitter" &&
                    search.SinceID == 123 &&
                    search.MaxID == 200 &&
                    search.ResultType == ResultType.Popular &&
                    search.IncludeEntities == true;
            var lambdaExpression = expression as LambdaExpression;

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
                    new KeyValuePair<string, string>("Count", "10")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Query", "LINQ to Twitter")));
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
                  new KeyValuePair<string, string>("IncludeEntities", "True")));
        }

        [Fact]
        public void BuildUrl_Includes_Parameters()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/search/tweets.json?geocode=40.757929%2C-73.985506%2C25km&lang=en&count=10&q=LINQ%20to%20Twitter&until=2011-07-04&since_id=1&result_type=popular&include_entities=false";
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "GeoCode", "40.757929,-73.985506,25km" },
                    { "SearchLanguage", "en" },
                    { "Count", "10" },
                    { "Query", "LINQ to Twitter" },
                    { "SinceID", "1" },
                    { "Until", new DateTime(2011, 7, 4).ToString() },
                    { "ResultType", ResultType.Popular.ToString() },
                    { "IncludeEntities", false.ToString() }
               };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Throws_With_Missing_Type_Parameter()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };
            var parameters = new Dictionary<string, string> { };

            ArgumentException ex =
                Assert.Throws<ArgumentException>(() =>
                {
                    searchReqProc.BuildUrl(parameters);
                });

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Throws_When_Parameters_Null()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            ArgumentException ex =
                Assert.Throws<ArgumentException>(() =>
                {
                    searchReqProc.BuildUrl(null);
                });

            Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Encodes_Query()
        {
            var searchReqProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };
            string expected = searchReqProc.BaseUrl + "tweets.json?q=Contains%20Space";
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Query", "Contains Space" }
                };

            Request req = searchReqProc.BuildUrl(parameters);

            Assert.Equal(expected, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Adds_True_IncludeEntities()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "IncludeEntities", true.ToString(CultureInfo.InvariantCulture) }
                };
            const string Expected = "https://api.twitter.com/1.1/search/tweets.json?include_entities=true";

            Request req = searchProc.BuildUrl(parameters);

            Assert.Equal(Expected, req.FullUrl);
        }

        [Fact]
        public void ProcessResults_Creates_List_Of_Status()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.NotNull(results.First().Statuses);
            Assert.True(results.First().Statuses.Any());
        }

        [Fact]
        public void ProcessResults_Populates_CreatedAt()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(
                new DateTimeOffset(2012, 9, 6, 0, 10, 12, new TimeSpan(0, 0, 0)), 
                results.First().Statuses.First().CreatedAt);
        }

        [Fact]
        public void ProcessResults_Instantiates_Entities()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.NotNull(results.First().Statuses.First().Entities);
        }

        [Fact]
        public void ProcessResults_Populates_Entity_Urls()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            List<UrlMention> urls = results.First().Statuses.First().Entities.UrlMentions;
            Assert.NotNull(urls);
            Assert.Equal(1, urls.Count);
            UrlMention firstUrl = urls.First();
            Assert.Equal("http://t.co/Cc85Yzpj", firstUrl.Url);
            Assert.Equal("http://bit.ly/PSOVso", firstUrl.ExpandedUrl);
            Assert.Equal("bit.ly/PSOVso", firstUrl.DisplayUrl);
            Assert.Equal(68, firstUrl.Start);
            Assert.Equal(88, firstUrl.End);
        }

        [Fact]
        public void ProcessResults_Populates_Entity_Hashtags()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            List<HashTagMention> hashes = results.First().Statuses[0].Entities.HashTagMentions;
            Assert.NotNull(hashes);
            Assert.Equal(3, hashes.Count);
            HashTagMention firstHash = hashes.First();
            Assert.Equal("twitterapi", firstHash.Tag);
            Assert.Equal(89, firstHash.Start);
            Assert.Equal(100, firstHash.End);
        }

        [Fact]
        public void ProcessResults_Populates_Entity_Users()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            List<UserMention> users = results.First().Statuses[0].Entities.UserMentions;
            Assert.NotNull(users);
            Assert.Equal(1, users.Count);
            UserMention firstUser = users.First();
            Assert.Equal("JoeMayo", firstUser.ScreenName);
            Assert.Equal("Joe Mayo", firstUser.Name);
            Assert.Equal(15411837ul, firstUser.Id);
            Assert.Equal(3, firstUser.Start);
            Assert.Equal(11, firstUser.End);
        }

        [Fact]
        public void ProcessResults_Populates_Entity_Media()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            List<MediaMention> media = results.First().Statuses[0].Entities.MediaMentions;
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
        public void ProcessResults_Populates_Search_MetaData()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.NotNull(results);
            Assert.Single(results);
            Search search = results.Single();
            Assert.NotNull(search);
            SearchMetaData metaData = search.SearchMetaData;
            Assert.NotNull(metaData);
            Assert.Equal(0.046m, metaData.CompletedIn);
            Assert.Equal(281604225014824960ul, metaData.MaxID);
            Assert.Equal(5ul, metaData.SinceID);
            Assert.Equal(10, metaData.Count);
            Assert.Equal("Twitter", metaData.Query);
            Assert.Equal(
                "?max_id=281562053162979328&q=Twitter&geocode=37.781157%2C-122.398720%2C1mi&include_entities=1",
                metaData.NextResults);
            Assert.Equal(
                "?since_id=281604225014824960&q=Twitter&geocode=37.781157%2C-122.398720%2C1mi&include_entities=1", 
                metaData.RefreshUrl);
        }

        [Fact]
        public void ProcessResults_Populates_Status_MetaData()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.NotNull(results);
            Assert.Single(results);
            Search search = results.Single();
            Assert.NotNull(search);
            List<Status> statuses = search.Statuses;
            Assert.NotNull(statuses);
            Assert.NotEmpty(statuses);
            Status status = search.Statuses.First();
            Assert.NotNull(status);
            StatusMetaData metaData = status.MetaData;
            Assert.NotNull(metaData);
            Assert.Equal("recent", metaData.ResultType);
            Assert.Equal("en", metaData.IsoLanguageCode);
        }

        [Fact]
        public void ProcessResults_Populates_Source()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal(@"<a href=""http://www.csharp-station.com/"" rel=""nofollow"">C# Station</a>", results.First().Statuses.First().Source);
        }

        [Fact]
        public void ProcessResults_Populates_Text()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> results = searchProc.ProcessResults(SearchJson);

            Assert.Equal("RT @JoeMayo: Blogged - Working with Timelines with LINQ to Twitter: http://t.co/Cc85Yzpj #twitterapi #linq #linq2twitter", results.First().Statuses.First().Text);
        }

        [Fact]
        public void ProcessResults_Handles_Response_With_No_Results()
        {
            var searchProc = new SearchRequestProcessor<Search> { BaseUrl = "https://api.twitter.com/1.1/search/" };

            List<Search> searches = searchProc.ProcessResults(EmptyResponse);

            Assert.NotNull(searches);
            Assert.Single(searches);
            var search = searches.Single();
            Assert.NotNull(search);
            var results = search.Statuses;
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        const string SearchJson = @"{
   ""statuses"":[
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""en""
         },
         ""created_at"":""Thu Sep 06 00:10:12 +0000 2012"",
         ""id"":243501315039322112,
         ""id_str"":""243501315039322112"",
         ""text"":""RT @JoeMayo: Blogged - Working with Timelines with LINQ to Twitter: http:\/\/t.co\/Cc85Yzpj #twitterapi #linq #linq2twitter"",
         ""source"":""\u003ca href=\""http:\/\/www.csharp-station.com\/\"" rel=\""nofollow\""\u003eC# Station\u003c\/a\u003e"",
         ""truncated"":false,
         ""in_reply_to_status_id"":null,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_user_id"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""entities"":{
            ""hashtags"":[
               {
                  ""text"":""twitterapi"",
                  ""indices"":[
                     89,
                     100
                  ]
               },
               {
                  ""text"":""linq"",
                  ""indices"":[
                     101,
                     106
                  ]
               },
               {
                  ""text"":""linq2twitter"",
                  ""indices"":[
                     107,
                     120
                  ]
               }
            ],
            ""urls"":[
               {
                  ""url"":""http:\/\/t.co\/Cc85Yzpj"",
                  ""expanded_url"":""http:\/\/bit.ly\/PSOVso"",
                  ""display_url"":""bit.ly\/PSOVso"",
                  ""indices"":[
                     68,
                     88
                  ]
               }
            ],
            ""user_mentions"":[
               {
                  ""screen_name"":""JoeMayo"",
                  ""name"":""Joe Mayo"",
                  ""id"":15411837,
                  ""id_str"":""15411837"",
                  ""indices"":[
                     3,
                     11
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
         ""favorited"":false,
         ""retweeted"":false,
         ""possibly_sensitive"":false
      },
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""vi""
         },
         ""created_at"":""Tue Sep 04 23:08:16 +0000 2012"",
         ""id"":243123342771625985,
         ""id_str"":""243123342771625985"",
         ""text"":""Testing LINQ to Twitter Windows 8 support: 05\/09\/2012 00:08:13"",
         ""source"":""\u003ca href=\""http:\/\/www.BradStevo.info\"" rel=\""nofollow\""\u003eIIVVYTest\u003c\/a\u003e"",
         ""truncated"":false,
         ""in_reply_to_status_id"":null,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_user_id"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""entities"":{
            ""hashtags"":[

            ],
            ""urls"":[

            ],
            ""user_mentions"":[

            ]
         },
         ""favorited"":false,
         ""retweeted"":false
      },
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""vi""
         },
         ""created_at"":""Tue Sep 04 23:01:51 +0000 2012"",
         ""id"":243121726920224769,
         ""id_str"":""243121726920224769"",
         ""text"":""Testing LINQ to Twitter Windows 8 support: 05\/09\/2012 00:01:47"",
         ""source"":""\u003ca href=\""http:\/\/www.BradStevo.info\"" rel=\""nofollow\""\u003eIIVVYTest\u003c\/a\u003e"",
         ""truncated"":false,
         ""in_reply_to_status_id"":null,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_user_id"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""entities"":{
            ""hashtags"":[

            ],
            ""urls"":[

            ],
            ""user_mentions"":[

            ]
         },
         ""favorited"":false,
         ""retweeted"":false
      },
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""vi""
         },
         ""created_at"":""Tue Sep 04 23:01:40 +0000 2012"",
         ""id"":243121682787741696,
         ""id_str"":""243121682787741696"",
         ""text"":""Testing LINQ to Twitter Windows 8 support: 05\/09\/2012 00:01:39"",
         ""source"":""\u003ca href=\""http:\/\/www.BradStevo.info\"" rel=\""nofollow\""\u003eIIVVYTest\u003c\/a\u003e"",
         ""truncated"":false,
         ""in_reply_to_status_id"":null,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_user_id"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""entities"":{
            ""hashtags"":[

            ],
            ""urls"":[

            ],
            ""user_mentions"":[

            ]
         },
         ""favorited"":false,
         ""retweeted"":false
      },
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""en""
         },
         ""created_at"":""Tue Sep 04 06:34:40 +0000 2012"",
         ""id"":242873292950757376,
         ""id_str"":""242873292950757376"",
         ""text"":""Check out Working with Timelines with LINQ to Twitter written by @JoeMayo http:\/\/t.co\/wTSodeyq"",
         ""source"":""\u003ca href=\""http:\/\/twitter.com\/tweetbutton\"" rel=\""nofollow\""\u003eTweet Button\u003c\/a\u003e"",
         ""truncated"":false,
         ""in_reply_to_status_id"":null,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_user_id"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""entities"":{
            ""hashtags"":[

            ],
            ""urls"":[
               {
                  ""url"":""http:\/\/t.co\/wTSodeyq"",
                  ""expanded_url"":""http:\/\/wblo.gs\/dB3"",
                  ""display_url"":""wblo.gs\/dB3"",
                  ""indices"":[
                     74,
                     94
                  ]
               }
            ],
            ""user_mentions"":[
               {
                  ""screen_name"":""JoeMayo"",
                  ""name"":""Joe Mayo"",
                  ""id"":15411837,
                  ""id_str"":""15411837"",
                  ""indices"":[
                     65,
                     73
                  ]
               }
            ]
         },
         ""favorited"":false,
         ""retweeted"":false,
         ""possibly_sensitive"":false
      },
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""en""
         },
         ""created_at"":""Mon Sep 03 06:18:02 +0000 2012"",
         ""id"":242506723078836224,
         ""id_str"":""242506723078836224"",
         ""text"":""RT @JoeMayo: Blogged - Working with Timelines with LINQ to Twitter: http:\/\/t.co\/Cc85Yzpj #twitterapi #linq #linq2twitter"",
         ""source"":""web"",
         ""truncated"":false,
         ""in_reply_to_status_id"":null,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_user_id"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""entities"":{
            ""hashtags"":[
               {
                  ""text"":""twitterapi"",
                  ""indices"":[
                     89,
                     100
                  ]
               },
               {
                  ""text"":""linq"",
                  ""indices"":[
                     101,
                     106
                  ]
               },
               {
                  ""text"":""linq2twitter"",
                  ""indices"":[
                     107,
                     120
                  ]
               }
            ],
            ""urls"":[
               {
                  ""url"":""http:\/\/t.co\/Cc85Yzpj"",
                  ""expanded_url"":""http:\/\/bit.ly\/PSOVso"",
                  ""display_url"":""bit.ly\/PSOVso"",
                  ""indices"":[
                     68,
                     88
                  ]
               }
            ],
            ""user_mentions"":[
               {
                  ""screen_name"":""JoeMayo"",
                  ""name"":""Joe Mayo"",
                  ""id"":15411837,
                  ""id_str"":""15411837"",
                  ""indices"":[
                     3,
                     11
                  ]
               }
            ]
         },
         ""favorited"":false,
         ""retweeted"":false,
         ""possibly_sensitive"":false
      },
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""en""
         },
         ""created_at"":""Mon Sep 03 04:12:43 +0000 2012"",
         ""id"":242475182780973056,
         ""id_str"":""242475182780973056"",
         ""text"":""Blogged - Working with Timelines with LINQ to Twitter: http:\/\/t.co\/Cc85Yzpj #twitterapi #linq #linq2twitter"",
         ""source"":""web"",
         ""truncated"":false,
         ""in_reply_to_status_id"":null,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_user_id"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":2,
         ""entities"":{
            ""hashtags"":[
               {
                  ""text"":""twitterapi"",
                  ""indices"":[
                     76,
                     87
                  ]
               },
               {
                  ""text"":""linq"",
                  ""indices"":[
                     88,
                     93
                  ]
               },
               {
                  ""text"":""linq2twitter"",
                  ""indices"":[
                     94,
                     107
                  ]
               }
            ],
            ""urls"":[
               {
                  ""url"":""http:\/\/t.co\/Cc85Yzpj"",
                  ""expanded_url"":""http:\/\/bit.ly\/PSOVso"",
                  ""display_url"":""bit.ly\/PSOVso"",
                  ""indices"":[
                     55,
                     75
                  ]
               }
            ],
            ""user_mentions"":[

            ]
         },
         ""favorited"":false,
         ""retweeted"":false,
         ""possibly_sensitive"":false
      },
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""en""
         },
         ""created_at"":""Fri Aug 31 20:02:43 +0000 2012"",
         ""id"":241627095502041088,
         ""id_str"":""241627095502041088"",
         ""text"":""@ChevonChr Here's an example what i use LINQ on to extract the user values http:\/\/t.co\/UL222Y5Q"",
         ""source"":""\u003ca href=\""http:\/\/www.metrotwit.com\/\"" rel=\""nofollow\""\u003eMetroTwit\u003c\/a\u003e"",
         ""truncated"":false,
         ""in_reply_to_status_id"":241623155712610306,
         ""in_reply_to_status_id_str"":""241623155712610306"",
         ""in_reply_to_user_id"":128105076,
         ""in_reply_to_user_id_str"":""128105076"",
         ""in_reply_to_screen_name"":""ChevonChr"",
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""entities"":{
            ""hashtags"":[

            ],
            ""urls"":[
               {
                  ""url"":""http:\/\/t.co\/UL222Y5Q"",
                  ""expanded_url"":""http:\/\/j.mp\/ODpb3E"",
                  ""display_url"":""j.mp\/ODpb3E"",
                  ""indices"":[
                     75,
                     95
                  ]
               }
            ],
            ""user_mentions"":[
               {
                  ""screen_name"":""ChevonChr"",
                  ""name"":""Chevon Christie"",
                  ""id"":128105076,
                  ""id_str"":""128105076"",
                  ""indices"":[
                     0,
                     10
                  ]
               }
            ]
         },
         ""favorited"":false,
         ""retweeted"":false,
         ""possibly_sensitive"":false
      },
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""en""
         },
         ""created_at"":""Thu Aug 30 22:48:05 +0000 2012"",
         ""id"":241306323923390464,
         ""id_str"":""241306323923390464"",
         ""text"":""Cool, I'm about to reach 100 followers! See the rest of my stats at Twitter Counter: http:\/\/t.co\/QH864mhf"",
         ""source"":""\u003ca href=\""http:\/\/twittercounter.com\"" rel=\""nofollow\""\u003eThe Visitor Widget\u003c\/a\u003e"",
         ""truncated"":false,
         ""in_reply_to_status_id"":null,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_user_id"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""entities"":{
            ""hashtags"":[

            ],
            ""urls"":[
               {
                  ""url"":""http:\/\/t.co\/QH864mhf"",
                  ""expanded_url"":""http:\/\/twtr.to\/lINq"",
                  ""display_url"":""twtr.to\/lINq"",
                  ""indices"":[
                     85,
                     105
                  ]
               }
            ],
            ""user_mentions"":[

            ]
         },
         ""favorited"":false,
         ""retweeted"":false,
         ""possibly_sensitive"":false
      },
      {
         ""metadata"":{
            ""result_type"":""recent"",
            ""iso_language_code"":""es""
         },
         ""created_at"":""Thu Aug 30 00:00:51 +0000 2012"",
         ""id"":240962249802530816,
         ""id_str"":""240962249802530816"",
         ""text"":""TWITTER to LINQ parece tan sencillo... No s\u00e9 porqu\u00e9 me trae tantos problemas :S"",
         ""source"":""\u003ca href=\""http:\/\/blackberry.com\/twitter\"" rel=\""nofollow\""\u003eTwitter for BlackBerry\u00ae\u003c\/a\u003e"",
         ""truncated"":false,
         ""in_reply_to_status_id"":null,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_user_id"":null,
         ""in_reply_to_user_id_str"":null,
         ""in_reply_to_screen_name"":null,
         ""geo"":null,
         ""coordinates"":null,
         ""place"":null,
         ""contributors"":null,
         ""retweet_count"":0,
         ""entities"":{
            ""hashtags"":[

            ],
            ""urls"":[

            ],
            ""user_mentions"":[

            ]
         },
         ""favorited"":false,
         ""retweeted"":false
      }
   ],
   ""search_metadata"":{
       ""completed_in"":0.046,
       ""max_id"":281604225014824960,
       ""max_id_str"":""281604225014824960"",
       ""next_results"":""?max_id=281562053162979328&q=Twitter&geocode=37.781157%2C-122.398720%2C1mi&include_entities=1"",
       ""query"":""Twitter"",
       ""refresh_url"":""?since_id=281604225014824960&q=Twitter&geocode=37.781157%2C-122.398720%2C1mi&include_entities=1"",
       ""count"":10,
       ""since_id"":5,
       ""since_id_str"":""5""
   }
}";

        const string EmptyResponse = @"{
   ""statuses"":[
   ],
   ""search_metadata"":{
      ""completed_in"":0.038,
      ""max_id"":243501315039322112,
      ""max_id_str"":""243501315039322112"",
      ""page"":1,
      ""query"":""LINQ+To+Twitter"",
      ""refresh_url"":""?since_id=243501315039322112&q=LINQ%20To%20Twitter&include_entities=1"",
      ""results_per_page"":15,
      ""since_id"":0,
      ""since_id_str"":""0""
   }
}";
    }
}
