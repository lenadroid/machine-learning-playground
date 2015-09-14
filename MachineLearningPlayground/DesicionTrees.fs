module DesicionTrees

open System.Data
open Accord.IO
open Accord.MachineLearning.DecisionTrees
open Accord.MachineLearning.DecisionTrees.Learning
open Accord.Math
open Accord.Statistics.Analysis
open Accord.Statistics.Filters
open AForge

let decisionTreesExample () = 
    let data = new DataTable("Mitchell's Tennis Example")
    data.Columns.Add "Day" |> ignore
    data.Columns.Add "Outlook" |> ignore
    data.Columns.Add "Temperature" |> ignore
    data.Columns.Add("Humidity") |> ignore
    data.Columns.Add("Wind") |> ignore
    data.Columns.Add("PlayTennis") |> ignore
    data.Rows.Add("D1", "Sunny", "Hot", "High", "Weak", "No") |> ignore
    data.Rows.Add("D2", "Sunny", "Hot", "High", "Strong", "No") |> ignore
    data.Rows.Add("D3", "Overcast", "Hot", "High", "Weak", "Yes") |> ignore
    data.Rows.Add("D4", "Rain", "Mild", "High", "Weak", "Yes") |> ignore
    data.Rows.Add("D5", "Rain", "Cool", "Normal", "Weak", "Yes") |> ignore
    data.Rows.Add("D6", "Rain", "Cool", "Normal", "Strong", "No") |> ignore
    data.Rows.Add("D7", "Overcast", "Cool", "Normal", "Strong", "Yes") |> ignore
    data.Rows.Add("D8", "Sunny", "Mild", "High", "Weak", "No") |> ignore
    data.Rows.Add("D9", "Sunny", "Cool", "Normal", "Weak", "Yes") |> ignore
    data.Rows.Add("D10", "Rain", "Mild", "Normal", "Weak", "Yes") |> ignore
    data.Rows.Add("D11", "Sunny", "Mild", "Normal", "Strong", "Yes") |> ignore
    data.Rows.Add("D12", "Overcast", "Mild", "High", "Strong", "Yes") |> ignore
    data.Rows.Add("D13", "Overcast", "Hot", "Normal", "Weak", "Yes") |> ignore
    data.Rows.Add("D14", "Rain", "Mild", "High", "Strong", "No") |> ignore

    let codebook = new Codification(data, "Outlook", "Temperature", "Humidity", "Wind", "PlayTennis")

    // Translate our training data into integer symbols using our codebook:
    let symbols = codebook.Apply data
    let inputs = symbols.ToArray<int>("Outlook", "Temperature", "Humidity", "Wind")
    let outputs = symbols.ToArray<int>("PlayTennis")

    let attributes = [|
              new DecisionVariable("Outlook",     3); // 3 possible values (Sunny, overcast, rain)
              new DecisionVariable("Temperature", 3); // 3 possible values (Hot, mild, cool)  
              new DecisionVariable("Humidity",    2); // 2 possible values (High, normal)    
              new DecisionVariable("Wind",        2)  // 2 possible values (Weak, strong) 
            |]

    let classCount = 2 // 2 possible output values for playing tennis: yes or no

    let tree = new DecisionTree(attributes, classCount)

    let id3learning = new ID3Learning(tree)

    // Learn the training instances!
    id3learning.Run(inputs, outputs) |> ignore

    let answer = codebook.Translate("PlayTennis", tree.Compute(codebook.Translate("Sunny", "Hot", "High", "Strong")))

    System.Console.WriteLine("Calculate for: Sunny, Hot, High, Strong")
    System.Console.WriteLine("Answer: " + answer)

    System.Console.ReadLine() |> ignore
