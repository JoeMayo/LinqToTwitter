using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter.Compliance;
using LinqToTwitter.OAuth;
using LinqToTwitter.Provider;
using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitter.Tests.ComplianceTests
{
    [TestClass]
    public class ComplianceCommandsTests
    {
        public ComplianceCommandsTests()
        {
            TestCulture.SetCulture();
        }

        static async Task<TwitterContext> InitializeTwitterContextAsync(string result)
        {
            await Task.Delay(1);
            var authMock = new Mock<IAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();

            var tcsAuth = new TaskCompletionSource<IAuthorizer>();
            tcsAuth.SetResult(authMock.Object);

            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(result);

            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.SendJsonToTwitterAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<ComplianceJobCreate>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task CreateComplianceJobAsync_WithReply_ReturnsTrue()
        {
            const string JobType = ComplianceJobType.Tweets;
            const string JobName = "abc";
            const bool Resumable = true;
            var ctx = await InitializeTwitterContextAsync(JobResponse);

            ComplianceQuerySingle query = await ctx.CreateComplianceJobAsync(JobType, JobName, Resumable);

            Assert.IsNotNull(query);
            ComplianceJob job = query.Job;
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

        const string JobResponse = @"{
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

    }
}
