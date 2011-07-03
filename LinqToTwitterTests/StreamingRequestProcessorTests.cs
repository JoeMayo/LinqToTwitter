using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterTests
{
    /// <summary>
    /// Summary description for StreamingTests
    /// </summary>
    [TestClass]
    public class StreamingRequestProcessorTests
    {
        public StreamingRequestProcessorTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void GetParameters_Returns_Parameters()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>();
            Expression<Func<Streaming, bool>> expression =
                strm =>
                    strm.Type == StreamingType.Sample &&
                    strm.Count == 10 &&
                    strm.Delimited == "length" &&
                    strm.Follow == "1,2,3" &&
                    strm.Track == "twitter,API,LINQ to Twitter" &&
                    strm.Locations == "-122.75,36.8,-121.75,37.8,-74,40,-73,41";
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var parms = reqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(parms.Contains(
                    new KeyValuePair<string, string>("Type", ((int)StreamingType.Sample).ToString())));
            Assert.IsTrue(parms.Contains(
                   new KeyValuePair<string, string>("Count", "10")));
            Assert.IsTrue(parms.Contains(
                  new KeyValuePair<string, string>("Delimited", "length")));
            Assert.IsTrue(parms.Contains(
                   new KeyValuePair<string, string>("Follow", "1,2,3")));
            Assert.IsTrue(parms.Contains(
                   new KeyValuePair<string, string>("Track", "twitter,API,LINQ to Twitter")));
            Assert.IsTrue(parms.Contains(
                  new KeyValuePair<string, string>("Locations", "-122.75,36.8,-121.75,37.8,-74,40,-73,41")));
        }

        [TestMethod]
        public void BuildFilterUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Filter.ToString() },
                { "Track", "LINQ to Twitter" }
            };

            Request req = reqProc.BuildURL(parms);

            Assert.AreEqual("http://stream.twitter.com/1/statuses/filter.json?track=LINQ%20to%20Twitter", req.FullUrl);
        }

        [TestMethod]
        public void BuildFilterUrl_Requires_FollowOrLocationsOrTrack()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Filter.ToString() },
                { "Count", "10" }
            };

            try
            {
                reqProc.BuildURL(parms);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException aex)
            {
                Assert.AreEqual("FollowOrLocationsOrTrack", aex.ParamName);
            }
        }

        [TestMethod]
        public void BuildFirehoseUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Firehose.ToString() },
                { "Count", "25" },
                { "Delimited", "length" }
            };

            Request req = reqProc.BuildURL(parms);

            Assert.AreEqual("http://stream.twitter.com/1/statuses/firehose.json?count=25&delimited=length", req.FullUrl);
        }

        [TestMethod]
        public void BuildLinksUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Links.ToString() },
                { "Count", "25" },
                { "Delimited", "length" }
            };

            Request req = reqProc.BuildURL(parms);

            Assert.AreEqual("http://stream.twitter.com/1/statuses/links.json?count=25&delimited=length", req.FullUrl);
        }

        [TestMethod]
        public void BuildRetweetUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Retweet.ToString() },
                { "Delimited", "length" }
            };

            Request req = reqProc.BuildURL(parms);

            Assert.AreEqual("http://stream.twitter.com/1/statuses/retweet.json?delimited=length", req.FullUrl);
        }

        [TestMethod]
        public void BuildSampleUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Sample.ToString() }
            };

            Request req = reqProc.BuildURL(parms);

            Assert.AreEqual("http://stream.twitter.com/1/statuses/sample.json", req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Requires_Type()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                //{ "Type", StreamingType.Sample.ToString() },
            };

            try
            {
                reqProc.BuildURL(parms);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Type", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildSampleUrl_Forbids_Count()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Sample.ToString() },
                { "Count", "5" }
            };

            try
            {
                reqProc.BuildURL(parms);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Count", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildSampleUrl_Only_Adds_Delimited_To_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Sample.ToString() },
                { "Delimited", "length" },
                { "Follow", "1,2,3" },
                { "Track", "twitter,LINQ to Twitter,Joe Mayo" },
                { "Locations", "123,456,789,012" }
            };

            Request req = reqProc.BuildURL(parms);

            Assert.AreEqual("http://stream.twitter.com/1/statuses/sample.json?delimited=length", req.FullUrl);
        }

        [TestMethod]
        public void ProcessResults_Returns_A_Streaming()
        {
            var execMock = new Mock<ITwitterExecute>();
            var reqProc = new StreamingRequestProcessor<Streaming>() 
            { 
                BaseUrl = "http://stream.twitter.com/1/",
                TwitterExecutor = execMock.Object
            };

            var streamList = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(1, streamList.Count);
            Assert.AreEqual(execMock.Object, streamList.First().TwitterExecutor);
        }
    }
}
