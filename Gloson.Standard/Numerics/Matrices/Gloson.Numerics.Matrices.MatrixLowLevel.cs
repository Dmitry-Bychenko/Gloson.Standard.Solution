﻿using System;
using System.Collections.Generic;

namespace Gloson.Numerics.Matrices {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Low level algorithms on double[][]
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  internal static partial class MatrixLowLevel {
    #region Public

    #region Standard

    // Unit square matrix
    internal static Double[][] Unit(int size) {
      double[][] result = new double[size][];

      for (int i = size - 1; i >= 0; --i)
        result[i] = new Double[size];

      for (int i = size - 1; i >= 0; --i)
        result[i][i] = 1;

      return result;
    }

    // Zero Square Matrix
    internal static double[][] Zero(int size) {
      double[][] result = new double[size][];

      for (int i = size - 1; i >= 0; --i)
        result[i] = new double[size];

      return result;
    }

    internal static double[][] Clone(double[][] source) {
      if (source is null)
        return null;

      double[][] result = new double[source.Length][];

      for (int i = source.Length - 1; i >= 0; --i) {
        double[] src = source[i];

        if (src is null)
          continue;

        double[] res = new double[src.Length];

        Array.Copy(src, 0, res, 0, src.Length);

        result[i] = res;
      }

      return result;
    }

    internal static double[][] Transpose(double[][] value) {
      if (value is null)
        return null;

      int colCount = value.Length;
      int rowCount = value[^1].Length;

      Double[][] result = new Double[rowCount][];

      for (int i = rowCount - 1; i >= 0; --i) {
        Double[] line = new Double[colCount];
        result[i] = line;

        for (int j = colCount - 1; j >= 0; --j)
          line[j] = value[j][i];
      }

      return result;
    }

    internal static Double[][] Multiply(double[][] left, double[][] right) {
      int leftRows = left.Length;
      int leftCols = left[0].Length;

      int rightRows = right.Length;
      int rightCols = right[0].Length;

      if (leftCols != rightRows)
        throw new ArgumentOutOfRangeException(nameof(right));

      double[][] result = new Double[leftRows][];

      for (int r = leftRows - 1; r >= 0; --r) {
        double[] leftLine = left[r];
        double[] line = new Double[rightCols];

        result[r] = line;

        for (int c = rightCols - 1; c >= 0; --c) {
          double s = 0.0;

          for (int i = leftCols - 1; i >= 0; --i)
            s += leftLine[i] * right[i][c];

          line[c] = s;
        }
      }

      return result;
    }

    internal static double Determinant(double[][] value) {
      double[][] m = Clone(value);

      double result = 1.0;
      int size = m.Length;

      for (int i = 0; i < size; ++i) {
        double[] line = m[i];

        // Find out a line
        if (line[i] == 0.0) {
          bool found = false;

          for (int j = i + 1; j < size; ++j) {
            double[] newLine = m[j];

            if (newLine[i] != 0) {
              found = true;

              m[i] = newLine;
              m[j] = line;

              result = -result;  // < -1 each time!

              line = m[i];

              break;
            }
          }

          if (!found)
            return 0.0;
        }

        // Elimination
        double mm = line[i];

        for (int j = i + 1; j < size; ++j) {
          double[] curLine = m[j];
          double coef = -curLine[i] / mm;

          for (int k = i + 1; k < size; ++k)
            curLine[k] += coef * line[k];
        }
      }

      // Backward
      for (int i = size - 1; i >= 0; --i)
        result *= m[i][i];

      return result;
    }

    internal static int Rank(double[][] value) {
      if (value is null)
        return 0;

      double[][] m = Clone(value);

      int size = m.Length;
      int sizeC = m[0].Length;

      for (int i = 0; i < size; ++i) {
        if (i >= sizeC)
          break;

        double[] line = m[i];

        // Find out a line
        if (line[i] == 0.0) {
          Boolean found = false;

          for (int j = i + 1; j < size; ++j) {
            double[] newLine = m[j];

            if (newLine[i] != 0) {
              found = true;

              m[i] = newLine;
              m[j] = line;

              line = m[i];

              break;
            }
          }

          if (!found)
            continue;
        }

        // Elimination
        double mm = line[i];

        for (int j = i + 1; j < size; ++j) {
          double[] curLine = m[j];
          double coef = -curLine[i] / mm;

          for (int k = i; k >= 0; --k)
            curLine[k] = 0.0;

          for (int k = i + 1; k < sizeC; ++k)
            curLine[k] += coef * line[k];
        }
      }

      int result = 0;

      for (int i = 0; i < size; ++i) {
        Double[] line = m[i];
        Boolean found = false;

        for (int j = line.Length - 1; j >= 0; --j)
          if (line[j] != 0) {
            result += 1;
            found = true;

            break;
          }

        if (found)
          continue;

        return result;
      }

      return result;
    }

    internal static double[][] Inverse(double[][] value) {
      double[][] m = Clone(value);
      int size = m.Length;
      Double result = 1.0;

      Double[][] inverted = Unit(size);

      for (int i = 0; i < size; ++i) {
        Double[] line = m[i];
        Double[] lineInverted = inverted[i];

        // Find out a line
        if (line[i] == 0.0) {
          Boolean found = false;

          for (int j = i + 1; j < size; ++j) {
            Double[] newLine = m[j];

            if (newLine[i] != 0) {
              found = true;

              m[i] = newLine;
              m[j] = line;

              (inverted[i], inverted[j]) = (inverted[j], inverted[i]);

              result = -result;  // < -1 each time!

              line = m[i];
              lineInverted = inverted[i];

              break;
            }
          }

          if (!found)
            throw new ArgumentException("Matrix is degenerated and can't be inverted.", nameof(value));
        }

        // Elimination
        Double mm = line[i];

        for (int j = 0; j < size; ++j) {
          line[j] /= mm;
          lineInverted[j] /= mm;
        }

        for (int j = i + 1; j < size; ++j) {
          Double[] curLine = m[j];
          Double[] curLineInverted = inverted[j];

          Double coef = -curLine[i];

          for (int k = 0; k < size; ++k) {
            curLine[k] += coef * line[k];
            curLineInverted[k] += coef * lineInverted[k];
          }
        }
      }

      // Backward
      for (int r = size - 1; r >= 1; --r) {
        for (int i = r - 1; i >= 0; --i) {
          Double coef = -m[i][r];

          for (int j = size - 1; j >= 0; --j) {
            //m[i][j] = m[i][j] + coef * m[r][j];
            inverted[i][j] = inverted[i][j] + coef * inverted[r][j];
          }
        }
      }

      return inverted;
    }

    internal static double[] Solve(double[][] value) {
      if (value is null)
        return Array.Empty<double>();
      else if (value.Length <= 0)
        return Array.Empty<double>();
      else if ((value.Length + 1) != value[0].Length)
        return Array.Empty<double>();

      Double[][] m = Clone(value);

      int size = m.Length;
      int sizeC = size + 1;

      for (int i = 0; i < size; ++i) {
        Double[] line = m[i];

        // Find out a line
        if (line[i] == 0.0) {
          Boolean found = false;

          for (int j = i + 1; j < size; ++j) {
            Double[] newLine = m[j];

            if (newLine[i] != 0) {
              found = true;

              m[i] = newLine;
              m[j] = line;

              line = m[i];

              break;
            }
          }

          if (!found)
            return Array.Empty<double>();
        }

        // Elimination
        Double mm = line[i];

        for (int j = i + 1; j < size; ++j) {
          Double[] curLine = m[j];
          Double coef = -curLine[i] / mm;

          for (int k = i + 1; k < sizeC; ++k)
            curLine[k] += coef * line[k];
        }
      }

      Double[] result = new Double[size];

      // Backward 
      for (int i = size - 1; i >= 0; --i) {
        Double[] line = m[i];

        Double s = line[sizeC - 1];

        for (int j = i + 1; j < size; ++j)
          s -= result[j] * line[j];

        result[i] = s / line[i];
      }

      return result;
    }

    internal static Boolean IsPositiveDefined(double[][] value) {
      Double[][] m = Clone(value);

      Double result = 1.0;
      int size = m.Length;

      for (int i = 0; i < size; ++i) {
        Double[] line = m[i];

        // Find out a line
        if (line[i] == 0.0) {
          Boolean found = false;

          for (int j = i + 1; j < size; ++j) {
            Double[] newLine = m[j];

            if (newLine[i] != 0) {
              found = true;

              m[i] = newLine;
              m[j] = line;

              result = -result;  // < -1 each time!

              line = m[i];

              break;
            }
          }

          if (!found)
            return false;
        }

        // Elimination
        Double mm = line[i];

        for (int j = i + 1; j < size; ++j) {
          Double[] curLine = m[j];
          Double coef = -curLine[i] / mm;

          for (int k = i + 1; k < size; ++k)
            curLine[k] += coef * line[k];
        }
      }

      // Backward
      for (int i = size - 1; i >= 0; --i)
        if (result * m[i][i] <= 0)
          return false;

      return true;
    }

    internal static Boolean IsNegativeDefined(double[][] value) {
      Double[][] m = Clone(value);

      Double result = 1.0;
      int size = m.Length;

      for (int i = 0; i < size; ++i) {
        Double[] line = m[i];

        // Find out a line
        if (line[i] == 0.0) {
          Boolean found = false;

          for (int j = i + 1; j < size; ++j) {
            Double[] newLine = m[j];

            if (newLine[i] != 0) {
              found = true;

              m[i] = newLine;
              m[j] = line;

              result = -result;  // < -1 each time!

              line = m[i];

              break;
            }
          }

          if (!found)
            return false;
        }

        // Elimination
        Double mm = line[i];

        for (int j = i + 1; j < size; ++j) {
          Double[] curLine = m[j];
          Double coef = -curLine[i] / mm;

          for (int k = i + 1; k < size; ++k)
            curLine[k] += coef * line[k];
        }
      }

      // Backward
      for (int i = size - 1; i >= 0; --i) {
        Double v = result * m[i][i] * (-1.0 + 2.0 * (i % 2));

        if (v <= 0)
          return false;
      }

      return true;
    }

    #endregion Standard

    #region Decomposition

    internal static double[][] Cholesky(double[][] value) {
      int n = value.Length;

      double[][] result = Zero(n);

      for (int r = 0; r < n; r++)
        for (int c = 0; c <= r; c++)
          if (c == r) {
            double sum = 0;

            for (int j = 0; j < c; j++)
              sum += result[c][j] * result[c][j];

            result[c][c] = Math.Sqrt(value[c][c] - sum);
          }
          else {
            double sum = 0;

            for (int j = 0; j < c; j++)
              sum += result[r][j] * result[c][j];

            result[r][c] = 1.0 / result[c][c] * (value[r][c] - sum);
          }

      return result;
    }

    //http://www.seas.ucla.edu/~vandenbe/133A/lectures/qr.pdf
    // H1, H2, ..., Hn, R
    internal static IEnumerable<double[][]> HouseholderFactors(double[][] value) {
      double[][] A = new double[value.Length][];

      for (int r = 0; r < value.Length; ++r) {
        A[r] = new double[value[r].Length];

        Array.Copy(value[r], A[r], A[r].Length);
      }

      int columns = A[0].Length;
      int rows = A.Length;

      for (int c = 0; c < Math.Min(columns, rows); ++c) {
        double[] y = new double[A.Length - c];
        double norm = 0.0;

        for (int r = c; r < A.Length; ++r) {
          double v = A[r][c];
          norm += v * v;

          y[r - c] = v;
        }

        double was = y[0];

        y[0] += (was < 0 ? -1 : 1) * Math.Sqrt(norm);

        norm = norm - was * was + y[0] * y[0];

        double[][] result = new double[y.Length + c][];

        for (int r = 0; r < y.Length + c; ++r)
          result[r] = new double[y.Length + c];

        for (int r = 0; r < result.Length; ++r)
          result[r][r] = 1;

        for (int i = 0; i < y.Length; ++i)
          for (int j = 0; j < y.Length; ++j)
            result[i + c][j + c] = result[i + c][j + c] - 2 / norm * y[i] * y[j];

        double[][] B = new double[A.Length][];

        for (int r = 0; r < A.Length; ++r) {
          double[] line = new double[columns];

          if (r < columns)
            Array.Copy(A[r], line, line.Length);

          B[r] = line;
        }

        for (int i = c; i < B[0].Length; ++i)
          for (int j = c; j < B.Length; ++j) {
            double s = 0.0;

            for (int k = c; k < y.Length + c; ++k)
              s += result[j][k] * A[k][i];

            B[j][i] = s;
          }

        A = B;

        yield return result;
      }

      yield return A;
    }

    // QR
    internal static void QR(double[][] value, out double[][] q, out double[][] r) {
      int rows = value.Length;

      q = new double[rows][];

      for (int i = 0; i < q.Length; ++i) {
        q[i] = new double[q.Length];

        q[i][i] = 1.0;
      }

      r = value;

      double[][] f = null;

      foreach (var factor in HouseholderFactors(value)) {
        if (f is not null) {
          double[][] z = new double[f.Length][];

          for (int j = 0; j < f.Length; ++j)
            z[j] = new double[f.Length];

          for (int y = 0; y < f.Length; ++y)
            for (int x = 0; x < f.Length; ++x) {
              double s = 0;

              for (int k = 0; k < f.Length; ++k)
                s += q[y][k] * f[k][x];

              z[y][x] = s;
            }

          q = z;
        }

        r = factor;
        f = factor;
      }
    }

    #endregion Decomposition 

    #endregion Public
  }

}
