using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToTwitter;
using System.Linq.Expressions;
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
            LambdaExpression lambdaExpression = expression as LambdaExpression;

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

            Request req = reqProc.BuildURL(parms);

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
                { "Track", "LINQ to Twitter" },
                { "With", "Follow" },
                { "AllReplies", "True" }
            };

            Request req = reqProc.BuildURL(parms);

            Assert.AreEqual("http://betastream.twitter.com/2b/site.json?delimited=length&follow=1%2C2%2C3&track=LINQ%20to%20Twitter&with=follow&replies=all", req.FullUrl);
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
                reqProc.BuildURL(parms);

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Follow", ane.ParamName);
            }
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
