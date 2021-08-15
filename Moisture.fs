module Moisture

open Meadow
open Meadow.Devices
open Meadow.Foundation
open Meadow.Foundation.Leds
open Meadow.Foundation.Sensors.Moisture
open Meadow.Hardware
open Meadow.Units
open System

let getMoistureSensor (device:F7Micro) =
    let port = device.CreateAnalogInputPort device.Pins.A00
    port, Capacitive(port, Voltage 2.84, Voltage 1.37)

let twoDp (v:float) = Math.Round(v, 2)

let (|Parched|Dry|Moist|Drowning|) (value:float) =
    if value < 25. then Parched
    elif value < 40. then Dry
    elif value < 65. then Moist
    else Drowning

/// Sets the on-board LED to a different RAG colour depending on moisture of the sensor.
let handleMoistureReading (deviceLed:RgbPwmLed) (voltagePort:IAnalogInputPort) (update:IChangeResult<float>) =
    let humidity = update.New * 100. |> twoDp
    printfn $"Humidity: {humidity}%% ({voltagePort.Voltage.Volts |> twoDp}v)"

    // You need to explicitly stop blinking - calling SetColor isn't sufficient
    match humidity with
    | Parched | Dry | Moist -> deviceLed.Stop()
    | Drowning -> ()

    match humidity with
    | Parched -> deviceLed.SetColor Color.Red
    | Dry -> deviceLed.SetColor Color.Yellow
    | Moist -> deviceLed.SetColor Color.Green
    | Drowning -> deviceLed.StartBlink Color.Blue

let app device =
    let deviceLed = Helpers.getLed device
    let voltagePort, moistureSensor = getMoistureSensor device

    // Set up the event handler
    moistureSensor.HumidityUpdated
    |> Event.add (handleMoistureReading deviceLed voltagePort)

    // Start listening on a regular basis
    moistureSensor.StartUpdating (TimeSpan.FromSeconds 2.)