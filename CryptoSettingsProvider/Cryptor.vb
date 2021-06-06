Imports System.Security.Cryptography

Public Class Cryptor
    Private ReadOnly Provider As New AesCryptoServiceProvider

    Public Sub New()
        Provider.GenerateKey()
    End Sub

    Public Sub New(ByVal Key As Byte())
        Provider.Key = Key
    End Sub

    Public Property Key As Byte()
        Get
            Return Provider.Key
        End Get
        Set(Key As Byte())
            Provider.Key = Key
        End Set
    End Property

    Public Property KeySize As Integer
        Get
            Return Provider.KeySize
        End Get
        Set(KeySize As Integer)
            Provider.KeySize = KeySize
            Provider.GenerateKey()
        End Set
    End Property

    Public ReadOnly Property IV As Byte()
        Get
            Return Provider.IV
        End Get
    End Property

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
