using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Adds one or more users to a Site Stream.
        /// </summary>
        /// <param name="userIDs">List of user IDs to add to Site Stream</param>
        /// <param name="streamID">ID of Site Stream to add users to</param>
        /// <returns>Control Stream with CommandResponse property for Twitter's response message</returns>
        public async Task<ControlStream> AddSiteStreamUserAsync(List<ulong> userIDs, string streamID)
        {
            if (string.IsNullOrWhiteSpace(streamID)) throw new ArgumentNullException("streamID", "streamID is required.");

            var newUrl = SiteStreamUrl + "site/c/" + streamID + "/add_user.json";

            string userIDString = string.Join(",", userIDs.Select(user => user.ToString()).ToArray());

            var reqProc = new ControlStreamRequestProcessor<ControlStream>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<ControlStream>(
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userIDString}
                    });

            return reqProc.ProcessActionResult(resultsJson, ControlStreamType.Info);
        }

        /// <summary>
        /// Removes one or more users from a Site Stream
        /// </summary>
        /// <param name="userIDs">List of user IDs to remove from Site Stream</param>
        /// <param name="streamID">ID of Site Stream to remove users from</param>
        /// <returns>Control Stream with CommandResponse property for Twitter's response message</returns>
        public async Task<ControlStream> RemoveSiteStreamUserAsync(List<ulong> userIDs, string streamID)
        {
            if (string.IsNullOrWhiteSpace(streamID)) throw new ArgumentNullException("streamID", "streamID is required.");

            var newUrl = SiteStreamUrl + "site/c/" + streamID + "/remove_user.json";

            string userIDString = string.Join(",", userIDs.Select(user => user.ToString()).ToArray());

            var reqProc = new ControlStreamRequestProcessor<ControlStream>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<ControlStream>(
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userIDString}
                    });

            return reqProc.ProcessActionResult(resultsJson, ControlStreamType.Info);
        }
    }
}
