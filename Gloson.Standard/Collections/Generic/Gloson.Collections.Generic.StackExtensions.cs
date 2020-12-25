using System;
using System.Collections.Generic;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Queue Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class StackExtensions {
    #region Public

    /// <summary>
    /// Consuming enumeration
    /// </summary>
    public static IEnumerable<T> Consume<T>(this Stack<T> value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      while (value.Count > 0)
        yield return value.Pop();
    }

    /// <summary>
    /// Add Range
    /// </summary>
    public static Stack<T> PushRange<T>(this Stack<T> value, IEnumerable<T> items) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      else if (items is null)
        throw new ArgumentNullException(nameof(items));

      foreach (var item in items)
        value.Push(item);

      return value;
    }

    #endregion Public
  }

}
