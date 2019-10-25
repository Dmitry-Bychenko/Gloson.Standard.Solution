using System;
using System.Collections.Generic;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Cartesian join
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public static IEnumerable<Result> CartesianJoin<Result, Left, Right>(
      IEnumerable<Left> left,
      IEnumerable<Right> right, 
      Func<Left, Right, Result> map) {

      if (null == left)
        throw new ArgumentNullException(nameof(left));
      else if (null == right)
        throw new ArgumentNullException(nameof(right));
      else if (null == map)
        throw new ArgumentNullException(nameof(map));
      
      if (right is ICollection<Right>) {
        // right is a materialized collection

        foreach (Left leftItem in left)
          foreach (Right rightItem in right)
            yield return map(leftItem, rightItem);

        yield break;
      }
      else if (left is ICollection<Left>) {
        // left is a materialized collection

        foreach (Right rightItem in right)
          foreach (Left leftItem in left)
            yield return map(leftItem, rightItem);

        yield break;
      }

      IEnumerable<Right> source = right;

      List<Right> list = new List<Right>();

      foreach (Left leftItem in left) {
        foreach (Right rightItem in source) {
          if (source != list)
            list.Add(rightItem);

          yield return map(leftItem, rightItem);
        }

        source = list;
      }
    }

    #endregion Public
  }

}
