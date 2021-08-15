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

/// Manually reads from the moisture sensor
let readMoisture (moistureSensor:Capacitive) =
    let moisture = moistureSensor.Read().Result * 100.
    printfn $"Read moisture manually: {moisture}%%"

let twoDp (v:float) = Math.Round(v, 2)

let (|Parched|Dry|Moist|Drowning|) v =
    if v < 25. then Parched
    elif v < 40. then Dry
    elif v < 65. then Moist
    else Drowning

/// Sets the on-board LED to a different RAG colour depending on moisture of the sensor.
let handleMoistureReading (deviceLed:RgbPwmLed) (voltagePort:IAnalogInputPort) (update:IChangeResult<float>) =
    let humidity = twoDp (update.New * 100.)
    printfn $"Humidity: {humidity}%% @ {twoDp (voltagePort.Voltage.Volts)}V"

    // Stop blinking if not drowning
    match humidity with
    | Parched | Dry | Moist -> deviceLed.Stop()
    | Drowning -> ()

    // Set the LED depending on humidity
    match humidity with
    | Parched -> deviceLed.SetColor Color.Red
    | Dry -> deviceLed.SetColor Color.Yellow
    | Moist -> deviceLed.SetColor Color.Green
    | Drowning -> deviceLed.StartBlink Color.Red

let app device =
    let deviceLed = Helpers.getLed device
    let voltagePort, moistureSensor = getMoistureSensor device

    readMoisture moistureSensor

    // Set up the event handler
    moistureSensor.HumidityUpdated
    |> Event.add (handleMoistureReading deviceLed voltagePort)

    // Start listening on a regular basis
    moistureSensor.StartUpdating (TimeSpan.FromSeconds 5.)