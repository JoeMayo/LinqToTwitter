Imports LinqToTwitter
 
Class StreamDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task

        Dim key As Char
        Do
            ShowMenu()

            key = Console.ReadKey(True).KeyChar

            Select Case key

                Case "0"c
                    Console.WriteLine(vbNewLine + vbTab + "Showing Filter Stream..." + vbNewLine)
                Case "1"c
                    Console.WriteLine(vbNewLine + vbTab + "Showing Sample Stream..." + vbNewLine)
                Case "2"c
                    Console.WriteLine(vbNewLine + vbTab + "Showing User Stream..." + vbNewLine)
                Case "3"c
                    Console.WriteLine(vbNewLine + vbTab + "Showing Site Stream..." + vbNewLine)
                Case "4"c
                    Console.WriteLine(vbNewLine + vbTab + "Using Control Stream..." + vbNewLine)
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
        Console.WriteLine("Streaming Demos - Please select:")
        Console.WriteLine()

        Console.WriteLine(vbTab + " 0. Filter Stream")
        Console.WriteLine(vbTab + " 1. Sample Stream")
        Console.WriteLine(vbTab + " 2. User Stream")
        Console.WriteLine(vbTab + " 3. Site Stream")
        Console.WriteLine(vbTab + " 4. Control Stream")
        Console.WriteLine()
        Console.WriteLine(vbTab + " Q. Return to Main menu")
    End Sub

    Shared Async Function DoFilterStreamAsync(twitterCtx As TwitterContext) As Task

        Console.WriteLine()
        Console.WriteLine("Streamed Content:")
        Console.WriteLine()

        Dim count As Integer = 0

        Await _
            (From strm In twitterCtx.Streaming _
             Where strm.Type = StreamingType.Filter And _
             strm.Track = "twitter") _
            .StartAsync( _
                Async Function(strm)
                    Console.WriteLine(strm.Content + vbLf)
                    If System.Math.Max(System.Threading.Interlocked.Increment(count), count - 1) >= 5 Then
                        strm.CloseStream()
                    End If
                    Return
                End Function)

    End Function


End Class
