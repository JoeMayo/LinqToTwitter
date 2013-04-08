using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows direct message demos
    /// </summary>
    public class DirectMessageDemos
    {
        /// <summary>
        /// Run all direct message related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            DirectMessageSentByQueryDemo(twitterCtx);
            //DirectMessageSentToQueryDemo(twitterCtx);
            //DirectMessageShowDemo(twitterCtx);
            //NewDirectMessageDemo(twitterCtx);
            //DestroyDirectMessageDemo(twitterCtx);
        }

        /// <summary>
        /// shows how to delete a direct message
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DestroyDirectMessageDemo(TwitterContext twitterCtx)
        {
            var message = twitterCtx.DestroyDirectMessage("243563161037455360", true);

            if (message != null)
            {
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}",
                    message.RecipientScreenName,
                    message.Text);
            }
        }

        /// <summary>
        /// shows how to send a new direct message
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void NewDirectMessageDemo(TwitterContext twitterCtx)
        {
            var message = twitterCtx.NewDirectMessage("16761255", "Direct Message Test - " + DateTime.Now + "!'");

            if (message != null)
            {
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}, Date: {2}",
                    message.RecipientScreenName,
                    message.Text,
                    message.CreatedAt);
            }
        }

        private static void DirectMessageShowDemo(TwitterContext twitterCtx)
        {
            var directMsg =
                (from dm in twitterCtx.DirectMessage
                 where dm.Type == DirectMessageType.Show &&
                       dm.ID == 2078013265
                 select dm)
                .SingleOrDefault();

            Console.WriteLine(
                "From: {0}\nTo:  {1}\nMessage: {2}",
                directMsg.Sender.Name,
                directMsg.Recipient.Name,
                directMsg.Text);
        }

        /// <summary>
        /// shows how to query direct messages
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DirectMessageSentToQueryDemo(TwitterContext twitterCtx)
        {
            var directMessages =
                (from tweet in twitterCtx.DirectMessage
                 where tweet.Type == DirectMessageType.SentTo &&
                       tweet.Count == 2
                 select tweet)
                .ToList();

            directMessages.ForEach(
                dm => Console.WriteLine(
                    "Sender: {0}, Tweet: {1}",
                    dm.SenderScreenName,
                    dm.Text));
        }

        /// <summary>
        /// shows how to query direct messages
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DirectMessageSentByQueryDemo(TwitterContext twitterCtx)
        {
            var directMessages =
                (from dm in twitterCtx.DirectMessage
                 where dm.Type == DirectMessageType.SentBy
                 select dm)
                .ToList();

            directMessages.ForEach(
                dm => Console.WriteLine(
                    "Sender: {0}, ID: {1}, Tweet: {2}",
                    dm.SenderScreenName,
                    dm.ID,
                    dm.Text));
        }
    }
}
