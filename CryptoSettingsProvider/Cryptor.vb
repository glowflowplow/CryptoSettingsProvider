Imports System.Security.Cryptography

' Lap AesCryptoSevrviceProvider
Public Class Cryptor
    Private ReadOnly Provider As New AesCryptoServiceProvider

    Public Sub New()
    End Sub

    Public Sub New(ByVal Key As Byte())
        Me.Key = Key
    End Sub

    Public Property Key As Byte()
        Get
            Return Provider.Key
        End Get
        Set(Key As Byte())
            ' KeySize setter could throw exception
            Provider.KeySize = Key.Length
            Provider.Key = Key
        End Set
    End Property

    Public ReadOnly Property IV As Byte()
        Get
            Return Provider.IV
        End Get
    End Property

    Public Sub GenerateKey()
        Provider.GenerateKey()
    End Sub

    Public Sub GenerateKey(ByVal KeySize)
        Provider.KeySize = KeySize
        Provider.GenerateKey()
    End Sub

    Public Function Encrypt(ByVal Value As Byte()) As Byte()
        Provider.GenerateIV()
        Dim Encryptor As ICryptoTransform = Provider.CreateEncryptor()
        Return Encryptor.TransformFinalBlock(Value, 0, Value.Length)
    End Function

    Public Function Decrypt(ByVal InitializeVector As Byte(), ByVal Value As Byte()) As Byte()
        Provider.IV = InitializeVector
        Dim Decryptor As ICryptoTransform = Provider.CreateDecryptor()
        Return Decryptor.TransformFinalBlock(Value, 0, Value.Length)
    End Function
End Class
