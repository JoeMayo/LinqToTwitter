using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            Uri verifyUrl = new Uri("https://api.twitter.com/1.1/account/verify_credentials.json");
            var client = new HttpClient();

            var req = new HttpRequestMessage(HttpMethod.Get, verifyUrl);
            var oAuth = new OAuth();
            oAuth.ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
            oAuth.ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];
            oAuth.OAuthToken = ConfigurationManager.AppSettings["twitterOAuthToken"];
            oAuth.OAuthTokenSecret = ConfigurationManager.AppSettings["twitterAccessToken"];

            var parameters =
                new Dictionary<string, string>
                {
                    {"oauth_consumer_key", oAuth.ConsumerKey },
                    {"oauth_nonce", oAuth.GenerateNonce() },
                    {"oauth_signature_method", "HMAC-SHA1" },
                    {"oauth_timestamp", oAuth.GenerateTimeStamp() },
                    {"oauth_token", oAuth.OAuthToken }, 
                    {"oauth_version", "1.0" }
                };

            req.Headers.ExpectContinue = false;
            req.Headers.Add("Authorization", oAuth.GetAuthorizationString(HttpMethod.Get.ToString(), verifyUrl.ToString(), parameters));
            var msg = await client.SendAsync(req);

            string response = await msg.Content.ReadAsStringAsync();

            Console.WriteLine("\nResponse: " + response);
        }
    }
}
