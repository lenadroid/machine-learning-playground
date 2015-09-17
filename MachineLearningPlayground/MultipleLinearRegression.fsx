#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.MachineLearning.2.15.0\lib\net45\Accord.MachineLearning.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.Math.2.15.0\lib\net45\Accord.Math.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.Statistics.2.15.0\lib\net45\Accord.Statistics.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.2.15.0\lib\net45\Accord.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"

open Accord
open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Linear
open Accord.Statistics.Models.Regression.Fitting
open FSharp.Data

let predictors = 
    [|
        "season";
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
    let inputs = bikes.Rows 
                    |> Seq.map (fun x -> 
                                    predictors |> Array.map (fun a -> float(x.GetColumn a))) 
                    |> Seq.toArray
    let outputs = bikes.Rows 
                     |> Seq.map (fun x -> [|float(x.GetColumn "cnt")|]) 
                     |> Seq.toArray
    {Predictors = inputs; Outputs = outputs}

let bikeDemand () = 
    let data = getBikeData ()
    // !!!
    let regression  = new MultivariateLinearRegression(13, 1)
    let error = regression.Regress(data.Predictors, data.Outputs)

    // 1:springer, 2:summer, 3:fall, 4:winter
    let season = 3.

    let year = 2015.

    // ( 1 to 12)
    let month = 9.

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
    let weathersit = 2.
    
    // Normalized temperature in Celsius. The values are divided to 41 (max)
    let temp = 8.849153

    // Normalized feeling temperature in Celsius. The values are divided to 50 (max)
    let atemp = 10.17435

    // Normalized humidity. The values are divided to 100 (max)
    let hum = 27.75

    // Normalized wind speed. The values are divided to 67 (max)
    let windspeed = 7.024682

    // Count of casual users
    let casual = 539.

    //	count of registered users
    let registered = 2290.
   
    let y = regression.Compute([|
                                   season;
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

    let thetas = regression.Coefficients.Length

    System.Console.ReadLine() |> ignore
    ()
