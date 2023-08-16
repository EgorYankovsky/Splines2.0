using System.Collections.Immutable;

namespace Project;

public class Spline
{
   // Сетка по оси Х.
   public ImmutableArray<double> X { get; init; }

   // Значения функции в точках.
   public ImmutableArray<double> FX { get; init; }

   // Значения производных в точках.
   public ImmutableArray<double> F1X { get; set; }

   /// <summary>
   /// Метод, возвращающий значение сплайна в точке, принадлежащей отрезку. 
   /// </summary>
   /// <param name="x">Точка.</param>
   /// <returns>Значение функции.</returns>
   /// <exception cref="IndexOutOfRangeException">Проверка на принадлежность границам.</exception>
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
   /// <summary>
   /// Конструктор сглаживающего сплайна (в разработке).
   /// </summary>
   public SmoothingSpline()
   {
      throw new NotImplementedException();
   }
} 

public class InterpolatingSpline : Spline
{
   /// <summary>
   /// Конструктор интерполирующего сплайна.
   /// </summary>
   /// <param name="x">Сетка.</param>
   /// <param name="fx">Значения функции в узлах сетки.</param>
   /// <param name="Slvr">Метод решения СЛАУ.</param>
   public InterpolatingSpline(double[] x, double[] fx, Solver Slvr)
   {
      X = x.ToImmutableArray();
      FX = fx.ToImmutableArray();
      F1X = Slvr.Solve().GetVector();
   }
}