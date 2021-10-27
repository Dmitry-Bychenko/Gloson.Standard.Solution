using System;

namespace Gloson.Numerics.Calculus {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Operators
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Operators {
    #region Algorithm

    private static double ToFinite(double value) {
      if (double.IsNaN(value))
        throw new ArgumentException("Argument must not be NaN", nameof(value));
      else if (double.IsNegativeInfinity(value))
        return -1e200; // double.MinValue;
      else if (double.IsPositiveInfinity(value))
        return +1e200; // double.MaxValue;
      else
        return value;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Inversed function
    /// </summary>
    /// <param name="function"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static Func<double, double> Inverse(this Func<Double, Double> function,
                                               Double from,
                                               Double to) {
      if (double.IsNaN(from))
        throw new ArgumentException("Argument must not be NaN", nameof(from));
      else if (double.IsNaN(to))
        throw new ArgumentException("Argument must not be NaN", nameof(to));

      return (toFind) => InverseAt(function, toFind, from, to);
    }

    /// <summary>
    /// Inversed function
    /// </summary>
    /// <param name="function"></param>
    /// <returns></returns>
    public static Func<double, double> Inverse(this Func<Double, Double> function) {
      return Inverse(function, Double.MinValue, Double.MaxValue);
    }

    /// <summary>
    /// Inversed function
    /// </summary>
    public static Double InverseAt(this Func<Double, Double> function,
                                        Double toFind,
                                        Double from,
                                        Double to) {
      if (double.IsNaN(toFind))
        throw new ArgumentException("Argument must not be NaN", nameof(toFind));
      else if (double.IsNaN(from))
        throw new ArgumentException("Argument must not be NaN", nameof(from));
      else if (double.IsNaN(to))
        throw new ArgumentException("Argument must not be NaN", nameof(to));

      toFind = ToFinite(toFind);
      from = ToFinite(from);
      to = ToFinite(to);

      if (to < from)
        throw new ArgumentOutOfRangeException(nameof(to), "Empty interval.");

      Double middle = (from + to) / 2.0;

      Double fromValue = function(from);
      Double toValue = function(to);
      Double middleValue;

      if (((fromValue < toFind) && (toValue < toFind)) ||
          ((fromValue > toFind) && (toValue > toFind)))
        throw new ArgumentException("Insufficient or incorrect interval", nameof(to));

      // Main loop
      while (true) {
        middleValue = function(middle);

        if (middleValue > toFind)
          if (fromValue > toFind) {
            fromValue = middleValue;
            from = middle;
          }
          else
            to = middle;
        else if (fromValue > toFind)
          to = middle;
        else {
          fromValue = middleValue;
          from = middle;
        }

        Double newMiddle = (from + to) / 2.0;

        // No progess, quit
        if (newMiddle == middle)
          break;

        middle = newMiddle;
      }

      return middle;
    }

    /// <summary>
    /// Inversed function
    /// </summary>
    /// <param name="function"></param>
    /// <param name="toFind"></param>
    /// <returns></returns>
    public static Double InverseAt(this Func<Double, Double> function, Double toFind) {
      return InverseAt(function, toFind, Double.MinValue, Double.MaxValue);
    }

    /// <summary>
    /// Solve function(x) = 0 on [from…to]
    /// </summary>
    /// <returns>Root</returns>
    public static Double Solve(this Func<Double, Double> function,
                               Double from,
                               Double to) {
      return InverseAt(function, 0.0, from, to);
    }

    /// <summary>
    /// Solve function(x) = 0 on [from…to]
    /// </summary>
    /// <param name="function">Function to solve</param>
    /// <returns>Root</returns>
    public static Double Solve(this Func<Double, Double> function) {
      return InverseAt(function, 0.0, Double.MinValue, Double.MaxValue);
    }

    #endregion Public
  }

}
