using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
            //HomeStatusQueryDemo(twitterCtx);
            //HomeSinceStatusQueryDemo(twitterCtx);
            //UserStatusQueryDemo(twitterCtx);
            //UserStatusSinceIDQueryDemo(twitterCtx);
            //UserStatusByNameQueryDemo(twitterCtx);
            //UserStatusWithRetweetsQueryDemo(twitterCtx);
            //MentionsStatusQueryDemo(twitterCtx);
            //MentionsWithSinceIDStatusQueryDemo(twitterCtx);
            //MentionsWithPagingQueryDemo(twitterCtx);
            //SingleStatusQueryDemo(twitterCtx);
            //UpdateStatusDemo(twitterCtx);
            //UpdateStatusWrapLinksDemo(twitterCtx);
            //UpdateStatusWithCallbackDemo(twitterCtx);
            //UpdateStatusWithReplyDemo(twitterCtx);
            //UpdateStatusWithLocationDemo(twitterCtx);
            //UpdateStatusWithPlaceDemo(twitterCtx);
            //DestroyStatusDemo(twitterCtx);
            //RetweetedByMeStatusQueryDemo(twitterCtx);
            //RetweetedByMeWithCountStatusQueryDemo(twitterCtx);
            //RetweetedToMeStatusQueryDemo(twitterCtx);
            //RetweetsOfMeStatusQueryDemo(twitterCtx);
            //RetweetedByUserStatusQueryDemo(twitterCtx);
            //RetweetDemo(twitterCtx);
            //RetweetsQueryDemo(twitterCtx);
            //RetweetsCount(twitterCtx);
            //FirstStatusQueryDemo(twitterCtx);
            //GetAllTweetsAndRetweetsDemo(twitterCtx);
            //ContributorIDsDemo(twitterCtx);
            //ContributorDetailsDemo(twitterCtx);
            //StatusCountDemo(twitterCtx);
            //StatusJoinDemo(twitterCtx);
            //TrimUserDemo(twitterCtx);
            //TweetWithMediaDemo(twitterCtx);
            //TweetEntityDemo(twitterCtx);
            RetweetedByDemo(twitterCtx);
        }

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
                    tweet.RetweetedStatus,
                    tweet.Text
                };

            Console.WriteLine("\nTweets for " + twitterCtx.UserName + "\n");
            foreach (var tweet in friendTweets)
            {
                Console.WriteLine(
                    "Friend: " + tweet.Name +
                    "\nRetweeted by: " +
                        (tweet.RetweetedStatus == null ?
                            "Original Tweet" :
                            tweet.RetweetedStatus.User.Name) +
                    "\nTweet: " + tweet.Text + "\n");
            }
        }

        /// <summary>
        /// Shows how to get statuses for logged-in user's friends, including retweets
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HomeSinceStatusQueryDemo(TwitterContext twitterCtx)
        {
            var friendTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User &&
                      tweet.ScreenName == "JoeMayo" &&
                      tweet.CreatedAt < DateTime.Now.AddDays(-10).Date
                select new
                {
                    tweet.User.Name,
                    tweet.RetweetedStatus,
                    tweet.Text
                };

            Console.WriteLine("\nTweets for " + twitterCtx.UserName + "\n");
            foreach (var tweet in friendTweets)
            {
                Console.WriteLine(
                    "Friend: " + tweet.Name +
                    "\nRetweeted by: " +
                        (tweet.RetweetedStatus == null ?
                            "Original Tweet" :
                            tweet.RetweetedStatus.User.Name) +
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
            var retweet = twitterCtx.Retweet("196991337554378752");

            Console.WriteLine("Retweeted Tweet: ");
            Console.WriteLine(
                "\nUser: " + retweet.RetweetedStatus.User.Name +
                "\nTweet: " + retweet.RetweetedStatus.Text +
                "\nTweet ID: " + retweet.RetweetedStatus.ID + "\n");
        }

        private static void RetweetsCount(TwitterContext twitterCtx)
        {
            var tweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home &&
                       tweet.IncludeRetweets == true
                 select tweet)
                .ToList();

            tweets.ForEach(tweet =>
                Console.WriteLine(
                    "* Tweet: {0}\nRetweeted: {1}, Retweet Count: {2}\n" +
                    "Retweet: {3}\nRetweeted: {4}, Retweet Count: {5}\n",
                    tweet.Text, tweet.Retweeted, tweet.RetweetCount,
                    tweet.RetweetedStatus == null ? "<none>" : tweet.RetweetedStatus.Text,
                    tweet.RetweetedStatus == null ? false : tweet.RetweetedStatus.Retweeted,
                    tweet.RetweetedStatus == null ? 0 : tweet.RetweetedStatus.RetweetCount));
        }

        /// <summary>
        /// Shows how to get retweets of a specified tweet
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void RetweetsQueryDemo(TwitterContext twitterCtx)
        {
            var publicTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.RetweetsOfMe
                select tweet;

            publicTweets.ToList().ForEach(
                tweet =>
                {
                    Console.WriteLine(
                        "@{0} {1} ({2})",
                        tweet.User.Identifier.ScreenName,
                        tweet.Text,
                        tweet.RetweetCount);

                    var friendTweets =
                        (from retweet in twitterCtx.Status
                         where retweet.Type == StatusType.Retweets && 
                               retweet.ID == tweet.StatusID
                         select retweet)
                        .ToList();

                    friendTweets.ForEach(
                        friendTweet => Console.WriteLine(".@{0}", friendTweet.User.Identifier.ScreenName));
                });
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
                    retweet.RetweetedStatus.User.Name, retweet.RetweetedStatus.Text));
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
                    retweet.RetweetedStatus.User.Name, retweet.RetweetedStatus.Text));
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
                    retweet.RetweetedStatus.User.Name, retweet.RetweetedStatus.Text));
        }

        /// <summary>
        /// Shows how to query retweets about the logged-in user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void RetweetsOfMeStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myRetweets =
                from retweet in twitterCtx.Status
                where retweet.Type == StatusType.RetweetsOfMe &&
                      retweet.Count == 100
                select retweet;

            myRetweets.ToList().ForEach(
                retweet => Console.WriteLine(
                    "Name: {0}, Tweet: {1}\n",
                    retweet.User.Name, retweet.Text));
        }

        /// <summary>
        /// Shows how to query retweets by the specified user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void RetweetedByUserStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myRetweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.RetweetedByUser &&
                       tweet.ScreenName == "JoeMayo"
                 select tweet.RetweetedStatus)
                .ToList();

            myRetweets.ForEach(
                retweet =>
                {
                    if (retweet != null)
                    {
                        Console.WriteLine(
                            "Name: {0}, Tweet: {1}\n",
                            retweet.User.Name, retweet.Text); 
                    }
                });
        }

        /// <summary>
        /// Shows how to query retweets to the specified user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void RetweetedToUserStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myRetweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.RetweetedToUser &&
                       tweet.ScreenName == "JoeMayo"
                 select tweet.RetweetedStatus)
                .ToList();

            myRetweets.ForEach(
                retweet =>
                {
                    if (retweet != null)
                    {
                        Console.WriteLine(
                            "Name: {0}, Tweet: {1}\n",
                            retweet.User.Name, retweet.Text);
                    }
                });
        }

        private static void RetweetedByDemo(TwitterContext twitterCtx)
        {
            var status =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.RetweetedBy &&
                       tweet.ID == "210591841312190464"
                 select tweet)
                .SingleOrDefault();

            status.Users.ForEach(
                user => Console.WriteLine("User: " + user.Identifier.ScreenName));
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
                    if (tweet.RetweetedStatus == null)
                    {
                        Console.WriteLine(
                            "Name: {0}, Tweet: {1}\n",
                            tweet.User.Name, tweet.Text);
                    }
                    else
                    {
                        Console.WriteLine(
                            "Name: {0}, ReTweet: {1}\n",
                            tweet.RetweetedStatus.User.Name, tweet.RetweetedStatus.Text);
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
                    ", Is Retweet: " + (tweet.RetweetedStatus != null).ToString() + "\n" +
                    tweet.User.Name + ", " + "\n" +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to query user status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserStatusQueryDemo(TwitterContext twitterCtx)
        {
            int maxStatuses = 30;
            int lastStatusCount = 0;
            ulong sinceID = 182583822993465346; // last tweet processed on previous query
            ulong maxID;
            int count = 10;
            var statusList = new List<Status>();

            // only count
            var userStatusResponse =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.User &&
                       tweet.ScreenName == "JoeMayo" &&
                       tweet.Count == count
                 select tweet)
                .ToList();

            maxID = userStatusResponse.Min(status => ulong.Parse(status.StatusID)) - 1; // first tweet processed on current query
            statusList.AddRange(userStatusResponse);

            do
            {
                // now add sinceID and maxID
                userStatusResponse =
                    (from tweet in twitterCtx.Status
                     where tweet.Type == StatusType.User &&
                           tweet.ScreenName == "JoeMayo" &&
                           tweet.Count == count &&
                           tweet.SinceID == sinceID &&
                           tweet.MaxID == maxID
                     select tweet)
                    .ToList();

                maxID = userStatusResponse.Min(status => ulong.Parse(status.StatusID)) - 1; // first tweet processed on current query
                statusList.AddRange(userStatusResponse);

                lastStatusCount = userStatusResponse.Count;
            }
            while (lastStatusCount != 0 && statusList.Count < maxStatuses);

            for (int i = 0; i < statusList.Count; i++)
            {
                Status status = statusList[i];

                Console.WriteLine("{0, 4}. [{1}] User: {2}\nStatus: {3}",
                    i + 1, status.StatusID, status.User.Name, status.Text);
            }
        }

        /// <summary>
        /// shows how to query status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserStatusSinceIDQueryDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();

            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User &&
                      tweet.ScreenName == "JoeMayo" &&
                      tweet.SinceID == 79608230359212032 &&
                      tweet.IncludeRetweets == true
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
            var status = twitterCtx.DestroyStatus("197005200609910784");

            Console.WriteLine(
                "(" + status.StatusID + ")" +
                "[" + status.User.ID + "]" +
                status.User.Name + ", " +
                status.Text + ", " +
                status.CreatedAt);
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
            //var status = "Testing LINQ to Twitter update status on " + DateTime.Now.ToString() + " #linqtotwitter";

            Console.WriteLine("\nStatus being sent: \n\n\"{0}\"", status);
            Console.WriteLine("\nPress any key to post tweet...\n");
            Console.ReadKey();

            var tweet = twitterCtx.UpdateStatus(status);

            Console.WriteLine(
                "Status returned: " +
                "(" + tweet.StatusID + ")" +
                "[" + tweet.User.ID + "]" +
                tweet.User.Name + ", " +
                tweet.Text + ", " +
                tweet.CreatedAt + "\n");
        }

        /// <summary>
        /// shows how to update a status and wrap links
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusWrapLinksDemo(TwitterContext twitterCtx)
        {
            var status = 
                "Test for LINQ to Twitter update status on " + DateTime.Now.ToString() + 
                " http://linqtotwitter.codeplex.com" + " #linq2twitter";

            bool wrapLinks = true;

            var tweet = twitterCtx.UpdateStatus(status, wrapLinks);

            Console.WriteLine(
                "Status returned: " +
                "(" + tweet.StatusID + ")" +
                "[" + tweet.User.Identifier.ScreenName + "]" +
                tweet.Text + ", " +
                tweet.CreatedAt + "\n");
        }

        /// <summary>
        /// shows how to asynchronously update a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusWithCallbackDemo(TwitterContext twitterCtx)
        {
            // the \u00C7 is C Cedilla, which I've included to ensure that non-ascii characters appear properly
            var status = "\u00C7 Testing LINQ to Twitter update status on " + DateTime.Now.ToString() + " #linqtotwitter";
            //var status = "Testing LINQ to Twitter update status on " + DateTime.Now.ToString() + " #linqtotwitter";

            Console.WriteLine("\nStatus being sent: \n\n\"{0}\"", status);
            Console.WriteLine("\nPress any key to post tweet...\n");
            Console.ReadKey();

            twitterCtx.UpdateStatus(status,
                response =>
                {
                    if (response.Status == TwitterErrorStatus.Success)
                    {
                        Status tweet = response.State;

                        Console.WriteLine(
                            "Status returned: " +
                            "(" + tweet.StatusID + ")" +
                            "[" + tweet.User.ID + "]" +
                            tweet.User.Name + ", " +
                            tweet.Text + ", " +
                            tweet.CreatedAt + "\n");
                    }
                    else
                    {
                        Console.WriteLine(response.Error.ToString());
                    }
                });
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
            var status = "\u00C7 Testing LINQ to Twitter update status on " + DateTime.Now.ToString() + " £ #linqtotwitter";
            //var status = "あいうえお" + DateTime.Now.ToString();
            string japaneseCultureString = "ja-JP";
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(japaneseCultureString);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(japaneseCultureString);
            decimal latitude = Decimal.Parse("37.78215", CultureInfo.CurrentUICulture);
            decimal longitude = Decimal.Parse("-122.40060", CultureInfo.CurrentUICulture);

            //decimal latitude = 37.78215m;
            //decimal longitude = -122.40060m;

            Console.WriteLine("Status being sent: " + status);

            var tweet = twitterCtx.UpdateStatus(status, latitude, longitude, true);

            Console.WriteLine(
                "User: {0}, Tweet: {1}\nLatitude: {2}, Longitude: {3}",
                tweet.User.Identifier.ScreenName,
                tweet.Text,
                tweet.Coordinates.Latitude,
                tweet.Coordinates.Longitude);
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

            contributedStatus.Contributors.ForEach(
                contr => Console.WriteLine("Contributor ID: " + contr));
        }

        /// <summary>
        /// Shows how to specify additional contributor info
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void ContributorDetailsDemo(TwitterContext twitterCtx)
        {
            var contributedStatus =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Show &&
                       tweet.ID == "7680619122" &&
                       tweet.IncludeContributorDetails == true
                 select tweet)
                .SingleOrDefault();

            Console.WriteLine("Contributors Enabled: {0}\n", contributedStatus.User.ContributorsEnabled);

            contributedStatus.Contributors.ForEach(
                contr => Console.WriteLine("ID: {0}", contr));
        }

        private static void StatusCountDemo(TwitterContext twitterCtx)
        {
            var tweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Show
                select tweet;

            var tweetCount = tweets.Count();

            foreach (var l in tweets)
            {
                Console.WriteLine(l.Text);
            }
        }

        private static void TrimUserDemo(TwitterContext twitterCtx)
        {
            var tweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home &&
                       tweet.TrimUser == true
                 select tweet)
                .ToList();

            tweets.ForEach(tweet => Console.WriteLine("User ID: {0}\nTweet: {1}\n", tweet.User.Identifier.ID, tweet.Text));
        }

        static void TweetWithMediaDemo(TwitterContext twitterCtx)
        {
            //string status = "Testing TweetWithMedia #Linq2Twitter £ " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            string status = "Testing TweetWithMedia #Linq2Twitter " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            const bool possiblySensitive = false;
            const decimal latitude = StatusExtensions.NoCoordinate; //37.78215m;
            const decimal longitude = StatusExtensions.NoCoordinate; // -122.40060m;
            const bool displayCoordinates = false;

            const string replaceThisWithYourImageLocation = @"..\..\images\200xColor_2.png";

            var mediaItems =
                new List<Media>
                {
                    new Media
                    {
                        Data = Utilities.GetFileBytes(replaceThisWithYourImageLocation),
                        FileName = "200xColor_2.png",
                        ContentType = MediaContentType.Png
                    }
                };

            Status tweet = twitterCtx.TweetWithMedia(
                status, possiblySensitive, latitude, longitude, 
                null, displayCoordinates, mediaItems, null);

            Console.WriteLine("Media item sent - Tweet Text: " + tweet.Text);
        }

        static void TweetEntityDemo(TwitterContext twitterCtx)
        {
            var tweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home &&
                       tweet.IncludeEntities == true &&
                       tweet.Count == 100
                 select tweet)
                .ToList();

            tweets.ForEach(tweet =>
            {
                Console.WriteLine("Tweet: " + tweet.Text);
                Console.WriteLine(
                    "Entities: \n\tHashes: {0}\n\tMedia: {1}\n\tUrl: {2}\n\tUser: {3}\n",
                    tweet.Entities.HashTagMentions.Count,
                    tweet.Entities.MediaMentions.Count,
                    tweet.Entities.UrlMentions.Count,
                    tweet.Entities.UserMentions.Count);
            });
        }
    }
}
