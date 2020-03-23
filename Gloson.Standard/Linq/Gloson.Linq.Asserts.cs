using System;
using System.Collections.Generic;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Assertions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Assert
    /// </summary>
    public static IEnumerable<T> Assert<T>(this IEnumerable<T> source,
                                           Func<T, bool> predicate,
                                           Func<T, Exception> exception) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == predicate)
        throw new ArgumentNullException(nameof(predicate));

      foreach (T item in source) {
        if (!predicate(item))
          throw exception?.Invoke(item) ?? new InvalidOperationException("Assertion failed. Sequence has an invalid item.");

        yield return item;
      }
    }

    public static IEnumerable<T> Assert<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
      Assert(source, predicate, null);

    #endregion Public
  }

}
