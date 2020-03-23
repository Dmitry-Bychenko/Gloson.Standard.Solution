using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Divisors
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Divisors {
    #region Public

    /// <summary>
    /// Chinese Reminder Theorem solution
    ///    x = value[0] (mod mods[0])
    ///    x = value[1] (mod mods[1])
    ///    ...
    ///    x = value[n] (mod mods[n])
    /// </summary>
    /// <param name="values">Values</param>
    /// <param name="mods">Corresponding mods</param>
    /// <returns></returns>
    /// <example>BigInteger x = Modular.Crt(new BigInteger[] { 1, 2, 6}, new BigInteger[] { 2, 3, 7});</example>
    public static BigInteger Crt(IEnumerable<BigInteger> values, IEnumerable<BigInteger> mods) {
      if (null == values)
        throw new ArgumentNullException("values");
      else if (null == mods)
        throw new ArgumentNullException("mods");

      BigInteger[] r = values.ToArray();

      if (r.Length <= 0)
        throw new ArgumentOutOfRangeException("values", "values must not be empty.");

      BigInteger[] a = mods.ToArray();

      if (r.Length != a.Length)
        throw new ArgumentOutOfRangeException("mods", "mods must be of the same length as values.");

      BigInteger M = a.Aggregate(BigInteger.One, (ss, item) => ss * item);

      BigInteger result = 0;

      for (int i = 0; i < a.Length; ++i) {
        BigInteger m = M / a[i];
        BigInteger m_1 = Modulo.ModInversion(m, a[i]);

        result += r[i] * m * m_1;
      }

      return Modulo.Mod(result, M);
    }

    /// <summary>
    /// Prime divisors
    /// </summary>
    public static IEnumerable<BigInteger> PrimeDivisors(this BigInteger value) {
      if (value <= 1)
        yield break;

      while (value % 2 == 0) {
        yield return 2;

        value /= 2;
      }

      while (value % 3 == 0) {
        yield return 3;

        value /= 3;
      }

      while (value % 5 == 0) {
        yield return 5;

        value /= 5;
      }

      BigInteger n = value.Sqrt() + 1;

      for (BigInteger i = 1; 6 * i < n; ++i) {
        BigInteger x = 6 * i + 1;

        while (value % x == 0) {
          yield return x;

          value /= x;
          n = value.Sqrt() + 1;
        }

        x = 6 * i + 5;

        while (value % x == 0) {
          yield return x;

          value /= x;
          n = value.Sqrt() + 1;
        }
      }

      if (value > 1)
        yield return value;
    }

    /// <summary>
    /// Distinct Prime divisors
    /// </summary>
    public static IEnumerable<BigInteger> DistinctPrimeDivisors(this BigInteger value) {
      if (value <= 1)
        yield break;

      BigInteger last = 0;

      while (value % 2 == 0) {
        if (last != 2)
          yield return 2;

        last = 2;
        value /= 2;
      }

      while (value % 3 == 0) {
        if (last != 3)
          yield return 3;

        last = 3;
        value /= 3;
      }

      while (value % 5 == 0) {
        if (last != 5)
          yield return 5;

        last = 5;
        value /= 5;
      }

      BigInteger n = value.Sqrt() + 1;

      for (BigInteger i = 1; 6 * i < n; ++i) {
        BigInteger x = 6 * i + 1;

        while (value % x == 0) {
          if (last != x)
            yield return x;

          last = x;
          value /= x;
          n = value.Sqrt() + 1;
        }

        x = 6 * i + 5;

        while (value % x == 0) {
          if (last != x)
            yield return x;

          last = x;
          value /= x;
          n = value.Sqrt() + 1;
        }
      }

      if (value > 1 && last != value)
        yield return value;
    }

    #endregion Public
  }
}
