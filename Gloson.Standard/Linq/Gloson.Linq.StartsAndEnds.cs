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
    /// Starts With
    /// </summary>
    public static bool StartsWith<T>(IEnumerable<T> source, Func<T, bool> predicate) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == predicate)
        throw new ArgumentNullException(nameof(predicate));

      foreach (var item in source) 
        return predicate(item);

      return false;
    }

    /// <summary>
    /// Ends With
    /// </summary>
    public static bool EndsWith<T>(IEnumerable<T> source, Func<T, bool> predicate) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == predicate)
        throw new ArgumentNullException(nameof(predicate));

      int count = -1;

      if (source is IList<T> list) 
        return (count = list.Count) <= 0 ? false : predicate(list[count - 1]);
      else if (source is T[] arr)
        return (count = arr.Length) <= 0 ? false : predicate(arr[count - 1]);
      else if (source is IReadOnlyList<T> rl)
        return (count = rl.Count) <= 0 ? false : predicate(rl[count - 1]);

      T last = default;

      foreach (var item in source) {
        count = 1;
        last = item;
      }

      return count <= 0 ? false : predicate(last);
    }

    #endregion Public
  }

}
