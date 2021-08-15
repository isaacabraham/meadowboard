open Meadow
open Meadow.Devices
open System.Threading

/// A helper module to adapt simple functions into the Meadow class / inheritance model.
module MeadowRunner =
    /// A program represented as a function that can operate on an F7Micro.
    type F7MicroApp = F7Micro -> unit

    type MeadowApp (runner) =
        inherit App<F7Micro, MeadowApp> ()
        do runner MeadowApp.Device

    /// Runs your F7Micro application.
    let runProgram (program:F7MicroApp) =
        MeadowApp (program) |> ignore
        Thread.Sleep Timeout.Infinite
        0

[<EntryPoint>]
let main args =
    match List.ofArray args with
    | "--exitOnDebug" :: _ -> 0
    | _ -> MeadowRunner.runProgram (Moisture.app)