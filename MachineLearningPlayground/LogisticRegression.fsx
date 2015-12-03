#load "Utils.fsx"

open System
open Accord
open Accord.Math
open FSharp.Data
open Accord.Statistics
open Accord.MachineLearning
open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Fitting

let logisticRegression () = 
    // The first variable is a person's age. 
    // The second variable gives whether they smoke or not
    // The output indicates wherher they have had lung cancer or not
    let input = [|
                    [| 55.; 0. |]; // will output 0
                    [| 28.; 0. |]; // will output 0
                    [| 65.; 1. |]; // will output 0
                    [| 46.; 0. |]; // will output 1
                    [| 86.; 1. |]; // will output 1
                    [| 56.; 1. |]; // will output 1
                    [| 85.; 0. |]; // will output 0
                    [| 33.; 0. |]; // will output 0
                    [| 21.; 1. |]; // will output 0
                    [| 42.; 1. |]; // will output 1
                |]

    let output = [|0.; 0.; 0.; 1.; 1.; 1.; 0.; 0.; 0.; 1.|]

    // And we would like to know whether smoking has any connection
    // with lung cancer

    // To verify this guess, we are going to create a logistic
    // regression model for those two inputs (age and smoking).

    let regression = new LogisticRegression 2

    // Next, we are going to estimate this model

    let teacher = new IterativeReweightedLeastSquares(regression)
    
    // teaching until we are satisfied 

    let rec teach () : unit = 
        match teacher.Run(input, output) with 
        | x when x > 0.001 -> teach ()
        | _ -> ()

    teach()

    // ready to use the model for predictions

    let computeOutput = regression.Compute([| 19.; 0. |])
    printfn "High probability of lung cancer - %A" (computeOutput >= 0.5)


// What if we want to have more inputs?