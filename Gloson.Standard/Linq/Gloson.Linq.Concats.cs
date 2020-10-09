using System;
using System.Collections.Generic;

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
    /// Concat ordered (ascending) sequences
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<T> ConcatOrdered<T>(this IEnumerable<T> source, IEnumerable<T> other, IComparer<T> comparer = null) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == other)
        throw new ArgumentNullException(nameof(other));

      if (null == comparer) {
        comparer = Comparer<T>.Default;

        if (null == comparer)
          throw new ArgumentNullException(nameof(comparer), $"No default {nameof(comparer)} for {typeof(T).Name} found");
      }

      using var enLeft = source.GetEnumerator();
      using var enRight = other.GetEnumerator();

      if (!enLeft.MoveNext()) {
        while (enRight.MoveNext())
          yield return enRight.Current;

        yield break;
      }
      else if (!enRight.MoveNext()) {
        do {
          yield return enLeft.Current;
        }
        while (enLeft.MoveNext());

        yield break;
      }

      while (true) {
        if (comparer.Compare(enLeft.Current, enRight.Current) <= 0) {
          yield return enLeft.Current;

          if (!enLeft.MoveNext()) {
            do {
              yield return enRight.Current;
            }
            while (enRight.MoveNext());

            yield break;
          }
        }
        else {
          yield return enRight.Current;

          if (!enRight.MoveNext()) {
            do {
              yield return enLeft.Current;
            }
            while (enLeft.MoveNext());

            yield break;
          }
        }
      }
    }

    #endregion Public
  }
}
