using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitter.Tests.SearchTests
{
    [TestClass]
    public class ComplianceRequestProcessorTests
    {
        const string BaseUrl2 = "https://api.twitter.com/2/";

        public ComplianceRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParametersTest()
        {
            var target = new ComplianceRequestProcessor<ComplianceQuery>();

            var endTime = new DateTime(2020, 8, 30);
            var startTime = new DateTime(2020, 8, 1);
            string status = string.Join(',', new[] { ComplianceStatus.InProgress, ComplianceStatus.Expired });
            Expression<Func<ComplianceQuery, bool>> expression =
                search =>
                    search.Type == ComplianceType.MultipleJobs &&
                    search.EndTime == endTime &&
                    search.ID == "123" &&
                    search.StartTime == startTime &&
                    search.Status == status;

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.Type), ((int)ComplianceType.MultipleJobs).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.EndTime), "08/30/2020 00:00:00")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.ID), "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.StartTime), "08/01/2020 00:00:00")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.Status), "in_progress,expired")));
        }

        [TestMethod]
        public void BuildUrl_ForMultipleJobs_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "tweets/compliance/jobs?" +
                "end_time=2021-01-01T12%3A59%3A59Z&" +
                "start_time=2020-12-31T00%3A00%3A01Z&" +
                "status=in_progress%2Cexpired";
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ComplianceQuery.Type), ComplianceType.MultipleJobs.ToString() },
                    { nameof(ComplianceQuery.EndTime), new DateTime(2021, 1, 1, 12, 59, 59).ToString() },
                    { nameof(ComplianceQuery.StartTime), new DateTime(2020, 12, 31, 0, 0, 1).ToString() },
                    { nameof(ComplianceQuery.Status), string.Join(',', new[] { ComplianceStatus.InProgress, ComplianceStatus.Expired }) }
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForSingleJob_IncludesID()
        {
            const string ExpectedUrl = BaseUrl2 + "tweets/compliance/jobs/123";
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ComplianceQuery.Type), ComplianceType.SingleJob.ToString() },
                    { nameof(ComplianceQuery.ID), "123" }
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

    }
}
