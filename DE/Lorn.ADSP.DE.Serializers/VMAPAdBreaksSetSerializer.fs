namespace Lorn.ADSP.DE.Serializers

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open System.Collections.Generic
open System.Text
open System.Xml
open System.Linq

[<Export(typeof<ISerializeAdBreaksSet>)>]
[<ExportMetadata("SerializerId","{18FB945C-75E3-40DA-961D-B30209038A4B}")>]
[<ExportMetadata("SerializerName","VMAPAdBreaksSetSerializer")>]
[<ExportMetadata("Description","按照VMAP规范对广告返回队列进行序列化")>]
[<ExportMetadata("Version","1.0")>]
type public VMAPAdBreaksSetSerializer() = 
    interface ISerializeAdBreaksSet with
        member this.SerializeAdBreaksSet(adBreaks, extensions) =
            let sb = new StringBuilder()
            let sbsettings = new XmlWriterSettings()
            sbsettings.OmitXmlDeclaration <- true
            let xmlTextWriter = XmlWriter.Create(sb,sbsettings);
            xmlTextWriter.WriteStartElement("vmap","VMAP","http://www.iab.net/vmap-1.0")
            xmlTextWriter.WriteAttributeString("version","1.0")
            for item in adBreaks do
                xmlTextWriter.WriteStartElement("AdBreak")
                if System.String.IsNullOrEmpty(item.Key.TimeOffset) then
                    xmlTextWriter.WriteAttributeString("timeOffset","start")
                else
                    xmlTextWriter.WriteAttributeString("timeOffset",item.Key.TimeOffset)
                xmlTextWriter.WriteAttributeString("breakType",item.Key.PositionType.ToString())
                xmlTextWriter.WriteAttributeString("breakId",item.Key.AdPositionId.ToString())
                xmlTextWriter.WriteStartElement("AdSource")
                if item.Key.VASTVersion >= (float32)3.0 then
                    xmlTextWriter.WriteStartElement("VASTData")
                    xmlTextWriter.WriteRaw(item.Value)
                else
                    xmlTextWriter.WriteStartElement("CustomAdData")
                    if item.Key.VASTVersion >= (float32)2.0 then
                        xmlTextWriter.WriteAttributeString("templateType","vast2")
                    else if item.Key.VASTVersion >= (float32)1.0 then
                        xmlTextWriter.WriteAttributeString("templateType","vast1")
                    else
                        xmlTextWriter.WriteAttributeString("templateType","proprietary")
                    xmlTextWriter.WriteCData(item.Value)
                xmlTextWriter.WriteEndElement()
                xmlTextWriter.WriteEndElement()
                if item.Key.TrackingEvents <> null && item.Key.TrackingEvents.Count > 0 then
                    xmlTextWriter.WriteStartElement("TrackingEvents")
                    for event in item.Key.TrackingEvents do
                        xmlTextWriter.WriteStartElement("Tracking")
                        xmlTextWriter.WriteAttributeString("Event",event.Key)
                        xmlTextWriter.WriteRaw(event.Value)
                        xmlTextWriter.WriteEndElement()
                    xmlTextWriter.WriteEndElement()
                xmlTextWriter.WriteEndElement()
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

