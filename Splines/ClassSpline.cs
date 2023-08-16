using System.Collections.Immutable;

namespace Project;

public class Spline
{
   public ImmutableArray<double> X { get; init; }

   public ImmutableArray<double> FX { get; init; }

   public ImmutableArray<double> F1X { get; set; }

   public double this[double x]
   {
      get
      {
         if (x < X[0] && x > X[^1])
         {
            throw new IndexOutOfRangeException($"Point {x} out range");
         }

         int i = 0;
         while (x > X[i + 1])
            i++;


         double hk = X[i + 1] - X[i];
         double xi = (x - X[i]) / hk;

         return FX[i] * (1.0 - 3.0 * xi * xi + 2.0 * xi * xi * xi)
            + hk * F1X[i] * (xi - 2.0 * xi * xi + xi * xi * xi)
            + FX[i + 1] * (3.0 * xi * xi - 2.0 * xi * xi *xi)
            + hk * F1X[i + 1] * (-1.0 * xi * xi + xi * xi * xi);
      }
   }
   
}

public class SmoothingSpline : Spline
{
   public SmoothingSpline()
   {

   }
} 

public class InterpolatingSpline : Spline
{
   public InterpolatingSpline(double[] x, double[] fx, Solver Slvr)
   {
      X = x.ToImmutableArray();
      FX = fx.ToImmutableArray();
      F1X = Slvr.Solve().GetVector();
   }
}