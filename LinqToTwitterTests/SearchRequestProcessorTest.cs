using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for SearchRequestProcessorTest and is intended
    ///to contain all SearchRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SearchRequestProcessorTest
    {
        private TestContext testContextInstance;

        #region Test Data

        private string m_testQueryResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<feed xmlns:google=""http://base.google.com/ns/1.0"" xml:lang=""en-US"" xmlns:openSearch=""http://a9.com/-/spec/opensearch/1.1/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:twitter=""http://api.twitter.com/"">
  <id>tag:search.twitter.com,2005:search/</id>
  <link type=""text/html"" href=""https://search.twitter.com/search?q="" rel=""alternate""/>
  <link type=""application/atom+xml"" href=""https://search.twitter.com/search.atom?geocode=39.5485127%2C-104.9230675%2C500km&amp;oauth_consumer_key=7fNSFc8WaIjqghRM5fkzw&amp;oauth_nonce=614273&amp;oauth_signature_method=HMAC-SHA1&amp;oauth_timestamp=1309632860&amp;oauth_token=15411837-wzrBGuT7n2fjDW8aed4qfokPL0y7b4r5cjE0yX7Oo&amp;oauth_verifier=7605900&amp;oauth_version=1.0&amp;oauth_signature=7NFFpuO8bexRC%2bwwlV7abmLOICo%3d"" rel=""self""/>
  <title> - Twitter Search</title>
  <link type=""application/opensearchdescription+xml"" href=""http://search.twitter.com/opensearch.xml"" rel=""search""/>
  <link type=""application/atom+xml"" href=""https://search.twitter.com/search.atom?geocode=39.5485127%2C-104.9230675%2C500km&amp;oauth_consumer_key=7fNSFc8WaIjqghRM5fkzw&amp;oauth_nonce=614273&amp;oauth_signature=7NFFpuO8bexRC%2BwwlV7abmLOICo%3D&amp;oauth_signature_method=HMAC-SHA1&amp;oauth_timestamp=1309632860&amp;oauth_token=15411837-wzrBGuT7n2fjDW8aed4qfokPL0y7b4r5cjE0yX7Oo&amp;oauth_verifier=7605900&amp;oauth_version=1.0&amp;since_id=87232168752988160"" rel=""refresh""/>
  <twitter:warning>adjusted since_id to 84682264511922176 (), requested since_id was older than allowedsince_id removed for pagination.</twitter:warning>
  <updated>2011-07-02T18:52:24Z</updated>
  <openSearch:itemsPerPage>15</openSearch:itemsPerPage>
  <link type=""application/atom+xml"" href=""https://search.twitter.com/search.atom?geocode=39.5485127%2C-104.9230675%2C500.0km&amp;max_id=87232168752988160&amp;page=2&amp;q="" rel=""next""/>
  <entry>
    <id>tag:search.twitter.com,2005:87232168752988160</id>
    <published>2011-07-02T18:52:24Z</published>
    <link type=""text/html"" href=""http://twitter.com/amberrmcfly/statuses/87232168752988160"" rel=""alternate""/>
    <title>@SarahNeateX thankyou :) i was gonna do that but i decided to try it just in case :L x</title>
    <content type=""html"">&lt;a href=&quot;http://twitter.com/SarahNeateX&quot;&gt;@SarahNeateX&lt;/a&gt; thankyou :) i was gonna do that but i decided to try it just in case :L x</content>
    <updated>2011-07-02T18:52:24Z</updated>
    <link type=""image/png"" href=""http://a3.twimg.com/profile_images/1396078312/cam_062_normal.jpg"" rel=""image""/>
    <google:location>Devizes</google:location>
    <twitter:geo>
    </twitter:geo>
    <twitter:metadata>
      <twitter:result_type>recent</twitter:result_type>
    </twitter:metadata>
    <twitter:source>&lt;a href=&quot;http://twitter.com/&quot;&gt;web&lt;/a&gt;</twitter:source>
    <twitter:lang>en</twitter:lang>
    <author>
      <name>amberrmcfly (Amber Elliott)</name>
      <uri>http://twitter.com/amberrmcfly</uri>
    </author>
  </entry>
</feed>";

        private string m_testQueryResponse2 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<feed xmlns:google=""http://base.google.com/ns/1.0"" xml:lang=""en-US"" xmlns:openSearch=""http://a9.com/-/spec/opensearch/1.1/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:twitter=""http://api.twitter.com/"">
  <id>tag:search.twitter.com,2005:search/LINQ to Twitter</id>
  <link type=""text/html"" rel=""alternate"" href=""http://search.twitter.com/search?q=LINQ+to+Twitter""/>
  <link type=""application/atom+xml"" rel=""self"" href=""http://search.twitter.com/search.atom?lang=en&amp;q=LINQ%20to%20Twitter&amp;rpp=2&amp;show_user=true""/>
  <title>LINQ to Twitter - Twitter Search</title>
  <link type=""application/opensearchdescription+xml"" rel=""search"" href=""http://search.twitter.com/opensearch.xml""/>
  <link type=""application/atom+xml"" rel=""refresh"" href=""http://search.twitter.com/search.atom?lang=en&amp;q=LINQ+to+Twitter&amp;rpp=2&amp;show_user=true&amp;since_id=1600414821""/>
  <twitter:warning>adjusted since_id, it was older than allowed</twitter:warning>
  <updated>2009-04-23T08:33:40Z</updated>
  <openSearch:itemsPerPage>2</openSearch:itemsPerPage>
  <openSearch:language>en</openSearch:language>
  <link type=""application/atom+xml"" rel=""next"" href=""http://search.twitter.com/search.atom?lang=en&amp;max_id=1600414821&amp;page=2&amp;q=LINQ+to+Twitter&amp;rpp=2""/>
  <entry>
    <id>tag:search.twitter.com,2005:1592662820</id>
    <published>2009-04-23T08:33:40Z</published>
    <link type=""text/html"" rel=""alternate"" href=""http://twitter.com/slecluyse/statuses/1592662820""/>
    <title>slecluyse: LINQ to Twitter http://bit.ly/11vAp</title>
    <content type=""html"">slecluyse: &lt;b&gt;LINQ&lt;/b&gt; &lt;b&gt;to&lt;/b&gt; &lt;b&gt;Twitter&lt;/b&gt; &lt;a href=""http://bit.ly/11vAp""&gt;http://bit.ly/11vAp&lt;/a&gt;</content>
    <updated>2009-04-23T08:33:40Z</updated>
    <link type=""image/png"" rel=""image"" href=""http://s3.amazonaws.com/twitter_production/profile_images/178528955/linkedin_normal.jpg""/>
    <twitter:source>&lt;a href=""http://www.tweetdeck.com/""&gt;TweetDeck&lt;/a&gt;</twitter:source>
    <twitter:lang>en</twitter:lang>
    <author>
      <name>slecluyse (Steven Lecluyse)</name>
      <uri>http://twitter.com/slecluyse</uri>
    </author>
  </entry>
  <entry>
    <id>tag:search.twitter.com,2005:1575147318</id>
    <published>2009-04-21T14:12:41Z</published>
    <link type=""text/html"" rel=""alternate"" href=""http://twitter.com/JackStow/statuses/1575147318""/>
    <title>JackStow: Blog post: A Simple Twitter Status ASP.NET User Control With LINQ To XML: 
The blog system I'm us.. http://tinyurl.com/cvdbvr</title>
    <content type=""html"">JackStow: Blog post: A Simple &lt;b&gt;Twitter&lt;/b&gt; Status ASP.NET User Control With &lt;b&gt;LINQ&lt;/b&gt; &lt;b&gt;To&lt;/b&gt; XML: 
The blog system I'm us.. &lt;a href=""http://tinyurl.com/cvdbvr""&gt;http://tinyurl.com/cvdbvr&lt;/a&gt;</content>
    <updated>2009-04-21T14:12:41Z</updated>
    <link type=""image/png"" rel=""image"" href=""http://s3.amazonaws.com/twitter_production/profile_images/63057949/Twitter-Jack-Small_normal.jpg""/>
    <twitter:source>&lt;a href=""http://twitterfeed.com""&gt;twitterfeed&lt;/a&gt;</twitter:source>
    <twitter:lang>en</twitter:lang>
    <author>
      <name>JackStow (Jack Stow)</name>
      <uri>http://twitter.com/JackStow</uri>
    </author>
  </entry>
</feed>";

        string m_emptyResponse = @"<feed xmlns:google=""http://base.google.com/ns/1.0"" xml:lang=""en-US"" xmlns:openSearch=""http://a9.com/-/spec/opensearch/1.1/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:twitter=""http://api.twitter.com/"">
  <id>tag:search.twitter.com,2005:search/twitter:LINQtoTwitter</id>
  <link type=""text/html"" rel=""alternate"" href=""http://search.twitter.com/search?q=twitter%3ALINQtoTwitter"" />
  <link type=""application/atom+xml"" rel=""self"" href=""http://search.twitter.com/search.atom?q=twitter:LINQtoTwitter"" />
  <title>twitter:LINQtoTwitter - Twitter Search</title>
  <link type=""application/opensearchdescription+xml"" rel=""search"" href=""http://search.twitter.com/opensearch.xml"" />
  <updated>2009-04-25T23:03:55+00:00</updated>
</feed>";

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>();

            IList actual = target.ProcessResults(m_testQueryResponse);

            var result = actual.Cast<Search>().First();
            Assert.AreEqual(1, result.Entries.Count);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessEmptyResultsTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>();

            IList actual = target.ProcessResults(m_emptyResponse);

            var result = actual.Cast<Search>().First();
            Assert.AreEqual(0, result.Entries.Count);
        }

        [TestMethod]
        public void ProcessResults_Returns_Values()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>();

            IList actual = target.ProcessResults(m_testQueryResponse);

            var result = actual.Cast<Search>().First();
            Assert.AreEqual("tag:search.twitter.com,2005:search/", result.ID);
            Assert.AreEqual(" - Twitter Search", result.Title);
            Assert.AreEqual(string.Empty, result.Language);
            Assert.AreEqual("https://search.twitter.com/search.atom?geocode=39.5485127%2C-104.9230675%2C500.0km&max_id=87232168752988160&page=2&q=", result.Next);
            Assert.AreEqual("https://search.twitter.com/search.atom?geocode=39.5485127%2C-104.9230675%2C500km&oauth_consumer_key=7fNSFc8WaIjqghRM5fkzw&oauth_nonce=614273&oauth_signature=7NFFpuO8bexRC%2BwwlV7abmLOICo%3D&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1309632860&oauth_token=15411837-wzrBGuT7n2fjDW8aed4qfokPL0y7b4r5cjE0yX7Oo&oauth_verifier=7605900&oauth_version=1.0&since_id=87232168752988160", result.Refresh);
            Assert.AreEqual("http://search.twitter.com/opensearch.xml", result.Search);
            Assert.AreEqual("https://search.twitter.com/search.atom?geocode=39.5485127%2C-104.9230675%2C500km&oauth_consumer_key=7fNSFc8WaIjqghRM5fkzw&oauth_nonce=614273&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1309632860&oauth_token=15411837-wzrBGuT7n2fjDW8aed4qfokPL0y7b4r5cjE0yX7Oo&oauth_verifier=7605900&oauth_version=1.0&oauth_signature=7NFFpuO8bexRC%2bwwlV7abmLOICo%3d", result.Self);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var searchProc = new SearchRequestProcessor<Search>();

            var searchResults = searchProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, searchResults.Count);
        }

        [TestMethod]
        public void ProcessResults_Returns_EntryValues()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>();

            IList actual = target.ProcessResults(m_testQueryResponse);

            var result = actual.Cast<Search>().First().Entries.First();
            Assert.AreEqual("tag:search.twitter.com,2005:87232168752988160", result.ID);
            Assert.AreEqual("@SarahNeateX thankyou :) i was gonna do that but i decided to try it just in case :L x", result.Title);
            Assert.AreEqual(@"<a href=""http://twitter.com/SarahNeateX"">@SarahNeateX</a> thankyou :) i was gonna do that but i decided to try it just in case :L x", result.Content);
            Assert.AreEqual(@"<a href=""http://twitter.com/"">web</a>", result.Source);
            Assert.AreEqual("http://twitter.com/amberrmcfly/statuses/87232168752988160", result.Alternate);
            Assert.AreEqual("http://a3.twimg.com/profile_images/1396078312/cam_062_normal.jpg", result.Image);
            Assert.AreEqual("amberrmcfly (Amber Elliott)", result.Author.Name);
            Assert.AreEqual("http://twitter.com/amberrmcfly", result.Author.URI);
            Assert.AreEqual(new DateTime(2011, 7, 2, 18, 52, 24, DateTimeKind.Utc), result.Published);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>();
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
                    search.WithRetweets == true;
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)SearchType.Search).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("GeoCode", "40.757929,-73.985506,25km")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SearchLanguage", "en")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("PageSize", "10")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Query", "LINQ to Twitter")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ShowUser", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SinceID", "123")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>("MaxID", "200")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>("ResultType", ((int)ResultType.Popular).ToString())));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WordPhrase", "LINQ to Twitter")));
            Assert.IsTrue(
             queryParams.Contains(
                 new KeyValuePair<string, string>("WordAnd", "LINQ Twitter")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WordOr", "LINQ Twitter")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WordNot", "LINQ Twitter")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Hashtag", "linqtotwitter")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("PersonFrom", "JoeMayo")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("PersonTo", "JoeMayo")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("PersonReference", "JoeMayo")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Attitude", ((int)Attitude.Positive).ToString())));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WithLinks", "True")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("WithRetweets", "True")));
        }

        [TestMethod]
        public void BuildURLTest()
        {
            var searchReqProc = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
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
                    { "Since", new DateTime(2010, 7, 4).ToString() },
                    { "Until", new DateTime(2011, 7, 4).ToString() },
                    { "ResultType", ResultType.Popular.ToString() },
               };
            string expected = "http://search.twitter.com/search.atom?geocode=40.757929%2C-73.985506%2C25km&lang=en&page=1&rpp=10&q=LINQ%20to%20Twitter&show_user=true&since=2010-07-04&until=2011-07-04&since_id=1&result_type=popular";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildURL_Uses_Only_Date_Part_Of_Since()
        {
            var searchReqProc = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Since", new DateTime(2010, 7, 4, 7, 30, 10).ToString() },
               };
            string expected = "http://search.twitter.com/search.atom?since=2010-07-04";

            Request req = searchReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL with ShowUser set to false
        ///</summary>
        [TestMethod()]
        public void BuildShowUserSetToFalseURLTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "ShowUser", false.ToString() },
                };
            string expected = "http://search.twitter.com/search.atom";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL with words query parameters
        ///</summary>
        [TestMethod()]
        public void BuildWordsURLTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "WordPhrase", "LINQ to Twitter" },
                    { "WordAnd", "LINQ Twitter" },
                    { "WordOr", "LINQ Twitter" },
                    { "WordNot", "LINQ Twitter" },
                    { "Hashtag", "linqtotwitter" },
               };
            string expected = "http://search.twitter.com/search.atom?exact=LINQ%20to%20Twitter&ands=LINQ%20Twitter&ors=LINQ%20Twitter&nots=LINQ%20Twitter&tag=linqtotwitter";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL with person query parameters
        ///</summary>
        [TestMethod()]
        public void BuildPersonURLTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "PersonFrom", "JoeMayo" },
                    { "PersonTo", "JoeMayo" },
                    { "PersonReference", "JoeMayo" },
              };
            string expected = "http://search.twitter.com/search.atom?from=JoeMayo&to=JoeMayo&ref=JoeMayo";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL with all attitude query parameters
        ///</summary>
        [TestMethod()]
        public void BuildAttitudeURLTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Attitude", (Attitude.Positive | Attitude.Negative | Attitude.Question).ToString() },
                };
            string expected = "http://search.twitter.com/search.atom?tude%5B%5D=%3A)&tude%5B%5D=%3A(&tude%5B%5D=%3F";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL without positive query parameters
        ///</summary>
        [TestMethod()]
        public void BuildAttitudeWithoutPositiveURLTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Attitude", (Attitude.Negative | Attitude.Question).ToString() },
                };
            string expected = "http://search.twitter.com/search.atom?tude%5B%5D=%3A(&tude%5B%5D=%3F";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL with other parameters
        ///</summary>
        [TestMethod()]
        public void BuildOtherURLTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "WithLinks", true.ToString() },
                    { "WithRetweets", true.ToString() }
                };
            string expected = "http://search.twitter.com/search.atom?filter%5B%5D=links&include%5B%5D=retweets";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL with other parameters set to false
        ///</summary>
        [TestMethod()]
        public void BuildOtherSetToFalseURLTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "WithLinks", false.ToString() },
                    { "WithRetweets", false.ToString() }
                };
            string expected = "http://search.twitter.com/search.atom";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters = new Dictionary<string, string> { };

            try
            {
                target.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual<string>("Type", ae.ParamName);
            }
        }

        /// <summary>
        ///A test for null parameters
        ///</summary>
        [TestMethod()]
        public void NullParametersTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters = null;

            try
            {
                target.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual<string>("Type", ae.ParamName);
            }
        }

        /// <summary>
        /// ensure query is URL encoded
        /// </summary>
        [TestMethod()]
        public void UrlEncodedQueryTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            string expected = target.BaseUrl + "search.atom?q=Contains%20Space";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Query", "Contains Space" }
                };

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        /// ensure query doesn't exceed 140 characters
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryTooLongTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            string expected = target.BaseUrl + "search.atom?q=Contains%20Space";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Query", "x01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" }
                };
            
            target.BuildURL(parameters);
        }
    }
}
