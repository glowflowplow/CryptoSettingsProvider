Imports CryptoSettingsProvider

<TestClass()>
Public Class HexConverterTest
    ' バイト配列を16進表記に変換するメソッドのテスト
    ' バイト配列が空の場合は空の文字列を返す
    ' バイト配列がNothingの場合はぬるぽ
    ' バイト配列の要素が16未満の場合、先頭に0をつける(15->0F)

    ' 16進表記文字列をバイト配列に変換するメソッドのテスト
    ' 文字列長が偶数のの場合正常に変換できる
    ' 文字列長が奇数の場合例外が投げられる
    ' 文字列が16進表記でない場合例外が投げられる
    ' 文字列が空の場合例外が投げられる
    ' 文字列が空文字の場合例外が投げられる

    <TestCategory("ToHexString")>
    <TestMethod>
    Public Sub ToHexString_FromCommonByteArray()
        Dim Arr As Byte() = {0, 1, 2, 4, 8, 16, 32, 64, 128, 255}
        Dim Excepted = "000102040810204080FF"
        Dim Result = HexConverter.ToHexString(Arr)
        Assert.AreEqual(Excepted, Result)
    End Sub

    <TestCategory("ToHexString")>
    <TestMethod>
    Public Sub ToHexString_FromNullByteArray_ThrownException()
        Dim Arr As Byte() = Nothing
        Dim Action As Func(Of Object) = Function() HexConverter.ToHexString(Arr)
        Dim Message = "Method should have Thrown ArgumentNullException"
        Assert.ThrowsException(Of ArgumentNullException)(Action, Message)
    End Sub

    <TestCategory("ToHexString")>
    <TestMethod>
    Public Sub ToHexString_FromEmptyByteArray_ReturnedEmptyString()
        Dim Arr As Byte() = Array.Empty(Of Byte)
        Dim Result = HexConverter.ToHexString(Arr)
        Dim Excepted = String.Empty
        Dim Message = "Method should have returned Empty String"
        Assert.AreEqual(Excepted, Result, Message)
    End Sub

    <TestCategory("ToHexString")>
    <TestMethod>
    Public Sub ToHexString_FromSingleArray()
        Dim Arr As Byte() = {23}
        Dim Result = HexConverter.ToHexString(Arr)
        Assert.AreEqual("17", Result, "Method could not have returned correctlly value by single element array")
    End Sub

    <TestCategory("ToHexString")>
    <TestMethod>
    Public Sub ToHexString_FromAllByteNumber()
        Dim Arr As Byte() = {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
            32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
            48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63,
            64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79,
            80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95,
            96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
            112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127,
            128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143,
            144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
            160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175,
            176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191,
            192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
            208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223,
            224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
            240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255
        }
        Dim Hex = HexConverter.ToHexString(Arr)
        Dim Str =
            "000102030405060708090A0B0C0D0E0F" &
            "101112131415161718191A1B1C1D1E1F" &
            "202122232425262728292A2B2C2D2E2F" &
            "303132333435363738393A3B3C3D3E3F" &
            "404142434445464748494A4B4C4D4E4F" &
            "505152535455565758595A5B5C5D5E5F" &
            "606162636465666768696A6B6C6D6E6F" &
            "707172737475767778797A7B7C7D7E7F" &
            "808182838485868788898A8B8C8D8E8F" &
            "909192939495969798999A9B9C9D9E9F" &
            "A0A1A2A3A4A5A6A7A8A9AAABACADAEAF" &
            "B0B1B2B3B4B5B6B7B8B9BABBBCBDBEBF" &
            "C0C1C2C3C4C5C6C7C8C9CACBCCCDCECF" &
            "D0D1D2D3D4D5D6D7D8D9DADBDCDDDEDF" &
            "E0E1E2E3E4E5E6E7E8E9EAEBECEDEEEF" &
            "F0F1F2F3F4F5F6F7F8F9FAFBFCFDFEFF"
        Assert.AreEqual(Hex, Str, "Method could not have returned correctlly value by all byte numbers")
    End Sub

    <TestCategory("ToByteArray")>
    <TestMethod>
    Public Sub ToByteArray_FromCommonHexString()
        Dim Hex As String = "1111"
        Dim ExceptedArray As Byte() = {17, 17}
        Dim Result As Byte() = HexConverter.ToByteArray(Hex)
        CollectionAssert.AreEqual(ExceptedArray, Result)
    End Sub

    <TestCategory("ToByteArray")>
    <TestMethod>
    Public Sub ToByteArray_FromNullString_ThrownException()
        Dim Hex As String = Nothing
        Dim Action As Action = Sub() HexConverter.ToByteArray(Hex)
        Assert.ThrowsException(Of ArgumentNullException)(Action, "Method should have thrown ArgumentNullException")
    End Sub

    <TestCategory("ToByteArray")>
    <TestMethod>
    Public Sub ToByteArray_FromEmptyString_ReturnedEmptyString()
        Dim Hex As String = String.Empty
        Dim ExceptedArray As Byte() = Array.Empty(Of Byte)
        Dim Result As Byte() = HexConverter.ToByteArray(Hex)
        CollectionAssert.AreEqual(ExceptedArray, Result, "Method should have returned empty byte array")
    End Sub

    <TestCategory("ToByteArray")>
    <TestMethod>
    Public Sub ToByteArray_FromOddLengthString_ThrownException()
        Dim Hex As String = "AAA"
        Dim Action As Action = Sub() HexConverter.ToByteArray(Hex)
        Assert.ThrowsException(Of ArgumentException)(Action, "Method should have thrown ArgumentException")
    End Sub

    <TestCategory("ToByteArray")>
    <TestMethod>
    Public Sub ToByteArray_FromSingleElementArray()
        Dim Hex As String = "0A"
        Dim ExceptedArray As Byte() = {10}
        Dim Result As Byte() = HexConverter.ToByteArray(Hex)
        CollectionAssert.AreEqual(ExceptedArray, Result)
    End Sub

    <TestCategory("ToByteArray")>
    <TestMethod>
    Public Sub ToByteArray_From()
        Dim Hex =
            "000102030405060708090A0B0C0D0E0F" &
            "101112131415161718191A1B1C1D1E1F" &
            "202122232425262728292A2B2C2D2E2F" &
            "303132333435363738393A3B3C3D3E3F" &
            "404142434445464748494A4B4C4D4E4F" &
            "505152535455565758595A5B5C5D5E5F" &
            "606162636465666768696A6B6C6D6E6F" &
            "707172737475767778797A7B7C7D7E7F" &
            "808182838485868788898A8B8C8D8E8F" &
            "909192939495969798999A9B9C9D9E9F" &
            "A0A1A2A3A4A5A6A7A8A9AAABACADAEAF" &
            "B0B1B2B3B4B5B6B7B8B9BABBBCBDBEBF" &
            "C0C1C2C3C4C5C6C7C8C9CACBCCCDCECF" &
            "D0D1D2D3D4D5D6D7D8D9DADBDCDDDEDF" &
            "E0E1E2E3E4E5E6E7E8E9EAEBECEDEEEF" &
            "F0F1F2F3F4F5F6F7F8F9FAFBFCFDFEFF"
        Dim ExceptedArray As Byte() = {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
            32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
            48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63,
            64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79,
            80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95,
            96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
            112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127,
            128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143,
            144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
            160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175,
            176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191,
            192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
            208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223,
            224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
            240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255
        }
        Dim Result = HexConverter.ToByteArray(Hex)
        CollectionAssert.AreEqual(ExceptedArray, Result)
    End Sub

    <TestCategory("MutualConversion")>
    <TestMethod>
    Public Sub ToHexStringToByteArray()
        Dim ByteArray As Byte() = {10, 100, 50, 23}
        Dim Hexed = HexConverter.ToHexString(ByteArray)
        Dim Reversed = HexConverter.ToByteArray(Hexed)
        CollectionAssert.AreEqual(ByteArray, Reversed)
    End Sub

    <TestCategory("MutualConversion")>
    <TestMethod>
    Public Sub ToByteArrayToHexString()
        Dim HexString = "000011AB35"
        Dim ByteArray = HexConverter.ToByteArray(HexString)
        Dim Reversed = HexConverter.ToHexString(ByteArray)
        Assert.AreEqual(HexString, Reversed)
    End Sub
End Class