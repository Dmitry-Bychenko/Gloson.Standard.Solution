﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Primes
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Primes {
    #region Private Data

    // Random generator
    private static readonly ThreadLocal<Random> s_Gen = new(() => new Random());

    // Random generator
    private static Random Gen {
      get {
        return s_Gen.Value;
      }
    }

    #endregion Private Data

    #region Public

    /// <summary>
    /// Primes
    /// </summary>
    public static IEnumerable<BigInteger> Generate() {
      List<BigInteger> primes = new();

      yield return 2;
      yield return 3;
      yield return 5;
      yield return 7;

      for (BigInteger p = 11; ; p += 2) {
        if (p % 3 == 0 || p % 5 == 0 || p % 7 == 0)
          continue;

        bool isPrime = true;

        BigInteger n = (BigInteger)(Math.Sqrt((double)p + 1) + 1);

        for (int i = 0; i < primes.Count; ++i) {
          BigInteger div = primes[i];

          if (div > n)
            break;
          else if (p % div == 0) {
            isPrime = false;

            break;
          }
        }

        if (!isPrime)
          continue;

        primes.Add(p);

        yield return p;
      }
    }

    /// <summary>
    /// Rabin-Milles prime test
    /// </summary>
    /// <param name="value">Value to test</param>
    /// <param name="certainty">Certainty (number of tests)</param>
    /// <returns>False if not prime, true if probably prime</returns>
    public static Boolean IsProbablePrime(this BigInteger value, int certainty) {
      if (value <= 1)
        return false;

      if (certainty <= 0)
        throw new ArgumentOutOfRangeException(nameof(certainty), "certainty should be a positive number.");

      if ((value % 2) == 0)
        return (value == 2);
      else if ((value % 3) == 0)
        return (value == 3);
      else if ((value % 5) == 0)
        return (value == 5);
      else if ((value % 7) == 0)
        return (value == 7);
      else if ((value % 11) == 0)
        return (value == 11);

      if (value < 1000000) {
        int v = (int)value;
        int n = (int)(Math.Sqrt(v) + 0.5);

        for (int i = 1; 6 * i <= n; ++i) {
          if (v % (6 * i + 1) == 0)
            return false;
          else if (v % (6 * i + 5) == 0)
            return false;
        }

        return true;
      }

      BigInteger d = value - 1;
      int s = 0;

      while (d % 2 == 0) {
        d /= 2;
        s += 1;
      }

      Byte[] bytes = new Byte[value.ToByteArray().Length];
      BigInteger a;

      for (int i = 0; i < certainty; i++) {
        do {
          Gen.NextBytes(bytes);

          a = new BigInteger(bytes);
        }
        while (a < 2 || a >= value - 2);

        BigInteger x = BigInteger.ModPow(a, d, value);

        if (x == 1 || x == value - 1)
          continue;

        for (int r = 1; r < s; r++) {
          x = BigInteger.ModPow(x, 2, value);

          if (x == 1)
            return false;
          else if (x == value - 1)
            break;
        }

        if (x != value - 1)
          return false;
      }

      return true;
    }

    /// <summary>
    /// Rabin-Milles prime test (with certainty = 10)
    /// </summary>
    /// <param name="value">Value to test</param>
    /// <returns>False if not prime, true if probably prime</returns>
    public static Boolean IsProbablePrime(this BigInteger value) {
      return IsProbablePrime(value, 10);
    }

    /// <summary>
    /// Moebius function
    /// https://en.wikipedia.org/wiki/Moebius_function
    /// </summary>
    public static int Moebius(this BigInteger value) {
      if (value <= 0)
        throw new ArgumentOutOfRangeException(nameof(value),
          "value must be a positive number.");

      if (value == 1)
        return 1;

      int count = 0;

      if (value % 2 == 0) {
        if (value == 2)
          return -1;

        count += 1;
        value /= 2;

        if (value % 2 == 0)
          return 0;
      }

      count += 1;

      for (long d = 3; d * d <= value; d += 2) {
        if (value % d == 0) {
          count += 1;
          value /= d;

          if (value % d == 0)
            return 0;
        }
      }

      return count % 2 == 0 ? 1 : -1;
    }

    #endregion Public
  }

}
