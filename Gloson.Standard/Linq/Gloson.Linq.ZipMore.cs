using System;
using System.Collections.Generic;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Zip extended version
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Zip
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="left">Left source</param>
    /// <param name="right">Right source</param>
    /// <param name="map">Mapping function: (left, leftExists, right, rightExists) - result</param>
    /// <returns></returns>
    public static IEnumerable<TResult> ZipMore<TLeft, TRight, TResult>(
      this IEnumerable<TLeft> left,
           IEnumerable<TRight> right,
           Func<TLeft, bool, TRight, bool, TResult> map) {

      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));
      else if (null == map)
        throw new ArgumentNullException(nameof(map));

      using var enFirst = left.GetEnumerator();
      using var enSecond = right.GetEnumerator();

      while (enFirst.MoveNext())
        if (enSecond.MoveNext())
          yield return map(enFirst.Current, true, enSecond.Current, true);
        else
          yield return map(enFirst.Current, true, default, false);

      while (enSecond.MoveNext())
        yield return map(default, false, enSecond.Current, true);
    }

    #endregion Public
  }
}
