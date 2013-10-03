using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Security;

namespace LinqToTwitterDemoPcl
{
    class Program
    {
        static void Main()
        {
            Task verifyTask = VerifyCredentials();
            verifyTask.Wait();

            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        async static Task VerifyCredentials()
        {
            var verifyUrl = new Uri("https://api.twitter.com/1.1/account/verify_credentials.json?skip_status=true&include_entities=false");
            var client = new HttpClient();

            const string UserAgent = "LINQ to Twitter v3.0";

            var credStore = new InMemoryCredentialStore
            {
                ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
            };

            var auth = new PinAuthorizer(credStore, false, AuthAccessType.NoChange, null)
            {
                UserAgent = UserAgent,
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

            var parameters =
                new Dictionary<string, string>
                {
                    { "oauth_consumer_key", credStore.ConsumerKey },
                    { "oauth_token", credStore.OAuthToken },
                    { "skip_status", "true" },
                    { "include_entities", "false"}
                };

            var req = new HttpRequestMessage(HttpMethod.Get, verifyUrl);
            string authorizationString = new OAuth().GetAuthorizationString(HttpMethod.Get.ToString(), verifyUrl.ToString(), parameters, credStore.ConsumerSecret, credStore.OAuthTokenSecret);
            req.Headers.Add("Authorization", authorizationString);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.ExpectContinue = false;

            var msg = await client.SendAsync(req);

            string response = await msg.Content.ReadAsStringAsync();

            Console.WriteLine("\nResponse: " + response);
        }
    }
}
