using System.Collections.Immutable;
using System.Runtime.InteropServices.Marshalling;

namespace Project;

public class Matrix
{
   // Размер матрицы.
   public int Size { get; init; }

   // Компоненты матрицы нижнего треугольника.
   private List<double> _al = new();

   // Компоненты диагонали матрицы.
   private List<double> _di = new();

   // Компоненты верхнего треугольника матрицы.
   private List<double> _au = new();

   /// <summary>
   /// Метод, возвращающий значение матрицы на i-ой строке и j-ом столбце. 
   /// </summary>
   /// <param name="i">Строка.</param>
   /// <param name="j">Столбец.</param>
   /// <returns>Значение матрицы.</returns>
   public double this[int i, int j]
   {
      get
      {
         switch (j - i)
         {
            case -1:
               return _al[j + 1];
            case 0:
               return _di[j];
            case 1:
               return _au[j - 1];
            default:
               return 0.0;
         }
      }
      set
      {
         switch(j - i)
         {
            case -1:
               {
                  _al[j + 1] = value;
                  break;
               }
            case 0:
               {
                  _di[j] = value;
                  break;
               }
            case 1:
               {
                  _au[j - 1] = value;
                  break;
               }
         }
      }
   }

   /// <summary>
   /// Конструктор матрицы.
   /// </summary>
   /// <param name="Size">Размерность матрицы.</param>
   public Matrix(int Size)
   {
      this.Size = Size;

      for (int i = 0; i < this.Size; i++)
      {
         _al.Add(0.0);
         _di.Add(0.0);
         _au.Add(0.0);
      }
   }

   /// <summary>
   /// Конструктор матрицы.
   /// </summary>
   /// <param name="spline">Сплайн.</param>
   public Matrix(Spline spline)
   {
      List<double> h = new();

      for (int i = 1; i < spline._X.Length; i++)
      {
         h.Add(spline._X[i] - spline._X[i - 1]);
      }

      Size = spline._X.Length;

      _al.Add(-0.0);
      _di.Add(1.0);
      _au.Add(0.0);

      for (int i = 1; i < spline._X.Length - 1; i++)
      {
         _al.Add(2.0 / h[i - 1]);
         _di.Add(4.0 * (1.0 / h[i - 1] + 1.0 / h[i]));
         _au.Add(2.0 / h[i]);
      }

      _au.Add(-0.0);
      _di.Add(1.0);
      _al.Add(0.0);
   }

   /// <summary>
   /// Метод, возвращающий матрицу в виде строки.
   /// </summary>
   /// <returns></returns>
   public override string ToString()
   {
      string ans = "";
      for (int i = 0; i < Size; i++)
      {
         for (int j = 0; j < Size; j++)
         {
            ans += $"{this[i, j]:E5} ";
         }
         ans += "\n";
      }
      return ans;
   }
}

public class Vector
{
   // Размер вектора.
   public int Size { get; init; }

   // Элементы вектора.
   private List<double> _elems = new();

   /// <summary>
   /// Метод, возвращающий i-ое значение вектора.
   /// </summary>
   /// <param name="i">Индекс.</param>
   /// <returns>Значение вектора.</returns>
   public double this[int i]
   {
      get => _elems[i];
      set => _elems[i] = value;
   }

   /// <summary>
   /// Конструктор вектора.
   /// </summary>
   /// <param name="arr">Массив.</param>
   public Vector(ImmutableArray<double> arr)
   {
      _elems = arr.ToList();
      Size = arr.Length;
   }

   // ? Зачем надо?
   /// <summary>
   /// Метод, возвращающий вектор.
   /// </summary>
   /// <returns></returns>
   public ImmutableArray<double> GetVector() => _elems.ToImmutableArray();

   /// <summary>
   /// Конструктор вектора.
   /// </summary>
   /// <param name="spline">Сплайн.</param>
   public Vector(Spline spline)
   {
      List<double> h = new();

      for (int i = 1; i < spline._X.Length; i++)
      {
         h.Add(spline._X[i] - spline._X[i - 1]);
      }

      Size = spline._X.Length;


      _elems.Add(-1.0 * (2.0 * h[1] + h[2]) / (h[1] * (h[1] + h[2])) * spline._FX[1] +
       (h[1] + h[2]) / (h[1] * h[2]) * spline._FX[2] - h[1] / ((h[1] + h[2]) * h[2]) * spline._FX[3]);

      for (int i = 1; i < spline._FX.Length - 1; i++)
      {
         _elems.Add(6.0 * ((spline._FX[i] - spline._FX[i - 1]) / (h[i - 1] * h[i - 1]) +
         (spline._FX[i + 1] - spline._FX[i]) / (h[i] * h[i])));
      }

      int n = spline._X.Length - 1;

      _elems.Add(1.0 / (h[n - 2] + h[n - 1]) * (h[n - 1] / h[n - 2] * spline._FX[n - 2] + (2 + h[n - 2] / h[n - 1]) * spline._FX[n]) -
       (h[n - 2] + h[n - 1]) / (h[n - 1] * h[n - 2]) * spline._FX[n - 1]);
   }

   /// <summary>
   /// Конструктор вектора.
   /// </summary>
   /// <param name="Size">Размер вектора.</param>
   public Vector(int Size)
   {
      this.Size = Size;
      for (int i = 0; i < Size; i++)
      {
         _elems.Add(0.0);
      }
   }

   /// <summary>
   /// Метод, возвращающий вектор в виде строки.
   /// </summary>
   /// <returns>Вектор в виде строки.</returns>
   public override string ToString()
   {
      string ans = "";
      foreach (var element in _elems)
         ans += $"{element:E5}\n";
      return ans;
   }
}
