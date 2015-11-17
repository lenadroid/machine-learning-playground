#load "Utils.fsx"

open Accord
open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Linear
open Accord.Statistics.Models.Regression.Fitting
open FSharp.Data

let predictors = 
    [|
        "yr";
        "mnth";
        "holiday";
        "weekday";
        "workingday";
        "weathersit";
        "temp";
        "atemp";
        "hum";
        "windspeed";
        "casual";
        "registered"
    |]

type BikeData = {Predictors: float[][]; Outputs: float[][]}

let getBikeData () = 
    let bikes = CsvFile.Load(__SOURCE_DIRECTORY__ + "\\BikeSharingDataset\\day.csv").Cache()
    let inputs = 
        bikes.Rows 
        |> Seq.map (fun x -> 
                        predictors |> Array.map (fun a -> 
                                                    float(x.GetColumn a))) 
        |> Seq.toArray
    let outputs = 
        bikes.Rows 
        |> Seq.map (fun x -> [|float(x.GetColumn "cnt")|]) 
        |> Seq.toArray
    {Predictors = inputs; Outputs = outputs}

let bikeDemand () = 
    let data = getBikeData ()
    // !!!
    let regression  = new MultivariateLinearRegression(12, 1)

    // learning phase
    // "Regress" funstion contains the logic for
    // random guess, searching for the mistake and fixing the mistake
    let error = regression.Regress(data.Predictors, data.Outputs)

    let year = 2015.

    // ( 1 to 12)
    let month = 11.

    //  weather day is holiday or not 
    let holiday = 0.

    // if day is neither weekend nor holiday is 1, otherwise is 0
    let workingday = 0.

    // Day of the week
    let weekday = 1.

    // 1: Clear, Few clouds, Partly cloudy, Partly cloudy
    // 2: Mist + Cloudy, Mist + Broken clouds, Mist + Few clouds, Mist
    // 3: Light Snow, Light Rain + Thunderstorm + Scattered clouds, Light Rain + Scattered clouds
    // 4: Heavy Rain + Ice Pallets + Thunderstorm + Mist, Snow + Fog
    let weathersit = 1.
    
    // Normalized temperature in Celsius. The values are divided to 41 (max)
    let temp = 12.849153

    // Normalized feeling temperature in Celsius. The values are divided to 50 (max)
    let atemp = 10.17435

    // Normalized humidity. The values are divided to 100 (max)
    let hum = 82.75

    // Normalized wind speed. The values are divided to 67 (max)
    let windspeed = 10.024682

    // Count of casual users
    let casual = 539.

    //	count of registered users
    let registered = 1586.
   
    let y = regression.Compute([|
                                   year;
                                   month;
                                   holiday;
                                   workingday;
                                   weekday;
                                   weathersit;
                                   temp / 41.;
                                   atemp / 50.;
                                   hum / 100.;
                                   windspeed / 67.;
                                   casual;
                                   registered;
                                |])
    
    printfn "Predicting the demand on bike rentals = %A" y.[0]

    let thetas = regression.Coefficients

    System.Console.ReadLine() |> ignore
    ()
