#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.MachineLearning.2.15.0\lib\net45\Accord.MachineLearning.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.Math.2.15.0\lib\net45\Accord.Math.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.Neuro.2.15.0\lib\net45\Accord.Neuro.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.Statistics.2.15.0\lib\net45\Accord.Statistics.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\AForge.2.2.5\lib\AForge.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\AForge.Genetic.2.2.5\lib\AForge.Genetic.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\Accord.2.15.0\lib\net45\Accord.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\AForge.Math.2.2.5\lib\AForge.Math.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\AForge.Neuro.2.2.5\lib\AForge.Neuro.dll"
#r @"D:\Программирование\github\machine-learning-playground\packages\FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"

open System
open Accord
open Accord.Math
open Accord.MachineLearning
open FSharp.Data
open AForge.Neuro
open Accord.Neuro
open Accord.Neuro.Learning
open Accord.Statistics

let trainData ="\\PokerDataset\\poker-hand-training-true.data"
let testData = "\\PokerDataset\\poker-hand-testing-big.data"

let classDataOffset = 1

let normalize (mins: float[]) (maxs: float[]) (r:float[]) = 
    r |> Array.mapi(fun i x -> (x - float(Array.get mins i))/float((Array.get maxs i) - (Array.get mins i)))

let getPredictedClassFrom outputLayer offset = 
    Array.IndexOf(outputLayer,(Array.max outputLayer)) + offset

let getPokerData (path) = 
    let poker = CsvFile.Load(__SOURCE_DIRECTORY__ + path).Cache()
    let columns = 
        [|
             "s1";
             "c1";
             "s2";
             "c2";
             "s3";
             "c3";
             "s4";
             "c4";
             "s5";
             "c5";
             "class";
        |]

    let inputData = poker.Rows 
                    |> Seq.map (fun x ->  
                                    columns |> Seq.take (columns.Length - 1) |> Seq.toArray |> Array.map (fun c -> float(x.GetColumn(c))))
                    |> Seq.toArray

    let minimums = inputData |> Array.map (fun r -> r |> Array.min)
    let maximums = inputData |> Array.map (fun r -> r |> Array.max)
    let normalizedData = inputData |> Array.map(fun r -> r |> Array.mapi(fun i x -> (x - float(Array.get minimums i))/float((Array.get maximums i) - (Array.get minimums i))))

    let classData = poker.Rows 
                    |> Seq.map (fun x -> (int(x.GetColumn "class"))) 
                    |> Seq.toArray
    let outputs = Accord.Statistics.Tools.Expand(classData, -1.0, +1.0)
    (normalizedData, classData, outputs, minimums, maximums)

let zipWithIndex list = Seq.zip list (seq {
                                             let i = ref 0
                                             while true do
                                                 let n = !i
                                                 i := !i + 1
                                                 yield n
                                           })

let testAndCheckAccuracy (network: ActivationNetwork) = 
    let input, classes, outputs, minimums, maximums = getPokerData trainData

    let correctGuesses = input 
                         |> Array.mapi (fun i x -> 
                            let outputLayer = network.Compute(x)
                            let predictedClass = getPredictedClassFrom outputLayer 0
                            printfn "%A - %A, %A, %A" i (predictedClass = Array.get classes i) predictedClass (Array.get classes i)
                            if (predictedClass = Array.get classes i) then 1 else 0
                            ) 
                         |> Array.sum
    printfn "Correct guesses %A/%A" correctGuesses (Array.length input)
    float(correctGuesses)/float(Array.length input) * 100.0

let isMyPokerHandWin () = 
    let input, classes, outputs, minimums, maximums = getPokerData trainData
    let activationFunction = BipolarSigmoidFunction()
    let network = new ActivationNetwork(activationFunction, 10, 82, 4)

    (new NguyenWidrow(network)).Randomize()

    // Teach the network using parallel Rprop:
    let teacher = new ParallelResilientBackpropagationLearning(network)

    let rec teach () : unit = 
        match teacher.RunEpoch(input, outputs) with 
        | x when x > 0.01 -> printfn "%A" x; teach ()
        | _ -> ()

    teach()
    
    // After the algorithm has been created, we can use it:

    let class0 = [|1.;1.;1.;13.;2.;4.;2.;3.;1.;12.|]
    let class1 = [|1.;12.;3.;6.;4.;13.;2.;6.;2.;1.|]

    let norm = class1 |> normalize minimums maximums

    let outputLayer = network.Compute(norm)

    let predictedClass = getPredictedClassFrom outputLayer 0

    let accuracy = testAndCheckAccuracy network
    printfn "Accuracy in results on poker hand prediction: %A percents" accuracy

    System.Console.ReadLine() |> ignore
    ()
