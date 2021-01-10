using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.StartTime), "08/01/2020 00:00:00")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.Status), "in_progress,expired")));
        }

    }
}
