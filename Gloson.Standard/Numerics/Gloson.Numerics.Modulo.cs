using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Modulo operations
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Modulo {
    #region Public

    /// <summary>
    /// Mod
    /// </summary>
    public static BigInteger Mod(this BigInteger value, BigInteger mod) {
      if (mod <= 0)
        throw new ArgumentOutOfRangeException("mod", $"mod == {mod} must be positive.");

      if (value >= 0 && value < mod)
        return value;

      value = value % mod;

      if (value >= 0)
        return value;
      else
        return value + mod;
    }

    /// <summary>
    /// -value (mod mod)
    /// </summary>
    public static BigInteger ModNegate(this BigInteger value, BigInteger mod) {
      return Mod(-value, mod);
    }

    /// <summary>
    /// Greatest Common Divisor
    /// </summary>
    public static BigInteger Gcd(this BigInteger left, BigInteger right) =>
      BigInteger.GreatestCommonDivisor(left, right);

    /// <summary>
    /// Least Common Multiply
    /// </summary>
    public static BigInteger Lcm(this BigInteger left, BigInteger right) =>
      left / BigInteger.GreatestCommonDivisor(left, right) * right;

    /// <summary>
    /// Extended Euclid Algorithm
    /// </summary>
    public static (BigInteger LeftFactor,
                   BigInteger RightFactor,
                   BigInteger Gcd) Egcd(this BigInteger left, BigInteger right) {
      BigInteger leftFactor = 0;
      BigInteger rightFactor = 1;

      BigInteger u = 1;
      BigInteger v = 0;
      BigInteger gcd = 0;

      while (left != 0) {
        BigInteger q = right / left;
        BigInteger r = right % left;

        BigInteger m = leftFactor - u * q;
        BigInteger n = rightFactor - v * q;

        right = left;
        left = r;
        leftFactor = u;
        rightFactor = v;
        u = m;
        v = n;

        gcd = right;
      }

      return (LeftFactor: leftFactor,
              RightFactor: rightFactor,
              Gcd: gcd);
    }

    /// <summary>
    /// Mod Inversion
    /// </summary>
    public static BigInteger ModInversion(this BigInteger value, BigInteger modulo) {
      var egcd = Egcd(value, modulo);

      if (egcd.Gcd != 1)
        throw new ArgumentException("Invalid modulo", "modulo");

      BigInteger result = egcd.LeftFactor;

      if (result < 0)
        result += modulo;

      return result % modulo;
    }

    /// <summary>
    /// Mod Division
    /// </summary>
    public static BigInteger ModDivision(this BigInteger left, BigInteger right, BigInteger modulo) {
      return (left * ModInversion(right, modulo)) % modulo;
    }

    #endregion Public
  }
}
