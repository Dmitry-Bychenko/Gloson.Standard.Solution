using System;
using System.Collections.Generic;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Linked List Extensions 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class LinkedListExtensions {
    #region Public

    /// <summary>
    /// Add range after
    /// </summary>
    public static void AddRangeAfter<T>(this LinkedList<T> list, IEnumerable<T> value) {
      if (null == list)
        throw new ArgumentNullException(nameof(list));
      else if (null == value)
        throw new ArgumentNullException(nameof(value));

      foreach (T item in value)
        list.AddLast(item);
    }

    /// <summary>
    /// Consume (as queue)
    /// </summary>
    public static IEnumerable<T> Consume<T>(this LinkedList<T> list) {
      if (null == list)
        throw new ArgumentNullException(nameof(list));

      while (list.Count > 0) {
        yield return list.First.Value;

        list.RemoveFirst();
      }
    }

    #endregion Public
  }

}
