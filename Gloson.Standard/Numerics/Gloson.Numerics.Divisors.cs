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
    #region Algorithm

    private static BigInteger[] CoreAllDivisors(IEnumerable<BigInteger> divisors) {
      var divs = divisors
        .GroupBy(x => x)
        .Select(group => (key: group.Key, count: group.Count()))
        .ToArray();

      int[] indexes = new int[divs.Length];

      List<BigInteger> result = new List<BigInteger>();

      do {
        BigInteger value = 1;

        for (int i = 0; i < indexes.Length; ++i) {
          for (int k = 0; k < indexes[i]; ++k)
            value *= divs[i].key;
        }

        result.Add(value);

        for (int i = 0; i < indexes.Length; ++i)
          if (indexes[i] < divs[i].count) {
            indexes[i] += 1;

            break;
          }
          else
            indexes[i] = 0;

      }
      while (!indexes.All(idx => idx == 0));

      result.Sort();

      return result.ToArray();
    }

    #endregion Algorithm

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
        throw new ArgumentNullException(nameof(values));
      else if (null == mods)
        throw new ArgumentNullException(nameof(mods));

      BigInteger[] r = values.ToArray();

      if (r.Length <= 0)
        throw new ArgumentOutOfRangeException(nameof(values), "values must not be empty.");

      BigInteger[] a = mods.ToArray();

      if (r.Length != a.Length)
        throw new ArgumentOutOfRangeException(nameof(mods), "mods must be of the same length as values.");

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
    public static IEnumerable<BigInteger> PrimeDivisorsFlat(this BigInteger value) {
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
    /// Prime divisors
    /// </summary>
    public static IEnumerable<BigInteger> PrimeDivisorsFlat(this BigInteger value, IEnumerable<BigInteger> primes) {
      if (value <= 1)
        yield break;
      else if (null == primes) {
        foreach (BigInteger div in PrimeDivisorsFlat(value))
          yield return div;

        yield break;
      }

      BigInteger threshold = value.Sqrt() + 1;

      foreach (BigInteger p in primes) {
        if (p > threshold)
          break;

        bool found = false;

        while (value % p == 0) {
          yield return p;

          value /= p;
          found = true;
        }

        if (found)
          threshold = value.Sqrt() + 1;
      }

      if (value > 1)
        yield return value;
    }

    /// <summary>
    /// Prime Divisors
    /// </summary>
    public static IEnumerable<(BigInteger prime, int power)> PrimeDivisors(this BigInteger value, IEnumerable<BigInteger> primes) {
      BigInteger prior = 0;
      int count = 0;

      foreach (BigInteger p in PrimeDivisorsFlat(value, primes)) {
        if (p == prior)
          count += 1;
        else {
          if (prior > 1)
            yield return (prior, count);

          prior = p;
          count = 1;
        }
      }

      if (count > 1)
        yield return (prior, count);
    }

    /// <summary>
    /// Prime Divisors
    /// </summary>
    public static IEnumerable<(BigInteger prime, int power)> PrimeDivisors(this BigInteger value) =>
      PrimeDivisors(value, null);

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

    /// <summary>
    /// Prime divisors
    /// </summary>
    public static IEnumerable<BigInteger> DistinctPrimeDivisors(this BigInteger value, IEnumerable<BigInteger> primes) {
      if (value <= 1)
        yield break;
      else if (null == primes) {
        foreach (BigInteger div in DistinctPrimeDivisors(value))
          yield return div;

        yield break;
      }

      BigInteger threshold = value.Sqrt() + 1;

      foreach (BigInteger p in primes) {
        if (p > threshold)
          break;

        bool found = false;

        while (value % p == 0) {
          if (!found)
            yield return p;

          value /= p;
          found = true;
        }

        if (found)
          threshold = value.Sqrt() + 1;
      }

      if (value > 1)
        yield return value;
    }

    /// <summary>
    /// All Divisors
    /// </summary>
    public static BigInteger[] AllDivisors(this BigInteger value, IEnumerable<BigInteger> primes) {
      if (value < 1)
        return Array.Empty<BigInteger>();
      else if (value == 1)
        return new BigInteger[] { 1 };

      return CoreAllDivisors(PrimeDivisorsFlat(value, primes));
    }

    /// <summary>
    /// All Divisors
    /// </summary>
    public static BigInteger[] AllDivisors(this BigInteger value) => AllDivisors(value, null);

    /// <summary>
    /// Number of all divisors
    /// </summary>
    public static int NumberOfDivisors(BigInteger value, IEnumerable<BigInteger> primes) {
      if (value < 0)
        return 0;
      else if (value == 1)
        return 1;

      int result = 1;

      foreach (var group in PrimeDivisorsFlat(value, primes).GroupBy(x => x))
        result *= (1 + group.Count());

      return result;
    }

    /// <summary>
    /// Number of all divisors
    /// </summary>
    public static int NumberOfDivisors(BigInteger value) => NumberOfDivisors(value, null);

    /// <summary>
    /// Sum of all divisors
    /// </summary>
    public static BigInteger SumOfDivisors(BigInteger value, IEnumerable<BigInteger> primes) {
      if (value < 0)
        return 0;
      else if (value == 1)
        return 1;

      BigInteger result = 1;

      foreach (var group in PrimeDivisorsFlat(value, primes).GroupBy(x => x))
        result *= (BigInteger.Pow(group.Key, group.Count() + 1) - 1) / (group.Key - 1);

      return result;
    }

    /// <summary>
    /// Sum of all divisors
    /// </summary>
    public static BigInteger SumOfDivisors(BigInteger value) => SumOfDivisors(value, null);


    /// <summary>
    /// Euler totient (fi) function
    /// </summary>
    public static BigInteger Totient(this BigInteger value, IEnumerable<BigInteger> primes) {
      BigInteger result = value;

      foreach (BigInteger div in DistinctPrimeDivisors(value, primes))
        result = (result / div) * (div - 1);

      return result;
    }

    /// <summary>
    /// Euler totient (fi) function
    /// </summary>
    public static BigInteger Totient(this BigInteger value) =>
      Totient(value, null);

    #endregion Public
  }
}
