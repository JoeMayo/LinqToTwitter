Imports LinqToTwitter

Friend Class FriendshipDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim Friendship =
            Await _
            (From frnd In twitterCtx.Friendship
             Where frnd.Type = FriendshipType.Show AndAlso
             frnd.SourceScreenName = "Linq2Twitr" AndAlso
             frnd.TargetScreenName = "JoeMayo"
             Select frnd) _
            .SingleOrDefaultAsync()

        If Friendship IsNot Nothing AndAlso
            Friendship.SourceRelationship IsNot Nothing AndAlso
            Friendship.TargetRelationship IsNot Nothing Then

            Console.WriteLine(
                "JoeMayo follows LinqToTweeter: " +
                Friendship.SourceRelationship.FollowedBy.ToString() +
                " - LinqToTweeter follows JoeMayo: " +
                Friendship.TargetRelationship.FollowedBy.ToString())
        End If
    End Function
End Class
