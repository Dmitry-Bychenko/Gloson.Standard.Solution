using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Numerics.Interpolation {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Abstract Interpolation Spline 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class BaseSpline {
    #region Private Data

    protected readonly List<double> m_X = new();
    protected readonly List<double> m_Y = new();

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public BaseSpline(IEnumerable<(double x, double y)> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      foreach (var (x, y) in source.OrderBy(p => p.x)) {
        m_X.Add(x);
        m_Y.Add(y);
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_X.Count;

    /// <summary>
    /// X
    /// </summary>
    public IReadOnlyList<double> X => m_X;

    /// <summary>
    /// Y
    /// </summary>
    public IReadOnlyList<double> Y => m_Y;

    /// <summary>
    /// Index [result..result + 1]
    /// </summary>
    public int Index(double x) {
      int result = m_X.BinarySearch(x);

      if (result < 0)
        result = ~result - 1;

      return result;
    }

    /// <summary>
    /// Range
    /// </summary>
    public virtual (double from, double to) Range(double x) {
      if (m_X.Count <= 1)
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
    /// Compute At 
    /// </summary>
    public abstract double At(double x);

    /// <summary>
    /// Polynom At 
    /// </summary>
    public abstract Polynom PolynomAt(double x);

    /// <summary>
    /// Derivative At
    /// </summary>
    public abstract double DerivativeAt(double x);

    #endregion Public
  }

}
