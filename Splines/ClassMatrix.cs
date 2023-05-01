using SplineLib;
using System.Collections.Immutable;
using System.Numerics;

namespace MatrixLib;

public class Matrix
{
   public int Size { get; init; }

   private List<double> _al = new();

   private List<double> _di = new();

   private List<double> _au = new();

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

   public void Show()
   {
      for (int i = 0; i < Size; i++)
      {
         for (int j = 0; j < Size; j++)
            Console.Write($"{this[i, j].ToString("E5")} ");
         Console.WriteLine();
      }
   }

}

public class Vector1
{
   public int Size { get; init; }

   private List<double> _elems = new();

   public double this[int i]
   {
      get => _elems[i];
      set => _elems[i] = value;
   }

   public Vector1(ImmutableArray<double> arr)
   {
      _elems = arr.ToList();
      Size = arr.Length;
   }

   public ImmutableArray<double> GetVector()
   {
      return _elems.ToImmutableArray<double>();
   }

   public Vector1(Spline spline)
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

   public Vector1(int Size)
   {
      this.Size = Size;
      for (int i = 0; i < Size; i++)
      {
         _elems.Add(0.0);
      }
   }

   public void Write(string outPath)
   {
      using (var sw = new StreamWriter(outPath))
      {
         foreach (double value in _elems)
         {
            sw.WriteLine($"{value.ToString("E5")} ");
         }
      }
   }

   public void Show()
   {
      foreach (double elem in _elems)
      {
         Console.WriteLine(elem.ToString("E5"));
      }
   }
}
