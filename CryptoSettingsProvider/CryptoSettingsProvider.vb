Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Security.Permissions
Imports System.Text

''' <summary>
''' Aesによる暗号化設定ファイルプロバイダ
''' </summary>

<
    PermissionSet(SecurityAction.LinkDemand, Name:="FullTrust"),
    PermissionSet(SecurityAction.InheritanceDemand, Name:="FullTrust")
>
Public Class CryptoSettingsProvider
    Inherits LocalFileSettingsProvider

    ' アプリ名
    Public Overrides Property ApplicationName As String

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

    ' コンストラクタ
    Public Sub New()
        ApplicationName = String.Empty
        Name = "CryptoSettingsProvider"
    End Sub

    ' 初期化メソッド
    Public Overrides Sub Initialize(
            ByVal pname As String,
            ByVal config As NameValueCollection)

        ' 設定プロバイダ名を指定する
        If (String.IsNullOrEmpty(pname)) Then
            pname = "CryptoSettingsProvider"
        End If

        MyBase.Initialize(pname, config)

        ' キーが存在しない場合、キーを生成する
        If Cryptor.Key Is Nothing Then
            Cryptor.GenerateKey(256)
        End If
    End Sub

    ' プロパティの設定
    Public Overrides Sub SetPropertyValues(
            context As SettingsContext,
            collection As SettingsPropertyValueCollection)

        Dim NewCollection = New SettingsPropertyValueCollection()
        For Each spv As SettingsPropertyValue In collection
            ' String以外は暗号化しない
            If spv.Property.PropertyType <> GetType(String) Then
                Continue For
            End If

            ' 値がNullの場合は更新対象に含めない
            If spv.PropertyValue Is Nothing Then
                Continue For
            End If

            ' 値を暗号化
            spv.PropertyValue = Encrypt(spv.PropertyValue, spv.Name)
            NewCollection.Add(spv)
        Next

        ' 設定ファイルに書き込み
        MyBase.SetPropertyValues(context, NewCollection)
    End Sub

    ' プロパティの取得
    Public Overrides Function GetPropertyValues(
            context As SettingsContext,
            collection As SettingsPropertyCollection) As SettingsPropertyValueCollection

        Dim SPVCollection = MyBase.GetPropertyValues(context, collection)

        For Each spv As SettingsPropertyValue In SPVCollection
            ' String以外は復号しない
            If spv.Property.PropertyType <> GetType(String) Then
                Continue For
            End If

            ' 復号
            ' 失敗した場合はNullを取得
            Try
                spv.PropertyValue = Decrypt(spv.PropertyValue, spv.Name)
            Catch ex As Exception
                spv.PropertyValue = Nothing
            End Try
        Next

        Return SPVCollection
    End Function

    ' 暗号化
    Private Function Encrypt(Value As String, Name As String) As String
        Dim DecodedValue As Byte() = Encoding.GetBytes(Value)
        Dim EncryptedValue As Byte() = Cryptor.Encrypt(DecodedValue)
        Dim IV As Byte() = Cryptor.IV
        Dim HexStringIV As String = ByteArrayHexStringConverter.ToHexString(IV)
        ' 初期化ベクトルを辞書に追加/上書き
        AddOrOverwirte(IVDictionary, Name, HexStringIV)
        Dim HexedString = Hex(EncryptedValue)
        Return HexedString
    End Function

    ' 復号
    Private Function Decrypt(Value As String, Name As String) As String
        ' 初期化ベクトル辞書が存在しない場合は例外を投げる
        If IVDictionary Is Nothing Then
            Throw New InvalidOperationException("IV Dictionary is Nothing")
        End If

        ' 初期化ベクトル辞書に該当するキーが存在しない場合は空文字を返す
        If Not IVDictionary.ContainsKey(Name) Then
            Return String.Empty
        End If

        Dim ByteArray = Unhex(Value)
        Dim HexStringIV = IVDictionary(Name)
        Dim IV As Byte() = Unhex(HexStringIV)
        Dim DecryptedValue = Cryptor.Decrypt(IV, ByteArray)
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
