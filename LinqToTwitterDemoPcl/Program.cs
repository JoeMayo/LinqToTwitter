using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

            string consumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];
            string oAuthTokenSecret = ConfigurationManager.AppSettings["twitterAccessToken"];
            string consumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
            string oAuthToken = ConfigurationManager.AppSettings["twitterOAuthToken"];

            var parameters =
                new Dictionary<string, string>
                {
                    { "oauth_consumer_key", consumerKey },
                    { "oauth_token", oAuthToken },
                    { "skip_status", "true" },
                    { "include_entities", "false"}
                };

            var req = new HttpRequestMessage(HttpMethod.Get, verifyUrl);
            string authorizationString = new OAuth().GetAuthorizationString(HttpMethod.Get.ToString(), verifyUrl.ToString(), parameters, consumerSecret, oAuthTokenSecret);
            req.Headers.Add("Authorization", authorizationString);
            req.Headers.Add("User-Agent", "LINQ to Twitter v3.0");
            req.Headers.ExpectContinue = false;

            var msg = await client.SendAsync(req);

            string response = await msg.Content.ReadAsStringAsync();

            Console.WriteLine("\nResponse: " + response);
        }
    }
}
