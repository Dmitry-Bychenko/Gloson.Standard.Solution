using System;
using System.Numerics;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// BigInteger extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class BigIntegerExtensions {
    #region Algorithm

    private static Boolean CoreIsSqrt(BigInteger n, BigInteger root) {
      BigInteger lowerBound = root * root;
      BigInteger upperBound = (root + 1) * (root + 1);

      return (n >= lowerBound && n < upperBound);
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Sum of digits
    /// </summary>
    /// <param name="value">value</param>
    /// <returns>sum of digits</returns>
    public static int SumOfDigits(this BigInteger value) {
      int result = 0;

      if (value < 0)
        value = -value;

      while (!value.IsZero) {
        result += (int)(value % 10);

        value /= 10;
      }

      return result;
    }

    /// <summary>
    /// Sqrt
    /// </summary>
    public static BigInteger Sqrt(this BigInteger value) {
      if (value == 0)
        return 0;

      if (value > 0) {
        int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(value, 2)));

        BigInteger root = BigInteger.One << (bitLength / 2);

        while (!CoreIsSqrt(value, root)) {
          root += value / root;
          root /= 2;
        }

        return root;
      }

      throw new ArithmeticException("NaN");
    }

    /// <summary>
    /// Dijkstra Fusc function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Calkin%E2%80%93Wilf_tree"/>
    public static BigInteger Fusc(this BigInteger value) {
      BigInteger result = 0;

      for (BigInteger n = value, a = 1; n != 0; n /= 2)
        if (n % 2 == 0)
          a += result;
        else
          result += a;

      return result;
    }

    #endregion Public
  }

}
