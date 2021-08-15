module Helpers

open Meadow
open Meadow.Devices
open Meadow.Foundation.Leds

let getLed (device:F7Micro) =
    RgbPwmLed(
        device,
        device.Pins.OnboardLedRed,
        device.Pins.OnboardLedGreen,
        device.Pins.OnboardLedBlue,
        3.3f, 3.3f, 3.3f,
        Peripherals.Leds.IRgbLed.CommonType.CommonAnode)
