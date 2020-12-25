using System;

namespace Gloson.Numerics.Calculus {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Integrals
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Integrals {
    #region Public

    /// <summary>
    /// Simpson integration At
    /// </summary>
    public static double SimpsonAt(this Func<double, double> func, double from, double to, int points) {
      if (func is null)
        throw new ArgumentNullException(nameof(func));

      if (points <= 0)
        points = 1000;

      if (double.IsNaN(from))
        return double.NaN;
      else if (double.IsNaN(to))
        return double.NaN;

      if (double.IsNegativeInfinity(from))
        from = double.MinValue;
      else if (double.IsPositiveInfinity(from))
        from = double.MaxValue;

      if (double.IsNegativeInfinity(to))
        to = double.MinValue;
      else if (double.IsPositiveInfinity(to))
        to = double.MaxValue;

      if (to == from)
        return 0.0;

      points += points % 2;

      double result = 0.0;
      double h = (to - from) / points;

      for (int i = 1; i < points; ++i)
        result += (4 - ((i + 1) % 2) * 2) * func(from + i * h);

      result += (func(from) + func(to));

      result = result / 3.0 / points * (to - from);

      return result;
    }

    /// <summary>
    /// Simpson integration At
    /// </summary>
    public static double SimpsonAt(this Func<double, double> func, double from, double to) =>
      SimpsonAt(func, from, to, 0);

    #endregion Public
  }

}
