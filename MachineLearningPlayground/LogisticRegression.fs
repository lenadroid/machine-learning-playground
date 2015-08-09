module LogisticRegression

open Accord
open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Fitting

let logisticRegression () = 
    // Port of Accord.NET F# sample of logistic regression
    // Suppose we have the following data about some patients.
    // The first variable is continuous and represent patient
    // age. The second variable is dichotomic and give whether
    // they smoke or not (This is completely fictional data).

    let input = [|
                    [| 55.0; 0.0 |];
                    [| 28.0; 0.0 |];
                    [| 65.0; 1.0 |];
                    [| 46.0; 0.0 |];
                    [| 86.0; 1.0 |];
                    [| 56.0; 1.0 |];
                    [| 85.0; 0.0 |];
                    [| 33.0; 0.0 |];
                    [| 21.0; 1.0 |];
                    [| 42.0; 1.0 |];
                |]

    // We also know if they have had lung cancer or not, and 
    // we would like to know whether smoking has any connection
    // with lung cancer (This is completely fictional data).

    let output = [|0.0; 0.0; 0.0; 1.0; 1.0; 1.0; 0.0; 0.0; 0.0; 1.0|]

    // To verify this hypothesis, we are going to create a logistic
    // regression model for those two inputs (age and smoking).

    let regression = new LogisticRegression 2

    // Next, we are going to estimate this model. For this, we
    // will use the Iteratively Reweighted Least Squares method.

    let teacher = new IterativeReweightedLeastSquares(regression)

    // Now, we will iteratively estimate our model. The Run method returns
    // the maximum relative change in the model parameters and we will use
    // it as the convergence criteria.

    let rec teach () : unit = 
        match teacher.Run(input, output) with 
        | x when x > 0.001 -> teach ()
        | _ -> ()

    teach()

    // At this point, we can compute the odds ratio of our variables.
    // In the model, the variable at 0 is always the intercept term, 
    // with the other following in the sequence. Index 1 is the age
    // and index 2 is whether the patient smokes or not.

    // For the age variable, we have that individuals with
    //   higher age have 1.021 greater odds of getting lung
    //   cancer controlling for cigarette smoking.

    let ageOdds = regression.GetOddsRatio 1
    printfn "Age Odds %A" ageOdds

    // For the smoking/non smoking category variable, however, we
    //   have that individuals who smoke have 5.858 greater odds
    //   of developing lung cancer compared to those who do not 
    //   smoke, controlling for age (remember, this is completely
    //   fictional and for demonstration purposes only).

    let smokeOdds = regression.GetOddsRatio 2
    printfn "Smoke Odds %A" smokeOdds

    let computeOutput = regression.Compute([| 56.0; 1.0 |])
    printfn "Compute Output %A" computeOutput

    System.Console.ReadLine() |> ignore