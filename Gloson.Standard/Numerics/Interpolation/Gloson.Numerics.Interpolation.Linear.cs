using System;
using System.Collections.Generic;

namespace Gloson.Numerics.Interpolation {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class LinearSpline : BaseSpline {
    #region Algorithm

    private (double k, double b) Line(double x) {
      if (m_X.Count <= 0)
        return (double.NaN, double.NaN);
      else if (m_X.Count == 1)
        return (m_Y[0], 0);

      int index = Index(x);

      if (index < 0)
        index = 0;
      else if (index >= m_X.Count - 1)
        index = m_X.Count - 2;

      double x0 = m_X[index];
      double y0 = m_Y[index];

      double x1 = m_X[index + 1];
      double y1 = m_Y[index + 1];

      return ((y0 - y1) / (x0 - x1), (x1 * y0 - y1 * x0) / (x1 - x0));
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public LinearSpline(IEnumerable<(double x, double y)> source)
      : base(source) {
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compute At 
    /// </summary>
    public override double At(double x) {
      var (k, b) = Line(x);

      return k * x + b;
    }

    /// <summary>
    /// Polynom At 
    /// </summary>
    public override Polynom PolynomAt(double x) {
      var (k, b) = Line(x);

      return new Polynom(new double[] { b, k });
    }

    /// <summary>
    /// Derivative At
    /// </summary>
    public override double DerivativeAt(double x) => Line(x).k;

    #endregion Public
  }

}
