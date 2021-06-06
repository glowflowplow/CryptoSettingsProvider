Imports System.Configuration
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

' XMLでシリアライズ属性付与
<SettingsSerializeAs(SettingsSerializeAs.Xml)>
Public Class SerializableStringDictionary
    Inherits Specialized.StringDictionary
    Implements IXmlSerializable

    ' XMLからオブジェクトを生成する
    Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        ' リーダーから読み込む
        While reader.Read() AndAlso Not (reader.NodeType = Xml.XmlNodeType.EndElement AndAlso reader.LocalName = Me.GetType().Name)
            ' nameを取得
            Dim name = reader("Name")
            If name = Nothing Then
                ' name
                Throw New FormatException()
            End If
            ' valueを取得
            Dim value = reader("Value")
            ' 辞書にnameをキー、valueを値にして追加
            Me(name) = value
        End While
    End Sub

    ' オブジェクトをXMLに変換する
    Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
        For Each entry As DictionaryEntry In Me
            ' エレメントの書き込みを開始
            writer.WriteStartElement("Pair")
            ' Name属性にKeyを設定
            writer.WriteAttributeString("Name", entry.Key)
            ' Value属性にValueを設定
            writer.WriteAttributeString("Value", entry.Value)
            ' エレメントの書き込みを終了
            writer.WriteEndElement()
        Next
    End Sub

    ' Nothingを返さなければいけないメソッド（なんであるんだよ）
    ' 参照(https://docs.microsoft.com/ja-jp/dotnet/api/system.xml.serialization.ixmlserializable?view=netframework-4.7.2#methods)
    Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
        Return Nothing
    End Function
End Class
