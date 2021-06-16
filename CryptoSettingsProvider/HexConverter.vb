<Assembly: Runtime.CompilerServices.InternalsVisibleTo("Tests")>
''' <summary>
''' 16進表記の文字列とByte型配列の変換をする
''' </summary>
Friend Class HexConverter
    ''' <summary>
    ''' 16進文字列からByte型配列へ変換する
    ''' </summary>
    ''' <param name="HexString">16進文字列</param>
    ''' <returns>Byte型配列</returns>
    Public Shared Function ToByteArray(HexString As String) As Byte()
        If HexString Is Nothing Then
            Throw New ArgumentNullException(NameOf(HexString))
        End If

        If HexString.Length Mod 2 <> 0 Then
            Throw New ArgumentException(NameOf(HexString))
        End If

        Dim ByteList = New List(Of Byte)
        For i = 0 To HexString.Length - 1 Step 2
            Dim s = HexString.Substring(i, 2)
            ByteList.Add(Convert.ToByte(s, 16))
        Next
        Return ByteList.ToArray()
    End Function

    ''' <summary>
    ''' Byte型配列から16進文字列へ変換する
    ''' </summary>
    ''' <param name="ByteArray">Byte型配列</param>
    ''' <returns>16進文字列</returns>
    Public Shared Function ToHexString(ByteArray As Byte()) As String
        If ByteArray Is Nothing Then
            Throw New ArgumentNullException(NameOf(ByteArray))
        End If

        Dim sb = New Text.StringBuilder()
        For i = 0 To ByteArray.Length - 1
            sb.Append(Conversion.Hex(ByteArray(i)).PadLeft(2, "0"c))
        Next
        Return sb.ToString()
    End Function
End Class
