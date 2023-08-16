using Project;

const string inputPath = @"Files\Input.txt";
const string outputPath = @"Files\Output.txt";

double[] x;
double[] fx;
using var sr = new StreamReader(inputPath);
{
   x = sr.ReadLine().Split().Select(double.Parse).ToArray();
   fx = sr.ReadLine().Split().Select(double.Parse).ToArray();
}

var mySpline = new InterpolatingSpline(x, fx, new LU(new Matrix(x), new Vector(x, fx)));
using var sw = new StreamWriter(outputPath);
for (int i = 0; i < (mySpline.X[^1] - mySpline.X[0]) / 0.05;  i++)
{
   sw.WriteLine($"P({mySpline.X[0] + i * 0.05:E4}) = {mySpline[mySpline.X[0] + i * 0.05]:E4}");
}