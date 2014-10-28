using System;
using System.Collections.Generic;

namespace LinqToTwitter
{
    public static class ReportSpamExtensions
    {

        /// <summary>
        /// lets logged-in user report spam
        /// </summary>
        /// <param name="userID">user id of alleged spammer</param>
        /// <param name="screenName">screen name of alleged spammer</param>
        /// <returns>Alleged spammer user info</returns>
        public static User ReportSpam(this TwitterContext ctx, string userID, string screenName)
        {
            return ReportSpam(ctx, userID, screenName, null);
        }

        /// <summary>
        /// lets logged-in user report spam
        /// </summary>
        /// <param name="userID">user id of alleged spammer</param>
        /// <param name="screenName">screen name of alleged spammer</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>Alleged spammer user info</returns>
        public static User ReportSpam(this TwitterContext ctx, string userID, string screenName, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either userID or screenName is a required parameter.");
            }

            string reportSpamUrl = ctx.BaseUrl + "users/report_spam.json";

            var createParams = new Dictionary<string, string>
                {
                    { "user_id", userID },
                    { "screen_name", screenName }
                };

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    reportSpamUrl,
                    createParams,
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }

    }
}
