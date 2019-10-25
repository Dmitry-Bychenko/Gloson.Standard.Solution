using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Gloson.Numerics.SpecialFunctions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Probability Integral
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ProbabilityIntegral {
    #region Public

    /// <summary>
    /// Standard error integral
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709")]
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public static Double Fi(Double value) {
      Double result = value;
      Double Add = value;
      Double i = 1.0;

      while (true) {
        Add = Add * 2 * value * value / i / (i + 1);

        if ((Add + result) == result)
          break;
        else if (Double.IsInfinity(result) || Double.IsNaN(result))
          break;

        result += Add;
        ++i;
      }

      return result * 2.0 / Math.Sqrt(Math.PI) * Math.Exp(-(value * value));
    }

    /// <summary>
    /// Static version of error integral
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709")]
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public static Double FiStat(Double value) {
      return (1.0 + Fi(value / Math.Sqrt(2.0))) / 2.0;
    }

    /// <summary>
    /// Erfectum (Erf)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709")]
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public static Double Erf(Double value) {
      if (value > 7)
        return 1.0;
      else if (value < -7)
        return -1.0;
      else if (value < 0)
        return -GammaFunctions.GammaLow(0.5, value * value) / Math.Sqrt(Math.PI);
      else
        return GammaFunctions.GammaLow(0.5, value * value) / Math.Sqrt(Math.PI);
    }

    /// <summary>
    /// Complimentary erfectum (ErfC)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709")]
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public static Double ErfC(Double value) {
      return 1.0 - Erf(value);
    }

    #endregion Public
  }

}
