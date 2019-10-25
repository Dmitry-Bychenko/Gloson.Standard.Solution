using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Gloson.Numerics.SpecialFunctions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Gamma Function(s)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class GammaFunctions {
    #region Private data

    // Gamma threshold
    private const int s_GammaThreshold = 14;
    // Bernulli coeffs for calc
    private const int s_BernulliCount = 5;

    #endregion Private data

    #region Algorithm

    // Integer Factoial
    internal static Double IntFactorial(int value) {
      if (value <= 0)
        return Double.PositiveInfinity;
      else if (value <= 1)
        return 1.0;

      Double result = 1.0;

      for (int i = value; i > 1; --i)
        result *= i;

      return result;
    }

    // Log Stirling
    internal static Double LogStirling(Double x) {
      return Math.Log(2 * Math.PI * x) / 2 +
             x * Math.Log(x) - x +
             Math.Log(1 + 1 / (12 * x) + 1 / (288 * x * x) - 139 / (51840 * x * x * x) - 571 / (2488320 * x * x * x * x) + 163879 / (209018880 * x * x * x * x * x));
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Bernulli coefficients
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1502")]
    public static Double BernoulliCoefficient(int value) {
      if (value < 0)
        return 0;

      // Table
      switch (value) {
        case 0:
          return 1.0;
        case 1:
          return -1.0 / 2.0;
        case 2:
          return 1.0 / 6.0;
        case 4:
          return -1.0 / 30.0;
        case 6:
          return 1.0 / 42.0;
        case 8:
          return -1.0 / 30.0;
        case 10:
          return 5.0 / 66.0;
        case 12:
          return -691.0 / 2730.0;
        case 14:
          return 7.0 / 6.0;
        case 16:
          return -3617.0 / 510.0;
        case 18:
          return 43867.0 / 798.0;
        case 20:
          return -174611.0 / 330.0;
        case 22:
          return 854513.0 / 138.0;
        default: {
          if (value % 2 == 1)
            return 0;
          else
            break;
        }
      }

      // Explicit calculation
      const int precision = 100;

      Double result = 0.0;

      for (int i = 1; i < precision; ++i)
        result += System.Math.Pow(i, -value);

      result *= 2 * IntFactorial((int)value) / System.Math.Pow(2 * System.Math.PI, value);

      if (value % 4 == 0)
        result = -result;

      return result;
    }

    /// <summary>
    /// Gamma
    /// </summary>
    public static Double Gamma(this Double value) {
      if (value <= 0.0)
        if (System.Math.Round(value) == value)
          return Double.PositiveInfinity;

      // Negative to positive plus correction
      if (value < 0) {
        Double ADelta = System.Math.Round(-value + s_GammaThreshold);
        Double result = Gamma(value + ADelta);

        // Correction
        Double idx = 0;

        while (idx < ADelta - 0.5) {
          idx += 1.0;

          result /= (value + idx);
        }

        return result;
      }

      // Positive only.
      // Shift if arg1 is too small
      if ((value > 0.0) && (value < s_GammaThreshold)) {
        Double result = Gamma(value + s_GammaThreshold);

        for (int i = 0; i < s_GammaThreshold; ++i)
          result /= (value + i);

        return result;
      }

      // Standard (big) value through Bernulli coefficients
      Double ARes = 0.0;

      for (int i = 1; i <= s_BernulliCount; ++i)
        ARes += BernoulliCoefficient(2 * i) / (2 * i) / (2 * i - 1) / System.Math.Pow(value, 2 * i - 1);

      return System.Math.Sqrt(2 * System.Math.PI) * System.Math.Pow(value, value - 0.5) *
             System.Math.Exp(-value) * System.Math.Exp(ARes);
    }

    /// <summary>
    /// Log Gamma
    /// </summary>
    public static Double LogGamma(Double value) {
      if (value <= 0.0)
        if (System.Math.Round(value) == value)
          return Double.PositiveInfinity;

      if (value > 0)
        if (value > 50.0)
          return LogStirling(value - 1);
        else
          return Math.Log(Gamma(value));
      else {
        Double sin = Math.Abs(Math.Sin(Math.PI * (1 + value)));

        if ((-value > 50.0) || (sin < 1e-40))
          return Math.Log(Math.PI) - Math.Log(sin) - LogStirling(value);
        else
          return Math.Log(Gamma(value));
      }
    }

    /// <summary>
    /// Sign of the Gamma function
    /// </summary>
    public static int SignGamma(Double value) {
      if (value >= 0)
        return 1;

      value = -value;

      if (value > UInt64.MaxValue)
        return 1;

      UInt64 v = (UInt64)value;

      if ((v % 2) == 0)
        return -1;
      else
        return 1;
    }

    /// <summary>
    /// Factorial (or Gauss function)
    /// </summary>
    public static Double Factorial(this Double value) {
      return Gamma(value + 1.0);
    }

    /// <summary>
    /// Beta (Euler function)
    /// </summary>
    public static Double BetaFunc(Double valueA, Double valueB) {
      //  Gamma(valueA + valueB) / (Gamma(valueA) * Gamma(valueB));
      return Math.Exp(LogGamma(valueA) + LogGamma(valueB) - LogGamma(valueA + valueB)) *
             SignGamma(valueA) * SignGamma(valueB) * SignGamma(valueA + valueB);
    }

    /// <summary>
    /// Incomplete Beta function
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
    public static Double BetaIncomplete(Double x, Double a, Double b) {
      Double p = 1.0;
      Double s = p / (a);

      for (int n = 1; n < 1000; ++n) {
        p = p * (n - b) / n * x;

        Double d = p / (a + n);

        if ((d < 1e-20) && (d > -1e-20))
          break;

        s += d;
      }

      return s * Math.Pow(x, a);
    }

    /// <summary>
    /// Incomplete regularized Beta function
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
    public static Double BetaIncompleteRegular(Double x, Double a, Double b) {
      return BetaIncomplete(x, a, b) / BetaFunc(a, b);
    }

    /// <summary>
    /// Incomplete Gamma function (gamma)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s")]
    public static Double GammaLow(Double s, Double x) {
      if (x == 0)
        return 0.0;

      Double result = 0.0;

      Double g = Gamma(s + 1);
      Double p;

      if (Double.IsInfinity(g)) {
        for (int k = 0; k < 100; ++k) {
          p = Math.Pow(x, k) / Gamma(s + k + 1);

          result += p;

          if (p < 1e-20)
            break;
        }
      }
      else {
        p = 1.0 / g;
        result = p;

        for (int k = 1; k < 100; ++k) {
          p = p * x / (s + k);
          result += p;

          if (p < 1e-20)
            break;
        }
      }

      // return result * Math.Pow(x, s) * RealSpecialFunction.Gamma(s) * Math.Exp(-x);

      return result * Math.Pow(x, s) * Gamma(s) * Math.Exp(-x);
    }

    /// <summary>
    /// Incomplete Gamma function (gamma), regularized
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s")]
    public static Double GammaLowRegular(Double s, Double x) {
      if (x == 0)
        return 0.0;

      Double result = 0.0;

      Double g = Gamma(s + 1);
      Double p;

      if (Double.IsInfinity(g)) {
        for (int k = 0; k < 1000; ++k) {
          p = Math.Pow(x, k) / Gamma(s + k + 1);

          result += p;

          if (p < 1e-20)
            break;
        }
      }
      else {
        p = 1.0 / g;
        result = p;

        for (int k = 1; k < 1000; ++k) {
          p = p * x / (s + k);
          result += p;

          if (p < 1e-20)
            break;
        }
      }

      // result * Math.Pow(x, s) * RealSpecialFunction.Gamma(s) * Math.Exp(-x) / RealSpecialFunction.Gamma(x);
      //s = Math.Log(result) + s * Math.Log(x) + LogGamma(s) - x - LogGamma(x);

      s = Math.Log(result) + s * Math.Log(x) - x;

      return Math.Exp(s);
    }

    /// <summary>
    /// Incomplete Gamma function (Gamma)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s")]
    public static Double GammaHigh(Double s, Double x) {
      return Gamma(s) - GammaLow(s, x);
    }

    /// <summary>
    /// Incomplete Gamma function (Gamma)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s")]
    public static Double GammaHighRegular(Double s, Double x) {
      return (Gamma(s) - GammaLow(s, x)) / Gamma(x);
    }

    /// <summary>
    /// Pochhammer symbol (x)n
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="n">Degree</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pochhammer")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "n")]
    public static Double Pochhammer(Double value, Double n) {
      return Gamma(value + n) / Gamma(n);
    }


    #endregion Public
  }

}
