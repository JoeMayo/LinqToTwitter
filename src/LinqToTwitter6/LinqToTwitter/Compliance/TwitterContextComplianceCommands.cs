using LinqToTwitter.Compliance;
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
        /// <param name="jobType">Type of job - e.g. tweets or users</param>
        /// <param name="jobName">Name of job</param>
        /// <param name="resumable">Allows resuming uploads</param>
        /// <param name="cancelToken">Optional cancellation token</param>
        /// <returns>New <see cref="ComplianceQuery"/> details</returns>
        public async Task<ComplianceQuerySingle?> CreateComplianceJobAsync(string jobType, string jobName, bool resumable, CancellationToken cancelToken = default)
        {
            string url = $"{BaseUrl2}compliance/jobs";

            var postData = new Dictionary<string, string>();

            var postObj = new ComplianceJobCreate
            {
                Type = jobType,
                Name = jobName,
                Resumable = resumable,
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ComplianceQuerySingle? job = JsonSerializer.Deserialize<ComplianceQuerySingle>(RawResult);

            return job;
        }
    }
}
