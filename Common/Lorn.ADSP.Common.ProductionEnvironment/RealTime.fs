namespace Lorn.ADSP.Common.ProductionEnvironment

open Lorn.ADSP.Common.Interfaces
open System
open System.ComponentModel.Composition

[<Export(typeof<ITime>)>]
type public RealTime() = 
     interface ITime with
        override this.Now with get() = DateTime.Now
