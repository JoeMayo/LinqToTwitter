using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterTests
{
    [TestClass]
    public class LegalRequestProcessorTests
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_Returns_Parameters()
        {
            var reqProc = new LegalRequestProcessor<Legal>();
            Expression<Func<Legal, bool>> expression =
                strm =>
                    strm.Type == LegalType.Privacy;
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var parms = reqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(parms.Contains(
                new KeyValuePair<string, string>("Type", ((int)LegalType.Privacy).ToString())));
        }

        [TestMethod]
        public void BuildPrivacyUrl_Returns_Url()
        {
            var reqProc = new LegalRequestProcessor<Legal>() { BaseUrl = "http://api.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", LegalType.Privacy.ToString() }
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.AreEqual("http://api.twitter.com/1/privacy.xml", req.FullUrl);
        }

        [TestMethod]
        public void BuildTosUrl_Returns_Url()
        {
            var reqProc = new LegalRequestProcessor<Legal>() { BaseUrl = "http://api.twitter.com/1/" };
            var parms = new Dictionary<string, string>
            {
                { "Type", LegalType.TOS.ToString() }
            };

            Request req = reqProc.BuildUrl(parms);

            Assert.AreEqual("http://api.twitter.com/1/tos.xml", req.FullUrl);
        }

        [TestMethod]
        public void ProcessResults_Returns_A_Legal()
        {
            var reqProc = new LegalRequestProcessor<Legal>()
            {
                BaseUrl = "http://api.twitter.com/1/"
            };

            var legalList = reqProc.ProcessResults(
                "<privacy>Twitter Privacy Policy</privacy>");

            Assert.AreEqual(1, legalList.Count);
            Assert.AreEqual("Twitter Privacy Policy", legalList.First().Text);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new LegalRequestProcessor<Legal>()
            {
                BaseUrl = "http://api.twitter.com/1/"
            };

            var legal = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(1, legal.Count);
        }
    }
}
