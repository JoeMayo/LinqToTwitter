Imports LinqToTwitter
 
Class FriendshipDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task

        Dim key As Char
        Do
            ShowMenu()

            key = Console.ReadKey(True).KeyChar

            Select Case key

                Case "0"c
                    Console.WriteLine(vbNewLine + vbTab + "Showing friends..." + vbNewLine)
                Case "1"c
                    Console.WriteLine(vbNewLine + vbTab + "Looking up user ids..." + vbNewLine)
                Case "2"c
                    Console.WriteLine(vbNewLine + vbTab + "Getting incoming..." + vbNewLine)
                Case "3"c
                    Console.WriteLine(vbNewLine + vbTab + "Getting Outgoing..." + vbNewLine)
                Case "4"c
                    Console.WriteLine(vbNewLine + vbTab + "Showing no retweet IDs..." + vbNewLine)
                Case "5"c
                    Console.WriteLine(vbNewLine + vbTab + "Getting friends list..." + vbNewLine)
                    Await ShowFriendsListAsync(twitterCtx)
                Case "6"c
                    Console.WriteLine(vbNewLine + vbTab + "Getting followers list..." + vbNewLine)
                Case "7"c
                    Console.WriteLine(vbNewLine + vbTab + "Showing followers ids..." + vbNewLine)
                Case "8"c
                    Console.WriteLine(vbNewLine + vbTab + "Showing friend ids..." + vbNewLine)
                Case "9"c
                    Console.WriteLine(vbNewLine + vbTab + "Creating friendship..." + vbNewLine)
                Case "a"c
                Case "A"c
                    Console.WriteLine(vbNewLine + vbTab + "Unfollowing..." + vbNewLine)
                Case "b"c
                Case "B"c
                    Console.WriteLine(vbNewLine + vbTab + "Updating friend settings..." + vbNewLine)
                Case "q"c
                Case "Q"c
                    Console.WriteLine()
                    Console.WriteLine("Returning..." + vbNewLine)
                    Return
                Case Else
                    Console.WriteLine(key + " is unknown")

            End Select

        Loop While Char.ToUpper(key) <> "Q"c

    End Function

    Shared Sub ShowMenu()
        Console.WriteLine()
        Console.WriteLine("Friendship Demos - Please select:")
        Console.WriteLine()

        Console.WriteLine(vbTab + " 0. Show Friends")
        Console.WriteLine(vbTab + " 1. Lookup Friendships")
        Console.WriteLine(vbTab + " 2. Incoming Friendships")
        Console.WriteLine(vbTab + " 3. Outgoing Friendships")
        Console.WriteLine(vbTab + " 4. No Retweet IDs")
        Console.WriteLine(vbTab + " 5. Friends List")
        Console.WriteLine(vbTab + " 6. Followers List")
        Console.WriteLine(vbTab + " 7. Follower IDs")
        Console.WriteLine(vbTab + " 8. Friend IDs")
        Console.WriteLine(vbTab + " 9. Create Friendship")
        Console.WriteLine(vbTab + " A. Delete Friendship")
        Console.WriteLine(vbTab + " B. Update Freindship Settings")
        Console.WriteLine()
        Console.WriteLine(vbTab + " Q. Return to Main menu")
    End Sub

    Shared Async Function ShowFriendsListAsync(twitterCtx As TwitterContext) As Task

        Dim cursor As Long = -1

        Do
            Dim friendship = Await (From [friend] In twitterCtx.Friendship Where _
                                 [friend].Type = FriendshipType.FriendsList AndAlso _
                                 [friend].ScreenName = "JoeMayo" AndAlso _
                                 [friend].Cursor = cursor _
                                 Select [friend]).SingleOrDefaultAsync()

            If Not friendship Is Nothing AndAlso _
               Not friendship.Users Is Nothing AndAlso
               Not friendship.CursorMovement Is Nothing Then

                cursor = friendship.CursorMovement.Next

                For Each twitterFriend As User In friendship.Users
                    Console.WriteLine("ID: {0} Name: {1}", twitterFriend.UserIDResponse, twitterFriend.ScreenNameResponse)
                Next

            End If

        Loop While cursor <> 0

    End Function


End Class
