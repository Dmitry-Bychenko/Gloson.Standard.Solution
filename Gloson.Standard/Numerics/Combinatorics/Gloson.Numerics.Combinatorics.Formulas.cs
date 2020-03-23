using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Gloson.Numerics.Combinatorics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Combinatoric formulae
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Formulas {
    #region Algorithm

    private static BigInteger Factorial(int value) {
      BigInteger result = 1;

      for (int i = 2; i <= value; ++i)
        result *= i;

      return result;
    }

    private static BigInteger C(int take, int from) {
      if (take < 0 || from < 0 || take > from)
        return 0;
      else if (take == from)
        return 1;

      BigInteger result = 1;

      for (int i = take; i > take - from; --i)
        result *= i;

      return result / Factorial(from);
    }

    private static BigInteger A(int take, int from) {
      if (take < 0 || from < 0 || take > from)
        return 0;

      BigInteger result = 1;

      for (int i = take; i > take - from; --i)
        result *= i;

      return result;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Distributions Count, i.e.
    /// </summary>
    /// <param name="take">Take take items</param>
    /// <param name="from">From from total items</param>
    /// <param name="withRepetitions">With or without repetitions</param>
    /// <param name="orderMatters">If order matters or not (i.e. if [a, b] =/!= [b, a])</param>
    /// <returns></returns>
    public static BigInteger DistributionCount(
      int take,
      int from,
      bool withRepetitions,
      bool orderMatters) {

      if (take < 0)
        throw new ArgumentOutOfRangeException(nameof(take));
      else if (from < 0)
        throw new ArgumentOutOfRangeException(nameof(from));

      if (withRepetitions) {
        if (orderMatters)
          return from == 0 && take == 0
            ? 1
            : BigInteger.Pow(from, take);

        // C(take, take + from - 1)
        return C(take, take + from - 1);
      }
      else {
        if (take > from)
          return 0;

        if (orderMatters)
          return A(take, from);
        else
          return C(take, from);
      }
    }

    /// <summary>
    ///  Distributions Count, i.e.
    /// </summary>
    /// <param name="source">items to count</param>
    /// <param name="take">Take take items</param>
    /// <param name="withRepetitions">With or without repetitions</param>
    /// <param name="orderMatters">If order matters or not (i.e. if [a, b] =/!= [b, a])</param>
    /// <param name="comparer">Comparer if required</param>
    /// <returns></returns>
    public static BigInteger DistributionCount<T>(
      IEnumerable<T> source,
      int take,
      bool withRepetitions,
      bool orderMatters,
      IEqualityComparer<T> comparer) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (take < 0)
        throw new ArgumentOutOfRangeException(nameof(take));

      if (null == comparer)
        comparer = EqualityComparer<T>.Default;

      if (null == comparer)
        throw new ArgumentNullException(nameof(comparer),
          $"{nameof(comparer)} is not provided and there are no default equality comparer for {typeof(T).Name} type");

      if (withRepetitions) {
        int from = source.Distinct(comparer).Count();

        if (orderMatters)
          return from == 0 && take == 0
            ? 1
            : BigInteger.Pow(from, take);

        return C(take, take + from - 1);
      }
      else {
        var data = source
          .GroupBy(item => item, comparer);

        int from = 0;
        BigInteger denominator = 1;

        foreach (var group in data) {
          from += group.Count();

          denominator *= Factorial(group.Count());
        }

        if (take > from)
          return 0;

        if (orderMatters)
          return A(take, from) / denominator;
        else
          return C(take, from) / denominator;
      }
    }

    /// <summary>
    ///  Distributions Count, i.e.
    /// </summary>
    /// <param name="source">items to count</param>
    /// <param name="take">Take take items</param>
    /// <param name="withRepetitions">With or without repetitions</param>
    /// <param name="orderMatters">If order matters or not (i.e. if [a, b] =/!= [b, a])</param>
    /// <returns></returns>
    public static BigInteger DistributionCount<T>(
     IEnumerable<T> source,
     int take,
     bool withRepetitions,
     bool orderMatters) {
      return DistributionCount(source, take, withRepetitions, orderMatters, null);
    }

    #endregion Public
  }
}
