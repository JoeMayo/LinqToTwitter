Imports System.IO
Imports System.Threading
Imports LinqToTwitter

Friend Class StreamDemos

    Friend Shared Async Function HandleStreamResponse(strm As StreamContent) As Task(Of Integer)
        If strm.HasError Then
            Console.WriteLine($"Error during streaming: {strm.ErrorMessage}")
        Else
            Dim tweet = strm.Entity.Tweet
            If tweet IsNot Nothing Then
                Console.WriteLine($"\nTweet ID: {tweet.ID}, Tweet Text: {tweet.Text}")
            End If
        End If
        Return Await Task.FromResult(0)
    End Function

    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Console.WriteLine()
        Console.WriteLine("Streamed Content:")
        Console.WriteLine()

        Dim count = 0
        Dim cancelTokenSrc = New CancellationTokenSource()

        Try
            Await _
            (From strm In twitterCtx.Streaming _
                                    .WithCancellation(cancelTokenSrc.Token)
             Where strm.Type = StreamingType.Sample
             Select strm) _
            .StartAsync(
                Async Function(strm)
                    Await HandleStreamResponse(strm)

                    count += 1

                    If count >= 10 Then
                        cancelTokenSrc.Cancel()
                    End If
                End Function)
        Catch ex As IOException
            ' Twitter might have closed the stream,
            ' which they do sometimes. You should
            ' restart the stream, but be sure to
            ' read Twitter documentation on stream
            ' back-off strategies to prevent your
            ' app from being blocked.
            Console.WriteLine(ex.ToString())
        Catch ocEx As OperationCanceledException
            Console.WriteLine("Stream cancelled.")
        End Try
    End Function
End Class
