using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Interleave
    /// </summary>
    /// <param name="source">Main Source</param>
    /// <param name="others">Other sources</param>
    /// <returns>Interleaved m, o1, o2, ..., oN, m, o1, ..., oN ... </returns>
    public static IEnumerable<T> Interleave<T>(this IEnumerable<T> source, params IEnumerable<T>[] others) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == others)
        throw new ArgumentNullException(nameof(others));

      IEnumerator<T>[] enums = new IEnumerator<T>[] { source.GetEnumerator() }
          .Concat(others
          .Where(item => item != null)
          .Select(item => item.GetEnumerator()))
        .ToArray();

      try {
        bool hasValue = true;

        while (hasValue) {
          hasValue = false;

          for (int i = 0; i < enums.Length; ++i) {
            if (enums[i] != null && enums[i].MoveNext()) {
              hasValue = true;

              yield return enums[i].Current;
            }
            else {
              enums[i].Dispose();
              enums[i] = null;
            }
          }
        }
      }
      finally {
        for (int i = enums.Length - 1; i >= 0; --i)
          if (enums[i] != null)
            enums[i].Dispose();
      }
    }

    /// <summary>
    /// Interleave as array (complete arrays only)
    /// </summary>
    /// <param name="source">Main Source</param>
    /// <param name="others">Other sources</param>
    /// <returns>Interleaved [m, o1, o2, ..., oN], [m, o1, ..., oN] ... </returns>
    public static IEnumerable<T[]> InterleaveAsArray<T>(this IEnumerable<T> source, params IEnumerable<T>[] others) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == others)
        throw new ArgumentNullException(nameof(others));

      IEnumerator<T>[] enums = new IEnumerator<T>[] { source.GetEnumerator() }
          .Concat(others
          .Where(item => item != null)
          .Select(item => item.GetEnumerator()))
        .ToArray();

      try {
        while (true) {
          T[] record = new T[enums.Length];

          for (int i = 0; i < enums.Length; ++i) {
            if (enums[i] != null && !enums[i].MoveNext()) 
              yield break;

            record[i] = enums[i].Current;
          }

          yield return record;
        }
      }
      finally {
        for (int i = enums.Length - 1; i >= 0; --i)
          if (enums[i] != null)
            enums[i].Dispose();
      }
    }

    #endregion Public
  }
}
