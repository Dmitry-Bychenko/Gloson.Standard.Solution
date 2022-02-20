using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Pareto Front
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ParetoExtensions {
    #region Public

    /// <summary>
    /// Pareto Front for X and Y variables
    /// </summary>
    /// <param name="source">Data Source</param>
    /// <param name="mapY">Y variable</param>
    /// <param name="mapX">X variable</param>
    /// <param name="ascendingY">If Y ascending</param>
    /// <param name="ascendingX">If X ascending</param>
    /// <param name="comparerY">Y variable comparer</param>
    /// <param name="comparerX">X variable comparer</param>
    /// <returns>Pareto Front for X and Y</returns>
    /// <exception cref="ArgumentNullException">When source, mapX or mapY is null</exception>
    public static IEnumerable<T> ParetoFront<T, Y, X>(this IEnumerable<T> source,
                                                           Func<T, Y> mapY,
                                                           Func<T, X> mapX,
                                                           bool ascendingY = true,
                                                           bool ascendingX = true,
                                                           IComparer<Y>? comparerY = null,
                                                           IComparer<X>? comparerX = null) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (mapY is null)
        throw new ArgumentNullException(nameof(mapY));
      if (mapX is null)
        throw new ArgumentNullException(nameof(mapX));

      comparerY = comparerY ?? Comparer<Y>.Default ?? throw new ArgumentNullException(nameof(comparerY));
      comparerX = comparerX ?? Comparer<X>.Default ?? throw new ArgumentNullException(nameof(comparerX));

      IEnumerable<T> data = ascendingX
        ? ascendingY
            ? source.OrderBy(item => mapX(item), comparerX).ThenByDescending(item => mapY(item), comparerY)
            : source.OrderBy(item => mapX(item), comparerX).ThenBy(item => mapY(item), comparerY)
        : ascendingY
            ? source.OrderByDescending(item => mapX(item), comparerX).ThenByDescending(item => mapY(item), comparerY)
            : source.OrderByDescending(item => mapX(item), comparerX).ThenBy(item => mapY(item), comparerY);

      bool first = true;

      Y? lastY = default;
      X? lastX = default;

      foreach (var item in data) {
        if (first) {
          first = false;

          yield return item;

          lastY = mapY(item);
          lastX = mapX(item);

          continue;
        }

        X x = mapX(item);

        if (comparerX.Compare(lastX, x) == 0)
          continue;

        Y y = mapY(item);

        int compare = comparerY.Compare(lastY, y);

        if (compare < 0 && ascendingY || compare > 0 && !ascendingY) {
          lastX = x;
          lastY = y;

          yield return item;
        }
      }
    }

    /// <summary>
    /// Pareto Front for X and Y variables
    /// </summary>
    /// <param name="source">Data Source</param>
    /// <param name="mapY">Y variable</param>
    /// <param name="mapX">X variable</param>
    /// <param name="ascendingY">If Y ascending</param>
    /// <param name="ascendingX">If X ascending</param>
    /// <returns>Pareto Front for X and Y</returns>
    /// <exception cref="ArgumentNullException">When source, mapX or mapY is null</exception>
    public static IEnumerable<T> ParetoFront<T>(this IEnumerable<T> source,
                                                     Func<T, double> mapY,
                                                     Func<T, double> mapX,
                                                     bool ascendingY = true,
                                                     bool ascendingX = true) =>
      ParetoFront<T, double, double>(source, mapY, mapX, ascendingY, ascendingX, null, null);

    #endregion Public 
  }

}
