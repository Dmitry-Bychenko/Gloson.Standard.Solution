using System;
using System.Collections.Generic;
using System.Text;
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
    /// <param name="WithRepetitions">With or without repetitions</param>
    /// <param name="OrderMatters">If order matters or not (i.e. if [a, b] =/!= [b, a])</param>
    /// <returns></returns>
    public static BigInteger DistributionCount(
      int take, 
      int from, 
      bool WithRepetitions,
      bool OrderMatters) {

      if (take < 0)
        throw new ArgumentOutOfRangeException(nameof(take));
      else if (from < 0)
        throw new ArgumentOutOfRangeException(nameof(from));

      if (WithRepetitions) {
        if (OrderMatters) 
          return from == 0 && take == 0 
            ? 1 
            : BigInteger.Pow(from, take);

        // C(take, take + from - 1)
        return C(take, take + from - 1);
      }
      else {
        if (take > from)
          return 0;

        if (OrderMatters)
          return A(take, from);
        else
          return C(take, from);
      }
    }

    #endregion Public
  }
}
