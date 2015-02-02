namespace Lorn.ADSP.DE.Serializers

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open System.Collections
open System.Collections.Generic
open System.Text
open System.Xml

[<Export(typeof<ISerializeRequestedServiceInformations>)>]
[<ExportMetadata("SerializerId","{F75805E7-AC7A-4B9E-80EE-8B67C2544673}")>]
[<ExportMetadata("SerializerName","XmlRequestedInformationsSerializer")>]
[<ExportMetadata("Description","以XML方式对信息服务网关返回数据进行序列化的序列化器")>]
[<ExportMetadata("Version","1.0")>]
type public XmlRequestedInformationsSerializer() = 
    interface ISerializeRequestedServiceInformations with
        member this.SerialzeRequestedServiceInformations(data) = 
            let document = new XmlDocument()
            let informationNode = document.CreateElement("InformationData")
            ignore(document.AppendChild(informationNode))
            for item in data do
                let serviceDataNode = document.CreateElement("ServiceData")
                ignore(informationNode.AppendChild(serviceDataNode))
                let serviceNameAttribute = document.CreateAttribute("ServiceName")
                serviceNameAttribute.Value <- item.Key
                ignore(serviceDataNode.Attributes.Append(serviceNameAttribute))
                serviceDataNode.InnerText <- item.Value
            document.OuterXml
