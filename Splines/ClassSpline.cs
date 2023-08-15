using System.Collections.Immutable;

namespace Project;

// Перечисление типов сплайнов.
public enum SplineType
{
   Interpolating,
   Smoothing,
   Unknown
};

public class Spline
{
   // Тип сплайна.
   public SplineType Type { get; init; }

   // Значения сетки по оси X.
   public ImmutableArray<double> _X { get; init; }

   // Значения функции по сетке.
   public ImmutableArray<double> _FX { get; init; }

   // Значения производной функции по сетке.
   public ImmutableArray<double> _F1X { get; set; }

   /// <summary>
   /// Метод, возвращающий размер сетки (ЧТО?).
   /// </summary>
   /// <returns>Размер в int.</returns>
   public int Size() => _FX.Length;

   /// <summary>
   /// Конструктор класса сплайна.
   /// </summary>
   /// <param name="_filePath">Файл с данными.</param>
   public Spline(string _filePath)
   {
      try
      {
         using var sr = new StreamReader(_filePath);
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
         _X = sr.ReadLine().Split().Select(double.Parse).ToImmutableArray();
         _FX = sr.ReadLine().Split().Select(double.Parse).ToImmutableArray();
      }
      catch (Exception ex)
      {
         throw new Exception("Exception during read file: ", ex);
      }
   }

   /// <summary>
   /// Метод, возвращающий значение функции в заданной точке.
   /// </summary>
   /// <param name="x">Точка.</param>
   /// <returns>Значение функции в точке.</returns>
   /// <exception cref="IndexOutOfRangeException">Проверка на принадлежность точки отрезку интерполяции.</exception>
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