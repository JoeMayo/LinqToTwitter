Imports System.Configuration
Imports LinqToTwitter

Module Module1

    Sub Main()
        '
        ' get user credentials and instantiate TwitterContext
        '

        If String.IsNullOrEmpty(ConfigurationManager.AppSettings("twitterConsumerKey")) Or String.IsNullOrEmpty(ConfigurationManager.AppSettings("twitterConsumerSecret")) Then
			Console.WriteLine("Please set the Twitter consumer key and secret values in the app.config file and run again.")
			Exit Sub
		End If

        Dim credentials As IOAuthCredentials = New InMemoryCredentials

        credentials.ConsumerKey = ConfigurationManager.AppSettings("twitterConsumerKey")
        credentials.ConsumerSecret = ConfigurationManager.AppSettings("twitterConsumerSecret")

        Dim auth As PinAuthorizer = New PinAuthorizer()
        auth.Credentials = credentials
        auth.GetPin = AddressOf VerifierCallback
        auth.GoToTwitterAuthorization = Function(pageLink) Process.Start(pageLink)
        auth.Authorize()

        Dim twitterCtx As TwitterContext = New TwitterContext(auth)

		twitterCtx.Log = Console.Out

        'auth.SignOn()

		'Dim twitterContext = New TwitterContext(auth)

		'
		' Status Demos
		'

        'UpdateStatusDemo(twitterCtx)
		'UpdateStatusWithReplyDemo(twitterCtx)
		'DestroyStatusDemo(twitterCtx)
		'MentionsDemo(twitterCtx)
		'SingleStatusQueryDemo(twitterCtx)
        'FriendsStatusQueryDemo(twitterCtx)
		'UserStatusByNameDemo(twitterCtx)
		'UserStatusByQueryDemo(twitterCtx)
		'FirstStatusByQueryDemo(twitterCtx)
        'PublicStatusQueryDemo(twitterCtx)
		'PublicStatusFilteredQueryDemo(twitterCtx)

		'
		' User Demos
		'
		'UserShowWithIDQueryDemo(twitterCtx)
		'UserShowWithScreenNameQueryDemo(twitterCtx)
		'UserShowLoggedInUserQueryDemo(twitterCtx)

		'
		' Direct Message Demos
		'

        'DirectMessageSentByQueryDemo(twitterCtx)
        'NewDirectMessageDemo(twitterCtx)

		'
		' Friendship Demos
		'

		'FriendshipExistsDemo(twitterCtx)

		'
		' Social Graph Demos
		'

		'ShowFriendsDemo(twitterCtx)

		'
		' Search Demos
		'

        SearchTwitterDemo(twitterCtx)

		'
		' Favorites Demos
		'

		'FavoritesQueryDemo(twitterCtx)

		'
		' Notifications Demos
		'

		'EnableNotificationsDemo(twitterCtx)
		'DisableNotificationsDemo(twitterCtx)

		'
		' Block Demos
		'

		'BlockIDsDemo(twitterCtx)

		'
		' Help Demos
		'

		'PerformHelpTest(twitterCtx)

		'
		' Account Demos
		'

		'VerifyAccountCredentialsDemo(twitterCtx)

		'
		' Trends Demos
		'

		'SearchCurrentTrendsDemo(twitterCtx)

		'
		' OAuth Demos
		'

		'HandleOAuthQueryDemo(twitterCtx)

		'
		' Saved Searches Demos
		'

		'QuerySavedSearchesDemo(twitterCtx)

		Console.ReadKey()
    End Sub

    Private Function VerifierCallback() As String
        Console.WriteLine("Next, you'll need to tell Twitter to authorize access.\nThis program will not have access to your credentials, which is the benefit of OAuth.\nOnce you log into Twitter and give this program permission,\n come back to this console.")
        Console.Write("Please enter the PIN that Twitter gives you after authorizing this client: ")
        Return Console.ReadLine()
    End Function

    Private Sub QuerySavedSearchesDemo(ByVal twitterCtx As TwitterContext)
        Dim savedSearches = _
            From search In twitterCtx.SavedSearch _
            Where search.Type = SavedSearchType.Searches

        For Each Search In savedSearches
            Console.WriteLine("ID: {0}, Search: {1}", Search.ID, Search.Name)
        Next
    End Sub

    Private Sub HandleOAuthQueryDemo(ByVal twitterCtx As TwitterContext)

        If twitterCtx.AuthorizedClient.IsAuthorized Then
            Dim tweets = _
                From tweet In twitterCtx.Status _
                Where tweet.Type = StatusType.Show

            For Each tweet In tweets
                Console.WriteLine( _
                    "Friend: {0}, Created: {1}\r\nTweet: {2}", _
                    tweet.User.Name, _
                    tweet.CreatedAt, _
                    tweet.Text)
            Next
        End If

    End Sub

    Private Sub VerifyAccountCredentialsDemo(ByVal twitterCtx As TwitterContext)
        Dim accounts = _
            From acct In twitterCtx.Account _
            Where acct.Type = AccountType.VerifyCredentials

        Try
            For Each Account In accounts
                Console.WriteLine("Credentials for account, {0}, are okay.", Account.User.Name)
            Next
        Catch ex As Exception
            Console.WriteLine("Twitter did not recognize the credentials. Response from Twitter: " & ex.Message)
        End Try
    End Sub

    Private Sub BlockIDsDemo(ByVal twitterCtx As TwitterContext)
        Dim result = _
            From blockItem In twitterCtx.Blocks _
            Where blockItem.Type = BlockingType.Ids


        If result.ToList().Count = 0 Then
            Console.WriteLine("No Blocks")
        End If

        For Each block In result
            Console.WriteLine("ID: {0}", block.UserID)
        Next
    End Sub

    Private Sub FavoritesQueryDemo(ByVal twitterCtx As TwitterContext)
        Dim favorites = _
            From fav In twitterCtx.Favorites _
            Where fav.Type = FavoritesType.Favorites

        For Each fav In favorites
            Console.WriteLine( _
                "User Name: {0}, Tweet: {1}", _
                fav.User.Name, fav.Text)
        Next
    End Sub

    Private Sub SearchTwitterDemo(ByVal twitterCtx As TwitterContext)
        Dim queryResults = _
            From search In twitterCtx.Search _
            Where search.Type = SearchType.Search _
            And search.Query = "LINQ to Twitter"

        Console.WriteLine("Query: " & Environment.NewLine)

        For Each searchResult In queryResults
            For Each entry In searchResult.Statuses
                Console.WriteLine( _
                    "ID: {0}, Source: {1} - Content: {2}" & Environment.NewLine, _
                    entry.ID, entry.Source, entry.Text)
            Next
        Next
    End Sub

    Private Sub ShowFriendsDemo(ByVal twitterCtx As TwitterContext)
        Dim friends = _
            (From graph In twitterCtx.SocialGraph _
             Where graph.Type = SocialGraphType.Friends _
             And graph.ScreenName = "LinqToTweeter").ToList()

        For Each frnd In friends
            Console.WriteLine("Friend ID: " & frnd.UserID)
        Next
    End Sub

    Private Sub DirectMessageSentByQueryDemo(ByVal twitterContext As TwitterContext)
        Dim directMessages = _
            From dm In twitterContext.DirectMessage _
            Where dm.Type = DirectMessageType.SentBy _
            And dm.Count = 2

        For Each dm In directMessages
            Console.WriteLine( _
                "Sender: {0}, Tweet: {1}", _
                dm.SenderScreenName, _
                dm.Text)
        Next
    End Sub

    Private Sub UserShowWithIDQueryDemo(ByVal twitterCtx As TwitterContext)
        Dim users = _
            From user In twitterCtx.User _
            Where user.Type = UserType.Show _
            And user.ID = "15411837"

        Dim singleUser As User = users.SingleOrDefault()

        Console.WriteLine( _
            "Name: {0}, Last Tweet: {1}" & Environment.NewLine, _
            singleUser.Name, singleUser.Status.Text)
    End Sub

    Private Sub UserShowWithScreenNameQueryDemo(ByVal twitterCtx As TwitterContext)
        Dim users = _
            From user In twitterCtx.User _
            Where user.Type = UserType.Show _
            And user.ScreenName = "JoeMayo"
        Dim singleUser As User = users.SingleOrDefault()

        Console.WriteLine( _
            "Name: {0}, Last Tweet: {1}" & Environment.NewLine, _
            singleUser.Name, singleUser.Status.Text)
    End Sub

    Private Sub UserShowLoggedInUserQueryDemo(ByVal twitterCtx As TwitterContext)
        Dim users = _
            From usr In twitterCtx.User _
            Where usr.Type = UserType.Show _
                AndAlso usr.ScreenName = twitterCtx.UserName _
            Select usr
        Dim singleUser As User = users.SingleOrDefault()

        Console.WriteLine( _
            "Name: {0}, Last Tweet: {1}" & Environment.NewLine, _
            singleUser.Name, singleUser.Status.Text)
    End Sub

    Private Sub FriendsStatusQueryDemo(ByVal twitterCtx As TwitterContext)
        Dim ts = From t In twitterCtx.Status _
                 Where t.Type = StatusType.Show

        For Each tweet In ts
            Console.WriteLine("Friend: " & tweet.User.Name & Environment.NewLine & _
                              "Tweet: " & tweet.Text & Environment.NewLine)
        Next
    End Sub

    Private Sub SingleStatusQueryDemo(ByVal twitterCtx As TwitterContext)
        Dim ts = From t In twitterCtx.Status _
                 Where t.Type = StatusType.Show _
                 And t.ID = "2534357295"

        For Each mention In ts
            Console.WriteLine("Mention: " & mention.Text)
        Next
    End Sub

    Private Sub MentionsDemo(ByVal twitterCtx As TwitterContext)
        Dim ts = From t In twitterCtx.Status _
                 Where t.Type = StatusType.Mentions

        For Each mention In ts
            Console.WriteLine("Mention: " & mention.Text)
        Next
    End Sub

    Private Sub UserStatusByNameDemo(ByVal twitterCtx As TwitterContext)
        Dim lastN As Integer = 20
        Dim screenName As String = "JoeMayo"

        Dim ts = From t In twitterCtx.Status _
                 Where t.Type = StatusType.User _
                 And t.ScreenName = screenName _
                 And t.Count = lastN

        For Each tweet In ts
            Console.WriteLine( _
                "(" & tweet.ID & ")" & _
                "[" & tweet.User.ID & "]" & _
                tweet.User.Name & ", " & _
                tweet.Text & ", " & _
                tweet.CreatedAt)
        Next
    End Sub

    Private Sub UserStatusByQueryDemo(ByVal twitterCtx As TwitterContext)
        Console.WriteLine()

        Dim statusTweets = From tweet In twitterCtx.Status _
                 Where tweet.Type = StatusType.User _
                 And tweet.ID = "15411837" _
                 And tweet.Page = 1 _
                 And tweet.Count = 20 _
                 And tweet.SinceID = 4444196804

        For Each tweet In statusTweets
            Console.WriteLine( _
                "(" & tweet.ID & ")" & _
                "[" & tweet.User.ID & "]" & _
                tweet.User.Name & ", " & _
                tweet.Text & ", " & _
                tweet.CreatedAt)
        Next
    End Sub

    Private Sub FirstStatusByQueryDemo(ByVal twitterCtx As TwitterContext)
        Console.WriteLine()

        Dim statusTweets = From tweet In twitterCtx.Status _
                 Where tweet.Type = StatusType.User _
                 And tweet.ID = "15411837" _
                 And tweet.Page = 1 _
                 And tweet.Count = 20 _
                 And tweet.SinceID = 4444196804

        Dim status As Status = statusTweets.FirstOrDefault()

        Console.WriteLine( _
            "(" & status.ID & ")" & _
            "[" & status.User.ID & "]" & _
            status.User.Name & ", " & _
            status.Text & ", " & _
            status.CreatedAt)
    End Sub


    Private Sub UpdateStatusDemo(ByVal twitterCtx As TwitterContext)
        Dim statusMsg As String = "\u00C7 Testing LINQ to Twitter update status (with VB) on " & DateTime.Now.ToString() & " #linqtotwitter"

        Console.WriteLine("Status being sent: " & statusMsg)

        Dim tweet = twitterCtx.UpdateStatus(statusMsg)

        Console.WriteLine( _
            "Status returned: " & _
            "(" & tweet.ID & ")" & _
            "[" & tweet.User.ID & "]" & _
            tweet.User.Name & ", " & _
            tweet.Text & ", " & _
            tweet.CreatedAt)
    End Sub

    Private Sub UpdateStatusWithReplyDemo(ByVal twitterCtx As TwitterContext)
        Dim statusMsg As String = "@LinqToTweeter Testing LINQ to Twitter (with VB) with reply on " & DateTime.Now.ToString() & " #linqtotwitter"

        Console.WriteLine("Status being sent: " & statusMsg)

        Dim tweet = twitterCtx.UpdateStatus(statusMsg)

        Console.WriteLine( _
            "Status returned: " & _
            "(" & tweet.ID & ")" & _
            "[" & tweet.User.ID & "]" & _
            tweet.User.Name & ", " & _
            tweet.Text & ", " & _
            tweet.CreatedAt)
    End Sub

    Private Sub DestroyStatusDemo(ByVal twitterCtx As TwitterContext)

        Dim tweet = twitterCtx.DestroyStatus("4491132999")

        Console.WriteLine( _
            "Deleted Status: " & _
            "(" & tweet.ID & ")" & _
            "[" & tweet.User.ID & "]" & _
            tweet.User.Name & ", " & _
            tweet.Text & ", " & _
            tweet.CreatedAt)
    End Sub

    Private Sub NewDirectMessageDemo(twitterCtx As TwitterContext)
        Dim message = twitterCtx.NewDirectMessage("16761255", "Direct Message VB Test - " + DateTime.Now + "!'")

        Console.WriteLine(
            "Recipient: {0}, Message: {1}, Date: {2}",
            message.RecipientScreenName,
            message.Text,
            message.CreatedAt)

    End Sub

End Module
