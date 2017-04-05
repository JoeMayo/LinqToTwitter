Imports System.Configuration
Imports LinqToTwitter

Module Module1

    Sub Main()
        SendTweet().Wait()
    End Sub

    Async Function SendTweet() As Task

        Dim credentials As InMemoryCredentialStore = New InMemoryCredentialStore
        credentials.ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey)
        credentials.ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)

        Dim auth As PinAuthorizer = New PinAuthorizer()
        auth.CredentialStore = credentials
        auth.GetPin = AddressOf VerifierCallback
        auth.GoToTwitterAuthorization = Function(pageLink) Process.Start(pageLink)

        Await auth.AuthorizeAsync()

        Dim twitterCtx As TwitterContext = New TwitterContext(auth)

        Await twitterCtx.TweetAsync("Test tweet: " & Date.Now)

    End Function

    Function VerifierCallback() As String

        Console.WriteLine("Next, you'll need to tell Twitter to authorize access.\nThis program will not have access to your credentials, which is the benefit of OAuth.\nOnce you log into Twitter and give this program permission,\n come back to this console.")

        Console.Write("Please enter the PIN that Twitter gives you after authorizing this client: ")

        Return Console.ReadLine()

    End Function

End Module
