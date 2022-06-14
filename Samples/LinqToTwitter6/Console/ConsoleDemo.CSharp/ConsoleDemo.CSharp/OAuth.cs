using LinqToTwitter.OAuth;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleDemo.CSharp
{
    public class OAuth
    {
        public static IAuthorizer ChooseAuthenticationStrategy()
        {
            Console.WriteLine("Authentication Strategy:\n\n");

            Console.WriteLine("  1 - Pin (default)");
            Console.WriteLine("  2 - Application-Only");
            Console.WriteLine("  3 - Single User");
            Console.WriteLine("  4 - XAuth");
            Console.WriteLine("  5 - OAuth 2.0");

            Console.Write("\nPlease choose (1, 2, 3, 4, or 5): ");
            ConsoleKeyInfo input = Console.ReadKey();
            Console.WriteLine("");

            IAuthorizer auth = input.KeyChar switch
            {
                '1' => DoPinOAuth(),
                '2' => DoApplicationOnlyAuth(),
                '3' => DoSingleUserAuth(),
                '4' => DoXAuth(),
                '5' => DoOAuth2ConfidentialAuth(),
                _ => DoPinOAuth(),
            };

            return auth;
        }

        static IAuthorizer DoPinOAuth()
        {
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
                },
                GoToTwitterAuthorization = pageLink =>
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = pageLink,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                },
                GetPin = () =>
                {
                    Console.WriteLine(
                        "\nAfter authorizing this application, Twitter " +
                        "will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine() ?? string.Empty;
                }
            };

            return auth;
        }

        static IAuthorizer DoApplicationOnlyAuth()
        {
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
                },
            };

            return auth;
        }

        static IAuthorizer DoSingleUserAuth()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret),
                    AccessToken = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessToken),
                    AccessTokenSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessTokenSecret)
                }
            };

            return auth;
        }

        static IAuthorizer DoXAuth()
        {
            var auth = new XAuthAuthorizer
            {
                CredentialStore = new XAuthCredentials
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret),
                    UserName = "YourUserName",
                    Password = "YourPassword"
                }
            };

            return auth;
        }

        // Not yet implemented
        static IAuthorizer DoOAuth2ConfidentialAuth()
        {
            var auth = new OAuth2Authorizer()
            {
                CredentialStore = new OAuth2CredentialStore
                {
                    ClientID = Environment.GetEnvironmentVariable(OAuthKeys.TwitterClientID),
                    ClientSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterClientSecret),
                    Scopes = new List<string>
                    {
                        "tweet.read",
                        "tweet.write",
                        "tweet.moderate.write",
                        "users.read",
                        "follows.read",
                        "follows.write",
                        "offline.access",
                        "space.read",
                        "mute.read",
                        "mute.write",
                        "like.read",
                        "like.write",
                        "block.read",
                        "block.write",
                        "bookmark.read"
                    },
                    RedirectUri = "http://127.0.0.1:8599"
                },
                GoToTwitterAuthorization = pageLink =>
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = pageLink,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                },
                HtmlResponseString = "<div>Awesome! Now you can use the app.</div>"
            };

            return auth;
        }
    }
}
