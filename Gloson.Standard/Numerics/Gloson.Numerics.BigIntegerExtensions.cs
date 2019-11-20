﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

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

    private static Boolean isSqrt(BigInteger n, BigInteger root) {
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

        while (!isSqrt(value, root)) {
          root += value / root;
          root /= 2;
        }

        return root;
      }

      throw new ArithmeticException("NaN");
    }

    #endregion Public
  }

}
