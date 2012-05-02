using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterTests
{
    [TestClass]
    public class UserStreamRequestProcessorTest
    {
        [TestMethod]
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

            Assert.IsTrue(parms.Contains(
                    new KeyValuePair<string, string>("Type", ((int)UserStreamType.User).ToString())));
            Assert.IsTrue(parms.Contains(
                  new KeyValuePair<string, string>("Delimited", "length")));
            Assert.IsTrue(parms.Contains(
                   new KeyValuePair<string, string>("Track", "twitter,API,LINQ to Twitter")));
            Assert.IsTrue(parms.Contains(
                  new KeyValuePair<string, string>("With", "Follow")));
            Assert.IsTrue(parms.Contains(
                  new KeyValuePair<string, string>("AllReplies", "True")));
            Assert.IsTrue(parms.Contains(
                  new KeyValuePair<string, string>("Follow", "1,2,3")));
        }

        [TestMethod]
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

            Assert.AreEqual("https://userstream.twitter.com/2/user.json?delimited=length&track=LINQ%20to%20Twitter&with=follow&replies=all", req.FullUrl);
        }

        [TestMethod]
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

            Assert.AreEqual("http://betastream.twitter.com/2b/site.json?delimited=length&follow=1%2C2%2C3&with=follow", req.FullUrl);
        }

        [TestMethod]
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

            try
            {
                reqProc.BuildUrl(parms);

                Assert.Fail("ArgumentException Expected.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Track", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildSiteUrl_Requires_Follow()
        {
            var reqProc = new UserStreamRequestProcessor<UserStream>() { SiteStreamUrl = "http://betastream.twitter.com/2b/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", UserStreamType.Site.ToString() },
                //{ "Follow", "1,2,3" },
            };

            try
            {
                reqProc.BuildUrl(parms);

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Follow", ane.ParamName);
            }
        }

        [TestMethod]
        public void BuildSiteUrl_Removes_Spaces_From_Follow()
        {
            var reqProc = new UserStreamRequestProcessor<UserStream>() { SiteStreamUrl = "http://betastream.twitter.com/2b/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", UserStreamType.Site.ToString() },
                { "Follow", "1, 2, 3" },
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.AreEqual("http://betastream.twitter.com/2b/site.json?follow=1%2C2%2C3", req.FullUrl);
        }

        [TestMethod]
        public void ProcessResults_Returns_A_UserStream()
        {
            var execMock = new Mock<ITwitterExecute>();
            var reqProc = new UserStreamRequestProcessor<UserStream>()
            {
                UserStreamUrl = "https://userstream.twitter.com/2/",
                TwitterExecutor = execMock.Object
            };

            var streamList = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(1, streamList.Count);
            Assert.AreEqual(execMock.Object, streamList.First().TwitterExecutor);
        }
    }
}
