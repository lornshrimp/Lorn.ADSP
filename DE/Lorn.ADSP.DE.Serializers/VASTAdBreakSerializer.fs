namespace Lorn.ADSP.DE.Serializers

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open System.Collections.Generic
open System.Text
open System.Xml
open System.Linq

[<Export(typeof<ISerializeAdBreak>)>]
[<ExportMetadata("SerializerId","{18FB945C-75E3-40DA-961D-B30209038532}")>]
[<ExportMetadata("SerializerName","VASTAdBreakSerializer")>]
[<ExportMetadata("Description","按照VAST规范对单个广告位内返回多支广告队列进行序列化")>]
[<ExportMetadata("Version","1.0")>]
type public VASTAdBreakSerializer() =
    interface ISerializeAdBreak with
        member this.SerializeAdBreak(adPositionRelease,adMaterialReleases,extensions) =
            let sb = new StringBuilder()
            let xmlTextWriter = XmlWriter.Create(sb);
            xmlTextWriter.WriteStartElement("VAST")
            xmlTextWriter.WriteAttributeString("version","3.0")
            while adMaterialReleases.Count > 0 do
                let material = adMaterialReleases.Dequeue()
                xmlTextWriter.WriteRaw(material)
            if extensions <> null && extensions.Count > 0 then
                xmlTextWriter.WriteStartElement("Extensions")
                for extension in extensions do
                    xmlTextWriter.WriteStartElement("Extension")
                    xmlTextWriter.WriteAttributeString("type",extension.Key)
                    xmlTextWriter.WriteString(extension.Value)
                    xmlTextWriter.WriteEndElement()
                xmlTextWriter.WriteEndElement()
            xmlTextWriter.WriteEndElement()
            xmlTextWriter.Flush()
            sb.ToString()


