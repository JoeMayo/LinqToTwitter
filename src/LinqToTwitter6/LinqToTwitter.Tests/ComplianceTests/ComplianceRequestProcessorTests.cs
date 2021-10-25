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
            Expression<Func<ComplianceQuery, bool>> expression =
                job =>
                    job.Type == ComplianceType.MultipleJobs &&
                    job.ID == "123" &&
                    job.JobType == ComplianceJobType.Tweets &&
                    job.Status == ComplianceStatus.Created;

            var lambdaExpression = expression as LambdaExpression;

            Dictionary<string, string> queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.Type), ((int)ComplianceType.MultipleJobs).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.ID), "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.JobType), ComplianceJobType.Tweets)));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>(nameof(ComplianceQuery.Status), ComplianceStatus.Created)));
        }

        [TestMethod]
        public void BuildUrl_ForMultipleJobs_IncludesParameters()
        {
            const string ExpectedUrl =
                BaseUrl2 + "compliance/jobs?" +
                "type=tweets&" +
                "status=in_progress";
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ComplianceQuery.Type), ComplianceType.MultipleJobs.ToString() },
                    { nameof(ComplianceQuery.JobType), ComplianceJobType.Tweets },
                    { nameof(ComplianceQuery.Status), ComplianceStatus.InProgress }
               };

            Request req = reqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_ForMultipleJobsWithMissingType_Throws()
        {
            var reqProc = new ComplianceRequestProcessor<ComplianceQuery> { BaseUrl = BaseUrl2 };
            var parameters =
                new Dictionary<string, string>
                {
                    { nameof(ComplianceQuery.Type), ComplianceType.MultipleJobs.ToString() },
                    //{ nameof(ComplianceQuery.JobType), ComplianceJobType.Tweets }
                };

            ArgumentException ex =
                L2TAssert.Throws<ArgumentException>(() =>
                    reqProc.BuildUrl(parameters));

            Assert.AreEqual(nameof(ComplianceQuery.JobType), ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_ForSingleJob_IncludesID()
        {
            const string ExpectedUrl = BaseUrl2 + "compliance/jobs/123";
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

            Assert.AreEqual(nameof(ComplianceQuery.ID), ex.ParamName);
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
            Assert.AreEqual("1452446437015314435", job.ID);
            Assert.AreEqual(DateTime.Parse("2021-11-01T01:26:30.000Z").ToUniversalTime(), job.DownloadExpiresAt);
            Assert.AreEqual(ComplianceStatus.Created, job.Status);
            Assert.AreEqual("https://storage.googleapis.com/up", job.UploadUrl);
            Assert.AreEqual("https://storage.googleapis.com/down", job.DownloadUrl);
            Assert.AreEqual(DateTime.Parse("2021-10-25T01:41:30.000Z").ToUniversalTime(), job.UploadExpiresAt);
            Assert.AreEqual("test-202110240626", job.Name);
            Assert.AreEqual(DateTime.Parse("2021-10-25T01:26:30.000Z").ToUniversalTime(), job.CreatedAt);
            Assert.AreEqual(ComplianceJobType.Tweets, job.JobType);
            Assert.AreEqual(true, job.Resumable);
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
            Assert.AreEqual(3, jobs.Count);
            ComplianceJob job = jobs.FirstOrDefault();
            Assert.AreEqual("1452493500373553153", job.ID);
            Assert.AreEqual(DateTime.Parse("2021-11-01T04:33:31.000Z").ToUniversalTime(), job.DownloadExpiresAt);
            Assert.AreEqual(ComplianceStatus.Created, job.Status);
            Assert.AreEqual("https://storage.googleapis.com/up", job.UploadUrl);
            Assert.AreEqual("https://storage.googleapis.com/down", job.DownloadUrl);
            Assert.AreEqual(DateTime.Parse("2021-10-25T04:48:31.000Z").ToUniversalTime(), job.UploadExpiresAt);
            Assert.AreEqual("test-202110240933", job.Name);
            Assert.AreEqual(DateTime.Parse("2021-10-25T04:33:31.000Z").ToUniversalTime(), job.CreatedAt);
            Assert.AreEqual(ComplianceJobType.Tweets, job.JobType);
            Assert.AreEqual(true, job.Resumable);
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
                JobType = ComplianceJobType.Tweets,
                Status = ComplianceStatus.Complete
            };

            var results = reqProc.ProcessResults(MultipleJobs);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            var complianceQuery = results.Single();
            Assert.IsNotNull(complianceQuery);
            Assert.AreEqual(ComplianceType.MultipleJobs, complianceQuery.Type);
            Assert.AreEqual(ComplianceJobType.Tweets, complianceQuery.JobType);
            Assert.AreEqual(ComplianceStatus.Complete, complianceQuery.Status);
        }

        const string SingleJob = @"{
	""data"": {
		""id"": ""1452446437015314435"",
		""download_expires_at"": ""2021-11-01T01:26:30.000Z"",
		""status"": ""created"",
		""upload_url"": ""https://storage.googleapis.com/up"",
		""download_url"": ""https://storage.googleapis.com/down"",
		""upload_expires_at"": ""2021-10-25T01:41:30.000Z"",
		""name"": ""test-202110240626"",
		""created_at"": ""2021-10-25T01:26:30.000Z"",
		""type"": ""tweets"",
		""resumable"": true
	}
}";

        const string MultipleJobs = @"{
	""data"": [
		{
			""download_url"": ""https://storage.googleapis.com/down"",
			""id"": ""1452493500373553153"",
			""download_expires_at"": ""2021-11-01T04:33:31.000Z"",
			""name"": ""test-202110240933"",
			""status"": ""created"",
			""type"": ""tweets"",
			""upload_url"": ""https://storage.googleapis.com/up"",
			""resumable"": true,
			""upload_expires_at"": ""2021-10-25T04:48:31.000Z"",
			""created_at"": ""2021-10-25T04:33:31.000Z""
		},
		{
			""download_url"": ""https://storage.googleapis.com/twttr-tweet-compliance/1452446437015314435/delivery/1021848369956700166_1452446437015314435?X-Goog-Algorithm=GOOG4-RSA-SHA256&X-Goog-Credential=complianceapi-public-svc-acct%40twttr-compliance-public-prod.iam.gserviceaccount.com%2F20211025%2Fauto%2Fstorage%2Fgoog4_request&X-Goog-Date=20211025T012630Z&X-Goog-Expires=604800&X-Goog-SignedHeaders=host&X-Goog-Signature=15c34a51d8778d867be1d5c59b5e4a4f38e0850d84febd5c8836d44851c08348a497ffb0b6cc9d6fb4e3f5fe50dbd7782fc3b6cfd926b9c433846fc93a3a9cd204055519578a87432c6d29cde179c251d22daa40a04edc5b1c12bf2d517dbb667461b902ac4aae1153b490e8694ab9d24bf4808ed2f8970bde12589a6ea4cdc8e22998af960fd10f1fb9274a51cc2b51543e9bb299191a1b043d777b74814b07921d2c0c503b1fa3e7b997ded194d34ac088d9d3dd952394d27849f35ee2f2f095d10fb79e8b4ba29c60a38e8cb70d45dd19798d03dc36d3eb083178d69da73c72d952505102bb2366c2ebed824f73380b56fc7ef2ab2cc5013cec950dd6b3a9"",
			""id"": ""1452446437015314435"",
			""download_expires_at"": ""2021-11-01T01:26:30.000Z"",
			""name"": ""test-202110240626"",
			""status"": ""expired"",
			""type"": ""tweets"",
			""upload_url"": ""https://storage.googleapis.com/twttr-tweet-compliance/1452446437015314435/submission/1021848369956700166_1452446437015314435?X-Goog-Algorithm=GOOG4-RSA-SHA256&X-Goog-Credential=complianceapi-public-svc-acct%40twttr-compliance-public-prod.iam.gserviceaccount.com%2F20211025%2Fauto%2Fstorage%2Fgoog4_request&X-Goog-Date=20211025T012630Z&X-Goog-Expires=900&X-Goog-SignedHeaders=content-length%3Bcontent-type%3Bhost%3Bx-goog-resumable&X-Goog-Signature=0e2e456a6a530a69fa31c9e5c65bbeff8f6b087635bf3ce5680a0a309281efd7e58067df61487e41a270d52e63f1dae6a7d67894965a92e8a062614abd6fc9e682860b814b13f761211aceac5334d4de6c382b7f39d29b50f8d68c11aef9b25641867f8f2b9cef68ea6ae1af724ca690cc22a8d97cb7e893a8eadeec3a97188c17fda4d00477460dfb31dd89dc8272fd1488c6c5fc621538d6fcc6d257cfb655592d28e4ec3c5bfd9e3d1ee8f99541a1f85ba9719e44418432c7c356c6785f17b1b521744666ed7b904f9b7d36fcce74621fc554c4ab1cad859c40caa727b70a2282a519e93a02e9bce769b76d20e0bc21ee6edd99d73f6442c5146dc7f6205f"",
			""resumable"": true,
			""upload_expires_at"": ""2021-10-25T01:41:30.000Z"",
			""created_at"": ""2021-10-25T01:26:30.000Z""
		},
		{
			""download_url"": ""https://storage.googleapis.com/twttr-tweet-compliance/1452416010036727813/delivery/1021848369956700166_1452416010036727813?X-Goog-Algorithm=GOOG4-RSA-SHA256&X-Goog-Credential=complianceapi-public-svc-acct%40twttr-compliance-public-prod.iam.gserviceaccount.com%2F20211024%2Fauto%2Fstorage%2Fgoog4_request&X-Goog-Date=20211024T232536Z&X-Goog-Expires=604800&X-Goog-SignedHeaders=host&X-Goog-Signature=30e8eba838c366aa6315a61e83ff029fc26d472787e6f2aa826df06678bdb915408d195da0035f90b0ffd7484d6d4854479dd8c14b785f795f5127d88ca852cff72023ff29f503f021bd40801175eac3a6eb9d5a065f6b54f30c3b7548a0e67fdd68b014b4856f710b7e5c7514f589f60db3d0d74e324fd8783a4d894756c0799789da78c16bb80dca71025ac586d02da5f680982726fee362ac2f88beff2deb119d4705ffcb434e359cd95cbad3771c60cde29b1e912f0a34534010fc0b18e14d2d928219cae13371b3d5b7e29df9bae44acd8742fc9fc83b4f13773430f847cbc2ea71e8df5ad18cccafa5c1ab1bf846e229af0bd10d7c949e095018633b77"",
			""id"": ""1452416010036727813"",
			""download_expires_at"": ""2021-10-31T23:25:36.000Z"",
			""name"": ""test-202110240425"",
			""status"": ""expired"",
			""type"": ""tweets"",
			""upload_url"": ""https://storage.googleapis.com/twttr-tweet-compliance/1452416010036727813/submission/1021848369956700166_1452416010036727813?X-Goog-Algorithm=GOOG4-RSA-SHA256&X-Goog-Credential=complianceapi-public-svc-acct%40twttr-compliance-public-prod.iam.gserviceaccount.com%2F20211024%2Fauto%2Fstorage%2Fgoog4_request&X-Goog-Date=20211024T232536Z&X-Goog-Expires=900&X-Goog-SignedHeaders=content-length%3Bcontent-type%3Bhost%3Bx-goog-resumable&X-Goog-Signature=6a3a1a71e67579f97748ba3e81b1ca75570514a8e6c866a4778c0d938fe40d4379fdbb7ab1aeed8309c352389c2fe2a1e2e238c5990b8c128da7db294324908ce9ae6036206f642319769cbd8507b1def8107b43c0c77bc5d2cc68782c0ff2bfc5bc47b31074c372c4f016745c828d5feca9949135be9bcec18e361d7055eec9a06f83c373c728ddcadbe33b5059b2e36fe0fc3f3e0ff9b5cea10e00957eeb787451d4759d10ac5b40920cd151295065355d04e9f2cb710469a141b273dd954c11c2568ace1c5b9e025744724c4216c9959730cc04062e24fd69c574df794edaafcaa429466e8c4a2ec7c31980c138a01012299f170ab88de8f4826efa74be35"",
			""resumable"": true,
			""upload_expires_at"": ""2021-10-24T23:40:36.000Z"",
			""created_at"": ""2021-10-24T23:25:36.000Z""
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
