using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class StreamingRequestProcessorTests
    {
        public StreamingRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
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
            var lambdaExpression = expression as LambdaExpression;

            var parms = reqProc.GetParameters(lambdaExpression);

            Assert.True(parms.Contains(
                    new KeyValuePair<string, string>("Type", ((int)StreamingType.Sample).ToString())));
            Assert.True(parms.Contains(
                   new KeyValuePair<string, string>("Count", "10")));
            Assert.True(parms.Contains(
                  new KeyValuePair<string, string>("Delimited", "length")));
            Assert.True(parms.Contains(
                   new KeyValuePair<string, string>("Follow", "1,2,3")));
            Assert.True(parms.Contains(
                   new KeyValuePair<string, string>("Track", "twitter,API,LINQ to Twitter")));
            Assert.True(parms.Contains(
                  new KeyValuePair<string, string>("Locations", "-122.75,36.8,-121.75,37.8,-74,40,-73,41")));
        }

        [Fact]
        public void BuildFilterUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Filter.ToString() },
                { "Track", "LINQ to Twitter" }
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.Equal("http://stream.twitter.com/1/statuses/filter.json?track=LINQ%20to%20Twitter", req.FullUrl);
        }

        [Fact]
        public void BuildFilterUrl_Requires_FollowOrLocationsOrTrack()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Filter.ToString() },
                { "Count", "10" }
            };

            var ex = Assert.Throws<ArgumentException>(() => reqProc.BuildUrl(parms));

            Assert.Equal("FollowOrLocationsOrTrack", ex.ParamName);
        }

        [Fact]
        public void BuildFirehoseUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Firehose.ToString() },
                { "Count", "25" },
                { "Delimited", "length" }
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.Equal("http://stream.twitter.com/1/statuses/firehose.json?count=25&delimited=length", req.FullUrl);
        }

        [Fact]
        public void BuildLinksUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Links.ToString() },
                { "Count", "25" },
                { "Delimited", "length" }
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.Equal("http://stream.twitter.com/1/statuses/links.json?count=25&delimited=length", req.FullUrl);
        }

        [Fact]
        public void BuildRetweetUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Retweet.ToString() },
                { "Delimited", "length" }
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.Equal("http://stream.twitter.com/1/statuses/retweet.json?delimited=length", req.FullUrl);
        }

        [Fact]
        public void BuildSampleUrl_Returns_Url()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Sample.ToString() }
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.Equal("http://stream.twitter.com/1/statuses/sample.json", req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Requires_Type()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                //{ "Type", StreamingType.Sample.ToString() },
            };

            var ex = Assert.Throws<ArgumentException>(() => reqProc.BuildUrl(parms));

             Assert.Equal("Type", ex.ParamName);
        }

        [Fact]
        public void BuildSampleUrl_Forbids_Count()
        {
            var reqProc = new StreamingRequestProcessor<Streaming>() { BaseUrl = "http://stream.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", StreamingType.Sample.ToString() },
                { "Count", "5" }
            };

            var ex = Assert.Throws<ArgumentException>(() => reqProc.BuildUrl(parms));

            Assert.Equal("Count", ex.ParamName);
        }

        [Fact]
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

            Request req = reqProc.BuildUrl(parms);

            Assert.Equal("http://stream.twitter.com/1/statuses/sample.json?delimited=length", req.FullUrl);
        }

        [Fact]
        public void ProcessResults_Returns_A_Streaming()
        {
            var execMock = new Mock<ITwitterExecute>();
            var reqProc = new StreamingRequestProcessor<Streaming>() 
            { 
                BaseUrl = "http://stream.twitter.com/1/",
                TwitterExecutor = execMock.Object
            };

            var streamList = reqProc.ProcessResults(string.Empty);

            Assert.Equal(1, streamList.Count);
            Assert.Equal(execMock.Object, streamList.First().TwitterExecutor);
        }
    }
}
