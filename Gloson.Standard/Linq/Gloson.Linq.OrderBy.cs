using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Order
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [Flags]
  public enum Orders {
    None = 0,
    Ascending = 1,
    Descending = 2,
    AscendingAndDescending = Orders.Ascending | Orders.Descending
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Orders Helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class OrdersHelpers {
    /// <summary>
    /// From Integer
    /// </summary>
    public static Orders FromInt(int value) {
      return
          value == 0 ? Orders.AscendingAndDescending
        : value < 0 ? Orders.Descending
        : Orders.Ascending;

    }

    /// <summary>
    /// To Integer
    /// </summary>
    public static int ToInt(Orders value) {
      return
          value == Orders.AscendingAndDescending ? 0
        : value == Orders.Descending ? -1
        : 1;
    }
  }

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
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == comparer)
        throw new ArgumentNullException(nameof(comparer));

      return source
        .OrderBy(item => item, Comparer<T>.Create((left, right) => comparer(left, right)));
    }

    /// <summary>
    /// Order By Descending
    /// </summary>
    public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, Func<T, T, int> comparer) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == comparer)
        throw new ArgumentNullException(nameof(comparer));

      return source
        .OrderByDescending(item => item, Comparer<T>.Create((left, right) => comparer(left, right)));
    }

    /// <summary>
    /// Then By
    /// </summary>
    public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, Func<T, T, int> comparer) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == comparer)
        throw new ArgumentNullException(nameof(comparer));

      return source
        .ThenBy(item => item, Comparer<T>.Create((left, right) => comparer(left, right)));
    }

    /// <summary>
    /// Then By Descending
    /// </summary>
    public static IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> source, Func<T, T, int> comparer) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == comparer)
        throw new ArgumentNullException(nameof(comparer));

      return source
        .ThenByDescending(item => item, Comparer<T>.Create((left, right) => comparer(left, right)));
    }

    /// <summary>
    /// Detect order of the sequence
    /// </summary>
    public static Orders Order<T>(this IEnumerable<T> source, IComparer<T> comparer = null) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == comparer)
        comparer = Comparer<T>.Default;

      if (null == comparer)
        throw new ArgumentNullException(nameof(comparer),
          $"Type {typeof(T).Name} doesn't have default comparer");

      Orders result = Orders.AscendingAndDescending;
      T prior = default;
      bool first = true;

      foreach (T next in source) {
        if (first) {
          prior = next;
          first = false;
        }
        else {
          int order = comparer.Compare(next, prior);

          prior = next;

          if (result == Orders.AscendingAndDescending) {
            if (order < 0)
              result = Orders.Descending;
            else if (order > 0)
              result = Orders.Ascending;
          }
          else if (result == Orders.Ascending && order < 0)
            return Orders.None;
          else if (result == Orders.Descending && order > 0)
            return Orders.None;
        }
      }

      return result;
    }

    #endregion Public
  }
}
