using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  /// <summary>
  /// Indexes Of (Knuth-Morris-Pratt algorithm)
  /// </summary>
  public static partial class EnumerableExtensions {
    #region Algorithm

    private static int[] KmpArray<T>(IEnumerable<T> sequence, IEqualityComparer<T> comparer = default) {
      if (sequence is null)
        throw new ArgumentNullException(nameof(sequence));

      comparer ??= EqualityComparer<T>.Default;

      IReadOnlyList<T> pattern = sequence as IReadOnlyList<T> ?? sequence.ToList();

      int[] result = new int[pattern.Count];

      int length = 0;
      int current = 1;

      while (current < pattern.Count)
        if (comparer.Equals(pattern[current], pattern[length]))
          result[current++] = ++length;
        else if (length != 0)
          length = result[length - 1];
        else
          result[current++] = length;

      return result;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Indexes of pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Sequence to find pattern from</param>
    /// <param name="pattern">Pattern to look for</param>
    /// <param name="comparer">Comparer to use</param>
    /// <returns>Indexes within source where pattern is found</returns>
    /// <exception cref="ArgumentNullException">When source or pattern is null</exception>
    /// <example>
    /// <code>
    /// int indexOf = source.IndexOf(new string[] {"a", "b", "c"}).DefaultIfEmpty(-1);
    /// </code>
    /// </example>
    public static IEnumerable<int> IndexesOf<T>(this IEnumerable<T> source,
                                                     IEnumerable<T> pattern,
                                                     IEqualityComparer<T> comparer = default) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (pattern is null)
        throw new ArgumentNullException(nameof(pattern));

      IReadOnlyList<T> pat = pattern as IReadOnlyList<T> ?? pattern.ToList();

      if (pat.Count == 0)
        yield break;

      int[] lps = KmpArray(pat);

      comparer ??= EqualityComparer<T>.Default;

      int i = 0;
      int j = 0;

      using var en = source.GetEnumerator();

      for (bool agenda = en.MoveNext(); agenda;) {
        if (comparer.Equals(pat[j], en.Current)) {
          i += 1;
          j += 1;

          agenda = en.MoveNext();
        }

        if (j == pat.Count) {
          yield return i - j;

          j = lps[j - 1];
        }
        else if (agenda && !comparer.Equals(pat[j], en.Current)) {
          if (j != 0)
            j = lps[j - 1];
          else {
            i += 1;

            agenda = en.MoveNext();
          }
        }
      }
    }

    #endregion Public
  }

}
