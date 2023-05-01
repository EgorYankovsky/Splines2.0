using System.Collections.Immutable;

using MatrixLib;

namespace SolverLib;

public class Solver
{
   public Matrix A { get; init; }

   public Vector1 b { get; init; }

   public Solver(Matrix A, Vector1 b)
   {
      if (A.Size != b.Size)
         throw new System.Exception("Different sizes of Matrix and Vector1");

      this.A = A;
      this.b = b;
   }

   public Vector1 LU()
   {
      Matrix L = new(A.Size);
      Matrix U = new(A.Size);
      Vector1 y = new(A.Size);

      for (int i = 0; i < A.Size; i++)
      {
         U[i, i] = 1.0;
         if (i == 0)
         {
            L[i, i] = A[i, i];
            L[i + 1, i] = A[i + 1, i];
         }
         else if (i == A.Size - 1)
         {
            U[i - 1, i] = A[i - 1, i] / L[i - 1, i - 1];
            L[i, i] = A[i, i] - L[i, i - 1] * U[i - 1, i];
         }
         else
         {
            L[i + 1, i] = A[i + 1, i];
            U[i - 1, i] = A[i - 1, i] / L[i - 1, i - 1];
            L[i, i] = A[i, i] - L[i, i - 1] * U[i - 1, i];
         }
      }

      b[0] = b[0] / L[0, 0];

      for (int i = 1; i < A.Size; i++)
      {
         b[i] = b[i] / L[i, i] - L[i, i - 1] / L[i, i] * b[i - 1];
      }

      y = b;

      for (int i = y.Size - 2; i >= 0; i--)
      {
         y[i] = y[i] - U[i, i + 1] * y[i + 1];
      }
      return y;
   }
}