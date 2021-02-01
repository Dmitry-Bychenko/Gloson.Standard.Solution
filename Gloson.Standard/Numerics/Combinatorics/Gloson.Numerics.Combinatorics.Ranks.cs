using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Gloson.Numerics.Combinatorics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Rank and Unrank combinatoric formulae
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Ranks {
    #region Public

    /// <summary>
    /// Permutation rank
    /// </summary>
    public static BigInteger PermutationRank(IEnumerable<int> permutation) {
      if (permutation is null)
        throw new ArgumentNullException(nameof(permutation));

      int[] value = permutation.ToArray();

      int n = value.Length;

      HashSet<int> usedDigits = new HashSet<int>();

      BigInteger rank = 0;

      for (int i = 0; i < n; ++i) {
        rank *= (n - i);

        int digit = 0;

        for (int j = 0; j < n; ++j) {
          if (usedDigits.Contains(j))
            continue;

          if (j < value[i])
            digit += 1;
          else {
            usedDigits.Add(j);

            break;
          }
        }

        rank += digit;
      }

      return rank;
    }

    /// <summary>
    /// Permutation unrank
    /// </summary>
    /// <param name="size">size of permutations (number of items)</param>
    /// <param name="rank">rank</param>
    /// <returns></returns>
    public static int[] PermutationUnrank(int size, BigInteger rank) {
      if (size < 0)
        throw new ArgumentOutOfRangeException(nameof(size), "size should not be negative.");
      else if (rank < 0)
        throw new ArgumentOutOfRangeException(nameof(rank), "rank should not be negative.");

      int[] digits = new int[size];

      for (int digit = 2; digit <= size; ++digit) {
        BigInteger divisor = digit;

        digits[size - digit] = (int)(rank % divisor);

        if (digit < size)
          rank /= divisor;
      }

      int[] permutation = new int[size];
      List<int> usedDigits = new List<int>(size);

      for (int i = 0; i < size; ++i)
        usedDigits.Add(0);

      for (int i = 0; i < size; ++i) {
        int v = usedDigits.IndexOf(0, 0);

        for (int k = 0; k < digits[i]; ++k)
          v = usedDigits.IndexOf(0, v + 1);

        permutation[i] = v;
        usedDigits[v] = 1;
      }

      return permutation;
    }

    #endregion Public
  }

}
