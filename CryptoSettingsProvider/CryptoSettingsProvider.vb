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

    ''' <summary>
    ''' アプリ名を取得または設定します
    ''' </summary>
    ''' <returns>アプリ名</returns>
    Public Overrides Property ApplicationName As String

    ''' <summary>
    ''' プロバイダ名を取得します
    ''' </summary>
    ''' <returns>プロバイダ名</returns>
    Public Overrides ReadOnly Property Name As String

    ''' <summary>
    ''' 暗号化するためのクラスを取得します
    ''' </summary>
    ''' <returns>暗号化するためのクラス</returns>
    Private ReadOnly Property Cryptor As New Cryptor

    ''' <summary>
    ''' 初期化ベクトルを保持する辞書を取得または設定します
    ''' プロパティ名がキー、初期化ベクトルが値に対応します
    ''' </summary>
    ''' <returns>初期化ベクトルを持つ辞書</returns>
    Public Property IVDictionary As StringDictionary

    ''' <summary>
    ''' 暗号化する際の文字コードを取得または設定します
    ''' </summary>
    ''' <returns>文字コード</returns>
    Public Property Encoding As Encoding = Encoding.UTF8

    ''' <summary>
    ''' キーを16進表記で取得または設定します
    ''' </summary>
    ''' <returns>文字列</returns>
    Public Property HexadecimalKey As String
        Get
            Return HexConverter.ToHexString(Cryptor.Key)
        End Get
        Set(HexedKey As String)
            Cryptor.Key = HexConverter.ToByteArray(HexedKey)
        End Set
    End Property

    ''' <summary>
    ''' Constructor
    ''' </summary>
    Public Sub New()
        ApplicationName = String.Empty
        Name = "CryptoSettingsProvider"
    End Sub

    ''' <summary>
    ''' 初期化メソッド
    ''' </summary>
    ''' <param name="pname">プロバイダ名</param>
    ''' <param name="config">設定</param>
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

    ''' <inheritdoc/>
    Public Overrides Sub SetPropertyValues(
            context As SettingsContext,
            collection As SettingsPropertyValueCollection)

        ' 更新対象コレクション
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

            ' 追加
            NewCollection.Add(spv)
        Next

        ' 設定ファイルに書き込み
        MyBase.SetPropertyValues(context, NewCollection)
    End Sub

    ''' <inheritdoc/>
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

    ''' <summary>
    ''' 値を暗号化し、辞書に初期化ベクトルを登録または更新する
    ''' </summary>
    ''' <param name="Value">値</param>
    ''' <param name="Name">プロパティ名</param>
    ''' <returns>暗号化された値</returns>
    Private Function Encrypt(Value As String, Name As String) As String
        ' 初期化ベクトル辞書が存在しない場合はNullを返す
        If IVDictionary Is Nothing Then
            Return Nothing
        End If

        Dim DecodedValue As Byte() = Encoding.GetBytes(Value)
        Dim EncryptedValue As Byte() = Cryptor.Encrypt(DecodedValue)
        Dim IV As Byte() = Cryptor.IV
        Dim HexStringIV As String = HexConverter.ToHexString(IV)
        ' 初期化ベクトルを辞書に追加/上書き
        AddOrOverwirte(IVDictionary, Name, HexStringIV)
        Dim HexedString = HexConverter.ToHexString(EncryptedValue)
        Return HexedString
    End Function

    ''' <summary>
    ''' 値を復号する
    ''' 復号するためには辞書に正確な初期化ベクトルが保持されている必要がある
    ''' </summary>
    ''' <param name="Value">値</param>
    ''' <param name="Name">プロパティ名</param>
    ''' <returns>復号された値</returns>
    Private Function Decrypt(Value As String, Name As String) As String
        ' 初期化ベクトル辞書が存在しない場合はNullを返す
        If IVDictionary Is Nothing Then
            Return Nothing
        End If

        ' 初期化ベクトル辞書に該当するキーが存在しない場合はNullを返す
        If Not IVDictionary.ContainsKey(Name) Then
            Return Nothing
        End If

        Dim ByteArray = HexConverter.ToByteArray(Value)
        Dim HexStringIV = IVDictionary(Name)
        Dim IV As Byte() = HexConverter.ToByteArray(HexStringIV)
        Dim DecryptedValue = Cryptor.Decrypt(IV, ByteArray)
        Dim EncodedValue = Encoding.GetString(DecryptedValue)
        Return EncodedValue
    End Function

    ''' <summary>
    ''' 辞書に値を登録する。既に値が存在する場合は更新する。
    ''' </summary>
    ''' <param name="Dictionary"></param>
    ''' <param name="Key"></param>
    ''' <param name="Value"></param>
    Private Sub AddOrOverwirte(ByRef Dictionary As StringDictionary, ByVal Key As String, ByVal Value As String)
        If Dictionary.ContainsKey(Key) Then
            Dictionary(Key) = Value
        Else
            Dictionary.Add(Key, Value)
        End If
    End Sub
End Class
