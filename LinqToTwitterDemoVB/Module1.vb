Imports System.Configuration
Imports LinqToTwitter

Module Module1

    Sub Main()
        '
        ' get user credentials and instantiate TwitterContext
        '
        Dim auth As ITwitterAuthorization

        If String.IsNullOrEmpty(ConfigurationManager.AppSettings("twitterConsumerKey")) Or String.IsNullOrEmpty(ConfigurationManager.AppSettings("twitterConsumerSecret")) Then
            Console.WriteLine("Skipping OAuth authorization demo because twitterConsumerKey and/or twitterConsumerSecret are not set in your .config file.")
            Console.WriteLine("Using username/password authorization instead.")

            ' For username/password authorization demo...
            auth = New UsernamePasswordAuthorization(Utilities.GetConsoleHWnd())
        Else
            Console.WriteLine("Discovered Twitter OAuth consumer key in .config file.  Using OAuth authorization.")

            ' For OAuth authorization demo...
            auth = New DesktopOAuthAuthorization()
        End If

        Dim twitterCtx As TwitterContext
        twitterCtx = New TwitterContext(auth, "https://twitter.com/", "http://search.twitter.com/")

        If twitterCtx.AuthorizedClient.GetType() Is GetType(OAuthAuthorization) Then
            InitializeOAuthConsumerStrings(twitterCtx)
        End If
        auth.SignOn()

        Dim twitterContext = New TwitterContext(auth)

        MentionsDemo(twitterContext)

        Console.ReadKey()
    End Sub

    Private Sub InitializeOAuthConsumerStrings(ByVal twitterCtx As TwitterContext)
        Dim oauth As DesktopOAuthAuthorization
        oauth = CType(twitterCtx.AuthorizedClient, DesktopOAuthAuthorization)
        oauth.GetVerifier = AddressOf VerifierCallback
    End Sub

    Private Function VerifierCallback() As String
        Console.WriteLine("Next, you'll need to tell Twitter to authorize access.\nThis program will not have access to your credentials, which is the benefit of OAuth.\nOnce you log into Twitter and give this program permission,\n come back to this console.")
        Console.Write("Please enter the PIN that Twitter gives you after authorizing this client: ")
        Return Console.ReadLine()
    End Function

    Private Sub MentionsDemo(ByVal twitterCtx As TwitterContext)
        Dim ts = From t In twitterCtx.Status _
                 Where t.Type = StatusType.Mentions

        For Each mention In ts
            Console.WriteLine("Mention: " & mention.Text)
        Next
    End Sub
End Module
