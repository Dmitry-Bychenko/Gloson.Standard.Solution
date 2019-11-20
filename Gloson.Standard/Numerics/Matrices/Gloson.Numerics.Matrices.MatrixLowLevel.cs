using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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

    // Unit square matrix
    internal static Double[][] Unit(int size) {
      Double[][] result = new Double[size][];

      for (int i = size - 1; i >= 0; --i)
        result[i] = new Double[size];

      for (int i = size - 1; i >= 0; --i)
        result[i][i] = 1;

      return result;
    }

    internal static double[][] Clone(double[][] source) {
      if (null == source)
        return null;

      double[][] result = new double[source.Length][];

      for (int i = source.Length - 1; i >= 0; --i) {
        double[] src = source[i];

        if (null == src)
          continue;

        double[] res = new double[src.Length];

        Array.Copy(src, 0, res, 0, src.Length);

        result[i] = res;
      }

      return result;
    }

    internal static double[][] Transpose(double[][] value) {
      if (Object.ReferenceEquals(null, value))
        return null;

      int colCount = value.Length;
      int rowCount = value[value.Length - 1].Length;

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
        throw new ArgumentOutOfRangeException("right");

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
      if (Object.ReferenceEquals(null, value))
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

              Double[] h = inverted[i];
              inverted[i] = inverted[j];
              inverted[j] = h;

              result = -result;  // < -1 each time!

              line = m[i];
              lineInverted = inverted[i];

              break;
            }
          }

          if (!found)
            throw new ArgumentException("Matrix is degenerated and can't be inverted.", "value");
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
      if (Object.ReferenceEquals(null, value))
        return new Double[0];
      else if (value.Length <= 0)
        return new Double[0];
      else if ((value.Length + 1) != value[0].Length)
        return new Double[0];

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
            return new Double[0];
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

    #endregion Public
  }

}
