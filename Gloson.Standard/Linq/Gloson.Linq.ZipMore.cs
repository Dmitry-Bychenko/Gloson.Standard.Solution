using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

      if (Object.ReferenceEquals(null, left))
        throw new ArgumentNullException("first");
      else if (Object.ReferenceEquals(null, right))
        throw new ArgumentNullException("second");
      else if (Object.ReferenceEquals(null, map))
        throw new ArgumentNullException("map");

      using (var enFirst = left.GetEnumerator()) {
        using (var enSecond = right.GetEnumerator()) {
          while (enFirst.MoveNext())
            if (enSecond.MoveNext())
              yield return map(enFirst.Current, true, enSecond.Current, true);
            else
              yield return map(enFirst.Current, true, default(TRight), false);

          while (enSecond.MoveNext())
            yield return map(default(TLeft), false, enSecond.Current, true);
        }
      }
    }

    #endregion Public
  }
}
