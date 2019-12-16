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
    /// IF Empty. Either source or sequence if source is empty
    /// </summary>
    public static IEnumerable<T> IfEmpty<T>(this IEnumerable<T> source, IEnumerable<T> sequence) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      bool isEmpty = true;

      foreach (T item in source) {
        isEmpty = false;

        yield return item;
      }

      if (isEmpty)
        foreach (T item in sequence)
          yield return item;
    }

    /// <summary>
    /// IF Empty. Either source or sequence if source is null or empty
    /// </summary>
    public static IEnumerable<T> IfNullOrEmpty<T>(this IEnumerable<T> source, IEnumerable<T> sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      if (null == source) {
        foreach (T item in sequence)
          yield return item;

        yield break;
      }

      bool isEmpty = true;

      foreach (T item in source) {
        isEmpty = false;

        yield return item;
      }

      if (isEmpty)
        foreach (T item in sequence)
          yield return item;
    }

    #endregion Public
  }
}
