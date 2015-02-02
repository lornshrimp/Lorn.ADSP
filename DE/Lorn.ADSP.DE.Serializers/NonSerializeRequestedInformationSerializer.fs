namespace Lorn.ADSP.DE.Serializers

open System.ComponentModel.Composition
open Lorn.ADSP.DE.DataModels
open Lorn.ADSP.DE.Interfaces
open System.Collections.Generic
open System.Text
open System.Xml
open System.Linq

[<Export(typeof<ISerializeRequestedServiceInformations>)>]
[<ExportMetadata("SerializerId","{9F3063DF-0E49-4265-8AE1-ADBC4F5BBE8D}")>]
[<ExportMetadata("SerializerName","NonSerializeRequestedInformationSerializer")>]
[<ExportMetadata("Description","对只有一个信息服务接入情况下不进行序列化的的序列化器")>]
[<ExportMetadata("Version","1.0")>]
type public NonSerializeRequestedInformationSerializer() = 
    interface ISerializeRequestedServiceInformations with
         member this.SerialzeRequestedServiceInformations(data) =
            let returnString = 
                if data.Count = 1 then
                    data.First().Value
                else
                    "Error"
            returnString

