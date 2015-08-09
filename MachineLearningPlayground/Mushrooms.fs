module Mushrooms

open System
open Accord
open Accord.Math
open Accord.MachineLearning
open FSharp.Data

// Distinguishing Poisonous from Edible Wild Mushrooms

let trainData ="\\MushroomsDataset\\agaricus-lepiota.data"
let testData = "\\MushroomsDataset\\agaricus-lepiota-test.data"

let getMushroomData (path) = 
    let mushrooms = CsvFile.Load(__SOURCE_DIRECTORY__ + path).Cache()

    let columns = 
        [|
             "class";
             "cap-shape";
             "cap-surface";
             "cap-color";
             "bruises";
             "odor";
             "gill-attachment";
             "gill-spacing";
             "gill-size";
             "gill-color";
             "stalk-shape";
             "stalk-root";
             "stalk-surface-above-ring";
             "stalk-surface-below-ring";
             "stalk-color-above-ring";
             "stalk-color-below-ring";
             "veil-type";
             "veil-color";
             "ring-number";
             "ring-type";
             "spore-print-color";
             "population";
             "habitat"
        |]

    let inputData = mushrooms.Rows 
                    |> Seq.map (fun x ->  
                                    columns |> Seq.skip 1 |> Seq.toArray |> Array.map (fun c -> x.GetColumn(c)))
                    |> Seq.toArray
    let classData = mushrooms.Rows 
                    |> Seq.map (fun x -> (x.GetColumn (Seq.head columns))) 
                    |> Seq.map (fun x -> if "e".Equals x then 0 else 1)
                    |> Seq.toArray
    (inputData, classData)

let LevenshteinDistance = Func<string[],string[],float>(fun s ss -> Distance.Levenshtein(s,ss))

let zipWithIndex list = Seq.zip list (seq {
                                             let i = ref 0
                                             while true do
                                                 let n = !i
                                                 i := !i + 1
                                                 yield n
                                           })

let testAndCheckAccuracy (knn: KNearestNeighbors<string[]>) = 
    let input, output = getMushroomData testData

    let correctGuesses = input 
                         |> Array.mapi (fun i x -> printfn "%A - %A" i (knn.Compute(x) = Array.get output i); if (knn.Compute(x) = Array.get output i) then 1 else 0) 
                         |> Array.sum
    printfn "Correct guesses %A/%A" correctGuesses (Array.length input)
    float(correctGuesses)/float(Array.length input) * 100.0

let mayIEatAMushroom () = 
    let input, output = getMushroomData trainData

    let knn = new KNearestNeighbors<string[]>(22, 2, input, output, LevenshteinDistance)

    let accuracy = testAndCheckAccuracy knn
    printfn "Accuracy in results on mushroom poisonness prediction: %A percents" accuracy
    
    // After the algorithm has been created, we can use it:

    let answer = knn.Compute([|"x";"s"; "n";"t";"n";"f";"c";"b";"e";"e";"?";"s";"s";"e";"e";"p";"w";"t";"e";"w";"c";"w"|]) 
    // answer should be 0.

    let answer = knn.Compute([|"f";"s"; "w";"t";"f";"f";"c";"b";"p";"t";"b";"f";"s";"w";"w";"p";"w";"o";"p";"h";"v";"u"|]) 
    // answer should be 1.

    System.Console.ReadLine() |> ignore
    ()
