module PartialLeastSquaresAnalysis

open Accord
open Accord.Statistics.Analysis

// Port of Accord.NET C# example

// Following the small example by Hervé Abdi (Hervé Abdi, Partial Least Square Regression),
// we will create a simple example where the goal is to predict the subjective evaluation of
// a set of 5 wines. The dependent variables that we want to predict for each wine are its 
// likeability, and how well it goes with meat, or dessert (as rated by a panel of experts).
// The predictors are the price, the sugar, alcohol, and acidity content of each wine.


// Here we will list the inputs, or characteristics we would like to use in order to infer
// information from our wines. Each row denotes a different wine and lists its corresponding
// observable characteristics. The inputs are usually denoted by X in the PLS literature.

let partialLeastSquaresAnalysis () = 
    let inputs = array2D [|
                        // Wine  | Price | Sugar | Alcohol | Acidity
                        [|           7.0;     7.0;      13.0;        7.0     |];
                        [|           4.0;     3.0;      14.0;        7.0     |];
                        [|          10.0;     5.0;      12.0;        5.0     |];
                        [|          16.0;     7.0;      11.0;        3.0     |];
                        [|          13.0;     3.0;      10.0;        3.0     |]
                 |] 
    
    // Here we will list our dependent variables. Dependent variables are the outputs, or what we
    // would like to infer or predict from our available data, given a new observation. The outputs
    // are usually denoted as Y in the PLS literature.
    let outputs = array2D [|
                    [|           14.0;          7.0;                 8.0         |];
                    [|           10.0;          7.0;                 6.0         |];
                    [|            8.0;          5.0;                 5.0         |];
                    [|            2.0;          4.0;                 7.0         |];
                    [|            6.0;          2.0;                 4.0         |]
                |]

    // Next, we will create our Partial Least Squares Analysis passing the inputs (values for 
    // predictor variables) and the associated outputs (values for dependent variables).

    // We will also be using the using the Covariance Matrix/Center method (data will only
    // be mean centered but not normalized) and the SIMPLS algorithm. 
    let pls = new PartialLeastSquaresAnalysis(inputs, outputs, AnalysisMethod.Center, PartialLeastSquaresAlgorithm.SIMPLS)

    // Compute the analysis with all factors. The number of factors
    // could also have been specified in a overload of this method.

    pls.Compute()

    // After computing the analysis, we can create a linear regression model in order
    // to predict new variables. To do that, we may call the CreateRegression() method.

    let regression = pls.CreateRegression()

    // After the regression has been created, we will be able to classify new instances. 
    // For example, we will compute the outputs for the first input sample:

    let y = regression.Compute([| 7.0; 7.0; 13.0; 7.0 |])

    // The y output will be very close to the corresponding output used as reference.
    // In this case, y is a vector of length 3 with values { 13.98, 7.00, 7.75 }.
    
    printfn "Compute bike regression %A" y

    System.Console.ReadLine() |> ignore
    ()
