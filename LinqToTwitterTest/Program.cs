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
            var tweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      //&& tweet.ID == "945932078" // ID for Show
                      && tweet.ID == "15411837"  // ID for User
                      && tweet.Page == 4
                      && tweet.Count == 20
                      && tweet.SinceID == 931894254
                      && tweet.Since == DateTime.Now.AddMonths(-1)
                select tweet;

            foreach (var tweet in tweets)
            {
                Console.WriteLine(
                    "(" + tweet.ID + ")" +
                    "[" + tweet.User.ID + "]" +
                    tweet.User.Name + ", " +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            } 

            Console.ReadKey();
        }
    }
}
