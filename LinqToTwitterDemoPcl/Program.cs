using System;
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

            await auth.AuthorizeAsync();

            var ctx = new TwitterContext(auth);

            var tweets = await
                (from tweet in ctx.Status
                 where tweet.Type == StatusType.Home
                 select tweet)
                .ToListAsync();

            tweets.ForEach(tweet =>
                Console.WriteLine("\nName:\n{0}\nTweet:{1}\n", tweet.ScreenName, tweet.Text));

            var firstTweet = await
                (from twt in ctx.Status
                 where twt.Type == StatusType.Home
                 select twt)
                .FirstOrDefaultAsync();

            Console.WriteLine("\nName:\n{0}\nTweet:{1}\n", firstTweet.ScreenName, firstTweet.Text);
        }
    }
}
