#load "Utils.fsx"
#load "..\packages\FsLab.0.3.10\FsLab.fsx"

open System
open Utils
open FSharp.Data
open Accord
open Accord.Math
open Accord.MachineLearning
open FSharp.Charting
open System.Drawing
open XPlot.GoogleCharts

let wb = WorldBankData.GetDataContext()

wb.Countries.``United Kingdom``.Indicators.``Adjusted savings: gross savings (% of GNI)``.[2010]

let data =
 [| for c in wb.Countries do
    let birthRate = c.Indicators.``Birth rate, crude (per 1,000 people)``.[2013]
    let nationalIncome = c.Indicators.``Adjusted net national income (annual % growth)``.[2010]
    let savings = c.Indicators.``Adjusted net savings, including particulate emission damage (current US$)``.[2010]
    let educationExpenditure = c.Indicators.``Adjusted savings: education expenditure (current US$)``.[2010]
    let grossSavings = c.Indicators.``Adjusted savings: gross savings (% of GNI)``.[2010]
    if (not(Double.IsNaN(birthRate)) && 
        not(Double.IsNaN(nationalIncome)) &&
        not(Double.IsNaN(savings)) &&
        not(Double.IsNaN(educationExpenditure)) &&
        not(Double.IsNaN(grossSavings))) 
    then yield (c.Name, [| birthRate; 
                           nationalIncome; 
                           savings; 
                           educationExpenditure;
                           grossSavings |])
 |]

let indicators = 
    data |> Array.map (fun (country, indicators) -> indicators)

let kmeans = new KMeans 4
kmeans.Tolerance <- 1e-7

let clusterColors = [|"green"; "blue"; "red"; "yellow"|]

let clusters = kmeans.Compute indicators

let mapsData = data |> Array.map (fun (country, _) -> country)

let geoData = Array.zip mapsData clusters

Chart.Geo(geoData)
|> Chart.WithOptions
    (Options(colorAxis=ColorAxis(colors = clusterColors)))

