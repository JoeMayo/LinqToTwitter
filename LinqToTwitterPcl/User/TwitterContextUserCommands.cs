using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Lets logged-in user report spam.
        /// </summary>
        /// <param name="userID">User id of alleged spammer.</param>
        /// <returns>Alleged spammer user info.</returns>
        public async Task<User> ReportSpamAsync(ulong userID)
        {
            if (userID == 0)
                throw new ArgumentException("Twitter doesn't have a user with ID == 0", "userID");

            var reportParams = new Dictionary<string, string>
            {
                { "user_id", userID.ToString() }
            };

            return await ReportSpamAsync(reportParams).ConfigureAwait(false);
        }


        /// <summary>
        /// Lets logged-in user report spam.
        /// </summary>
        /// <param name="screenName">Screen name of alleged spammer.</param>
        /// <returns>Alleged spammer user info.</returns>
        public async Task<User> ReportSpamAsync(string screenName)
        {
            if (string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("Please supply a valid screen name", "screenName");

            var reportParams = new Dictionary<string, string>
            {
                { "screen_name", screenName }
            };

            return await ReportSpamAsync(reportParams).ConfigureAwait(false);
        }

        internal async Task<User> ReportSpamAsync(IDictionary<string, string> reportParams)
        {
            string reportSpamUrl = BaseUrl + "users/report_spam.json";

            string resultsJson =
                await TwitterExecutor
                    .PostToTwitterAsync<User>(reportSpamUrl, reportParams)
                    .ConfigureAwait(false);

            return new UserRequestProcessor<User>()
                .ProcessActionResult(resultsJson, StatusAction.SingleStatus);
        }
    }
}
