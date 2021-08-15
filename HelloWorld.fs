module HelloWorld

open Meadow.Foundation
open Meadow.Foundation.Leds
open System.Threading

let colors = [ Color.AliceBlue; Color.AntiqueWhite; Color.Black; Color.DarkGray; Color.Cyan; Color.Green; Color.GreenYellow; Color.Yellow; Color.Orange; Color.OrangeRed; Color.Red; Color.MediumVioletRed; Color.Purple; Color.Magenta; Color.Pink ]

type RgbPwmLed with
    member led.ShowColorPulse color duration =
        led.StartPulse (color, duration / 2)
        Thread.Sleep duration
        led.Stop ()

let app duration device =
    printfn "Starting app!"
    let onboardLed = Helpers.getLed device
    while true do
        for color in colors do
            onboardLed.ShowColorPulse color duration
