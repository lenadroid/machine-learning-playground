#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.MachineLearning.2.15.0\lib\net45\Accord.MachineLearning.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.Math.2.15.0\lib\net45\Accord.Math.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.Statistics.2.15.0\lib\net45\Accord.Statistics.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\AForge.2.2.5\lib\AForge.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\AForge.Genetic.2.2.5\lib\AForge.Genetic.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.2.15.0\lib\net45\Accord.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\AForge.Math.2.2.5\lib\AForge.Math.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\FSharp.Charting.0.90.12\lib\net40\FSharp.Charting.dll"
#r @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Windows.Forms.DataVisualization.dll"

open System
open Accord
open Accord.Math
open Accord.MachineLearning
open FSharp.Data
open FSharp.Charting

let data ="\\BirthDeathRatesDataset\\data.csv"

let getRatesData (path) = 
    let wine = CsvFile.Load(__SOURCE_DIRECTORY__ + path).Cache()
    let columns = 
        [|
             "country";
             "birthrate";
             "deathrate";
        |]

    let inputData = wine.Rows 
                    |> Seq.map (fun x ->  
                                    columns |> Seq.skip 1 |> Seq.toArray |> Array.map (fun c -> float(x.GetColumn(c))))
                    |> Seq.toArray
    inputData

let findClusters () = 
    let input = getRatesData data
    // Create a new K-Means algorithm with N clusters 

    let kmeans = new KMeans 2
    kmeans.Tolerance <- 1e-7

    let labels = kmeans.Compute input

    let clustered = input |> Array.map (fun x -> x.[0], x.[1])

    let getColor n = 
        match n with
        | 0 -> System.Drawing.Color.Blue
        | 1 -> System.Drawing.Color.Green
        | 2 -> System.Drawing.Color.Yellow
        | 3 -> System.Drawing.Color.Purple
        | _ -> System.Drawing.Color.Black
    
    let ch = 
        Chart.Combine(clustered |> Array.mapi (fun i x -> (Chart.Line[fst x; snd x] |> Chart.WithStyling(Color= getColor (Array.get labels i)))))
    ch.ShowChart()

    System.Console.ReadLine() |> ignore
    ()