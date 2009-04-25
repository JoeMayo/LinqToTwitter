using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Linq;
using System;

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
            SearchRequestProcessor target = new SearchRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_testQueryResponse);
            IQueryable actual = target.ProcessResults(twitterResponse);
            var result = actual.Cast<TwitterSearch>().First();
            Assert.AreEqual(2, result.SearchResults.Entries.Count);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessEmptyResultsTest()
        {
            SearchRequestProcessor target = new SearchRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_emptyResponse);
            IQueryable actual = target.ProcessResults(twitterResponse);
            var result = actual.Cast<TwitterSearch>().First();
            Assert.AreEqual(0, result.SearchResults.Entries.Count);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            SearchRequestProcessor target = new SearchRequestProcessor(); // TODO: Initialize to an appropriate value
            Expression<Func<TwitterSearch, bool>> expression =
                search =>
                    search.Type == SearchType.Search &&
                    search.GeoCode == "40.757929,-73.985506,25km" &&
                    search.Language == "en" &&
                    search.Page == 1 &&
                    search.PageSize == 10 &&
                    search.Query == "LINQ to Twitter" &&
                    search.ShowUser == "true" &&
                    search.SinceID == 123;
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
                    new KeyValuePair<string, string>("Language", "en")));
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
                    new KeyValuePair<string, string>("ShowUser", "true")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SinceID", "123")));
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildURLTest()
        {
            SearchRequestProcessor target = new SearchRequestProcessor() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "GeoCode", "40.757929,-73.985506,25km" },
                    { "Language", "en" },
                    { "Page", "1" },
                    { "PageSize", "10" },
                    { "Query", "LINQ to Twitter" },
                    { "ShowUser", "true" },
                    { "SinceID", "1" }
                };
            string expected = "http://search.twitter.com/search.xml?geocode=40.757929%2c-73.985506%2c25km&lang=en&page=1&rpp=10&q=LINQ+to+Twitter&show_user=true&since_id=1:LINQ+to+Twitter";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }
    }
}
