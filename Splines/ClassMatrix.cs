using System.Collections.Immutable;

namespace Project;

public class Matrix
{

   private int[]? _ig;

   private int[]? _jg;

   // Размер матрицы.
   public int Size { get; init; }

   // Компоненты матрицы нижнего треугольника.
   private double[]? _al;

   // Компоненты диагонали матрицы.
   private double[]? _di;

   // Компоненты верхнего треугольника матрицы.
   private double[]? _au;

   /// <summary>
   /// Метод, определяющий положительную определенность матрицы (в разработке).
   /// </summary>
   /// <returns>Логическое значение.</returns>
   public bool IsPositivelyDefined()
   {
      throw new NotImplementedException();
   }

   /// <summary>
   /// Конструктор класса.
   /// </summary>
   /// <param name="al">Нижний треугольник матрицы.</param>
   /// <param name="au">Верхний треугольник матрицы.</param>
   /// <param name="diag">Диагональные элементы.</param>
   public Matrix(double[] al, double[] au, double[] diag)
   {
      Size = diag.Length;
      _al = al;
      _di = diag;
      _au = au;
   }

   /// <summary>
   /// Метод, определяющий симметричность матрицы.
   /// </summary>
   /// <returns>Логическое значение.</returns>
   public bool IsSimmetrical()
   {
      for (int i = 0; i < Size - 1; i++)
         if (_al[i + 1] != _au[i])
            return false;
      return true;
   }

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
      _di = new double[Size];
      _al = new double[Size]; 
      _au = new double[Size];
   }

   public Matrix(double[] x, int[] elems, double gamma, double alpha, double beta)
   {
      _ig = new int[2 * (elems.Length) + 1];
      _ig[0] = 0;
      _ig[1] = 0;
      _ig[2] = 1;
      for (int i = 3; i < _ig.Length; i++)
         _ig[i] = _ig[i - 1] + 2 + (i + 1) % 2;

      _jg = new int[_ig[^1]];
      _jg[0] = 0;
      _jg[1] = 0;
      _jg[2] = 1;
      int lastValue = 0;

      for (int i = 3; i < _ig.Length - 1; i++)
      {
         int diff = _ig[i + 1] - _ig[i];
         for (int j = 0; j < diff; j++)
         {
            _jg[_ig[i] + j] = lastValue;
            lastValue++;
         }
         if (i % 2 == 0)
            lastValue--;
         lastValue--;
      }
      _jg[^1] = lastValue;
   }

   /// <summary>
   /// Конструктор класса.
   /// </summary>
   /// <param name="x">Сетка по оси Х.</param>
   public Matrix(double[] x)
   {
      Size = x.Length;
      _di = new double[x.Length];
      _al = new double[x.Length];
      _au = new double[x.Length];

      double[] h = new double[x.Length - 1];
      for (int i = 0; i < h.Length; i++)
         h[i] = x[i + 1] - x[i];

      _di[0] = 1.0;
      _al[0] = 0.0;
      _au[0] = 0.0;

      for (int i = 1; i < Size - 1; i++)
      {
         _al[i] = 2.0 / h[i - 1];
         _di[i] = 4.0 * (h[i] + h[i - 1]) / (h[i] * h[i - 1]);
         _au[i] = 2.0 / h[i];
      }

      _di[^1] = 1.0;
      _al[^1] = 0.0;
      _au[^1] = 0.0;

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
   private double[]? _elems;

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
   public Vector(double[] arr)
   {
      _elems = arr;
      Size = arr.Length;
   }

   // ? Зачем надо?
   /// <summary>
   /// Метод, возвращающий вектор.
   /// </summary>
   /// <returns></returns>
   public ImmutableArray<double> GetVector() => _elems.ToImmutableArray();

   /// <summary>
   /// Конструктор класса.
   /// </summary>
   /// <param name="x">Сетка по оси Х.</param>
   /// <param name="fx">Значения функции в сетке.</param>
   public Vector(double[] x, double[] fx)
   {
      _elems = new double[x.Length];
      Size = x.Length;

      double[] h = new double[x.Length - 1];
      for (int i = 0; i < x.Length - 1; i++)
         h[i] = x[i + 1] - x[i];

      _elems[0] = -1.0 * (2.0 * h[1] + h[2]) / (h[1] * (h[1] + h[2])) * fx[1] +
                  (h[1] + h[2]) / (h[1] * h[2]) * fx[2] - h[1] / ((h[1] + h[2]) * h[2]) * fx[3];

      for (int i = 1; i < fx.Length - 1; i++)
         _elems[i] = 6.0 * ((fx[i] - fx[i - 1]) / (h[i - 1] * h[i - 1]) +
                             (fx[i + 1] - fx[i]) / (h[i] * h[i]));

      _elems[^1] = 1.0 / (h[^2] + h[^1]) * (h[^1] / h[^2] * fx[^3] + (2 + h[^2] / h[^1]) * fx[^1]) -
                          (h[^2] + h[^1]) / (h[^1] * h[^2]) * fx[^2];
   }

   /// <summary>
   /// Конструктор вектора.
   /// </summary>
   /// <param name="Size">Размер вектора.</param>
   public Vector(int Size)
   {
      this.Size = Size;
      _elems = new double[Size];
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
