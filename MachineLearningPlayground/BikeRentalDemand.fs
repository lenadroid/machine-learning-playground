module BikeRentalDemand

open Accord
open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Linear
open Accord.Statistics.Models.Regression.Fitting
open FSharp.Data

// instant,dteday,season,yr,mnth,holiday,weekday,workingday,weathersit,temp,atemp,hum,windspeed,casual,registered,cnt

let getBikeData () = 
    let bikes = CsvFile.Load(__SOURCE_DIRECTORY__ + "\\BikeSharingDataset\\day.csv").Cache()
    let inputData = bikes.Rows 
                    |> Seq.map (fun x ->  
                                    printfn "%A,  %A - %A" (x.GetColumn "temp") (x.GetColumn "workingday") (x.GetColumn "cnt") 
                                    [|  float(x.GetColumn "season"); 
                                        float(x.GetColumn "yr"); 
                                        float(x.GetColumn "mnth"); 
                                        float(x.GetColumn "holiday"); 
                                        float(x.GetColumn "weekday"); 
                                        float(x.GetColumn "workingday"); 
                                        float(x.GetColumn "weathersit"); 
                                        float(x.GetColumn "temp"); 
                                        float(x.GetColumn "atemp");
                                        float(x.GetColumn "hum");
                                        float(x.GetColumn "windspeed");
                                        float(x.GetColumn "casual");
                                        float(x.GetColumn "registered");
                                    |]
                                ) 
                    |> Seq.toArray
    let outputData = bikes.Rows 
                    |> Seq.map (fun x -> [|float(x.GetColumn "cnt")|]) 
                    |> Seq.toArray
    (inputData, outputData)

let bikeDemand () = 
    let input, output = getBikeData () 

    let regression  = new MultivariateLinearRegression(13, 1)
    regression.Regress(input, output) |> ignore
    let y = regression.Compute([|3.0; 1.0; 5.0; 0.0; 1.0; 0.0; 2.0; 0.215833; 0.203487; 0.2775; 0.104846; 539.0; 2290.0|])
    printfn "Compute bike regression %A" y

    System.Console.ReadLine() |> ignore
    ()