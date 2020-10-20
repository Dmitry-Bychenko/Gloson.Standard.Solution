using System;
using System.Collections.Generic;
using System.Linq;
using Gloson.Numerics.Interpolation;
using Gloson.Numerics.Matrices;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Cubic Spline  
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CubicSpline : BaseSpline {
    #region Private Data

    private double[] m_A;
    private double[] m_B;
    private double[] m_C;
    private double[] m_D;

    #endregion Private Data

    #region Algorithm

    private bool CoreBuildSpecial() {
      if (m_X.Count > 4)
        return false;

      if (m_X.Count == 0) {
        m_A = new double[] { double.NaN };
        m_B = new double[] { 0 };
        m_C = new double[] { 0 };
        m_D = new double[] { 0 };
      }
      else if (m_X.Count == 1) {
        m_A = new double[] { m_Y[0] };
        m_B = new double[] { 0 };
        m_C = new double[] { 0 };
        m_D = new double[] { 0 };
      }
      else if (m_X.Count == 2) {
        m_A = new double[] { (m_Y[0] * m_X[1] - m_Y[1] * m_X[0]) / (m_X[1] - m_X[0]) };
        m_B = new double[] { (m_Y[1] - m_Y[0]) / (m_X[1] - m_X[0]) };
        m_C = new double[] { 0 };
        m_D = new double[] { 0 };
      }
      else if (m_X.Count == 3) {
        double[] s = MatrixLowLevel.Solve(new double[][] {
          new double[] { 1, m_X[0], m_X[0] * m_X[0], m_Y[0]},
          new double[] { 1, m_X[1], m_X[1] * m_X[1], m_Y[1]},
          new double[] { 1, m_X[2], m_X[2] * m_X[2], m_Y[2]},
        });

        m_A = new double[] { s[0] };
        m_B = new double[] { s[1] };
        m_C = new double[] { s[2] };
        m_D = new double[] { 0 };
      }
      else if (m_X.Count == 4) {
        double[] s = MatrixLowLevel.Solve(new double[][] {
          new double[] { 1, m_X[0], m_X[0] * m_X[0], m_X[0] * m_X[0] * m_X[0], m_Y[0]},
          new double[] { 1, m_X[1], m_X[1] * m_X[1], m_X[1] * m_X[1] * m_X[1], m_Y[1]},
          new double[] { 1, m_X[2], m_X[2] * m_X[2], m_X[2] * m_X[2] * m_X[2], m_Y[2]},
          new double[] { 1, m_X[3], m_X[3] * m_X[3], m_X[3] * m_X[3] * m_X[3], m_Y[3]},
        });

        m_A = new double[] { s[0] };
        m_B = new double[] { s[1] };
        m_C = new double[] { s[2] };
        m_D = new double[] { s[3] };
      }

      return true;
    }

    // http://www.astro.tsu.ru/OsChMet/7_7.html
    private void CoreBuild() {
      if (CoreBuildSpecial())
        return;
      
      int N = m_X.Count - 1;

      m_A = new double[N + 1];

      for (int i = 0; i < m_A.Length; ++i)
        m_A[i] = m_Y[i];

      double[] h = new double[N];

      for (int i = 0; i < N; ++i)
        h[i] = m_X[i + 1] - m_X[i];

      double[] delta = new double[N - 1];

      for (int i = 0; i < delta.Length; ++i)
        delta[i] = 6 * ((m_Y[i + 2] - m_Y[i + 1]) / h[i + 1] - (m_Y[i + 1] - m_Y[i]) / h[i]);

      (double a, double b, double c)[] data = new (double a, double b, double c)[N - 1];

      for (int i = 0; i < data.Length; ++i)
        data[i] = (h[i], 2 * (h[i] + h[i + 1]), h[i + 1]);

      TriDiagonalMatrix matrix = new TriDiagonalMatrix(data);

      double[] s = matrix.Solve(delta);

      m_C = new double[N + 1];

      for (int i = 0; i < s.Length; ++i)
        m_C[i + 1] = s[i];

      m_D = new double[N + 1];

      for (int i = 0; i < N; ++i)
        m_D[i] = (m_C[i + 1] - m_C[i]) / h[i];

      m_B = new double[N + 1];

      for (int i = 0; i < N; ++i)
        m_B[i] = (m_Y[i + 1] - m_Y[i]) / h[i] - m_C[i] * h[i] / 2 - (m_C[i + 1] - m_C[i]) * h[i] / 6;

      for (int i = 0; i < m_C.Length; ++i)
        m_C[i] /= 2;

      for (int i = 0; i < m_D.Length; ++i)
        m_D[i] /= 6;

      double x = m_X[N] - m_X[N - 1];

      m_B[N] = m_B[N - 1] + 2 * m_C[N - 1] * x + 3 * m_D[N - 1] * x * x;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public CubicSpline(IEnumerable<(double x, double y)> source) 
      : base(source) {

      CoreBuild();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Range
    /// </summary>
    public override (double from, double to) Range(double x) {
      if (m_X.Count <= 4)
        return (double.NegativeInfinity, double.PositiveInfinity);

      int index = Index(x);

      if (index < 0)
        return (double.NegativeInfinity, m_X[index]);
      else if (index >= m_X.Count - 1)
        return (m_X[index], double.PositiveInfinity);
      else
        return (m_X[index], m_X[index + 1]);
    }

    /// <summary>
    /// Polynom At 
    /// </summary>
    public override Polynom PolynomAt(double x) {
      if (m_X.Count <= 4)
        return new Polynom(new double[] { m_A[0], m_B[0], m_C[0], m_D[0] });

      int index = Index(x);

      Polynom poly;

      if (index < 0) {
        poly = new Polynom(new double[] { m_A[0], m_B[0] });

        return poly.WithShift(-m_X[0]);
      }
      else if (index >= m_X.Count - 1) {
        poly = new Polynom(new double[] { m_A[m_X.Count - 1], m_B[m_X.Count - 1] });

        return poly.WithShift(-m_X[m_X.Count - 1]);
      }

      poly = new Polynom(new double[] { m_A[index], m_B[index], m_C[index], m_D[index] });

      return poly.WithShift(-m_X[index]);
    }

    /// <summary>
    /// Compute At 
    /// </summary>
    public override double At(double x) {
      if (m_X.Count <= 4)
        return m_A[0] + x * (m_B[0] + x * (m_C[0] + x * m_D[0]));

      int index = Index(x);

      if (index < 0)
        return m_A[0] + (x - m_X[0]) * (m_B[0]);
      else if (index >= m_X.Count)
        return m_A[m_A.Length - 1] + (x - m_X[m_A.Length - 1]) * (m_B[m_A.Length - 1]);

      double v = (x - m_X[index]);

      return m_A[index] + v * (m_B[index] + v * (m_C[index] + v * m_D[index]));
    }

    /// <summary>
    /// Derivative At
    /// </summary>
    public override double DerivativeAt(double x) {
      if (m_X.Count <= 4)
        return m_B[0] + 2 * x * m_C[0] + 3 * x * x * m_D[0];

      int index = Index(x);

      if (index < 0)
        return m_B[0];
      else if (index >= m_X.Count)
        return m_B[m_A.Length - 1];

      double v = (x - m_X[index]);

      return m_B[index] + 2 * v * m_C[index] + 3 * v * v * m_D[index];
    }

    #endregion Public
  }

}
