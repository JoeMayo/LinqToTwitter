using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Creates a new compliance job
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="resumable"></param>
        /// <param name="cancelToken">Optional cancellation token</param>
        /// <returns>New <see cref="ComplianceJob"/> details</returns>
        public async Task<ComplianceJob?> CreateComplianceJobAsync(string jobName, bool resumable, CancellationToken cancelToken = default)
        {
            string url = $"{BaseUrl2}tweets/compliance/jobs";

            var postData = new Dictionary<string, string?>
            {
                { "job_name", jobName },
                { "resumable", resumable ? "true" : "false" }
            };

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<ComplianceJob>(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    cancelToken)
                   .ConfigureAwait(false);

            ComplianceJob? job = JsonSerializer.Deserialize<ComplianceJob>(RawResult);

            return job;
        }
    }
}
