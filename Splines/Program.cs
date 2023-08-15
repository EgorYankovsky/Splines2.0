using Project;

const string inputPath = @"D:\CodeRepos\CS\Splines\Splines\Files\Input.txt";
const string outputPath = @"D:\CodeRepos\CS\Splines\Splines\Files\Output.txt";

Spline mySpline = new(inputPath);
Matrix A = new(mySpline);
Vector b = new(mySpline);
Solver mySolver = new(A, b);
Vector x = mySolver.LU();
mySpline._F1X = x.GetVector();
using var sw = new StreamWriter(outputPath);
sw.WriteLine(x.ToString());
for (int i = 0; i < (mySpline._X[^1] - mySpline._X[0]) / 0.05;  i++)
{
   Console.WriteLine($"P({mySpline._X[0] + i * 0.05}) = {mySpline[mySpline._X[0] + i * 0.05].ToString("E4")}");
}