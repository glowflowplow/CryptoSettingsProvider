Imports CryptoSettingsProvider

<TestClass()> Public Class CryptorTest

    ' 暗号化出来ていることを確認する
    ' 暗号化して同一でなくなっていることを確認
    ' 復号できることを確認
    ' キーを変えると複合できないことを確認
    ' 初期化ベクトルを変えると複合できないことを確認

    <TestMethod>
    Public Sub CanEncryptAndDecrypt()
        Dim Cryptor As Cryptor = New Cryptor()
        Dim ByteArray As Byte() = {10, 20, 30, 40, 255}
        Dim Encrypted = Cryptor.Encrypt(ByteArray)
        Dim Decrypted = Cryptor.Decrypt(Cryptor.IV, Encrypted)
        CollectionAssert.AreNotEqual(ByteArray, Encrypted)
        CollectionAssert.AreEqual(ByteArray, Decrypted)
    End Sub

    <TestMethod>
    Public Sub DefferenceKey_CannotDecrypt()
        Dim Cryptor As Cryptor = New Cryptor()
        Dim ByteArray As Byte() = {10, 20, 30, 40, 255}
        Dim Encrypted = Cryptor.Encrypt(ByteArray)
        ' Change Key
        Dim Key = Cryptor.Key
        Key(0) = (Key(0) + 1) Mod 256
        Cryptor.Key = Key
        Dim Action As Action = Sub() Cryptor.Decrypt(Key, Encrypted)
        CollectionAssert.AreNotEqual(ByteArray, Encrypted)
        Assert.ThrowsException(Of Security.Cryptography.CryptographicException)(Action, "")
    End Sub

    <TestMethod>
    Public Sub DefferenceIV_CannotDecrypt()
        Dim Cryptor As New Cryptor()
        Dim ByteArray As Byte() = {10, 20, 30, 40, 255}
        Dim Encrypted = Cryptor.Encrypt(ByteArray)
        ' Change IV
        Dim IV = Cryptor.IV
        IV(0) = (IV(0) + 1) Mod 256
        Dim Decrypted = Cryptor.Decrypt(IV, Encrypted)
        CollectionAssert.AreNotEqual(ByteArray, Encrypted)
        CollectionAssert.AreNotEqual(ByteArray, Decrypted)
    End Sub
End Class