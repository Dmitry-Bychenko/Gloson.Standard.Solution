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
    /// For Each
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == action)
        throw new ArgumentNullException(nameof(action));

      foreach (T item in source)
        action(item);
    }

    /// <summary>
    /// For Each
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Func<T, bool> action) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == action)
        throw new ArgumentNullException(nameof(action));

      foreach (T item in source)
        if (!action(item))
          break;
    }

    /// <summary>
    /// For Each
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == action)
        throw new ArgumentNullException(nameof(action));

      int index = 0;

      foreach (T item in source) {
        action(item, index);

        index += 1;
      }
    }

    /// <summary>
    /// For Each
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Func<T, int, bool> action) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == action)
        throw new ArgumentNullException(nameof(action));

      int index = 0;

      foreach (T item in source) {
        if (!action(item, index))
          break;

        index += 1;
      }
    }

    #endregion Public
  }

}
