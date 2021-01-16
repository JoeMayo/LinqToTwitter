using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitter.Tests.ComplianceTests
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

        [TestMethod]
        public void BuildUrl_ForSingleJobWithoutID_Throws()
        {
            const string ExpectedUrl = BaseUrl2 + "tweets/compliance/jobs/123";
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ComplianceQuery.Type), ComplianceType.SingleJob.ToString() },
                    //{ nameof(ComplianceQuery.ID), "123" }
               };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
                    reqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(TweetQuery.ID), ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_WithNullParameters_Throws()
        {
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery> { BaseUrl = BaseUrl2 };

            L2TAssert.Throws<NullReferenceException>(() =>
            {
                reqProc.BuildUrl(null);
            });
        }

        [TestMethod]
        public void BuildUrl_WithSpacesInFields_FixesSpaces()
        {
            const string ExpectedUrl =
                BaseUrl2 + "tweets/compliance/jobs?" +
                "end_time=2021-01-01T12%3A59%3A59Z&" +
                "start_time=2020-12-31T00%3A00%3A01Z&" +
                "status=in_progress%2Cexpired";
            const string StatusWithSpaces = "in_progress, expired";
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ComplianceQuery.Type), ComplianceType.MultipleJobs.ToString() },
                    { nameof(ComplianceQuery.EndTime), new DateTime(2021, 1, 1, 12, 59, 59).ToString() },
                    { nameof(ComplianceQuery.StartTime), new DateTime(2020, 12, 31, 0, 0, 1).ToString() },
                    { nameof(ComplianceQuery.Status), StatusWithSpaces }
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void ProcessResults_Populates_SingleJob()
        {
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery> 
            { 
                BaseUrl = BaseUrl2,
                Type = ComplianceType.SingleJob
            };

            List<ComplianceQuery> results = reqProc.ProcessResults(SingleJob);

            Assert.IsNotNull(results);
            ComplianceQuery complianceQuery = results.SingleOrDefault();
            Assert.IsNotNull(complianceQuery);
            List<ComplianceJob> jobs = complianceQuery.Jobs;
            Assert.IsNotNull(jobs);
            Assert.AreEqual(1, jobs.Count);
            ComplianceJob job = jobs.FirstOrDefault();
            Assert.IsNotNull(job);
            Assert.AreEqual("jU8rFK", job.JobID);
            Assert.AreEqual("Troglomyces twitteri", job.JobName);
            Assert.AreEqual("https://storage.googleapis.com/twitter-compliance/test_user_ids", job.DownloadUrl);
            Assert.AreEqual(DateTime.Parse("2020-09-04T20:04:41.819+00:00"), job.DownloadExpiresAt);
            Assert.AreEqual(ComplianceStatus.Complete, job.Status);
        }

        [TestMethod]
        public void ProcessResults_Populates_MultipleJobs()
        {
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery>
            {
                BaseUrl = BaseUrl2,
                Type = ComplianceType.MultipleJobs
            };

            List<ComplianceQuery> results = reqProc.ProcessResults(MultipleJobs);

            Assert.IsNotNull(results);
            ComplianceQuery complianceQuery = results.SingleOrDefault();
            Assert.IsNotNull(complianceQuery);
            List<ComplianceJob> jobs = complianceQuery.Jobs;
            Assert.IsNotNull(jobs);
            Assert.AreEqual(2, jobs.Count);
            ComplianceJob job = jobs.FirstOrDefault();
            Assert.IsNotNull(job);
            Assert.AreEqual("NIXh2p", job.JobID);
            Assert.AreEqual("Feline species research", job.JobName);
            Assert.AreEqual("https://storage.googleapis.com/twitter-compliance/test_user_ids", job.DownloadUrl);
            Assert.AreEqual(DateTime.Parse("2020-06-16T11:17:32.819+00:00"), job.DownloadExpiresAt);
            Assert.AreEqual(ComplianceStatus.Complete, job.Status);
        }

        [TestMethod]
        public void ProcessResults_WithNoResults_ReturnsNoJobs()
        {
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery> { BaseUrl = BaseUrl2 };

            List<ComplianceQuery> results = reqProc.ProcessResults(ErrorTweet);

            Assert.IsNotNull(results);
            ComplianceQuery complianceQuery = results.SingleOrDefault();
            Assert.IsNotNull(complianceQuery);
            List<ComplianceJob> jobs = complianceQuery.Jobs;
            Assert.IsNull(jobs);
        }

        [TestMethod]
        public void ProcessResults_ForSingleJob_PopulatesInputParameters()
        {
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery>()
            {
                BaseUrl = BaseUrl2,
                Type = ComplianceType.SingleJob,
                ID = "123"
            };

            var results = reqProc.ProcessResults(SingleJob);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            var complianceQuery = results.Single();
            Assert.IsNotNull(complianceQuery);
            Assert.AreEqual(ComplianceType.SingleJob, complianceQuery.Type);
            Assert.AreEqual("123", complianceQuery.ID);
        }

        [TestMethod]
        public void ProcessResults_ForMultipleJobs_PopulatesInputParameters()
        {
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery>()
            {
                BaseUrl = BaseUrl2,
                Type = ComplianceType.MultipleJobs,
                EndTime = new DateTime(2020, 12, 31),
                StartTime = new DateTime(2020, 1, 1),
                Status = ComplianceStatus.Complete
            };

            var results = reqProc.ProcessResults(SingleJob);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            var complianceQuery = results.Single();
            Assert.IsNotNull(complianceQuery);
            Assert.AreEqual(ComplianceType.MultipleJobs, complianceQuery.Type);
            Assert.AreEqual(new DateTime(2020, 12, 31), complianceQuery.EndTime);
            Assert.AreEqual(new DateTime(2020, 1, 1), complianceQuery.StartTime);
            Assert.AreEqual(ComplianceStatus.Complete, complianceQuery.Status);
        }

        const string SingleJob = @"{
  ""job_id"": ""jU8rFK"",
  ""job_name"": ""Troglomyces twitteri"",
  ""download_url"": ""https://storage.googleapis.com/twitter-compliance/test_user_ids"",
  ""download_expires_at"": ""2020-09-04T20:04:41.819+00:00"",
  ""status"": ""complete""
}";

        const string MultipleJobs = @"{
  ""jobs"": [
    {
      ""job_id"": ""NIXh2p"",
      ""job_name"": ""Feline species research"",
      ""created_at"": ""2020-09-03T21:17:43.819+00:00"",
      ""download_url"": ""https://storage.googleapis.com/twitter-compliance/test_user_ids"",
      ""download_expires_at"": ""2020-06-16T11:17:32.819+00:00"",
      ""status"": ""complete""
    },
    {
      ""job_id"": ""jU8rFK"",
      ""job_name"": ""Troglomyces twitteri"",
      ""download_url"": ""https://storage.googleapis.com/twitter-compliance/test_user_ids"",
      ""download_expires_at"": ""2020-09-04T20:04:41.819+00:00"",
      ""status"": ""complete""
    }
  ]
}";

        const string ErrorTweet = @"{
	""errors"": [
		{
			""detail"": ""Could not find tweet with ids: [1]."",
			""title"": ""Not Found Error"",
			""resource_type"": ""tweet"",
			""parameter"": ""ids"",
			""value"": ""1"",
			""type"": ""https://api.twitter.com/2/problems/resource-not-found""
		}
	]
}";
    }
}
