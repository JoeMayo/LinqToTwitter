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

            //var type = "Public";
            //var type = "Friends";
            var type = "User";

            var tweets =
                from tweet in twitterCtx.Status
                where tweet.Type == type
                      && tweet.ID == "15411837"
                      //&& tweet.Page == 0
                      //&& tweet.Count == 21
                      //&& tweet.SinceID == 934818247
                      //&& tweet.Since == DateTime.Now.AddHours(-8)
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
