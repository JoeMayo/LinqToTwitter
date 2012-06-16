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
    public class UserStreamRequestProcessorTest
    {
        public UserStreamRequestProcessorTest()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void GetParameters_Returns_Parameters()
        {
            var reqProc = new UserStreamRequestProcessor<UserStream>();
            Expression<Func<UserStream, bool>> expression =
                strm =>
                    strm.Type == UserStreamType.User &&
                    strm.Delimited == "length" &&
                    strm.Follow == "1,2,3" &&
                    strm.Track == "twitter,API,LINQ to Twitter" &&
                    strm.With == "Follow" &&
                    strm.AllReplies == true;
            var lambdaExpression = expression as LambdaExpression;

            var parms = reqProc.GetParameters(lambdaExpression);

            Assert.True(parms.Contains(
                    new KeyValuePair<string, string>("Type", ((int)UserStreamType.User).ToString())));
            Assert.True(parms.Contains(
                  new KeyValuePair<string, string>("Delimited", "length")));
            Assert.True(parms.Contains(
                   new KeyValuePair<string, string>("Track", "twitter,API,LINQ to Twitter")));
            Assert.True(parms.Contains(
                  new KeyValuePair<string, string>("With", "Follow")));
            Assert.True(parms.Contains(
                  new KeyValuePair<string, string>("AllReplies", "True")));
            Assert.True(parms.Contains(
                  new KeyValuePair<string, string>("Follow", "1,2,3")));
        }

        [Fact]
        public void BuildUserUrl_Returns_Url()
        {
            var reqProc = new UserStreamRequestProcessor<UserStream>() { UserStreamUrl = "https://userstream.twitter.com/2/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", UserStreamType.User.ToString() },
                { "Delimited", "length" },
                { "Track", "LINQ to Twitter" },
                { "With", "Follow" },
                { "AllReplies", "True" }
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.Equal("https://userstream.twitter.com/2/user.json?delimited=length&track=LINQ%20to%20Twitter&with=follow&replies=all", req.FullUrl);
        }

        [Fact]
        public void BuildSiteUrl_Returns_Url()
        {
            var reqProc = new UserStreamRequestProcessor<UserStream>() { SiteStreamUrl = "http://betastream.twitter.com/2b/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", UserStreamType.Site.ToString() },
                { "Delimited", "length" },
                { "Follow", "1,2,3" },
                { "With", "Follow" },
                { "AllReplies", "True" }
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.Equal("http://betastream.twitter.com/2b/site.json?delimited=length&follow=1%2C2%2C3&with=follow", req.FullUrl);
        }

        [Fact]
        public void BuildSiteUrl_Throws_On_Track()
        {
            var reqProc = new UserStreamRequestProcessor<UserStream>() { SiteStreamUrl = "http://betastream.twitter.com/2b/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", UserStreamType.Site.ToString() },
                { "Delimited", "length" },
                { "Follow", "1,2,3" },
                { "Track", "LINQ to Twitter" },
                { "With", "Follow" },
                { "AllReplies", "True" }
            };

            var ex = Assert.Throws<ArgumentException>(() => reqProc.BuildUrl(parms));

            Assert.Equal("Track", ex.ParamName);
        }

        [Fact]
        public void BuildSiteUrl_Requires_Follow()
        {
            var reqProc = new UserStreamRequestProcessor<UserStream>() { SiteStreamUrl = "http://betastream.twitter.com/2b/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", UserStreamType.Site.ToString() },
                //{ "Follow", "1,2,3" },
            };

            var ex = Assert.Throws<ArgumentNullException>(() => reqProc.BuildUrl(parms));

            Assert.Equal("Follow", ex.ParamName);
        }

        [Fact]
        public void BuildSiteUrl_Removes_Spaces_From_Follow()
        {
            var reqProc = new UserStreamRequestProcessor<UserStream>() { SiteStreamUrl = "http://betastream.twitter.com/2b/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", UserStreamType.Site.ToString() },
                { "Follow", "1, 2, 3" },
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.Equal("http://betastream.twitter.com/2b/site.json?follow=1%2C2%2C3", req.FullUrl);
        }

        [Fact]
        public void ProcessResults_Returns_A_UserStream()
        {
            var execMock = new Mock<ITwitterExecute>();
            var reqProc = new UserStreamRequestProcessor<UserStream>()
            {
                UserStreamUrl = "https://userstream.twitter.com/2/",
                TwitterExecutor = execMock.Object
            };

            var streamList = reqProc.ProcessResults(string.Empty);

            Assert.Equal(1, streamList.Count);
            Assert.Equal(execMock.Object, streamList.First().TwitterExecutor);
        }
    }
}
