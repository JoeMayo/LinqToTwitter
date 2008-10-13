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

            var tweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Friends
                      //&& tweet.ID == "945932078" // ID for Show
                      //&& tweet.ID == "15411837"  // ID for User
                      && tweet.Page == 0
                      && tweet.Count == 21
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
