open Meadow
open System
open System.Threading
open Meadow.Devices
open Meadow.Foundation
open Meadow.Foundation.Leds

module HelloWorld =
    let colors = [ Color.AliceBlue; Color.AntiqueWhite; Color.Black; Color.DarkGray; Color.Cyan; Color.Green; Color.GreenYellow; Color.Yellow; Color.Orange; Color.OrangeRed; Color.Red; Color.MediumVioletRed; Color.Purple; Color.Magenta; Color.Pink ]

    type F7Micro with
        member device.CreateLed () =
            RgbPwmLed(
                device,
                device.Pins.OnboardLedRed,
                device.Pins.OnboardLedGreen,
                device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Peripherals.Leds.IRgbLed.CommonType.CommonAnode)

    type RgbPwmLed with
        member led.ShowColorPulse color duration =
            led.StartPulse (color, duration / 2)
            Thread.Sleep duration
            led.Stop ()

    let app duration (device:F7Micro) =
        printfn "Starting app!"
        let onboardLed = device.CreateLed ()
        while true do
            for color in colors do
                onboardLed.ShowColorPulse color duration

/// A helper module to adapt simple functions into the Meadow class / inheritance model.
module MeadowRunner =
    /// A program represented as a function that can operate on an F7Micro.
    type F7MicroApp = F7Micro -> unit

    type MeadowApp (runner) =
        inherit App<F7Micro, MeadowApp> ()
        do runner MeadowApp.Device

    /// Runs your F7Micro application.
    let runProgram (program:F7MicroApp) =
        MeadowApp(program) |> ignore
        Thread.Sleep Timeout.Infinite
        0

[<EntryPoint>]
let main args =
    match List.ofArray args with
    | "--exitOnDebug" :: _ -> 0
    | _ -> MeadowRunner.runProgram (HelloWorld.app 1000)