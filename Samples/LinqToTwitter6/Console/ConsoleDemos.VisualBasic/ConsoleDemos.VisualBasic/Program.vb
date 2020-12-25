Imports System.Net
Imports LinqToTwitter
Imports LinqToTwitter.OAuth

Module Program

    Sub Main()
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        SendTweet().Wait()

    End Sub

    Async Function SendTweet() As Task

        Dim auth As IAuthorizer = OAuth.ChooseAuthenticationStrategy()

        Await auth.AuthorizeAsync()


        'This Is how you access credentials after authorization.
        'The oauthToken And oauthTokenSecret do Not expire.
        'You can use the userID to associate the credentials with the user.
        'You can save credentials any way you want -database, isolated storage, etc. - it's up to you.
        'You can retrieve And load all 4 credentials on subsequent queries to avoid the need to re-authorize.
        'When you've loaded all 4 credentials, LINQ to Twitter will let you make queries without re-authorizing.
        '
        'Dim credentials = auth.CredentialStore
        'Dim oauthToken = credentials.OAuthToken
        'Dim oauthTokenSecret = credentials.OAuthTokenSecret
        'Dim screenName = credentials.ScreenName
        'Dim userID = credentials.UserID

        Dim twitterCtx = New TwitterContext(auth)
        Dim key As Char

        Do
            ShowMenu()

            key = Console.ReadKey(True).KeyChar

            Select Case key
                Case "0"
                    Console.WriteLine()
                    Console.WriteLine("  Running Account Demos...")
                    Console.WriteLine()
                    Await AccountDemos.RunAsync(twitterCtx)
                Case "1"
                    Console.WriteLine()
                    Console.WriteLine("  Running Account Activity Demos...")
                    Console.WriteLine()
                    Await AccountActivityDemos.RunAsync(twitterCtx)
                Case "2"
                    Console.WriteLine()
                    Console.WriteLine("  Running Block Demos...")
                    Console.WriteLine()
                    Await BlockDemos.RunAsync(twitterCtx)
                Case "3"
                    Console.WriteLine()
                    Console.WriteLine("  Running Direct Message Events Demos...")
                    Console.WriteLine()
                    Await DirectMessageEventsDemos.RunAsync(twitterCtx)
                Case "4"
                    Console.WriteLine()
                    Console.WriteLine("  Running Favorite Demos...")
                    Console.WriteLine()
                    Await FavoriteDemos.RunAsync(twitterCtx)
                Case "5"
                    Console.WriteLine()
                    Console.WriteLine("  Running Friendship Demos...")
                    Console.WriteLine()
                    Await FriendshipDemos.RunAsync(twitterCtx)
                Case "6"
                    Console.WriteLine()
                    Console.WriteLine("  Running Geo Demos...")
                    Console.WriteLine()
                    Await GeoDemos.RunAsync(twitterCtx)
                Case "7"
                    Console.WriteLine()
                    Console.WriteLine("  Running Help Demos...")
                    Console.WriteLine()
                    Await HelpDemos.RunAsync(twitterCtx)
                Case "8"
                    Console.WriteLine()
                    Console.WriteLine("  Running List Demos...")
                    Console.WriteLine()
                    Await ListDemos.RunAsync(twitterCtx)
                Case "9"
                    Console.WriteLine()
                    Console.WriteLine("  Running Media Demos...")
                    Console.WriteLine()
                    Await MediaDemos.RunAsync(twitterCtx)
                Case "a", "A"
                    Console.WriteLine()
                    Console.WriteLine("  Running Mutes Demos...")
                    Console.WriteLine()
                    Await MuteDemos.RunAsync(twitterCtx)
                Case "b", "B"
                    Console.WriteLine()
                    Console.WriteLine("  Running Raw Demos...")
                    Console.WriteLine()
                    Await RawDemos.RunAsync(twitterCtx)
                Case "c", "C"
                    Console.WriteLine()
                    Console.WriteLine("  Running Saved Search Demos...")
                    Console.WriteLine()
                    'Await SavedSearchDemos.RunAsync(twitterCtx)
                Case "d", "D"
                    Console.WriteLine()
                    Console.WriteLine("  Running Search Demos...")
                    Console.WriteLine()
                    Await SearchDemos.RunAsync(twitterCtx)
                Case "e", "E"
                    Console.WriteLine()
                    Console.WriteLine("  Running Status Demos...")
                    Console.WriteLine()
                    'Await StatusDemos.RunAsync(twitterCtx)
                Case "f", "F"
                    Console.WriteLine()
                    Console.WriteLine("  Running Stream Demos...")
                    Console.WriteLine()
                    'Await StreamDemos.RunAsync(twitterCtx)
                Case "g", "G"
                    Console.WriteLine()
                    Console.WriteLine("  Running Trend Demos...")
                    Console.WriteLine()
                    'Await TrendDemos.RunAsync(twitterCtx)
                Case "h", "H"
                    Console.WriteLine()
                    Console.WriteLine("  Running User Demos...")
                    Console.WriteLine()
                Case "i", "I"
                    Console.WriteLine()
                    Console.WriteLine("  Running Welcome Message Demos...")
                    Console.WriteLine()
                    'Await WelcomeMessageDemos.RunAsync(twitterCtx)
                Case "j", "J"
                    Console.WriteLine()
                    Console.WriteLine("  Running Tweet Demos...")
                    Console.WriteLine()
                    Await TweetDemos.RunAsync(twitterCtx)
                Case "q", "Q"
                    key = "Q"
                    Console.WriteLine()
                    Console.WriteLine("Quitting...")
                    Console.WriteLine()
                Case Else
                    Console.WriteLine(key + " is unknown")

            End Select
        Loop Until (key = "Q")

        Console.WriteLine()
        Console.Write("Press any key to continue...")
        Console.ReadKey()

    End Function

    Sub ShowMenu()

        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine("Please select category:")
        Console.WriteLine()

        Console.WriteLine("    0. Account Demos")
        Console.WriteLine("    1. Account Activity Demos")
        Console.WriteLine("    2. Block Demos")
        Console.WriteLine("    3. Direct Message Event Demos")
        Console.WriteLine("    4. Favorite Demos")
        Console.WriteLine("    5. Friendship Demos")
        Console.WriteLine("    6. Geo Demos")
        Console.WriteLine("    7. Help Demos")
        Console.WriteLine("    8. List Demos")
        Console.WriteLine("    9. Media Demos")
        Console.WriteLine("    A. Mutes Demos")
        Console.WriteLine("    B. Raw Demos")
        Console.WriteLine("    C. Saved Search Demos")
        Console.WriteLine("    D. Search Demos")
        Console.WriteLine("    E. Status Demos")
        Console.WriteLine("    F. Stream Demos")
        Console.WriteLine("    G. Trend Demos")
        Console.WriteLine("    H. User Demos")
        Console.WriteLine("    I. Welcome Message Demos")
        Console.WriteLine("    J. Tweet Demos")
        Console.WriteLine()
        Console.Write("    Q. End Program")

    End Sub

End Module
