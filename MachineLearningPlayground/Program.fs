module Main

open DesicionTrees
open LogisticRegression
open BikeRentalDemand
open PartialLeastSquaresAnalysis
open System.Data
open Accord.IO
open Accord.MachineLearning.DecisionTrees
open Accord.MachineLearning.DecisionTrees.Learning
open Accord.Math
open Accord.Statistics.Analysis
open Accord.Statistics.Filters
open AForge

#nowarn

[<EntryPoint>]
let main argv = 
    decisionTreesExample()
    logisticRegression()
    bikeDemand()
    partialLeastSquaresAnalysis()
    printfn "%A" argv
    0 // return an integer exit code
