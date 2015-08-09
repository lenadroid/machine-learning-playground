module BikeRentalDemand

open Accord
open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Linear
open Accord.Statistics.Models.Regression.Fitting
open FSharp.Data

// instant,dteday,season,yr,mnth,holiday,weekday,workingday,weathersit,temp,atemp,hum,windspeed,casual,registered,cnt

let getBikeData () = 
    let bikes = CsvFile.Load(__SOURCE_DIRECTORY__ + "\\BikeSharingDataset\\day.csv").Cache()
    let inputData = bikes.Rows 
                    |> Seq.map (fun x ->  
                                    printfn "%A,  %A - %A" (x.GetColumn "temp") (x.GetColumn "workingday") (x.GetColumn "cnt") 
                                    [|  float(x.GetColumn "season"); 
                                        float(x.GetColumn "yr"); 
                                        float(x.GetColumn "mnth"); 
                                        float(x.GetColumn "holiday"); 
                                        float(x.GetColumn "weekday"); 
                                        float(x.GetColumn "workingday"); 
                                        float(x.GetColumn "weathersit"); 
                                        float(x.GetColumn "temp"); 
                                        float(x.GetColumn "atemp");
                                        float(x.GetColumn "hum");
                                        float(x.GetColumn "windspeed");
                                        float(x.GetColumn "casual");
                                        float(x.GetColumn "registered");
                                    |]
                                ) 
                    |> Seq.toArray
    let outputData = bikes.Rows 
                    |> Seq.map (fun x -> [|float(x.GetColumn "cnt")|]) 
                    |> Seq.toArray
    (inputData, outputData)

let bikeDemand () = 
    let input, output = getBikeData () 

//    // This is for univariate regression...
//    let simpleRegression = SimpleLinearRegression()
//    simpleRegression.Regress(input, output) |> ignore
//    let y = simpleRegression.Compute(0.215833)
//    printfn "Compute bike regression %A" y

    // Create a new multivariate linear regression with 2 inputs and 1 output
    let regression  = new MultivariateLinearRegression(13, 1)
    regression.Regress(input, output) |> ignore
    let y = regression.Compute([|3.0; 1.0; 5.0; 0.0; 1.0; 0.0; 2.0; 0.215833; 0.203487; 0.2775; 0.104846; 539.0; 2290.0|])
    printfn "Compute bike regression %A" y

//    let regression = new LogisticRegression 2
//
//    // Next, we are going to estimate this model. For this, we
//    // will use the Iteratively Reweighted Least Squares method.
//
//    let teacher = new IterativeReweightedLeastSquares(regression)
//
//    // Now, we will iteratively estimate our model. The Run method returns
//    // the maximum relative change in the model parameters and we will use
//    // it as the convergence criteria.
//    let input, output = getBikeData () 
//    let rec teach () : unit = 
//        match teacher.Run(input, output) with 
//        | x when x > 0.001 -> teach ()
//        | _ -> ()
//
//    teach()
//
//    // At this point, we can compute the odds ratio of our variables.
//    // In the model, the variable at 0 is always the intercept term, 
//    // with the other following in the sequence. Index 1 is the age
//    // and index 2 is whether the patient smokes or not.
//
//    // For the age variable, we have that individuals with
//    //   higher age have 1.021 greater odds of getting lung
//    //   cancer controlling for cigarette smoking.
//
//    let temp = regression.GetOddsRatio 1
//    printfn "temp Odds %A" temp
//
//    // For the smoking/non smoking category variable, however, we
//    //   have that individuals who smoke have 5.858 greater odds
//    //   of developing lung cancer compared to those who do not 
//    //   smoke, controlling for age (remember, this is completely
//    //   fictional and for demonstration purposes only).
//
//    let workingday = regression.GetOddsRatio 2
//    printfn "workingday Odds %A" workingday
//
//    let computeOutput = regression.Compute([| 0.363625; 1.0 |])
//    printfn "Compute Bike Output %A" computeOutput

    System.Console.ReadLine() |> ignore
    ()