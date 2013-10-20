using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace LinqToTwitterDemoPcl
{
    class Program
    {
        static void Main()
        {
            Task verifyTask = TestLinqToTwitterAsync();
            verifyTask.Wait();

            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        async static Task TestLinqToTwitterAsync()
        {
            var auth = ChooseAuthenticationStrategy();

            await auth.AuthorizeAsync();

            var ctx = new TwitterContext(auth);

            //var tweets = await
            //    (from tweet in ctx.Status
            //     where tweet.Type == StatusType.Home &&
            //           tweet.Count == 5
            //     select tweet)
            //    .ToListAsync();

            //tweets.ForEach(tweet =>
            //    Console.WriteLine("\nName:\n{0}\nTweet:{1}\n", tweet.ScreenName, tweet.Text));

            var searchResponse = await
                (from search in ctx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "LINQ to Twitter"
                 select search)
                .FirstOrDefaultAsync();

            searchResponse.Statuses.ForEach(tweet =>
                Console.WriteLine("\nName:\n{0}\nTweet:{1}\n", tweet.ScreenName, tweet.Text));

            Status newTweet = await ctx.TweetAsync("Testing UpdateStatusAsync in LINQ to Twitter - " + DateTime.Now);

            Console.WriteLine("\nName:\n{0}\nTweet:{1}\n", newTweet.ScreenName, newTweet.Text);
        }
  
        static IAuthorizer ChooseAuthenticationStrategy()
        {
            Console.WriteLine("Authentication Strategy:\n\n");

            Console.WriteLine("  1 - Pin (default)");
            Console.WriteLine("  2 - Application-Only");
            Console.WriteLine("  3 - Single User");
            Console.WriteLine("  4 - XAuth");

            Console.Write("\nPlease choose (1, 2, 3, or 4): ");
            ConsoleKeyInfo input = Console.ReadKey();
            Console.WriteLine("");

            IAuthorizer auth = null;

            switch (input.Key)
            {

                case ConsoleKey.D1:
                    auth = DoPinOAuth();
                    break;
                case ConsoleKey.D2:
                    auth = DoApplicationOnly();
                    break;
                //case ConsoleKey.D3:
                //    auth = DoSingleUserAuth();
                //    break;
                //case ConsoleKey.D4:
                //    auth = DoXAuth();
                //    break;
                default:
                    auth = DoPinOAuth();
                    break;
            }

            return auth;
        }
  
        static IAuthorizer DoPinOAuth()
        {
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    // this executes after user authorizes, which begins with the call to auth.Authorize() below.
                    Console.WriteLine("\nAfter authorizing this application, Twitter will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };
            return auth;
        }

        static IAuthorizer DoApplicationOnly()
        {
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
                },
            };
            return auth;
        }
    }
}
