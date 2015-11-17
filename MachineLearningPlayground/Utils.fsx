#r @"..\packages\Accord.MachineLearning.2.15.0\lib\net45\Accord.MachineLearning.dll"
#r @"..\packages\Accord.Math.2.15.0\lib\net45\Accord.Math.dll"
#r @"..\packages\Accord.Neuro.2.15.0\lib\net45\Accord.Neuro.dll"
#r @"..\packages\Accord.Statistics.2.15.0\lib\net45\Accord.Statistics.dll"
#r @"..\packages\AForge.2.2.5\lib\AForge.dll"
#r @"..\packages\AForge.Genetic.2.2.5\lib\AForge.Genetic.dll"
#r @"..\packages\Accord.2.15.0\lib\net45\Accord.dll"
#r @"..\packages\AForge.Math.2.2.5\lib\AForge.Math.dll"
#r @"..\packages\AForge.Neuro.2.2.5\lib\AForge.Neuro.dll"
#r @"..\packages\FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"
#r @"..\packages\FSharp.Charting.0.90.13\lib\net40\FSharp.Charting.dll"
#r @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Windows.Forms.DataVisualization.dll"

let getColor n = 
    match n with
    | 0 -> System.Drawing.Color.Blue
    | 1 -> System.Drawing.Color.Green
    | 2 -> System.Drawing.Color.Gray
    | 3 -> System.Drawing.Color.Purple
    | _ -> System.Drawing.Color.Black
