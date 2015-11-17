#load "Utils.fsx"

open System
open Accord
open Accord.Math
open FSharp.Data
open Accord.Statistics
open Accord.MachineLearning
open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Fitting

let trainData = __SOURCE_DIRECTORY__ + "\\WineDataset\\wine.training.data"
let testData = __SOURCE_DIRECTORY__ + "\\WineDataset\\wine.testing.data"

type Wine = CsvProvider<"WineDataset\\wine.training.data">

let wineTrain = Wine.Load(trainData)
let wineTest = Wine.Load(testData)
let classDataOffset = 1

let inputs = 
    wineTrain.Rows 
    |> Seq.map 
        (fun row -> [|
                        row.Alcohol;
                        row.MalicAcid;
                        row.Ash;
                        row.AlcalinityOfAsh;
                        row.Magnesium |> decimal;
                        row.TotalPhenols;
                        row.Flavanoids;
                        row.NonflavanoidPhenols;
                        row.Proanthocyanins;
                        row.ColorIntensity;
                        row.OD280OD315OfDilutedWines;
                        row.Hue;
                        row.Proline |> decimal;
                    |] |> Seq.map float |> Seq.toArray)
    |> Seq.toArray

let classes = 
    wineTrain.Rows 
    |> Seq.map (fun row -> row.Class - classDataOffset) 
    |> Seq.toArray

let columnMinsAndMax = 
    [0 .. (wineTrain.NumberOfColumns - 2)] 
    |> Seq.map (fun i -> inputs.GetColumn(i).Min(), inputs.GetColumn(i).Max())
    |> Seq.toArray

let normalize minmax i (value: float) = 
    (value - (fst (Array.get minmax i)))/((snd (Array.get minmax i)) - (fst (Array.get minmax i)))

let normalizedData = 
    inputs |> 
    Array.map(fun row -> row |> Array.mapi(fun columnNumber value -> 
                                               normalize columnMinsAndMax columnNumber value))

// Create a new Multinomial Logistic Regression for 3 categories
let mlr = new MultinomialLogisticRegression(13,3)

// Create a estimation algorithm to estimate the regression
let lbnr = new LowerBoundNewtonRaphson(mlr)

// Now, we will iteratively estimate our model. The Run method returns
// the maximum relative

let mutable iter = 0
let rec teach () : unit = 
    iter <- iter + 1
    match lbnr.Run(normalizedData, classes) with 
    | x when (x > 1e-4 && iter < 1000) -> printfn "%A" x; teach ();
    | _ -> ()
    
teach()

let getPredictedClassFrom outputLayer offset = 
    Array.IndexOf(outputLayer,(Array.max outputLayer)) + offset

let test = wineTest.Rows |> Seq.map 
                                (fun row -> [|
                                                row.Alcohol;
                                                row.MalicAcid;
                                                row.Ash;
                                                row.AlcalinityOfAsh;
                                                row.Magnesium |> decimal;
                                                row.TotalPhenols;
                                                row.Flavanoids;
                                                row.NonflavanoidPhenols;
                                                row.Proanthocyanins;
                                                row.ColorIntensity;
                                                row.OD280OD315OfDilutedWines;
                                                row.Hue;
                                                row.Proline |> decimal;
                                            |] |> Seq.map float |> Seq.toArray)
                             |> Seq.toArray

let testClasses = wineTest.Rows |> Seq.map (fun row -> row.Class - classDataOffset) |> Seq.toArray

let columnMinsAndMaxTest = 
    [0 .. (wineTest.NumberOfColumns - 2)] 
    |> Seq.map (fun i -> test.GetColumn(i).Min(), test.GetColumn(i).Max())
    |> Seq.toArray

let normalizedTestData = 
    test |> 
    Array.map(fun row -> row |> Array.mapi(fun columnNumber value -> 
                                               normalize columnMinsAndMaxTest columnNumber value))

let testAndCheckAccuracy (mlr: MultinomialLogisticRegression) (data:float[][]) testCl =
    let correctGuesses = data 
                         |> Array.mapi (fun i row -> 
                            let outputLayer = mlr.Compute(row)
                            let predictedClass = getPredictedClassFrom outputLayer 0
                            printfn "%A - %A, %A, %A" i (predictedClass = Array.get testCl i) predictedClass (Array.get testCl i)
                            if (predictedClass = Array.get testCl i) then 1 else 0
                            ) 
                         |> Array.sum
    printfn "Correct guesses %A/%A" correctGuesses (Array.length data)
    float(correctGuesses)/float(Array.length data) * 100.0

let accuracy = testAndCheckAccuracy mlr normalizedTestData testClasses


