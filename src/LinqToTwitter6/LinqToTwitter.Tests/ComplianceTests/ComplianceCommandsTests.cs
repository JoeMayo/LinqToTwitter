using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

        async Task<TwitterContext> InitializeTwitterContextAsync(string result)
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
                exec.PostFormUrlEncodedToTwitterAsync<ComplianceJob>(
                    HttpMethod.Post.ToString(),
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(tcsResponse.Task);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [TestMethod]
        public async Task CreateComplianceJobAsync_WithReply_ReturnsTrue()
        {
            const string JobName = "abc";
            const bool Resumable = true;
            var ctx = await InitializeTwitterContextAsync(JobResponse);

            ComplianceJob job = await ctx.CreateComplianceJobAsync(JobName, Resumable);

            Assert.IsNotNull(job);
            Assert.AreEqual("YIAh2p", job.JobID);
            Assert.AreEqual("https://storage.googleapis.com/twitter-compliance/test_tweet_ids", job.DownloadUrl);
            Assert.AreEqual(DateTime.Parse("2020-06-16T21:17:43.819+00:00"), job.DownloadExpiresAt);
            Assert.AreEqual("https://storage.googleapis.com/twitter-compliance/customer_test_object_123456_d8ske9.json", job.UploadUrl);
            Assert.AreEqual(DateTime.Parse("2020-06-16T21:17:43.818+00:00"), job.UploadExpiresAt);
        }

        const string JobResponse = @"{
  ""upload_url"" : ""https://storage.googleapis.com/twitter-compliance/customer_test_object_123456_d8ske9.json"",
  ""upload_expires_at"" : ""2020-06-16T21:17:43.818+00:00"",
  ""download_url"" : ""https://storage.googleapis.com/twitter-compliance/test_tweet_ids"",
  ""download_expires_at"" : ""2020-06-16T21:17:43.819+00:00"",
  ""job_id"" : ""YIAh2p""
}";

    }
}
