using System;
using System.Linq;

using LinqToTwitter;

namespace LinqToTwitterDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Twitter User Name: ");
            string userName = Console.ReadLine();

            Console.Write("Twitter Password: ");
            string password = Console.ReadLine();

            var twitterCtx = new TwitterContext(userName, password, "http://www.twitter.com/");

            //
            // status tweets
            //

            //var statusResult = twitterCtx.UpdateStatus("@TwitterUser Testing LINQ to Twitter with reply", "961760788");
            //var statusResult = twitterCtx.UpdateStatus("Testing LINQ to Twitter with only status");
            //var statusResult = twitterCtx.Destroy("961866827");

            //foreach (var tweet in statusResult)
            //{
            //    Console.WriteLine(
            //        "(" + tweet.ID + ")" +
            //        "[" + tweet.User.ID + "]" +
            //        tweet.User.Name + ", " +
            //        tweet.Text + ", " +
            //        tweet.CreatedAt);
            //}
            Console.WriteLine();
            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      //&& tweet.ID == "945932078" // ID for Show
                      && tweet.ID == "15411837"  // ID for User
                      && tweet.Page == 1
                      && tweet.Count == 20
                      && tweet.SinceID == 931894254
                      && tweet.Since == DateTime.Now.AddMonths(-1)
                select tweet;

            foreach (var tweet in statusTweets)
            {
                Console.WriteLine(
                    "(" + tweet.ID + ")" +
                    "[" + tweet.User.ID + "]" +
                    tweet.User.Name + ", " +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            } 

            //
            // user tweets
            //

            var userTweets =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Show &&
                      tweet.ID == "15411837"
                select tweet;

            var tweetList = userTweets.ToList();

            Console.ReadKey();
        }
    }
}
