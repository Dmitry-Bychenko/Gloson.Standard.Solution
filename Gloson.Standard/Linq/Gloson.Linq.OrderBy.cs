using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable Extensions - Order By
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Order By
    /// </summary>
    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, Func<T, T, int> comparer) {
      if (Object.ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (Object.ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer));

      return source
        .OrderBy(item => item, Comparer<T>.Create((left, right) => comparer(left, right)));
    }

    /// <summary>
    /// Order By Descending
    /// </summary>
    public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, Func<T, T, int> comparer) {
      if (Object.ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (Object.ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer));

      return source
        .OrderByDescending(item => item, Comparer<T>.Create((left, right) => comparer(left, right)));
    }

    /// <summary>
    /// Then By
    /// </summary>
    public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, Func<T, T, int> comparer) {
      if (Object.ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (Object.ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer));

      return source
        .ThenBy(item => item, Comparer<T>.Create((left, right) => comparer(left, right)));
    }

    /// <summary>
    /// Then By Descending
    /// </summary>
    public static IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> source, Func<T, T, int> comparer) {
      if (Object.ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (Object.ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer));

      return source
        .ThenByDescending(item => item, Comparer<T>.Create((left, right) => comparer(left, right)));
    }

    #endregion Public
  }
}
