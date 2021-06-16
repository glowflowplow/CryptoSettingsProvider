Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim Provider As CryptoSettingsProvider.CryptoSettingsProvider = CryptoSettings.Default.Providers("CryptoSettingsProvider")
        If My.Settings.IVs Is Nothing Then My.Settings.IVs = New SerializableStringDictionary()
        Provider.IVDictionary = My.Settings.IVs
        Provider.HexadecimalKey = My.Settings.AesKey
        CryptoSettings.Default.Reload()
        TextBox1.Text = CryptoSettings.Default.SettingA
        TextBox2.Text = CryptoSettings.Default.SettingB
        TextBox3.Text = CryptoSettings.Default.SettingC
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        CryptoSettings.Default.SettingA = TextBox1.Text
        CryptoSettings.Default.SettingB = TextBox2.Text
        CryptoSettings.Default.SettingC = TextBox3.Text
        CryptoSettings.Default.Save()
        My.Settings.Save()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        My.Settings.Reload()
        CryptoSettings.Default.Reload()
        TextBox1.Text = CryptoSettings.Default.SettingA
        TextBox2.Text = CryptoSettings.Default.SettingB
        TextBox3.Text = CryptoSettings.Default.SettingC
    End Sub
End Class
