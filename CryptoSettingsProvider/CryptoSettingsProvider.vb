Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Text

''' <summary>
''' Aesによる暗号化設定ファイルプロバイダ
''' </summary>
Public Class CryptoSettingsProvider
    Inherits LocalFileSettingsProvider

    ' プロバイダ名
    Public Overrides ReadOnly Property Name As String

    ' Aes暗号化クラス
    Public ReadOnly Property Cryptor As New Cryptor

    ' 初期化ベクトルをプロパティ名をキーに辞書に格納する
    Public Property IVDictionary As StringDictionary

    ' バイト配列と文字列の変換に使用する
    Public Property Encoding As Encoding = Encoding.UTF8

    ' 16進表記のキー
    Public Property HexedKey As String
        Get
            Return Hex(Cryptor.Key)
        End Get
        Set(HexedKey As String)
            Cryptor.Key = Unhex(HexedKey)
        End Set
    End Property

    ' おまじない
    Public Sub New()
        ApplicationName = String.Empty
        Name = "CryptSettingProvider"
    End Sub

    ' 初期化メソッド
    Public Overrides Sub Initialize(
            ByVal pname As String,
            ByVal config As NameValueCollection)

        ' 設定プロバイダ名を指定する
        MyBase.Initialize(Name, config)

        ' キーが存在しない場合、キーサイズを指定し、キーを再生成する
        If Cryptor.Key Is Nothing Then
            Cryptor.KeySize = 256
        End If
    End Sub

    ' プロパティの設定
    Public Overrides Sub SetPropertyValues(
            context As SettingsContext,
            collection As SettingsPropertyValueCollection)

        For Each spv As SettingsPropertyValue In collection
            ' 値を暗号化
            Dim Encrypted As Byte() = Encrypt(spv.PropertyValue, spv.Name)

            ' 16進表記に変換
            Dim Hexed As String = Me.Hex(Encrypted)

            ' 更新
            spv.PropertyValue = Hexed
        Next

        ' 設定ファイルに書き込み
        MyBase.SetPropertyValues(context, collection)
    End Sub

    ' プロパティの取得
    Public Overrides Function GetPropertyValues(
            context As SettingsContext,
            collection As SettingsPropertyCollection) As SettingsPropertyValueCollection

        Dim SPVCollection = MyBase.GetPropertyValues(context, collection)

        For Each spv As SettingsPropertyValue In SPVCollection

            ' Byte配列に変換
            Dim ByteArray As Byte() = Unhex(spv.PropertyValue)

            ' 復号
            Dim Decrypted = Decrypt(ByteArray, spv.Name)

            ' 更新
            spv.PropertyValue = Decrypted
        Next

        Return SPVCollection
    End Function

    ' 暗号化
    Private Function Encrypt(Value As String, Name As String) As Byte()
        Dim DecodedValue As Byte() = Encoding.GetBytes(Value)
        Dim EncryptedValue As Byte() = Cryptor.Encrypt(DecodedValue)
        Dim IV As Byte() = Cryptor.IV
        Dim HexStringIV As String = ByteArrayHexStringConverter.ToHexString(IV)
        ' 初期化ベクトルを辞書に追加/上書き
        AddOrOverwirte(IVDictionary, Name, HexStringIV)
        Return EncryptedValue
    End Function

    ' 復号
    Private Function Decrypt(Value As Byte(), Name As String) As String
        ' 初期化ベクトル辞書が存在しない場合は例外を投げる
        If IVDictionary Is Nothing Then
            Throw New InvalidOperationException("IV Dictionary is Nothing")
        End If

        ' 初期化ベクトル辞書に該当するキーが存在しない場合は空文字を返す
        If Not IVDictionary.ContainsKey(Name) Then
            Return String.Empty
        End If

        Dim HexStringIV = IVDictionary(Name)
        Dim IV As Byte() = Unhex(HexStringIV)
        Dim DecryptedValue = Cryptor.Decrypt(IV, Value)
        Dim EncodedValue = Encoding.GetString(DecryptedValue)
        Return EncodedValue
    End Function

    ' 16進表記からバイト配列へ変換する
    Private Function Unhex(Hexed As String) As Byte()
        Return ByteArrayHexStringConverter.ToByteArray(Hexed)
    End Function

    ' バイト配列から16進表記に変換する
    Private Function Hex(ByteArray As Byte()) As String
        Return ByteArrayHexStringConverter.ToHexString(ByteArray)
    End Function

    Private Sub AddOrOverwirte(ByRef Dictionary As StringDictionary, ByVal Key As String, ByVal Value As String)
        If Dictionary.ContainsKey(Key) Then
            Dictionary(Key) = Value
        Else
            Dictionary.Add(Key, Value)
        End If
    End Sub

End Class
