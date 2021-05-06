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
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      if (comparer is null)
        comparer = Comparer<T>.Default;

      if (comparer is null)
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

    /// <summary>
    /// Longest Common Subsequence
    /// </summary>
    public static T[] LongestCommonSubsequence<T>(this IEnumerable<T> source,
                                                       IEnumerable<T> target,
                                                       IEqualityComparer<T> comparer = null) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (target is null)
        throw new ArgumentNullException(nameof(target));

      comparer = (comparer ?? EqualityComparer<T>.Default)
                           ?? throw new ArgumentNullException(nameof(comparer),
                                $"Type {typeof(T).Name} doesn't have default equality comparer.");

      T[] left = source.ToArray();
      T[] right = target.ToArray();

      int[][] data = Enumerable
        .Range(0, left.Length + 1)
        .Select(_ => new int[right.Length + 1])
        .ToArray();

      for (int i = 1; i <= left.Length; ++i)
        for (int j = 1; j <= right.Length; ++j)
          if (comparer.Equals(left[i - 1], right[j - 1]))
            data[i][j] = data[i - 1][j - 1] + 1;
          else
            data[i][j] = Math.Max(data[i - 1][j], data[i][j - 1]);

      List<T> result = new();

      for (int y = left.Length, x = right.Length, v = data[y][x]; x > 0 && y > 0 && v > 0;) {
        int dd = data[y - 1][x - 1];
        int dy = data[y - 1][x];
        int dx = data[y][x - 1];

        if (dd >= dx)
          if (dd >= dy) {
            x -= 1;
            y -= 1;
          }
          else
            y -= 1;
        else if (dy >= dx)
          if (dy >= dd)
            y -= 1;
          else {
            x -= 1;
            y -= 1;
          }
        else
          x -= 1;

        int newV = data[y][x];

        if (newV < v) {
          result.Add(left[y]);

          v = newV;
        }
      }

      T[] dataToReturn = result.ToArray();

      Array.Reverse(dataToReturn);

      return dataToReturn;
    }

    #endregion Public
  }
}
