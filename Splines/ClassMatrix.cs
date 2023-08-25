using System.Collections.Immutable;

namespace Project;

public class LocalMatrix
{
   private protected double[,] _values;

   public double this[int i, int j]
   {
      get => _values[i, j];
      set => _values[i, j] = value;
   }
}

public class LocalMatrixG : LocalMatrix 
{
   public LocalMatrixG (double h)
   {
      _values = new double[4, 4]
      {
         {1.2 / h, 0.1, -1.2 / h, 0.1},
         {0.1, 0.133333333333333 * h, -0.1, -0.033333333333333 * h},
         {-1.2 / h, -0.1, 1.2 / h, -0.1},
         {0.1, -0.033333333333333 * h, -0.1, 0.133333333333333 * h}
      };
   }
}

// TODO: Переставить в internal
public class LocalMatrixSecondDeriv : LocalMatrix
{
   public LocalMatrixSecondDeriv (double h)
   {
      _values = new double[4, 4] 
      {
         {60.0 / (h * h * h), 30.0 / (h * h), -60.0 / (h * h * h), 30.0 / (h * h)},
         {30.0 / (h * h), 16 / h, -30.0 / (h * h), 14.0 / h},
         {-60.0 / (h * h * h), -30.0 / (h * h), 60.0 / (h * h * h), -30.0 / (h * h)},
         {30.0 / (h * h), 14 / h, -30.0 / (h * h), 16.0 / h}
      };
   }
}


public class Matrix
{
   // Массив ссылок на элементы матрицы.
   private int[]? _ig;

   // Массив ссылок на элементы матрицы.
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
   ///
   /// Метод, возвращающий значение матрицы на i-ой строке и j-ом столбце. 
   /// </summary>
   /// <param name="i">Строка.</param>
   /// <param name="j">Столбец.</param>
   /// <returns>Значение матрицы.</returns>
   public double this[int i, int j]
   {
      get
      {
         return (j - i) switch
         {
               -1 => _al[j + 1],
               0 => _di[j],
               1 => _au[j - 1],
               _ => 0.0,
         };
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

   /// <summary>
   /// Конструктор матрицы.
   /// </summary>
   /// <param name="x">Массив узлов.</param>
   /// <param name="elems">Массив ссылок на элементы.</param>
   /// <param name="gamma">Параметр гамма.</param>
   /// <param name="alpha">Параметр альфа.</param>
   /// <param name="beta">Параметр бета.</param>
   public Matrix(double[] x, int[] elems, double gamma, double alpha, double beta)
   {
      _ig = new int[2 * elems.Length + 1];
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

      _di = new double[2 * elems.Length];
      _au = new double[_ig[^1]];
      _al = new double[_ig[^1]];

      int fundamentalIterator = 0;
      for (int i = 0; i < elems.Length - 1; i++)
      {
         double h = x[elems[i + 1]] - x[elems[i]];
         var G = new LocalMatrixG(h);
         var SD = new LocalMatrixSecondDeriv(h);
         var localMatrix = new double[4, 4];
         for (int ii = 0; ii < 4; ii++)
         {
            for (int jj = 0; jj < 4; jj++)
            {
               int counter = 0;
               for (int k = elems[i]; k < elems[i + 1]; k++)
               {
                  localMatrix[ii, jj] += gamma * BasisFunction.GetValue(ii, h, (x[k] - x[elems[i]]) / h) * BasisFunction.GetValue(jj, h, (x[k] - x[elems[i]]) / h);
                  counter++;
               }
               if (counter < 4)
                  Console.WriteLine("Attention!\n"+
                  $"Between {x[elems[i]]} and {x[elems[i + 1]]} there are only {counter} points!\n"+
                  "Possible inaccuracies in calculations.");

               localMatrix[ii, jj] += alpha * G[ii, jj] + beta * SD[ii, jj];

            }  
         }


         int _i = 0;
         int[] pointers = new int[4] {fundamentalIterator,
                                      fundamentalIterator + 1,
                                      fundamentalIterator + 2,
                                      fundamentalIterator + 3};
         foreach (var pi in pointers)
         {
            int _j = 0;
            foreach (var pj in pointers)
            {
                  switch(pi - pj)
                  {
                     case 0:
                     {
                        _di[pi] += localMatrix[_i, _j];
                        break;
                     }
                     case < 0:
                     {
                        int ind = _ig[pj];

                        for (; ind <= _ig[pj + 1] - 1; ind++)
                              if (_jg[ind] == pi)
                                 break;

                        _au[ind] += localMatrix[_i, _j];
                        break;
                     }
                     case > 0:
                     {
                        int ind = _ig[pi];
                        
                        for (; ind <= _ig[pi + 1] - 1; ind++)
                              if (_jg[ind] == pj)
                                 break;

                        _al[ind] += localMatrix[_i, _j];
                        break;
                     }
                  }
                  _j++;
            }
            _i++;
         }
         fundamentalIterator += 2;
      }
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
   /// <param name="x">Массив точек.</param>
   /// <param name="elems">Целочисленный массив границ элементов.</param>
   /// <param name="fx">Массив значений функции.</param>
   /// <param name="omega">Параметр омега.</param>
   public Vector(double[] x, int[] elems, double[] fx, double omega)
   {
      _elems = new double[2 * elems.Length];

      int fundamentalIterator = 0;
      for (int i = 0; i < elems.Length - 1; i++)
      {
         double[] localVector = new double[4];

         double h = x[elems[i + 1]] - x[elems[i]]; 
         for (int ii = 0; ii < 4; ii++)
         {
            int counter = 0;
            for (int k = elems[i]; k < elems[i + 1]; k++)
            {
               localVector[ii] += omega * BasisFunction.GetValue(ii, h, (x[k] - x[elems[i]]) / h) * fx[k];
               counter++;
            }
            if (counter < 4)
               Console.WriteLine("Attention!\n"+
               $"Between {x[elems[i]]} and {x[elems[i + 1]]} there are only {counter} points!\n"+
               "Possible inaccuracies in calculations.");
         }

         for (int j = 0; j < 4; j++)
            _elems[j + fundamentalIterator] += localVector[j];
         fundamentalIterator += 2;
      }
   }


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
