Imports System.Net
Imports System.Net.Sockets
Imports LinqToTwitter

Module Program

    Sub Main()
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        SendTweet().Wait()
    End Sub

    Async Function SendTweet() As Task

        Dim credentials As InMemoryCredentialStore = New InMemoryCredentialStore
        credentials.ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey)
        credentials.ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)

        Dim auth As PinAuthorizer = New PinAuthorizer()
        auth.CredentialStore = credentials
        auth.GetPin =
                Function()
                    Console.WriteLine("Next, you'll need to tell Twitter to authorize access. This program will not have access to your credentials, which is the benefit of OAuth. Once you log into Twitter and give this program permission, come back to this console.")
                    Console.WriteLine()
                    Console.Write("Please enter the PIN from Twitter: ")

                    Return Console.ReadLine()
                End Function
        auth.GoToTwitterAuthorization =
            Function(pageLink)
                Dim psi As ProcessStartInfo = New ProcessStartInfo
                With psi
                    .FileName = pageLink
                    .UseShellExecute = True
                End With
                Return Process.Start(psi)
            End Function

        Await auth.AuthorizeAsync()

        Dim twitterCtx = New TwitterContext(auth)

        Console.Write("Type '1' to tweet or '2' to search: ")
        Dim key = Console.ReadKey()

        If (key.Key = ConsoleKey.D1) Then
            Await twitterCtx.TweetAsync("Test tweet: " & Date.Now)
        Else
            Await SearchAsync(twitterCtx)
        End If

        Console.WriteLine()
        Console.Write("Press any key to continue...")
        Console.ReadKey()

    End Function

    Private Async Function SearchAsync(twitterCtx As TwitterContext) As Task
        Dim Response As Search =
            Await (From search In twitterCtx.Search()
                   Where search.Type = SearchType.Search _
                   AndAlso search.TweetMode = TweetMode.Extended _
                   AndAlso search.Query = "Coronavirus") _
                  .SingleOrDefaultAsync()

        Dim tweets As List(Of Status) = Response.Statuses()

        If Response IsNot Nothing AndAlso Response.Statuses IsNot Nothing Then
            For Each str As Status In tweets
                Console.WriteLine(str.StatusID.ToString() + " " + str.FullText)

                If str.ExtendedEntities.MediaEntities.Count > 0 Then
                    Console.WriteLine(" - Media URL: " + str.ExtendedEntities.MediaEntities(0).MediaUrl)
                End If
            Next
        End If
    End Function
End Module
