Imports LinqToTwitter.OAuth

Public Class OAuth
    Shared Function ChooseAuthenticationStrategy() As IAuthorizer
        Console.WriteLine("Authentication Strategy:")
        Console.WriteLine()
        Console.WriteLine()

        Console.WriteLine("  1 - Pin (default)")
        Console.WriteLine("  2 - Application-Only")
        Console.WriteLine("  3 - Single User")
        Console.WriteLine("  4 - XAuth")

        Console.WriteLine()
        Console.Write("Please choose (1, 2, 3, or 4): ")
        Dim Input As ConsoleKeyInfo = Console.ReadKey()
        Console.WriteLine("")

        Dim auth As IAuthorizer

        Select Case Input.Key
            Case ConsoleKey.D1
                auth = DoPinOAuth()
            Case ConsoleKey.D2
                auth = DoApplicationOnlyAuth()
            Case ConsoleKey.D3
                auth = DoSingleUserAuth()
            Case ConsoleKey.D4
                auth = DoXAuth()
            Case Else
                auth = DoPinOAuth()

        End Select

        Return auth
    End Function

    Private Shared Function DoPinOAuth() As IAuthorizer
        Dim auth = New PinAuthorizer With {
            .CredentialStore =
                New InMemoryCredentialStore With {
                    .ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    .ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
                },
            .GetPin =
                Function()
                    Console.WriteLine("Next, you'll need to tell Twitter to authorize access. This program will not have access to your credentials, which is the benefit of OAuth. Once you log into Twitter and give this program permission, come back to this console.")
                    Console.WriteLine()
                    Console.Write("Please enter the PIN from Twitter: ")

                    Return Console.ReadLine()
                End Function,
            .GoToTwitterAuthorization =
                Function(pageLink)
                    Dim psi As ProcessStartInfo = New ProcessStartInfo
                    With psi
                        .FileName = pageLink
                        .UseShellExecute = True
                    End With
                    Return Process.Start(psi)
                End Function
        }

        Return auth
    End Function

    Private Shared Function DoApplicationOnlyAuth() As IAuthorizer
        Dim auth = New ApplicationOnlyAuthorizer With {
            .CredentialStore =
                New InMemoryCredentialStore With {
                    .ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    .ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
                }
        }

        Return auth
    End Function

    Private Shared Function DoSingleUserAuth() As IAuthorizer
        Dim auth = New SingleUserAuthorizer With {
            .CredentialStore =
                New SingleUserInMemoryCredentialStore With {
                    .ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    .ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret),
                    .AccessToken = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessToken),
                    .AccessTokenSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessTokenSecret)
               }
        }

        Return auth
    End Function

    Private Shared Function DoXAuth() As IAuthorizer
        Dim auth = New XAuthAuthorizer With {
            .CredentialStore =
                New XAuthCredentials With {
                    .ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    .ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret),
                    .UserName = "YourUserName",
                    .Password = "YourPassword"
               }
        }

        Return auth
    End Function

End Class
