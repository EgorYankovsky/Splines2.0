using System.Collections.Immutable;

namespace SplineLib;

public enum SplineType
{
   Interpolating,
   Smoothing,
   Unknown
};

public class Spline
{

   private List<double> _xMesh = new();

   private List<double> __fMesh = new();

   public SplineType Type { get; init; }

   public ImmutableArray<double> _X { get; init; }

   public ImmutableArray<double> _FX { get; init; }

   public ImmutableArray<double> _F1X { get; set; }

   public int GetLength() => _FX.Length;

   public Spline(string _filePath)
   {
      using (var sr = new StreamReader(_filePath))
      {
         try
         {
            string splineType = sr.ReadLine();
            if (splineType == "Interpolating" || splineType == "Интерполирующий")
               Type = SplineType.Interpolating;
            else if (splineType == "Smoothing" || splineType == "Сглаживающий")
               Type = SplineType.Smoothing;
            else
            {
               Console.WriteLine("Unkown spline type. Spline type set to Interpolating");
               Type = SplineType.Interpolating;
            }
            _X = sr.ReadLine().Split().Select(item => double.Parse(item)).ToImmutableArray<double>();
            _FX = sr.ReadLine().Split().Select(item => double.Parse(item)).ToImmutableArray<double>();
         }
         catch (Exception ex)
         {
            Console.WriteLine("Exception during reading file: ", ex);
         }
      }
   }

   public double this[double x]
   {
      get
      {
         if (x < _X[0] && x > _X[^1])
         {
            throw new IndexOutOfRangeException($"Point {x} out range");
         }

         int i = 0;
         while (x > _X[i + 1])
            i++;


         double hk = _X[i + 1] - _X[i];
         double xi = (x - _X[i]) / hk;

         return _FX[i] * (1.0 - 3.0 * xi * xi + 2.0 * xi * xi * xi)
            + hk * _F1X[i] * (xi - 2.0 * xi * xi + xi * xi * xi)
            + _FX[i + 1] * (3.0 * xi * xi - 2.0 * xi * xi *xi)
            + hk * _F1X[i + 1] * (-1.0 * xi * xi + xi * xi * xi);
      }
   }
}