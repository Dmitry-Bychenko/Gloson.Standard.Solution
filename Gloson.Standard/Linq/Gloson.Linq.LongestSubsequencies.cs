using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Longest Subsequencies
  /// </summary>
  // https://en.wikipedia.org/wiki/Longest_increasing_subsequence
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Longest Increasing Subsequence
    /// </summary>
    public static T[] LongestIncreasingSubsequence<T>(this IEnumerable<T> source, bool duplicatesAllowed, IComparer<T> comparer) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == comparer)
        comparer = Comparer<T>.Default;

      if (null == comparer)
        throw new ArgumentNullException(nameof(comparer), $"Type {typeof(T).Name} doesn't have default comparer.");

      T[] X = source.ToArray();

      int[] P = new int[X.Length];
      int[] M = new int[X.Length + 1];

      int L = 0;

      int criterium = duplicatesAllowed ? 1 : 0;

      for (int i = 0; i < X.Length; ++i) {
        int lo = 1;
        int hi = L;

        while (lo <= hi) {
          int mid = (lo + hi + 1) / 2;

          if (comparer.Compare(X[M[mid]], X[i]) < criterium)
            lo = mid + 1;
          else
            hi = mid - 1;
        }

        int newL = lo;

        P[i] = M[newL - 1];
        M[newL] = i;

        if (newL > L)
          L = newL;
      }

      T[] S = new T[L];

      int k = M[L];

      for (int i = L - 1; i >= 0; --i) {
        S[i] = X[k];
        k = P[k];
      }

      return S;
    }

    /// <summary>
    /// Longest Increasing Subsequence
    /// </summary>
    public static T[] LongestIncreasingSubsequence<T>(this IEnumerable<T> source, bool duplicatesAllowed) =>
      LongestIncreasingSubsequence<T>(source, duplicatesAllowed, null);

    /// <summary>
    /// Longest Increasing Subsequence
    /// </summary>
    public static T[] LongestIncreasingSubsequence<T>(this IEnumerable<T> source) =>
      LongestIncreasingSubsequence<T>(source, false, null);

    /// <summary>
    /// Longest Increasing Subsequence
    /// </summary>
    public static T[] LongestIncreasingSubsequence<T>(this IEnumerable<T> source, IComparer<T> comparer) =>
      LongestIncreasingSubsequence<T>(source, false, comparer);

    /// <summary>
    /// Longest Non Decreasing Subsequence
    /// </summary>
    public static T[] LongestNonDecreasingSubsequence<T>(this IEnumerable<T> source) =>
      LongestIncreasingSubsequence<T>(source, true, null);

    /// <summary>
    /// Longest Non Decreasing Subsequence
    /// </summary>
    public static T[] LongestNonDecreasingSubsequence<T>(this IEnumerable<T> source, IComparer<T> comparer) =>
      LongestIncreasingSubsequence<T>(source, true, comparer);

    #endregion Public
  }
}
