using SplineLib;
using MatrixLib;
using SolverLib;
using System.Numerics;

const string inputPath = @"D:\CodeRepos\CS\Splines\Splines\Files\Input.txt";
const string outputPath = @"D:\CodeRepos\CS\Splines\Splines\Files\Output.txt";

Spline mySpline = new(inputPath);
Matrix A = new(mySpline);
Vector1 b = new(mySpline);
Solver mySolver = new(A, b);
Vector1 x = mySolver.LU();
x.Write(outputPath);