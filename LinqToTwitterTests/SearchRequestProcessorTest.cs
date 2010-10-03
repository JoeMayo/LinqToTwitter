using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Collections;

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

        private string m_testQueryResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
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
            Assert.AreEqual(2, result.Entries.Count);
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

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsNextTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>();

            IList actual = target.ProcessResults(m_testQueryResponse);

            var result = actual.Cast<Search>().First();
            Assert.AreEqual("http://search.twitter.com/search.atom?lang=en&max_id=1600414821&page=2&q=LINQ+to+Twitter&rpp=2", result.Next);
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

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildURLTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
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
                    { "Since", "7/4/2010" },
                    { "Until", "7/4/2011" },
                    { "ResultType", ResultType.Popular.ToString()},
               };
            string expected = "http://search.twitter.com/search.atom?geocode=40.757929%2c-73.985506%2c25km&lang=en&page=1&rpp=10&q=LINQ+to+Twitter&show_user=true&since=2010-07-04&until=2011-07-04&since_id=1&result_type=popular";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string expected = "http://search.twitter.com/search.atom?exact=LINQ+to+Twitter&ands=LINQ+Twitter&ors=LINQ+Twitter&nots=LINQ+Twitter&tag=linqtotwitter";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string expected = "http://search.twitter.com/search.atom?tude%5B%5D=%3A%29&tude%5B%5D=%3A%28&tude%5B%5D=%3F";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string expected = "http://search.twitter.com/search.atom?tude%5B%5D=%3A%28&tude%5B%5D=%3F";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters = new Dictionary<string, string> { };
            string actual;
            try
            {
                actual = target.BuildURL(parameters);
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
            string actual;
            try
            {
                actual = target.BuildURL(parameters);
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
            string expected = target.BaseUrl + "search.atom?q=Contains+Space";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Query", "Contains Space" }
                };
            string actual = target.BuildURL(parameters);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// ensure query doesn't exceed 140 characters
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryTooLongTest()
        {
            SearchRequestProcessor<Search> target = new SearchRequestProcessor<Search>() { BaseUrl = "http://search.twitter.com/" };
            string expected = target.BaseUrl + "search.atom?q=Contains+Space";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SearchType.Search.ToString() },
                    { "Query", "x01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789" }
                };
            string actual = target.BuildURL(parameters);
        }
    }
}
