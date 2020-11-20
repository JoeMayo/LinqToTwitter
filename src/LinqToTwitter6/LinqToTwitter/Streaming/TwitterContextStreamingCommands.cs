using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Add rules to the filter stream
        /// </summary>
        /// <param name="rules">List of tag/value rules to add</param>
        /// <param name="isValidateOnly">"true" says don't apply rule - just validate to see if it's formatted correctly (dry run)</param>
        /// <param name="cancelToken">Allows you to cancel async operation</param>
        /// <returns></returns>
        public virtual async Task<Streaming?> AddStreamingFilterRulesAsync(List<StreamingAddRule> rules, bool isValidateOnly = false, CancellationToken cancelToken = default)
        {
            _ = rules ?? throw new ArgumentNullException(nameof(rules), $"{nameof(rules)} is required!");

            var addRules = new AddStreamingFilterRules 
            { 
                Add = rules 
            };
            return await AddOrValidateStreamingFilterRulesAsync(addRules, isValidateOnly, cancelToken);
        }

        /// <summary>
        /// Delete rules from the filter stream
        /// </summary>
        /// <param name="ruleIds">List of ids of rules to delete</param>
        /// <param name="isValidateOnly">"true" says don't apply rule - just validate to see if it's formatted correctly (dry run)</param>
        /// <param name="cancelToken">Allows you to cancel async operation</param>
        /// <returns></returns>
        public virtual async Task<Streaming?> DeleteStreamingFilterRulesAsync(List<string> ruleIds, bool isValidateOnly = false, CancellationToken cancelToken = default)
        {
            _ = ruleIds ?? throw new ArgumentNullException(nameof(ruleIds), $"{nameof(ruleIds)} is required!");

            var deleteRules = new DeleteStreamingFilterRules 
            { 
                Delete = new DeleteIds
                {
                    Ids = ruleIds
                }
            };
            return await AddOrValidateStreamingFilterRulesAsync(deleteRules, isValidateOnly, cancelToken);
        }

        async Task<Streaming?> AddOrValidateStreamingFilterRulesAsync<T>(T rules, bool isValidateOnly, CancellationToken cancelToken)
        {
            string rulesUrl = BaseUrl2 + "tweets/search/stream/rules";

            var postData = new Dictionary<string, string>();

            if (isValidateOnly)
            {
                postData["dry_run"] = true.ToString();
                rulesUrl += "?dry_run=true";
            }

            var reqProc = new StreamingRequestProcessor<Streaming>();

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    rulesUrl,
                    postData,
                    rules,
                    cancelToken)
                   .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, StreamingType.Filter);
        }
    }
}
