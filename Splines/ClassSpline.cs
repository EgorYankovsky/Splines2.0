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
   private SplineType _type = SplineType.Unknown;

   public ImmutableArray<double> _X { get; init; }

   public ImmutableArray<double> _FX { get; init; }

   public ImmutableArray<double> _F1X { get; init; }

   public int GetLength() => _FX.Length;

   public Spline(string _filePath)
   {
      using (var sr = new StreamReader(_filePath))
      {
         try
         {
            string splineType = sr.ReadLine();
            if (splineType == "Interpolating" || splineType == "Интерполирующий")
               _type = SplineType.Interpolating;
            else if (splineType == "Smoothing" || splineType == "Сглаживающий")
               _type = SplineType.Smoothing;
            else
            {
               Console.WriteLine("Unkown spline type. Spline type set to Interpolating");
               _type = SplineType.Interpolating;
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
}