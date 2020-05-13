using System;
using System.Collections.Generic;

namespace Gloson.Linq {

  public static class ConsumingEnumerables {
    #region public

    /// <summary>
    /// Consuming enumeration
    /// </summary>
    public static IEnumerable<T> Consuming<T>(this Queue<T> value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      while (value.Count > 0)
        yield return value.Dequeue();
    }

    /// <summary>
    /// Consuming enumeration
    /// </summary>
    public static IEnumerable<T> Consuming<T>(this Stack<T> value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      while (value.Count > 0)
        yield return value.Pop();
    }

    #endregion public
  }
}
