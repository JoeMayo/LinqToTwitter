Imports System
Imports System.Configuration
Imports LinqToTwitter

Class Program

    Shared Sub Main()
        Try
            Dim demoTask = DoDemosAsync()
            demoTask.Wait()
        Catch ex As Exception
            Console.WriteLine(ex.ToString())
        End Try

        Console.Write(vbNewLine + "Press any key to close console window...")
        Console.ReadKey(True)
    End Sub

    Shared Async Function DoDemosAsync() As Task
        Dim auth = ChooseAuthenticationStrategy()

        Await auth.AuthorizeAsync()


        ' This is how you access credentials after authorization.
        ' The oauthToken and oauthTokenSecret do not expire.
        ' You can use the userID to associate the credentials with the user.
        ' You can save credentials any way you want - database, isolated storage, etc. - it's up to you.
        ' You can retrieve and load all 4 credentials on subsequent queries to avoid the need to re-authorize.
        ' When you've loaded all 4 credentials, LINQ to Twitter will let you make queries without re-authorizing.

        'Dim credentials = auth.CredentialStore
        'Dim oauthToken = credentials.OAuthToken
        'Dim oauthTokenSecret = credentials.OAuthTokenSecret
        'Dim screenName = credentials.ScreenName
        'Dim userID = credentials.UserID

        Dim twitterCtx As New TwitterContext(auth)
        Dim key As Char

        Do
            ShowMenu()

            key = Console.ReadKey(True).KeyChar

            Select Case key

                Case "0"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Account Demos..." + vbNewLine)
                    'await AccountDemos.RunAsync(twitterCtx)
                Case "1"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Block Demos..." + vbNewLine)
                    'Await BlockDemos.RunAsync(twitterCtx)
                Case "2"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Direct Message Demos..." + vbNewLine)
                    'Await DirectMessageDemos.RunAsync(twitterCtx)
                Case "3"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Favorite Demos..." + vbNewLine)
                    'Await FavoriteDemos.RunAsync(twitterCtx)
                Case "4"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Friendship Demos..." + vbNewLine)
                    Await FriendshipDemos.RunAsync(twitterCtx)
                Case "5"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Geo Demos..." + vbNewLine)
                    'Await GeoDemos.RunAsync(twitterCtx)
                Case "6"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Help Demos..." + vbNewLine)
                    'Await HelpDemos.RunAsync(twitterCtx)
                Case "7"c
                    Console.WriteLine(vbNewLine + vbTab + "Running List Demos..." + vbNewLine)
                    'Await ListDemos.RunAsync(twitterCtx)
                Case "8"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Raw Demos..." + vbNewLine)
                    'Await RawDemos.RunAsync(twitterCtx)
                Case "9"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Saved Search Demos..." + vbNewLine)
                    'Await SavedSearchDemos.RunAsync(twitterCtx)
                Case "a"c
                Case "A"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Search Demos..." + vbNewLine)
                    'Await SearchDemos.RunAsync(twitterCtx)
                Case "b"c
                Case "B"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Status Demos..." + vbNewLine)
                    'Await StatusDemos.RunAsync(twitterCtx)
                Case "c"c
                Case "C"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Stream Demos..." + vbNewLine)
                    Await StreamDemos.RunAsync(twitterCtx)
                Case "d"c
                Case "D"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Trend Demos..." + vbNewLine)
                    'Await TrendDemos.RunAsync(twitterCtx)
                Case "e"c
                Case "E"c
                    Console.WriteLine(vbNewLine + vbTab + "Running User Demos..." + vbNewLine)
                    'Await UserDemos.RunAsync(twitterCtx)
                Case "f"c
                Case "F"c
                    Console.WriteLine(vbNewLine + vbTab + "Running Mutes Demos..." + vbNewLine)
                    'Await MuteDemos.RunAsync(twitterCtx)
                Case "q"c
                Case "Q"c
                    Console.WriteLine()
                    Console.WriteLine("Quitting..." + vbNewLine)
                Case Else
                    Console.WriteLine(key + " is unknown")

            End Select

        Loop While Char.ToUpper(key) <> "Q"c

        Return
    End Function

    Shared Sub ShowMenu()
        Console.WriteLine()
        Console.WriteLine("Please select category:")
        Console.WriteLine()

        Console.WriteLine(vbTab + " 0. Account Demos")
        Console.WriteLine(vbTab + " 1. Block Demos")
        Console.WriteLine(vbTab + " 2. Direct Message Demos")
        Console.WriteLine(vbTab + " 3. Favorite Demos")
        Console.WriteLine(vbTab + " 4. Friendship Demos")
        Console.WriteLine(vbTab + " 5. Geo Demos")
        Console.WriteLine(vbTab + " 6. Help Demos")
        Console.WriteLine(vbTab + " 7. List Demos")
        Console.WriteLine(vbTab + " 8. Raw Demos")
        Console.WriteLine(vbTab + " 9. Saved Search Demos")
        Console.WriteLine(vbTab + " A. Search Demos")
        Console.WriteLine(vbTab + " B. Status Demos")
        Console.WriteLine(vbTab + " C. Stream Demos")
        Console.WriteLine(vbTab + " D. Trend Demos")
        Console.WriteLine(vbTab + " E. User Demos")
        Console.WriteLine(vbTab + " F. Mutes Demos")
        Console.WriteLine()
        Console.WriteLine(vbTab + " Q. End Program")
    End Sub


    Shared Function ChooseAuthenticationStrategy() As IAuthorizer
        Console.WriteLine("Authentication Strategy:")
        Console.WriteLine()

        Console.WriteLine("  1 - Pin (default)")
        Console.WriteLine("  2 - Application-Only")
        Console.WriteLine("  3 - Single User")
        Console.WriteLine("  4 - XAuth")

        Console.WriteLine()
        Console.Write("Please choose (1, 2, 3, or 4): ")

        Dim input As ConsoleKeyInfo = Console.ReadKey()
        Console.WriteLine()

        Dim auth As IAuthorizer = Nothing

        Select Case input.Key

            Case ConsoleKey.D1
                auth = DoPinAuth()
            Case ConsoleKey.D2
                auth = DoApplicationOnlyAuth()
            Case ConsoleKey.D3
                auth = DoSingleUserAuth()
            Case ConsoleKey.D4
                auth = DoXAuth()
            Case Else
                auth = DoPinAuth()
        End Select

        Return auth
    End Function

    Shared Function DoPinAuth() As IAuthorizer

        Dim credentials As New InMemoryCredentialStore()
        credentials.ConsumerKey = ConfigurationManager.AppSettings("consumerKey")
        credentials.ConsumerSecret = ConfigurationManager.AppSettings("consumerSecret")

        Dim auth As New PinAuthorizer()
        auth.CredentialStore = credentials
        auth.GetPin =
            Function()
                Console.WriteLine()
                Console.WriteLine("After authorizing this application, Twitter will give you a 7-digit PIN Number.")
                Console.WriteLine()
                Console.Write("Enter the PIN number here: ")
                Return Console.ReadLine()
            End Function
        auth.GoToTwitterAuthorization = Function(pageLink) Process.Start(pageLink)

        Return auth

    End Function

    Shared Function DoApplicationOnlyAuth() As IAuthorizer

        Dim credentials As New InMemoryCredentialStore()
        credentials.ConsumerKey = ConfigurationManager.AppSettings("consumerKey")
        credentials.ConsumerSecret = ConfigurationManager.AppSettings("consumerSecret")

        Dim auth As New ApplicationOnlyAuthorizer()
        auth.CredentialStore = credentials

        Return auth

    End Function

    Shared Function DoSingleUserAuth() As IAuthorizer

        Dim credentials As New SingleUserInMemoryCredentialStore()
        credentials.ConsumerKey = ConfigurationManager.AppSettings("consumerKey")
        credentials.ConsumerSecret = ConfigurationManager.AppSettings("consumerSecret")
        credentials.AccessToken = ConfigurationManager.AppSettings("accessToken")
        credentials.AccessTokenSecret = ConfigurationManager.AppSettings("accessTokenSecret")

        Dim auth As New SingleUserAuthorizer()
        auth.CredentialStore = credentials

        Return auth

    End Function

    Shared Function DoXAuth() As IAuthorizer

        Dim credentials As New XAuthCredentials()
        credentials.ConsumerKey = ConfigurationManager.AppSettings("consumerKey")
        credentials.ConsumerSecret = ConfigurationManager.AppSettings("consumerSecret")
        credentials.UserName = "YourUserName"
        credentials.Password = "YourPassword"

        Dim auth As New XAuthAuthorizer()
        auth.CredentialStore = credentials

        Return auth

    End Function

End Class
