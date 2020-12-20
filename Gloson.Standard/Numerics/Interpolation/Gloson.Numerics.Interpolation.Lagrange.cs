using Gloson.Numerics.Matrices;
using System.Collections.Generic;

namespace Gloson.Numerics.Interpolation {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Lagrange Spline
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class LagrangeSpline : BaseSpline {
    #region Private Data

    double[] m_A;

    Polynom m_Polynom;

    #endregion Private Data

    #region Algorithm

    private void CoreBuild() {
      if (m_X.Count == 0) {
        m_A = new double[] { double.NaN };

        m_Polynom = Polynom.NaN;

        return;
      }
      else if (m_X.Count == 1) {
        m_A = new double[] { m_Y[0] };

        m_Polynom = new Polynom(m_A);

        return;
      }

      double[][] M = new double[m_Y.Count][];

      for (int r = 0; r < M.Length; ++r) {
        double[] row = new double[M.Length + 1];

        row[M.Length] = m_Y[r];

        double x = m_X[r];
        double v = 1;

        for (int i = 0; i < M.Length; ++i) {
          row[i] = v;

          v *= x;
        }

        M[r] = row;
      }

      m_A = MatrixLowLevel.Solve(M);
      m_Polynom = new Polynom(m_A);
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public LagrangeSpline(IEnumerable<(double x, double y)> source)
      : base(source) {

      CoreBuild();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compute At 
    /// </summary>
    public override double At(double x) {
      double result = 0;
      double v = 1.0;

      for (int i = 0; i < m_A.Length; ++i) {
        result += m_A[i] * v;

        v *= x;
      }

      return result;
    }

    /// <summary>
    /// Range
    /// </summary>
    public override (double from, double to) Range(double x) =>
      (double.NegativeInfinity, double.PositiveInfinity);

    /// <summary>
    /// Polynom At 
    /// </summary>
    public override Polynom PolynomAt(double x) => m_Polynom;

    /// <summary>
    /// Derivative At
    /// </summary>
    public override double DerivativeAt(double x) {
      double result = 0;
      double v = 1.0;

      for (int i = 1; i < m_A.Length; ++i) {
        result += i * m_A[i] * v;

        v *= x;
      }

      return result;
    }

    #endregion Public
  }

}
