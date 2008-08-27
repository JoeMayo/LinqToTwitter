using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;

namespace LinqToTwitterTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Twitter User Name: ");
            string userName = Console.ReadLine();

            Console.Write("Twitter Password: ");
            string password = Console.ReadLine();

            var twitterCtx = new TwitterContext(userName, password);

            //var type = "Public";
            var type = "Friends";

            var tweets =
                from tweet in twitterCtx.Status
                where tweet.Type == type
                select tweet;

            foreach (var tweet in tweets)
            {
                Console.WriteLine(tweet.User.Name + ", " + tweet.Text);
            }

            Console.ReadKey();
        }
    }
}
