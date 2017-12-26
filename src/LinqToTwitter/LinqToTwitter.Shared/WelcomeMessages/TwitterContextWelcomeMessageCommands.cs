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
        /// Sends a new welcome message to specified user.
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
                    msgParams,
                    cancelToken)
                    .ConfigureAwait(false);

            var reqProc = new WelcomeMessageRequestProcessor<WelcomeMessage>();
            WelcomeMessage msg = reqProc.ProcessActionResult(RawResult, WelcomeMessageType.Show);

            msg.Name = name;
            msg.Text = text;

            return msg;
        }
    }
}
