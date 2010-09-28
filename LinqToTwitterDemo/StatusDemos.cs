using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows status demos
    /// </summary>
    public class StatusDemos
    {
        /// <summary>
        /// Run all status related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //
            // status tweets
            //

            //PublicStatusQueryDemo(twitterCtx);
            //PublicStatusFilteredQueryDemo(twitterCtx);
            //HomeStatusQueryDemo(twitterCtx);
            //FriendStatusQueryDemo(twitterCtx);
            //UserStatusQueryDemo(twitterCtx);
            //UserStatusByNameQueryDemo(twitterCtx);
            //UserStatusWithRetweetsQueryDemo(twitterCtx);
            //MentionsStatusQueryDemo(twitterCtx);
            //MentionsWithSinceIDStatusQueryDemo(twitterCtx);
            //MentionsWithPagingQueryDemo(twitterCtx);
            //SingleStatusQueryDemo(twitterCtx);
            UpdateStatusDemo(twitterCtx);
            //UpdateStatusWithReplyDemo(twitterCtx);
            //UpdateStatusWithLocationDemo(twitterCtx);
            //UpdateStatusWithPlaceDemo(twitterCtx);
            //DestroyStatusDemo(twitterCtx);
            //RetweetedByMeStatusQueryDemo(twitterCtx);
            //RetweetedByMeWithCountStatusQueryDemo(twitterCtx);
            //RetweetedToMeStatusQueryDemo(twitterCtx);
            //RetweetsOfMeStatusQueryDemo(twitterCtx);
            //RetweetDemo(twitterCtx);
            //RetweetsQueryDemo(twitterCtx);
            //RetweetsCount(twitterCtx);
            //FirstStatusQueryDemo(twitterCtx);
            //GetAllTweetsAndRetweetsDemo(twitterCtx);
            //ContributorIDsDemo(twitterCtx);
            //StatusCountDemo(twitterCtx);
        }

        #region Status Demos

        /// <summary>
        /// Shows how to get statuses for logged-in user's friends - just like main Twitter page
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SingleStatusQueryDemo(TwitterContext twitterCtx)
        {
            var friendTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Show &&
                      tweet.ID == "10520783556"
                select tweet;

            Console.WriteLine("\nRequested Tweet: \n");
            foreach (var tweet in friendTweets)
            {
                Console.WriteLine(
                    "User: " + tweet.User.Name +
                    "\nTweet: " + tweet.Text +
                    "\nTweet ID: " + tweet.ID + "\n");
            }
        }

        /// <summary>
        /// Shows how to get statuses for logged-in user's friends - just like main Twitter page
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void FriendStatusQueryDemo(TwitterContext twitterCtx)
        {
            var friendTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Friends
                select tweet;

            Console.WriteLine("\nTweets for " + twitterCtx.UserName + "\n");
            foreach (var tweet in friendTweets)
            {
                Console.WriteLine(
                    "Friend: " + tweet.User.Name +
                    "\nTweet: " + tweet.Text + "\n");
            }
        }

        /// <summary>
        /// Shows how to get statuses for logged-in user's friends, including retweets
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HomeStatusQueryDemo(TwitterContext twitterCtx)
        {
            var friendTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Home &&
                      tweet.Page == 2
                select new
                {
                    tweet.User.Name,
                    tweet.Retweet,
                    tweet.Text
                };

            Console.WriteLine("\nTweets for " + twitterCtx.UserName + "\n");
            foreach (var tweet in friendTweets)
            {
                Console.WriteLine(
                    "Friend: " + tweet.Name +
                    "\nRetweeted by: " +
                        (tweet.Retweet == null ?
                            "Original Tweet" :
                            tweet.Retweet.RetweetingUser.Name) +
                    "\nTweet: " + tweet.Text + "\n");
            }
        }

        /// <summary>
        /// Shows how to query tweets menioning logged-in user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void MentionsStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myMentions =
                from mention in twitterCtx.Status
                where mention.Type == StatusType.Mentions
                select mention;

            myMentions.ToList().ForEach(
                mention => Console.WriteLine(
                    "Name: {0}, Tweet[{1}]: {2}\n",
                    mention.User.Name, mention.StatusID, mention.Text));
        }

        /// <summary>
        /// Shows how to query tweets menioning logged-in user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void MentionsWithSinceIDStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myMentions =
                from mention in twitterCtx.Status
                where mention.Type == StatusType.Mentions
                    && mention.SinceID == 7841796067
                select mention;

            myMentions.ToList().ForEach(
                mention => Console.WriteLine(
                    "Name: {0}, Tweet[{1}]: {2}\n",
                    mention.User.Name, mention.StatusID, mention.Text));
        }

        /// <summary>
        /// Shows how to query tweets menioning logged-in user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void MentionsWithPagingQueryDemo(TwitterContext twitterCtx)
        {
            bool hasMoreTweets;
            int page = 1;

            do
            {
                var myMentions =
                    (from mention in twitterCtx.Status
                     where mention.Type == StatusType.Mentions
                        && mention.Page == page
                     select mention)
                     .ToList();

                hasMoreTweets = myMentions.Count > 0;
                if (hasMoreTweets)
                {
                    Console.WriteLine("\n*** Page {0} ***\n", page);

                    myMentions.ForEach(
                        mention => Console.WriteLine(
                            "Name: {0}, Tweet[{1}]: {2}\n",
                            mention.User.Name, mention.StatusID, mention.Text));
                }

                page++;

            } while (hasMoreTweets);
        }

        private static void RetweetDemo(TwitterContext twitterCtx)
        {
            var retweet = twitterCtx.Retweet("11515561768");

            Console.WriteLine("Retweeted Tweet: ");
            Console.WriteLine(
                "\nUser: " + retweet.Retweet.RetweetingUser.Name +
                "\nTweet: " + retweet.Retweet.Text +
                "\nTweet ID: " + retweet.Retweet.ID + "\n");
        }

        private static void RetweetsCount(TwitterContext twitterCtx)
        {
            long idTweet = 16151285130;

            var result = 
                from tweet in twitterCtx.Status
                where tweet.ID == idTweet.ToString() &&
                      tweet.Type == StatusType.Retweets
                select tweet;

            Console.WriteLine("Retweet Count: " + result.Count<LinqToTwitter.Status>()); 
        }

        /// <summary>
        /// Shows how to get retweets of a specified tweet
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void RetweetsQueryDemo(TwitterContext twitterCtx)
        {
            var friendTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Retweets &&
                      tweet.ID == "10875450034"
                select tweet;

            Console.WriteLine("\nReTweets: \n");
            foreach (var tweet in friendTweets)
            {
                Console.WriteLine(
                    "\nUser: " + tweet.Retweet.RetweetingUser.Name +
                    "\nTweet: " + tweet.Retweet.Text +
                    "\nTweet ID: " + tweet.Retweet.ID + "\n");
            }
        }

        /// <summary>
        /// Shows how to query retweets by the logged-in user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void RetweetedByMeStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myRetweets =
                from retweet in twitterCtx.Status
                where retweet.Type == StatusType.RetweetedByMe
                select retweet;

            myRetweets.ToList().ForEach(
                retweet => Console.WriteLine(
                    "Name: {0}, Tweet: {1}\n",
                    retweet.Retweet.RetweetingUser.Name, retweet.Retweet.Text));
        }

        /// <summary>
        /// Shows how to query retweets by the logged-in user, specifying the number of tweets
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void RetweetedByMeWithCountStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myRetweets =
                from retweet in twitterCtx.Status
                where retweet.Type == StatusType.RetweetedByMe
                   && retweet.Count == 5
                select retweet;

            myRetweets.ToList().ForEach(
                retweet => Console.WriteLine(
                    "Name: {0}, Tweet: {1}\n",
                    retweet.Retweet.RetweetingUser.Name, retweet.Retweet.Text));
        }

        /// <summary>
        /// Shows how to query retweets to the logged-in user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void RetweetedToMeStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myRetweets =
                from retweet in twitterCtx.Status
                where retweet.Type == StatusType.RetweetedToMe
                select retweet;

            myRetweets.ToList().ForEach(
                retweet => Console.WriteLine(
                    "Name: {0}, Tweet: {1}\n",
                    retweet.Retweet.RetweetingUser.Name, retweet.Retweet.Text));
        }

        /// <summary>
        /// Shows how to query retweets about the logged-in user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void RetweetsOfMeStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myRetweets =
                from retweet in twitterCtx.Status
                where retweet.Type == StatusType.RetweetsOfMe
                select retweet;

            myRetweets.ToList().ForEach(
                retweet => Console.WriteLine(
                    "Name: {0}, Tweet: {1}\n",
                    retweet.User.Name, retweet.Text));
        }

        /// <summary>
        /// Shows how to get tweets and retweets by the logged-in user through a union
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetAllTweetsAndRetweetsDemo(TwitterContext twitterCtx)
        {
            var myTweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.User
                      && tweet.ScreenName == "JoeMayo"
                 select tweet)
                 .ToList();

            var myRetweets =
                (from retweet in twitterCtx.Status
                 where retweet.Type == StatusType.RetweetedByMe
                 select retweet)
                 .ToList();

            var allTweets = myTweets.Union(myRetweets);

            allTweets.ToList().ForEach(
                tweet =>
                {
                    if (tweet.Retweet == null)
                    {
                        Console.WriteLine(
                            "Name: {0}, Tweet: {1}\n",
                            tweet.User.Name, tweet.Text);
                    }
                    else
                    {
                        Console.WriteLine(
                            "Name: {0}, ReTweet: {1}\n",
                            tweet.Retweet.RetweetingUser.Name, tweet.Retweet.Text);
                    }
                });
        }

        /// <summary>
        /// shows how to query status with a screen name for specified number of tweets
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserStatusByNameQueryDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();

            var lastN = 11;
            var screenName = "JoeMayo";

            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      && tweet.ScreenName == screenName
                      && tweet.Count == lastN
                select tweet;

            foreach (var tweet in statusTweets)
            {
                Console.WriteLine(
                    "(" + tweet.StatusID + ")" +
                    "[" + tweet.User.ID + "]" +
                    tweet.User.Name + ", " +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to include retweets with user statuses
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserStatusWithRetweetsQueryDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();

            var screenName = "JoeMayo";

            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      && tweet.ScreenName == screenName
                      && tweet.IncludeRetweets == true
                select tweet;

            foreach (var tweet in statusTweets)
            {
                Console.WriteLine(
                    "(" + tweet.StatusID + ")" +
                    "[" + tweet.User.ID + "]" + 
                    ", Is Retweet: " + (tweet.Retweet != null).ToString() + "\n" +
                    tweet.User.Name + ", " + "\n" +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to query status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserStatusQueryDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();

            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      && tweet.ID == "15411837"  // ID for User
                select tweet;

            foreach (var tweet in statusTweets)
            {
                Console.WriteLine(
                    "(" + tweet.StatusID + ")" +
                    "[" + tweet.User.ID + "]" +
                    tweet.User.Name + ", " +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to query status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void FirstStatusQueryDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();

            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      && tweet.ID == "15411837"  // ID for User
                      && tweet.Page == 1
                      && tweet.Count == 20
                      && tweet.SinceID == 931894254
                select tweet;

            var status = statusTweets.FirstOrDefault();

            Console.WriteLine(
                "(" + status.StatusID + ")" +
                "[" + status.User.ID + "]" +
                status.User.Name + ", " +
                status.Text + ", " +
                status.CreatedAt);
        }

        /// <summary>
        /// shows how to delete a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DestroyStatusDemo(TwitterContext twitterCtx)
        {
            //var status = twitterCtx.DestroyStatus("1539399086");

            //Console.WriteLine(
            //    "(" + status.StatusID + ")" +
            //    "[" + status.User.ID + "]" +
            //    status.User.Name + ", " +
            //    status.Text + ", " +
            //    status.CreatedAt);
        }

        /// <summary>
        /// shows how to update a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusWithReplyDemo(TwitterContext twitterCtx)
        {
            var tweet = twitterCtx.UpdateStatus("@LinqToTweeter Testing LINQ to Twitter with reply on " + DateTime.Now.ToString() + " #linqtotwitter", "961760788");

            Console.WriteLine(
                "(" + tweet.StatusID + ")" +
                "[" + tweet.User.ID + "]" +
                tweet.User.Name + ", " +
                tweet.Text + ", " +
                tweet.CreatedAt);
        }

        /// <summary>
        /// shows how to update a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusDemo(TwitterContext twitterCtx)
        {
            // the \u00C7 is C Cedilla, which I've included to ensure that non-ascii characters appear properly
            var status = "\u00C7 Testing LINQ to Twitter update status on " + DateTime.Now.ToString() + " #linqtotwitter";

            Console.WriteLine("Status being sent: " + status);

            var tweet = twitterCtx.UpdateStatus(status);

            Console.WriteLine(
                "Status returned: " +
                "(" + tweet.StatusID + ")" +
                "[" + tweet.User.ID + "]" +
                tweet.User.Name + ", " +
                tweet.Text + ", " +
                tweet.CreatedAt);
        }

        /// <summary>
        /// shows how to update a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusWithPlaceDemo(TwitterContext twitterCtx)
        {
            // the \u00C7 is C Cedilla, which I've included to ensure that non-ascii characters appear properly
            var status = "\u00C7 Testing LINQ to Twitter update status on " + DateTime.Now.ToString() + " #linqtotwitter";

            Console.WriteLine("Status being sent: " + status);

            var tweet = twitterCtx.UpdateStatus(status, 37.78215m, -122.40060m, "fbd6d2f5a4e4a15e");

            Console.WriteLine(
                "User: {0}, Tweet: {1}\nLatitude: {2}, Longitude: {3}, Place: {4}",
                tweet.User.Identifier.ScreenName,
                tweet.Text,
                tweet.Coordinates.Latitude,
                tweet.Coordinates.Longitude,
                tweet.Place.Name);
        }

        /// <summary>
        /// shows how to update a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusWithLocationDemo(TwitterContext twitterCtx)
        {
            // the \u00C7 is C Cedilla, which I've included to ensure that non-ascii characters appear properly
            var status = "\u00C7 Testing LINQ to Twitter update status on " + DateTime.Now.ToString() + " #linqtotwitter";

            Console.WriteLine("Status being sent: " + status);

            var tweet = twitterCtx.UpdateStatus(status, 37.78215m, -122.40060m, true);

            Console.WriteLine(
                "User: {0}, Tweet: {1}\nLatitude: {2}, Longitude: {3}",
                tweet.User.Identifier.ScreenName,
                tweet.Text,
                tweet.Coordinates.Latitude,
                tweet.Coordinates.Longitude);
        }

        /// <summary>
        /// shows how to send a public status query and then filter
        /// </summary>
        /// <remarks>
        /// since Twitter API doesn't filter public status,
        /// you can grab the results and then filter with
        /// LINQ to Objects.
        /// </remarks>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void PublicStatusFilteredQueryDemo(TwitterContext twitterCtx)
        {
            //var publicTweets =
            //    (from tweet in twitterCtx.Status
            //     where tweet.Type == StatusType.Public &&
            //           tweet.User.Name.StartsWith("S")
            //     orderby tweet.Source descending
            //     select new MyTweetClass
            //     {
            //         UserName = tweet.User.Name,
            //         Text = tweet.Text
            //     })
            //     .ToArray();

            var publicTweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Public
                 orderby tweet.Source descending, tweet.User.Name
                 select tweet)
                 .ToArray();

            publicTweets.ToList().ForEach(
                tweet => Console.WriteLine(
                    "Source: {0}, Name: {1}",
                    tweet.Source,
                    tweet.User.Name));

            //publicTweets.ToList().ForEach(
            //    tweet => Console.WriteLine(
            //        "User Name: {0}, Tweet: {1}",
            //        tweet.User.Name,
            //        tweet.Text));

            //var publicTweets = twitterCtx.Status
            //    .Where(x => x.Type == StatusType.Public)
            //    .Select(x => x.Text.Replace('\n', ' '))
            //    .ToArray();

            //publicTweets.ToList().ForEach(
            //    tweet => Console.WriteLine(
            //        "Tweet: {0}",
            //        tweet));
        }

        /// <summary>
        /// shows how to send a public status query
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void PublicStatusQueryDemo(TwitterContext twitterCtx)
        {
            var publicTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            publicTweets.ToList().ForEach(
                tweet => Console.WriteLine(
                    "User Name: {0}, Tweet: {1}",
                    tweet.User.Name,
                    tweet.Text));
        }

        /// <summary>
        /// Shows how to read contributor IDs
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ContributorIDsDemo(TwitterContext twitterCtx)
        {
            var contributedStatus =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Show &&
                       tweet.ID == "7680619122"
                 select tweet)
                 .SingleOrDefault();

            Console.WriteLine("Contributors Enabled: {0}\n", contributedStatus.User.ContributorsEnabled);

            contributedStatus.ContributorIDs.ForEach(
                id => Console.WriteLine("ContributorID: " + id));
        }

        private static void StatusCountDemo(TwitterContext twitterCtx)
        {
            var tweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Friends
                select tweet;

            var tweetCount = tweets.Count();

            foreach (var l in tweets)
            {
                Console.WriteLine(l.Text);
            }
        }

        #endregion

    }
}
