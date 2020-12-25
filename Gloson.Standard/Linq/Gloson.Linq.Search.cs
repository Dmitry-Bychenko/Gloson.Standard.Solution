using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Search 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Binary Search At
    /// </summary>
    /// <param name="source">Source (must be sorted by map(item))</param>
    /// <param name="value">Value to find</param>
    /// <param name="map">Map function</param>
    /// <param name="comparer">Comparer</param>
    /// <returns>(left, right) range</returns>
    public static (int left, int right) BinarySearchAt<T, V>(
      this IEnumerable<T> source,
           V value,
           Func<T, V> map = null,
           IComparer<V> comparer = null) {

      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (map is null) {
        if (typeof(V).IsAssignableFrom(typeof(T)))
          map = (t) => (V)Convert.ChangeType(t, typeof(V));
        else if (TypeDescriptor.GetConverter(typeof(T)).CanConvertTo(typeof(V)))
          map = (t) => (V)Convert.ChangeType(t, typeof(V));

        if (map is null)
          throw new ArgumentNullException(nameof(map));
      }

      if (comparer is null) {
        comparer = Comparer<V>.Default;

        if (comparer is null)
          throw new ArgumentNullException(nameof(comparer));
      }

      IEnumerable<T> data =
          source.GetType().IsArray ? source
        : source is IList<T> ? source
        : source is IReadOnlyList<T> ? source
        : source.ToList();

      if (!data.Any())
        throw new ArgumentException("source must not be empty", nameof(source));

      V leftValue = map(data.First());
      V rightValue = map(data.Last());

      int sign = comparer.Compare(leftValue, rightValue);

      if (0 == sign) {
        int d = comparer.Compare(leftValue, value);

        if (d < 0)
          return (-1, 0);
        else if (d > 0)
          return (data.Count() - 1, data.Count());
        else
          return (0, 0);
      }

      int dLeft = comparer.Compare(value, leftValue);
      int dRight = comparer.Compare(value, rightValue);

      int leftIndex = 0;
      int rightIndex = data.Count() - 1;

      if (dLeft < 0 && dRight < 0)
        if (sign < 0)
          return (-1, 0);
        else
          return (data.Count() - 1, data.Count());
      else if (dLeft > 0 && dRight > 0)
        if (sign < 0)
          return (data.Count() - 1, data.Count());
        else
          return (-1, 0);
      else if (dLeft == 0)
        return (0, 0);
      else if (dRight == 0)
        return (data.Count() - 1, data.Count() - 1);

      while (true) {
        int middleIndex = (leftIndex / 2 + rightIndex / 2) + (leftIndex % 2 + rightIndex % 2) / 2;
        int dMiddle = comparer.Compare(value, map(data.ElementAt(middleIndex)));

        if (dMiddle == 0)
          return (middleIndex, middleIndex);

        if (dMiddle > 0 && dLeft > 0 || dMiddle < 0 && dLeft < 0)
          leftIndex = middleIndex;
        else
          rightIndex = middleIndex;

        if (rightIndex - leftIndex == 1)
          return (leftIndex, rightIndex);
      }
    }

    #endregion Public
  }

}
