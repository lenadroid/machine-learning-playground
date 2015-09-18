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

let trainData ="\\WineDataset\\wine.training.data"
let testData = "\\WineDataset\\wine.testing.data"
let classDataOffset = 1

let normalize (mins: float[]) (maxs: float[]) (r:float[]) = 
    r |> Array.mapi(fun i x -> (x - float(Array.get mins i))/float((Array.get maxs i) - (Array.get mins i)))

let getPredictedClassFrom outputLayer offset = 
    Array.IndexOf(outputLayer,(Array.max outputLayer)) + offset

let getWineData (path) = 
    let wine = CsvFile.Load(__SOURCE_DIRECTORY__ + path).Cache()
    let columns = 
        [|
             "Class";
             "Alcohol";
             "MalicAcid";
             "Ash";
             "AlcalinityOfAsh";
             "Magnesium";
             "TotalPhenols";
             "Flavanoids";
             "NonflavanoidPhenols";
             "Proanthocyanins";
             "ColorIntensity";
             "Hue";
             "OD280OD315OfDilutedWines";
             "Proline";
        |]

    let inputData = wine.Rows 
                    |> Seq.map (fun x ->  
                                    columns |> Seq.skip 1 |> Seq.toArray |> Array.map (fun c -> float(x.GetColumn(c))))
                    |> Seq.toArray

    let minimums = inputData |> Array.map (fun r -> r |> Array.min)
    let maximums = inputData |> Array.map (fun r -> r |> Array.max)
    let normalizedData = inputData |> Array.map(fun r -> r |> Array.mapi(fun i x -> (x - float(Array.get minimums i))/float((Array.get maximums i) - (Array.get minimums i))))

    let classData = wine.Rows 
                    |> Seq.map (fun x -> (int(x.GetColumn (Seq.head columns))) - classDataOffset) 
                    |> Seq.toArray
    let outputs = Accord.Statistics.Tools.Expand(classData, -1.0, +1.0)
    (normalizedData, classData, outputs, minimums, maximums)

let testAndCheckAccuracy (network: ActivationNetwork) = 
    let input, classes, outputs, minimums, maximums = getWineData testData

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

let predictWineCategories () = 
    let input, classes, outputs, minimums, maximums = getWineData trainData
    let activationFunction = BipolarSigmoidFunction()
    let network = new ActivationNetwork(activationFunction, 13, 79, 3)
    // Randomly initialize the network
    (new NguyenWidrow(network)).Randomize()

    // Teach the network using parallel Rprop:
    let teacher = new ParallelResilientBackpropagationLearning(network)

    let rec teach () : unit = 
        match teacher.RunEpoch(input, outputs) with 
        | x when x > 0.01 -> printfn "%A" x; teach ()
        | _ -> ()
    
        teach()

    // After the algorithm has been created, we can use it:

    let class1 = [|13.2;1.78;2.14;11.2;100.0;2.65;2.76;0.26;1.28;4.38;1.05;3.4;1050.0|]
    let class2 = [|13.05;5.8;2.13;21.5;86.0;2.62;2.65;0.3;2.01;2.6;0.73;3.1;380.0|]
    let class3 = [|13.27;4.28;2.26;20.0;120.0;1.59;0.69;0.43;1.35;10.2;0.59;1.56;835.0|]

    let norm = class1 |> normalize minimums maximums
    let outputLayer = network.Compute(norm)
    let predictedClass = getPredictedClassFrom outputLayer 1

    let accuracy = testAndCheckAccuracy network
    printfn "Accuracy in results on wine classification: %A percents" accuracy

    System.Console.ReadLine() |> ignore
    ()
