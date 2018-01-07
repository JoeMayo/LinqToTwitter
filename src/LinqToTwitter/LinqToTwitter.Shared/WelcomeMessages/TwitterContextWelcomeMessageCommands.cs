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
        /// Creates a new welcome message.
        /// </summary>
        /// <param name="name">Name of welcome message.</param>
        /// <param name="text">Welcome message contents.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<WelcomeMessage> NewWelcomeMessageAsync(string name, string text, CancellationToken cancelToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException($"{nameof(text)} is a required parameter.", nameof(text));

            var newUrl = BaseUrl + "direct_messages/welcome_messages/new.json";

            var msgParams = new WelcomeMessageValue
            {
                WelcomeMessage = new WelcomeMsg
                {
                    Name = name,
                    MessageData = new WelcomeMessageData
                    {
                        Text = text
                    }
                }
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    newUrl,
                    new Dictionary<string, string>(),
                    msgParams,
                    cancelToken)
                   .ConfigureAwait(false);

            var reqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>();
            WelcomeMessage msg = reqProc.ProcessActionResult(RawResult, WelcomeMessageType.Show);

            msg.Name = name;
            msg.Text = text;

            return msg;
        }

        /// <summary>
        /// Updates an existing welcome message.
        /// <param name="welcomeMessageID">ID of the welcome message.</param>
        /// <paramref name="name">Updated welcome message name.</paramref>
        /// <param name="text">Updated welcome message contents.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<WelcomeMessage> UpdateWelcomeMessageAsync(ulong welcomeMessageID, string name, string text, CancellationToken cancelToken = default(CancellationToken))
        {
            if (welcomeMessageID == 0)
                throw new ArgumentException($"{nameof(welcomeMessageID)} is a required parameter, but it's value is 0, which is invalid.", nameof(welcomeMessageID));

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException($"{nameof(text)} is a required parameter.", nameof(text));

            var newUrl = BaseUrl + "direct_messages/welcome_messages/update.json?id=" + welcomeMessageID;

            var postData = new Dictionary<string, string>
            {
                ["id"] = welcomeMessageID.ToString()
            };

            var msgParams = new WelcomeMsg
            {
                Name = name,
                MessageData = new WelcomeMessageData
                {
                    Text = text
                }
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Put.ToString(),
                    newUrl,
                    postData,
                    msgParams,
                    cancelToken)
                   .ConfigureAwait(false);

            var reqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>();
            WelcomeMessage msg = reqProc.ProcessActionResult(RawResult, WelcomeMessageType.Show);

            msg.WelcomeMessageID = welcomeMessageID;
            msg.Text = text;

            return msg;
        }

        /// <summary>
        /// Marks a welcome message as the default.
        /// </summary>
        /// <param name="welcomeMessageID">ID of the welcome message.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<WelcomeMessage> NewWelcomeMessageRuleAsync(ulong welcomeMessageID, CancellationToken cancelToken = default(CancellationToken))
        {
            if (welcomeMessageID == 0)
                throw new ArgumentException($"{nameof(welcomeMessageID)} is a required parameter, but it's value is 0, which is invalid.", nameof(welcomeMessageID));

            var newUrl = BaseUrl + "direct_messages/welcome_messages/rules/new.json";

            var msgParams = new WelcomeMessageValue
            {
                WelcomeMessageRule = new WelcomeMessageRule
                {
                    WelcomeMessageID = welcomeMessageID.ToString()
                }
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    newUrl,
                    new Dictionary<string, string>(),
                    msgParams,
                    cancelToken)
                   .ConfigureAwait(false);

            var reqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>();
            WelcomeMessage msg = reqProc.ProcessActionResult(RawResult, WelcomeMessageType.Show);

            msg.WelcomeMessageID = welcomeMessageID;

            return msg;
        }
    }
}
