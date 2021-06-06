Imports System.Configuration
'このクラスでは設定クラスでの特定のイベントを処理することができます:
' SettingChanging イベントは、設定値が変更される前に発生します。
' PropertyChanged イベントは、設定値が変更された後に発生します。
' SettingsLoaded イベントは、設定値が読み込まれた後に発生します。
' SettingsSaving イベントは、設定値が保存される前に発生します。

<SettingsProvider(GetType(CryptoSettingsProvider.CryptoSettingsProvider))>
Partial Friend NotInheritable Class CryptoSettings
    Public Sub New()
        AES_KEY_SIZE = 256
        AES_KEY = My.Settings.AesKey
        If My.Settings.IVs Is Nothing Then
            My.Settings.IVs = New SerializableStringDictionary()
        End If
        AES_IV_DICTIONARY = My.Settings.IVs
    End Sub

    ' プロバイダ
    Private Property Provider As CryptoSettingsProvider.CryptoSettingsProvider = Providers.Item("CryptSettingProvider")

    ' 暗号キー
    Public Property AES_KEY As String
        Get
            Return Provider.HexedKey
        End Get
        Set(HexedKey As String)
            Provider.HexedKey = HexedKey
        End Set
    End Property

    ' 暗号キーサイズ
    Public Property AES_KEY_SIZE As Integer
        Get
            Return Provider.Cryptor.KeySize
        End Get
        Set(KeySize As Integer)
            Provider.Cryptor.KeySize = KeySize
        End Set
    End Property

    ' 初期化ベクトル
    Public Property AES_IV_DICTIONARY As SerializableStringDictionary
        Get
            Return Provider.IVDictionary
        End Get
        Set(IV As SerializableStringDictionary)
            Provider.IVDictionary = IV
        End Set
    End Property
End Class
